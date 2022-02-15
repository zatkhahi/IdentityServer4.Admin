using MediatR;
using Newtonsoft.Json;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.Api.Notifications
{
    public abstract class PublishableNotification : INotification
    {
        public string EventType { get; protected set; }
        public object Data { get; protected set; }

        public virtual byte[] EventBody()
        {
            var message = JsonConvert.SerializeObject(Data, Formatting.None);
            var body = Encoding.UTF8.GetBytes(message);
            return body;
        }
    }
}
