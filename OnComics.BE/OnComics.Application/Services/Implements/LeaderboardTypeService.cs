using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Constants;
using OnComics.Application.Enums.LeaderboardType;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Request.LeaderboardType;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.LeaderboardType;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnComics.Application.Services.Implements
{
    public class LeaderboardTypeService : ILeaderboardTypeService
    {
        private readonly ILeaderboardTypeRepository _leaderboardTypeRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public LeaderboardTypeService(
            ILeaderboardTypeRepository leaderboardTypeRepository,
            IMapper mapper,
            Util util)
        {
            _leaderboardTypeRepository = leaderboardTypeRepository;
            _mapper = mapper;
            _util = util;
        }

        //Get All Leaderboard Types
        public async Task<ObjectResponse<IEnumerable<LeaderboardTypeRes>?>> GetLdbTypesAsync(GetLdbTypeReq getLdbTypeReq)
        {
            string? searchKey = getLdbTypeReq.SearchKey;

            string? status = getLdbTypeReq.Status switch
            {
                LdbTypeStatus.ACTIVE => StatusConstant.ACTIVE,
                LdbTypeStatus.INACTIVE => StatusConstant.INACTIVE,
                _ => null
            };

            bool isDescending = getLdbTypeReq.IsDescending;

            int pageNum = getLdbTypeReq.PageNum;
            int pageIndex = getLdbTypeReq.PageIndex;

            Expression<Func<Leaderboardtype, bool>>? search = t =>
                (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(t.Name, $"%{searchKey}%")) &&
                (string.IsNullOrEmpty(status) || t.Status.Equals(status));

            Func<IQueryable<Leaderboardtype>, IOrderedQueryable<Leaderboardtype>>? order = t => getLdbTypeReq.SortBy switch
            {
                LdbTypeSortOption.NAME => isDescending
                    ? t.OrderByDescending(t => t.Name)
                    : t.OrderBy(t => t.Name),
                LdbTypeSortOption.STATUS => isDescending
                    ? t.OrderByDescending(t => t.Status)
                    : t.OrderBy(t => t.Status),
                _ => t.OrderBy(t => t.Id)
            };

            var types = await _leaderboardTypeRepository.GetAsync(search, order, pageNum, pageIndex);

            if (types == null)
                return new ObjectResponse<IEnumerable<LeaderboardTypeRes>?>("Error", 404, "Leaderboard Type Data Empty!");

            var data = types.Adapt<IEnumerable<LeaderboardTypeRes>>();

            var totalData = await _leaderboardTypeRepository.CountRecordAsync();
            var toatlPage = (int)Math.Ceiling((decimal)totalData / getLdbTypeReq.PageIndex);
            var pagination = new Pagination(totalData, pageIndex, pageNum, toatlPage);

            return new ObjectResponse<IEnumerable<LeaderboardTypeRes>?>("Success", 200, "Fetch Data Successfully!", data, pagination);
        }

        //Get Leaderboard Type By Id
        public async Task<ObjectResponse<LeaderboardTypeRes?>> GetLbdTypeByIdAsync(int id)
        {
            var type = await _leaderboardTypeRepository.GetByIdAsync(id);

            if (type == null) return new ObjectResponse<LeaderboardTypeRes?>("Error", 404, "Leaderboard Type Not Found!");

            var data = _mapper.Map<LeaderboardTypeRes>(type);

            return new ObjectResponse<LeaderboardTypeRes?>("Success", 200, "Data Fetch Successfully!", data);
        }

        //Create Leaderboard Type
        public async Task<ObjectResponse<Leaderboardtype>> CreateLdbTypeAsync(CreateLdbTypeReq createLdbTypeReq)
        {
            var typeName = _util.FormatStringName(createLdbTypeReq.Name);

            var isExisted = await _leaderboardTypeRepository.CheckTypeNameExistedAsync(typeName);

            if (isExisted) return new ObjectResponse<Leaderboardtype>("Error", 400, "Leaderboard Type Is Existed!");

            var newType = _mapper.Map<Leaderboardtype>(createLdbTypeReq);
            newType.Name = typeName;

            try
            {
                await _leaderboardTypeRepository.InsertAsync(newType);

                return new ObjectResponse<Leaderboardtype>("Success", 201, "Create Leaderboard Tyype Successfully!", newType);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Leaderboardtype>("Error", 400, "Create Leaderboaard Type Fail!, Error Message:\n\n" + ex);
            }
        }

        //Update Leaderboard Type
        public async Task<VoidResponse> UpdateLdbTypeAsync(int id, UpdateLdbTypeReq updateLdbTypeReq)
        {
            var typeName = _util.FormatStringName(updateLdbTypeReq.Name);

            var isExisted = await _leaderboardTypeRepository.CheckTypeNameExistedAsync(typeName);

            if (isExisted) return new VoidResponse("Error", 400, "Leaderboard Type Is Existed!");

            var oldType = await _leaderboardTypeRepository.GetByIdAsync(id, true);

            if (oldType == null) return new VoidResponse("Error", 404, "Leaderboard Type Not Found!");

            var newType = _mapper.Map(updateLdbTypeReq, oldType);
            newType.Name = typeName;

            try
            {
                await _leaderboardTypeRepository.UpdateAsync(newType);

                return new VoidResponse("Success", 200, "Update Leaderboard Type Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Leaderboard Type Fail!, Error Message:\n\n" + ex);
            }
        }

        //Update Leaderboard Type Status
        public async Task<VoidResponse> UpdateLdbTypeStatusAsync(int id, UpdateStatusReq<LdbTypeStatus> updateStatusReq)
        {
            var status = updateStatusReq.Status switch
            {
                LdbTypeStatus.ACTIVE => StatusConstant.ACTIVE,
                _ => StatusConstant.INACTIVE,
            };

            var type = await _leaderboardTypeRepository.GetByIdAsync(id, true);

            if (type == null) return new VoidResponse("Error", 404, "Leaderboard Type Not Found!");

            type.Status = status;

            try
            {
                await _leaderboardTypeRepository.UpdateAsync(type);

                return new VoidResponse("Success", 200, "Update Status Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Status Fail!, Error Message:\n\n" + ex);
            }
        }

        //Delete Leaderboard Type
        public async Task<VoidResponse> DeleteLbdTypeAsync(int id)
        {
            var type = await _leaderboardTypeRepository.GetByIdAsync(id);

            if (type == null) return new VoidResponse("Error", 404, "Leaderboard Type Not Found!");

            try
            {
                await _leaderboardTypeRepository.DeleteAsync(id);

                return new VoidResponse("Success", 200, "Delete Leaderboard Type Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Delete Leaderboard Type Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}
