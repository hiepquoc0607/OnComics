using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnComics.Application.Enums.Comment;
using OnComics.Application.Models.Request.Comment;
using OnComics.Application.Models.Response.Appwrite;
using OnComics.Application.Models.Response.Attachment;
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
        private readonly IComicRepository _comicRepository;
        private readonly IAttachmentRepsitory _attachmentRepsitory;
        private readonly IAppwriteService _appwriteService;
        private readonly IMapper _mapper;

        public CommentService(
            ICommentRepository commentRepository,
            IComicRepository comicRepository,
            IAttachmentRepsitory attachmentRepsitory,
            IAppwriteService appwriteService,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _comicRepository = comicRepository;
            _attachmentRepsitory = attachmentRepsitory;
            _appwriteService = appwriteService;
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
                        c.AccountId == searchId &&
                        c.IsMainCmt == true;

                    totalData = await _commentRepository.CountCommentAsync(searchId.Value, false);
                }
                else if (searchId.HasValue && isComicId == true)
                {
                    seacrh = c => (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(c.Account.Fullname, $"%{searchKey}%") ||
                        EF.Functions.Like(c.Comic.Name, $"%{searchKey}%")) &&
                        c.ComicId == searchId &&
                        c.IsMainCmt == true;

                    totalData = await _commentRepository.CountCommentAsync(searchId.Value, true);
                }
                else
                {
                    seacrh = c => (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(c.Account.Fullname, $"%{searchKey}%") ||
                        EF.Functions.Like(c.Comic.Name, $"%{searchKey}%")) &&
                        c.IsMainCmt == true;

                    totalData = await _commentRepository.CountRecordAsync(seacrh);
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

                var (comments, accounts, comics, attachments) = await _commentRepository
                    .GetCommentsAsync(seacrh, order, pageNum, pageIndex);

                if (comments == null)
                    return new ObjectResponse<IEnumerable<CommentRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Comment Data Empty!");

                var data = comments.Select(c => new CommentRes
                {
                    Id = c.Id,
                    AccountId = accounts[c.Id].Item1,
                    Fullname = accounts[c.Id].Item2,
                    ComicId = comics[c.Id].Item1,
                    ComicName = comics[c.Id].Item2,
                    Content = c.Content,
                    IsMainCmt = c.IsMainCmt,
                    MainCmtId = c.MainCmtId,
                    CmtTime = c.CmtTime,
                    InteractionNum = c.InteractionNum,
                    Attachments = attachments[c.Id].Adapt<List<AttachmentRes>>()
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
                var (comments, accounts, attachments) = await _commentRepository
                    .GetReplyCommentsAsync(mainCmtId);

                if (comments == null)
                    return new ObjectResponse<IEnumerable<CommentRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Comment Has No Reply!");

                var data = comments.Select(c => new CommentRes
                {
                    Id = c.Id,
                    AccountId = accounts![c.Id].Item1,
                    Fullname = accounts[c.Id].Item2,
                    ComicId = null,
                    ComicName = null,
                    Content = c.Content,
                    IsMainCmt = c.IsMainCmt,
                    MainCmtId = c.MainCmtId,
                    CmtTime = c.CmtTime,
                    InteractionNum = c.InteractionNum,
                    Attachments = attachments![c.Id].Adapt<List<AttachmentRes>>()
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
        public async Task<ObjectResponse<Comment>> CreateCommentAsync(
            Guid accId,
            List<IFormFile>? files,
            CreateCommentReq createCommentReq)
        {
            Guid id = Guid.NewGuid();

            try
            {
                if (files != null && files.Count > 5)
                    return new ObjectResponse<Comment>(
                        (int)HttpStatusCode.BadRequest,
                        "Only Create Max 5 Record At Once!");

                var comic = await _comicRepository.GetByIdAsync(createCommentReq.ComicId, false);

                if (comic == null)
                    return new ObjectResponse<Comment>(
                       (int)HttpStatusCode.NotFound,
                       "Comic Not Found");

                var newCmt = _mapper.Map<Comment>(createCommentReq);
                newCmt.Id = id;
                newCmt.AccountId = accId;
                newCmt.MainCmtId = null;

                await _commentRepository.InsertAsync(newCmt, true);

                if (files != null && files.Count > 0)
                {
                    const long maxFileSize = 2 * 1024 * 1024; // 2MB

                    var fileRes = new FileRes();

                    var attachments = new List<Attachment>();

                    foreach (var file in files)
                    {
                        if (file.Length > maxFileSize)
                            return new ObjectResponse<Comment>(
                            (int)HttpStatusCode.BadRequest,
                            "Max Size Per File Is 2MB!");

                        var attach = new Attachment();
                        attach.Id = Guid.NewGuid();
                        attach.CommentId = newCmt.Id;

                        fileRes = await _appwriteService
                            .CreateFileAsync(file, attach.Id.ToString());

                        attach.SrcUrl = fileRes.Url;

                        attachments.Add(attach);
                    }

                    newCmt.Attachments = attachments;

                    await _attachmentRepsitory.BulkInsertAsync(attachments);
                }

                return new ObjectResponse<Comment>(
                    (int)HttpStatusCode.Created,
                    "Create Comment Successfully!",
                    newCmt);
            }
            catch (Exception ex)
            {
                var cmt = await _commentRepository.GetByIdAsync(id, true);

                if (cmt != null)
                    await _commentRepository.DeleteAsync(cmt, true);

                return new ObjectResponse<Comment>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Reply Comment
        public async Task<ObjectResponse<Comment>> ReplyCommentAsync(
            Guid mainCmtId,
            Guid accId,
            List<IFormFile>? files,
            CreateCommentReq createCommentReq)
        {
            Guid id = Guid.NewGuid();

            try
            {
                if (files != null && files.Count > 5)
                    return new ObjectResponse<Comment>(
                        (int)HttpStatusCode.BadRequest,
                        "Only Create Max 5 Record At Once!");

                var comic = await _comicRepository.GetByIdAsync(createCommentReq.ComicId, false);

                if (comic == null)
                    return new ObjectResponse<Comment>(
                       (int)HttpStatusCode.NotFound,
                       "Comic Not Found");

                var mainCmt = await _commentRepository.GetByIdAsync(mainCmtId, false);

                if (mainCmt == null)
                    return new ObjectResponse<Comment>(
                        (int)HttpStatusCode.NotFound,
                        "Comment Not Found!");

                var newCmt = _mapper.Map<Comment>(createCommentReq);
                newCmt.Id = id;
                newCmt.AccountId = accId;
                newCmt.IsMainCmt = false;
                newCmt.MainCmtId = mainCmtId;

                await _commentRepository.InsertAsync(newCmt, true);

                if (files != null && files.Count > 0)
                {
                    const long maxFileSize = 2 * 1024 * 1024; // 2MB

                    var fileRes = new FileRes();

                    var attachments = new List<Attachment>();

                    foreach (var file in files)
                    {
                        if (file.Length > maxFileSize)
                            return new ObjectResponse<Comment>(
                            (int)HttpStatusCode.BadRequest,
                            "Max Size Per File Is 2MB!");

                        var attach = new Attachment();
                        attach.Id = Guid.NewGuid();
                        attach.CommentId = newCmt.Id;

                        fileRes = await _appwriteService
                            .CreateFileAsync(file, attach.Id.ToString());

                        attach.SrcUrl = fileRes.Url;

                        attachments.Add(attach);
                    }

                    await _attachmentRepsitory.BulkInsertAsync(attachments);
                }

                return new ObjectResponse<Comment>(
                    (int)HttpStatusCode.Created,
                    "Reply Comment Successfully!",
                    newCmt);
            }
            catch (Exception ex)
            {
                var cmt = await _commentRepository.GetByIdAsync(id, true);

                if (cmt != null)
                    await _commentRepository.DeleteAsync(cmt, true);

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

                await _commentRepository.UpdateAsync(newCmt, true);

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

                var attachs = await _attachmentRepsitory.GetAttachIdsByCmtIdAsync(cmt.Id);

                await _commentRepository.DeleteAsync(cmt, true);

                if (attachs != null)
                {
                    foreach (var item in attachs)
                    {
                        await _appwriteService.DeleteFileAsync(item.ToString());
                    }
                }

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
