using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Should;
using Xunit;

namespace ApiTest
{
    public class Class1
    {
        public User User { get; set; }
        public Class1()
        {
            User = new User { Username = "login@user.con", Password = "P@ssw0rd" };
        }

        [Fact]
        public void CheckConstructor()
        {
            User.Username.ShouldEqual("login@user.con");
        }

        [Fact]
        public void CheckConstructor2()
        {
            User.Password.ShouldEqual("P@ssw0rd");
        }

        

        [Fact]
        public async void RunGetAll()
        {
            using (var client = new CustomHttpClient())
            {
                var result = await client.GetAsync("api/test/1");
                result.IsSuccessStatusCode.ShouldEqual(true);
                result.Content.ReadAsAsync<string>().Result.ShouldEqual("value");
            }
        }

        [Fact]
        public void RunGet()
        {
            using (var client = new CustomHttpClient())
            {
                var result = client.GetAsync("api/test").Result;
                result.IsSuccessStatusCode.ShouldEqual(true);
                var expectedResult = new[] { "value1", "value2" };
                result.Content.ReadAsAsync<string[]>().Result.ShouldEqual(expectedResult);

            }
        }
    }
}
