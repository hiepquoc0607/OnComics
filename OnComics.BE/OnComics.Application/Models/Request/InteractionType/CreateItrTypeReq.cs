using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.InteractionType
{
    public class CreateItrTypeReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
