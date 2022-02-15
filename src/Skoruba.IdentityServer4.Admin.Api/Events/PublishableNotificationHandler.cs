using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Skoruba.IdentityServer4.Admin.Api.Notifications;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Api.Events
{
    public class PublishableNotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : PublishableNotification
    {
        public Task Handle(TNotification notification, CancellationToken cancellationToken)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "identityserver4_admin", type: ExchangeType.Direct);
                var body = notification.EventBody();
                channel.BasicPublish(exchange: "identityserver4_admin",
                                     routingKey: notification.EventType,
                                     basicProperties: null,
                                     body: body);

                return Task.CompletedTask;
            }
        }
    }
}