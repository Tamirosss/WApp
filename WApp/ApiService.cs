using Newtonsoft.Json;
using System.Text;

namespace WApp
{
    public class ApiService
    {
        // HttpClient סטטי - משותף לכל המחלקה כדי למנוע בעיות ביצועים
        private static readonly HttpClient client = new HttpClient();

        // ============================================================
        // שולחת בקשת GET לכתובת URL ומחזירה את התגובה כ-JSON
        // url - הכתובת לשליחת הבקשה
        // מחזירה string של JSON, או null אם הבקשה נכשלה
        // ============================================================
        public async Task<string> GetJsonAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                // זריקת שגיאה אם קוד הסטטוס אינו הצלחה (2xx)
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

        // ============================================================
        // שולחת בקשת POST עם תוכן JSON ומחזירה תגובת אימות
        // url - הכתובת לשליחת הבקשה
        // jsonContent - גוף הבקשה בפורמט JSON
        // מחזירה AuthResponse עם סטטוס הצלחה/כישלון ונתוני המשתמש
        // ============================================================
        public async Task<AuthResponse> PostJsonAsync(string url, string jsonContent)
        {
            try
            {
                // עטיפת ה-JSON בתוכן HTTP עם encoding UTF-8
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                string responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // הצלחה - פענוח התגובה לאובייקט AuthResponse
                    return JsonConvert.DeserializeObject<AuthResponse>(responseJson);
                }
                else
                {
                    try
                    {
                        // ניסיון לפענח את הודעת השגיאה מהשרת
                        return JsonConvert.DeserializeObject<AuthResponse>(responseJson);
                    }
                    catch
                    {
                        // אם הפענוח נכשל - החזרת הודעת שגיאה כללית
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

    // ============================================================
    // מודל תגובה לבקשות התחברות והרשמה מהשרת
    // ============================================================
    public class AuthResponse
    {
        public bool success { get; set; }       // האם הבקשה הצליחה
        public string message { get; set; }     // הודעת הצלחה או שגיאה
        public string username { get; set; }    // שם המשתמש שהתחבר
        public int userId { get; set; }         // מזהה המשתמש במסד הנתונים
    }
}
