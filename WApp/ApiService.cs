using Newtonsoft.Json;
using System.Text;

namespace WApp
{
    public class ApiService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetJsonAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                return json;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return null;
            }
        }

        public async Task<AuthResponse> PostJsonAsync(string url, string jsonContent)
        {
            try
            {
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                string responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<AuthResponse>(responseJson);
                }
                else
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<AuthResponse>(responseJson);
                    }
                    catch
                    {
                        return new AuthResponse { success = false, message = "Error communicating with server" };
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return new AuthResponse { success = false, message = ex.Message };
            }
        }
    }

    public class AuthResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string username { get; set; }
        public int userId { get; set; }
    }
}