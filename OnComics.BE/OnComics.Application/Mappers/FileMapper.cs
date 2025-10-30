using Mapster;
using OnComics.Application.Models.Response.Appwrite;
using File = Appwrite.Models.File;

namespace OnComics.Application.Mappers
{
    public class FileMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<File, FileRes>();
        }
    }
}
