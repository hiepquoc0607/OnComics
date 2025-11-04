using Microsoft.AspNetCore.Http;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepsitory _attachmentRepsitory;
        private readonly IAppwriteService _appwriteService;

        public AttachmentService(
            IAttachmentRepsitory attachmentRepsitory,
            IAppwriteService appwriteService)
        {
            _attachmentRepsitory = attachmentRepsitory;
            _appwriteService = appwriteService;
        }

        //Create Attachment
        public async Task<ObjectResponse<Attachment>> CreateAttachmentAsync(Guid commentId, List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return new ObjectResponse<Attachment>(
                        (int)HttpStatusCode.BadRequest,
                        "no file uploaded!");

                var attachments = new List<Attachment>();

                foreach (var item in files)
                {
                    var id = new Guid();

                    var file = await _appwriteService.CreateFileAsync(item, id.ToString());

                    var atm = new Attachment
                    {
                        Id = id,
                        CommentId = commentId,
                        StorageUrl = file.Url
                    };

                    attachments.Add(atm);
                }

                await _attachmentRepsitory.BulkInsertRangeAsync(attachments);

                return new ObjectResponse<Attachment>(
                    (int)HttpStatusCode.OK,
                    "Create Attachment Successfully!");

            }
            catch (Exception ex)
            {
                return new ObjectResponse<Attachment>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }
    }
}
