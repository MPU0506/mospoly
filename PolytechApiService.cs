using Newtonsoft.Json;
using SafeDriving.Models;
using System.Text;

namespace SafeDriving.Service.API
{
    public class PolytechApiService : IApi
    {
        private HttpClient _httpClient;
        private string _authToken;


        public PolytechApiService(string baseUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public void SetAuthToken(string authToken)
        {
            _authToken = authToken;
        }

        public async Task<Schedule> GetSchedule(string groupName)
        {
            var queryParams = new Dictionary<string, object>
            {
                { "group", groupName },
                { "getSchedule", "" },
                { "token", _authToken ?? "" }
            };

            var response = await GetAsync("old/lk_api.php/", queryParams: queryParams);

            if(response.IsSuccessStatusCode)
            {
                try
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Schedule>(result);
                }
                catch(Exception ex)
                {
                    // TODO: Тут по хорошему надо пробросить исключение наверх, и обработать в вызывающем коде 
                }
            }

            // TODO: Тут по хорошему надо пробросить исключение наверх, и обработать в вызывающем коде 
            return new Schedule();
        }

        public async Task<User> GetUser(string name, string password)
        {
            var queryParams = new Dictionary<string, object>
            {
                { "getUser", "" },
                { "token", _authToken ?? "" }
            };

            var response = await GetAsync("old/lk_api.php/", queryParams: queryParams);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var anonymousObject = JsonConvert.DeserializeAnonymousType(result, new { user = new User()});
                    return anonymousObject.user;
                }
                catch (Exception ex)
                {
                    // TODO: Тут по хорошему надо пробросить исключение наверх, и обработать в вызывающем коде 
                    return new User();
                }
            }

            // TODO: Тут по хорошему надо пробросить исключение наверх, и обработать в вызывающем коде 
            return new User();
        }

        private async Task<HttpResponseMessage> GetAsync(string endpoint, Dictionary<string, object> queryParams = null)
        {
            var uriBuilder = new UriBuilder(_httpClient.BaseAddress)
            {
                Path = $"/{endpoint}"
            };

            if (queryParams != null && queryParams.Any())
            {
                var query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                uriBuilder.Query = query;
            }

            return await _httpClient.GetAsync(uriBuilder.Uri);
        }

       
    }
}
