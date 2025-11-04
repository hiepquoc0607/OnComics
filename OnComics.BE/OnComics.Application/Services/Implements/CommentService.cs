using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Enums.Comment;
using OnComics.Application.Models.Request.Comment;
using OnComics.Application.Models.Response.Comment;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public CommentService(
            ICommentRepository commentRepository,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        //Get All Comments
        public async Task<ObjectResponse<IEnumerable<CommentRes>?>> GetCommentsAsync(GetCommentReq getCommentReq)
        {
            try
            {
                string? searchKey = getCommentReq.SearchKey;

                int pageNum = getCommentReq.PageNum;
                int pageIndex = getCommentReq.PageIndex;

                bool isDecending = getCommentReq.IsDescending;

                Guid? searchId = getCommentReq.Id;

                bool? isComicId = getCommentReq.IdType switch
                {
                    CmtIdType.ACCOUNT => false,
                    CmtIdType.COMIC => true,
                    _ => null
                };

                Expression<Func<Comment, bool>>? seacrh = null;

                int totalData = 0;

                if (searchId.HasValue && isComicId == false)
                {
                    seacrh = c => (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(c.Account.Fullname, $"%{searchKey}%") ||
                        EF.Functions.Like(c.Comic.Name, $"%{searchKey}%")) &&
                        c.AccountId == searchId;

                    totalData = await _commentRepository.CountCommentAsync(searchId.Value, false);
                }
                else if (searchId.HasValue && isComicId == true)
                {
                    seacrh = c => (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(c.Account.Fullname, $"%{searchKey}%") ||
                        EF.Functions.Like(c.Comic.Name, $"%{searchKey}%")) &&
                        c.ComicId == searchId;

                    totalData = await _commentRepository.CountCommentAsync(searchId.Value, true);
                }
                else
                {
                    seacrh = c => (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(c.Account.Fullname, $"%{searchKey}%") ||
                        EF.Functions.Like(c.Comic.Name, $"%{searchKey}%"));

                    totalData = await _commentRepository.CountRecordAsync();
                }

                Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? order = c => getCommentReq.SortBy switch
                {
                    CmtSortOption.ACCOUNT => isDecending
                        ? c.OrderByDescending(c => c.Account.Fullname)
                        : c.OrderBy(c => c.Account.Fullname),
                    CmtSortOption.COMIC => isDecending
                        ? c.OrderByDescending(c => c.Account.Fullname)
                        : c.OrderBy(c => c.Account.Fullname),
                    CmtSortOption.TIME => isDecending
                        ? c.OrderByDescending(c => c.CmtTime)
                        : c.OrderBy(c => c.CmtTime),
                    CmtSortOption.INTERACTION => isDecending
                        ? c.OrderByDescending(c => c.InteractionNum)
                        : c.OrderBy(c => c.InteractionNum),
                    _ => c.OrderBy(c => c.Id)
                };

                var (comments, accounts, comics) = await _commentRepository
                    .GetCommentsAsync(seacrh, order, pageNum, pageIndex);

                if (comments == null)
                    return new ObjectResponse<IEnumerable<CommentRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Comment Data Empty!");

                var data = comments.Select(c => new CommentRes
                {
                    Id = c.Id,
                    AccountId = c.Account.Id,
                    Fullname = accounts[c.AccountId],
                    ComicId = c.ComicId,
                    ComicName = comics[c.ComicId],
                    Content = c.Content,
                    IsMainCmt = c.IsMainCmt,
                    MainCmtId = c.MainCmtId,
                    CmtTime = c.CmtTime,
                    InteractionNum = c.InteractionNum
                });

                var toatlPage = (int)Math.Ceiling((decimal)totalData / pageIndex);
                var pagination = new Pagination(totalData, pageIndex, pageNum, toatlPage);

                return new ObjectResponse<IEnumerable<CommentRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<CommentRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Get Reply Comments
        public async Task<ObjectResponse<IEnumerable<CommentRes>?>> GetReplyCommentsAsync(Guid mainCmtId)
        {
            try
            {
                var (comments, accounts) = await _commentRepository.GetReplyCommentsAsync(mainCmtId);

                if (comments == null)
                    return new ObjectResponse<IEnumerable<CommentRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Comment Has No Reply!");

                var data = comments.Select(d => new CommentRes
                {
                    Id = d.Id,
                    AccountId = d.AccountId,
                    Fullname = accounts[d.AccountId],
                    ComicId = null,
                    ComicName = null,
                    Content = d.Content,
                    IsMainCmt = d.IsMainCmt,
                    MainCmtId = d.MainCmtId,
                    CmtTime = d.CmtTime,
                    InteractionNum = d.InteractionNum
                });

                return new ObjectResponse<IEnumerable<CommentRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Sucessfully!",
                    data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<CommentRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Create Comment
        public async Task<ObjectResponse<Comment>> CreateCommentAsync(CreateCommentReq createCommentReq)
        {
            try
            {
                var isExisted = await _commentRepository.CheckCommentExistedAsync(
                    createCommentReq.AccountId,
                    createCommentReq.ComicId);

                if (isExisted)
                    return new ObjectResponse<Comment>(
                        (int)HttpStatusCode.BadRequest,
                        "Comment Is Existed!");

                var newCmt = _mapper.Map<Comment>(createCommentReq);

                await _commentRepository.InsertAsync(newCmt);

                return new ObjectResponse<Comment>(
                    (int)HttpStatusCode.Created,
                    "Create Comment Successfully!",
                    newCmt);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Comment>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Reply Comment
        public async Task<ObjectResponse<Comment>> ReplyCommentAsync(Guid mainCmtId, CreateCommentReq createCommentReq)
        {
            try
            {
                var mainCmt = await _commentRepository.GetByIdAsync(mainCmtId, false);

                if (mainCmt == null)
                    return new ObjectResponse<Comment>(
                        (int)HttpStatusCode.NotFound,
                        "Comment Not Found!");

                var newCmt = _mapper.Map<Comment>(createCommentReq);
                newCmt.IsMainCmt = false;
                newCmt.MainCmtId = mainCmtId;

                await _commentRepository.InsertAsync(newCmt);

                return new ObjectResponse<Comment>(
                    (int)HttpStatusCode.Created,
                    "Reply Comment Successfully!",
                    newCmt);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Comment>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Comment
        public async Task<VoidResponse> UpdateCommentAsync(Guid id, UpdateCommentReq updateCommentReq)
        {
            try
            {
                var oldCmt = await _commentRepository.GetByIdAsync(id, true);

                if (oldCmt == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Comment Not Found!");

                var newCmt = _mapper.Map(updateCommentReq, oldCmt);

                await _commentRepository.UpdateAsync(newCmt);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Comment Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Delete Comment
        public async Task<VoidResponse> DeleteCommentAsync(Guid id)
        {
            try
            {
                var cmt = await _commentRepository.GetByIdAsync(id, true);

                if (cmt == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Comment Not Found!");

                await _commentRepository.DeleteAsync(cmt);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Comment Successfully!");
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
