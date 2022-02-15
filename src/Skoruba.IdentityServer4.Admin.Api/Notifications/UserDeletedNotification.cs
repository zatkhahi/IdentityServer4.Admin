using MediatR;

namespace Skoruba.IdentityServer4.Admin.Api.Notifications
{
    public class UserDeletedNotification<TUserDto> : PublishableNotification
    {
        public UserDeletedNotification(TUserDto user)
        {
            Data = user;
            EventType = "user_deleted";
        }
    }
}







