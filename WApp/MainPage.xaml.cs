
using Newtonsoft.Json;

namespace WApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // הגדרת תאריך מקסימלי להיום ותאריך ברירת מחדל לשנת 2000
            birthDatePicker.MaximumDate = DateTime.Now;
            birthDatePicker.Date = new DateTime(2000, 1, 1);
        }

        // ============================================================
        // כפתור חזור - חוזר לעמוד הקודם בניווט
        // ============================================================
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        // ============================================================
        // כפתור התנתקות - מנקה את הסשן ומחזיר למסך ההתחברות
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
                // איפוס כל נתוני המשתמש
                UserSession.Username = null;
                UserSession.UserId = 0;
                UserSession.IsLoggedIn = false;
                DataHolder.Workouts = null;

                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        // ============================================================
        // לחיצה על "Create Workout Plan" - מאמתת קלט ושולחת בקשה ל-API
        // יוצרת תוכנית אימון מותאמת אישית באמצעות Gemini AI
        // ============================================================
        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            // איסוף כל הערכים מהטפסים
            string gender = genderPicker.SelectedItem as string;
            string workoutHistory = historyPicker.SelectedItem as string;
            string goal = goalPicker.SelectedItem as string;
            string amount = amountworkouts.SelectedItem as string;
            DateTime birthdate = birthDatePicker.Date;
            int age = CalculateAge(birthdate);
            string location = locationPicker.SelectedItem as string;
            string height = heightPicker.Text;
            string weight = weightPicker.Text;

            // ולידציה - בדיקה שכל השדות מולאו
            if (string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(workoutHistory) ||
                string.IsNullOrEmpty(goal) || string.IsNullOrEmpty(amount) ||
                string.IsNullOrEmpty(location) || string.IsNullOrEmpty(height) ||
                string.IsNullOrEmpty(weight))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            // הצגת מסך טעינה אנימטיבי בזמן יצירת התוכנית
            var loadingPage = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#667eea")
            };

            var loadingStack = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 30,
                Padding = 40
            };

            var activityIndicator = new ActivityIndicator
            {
                IsRunning = true,
                Color = Colors.White,
                HeightRequest = 80,
                WidthRequest = 80
            };

            var loadingLabel = new Label
            {
                Text = "🤖 Creating your workout plan...",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var subLabel = new Label
            {
                Text = "This may take a few seconds",
                FontSize = 16,
                TextColor = Color.FromArgb("#e0e7ff"),
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, -10, 0, 0)
            };

            // פריים שמציג את שלבי היצירה
            var progressFrame = new Frame
            {
                BackgroundColor = Color.FromArgb("#8b5cf6"),
                CornerRadius = 15,
                Padding = 20,
                HasShadow = false,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var progressStack = new VerticalStackLayout { Spacing = 10 };

            // תוויות לשלבי התהליך
            var step1 = new Label { Text = "⟳ Analyzing your data", FontSize = 14, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center };
            var step2 = new Label { Text = "⟳ Building personalized plan", FontSize = 14, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center };
            var step3 = new Label { Text = "⟳ Selecting exercises", FontSize = 14, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center };

            progressStack.Children.Add(step1);
            progressStack.Children.Add(step2);
            progressStack.Children.Add(step3);
            progressFrame.Content = progressStack;

            loadingStack.Children.Add(activityIndicator);
            loadingStack.Children.Add(loadingLabel);
            loadingStack.Children.Add(subLabel);
            loadingStack.Children.Add(progressFrame);

            loadingPage.Content = loadingStack;

            await Navigation.PushModalAsync(loadingPage, animated: true);

            try
            {
                // הפעלת אנימציית השלבים ברקע
                _ = AnimateSteps(step1, step2, step3);

                ApiService api = new ApiService();

                // שליחת בקשה ל-API ליצירת תוכנית אימון
                string url = $"http://fit4you.185.181.10.185.sslip.io/workouts?userId={UserSession.UserId}&age={age}&gender={gender}&history={workoutHistory}&goal={goal}&location={location}&weight={weight}&height={height}&amount={amount}";

                string json = await api.GetJsonAsync(url);

                await Navigation.PopModalAsync(animated: true);

                if (!string.IsNullOrEmpty(json))
                {
                    // שמירת התוכנית ומעבר לעמוד האימונים
                    Workout[] workouts = JsonConvert.DeserializeObject<Workout[]>(json);
                    DataHolder.Workouts = workouts;

                    await DisplayAlert("🎉 Success!", "Your workout plan is ready!", "Let's Start");

                    await Navigation.PushAsync(new WorkoutPage());
                }
                else
                {
                    await DisplayAlert("Error", "Failed to create workout plan", "OK");
                }
            }
            catch (Exception ex)
            {
                await Navigation.PopModalAsync(animated: true);
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        // ============================================================
        // מאניימטת את תוויות השלבים בזמן יצירת תוכנית האימון
        // מחכה ומעדכנת כל תווית עם ✓ אחרי עיכוב קצר
        // ============================================================
        private async Task AnimateSteps(Label step1, Label step2, Label step3)
        {
            try
            {
                await Task.Delay(1000);
                step1.Text = "✓ Analyzing your data";

                await Task.Delay(1500);
                step2.Text = "✓ Building personalized plan";

                await Task.Delay(1500);
                step3.Text = "✓ Selecting exercises";
            }
            catch { }
        }

        // ============================================================
        // מחשבת גיל לפי תאריך לידה
        // מתחשבת בכך שייתכן שהיום ביום השנה עוד לא עבר
        // ============================================================
        private int CalculateAge(DateTime birthdate)
        {
            int age = DateTime.Now.Year - birthdate.Year;

            // הפחתת שנה אם יום ההולדת של השנה עוד לא עבר
            if (DateTime.Now < birthdate.AddYears(age))
                age--;

            return age;
        }
    }
}