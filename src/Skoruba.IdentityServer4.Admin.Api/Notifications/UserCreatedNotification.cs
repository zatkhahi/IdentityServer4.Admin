using MediatR;

namespace Skoruba.IdentityServer4.Admin.Api.Notifications
{
    public class UserCreatedNotification<TUserDto> : PublishableNotification
    {
        public UserCreatedNotification(TUserDto user)
        {
            Data = user;
            EventType = "user_created";
        }
    }
}







