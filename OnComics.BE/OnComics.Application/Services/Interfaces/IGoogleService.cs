using OnComics.Application.Models.Response.Google;

namespace OnComics.Application.Services.Interfaces
{
    public interface IGoogleService
    {
        string CreateLoginLinkAsync();

        Task<GoogleProfileRes> GetGoogleProfileAsync(string code, HttpClient httpClient);
    }
}
