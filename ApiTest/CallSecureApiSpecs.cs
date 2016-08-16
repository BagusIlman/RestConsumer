using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Should;
using Xunit;

namespace ApiTest
{
    public class CallSecureApiSpecs
    {
        public User User { get; set; }

        public CallSecureApiSpecs()
        {
            User = new User { Username = "login@user.con", Password = "P@ssw0rd" };
        }
        [Fact]
        public void GetToken()
        {
            var token = GetApiToken(User.Username, User.Password).Result;
            token.ShouldNotBeNull();
            token.access_token.Length.ShouldBeGreaterThan(1);
            var response = GetRequest(token.access_token, "api/values").Result;
            response.ShouldEqual("blablablba");
        }
        private static async Task<AccessToken> GetApiToken(string userName, string password)
        {
            using (var client = new CustomHttpClient())
            {
                //setup login data
                var formContent = new FormUrlEncodedContent(new[]
                {
                 new KeyValuePair<string, string>("grant_type", "password"),
                 new KeyValuePair<string, string>("username", userName),
                 new KeyValuePair<string, string>("password", password),
                 });

                HttpResponseMessage responseMessage = await client.PostAsync("/Token", formContent);

                return await responseMessage.Content.ReadAsAsync<AccessToken>();
            }
        }

        static async Task<string> GetRequest(string token, string requestPath)
        {
            using (var client = new CustomHttpClient())
            {
                //setup client
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                //make request
                HttpResponseMessage response = await client.GetAsync(requestPath);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
        }
    }
    public class CustomHttpClient : HttpClient
    {
        public CustomHttpClient()
        {
            BaseAddress = new Uri("http://localhost:12188/");
            DefaultRequestHeaders.Accept.Clear();
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string userName { get; set; }
        [JsonProperty(PropertyName = ".issued")]
        public string issued { get; set; }
        [JsonProperty(PropertyName = ".expires")]
        public string expires { get; set; }
    }
}
