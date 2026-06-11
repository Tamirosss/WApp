using Newtonsoft.Json;

namespace WApp
{
    public partial class HomePage : ContentPage
    {
        private readonly ApiService _apiService;
        private const string BaseUrl = "http://fit4you.185.181.10.185.sslip.io";

        public HomePage()
        {
            InitializeComponent();
            _apiService = new ApiService();

            // הצגת שם המשתמש בברכה
            if (!string.IsNullOrEmpty(UserSession.Username))
                WelcomeLabel.Text = $"Hello, {UserSession.Username}! 👋";

            CheckSavedWorkout();
        }

        // ============================================================
        // נקרא בכל פעם שהעמוד מופיע מחדש - מרענן את סטטוס האימון
        // ============================================================
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckSavedWorkout();
        }

        // ============================================================
        // בודקת אם יש תוכנית אימון שמורה למשתמש בשרת
        // מפעילה/מכבה את כפתור "My Workout Plan" בהתאם
        // ============================================================
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
                        // יש אימון שמור - מפעילים את הכפתור
                        MyWorkoutButton.IsEnabled = true;
                        NoWorkoutLabel.IsVisible = false;
                        DataHolder.Workouts = workouts;
                        return;
                    }
                }

                // אין אימון שמור - מכבים את הכפתור ומציגים הודעה
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

        // ============================================================
        // כפתור התנתקות - מנקה את כל נתוני המשתמש ומחזיר למסך ההתחברות
        // ============================================================
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

        // ============================================================
        // כפתור "My Workout Plan" - מעבר לעמוד האימונים השמורים
        // ============================================================
        private async void OnMyWorkoutClicked(object sender, EventArgs e)
        {
            if (DataHolder.Workouts != null && DataHolder.Workouts.Length > 0)
                await Navigation.PushAsync(new WorkoutPage());
            else
                await DisplayAlert("Error", "No saved workout plan found", "OK");
        }

        // ============================================================
        // כפתור "Create New Workout" - מעבר לעמוד יצירת תוכנית חדשה
        // ============================================================
        private async void OnCreateNewWorkoutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }

        // ============================================================
        // כפתור "Find Gyms Nearby" - מעבר לעמוד חדרי כושר בסביבה
        // ============================================================
        private async void OnFindGymsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GymsNearbyPage());
        }
    }
}








































