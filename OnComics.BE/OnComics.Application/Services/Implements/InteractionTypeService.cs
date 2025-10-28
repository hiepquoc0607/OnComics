using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Constants;
using OnComics.Application.Enums.InteractionType;
using OnComics.Application.Models.Request.General;
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
        private readonly IMapper _mapper;
        private readonly Util _util;

        public InteractionTypeService(
            IInteractionTypeRepository interactionTypeRepository,
            IMapper mapper,
            Util util)
        {
            _interactionTypeRepository = interactionTypeRepository;
            _mapper = mapper;
            _util = util;
        }

        //Get All Interaction Types
        public async Task<ObjectResponse<IEnumerable<InteractionTypeRes>?>> GetItrTypesAsync(GetItrTypeReq getItrTypeReq)
        {
            try
            {
                string? searchKey = getItrTypeReq.SearchKey;

                string? status = getItrTypeReq.Status switch
                {
                    ItrTypeStatus.ACTIVE => StatusConstant.ACTIVE,
                    ItrTypeStatus.INACTIVE => StatusConstant.INACTIVE,
                    _ => null
                };

                bool isDescending = getItrTypeReq.IsDescending;

                int pageNum = getItrTypeReq.PageNum;
                int pageIndex = getItrTypeReq.PageIndex;

                Expression<Func<Interactiontype, bool>>? search = i =>
                    (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(i.Name, $"%{searchKey}%")) &&
                    (string.IsNullOrEmpty(status) || i.Status.Equals(status));

                Func<IQueryable<Interactiontype>, IOrderedQueryable<Interactiontype>>? order = i => getItrTypeReq.SortBy switch
                {
                    ItrTypeSortOption.NAME => isDescending
                        ? i.OrderByDescending(i => i.Name)
                        : i.OrderBy(i => i.Name),
                    ItrTypeSortOption.STATUS => isDescending
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

                var totalData = await _interactionTypeRepository.CountRecordAsync();
                var toatlPage = (int)Math.Ceiling((decimal)totalData / getItrTypeReq.PageIndex);
                var pagination = new Pagination(totalData, pageIndex, pageNum, toatlPage);

                return new ObjectResponse<IEnumerable<InteractionTypeRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);
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
                var type = await _interactionTypeRepository.GetByIdAsync(id);

                if (type == null)
                    return new ObjectResponse<InteractionTypeRes?>(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Type Not Found!");

                var data = _mapper.Map<InteractionTypeRes>(type);

                return new ObjectResponse<InteractionTypeRes?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data);
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
            try
            {
                string name = _util.FormatStringName(createItrTypeReq.Name);

                var isExisted = await _interactionTypeRepository.CheckTypeNameExistedAsync(name);

                if (isExisted)
                    return new ObjectResponse<Interactiontype>(
                        (int)HttpStatusCode.BadRequest,
                        "Interaction Type Is Existed!");

                var newType = _mapper.Map<Interactiontype>(createItrTypeReq);
                newType.Name = name;

                await _interactionTypeRepository.InsertAsync(newType);

                return new ObjectResponse<Interactiontype>(
                    (int)HttpStatusCode.Created,
                    "Create Interaction Type Successfully!", newType);
            }
            catch (Exception ex)
            {
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
                        "Interaaction Type Is Existed!");

                var oldType = await _interactionTypeRepository.GetByIdAsync(id, true);

                if (oldType == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Type Not Found!");

                var newType = _mapper.Map(updateItrTypeReq, oldType);
                newType.Name = name;

                await _interactionTypeRepository.UpdateAsync(newType);

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

        //Update Interaction Type Status
        public async Task<VoidResponse> UpdateItrTypeStatusAsync(Guid id, UpdateStatusReq<ItrTypeStatus> updateStatusReq)
        {
            try
            {
                string status = updateStatusReq.Status switch
                {
                    ItrTypeStatus.ACTIVE => StatusConstant.ACTIVE,
                    _ => StatusConstant.INACTIVE,
                };

                var type = await _interactionTypeRepository.GetByIdAsync(id, true);

                if (type == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Type Not Found!");

                type.Status = status;

                await _interactionTypeRepository.UpdateAsync(type);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Status Successfully!");
            }
            catch (Exception ex)
            {
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
                var type = await _interactionTypeRepository.GetByIdAsync(id);

                if (type == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Type Not Found!");

                await _interactionTypeRepository.DeleteAsync(id);

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
