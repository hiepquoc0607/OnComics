using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnComics.Application.Enums.InteractionType;
using OnComics.Application.Models.Request.InteractionType;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.InteractionType;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class InteractionTypeService : IInteractionTypeService
    {
        private readonly IInteractionTypeRepository _interactionTypeRepository;
        private readonly IAppwriteService _appwriteService;
        private readonly IRedisService _redisService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly Util _util;

        private static string cacheKey = "itrtypes";

        public InteractionTypeService(
            IInteractionTypeRepository interactionTypeRepository,
            IAppwriteService appwriteService,
            IRedisService redisService,
            IFileService fileService,
            IMapper mapper,
            Util util)
        {
            _interactionTypeRepository = interactionTypeRepository;
            _appwriteService = appwriteService;
            _redisService = redisService;
            _fileService = fileService;
            _mapper = mapper;
            _util = util;
        }

        //Get All Interaction Types
        public async Task<ObjectResponse<IEnumerable<InteractionTypeRes>?>> GetItrTypesAsync(GetItrTypeReq getItrTypeReq)
        {
            try
            {
                string? searchKey = getItrTypeReq.SearchKey;

                bool isDescending = getItrTypeReq.IsDescending;

                int pageNum = getItrTypeReq.PageNum;
                int pageIndex = getItrTypeReq.PageIndex;

                var typeCache = await _redisService
                    .GetAsync<IEnumerable<InteractionTypeRes>?>(cacheKey);

                if (typeCache is not null)
                {
                    var query = typeCache.AsQueryable();

                    if (!string.IsNullOrEmpty(searchKey))
                        query = query.Where(i => EF.Functions.Like(i.Name, $"%{searchKey}%"));

                    query = getItrTypeReq.SortBy switch
                    {
                        ItrTypeSortOption.NAME => isDescending
                            ? query.OrderByDescending(i => i.Name)
                            : query.OrderBy(i => i.Name),
                        _ => query.OrderBy(i => i.Id)
                    };

                    var categories = await query
                        .Skip((pageNum - 1) * pageIndex)
                        .Take(pageIndex)
                        .ToListAsync();

                    var totalData = typeCache.Count();
                    int totalPage = (int)Math.Ceiling((decimal)totalData / pageIndex);
                    var data = categories.Adapt<IEnumerable<InteractionTypeRes>>();

                    var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

                    return new ObjectResponse<IEnumerable<InteractionTypeRes>?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        data,
                        pagination);
                }
                else
                {
                    Expression<Func<Interactiontype, bool>>? search = i =>
                        (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(i.Name, $"%{searchKey}%"));

                    Func<IQueryable<Interactiontype>, IOrderedQueryable<Interactiontype>>? order = i => getItrTypeReq.SortBy switch
                    {
                        ItrTypeSortOption.NAME => isDescending
                            ? i.OrderByDescending(i => i.Name)
                            : i.OrderBy(i => i.Name),
                        _ => i.OrderBy(i => i.Id)
                    };

                    var types = await _interactionTypeRepository.GetAsync(search, order, pageNum, pageIndex);

                    if (types == null)
                        return new ObjectResponse<IEnumerable<InteractionTypeRes>?>(
                            (int)HttpStatusCode.NotFound,
                            "Interaction Type Data Empty!");

                    var data = types.Adapt<IEnumerable<InteractionTypeRes>>();

                    var totalData = await _interactionTypeRepository.CountRecordAsync(search);
                    var toatlPage = (int)Math.Ceiling((decimal)totalData / getItrTypeReq.PageIndex);
                    var pagination = new Pagination(totalData, pageIndex, pageNum, toatlPage);

                    var itrTypes = await _interactionTypeRepository.GetInteractiontypesAsync();

                    if (itrTypes is not null)
                    {
                        var cache = itrTypes.Adapt<IEnumerable<InteractionTypeRes>>();

                        await _redisService
                            .SetAsync<IEnumerable<InteractionTypeRes>?>(cacheKey, cache, TimeSpan.FromDays(1));
                    }

                    return new ObjectResponse<IEnumerable<InteractionTypeRes>?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        data,
                        pagination);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<InteractionTypeRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Get Interaction Type By Id
        public async Task<ObjectResponse<InteractionTypeRes?>> GetItrTypeByIdAsync(Guid id)
        {
            try
            {
                string key = cacheKey + $":{id}";

                var typeCache = await _redisService.GetAsync<InteractionTypeRes?>(key);

                if (typeCache is not null)
                {
                    return new ObjectResponse<InteractionTypeRes?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        typeCache);
                }
                else
                {
                    var type = await _interactionTypeRepository.GetByIdAsync(id, false);

                    if (type == null)
                        return new ObjectResponse<InteractionTypeRes?>(
                            (int)HttpStatusCode.NotFound,
                            "Interaction Type Not Found!");

                    var data = _mapper.Map<InteractionTypeRes>(type);

                    await _redisService.SetAsync<InteractionTypeRes?>(key, data, TimeSpan.FromMinutes(10));

                    return new ObjectResponse<InteractionTypeRes?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        data);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResponse<InteractionTypeRes?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Create Interaction Type
        public async Task<ObjectResponse<Interactiontype>> CreateItrTypeAsync(CreateItrTypeReq createItrTypeReq)
        {
            Guid id = Guid.NewGuid();

            try
            {
                var file = createItrTypeReq.File;

                if (file == null || file.Length == 0)
                    return new ObjectResponse<Interactiontype>(
                        (int)HttpStatusCode.BadRequest,
                        "No File Uploaded!");

                if (!file.ContentType.Contains("image"))
                    return new ObjectResponse<Interactiontype>(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid Picture File Format!");

                string name = _util.FormatStringName(createItrTypeReq.Name);

                var isExisted = await _interactionTypeRepository
                    .CheckTypeNameExistedAsync(name);

                if (isExisted)
                    return new ObjectResponse<Interactiontype>(
                        (int)HttpStatusCode.BadRequest,
                        "Interaction Type Is Existed!");

                var fileRes = await _appwriteService
                    .CreateEmoteFileAsync(file, id.ToString());

                var newType = _mapper.Map<Interactiontype>(createItrTypeReq);
                newType.Id = id;
                newType.Name = name;
                newType.ImgUrl = fileRes.Url;

                await _interactionTypeRepository.InsertAsync(newType);

                return new ObjectResponse<Interactiontype>(
                    (int)HttpStatusCode.Created,
                    "Create Interaction Type Successfully!", newType);
            }
            catch (Exception ex)
            {
                var upFile = await _appwriteService.GetFileAsync(id.ToString());

                if (upFile != null)
                    await _appwriteService.DeleteFileAsync(id.ToString());

                return new ObjectResponse<Interactiontype>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Interaction Type
        public async Task<VoidResponse> UpdateItrTypeAsync(Guid id, UpdateItrTypeReq updateItrTypeReq)
        {
            try
            {
                string name = _util.FormatStringName(updateItrTypeReq.Name);

                var isExisted = await _interactionTypeRepository.CheckTypeNameExistedAsync(name);

                if (isExisted)
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Interaction Type Is Existed!");

                var oldType = await _interactionTypeRepository.GetByIdAsync(id, true);

                if (oldType == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Type Not Found!");

                var newType = _mapper.Map(updateItrTypeReq, oldType);
                newType.Name = name;

                await _interactionTypeRepository.UpdateAsync(newType);

                string key = cacheKey + $":{id}";

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Interaction Type Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Interaction Type Picture
        public async Task<VoidResponse> UpdateItrTypeImgAsync(Guid id, IFormFile file)
        {
            IFormFile formFile = null!;

            try
            {
                if (file == null || file.Length == 0)
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "No File Uploaded!");

                if (!file.ContentType.Contains("image"))
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid Picture File Format!");

                var oldType = await _interactionTypeRepository.GetByIdAsync(id, true);

                if (oldType == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Type Not Found!");

                var fileBytes = await _appwriteService.GetFileDownloadAsync(id.ToString());

                if (fileBytes.IsNullOrEmpty() || fileBytes.Length == 0)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Source File Not Found!");

                formFile = await _fileService.ConvertIFormFileAsync(fileBytes, id.ToString());

                await _appwriteService.DeleteFileAsync(id.ToString());

                var fileRes = await _appwriteService.CreateEmoteFileAsync(file, id.ToString());

                oldType.ImgUrl = fileRes.Url;

                await _interactionTypeRepository.UpdateAsync(oldType);

                string key = cacheKey + $":{id}";

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Interaction Type Image Successfully!");
            }
            catch (Exception ex)
            {
                var img = await _appwriteService.GetFileAsync(id.ToString());

                if (img == null)
                    await _appwriteService.CreateEmoteFileAsync(formFile, id.ToString());

                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Delete Interaction Type
        public async Task<VoidResponse> DeleteItrTypeAsync(Guid id)
        {
            try
            {
                var type = await _interactionTypeRepository.GetByIdAsync(id, true);

                if (type == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Type Not Found!");

                await _interactionTypeRepository.DeleteAsync(type);

                await _appwriteService.DeleteFileAsync(id.ToString());

                string key = cacheKey + $":{id}";

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Interaction Type Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }
    }
}
