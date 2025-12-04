using Mapster;
using OnComics.Application.Models.Response.Notification;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class NotificationMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Notification, NotificationRes>();
        }
    }
}
