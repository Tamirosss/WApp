using System.Text;
using Newtonsoft.Json;

namespace WApp
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiService _apiService;
        private const string BaseUrl = "http://fit4you.185.181.10.185.sslip.io";

        public LoginPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        // ============================================================
        // לחיצה על טאב "Login" - מציגה את טופס ההתחברות ומסתירה את טופס ההרשמה
        // ============================================================
        private void OnLoginTabClicked(object sender, EventArgs e)
        {
            LoginForm.IsVisible = true;
            RegisterForm.IsVisible = false;

            // עדכון עיצוב הכפתורים - הטאב הפעיל כחול, הלא פעיל לבן
            LoginTabButton.BackgroundColor = Color.FromArgb("#667eea");
            LoginTabButton.TextColor = Colors.White;
            RegisterTabButton.BackgroundColor = Colors.White;
            RegisterTabButton.TextColor = Color.FromArgb("#667eea");
        }

        // ============================================================
        // לחיצה על טאב "Register" - מציגה את טופס ההרשמה ומסתירה את טופס ההתחברות
        // ============================================================
        private void OnRegisterTabClicked(object sender, EventArgs e)
        {
            LoginForm.IsVisible = false;
            RegisterForm.IsVisible = true;

            RegisterTabButton.BackgroundColor = Color.FromArgb("#667eea");
            RegisterTabButton.TextColor = Colors.White;
            LoginTabButton.BackgroundColor = Colors.White;
            LoginTabButton.TextColor = Color.FromArgb("#667eea");
        }

        // ============================================================
        // לחיצה על "Login" - מאמתת קלט ושולחת בקשת התחברות לשרת
        // בהצלחה - שומרת את נתוני המשתמש ומעברת לדף הבית
        // ============================================================
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = LoginUsernameEntry.Text?.Trim();
            string password = LoginPasswordEntry.Text;

            // ולידציה בסיסית
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                var loginData = new { username, password };
                string json = JsonConvert.SerializeObject(loginData);
                var response = await _apiService.PostJsonAsync($"{BaseUrl}/login", json);

                if (response != null && response.success)
                {
                    // שמירת נתוני המשתמש בזיכרון הזמני של האפליקציה
                    UserSession.Username = username;
                    UserSession.UserId = response.userId;
                    UserSession.IsLoggedIn = true;

                    // שמירת נתוני המשתמש בזיכרון הקבוע של המכשיר (להישאר מחובר)
                    Preferences.Set("UserId", response.userId);
                    Preferences.Set("Username", username);

                    Application.Current.MainPage = new NavigationPage(new HomePage());
                }
                else
                {
                    await DisplayAlert("Error", response?.message ?? "Invalid username or password", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        // ============================================================
        // לחיצה על "Register" - מאמתת קלט ושולחת בקשת הרשמה לשרת
        // בהצלחה - מעברת אוטומטית לטופס ההתחברות
        // ============================================================
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string username = RegisterUsernameEntry.Text?.Trim();
            string password = RegisterPasswordEntry.Text;
            string confirmPassword = RegisterConfirmPasswordEntry.Text;

            // ולידציה - בדיקת שדות ריקים
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            // ולידציה - אורך מינימלי לשם משתמש
            if (username.Length < 3)
            {
                await DisplayAlert("Error", "Username must be at least 3 characters", "OK");
                return;
            }

            // ולידציה - אורך מינימלי לסיסמה
            if (password.Length < 6)
            {
                await DisplayAlert("Error", "Password must be at least 6 characters", "OK");
                return;
            }

            // ולידציה - בדיקת התאמת סיסמאות
            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                var registerData = new { username, password };
                string json = JsonConvert.SerializeObject(registerData);
                var response = await _apiService.PostJsonAsync($"{BaseUrl}/register", json);

                if (response != null && response.success)
                {
                    await DisplayAlert("Success", "User registered successfully! You can now login", "OK");

                    // מעבר אוטומטי לטאב ההתחברות וניקוי השדות
                    OnLoginTabClicked(null, null);
                    RegisterUsernameEntry.Text = "";
                    RegisterPasswordEntry.Text = "";
                    RegisterConfirmPasswordEntry.Text = "";
                }
                else
                {
                    await DisplayAlert("Error", response?.message ?? "Registration failed", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }
    }

    // ============================================================
    // מחזיק את נתוני המשתמש המחובר בזיכרון הזמני של האפליקציה
    // ============================================================
    public static class UserSession
    {
        public static string Username { get; set; }
        public static int UserId { get; set; }
        public static bool IsLoggedIn { get; set; }
    }
}