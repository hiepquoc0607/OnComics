using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.General
{
    public class UpdateStatusReq<T>
    {
        [Required]
        public T? Status { get; set; }
    }
}
