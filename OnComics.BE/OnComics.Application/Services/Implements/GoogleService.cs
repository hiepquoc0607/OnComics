using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OnComics.Application.Helpers;
using OnComics.Application.Models.Response.Google;
using OnComics.Application.Services.Interfaces;

namespace OnComics.Application.Services.Implements
{
    public class GoogleService : IGoogleService
    {
        private readonly GoogleHelper _googleHelper;

        public GoogleService(IOptions<GoogleHelper> googleHelper)
        {
            _googleHelper = googleHelper.Value;
        }

        //Create Login Link
        public string CreateLoginLinkAsync()
        {
            try
            {
                var redirectUrl = _googleHelper.ReturnUrl;
                var clientId = _googleHelper.ClientId;
                var scope = "openid email profile";

                if (string.IsNullOrEmpty(redirectUrl) ||
                    string.IsNullOrEmpty(clientId))
                    throw new ArgumentNullException("Invalid Google Authentication Configuration!");

                var encodedRedirect = Uri.EscapeDataString(redirectUrl!);
                var encodedScope = Uri.EscapeDataString(scope);
                var url = "https://accounts.google.com/o/oauth2/v2/auth?" +
                          "client_id={clientId}&redirect_uri={redirect}" +
                          "&response_type=code&scope={scope}&access_type=online"
                              .Replace("{clientId}", clientId)
                              .Replace("{redirect}", encodedRedirect)
                              .Replace("{scope}", encodedScope);

                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? link))
                    throw new Exception("Invalid Login Url!");

                return link.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get Google Profile Information
        public async Task<GoogleProfileRes> GetGoogleProfileAsync(string code, HttpClient httpClient)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    throw new ArgumentNullException("Missing 'code' In Query String!");

                var clientId = _googleHelper.ClientId;
                var clientSecret = _googleHelper.ClientSecret;
                var redirectUri = _googleHelper.ReturnUrl;

                if (string.IsNullOrEmpty(clientId) ||
                string.IsNullOrEmpty(clientSecret) ||
                string.IsNullOrEmpty(redirectUri))
                    throw new Exception("Google Configuration Missing!");

                var tokenResponse = await httpClient.PostAsync(
                    "https://oauth2.googleapis.com/token",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["code"] = code,
                        ["client_id"] = clientId,
                        ["client_secret"] = clientSecret,
                        ["redirect_uri"] = redirectUri,
                        ["grant_type"] = "authorization_code"
                    }));

                var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JObject.Parse(tokenResponseJson);
                var accessToken = tokenData.Value<string>("access_token");

                if (string.IsNullOrEmpty(accessToken))
                    throw new ArgumentNullException("Failed To Obtain Access RefreshToken From Google!");

                var credential = GoogleCredential.FromAccessToken(accessToken);

                var peopleService = new PeopleServiceService(
                    new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "MyAppName"
                    });

                var request = peopleService.People.Get("people/me");
                request.PersonFields = "names,emailAddresses,photos";

                var person = await request.ExecuteAsync();

                var profile = new GoogleProfileRes();
                profile.Fullname = person.Names?.FirstOrDefault()?.DisplayName;
                profile.Email = person.EmailAddresses?
                        .Select(e => e.Value)
                        .Where(e => !string.IsNullOrEmpty(e))
                        .FirstOrDefault();
                profile.PictureUrl = person.Photos?
                        .Select(p => p.Url)
                        .Where(p => !string.IsNullOrEmpty(p))
                        .FirstOrDefault();

                return profile;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
