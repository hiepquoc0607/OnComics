using Mapster;
using MapsterMapper;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Comic;
using OnComics.Library.Models.Request.General;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Models.Response.Comic;
using OnComics.Library.Utils.Constants;
using OnComics.Library.Utils.Utils;
using OnComics.Repository.Interface;
using OnComics.Service.Interface;

namespace OnComics.Service.Implement
{
    public class ComicService : IComicService
    {
        private readonly IComicRepository _comicRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public ComicService(
            IComicRepository comicRepository,
            IChapterRepository chapterRepository,
            IMapper mapper,
            Util util)
        {
            _comicRepository = comicRepository;
            _chapterRepository = chapterRepository;
            _mapper = mapper;
            _util = util;
        }

        //Get All Comics
        public async Task<ObjectResponse<IEnumerable<ComicRes>?>> GetComicsAsync(GetComicReq getComicReq)
        {
            var (comics, pagination) = await _comicRepository.GetComicsAsync(getComicReq);

            if (comics == null) return new ObjectResponse<IEnumerable<ComicRes>?>("Error", 404, "Comic Data Empty!");

            var data = comics.Adapt<IEnumerable<ComicRes>>();

            return new ObjectResponse<IEnumerable<ComicRes>?>("Success", 200, "Fetch Data Successfully!", data, pagination);
        }

        //Get Comic By Id
        public async Task<ObjectResponse<ComicRes?>> GetComicByIdAsync(int id)
        {
            var comic = await _comicRepository.GetComicByIdNoTrackingAsync(id);

            if (comic == null) return new ObjectResponse<ComicRes?>("Error", 404, "Comic Not Found!");

            var data = comic.Adapt<ComicRes>();

            return new ObjectResponse<ComicRes?>("Error", 200, "Fetch Data Successfully!", data);
        }

        //Create Comic
        public async Task<ObjectResponse<ComicRes>> CreateComicAsync(CreateComicReq createComicReq)
        {
            var IsComicExisted = await _comicRepository.CheckComicExistedAsync(createComicReq.Name, createComicReq.Author);

            if (IsComicExisted) return new ObjectResponse<ComicRes>("Error", 400, "Comic Is Existed!");

            var newComic = _mapper.Map<Comic>(createComicReq);

            try
            {
                await _comicRepository.CreateComicAsync(newComic);

                var newCm = await _comicRepository.GetComicByIdNoTrackingAsync(newComic.Id);

                return new ObjectResponse<ComicRes>("Success", 200, "Create Comic Successfully!", newCm);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<ComicRes>("Error", 400, "Create Comic Fail!, Error Message:\n\n" + ex);
            }
        }

        //Update Comic
        public async Task<VoidResponse> UpdateComicAsync(int id, UpdateComicReq updateComicReq)
        {
            var oldComic = await _comicRepository.GetComicByIdAsync(id);

            if (oldComic == null) return new VoidResponse("Error", 404, "Comic Not Found!");

            var newComic = _mapper.Map(updateComicReq, oldComic);
            newComic.Name = _util.FormatStringName(updateComicReq.Name);

            try
            {
                await _comicRepository.UpdateComicAsync(newComic);

                return new VoidResponse("Success", 200, "Update Comic Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Comic Fail!, Error Message:\n\n" + ex);
            }

            throw new NotImplementedException();
        }

        //Update Comic Status
        public async Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<ComicStatus> updateStatusReq)
        {
            var comic = await _comicRepository.GetComicByIdAsync(id);

            if (comic == null) return new VoidResponse("Error", 404, "Comic Not Found!");

            comic.Status = updateStatusReq.Status switch
            {
                ComicStatus.ONGOING => StatusConstant.ONGOING,
                ComicStatus.UPCOMING => StatusConstant.UPCOMING,
                _ => StatusConstant.FINISHED
            };

            try
            {
                await _comicRepository.UpdateComicAsync(comic);

                return new VoidResponse("Success", 200, "Update Status Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Status Fail!, Error Message:\n\n" + ex);
            }
        }

        //Delete Comic
        public async Task<VoidResponse> DeleteComicAsync(int id)
        {
            var comic = await _comicRepository.GetComicByIdNoTrackingAsync(id);

            if (comic == null) return new VoidResponse("Error", 404, "Comic Not Found!");

            try
            {
                await _comicRepository.DeleteComicAsync(id);

                return new VoidResponse("Success", 200, "Delete Comic Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Delete Comic Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}