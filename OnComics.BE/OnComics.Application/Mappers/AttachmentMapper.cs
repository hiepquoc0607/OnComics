using Mapster;
using OnComics.Application.Models.Response.Attachment;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class AttachmentMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Attachment, AttachmentRes>();
        }
    }
}
