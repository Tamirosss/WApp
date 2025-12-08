using System.Text;
using Newtonsoft.Json;

namespace WApp
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiService _apiService;
        private const string BaseUrl = "http://localhost:5000";

        public LoginPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private void OnLoginTabClicked(object sender, EventArgs e)
        {
            LoginForm.IsVisible = true;
            RegisterForm.IsVisible = false;

            LoginTabButton.BackgroundColor = Color.FromArgb("#667eea");
            LoginTabButton.TextColor = Colors.White;
            RegisterTabButton.BackgroundColor = Colors.White;
            RegisterTabButton.TextColor = Color.FromArgb("#667eea");
        }

        private void OnRegisterTabClicked(object sender, EventArgs e)
        {
            LoginForm.IsVisible = false;
            RegisterForm.IsVisible = true;

            RegisterTabButton.BackgroundColor = Color.FromArgb("#667eea");
            RegisterTabButton.TextColor = Colors.White;
            LoginTabButton.BackgroundColor = Colors.White;
            LoginTabButton.TextColor = Color.FromArgb("#667eea");
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = LoginUsernameEntry.Text?.Trim();
            string password = LoginPasswordEntry.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                var loginData = new
                {
                    username = username,
                    password = password
                };

                string json = JsonConvert.SerializeObject(loginData);
                var response = await _apiService.PostJsonAsync($"{BaseUrl}/login", json);

                if (response != null && response.success)
                {
                    // Save user info with userId
                    UserSession.Username = username;
                    UserSession.UserId = response.userId;
                    UserSession.IsLoggedIn = true;

                    // Navigate to HomePage
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

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string username = RegisterUsernameEntry.Text?.Trim();
            string password = RegisterPasswordEntry.Text;
            string confirmPassword = RegisterConfirmPasswordEntry.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            if (username.Length < 3)
            {
                await DisplayAlert("Error", "Username must be at least 3 characters", "OK");
                return;
            }

            if (password.Length < 6)
            {
                await DisplayAlert("Error", "Password must be at least 6 characters", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                var registerData = new
                {
                    username = username,
                    password = password
                };

                string json = JsonConvert.SerializeObject(registerData);
                var response = await _apiService.PostJsonAsync($"{BaseUrl}/register", json);

                if (response != null && response.success)
                {
                    await DisplayAlert("Success", "User registered successfully! You can now login", "OK");

                    // Switch to login tab
                    OnLoginTabClicked(null, null);

                    // Clear registration fields
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

    public static class UserSession
    {
        public static string Username { get; set; }
        public static int UserId { get; set; }
        public static bool IsLoggedIn { get; set; }
    }
}