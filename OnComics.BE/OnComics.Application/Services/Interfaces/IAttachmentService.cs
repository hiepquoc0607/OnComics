using Microsoft.AspNetCore.Http;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<ObjectResponse<Attachment>> CreateAttachmentAsync(Guid commentId, List<IFormFile> files);
    }
}
