using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnComics.Application.Enums.Interaction;
using OnComics.Application.Models.Request.Interaction;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.Interaction;
using OnComics.Application.Models.Response.InteractionType;
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
        private readonly IInteractionTypeRepository _interactionTypeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public InteractionService(
            IInteractionRepository interactionRepository,
            IInteractionTypeRepository interactionTypeRepository,
            ICommentRepository commentRepository,
            IMapper mapper)
        {
            _interactionRepository = interactionRepository;
            _interactionTypeRepository = interactionTypeRepository;
            _commentRepository = commentRepository;
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

                Guid? searchId = getInteractionReq.Id;

                bool? isAccId = getInteractionReq.IdType switch
                {
                    InteractionIdType.ACCOUNT => true,
                    InteractionIdType.COMMENT => false,
                    _ => null
                };

                Expression<Func<Interaction, bool>>? search = null;

                int totalData = 0;

                if (searchId.HasValue && isAccId == true)
                {
                    search = i =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(i.Account.Fullname, $"%{search}%")) &&
                        i.AccountId == searchId;

                    totalData = await _interactionRepository.CountInteractionAsync(searchId);
                }
                else if (searchId.HasValue && isAccId == false)
                {
                    search = i =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(i.Comment.Account.Fullname, $"%{search}%")) &&
                        i.CommentId == searchId;

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

                Func<IQueryable<Interaction>, IOrderedQueryable<Interaction>>? order = r =>
                    getInteractionReq.SortBy switch
                    {
                        InteractionSortOption.ACCOUNT => isDecending
                            ? r.OrderByDescending(r => r.Account.Fullname)
                            : r.OrderBy(r => r.Account.Fullname),
                        InteractionSortOption.TIME => isDecending
                            ? r.OrderByDescending(r => r.ReactTime)
                            : r.OrderBy(r => r.ReactTime),
                        _ => r.OrderBy(r => r.Id)
                    };

                var (interactions, accounts, comments, types) = await _interactionRepository
                    .GetInteractionsAsync(search, order, pageNum, pageIndex);

                if (interactions == null || interactions.IsNullOrEmpty())
                    return new ObjectResponse<IEnumerable<InteractionRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Data Empty!");

                var data = interactions.Select(i => new InteractionRes
                {
                    Id = i.Id,
                    AccountId = accounts![i.Id].Item1,
                    Fullname = accounts![i.Id].Item2,
                    CommentId = comments![i.Id].Item1,
                    CommentAuthor = comments![i.Id].Item2,
                    Type = _mapper.Map<InteractionTypeRes>(types![i.Id]),
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
        public async Task<ObjectResponse<InteractionRes?>> GetInteractionByIdAsync(Guid id)
        {
            try
            {
                var (interaction, fullname, author, type) = await _interactionRepository
                    .GetInteractionById(id);

                if (interaction == null)
                    return new ObjectResponse<InteractionRes?>(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Not Found!");

                var data = _mapper.Map<InteractionRes>(interaction);
                data.Fullname = fullname;
                data.CommentAuthor = author;
                data.Type = _mapper.Map<InteractionTypeRes>(type!);

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
        public async Task<ObjectResponse<Interaction>> CreateInteractionAsync(Guid accId, CreateInteractionReq createInteractionReq)
        {
            var newItr = new Interaction();

            try
            {
                var comment = await _commentRepository
                    .GetByIdAsync(createInteractionReq.CommentId, true);

                if (comment == null)
                    return new ObjectResponse<Interaction>(
                        (int)HttpStatusCode.NotFound,
                        "Comment Not Found!");

                var type = await _interactionTypeRepository
                    .GetByIdAsync(createInteractionReq.TypeId, false);

                if (type == null)
                    return new ObjectResponse<Interaction>(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Type Not Found!");

                var isExisted = await _interactionRepository
                    .CheckInteractionExistedAsync(accId, createInteractionReq.CommentId);

                if (isExisted)
                    return new ObjectResponse<Interaction>(
                        (int)HttpStatusCode.BadRequest,
                        "Interaction Is Existed!");

                newItr = _mapper.Map<Interaction>(createInteractionReq);
                newItr.AccountId = accId;

                comment.InteractionNum = comment.InteractionNum + 1;

                await _interactionRepository.InsertAsync(newItr);

                await _commentRepository.UpdateAsync(comment);

                return new ObjectResponse<Interaction>(
                    (int)HttpStatusCode.Created,
                    "Create Interaction Successfully!",
                    newItr);
            }
            catch (Exception ex)
            {
                var interaction = await _interactionRepository.GetByIdAsync(newItr.Id, true);

                if (interaction != null)
                    await _interactionRepository.DeleteAsync(interaction);

                return new ObjectResponse<Interaction>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Interaction
        public async Task<VoidResponse> UpdateInteractionAsync(Guid id, UpdateInteractionReq updateInteractionReq)
        {
            try
            {
                var type = await _interactionTypeRepository
                    .GetByIdAsync(updateInteractionReq.TypeId, false);

                if (type == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Type Not Found!");

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
        public async Task<VoidResponse> DeleteInteractionAsync(Guid id)
        {
            var interaction = new Interaction();

            try
            {
                interaction = await _interactionRepository.GetByIdAsync(id, true);

                if (interaction == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Interaction Not Found!");

                var comment = await _commentRepository.GetByIdAsync(interaction.CommentId, true);

                if (comment == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Comment Not Found!");

                comment.InteractionNum = comment.InteractionNum - 1;

                await _interactionRepository.DeleteAsync(interaction);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Interaction Successfully!");
            }
            catch (Exception ex)
            {
                var data = await _interactionRepository.GetByIdAsync(id, false);

                if (data == null && interaction != null)
                    await _interactionRepository.InsertAsync(interaction);

                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }
    }
}
