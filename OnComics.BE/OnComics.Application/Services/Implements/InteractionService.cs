using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Enums.Interaction;
using OnComics.Application.Models.Request.Interaction;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.Interaction;
using OnComics.Application.Services.Interfaces;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class InteractionService : IInteractionService
    {
        private readonly IInteractionRepository _interactionRepository;
        private readonly IMapper _mapper;

        public InteractionService(
            IInteractionRepository interactionRepository,
            IMapper mapper)
        {
            _interactionRepository = interactionRepository;
            _mapper = mapper;
        }

        //Get All Interactions
        public async Task<ObjectResponse<IEnumerable<InteractionRes>?>> GetInteractionsAsync(GetInteractionReq getInteractionReq)
        {
            try
            {
                string? searchKey = getInteractionReq.SearchKey;

                int pageNum = getInteractionReq.PageNum;
                int pageIndex = getInteractionReq.PageIndex;

                bool isDecending = getInteractionReq.IsDescending;

                int? searchId = getInteractionReq.AccountId;

                Expression<Func<Interaction, bool>>? search = null;

                int totalData = 0;

                if (searchId.HasValue)
                {
                    search = i =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(i.Comment.Account.Fullname, $"%{search}%") &&
                        i.AccountId == searchId);

                    totalData = await _interactionRepository.CountInteractionAsync(searchId);
                }
                else
                {
                    search = i =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(i.Account.Fullname, $"%{search}%") ||
                        EF.Functions.Like(i.Comment.Account.Fullname, $"%{search}%"));

                    totalData = await _interactionRepository.CountInteractionAsync();
                }

                Func<IQueryable<Interaction>, IOrderedQueryable<Interaction>>? order = r => getInteractionReq.SortBy switch
                {
                    InteractionSortOption.ACCOUNT => isDecending
                        ? r.OrderByDescending(r => r.Account.Fullname)
                        : r.OrderBy(r => r.Account.Fullname),
                    InteractionSortOption.TIME => isDecending
                        ? r.OrderByDescending(r => r.ReactTime)
                        : r.OrderBy(r => r.ReactTime),
                    _ => r.OrderBy(r => r.Id)
                };

                var (interactions, accounts, comments) = await _interactionRepository
                    .GetInteractionsAsync(search, order, pageNum, pageIndex);

                if (interactions == null)
                    return new ObjectResponse<IEnumerable<InteractionRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Data Empty!");

                var data = interactions.Select(i => new InteractionRes
                {
                    Id = i.Id,
                    AccountId = i.AccountId,
                    Fullname = accounts[i.AccountId],
                    CommentId = i.CommentId,
                    CommentAuthor = comments[i.CommentId],
                    TypeId = i.TypeId,
                    ReactTime = i.ReactTime
                });

                var toatlPage = (int)Math.Ceiling((decimal)totalData / pageIndex);
                var pagination = new Pagination(totalData, pageIndex, pageNum, toatlPage);

                return new ObjectResponse<IEnumerable<InteractionRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<InteractionRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Get Interaction By Id
        public async Task<ObjectResponse<InteractionRes?>> GetInteractionByIdAsync(int id)
        {
            try
            {
                var (interaction, fullname, author) = await _interactionRepository
                    .GetInteractionById(id);

                if (interaction == null)
                    return new ObjectResponse<InteractionRes?>(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Not Found!");

                var data = _mapper.Map<InteractionRes>(interaction);
                data.Fullname = fullname;
                data.CommentAuthor = author;

                return new ObjectResponse<InteractionRes?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<InteractionRes?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Create Interaction
        public async Task<ObjectResponse<Interaction>> CreateInteractionAsync(int accId, CreateInteractionReq createInteractionReq)
        {
            try
            {
                var isExisted = await _interactionRepository
                    .CheckInteractionExistedAsync(accId, createInteractionReq.CommentId);

                if (isExisted)
                    return new ObjectResponse<Interaction>(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Is Existed!");

                var newItr = _mapper.Map<Interaction>(createInteractionReq);

                await _interactionRepository.InsertAsync(newItr);

                return new ObjectResponse<Interaction>(
                    (int)HttpStatusCode.Created,
                    "Create Interaction Successfully!",
                    newItr);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Interaction>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Interaction
        public async Task<VoidResponse> UpdateInteractionAsync(int id, UpdateInteractionReq updateInteractionReq)
        {
            try
            {
                var oldItr = await _interactionRepository.GetByIdAsync(id, true);

                if (oldItr == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Not Found!");

                var newItr = _mapper.Map(updateInteractionReq, oldItr);

                await _interactionRepository.UpdateAsync(newItr);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Interaction Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Delete Interaction
        public async Task<VoidResponse> DeleteInteractionAsync(int id)
        {
            try
            {
                var interaction = await _interactionRepository.GetByIdAsync(id);

                if (interaction == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Not Found!");

                await _interactionRepository.DeleteAsync(id);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Interaction Successfully!");
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
