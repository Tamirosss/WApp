using Newtonsoft.Json;

namespace WApp
{
    public partial class HomePage : ContentPage
    {
        private readonly ApiService _apiService;
        private const string BaseUrl = "http://localhost:5000";

        public HomePage()
        {
            InitializeComponent();
            _apiService = new ApiService();

            if (!string.IsNullOrEmpty(UserSession.Username))
            {
                WelcomeLabel.Text = $"Hello, {UserSession.Username}! 👋";
            }

            CheckSavedWorkout();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckSavedWorkout();
        }

        private async void CheckSavedWorkout()
        {
            try
            {
                string url = $"{BaseUrl}/get-user-workout?userId={UserSession.UserId}";
                string json = await _apiService.GetJsonAsync(url);

                if (!string.IsNullOrEmpty(json) && json != "null" && json != "[]")
                {
                    var workouts = JsonConvert.DeserializeObject<Workout[]>(json);
                    if (workouts != null && workouts.Length > 0)
                    {
                        MyWorkoutButton.IsEnabled = true;
                        NoWorkoutLabel.IsVisible = false;
                        DataHolder.Workouts = workouts;
                        return;
                    }
                }

                MyWorkoutButton.IsEnabled = false;
                NoWorkoutLabel.IsVisible = true;
                DataHolder.Workouts = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking saved workout: {ex.Message}");
                MyWorkoutButton.IsEnabled = false;
                NoWorkoutLabel.IsVisible = true;
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Logout",
                "Are you sure you want to logout?",
                "Yes",
                "No"
            );

            if (confirm)
            {
                UserSession.Username = null;
                UserSession.UserId = 0;
                UserSession.IsLoggedIn = false;
                DataHolder.Workouts = null;

                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        private async void OnMyWorkoutClicked(object sender, EventArgs e)
        {
            if (DataHolder.Workouts != null && DataHolder.Workouts.Length > 0)
            {
                await Navigation.PushAsync(new WorkoutPage());
            }
            else
            {
                await DisplayAlert("Error", "No saved workout plan found", "OK");
            }
        }

        private async void OnCreateNewWorkoutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
    }
}