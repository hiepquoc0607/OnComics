using Mapster;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Chapter;
using OnComics.Library.Models.Response.Chapter;
using OnComics.Library.Utils.Constants;

namespace OnComics.Library.Utils.Mapper
{
    public class ChapterMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Chapter, ChapterRes>();

            config.NewConfig<CreateChapterReq, Chapter>()
                .Map(dest => dest.ReadNum, src => 0)
                .Map(dest => dest.ReleaseTime, src => DateTime.UtcNow)
                .Map(dest => dest.Status, src => StatusConstant.ACTIVE);

            config.NewConfig<UpdateChapterReq, ChapterRes>();
        }
    }
}
