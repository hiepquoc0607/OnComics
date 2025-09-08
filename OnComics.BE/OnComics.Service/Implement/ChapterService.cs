using Mapster;
using MapsterMapper;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Chapter;
using OnComics.Library.Models.Request.General;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Models.Response.Chapter;
using OnComics.Library.Utils.Constants;
using OnComics.Library.Utils.Utils;
using OnComics.Repository.Interface;
using OnComics.Service.Interface;

namespace OnComics.Service.Implement
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IComicRepository _comicRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public ChapterService(
            IChapterRepository chapterRepository,
            IComicRepository comicRepository,
            IMapper mapper,
            Util util)
        {
            _chapterRepository = chapterRepository;
            _comicRepository = comicRepository;
            _mapper = mapper;
            _util = util;
        }

        //Get All Chapters By Comic Id
        public async Task<ObjectResponse<IEnumerable<ChapterRes>?>> GetChaptersByComicIdAsync(GetChapterReq getChapterReq)
        {
            var isComicExisted = await _comicRepository.CheckComicIdAsync(getChapterReq.ComicId);

            if (!isComicExisted)
                return new ObjectResponse<IEnumerable<ChapterRes>?>("Error", 404, "Comic Not Found!");

            var (chapters, pagination) = await _chapterRepository.GetChaptersAsync(getChapterReq);

            if (chapters == null)
                return new ObjectResponse<IEnumerable<ChapterRes>?>("Error", 404, "Comic Has No Chapter Data!");

            var data = _mapper.Map<IEnumerable<ChapterRes>>(chapters);

            return new ObjectResponse<IEnumerable<ChapterRes>?>("Success", 200, "Fetch Data Successfully!", data, pagination);
        }

        //Get Chapter By Id
        public async Task<ObjectResponse<ChapterRes?>> GetChapterByIdAsync(int id)
        {
            var chapter = await _chapterRepository.GetChapterByIdAsync(id, false);

            if (chapter == null) return new ObjectResponse<ChapterRes?>("Error", 404, "Chapter Not Found!");

            var data = _mapper.Map<ChapterRes>(chapter);

            return new ObjectResponse<ChapterRes?>("Success", 400, "Fetch Data Successfully!", data);
        }

        //Create Chapter
        public async Task<ObjectResponse<Chapter>> CreateChapterAsync(CreateChapterReq createChapterReq)
        {
            var isComicExist = await _comicRepository.CheckComicIdAsync(createChapterReq.ComicId);

            if (!isComicExist) return new ObjectResponse<Chapter>("Error", 404, "Comic Not Found!");

            var newChapter = _mapper.Map<Chapter>(createChapterReq);
            newChapter.ChapNo = await _chapterRepository.GetMaxChapNoByComicIdAsync(createChapterReq.ComicId) + 1;

            if (!string.IsNullOrEmpty(newChapter.Name))
                newChapter.Name = _util.FormatStringName(newChapter.Name);

            try
            {
                await _chapterRepository.CreateChapterAsync(newChapter);

                return new ObjectResponse<Chapter>("Success", 200, "Create Chapter Successfully!", newChapter);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Chapter>("Error", 400, "Create Chapter Fail!, Message Error:\n\n" + ex);
            }

            throw new NotImplementedException();
        }

        //Bulk (Range) Create Chapters
        public async Task<ObjectResponse<IEnumerable<Chapter>>> CreateChaptersAsync(List<CreateChapterReq> chapters)
        {
            if (chapters.Count > 10)
                return new ObjectResponse<IEnumerable<Chapter>>("Error", 400, "Only Create Max 10 Record At Once!");

            int[] ids = chapters.Select(c => c.ComicId).ToArray();
            int[] dataIds = await _comicRepository.GetComicIdsAsync();
            int[] nonIds = _util.CompareIntArray(ids, dataIds);

            if (nonIds.Length > 0)
                return new ObjectResponse<IEnumerable<Chapter>>("Error", 404, "Comics Not Found!, IDs: " + string.Join(", ", nonIds));

            var chapNos = await _chapterRepository.GetMaxChapNosByComicIdsAsync(ids);

            var newChapters = chapters.Adapt<IEnumerable<Chapter>>();

            foreach (var items in newChapters)
            {
                if (!string.IsNullOrEmpty(items.Name))
                    items.Name = _util.FormatStringName(items.Name);

                items.ChapNo = chapNos[items.ComicId] + 1;
            }

            try
            {
                await _chapterRepository.CreateChaptersAsync(newChapters);

                return new ObjectResponse<IEnumerable<Chapter>>("Success", 200, "Create Chapters Successfully!", newChapters);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<Chapter>>("Error", 400, "Create Chapters Fail!, Error Message:\n\n" + ex);
            }
        }

        //Update Chapter
        public async Task<VoidResponse> UpdateChapterAsync(int id, UpdateChapterReq updateChapterReq)
        {
            var oldChapter = await _chapterRepository.GetChapterByIdAsync(id, true);

            if (oldChapter == null) return new VoidResponse("Error", 404, "Chapter Not Found!");

            var newChapter = _mapper.Map(updateChapterReq, oldChapter);

            if (!string.IsNullOrEmpty(newChapter.Name))
                newChapter.Name = _util.FormatStringName(newChapter.Name);

            try
            {
                await _chapterRepository.UpdateChapterAsync(newChapter);

                return new VoidResponse("Success", 200, "Update Chapter Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Chapter Fail!, Error Message:\n\n" + ex);
            }

            throw new NotImplementedException();
        }

        //Update Chapter Status
        public async Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<ChapStatus> updateStatusReq)
        {
            var chapter = await _chapterRepository.GetChapterByIdAsync(id, true);

            if (chapter == null) return new VoidResponse("Error", 404, "Chapter Not Found!");

            chapter.Status = updateStatusReq.Status switch
            {
                ChapStatus.ACTIVE => StatusConstant.ACTIVE,
                ChapStatus.INACTIVE => StatusConstant.INACTIVE,
                _ => StatusConstant.ERROR
            };

            try
            {
                await _chapterRepository.UpdateChapterAsync(chapter);

                return new VoidResponse("Success", 200, "Update Status Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Status Fail!, Message Error:\n\n" + ex);
            }
        }

        //Delete Chapter
        public async Task<VoidResponse> DeleteChapterAsync(int id)
        {
            var chapter = await _chapterRepository.GetChapterByIdAsync(id, true);

            if (chapter == null) return new VoidResponse("Error", 404, "Chapter Not Found!");

            try
            {
                await _chapterRepository.DeleteChapterAsync(id);

                return new VoidResponse("Successs", 200, "Delete Chapter Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Delete Chapter Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}