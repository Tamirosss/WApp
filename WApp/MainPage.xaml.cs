using Newtonsoft.Json;

namespace WApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            birthDatePicker.MaximumDate = DateTime.Now;
            birthDatePicker.Date = new DateTime(2000, 1, 1);
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
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

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            string workoutHistory = historyPicker.SelectedItem as string;
            string goal = goalPicker.SelectedItem as string;
            string amount = amountworkouts.SelectedItem as string;
            DateTime birthdate = birthDatePicker.Date;
            int age = CalculateAge(birthdate);
            string location = locationPicker.SelectedItem as string;
            string heightText = heightPicker.Text;
            string weightText = weightPicker.Text;

            // Basic field validation
            if (string.IsNullOrEmpty(workoutHistory) || string.IsNullOrEmpty(goal) ||
                string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(location) ||
                string.IsNullOrEmpty(heightText) || string.IsNullOrEmpty(weightText))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            // Parse height and weight
            if (!int.TryParse(heightText, out int height) || !int.TryParse(weightText, out int weight))
            {
                await DisplayAlert("Error", "Height and weight must be valid numbers", "OK");
                return;
            }

            // Validate data
            var validationResult = ValidateUserData(age, height, weight);
            if (!validationResult.IsValid)
            {
                await DisplayAlert("Invalid Data", validationResult.Message, "OK");
                return;
            }

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
                Text = "Creating your workout plan...",
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

            var progressFrame = new Frame
            {
                BackgroundColor = Color.FromArgb("#8b5cf6"),
                CornerRadius = 15,
                Padding = 20,
                HasShadow = false,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var progressStack = new VerticalStackLayout
            {
                Spacing = 10
            };

            var step1 = new Label
            {
                Text = "⟳ Analyzing your data",
                FontSize = 14,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var step2 = new Label
            {
                Text = "⟳ Building personalized plan",
                FontSize = 14,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var step3 = new Label
            {
                Text = "⟳ Selecting exercises",
                FontSize = 14,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };

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
                _ = AnimateSteps(step1, step2, step3);

                ApiService api = new ApiService();

                string url = $"http://localhost:5000/workouts?userId={UserSession.UserId}&age={age}&history={workoutHistory}&goal={goal}&location={location}&weight={weight}&height={height}&amount={amount}";

                string json = await api.GetJsonAsync(url);

                await Navigation.PopModalAsync(animated: true);

                if (!string.IsNullOrEmpty(json))
                {
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

        private ValidationResult ValidateUserData(int age, int height, int weight)
        {
            // Age validation
            if (age < 13)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "You must be at least 13 years old to use this app.\n\nFor safety reasons, young children should consult with a doctor or fitness professional before starting a workout program."
                };
            }

            if (age > 100)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Please enter a valid birth date.\n\nThe age seems unrealistic. Check your birth date and try again."
                };
            }

            if (age > 80)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "For your safety, we recommend consulting with a doctor before starting any workout program at this age.\n\nPlease get medical clearance first."
                };
            }

            // Height validation (in cm)
            if (height < 120)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Height seems too low.\n\nPlease enter a realistic height in centimeters (e.g., 170 for 170cm)."
                };
            }

            if (height > 250)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Height seems unrealistic.\n\nPlease enter your height in centimeters (e.g., 175 for 175cm)."
                };
            }

            // Weight validation (in kg)
            if (weight < 30)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Weight seems too low.\n\nFor safety reasons, please consult with a healthcare professional before starting a workout program."
                };
            }

            if (weight > 300)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "For your safety, we strongly recommend consulting with a doctor before starting any workout program.\n\nPlease get medical clearance first."
                };
            }

            // BMI validation
            double heightInMeters = height / 100.0;
            double bmi = weight / (heightInMeters * heightInMeters);

            if (bmi < 14)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Your BMI indicates you may be severely underweight.\n\nPlease consult with a healthcare professional before starting a workout program."
                };
            }

            if (bmi > 50)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "For your safety and health, we recommend consulting with a doctor before starting any intensive workout program.\n\nA medical professional can help create a safe plan for you."
                };
            }

            // Check age-specific weight concerns
            if (age >= 13 && age <= 17)
            {
                // Teen validation - more strict
                if (bmi < 15.5 || bmi > 35)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "As a teenager, it's important to consult with a parent, doctor, or fitness professional before starting a workout program.\n\nYour health and safety come first!"
                    };
                }
            }

            // All validations passed
            return new ValidationResult { IsValid = true };
        }

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
            catch
            {
            }
        }

        private int CalculateAge(DateTime birthdate)
        {
            int age = DateTime.Now.Year - birthdate.Year;

            if (DateTime.Now < birthdate.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}