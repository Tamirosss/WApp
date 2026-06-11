// ============================================================
// WorkoutPage.xaml.cs
// עמוד הצגת תוכנית האימון - מציג ימים, תרגילים, טיימר וסרטונים
// חשוב: קובץ זה מחליף לחלוטין את WorkoutPage.xaml.cs המקורי
// ============================================================
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.Linq;

namespace WApp
{
    public partial class WorkoutPage : ContentPage
    {
        private readonly ApiService _apiService;
        private const string BaseUrl = "http://fit4you.185.181.10.185.sslip.io";

        // שומר את אינדקס היום הנוכחי שמוצג
        private int currentDayIndex = 0;

        public WorkoutPage()
        {
            InitializeComponent();
            _apiService = new ApiService();

            // טעינת היום הראשון בפתיחת העמוד
            LoadWorkoutDay(0);
        }

        // ============================================================
        // כפתור התנתקות - מבקש אישור, מוחק את נתוני המשתמש ומחזיר למסך ההתחברות
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
                // מחיקת נתוני המשתמש מהזיכרון הקבוע
                Preferences.Remove("UserId");
                Preferences.Remove("Username");

                // איפוס הסשן הזמני
                UserSession.Username = null;
                UserSession.UserId = 0;
                UserSession.IsLoggedIn = false;
                DataHolder.Workouts = null;
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        // ============================================================
        // כפתור חזור - חוזר לעמוד הראשי
        // ============================================================
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        // ============================================================
        // טוענת ומציגה את האימון של יום מסוים לפי אינדקס
        // dayIndex - מספר היום (0 = יום ראשון)
        // ============================================================
        private void LoadWorkoutDay(int dayIndex)
        {
            // בדיקה שיש אימונים
            if (DataHolder.Workouts == null || DataHolder.Workouts.Length == 0)
            {
                DisplayAlert("Error", "No workouts available", "OK");
                return;
            }

            // בדיקה שהאינדקס בטווח התקין
            if (dayIndex < 0 || dayIndex >= DataHolder.Workouts.Length)
                return;

            currentDayIndex = dayIndex;
            WorkoutsContainer.Children.Clear();

            var workout = DataHolder.Workouts[dayIndex];

            // ---- בניית כותרת ניווט בין ימים ----
            var navFrame = new Frame
            {
                BackgroundColor = Color.FromArgb("#667eea"),
                CornerRadius = 15,
                Padding = 0,
                HasShadow = true,
                Margin = new Thickness(0, 0, 0, 20)
            };

            var navGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Padding = 15,
                ColumnSpacing = 10
            };

            // כפתור יום קודם - מושבת אם אנחנו ביום הראשון
            var prevButton = new Button
            {
                Text = "◀",
                BackgroundColor = currentDayIndex > 0 ? Colors.White : Color.FromArgb("#9ca3af"),
                TextColor = Color.FromArgb("#667eea"),
                CornerRadius = 10,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                IsEnabled = currentDayIndex > 0
            };
            prevButton.Clicked += (s, e) => LoadWorkoutDay(currentDayIndex - 1);
            navGrid.Children.Add(prevButton);
            Grid.SetColumn(prevButton, 0);

            // תצוגת מידע על היום הנוכחי
            var dayInfoStack = new VerticalStackLayout
            {
                Spacing = 5,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var dayLabel = new Label
            {
                Text = $"Day {dayIndex + 1} of {DataHolder.Workouts.Length}",
                FontSize = 14,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var dayNameLabel = new Label
            {
                Text = workout.Name ?? $"Workout {dayIndex + 1}",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };

            dayInfoStack.Children.Add(dayLabel);
            dayInfoStack.Children.Add(dayNameLabel);
            navGrid.Children.Add(dayInfoStack);
            Grid.SetColumn(dayInfoStack, 1);

            // כפתור יום הבא - מושבת אם אנחנו ביום האחרון
            var nextButton = new Button
            {
                Text = "▶",
                BackgroundColor = currentDayIndex < DataHolder.Workouts.Length - 1 ? Colors.White : Color.FromArgb("#9ca3af"),
                TextColor = Color.FromArgb("#667eea"),
                CornerRadius = 10,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                IsEnabled = currentDayIndex < DataHolder.Workouts.Length - 1
            };
            nextButton.Clicked += (s, e) => LoadWorkoutDay(currentDayIndex + 1);
            navGrid.Children.Add(nextButton);
            Grid.SetColumn(nextButton, 2);

            navFrame.Content = navGrid;
            WorkoutsContainer.Children.Add(navFrame);

            // תצוגת מספר התרגילים ביום זה
            var infoLabel = new Label
            {
                Text = $"💪 {workout.Excercises?.Length ?? 0} exercises today",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#667eea"),
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            WorkoutsContainer.Children.Add(infoLabel);

            // הצגת רשימת התרגילים
            if (workout.Excercises == null || workout.Excercises.Length == 0)
            {
                WorkoutsContainer.Children.Add(new Label
                {
                    Text = "No exercises for this day",
                    FontSize = 14,
                    TextColor = Colors.Gray,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 20)
                });
            }
            else
            {
                for (int i = 0; i < workout.Excercises.Length; i++)
                {
                    var exerciseFrame = CreateExerciseFrame(workout.Excercises[i], currentDayIndex, i);
                    WorkoutsContainer.Children.Add(exerciseFrame);
                }
            }

            // נקודות ניווט בתחתית - מראות את מספר הימים והיום הנוכחי
            var dotsStack = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10,
                Margin = new Thickness(0, 20, 0, 0)
            };

            for (int i = 0; i < DataHolder.Workouts.Length; i++)
            {
                dotsStack.Children.Add(new BoxView
                {
                    WidthRequest = i == currentDayIndex ? 30 : 10,
                    HeightRequest = 10,
                    CornerRadius = 5,
                    BackgroundColor = i == currentDayIndex
                        ? Color.FromArgb("#667eea")
                        : Color.FromArgb("#cbd5e1")
                });
            }

            WorkoutsContainer.Children.Add(dotsStack);
        }

        // ============================================================
        // יוצרת כרטיסיית UI עבור תרגיל בודד
        // exercise - נתוני התרגיל
        // dayIndex, exerciseIndex - מיקום התרגיל במערך לצורך עריכה
        // ============================================================
        private Frame CreateExerciseFrame(Excercise exercise, int dayIndex, int exerciseIndex)
        {
            var exerciseFrame = new Frame
            {
                BackgroundColor = Color.FromArgb("#f5f7fa"),
                CornerRadius = 15,
                Padding = 20,
                HasShadow = true,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var exerciseStack = new VerticalStackLayout { Spacing = 15 };

            // ---- שורת כותרת: שם התרגיל + כפתור עריכה ----
            var headerGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var nameLabel = new Label
            {
                Text = exercise.Name ?? "Unknown Exercise",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2d3748"),
                VerticalOptions = LayoutOptions.Center
            };
            headerGrid.Children.Add(nameLabel);
            Grid.SetColumn(nameLabel, 0);

            // כפתור עריכה - פותח תפריט לשינוי פרטי התרגיל
            var editButton = new Button
            {
                Text = "✏️",
                BackgroundColor = Color.FromArgb("#667eea"),
                TextColor = Colors.White,
                CornerRadius = 10,
                WidthRequest = 40,
                HeightRequest = 40,
                FontSize = 18
            };
            editButton.Clicked += async (s, e) => await OnEditExerciseClicked(dayIndex, exerciseIndex);
            headerGrid.Children.Add(editButton);
            Grid.SetColumn(editButton, 1);

            exerciseStack.Children.Add(headerGrid);

            // קו מפריד צבעוני
            exerciseStack.Children.Add(new BoxView
            {
                HeightRequest = 3,
                BackgroundColor = Color.FromArgb("#667eea"),
                HorizontalOptions = LayoutOptions.FillAndExpand
            });

            // ---- תצוגת פרטי התרגיל: סטים, מנוחה, חזרות ----
            var detailsGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                ColumnSpacing = 10
            };

            var setsFrame = CreateDetailFrame("Sets", exercise.Sets.ToString());
            detailsGrid.Children.Add(setsFrame);
            Grid.SetColumn(setsFrame, 0);

            var timeFrame = CreateDetailFrame("Rest", $"{exercise.RestTime}s");
            detailsGrid.Children.Add(timeFrame);
            Grid.SetColumn(timeFrame, 1);

            var repsFrame = CreateDetailFrame("Reps", exercise.Reps.ToString());
            detailsGrid.Children.Add(repsFrame);
            Grid.SetColumn(repsFrame, 2);

            exerciseStack.Children.Add(detailsGrid);

            // ---- כפתורי פעולה: סרטון ב-YouTube וטיימר מנוחה ----
            var buttonGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                ColumnSpacing = 10,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var videoButton = new Button
            {
                Text = "🎥 Watch",
                BackgroundColor = Color.FromArgb("#ef4444"),
                TextColor = Colors.White,
                CornerRadius = 10,
                HeightRequest = 40
            };
            videoButton.Clicked += async (s, e) => await OpenYouTubeVideo(exercise.Name);
            buttonGrid.Children.Add(videoButton);
            Grid.SetColumn(videoButton, 0);

            var timerButton = new Button
            {
                Text = "⏱️ Timer",
                BackgroundColor = Color.FromArgb("#10b981"),
                TextColor = Colors.White,
                CornerRadius = 10,
                HeightRequest = 40
            };
            timerButton.Clicked += async (s, e) => await OpenTimer(exercise.RestTime);
            buttonGrid.Children.Add(timerButton);
            Grid.SetColumn(timerButton, 1);

            exerciseStack.Children.Add(buttonGrid);

            exerciseFrame.Content = exerciseStack;
            return exerciseFrame;
        }

        // ============================================================
        // פותח חיפוש יוטיוב ישיר לתרגיל - תמיד עובד!
        // קודם מנסה מאגר מקומי, אחרת חיפוש יוטיוב ישיר
        // ============================================================
        private async Task OpenYouTubeVideo(string exerciseName)
        {
            try
            {
                // 1. ניסיון מאגר מקומי
                string videoUrl = ExerciseVideoDatabase.GetVideoUrl(exerciseName);

                // 2. אם לא נמצא - חיפוש ישיר ביוטיוב
                if (videoUrl == null)
                {
                    string searchQuery = Uri.EscapeDataString($"{exerciseName} exercise tutorial proper form");
                    videoUrl = $"https://www.youtube.com/results?search_query={searchQuery}";
                    Console.WriteLine($"[VIDEO] Not in DB, searching YouTube for: {exerciseName}");
                }

                Console.WriteLine($"[VIDEO] Opening: {videoUrl}");
                bool opened = await Launcher.Default.OpenAsync(videoUrl);

                if (!opened)
                    await DisplayAlert("Error", "Could not open browser", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VIDEO] Error: {ex.Message}");
                await DisplayAlert("Error", $"Could not open YouTube: {ex.Message}", "OK");
            }
        }

        // ============================================================
        // פותח עמוד טיימר מנוחה מודאלי עם ספירה לאחור
        // seconds - כמות השניות למנוחה
        // ============================================================
        private async Task OpenTimer(int seconds)
        {
            var timerPage = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#10b981")
            };

            var timerStack = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 40,
                Padding = 40
            };

            var titleLabel = new Label
            {
                Text = "Rest Timer",
                FontSize = 32,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };

            // תצוגת הזמן הנותר
            var timeLabel = new Label
            {
                Text = FormatTime(seconds),
                FontSize = 72,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };

            // פס התקדמות - מתרוקן ככל שהזמן עובר
            var progressBar = new ProgressBar
            {
                Progress = 1.0,
                ProgressColor = Colors.White,
                HeightRequest = 20,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var buttonGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                ColumnSpacing = 20,
                Margin = new Thickness(0, 40, 0, 0)
            };

            // משתנים לניהול מצב הטיימר
            bool isRunning = false;
            bool isPaused = false;
            int remainingSeconds = seconds;
            int totalSeconds = seconds;

            var startPauseButton = new Button
            {
                Text = "▶️ Start",
                BackgroundColor = Colors.White,
                TextColor = Color.FromArgb("#10b981"),
                CornerRadius = 15,
                HeightRequest = 60,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };

            var resetButton = new Button
            {
                Text = "🔄 Reset",
                BackgroundColor = Color.FromArgb("#ef4444"),
                TextColor = Colors.White,
                CornerRadius = 15,
                HeightRequest = 60,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };

            var closeButton = new Button
            {
                Text = "✖️ Close",
                BackgroundColor = Color.FromArgb("#6b7280"),
                TextColor = Colors.White,
                CornerRadius = 15,
                HeightRequest = 60,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 20, 0, 0)
            };

            // לחיצה על Start/Pause - מתחיל או משהה את הטיימר
            startPauseButton.Clicked += async (s, e) =>
            {
                if (!isRunning)
                {
                    isRunning = true;
                    isPaused = false;
                    startPauseButton.Text = "⏸️ Pause";
                    startPauseButton.BackgroundColor = Color.FromArgb("#f59e0b");
                    startPauseButton.TextColor = Colors.White;

                    // לולאת הטיימר - מעדכנת כל שנייה
                    while (isRunning && remainingSeconds > 0)
                    {
                        await Task.Delay(1000);
                        if (isRunning && !isPaused)
                        {
                            remainingSeconds--;
                            timeLabel.Text = FormatTime(remainingSeconds);
                            progressBar.Progress = (double)remainingSeconds / totalSeconds;

                            if (remainingSeconds == 0)
                            {
                                isRunning = false;
                                startPauseButton.Text = "✓ Done";
                                startPauseButton.BackgroundColor = Color.FromArgb("#10b981");
                                await DisplayAlert("Time's Up!", "Rest period completed!", "OK");
                            }
                        }
                    }
                }
                else
                {
                    // השהיית הטיימר
                    isPaused = true;
                    isRunning = false;
                    startPauseButton.Text = "▶️ Resume";
                    startPauseButton.BackgroundColor = Colors.White;
                    startPauseButton.TextColor = Color.FromArgb("#10b981");
                }
            };

            // לחיצה על Reset - מאפסת את הטיימר להתחלה
            resetButton.Clicked += (s, e) =>
            {
                isRunning = false;
                isPaused = false;
                remainingSeconds = totalSeconds;
                timeLabel.Text = FormatTime(remainingSeconds);
                progressBar.Progress = 1.0;
                startPauseButton.Text = "▶️ Start";
                startPauseButton.BackgroundColor = Colors.White;
                startPauseButton.TextColor = Color.FromArgb("#10b981");
            };

            // לחיצה על Close - סוגרת את הטיימר וחוזרת לדף האימון
            closeButton.Clicked += async (s, e) =>
            {
                isRunning = false;
                await Navigation.PopModalAsync();
            };

            buttonGrid.Children.Add(startPauseButton);
            Grid.SetColumn(startPauseButton, 0);

            buttonGrid.Children.Add(resetButton);
            Grid.SetColumn(resetButton, 1);

            timerStack.Children.Add(titleLabel);
            timerStack.Children.Add(timeLabel);
            timerStack.Children.Add(progressBar);
            timerStack.Children.Add(buttonGrid);
            timerStack.Children.Add(closeButton);

            timerPage.Content = timerStack;

            await Navigation.PushModalAsync(timerPage, animated: true);
        }

        // ============================================================
        // ממיר שניות לפורמט MM:SS להצגה בטיימר
        // לדוגמה: 90 → "01:30"
        // ============================================================
        private string FormatTime(int seconds)
        {
            int mins = seconds / 60;
            int secs = seconds % 60;
            return $"{mins:D2}:{secs:D2}";
        }

        // ============================================================
        // פותח תפריט עריכה לתרגיל - מאפשר החלפה, שינוי סטים/חזרות/מנוחה
        // ============================================================
        private async Task OnEditExerciseClicked(int dayIndex, int exerciseIndex)
        {
            var exercise = DataHolder.Workouts[dayIndex].Excercises[exerciseIndex];

            var action = await DisplayActionSheet(
                $"Edit: {exercise.Name}",
                "Cancel",
                null,
                "Replace Exercise",
                "Change Sets",
                "Change Reps",
                "Change Rest Time"
            );

            switch (action)
            {
                case "Replace Exercise":
                    await ReplaceExercise(dayIndex, exerciseIndex);
                    break;
                case "Change Sets":
                    await ChangeSets(dayIndex, exerciseIndex);
                    break;
                case "Change Reps":
                    await ChangeReps(dayIndex, exerciseIndex);
                    break;
                case "Change Rest Time":
                    await ChangeRestTime(dayIndex, exerciseIndex);
                    break;
            }
        }

        // ============================================================
        // מבקשת מה-API תרגיל חלופי ומחליפה את התרגיל הנוכחי
        // ============================================================
        private async Task ReplaceExercise(int dayIndex, int exerciseIndex)
        {
            var currentExercise = DataHolder.Workouts[dayIndex].Excercises[exerciseIndex];

            bool confirm = await DisplayAlert(
                "Replace Exercise",
                $"Replace '{currentExercise.Name}' with an alternative exercise?",
                "Yes",
                "No"
            );

            if (!confirm) return;

            // הצגת מסך טעינה בזמן שמחפשים תרגיל חלופי
            var loadingPage = new ContentPage
            {
                Content = new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 20,
                    Children =
                    {
                        new ActivityIndicator
                        {
                            IsRunning = true,
                            Color = Color.FromArgb("#667eea"),
                            HeightRequest = 60,
                            WidthRequest = 60
                        },
                        new Label
                        {
                            Text = "Finding alternative...",
                            FontSize = 18,
                            TextColor = Color.FromArgb("#667eea"),
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                    }
                },
                BackgroundColor = Colors.White
            };

            await Navigation.PushModalAsync(loadingPage);

            try
            {
                string url = $"{BaseUrl}/replace-exercise?exerciseName={Uri.EscapeDataString(currentExercise.Name)}";
                string json = await _apiService.GetJsonAsync(url);

                if (!string.IsNullOrEmpty(json))
                {
                    // החלפת התרגיל בנתונים ורענון התצוגה
                    var newExercise = JsonConvert.DeserializeObject<Excercise>(json);
                    DataHolder.Workouts[dayIndex].Excercises[exerciseIndex] = newExercise;
                    await SaveWorkoutToServer();   // שמירת השינוי בשרת

                    await Navigation.PopModalAsync();
                    await DisplayAlert("Success", $"Exercise replaced with {newExercise.Name}", "OK");
                    LoadWorkoutDay(currentDayIndex);
                }
                else
                {
                    await Navigation.PopModalAsync();
                    await DisplayAlert("Error", "Could not find alternative exercise", "OK");
                }
            }
            catch (Exception ex)
            {
                await Navigation.PopModalAsync();
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        // ============================================================
        // מאפשרת למשתמש לשנות את מספר הסטים לתרגיל
        // ============================================================
        private async Task ChangeSets(int dayIndex, int exerciseIndex)
        {
            var exercise = DataHolder.Workouts[dayIndex].Excercises[exerciseIndex];

            string result = await DisplayPromptAsync(
                "Change Sets",
                $"How many sets? (Current: {exercise.Sets})",
                "Save",
                "Cancel",
                keyboard: Keyboard.Numeric,
                initialValue: exercise.Sets.ToString()
            );

            if (int.TryParse(result, out int newSets) && newSets > 0)
            {
                exercise.Sets = newSets;
                await SaveWorkoutToServer();   // שמירת השינוי בשרת
                LoadWorkoutDay(currentDayIndex);
            }
        }

        // ============================================================
        // מאפשרת למשתמש לשנות את מספר החזרות לתרגיל
        // ============================================================
        private async Task ChangeReps(int dayIndex, int exerciseIndex)
        {
            var exercise = DataHolder.Workouts[dayIndex].Excercises[exerciseIndex];

            string result = await DisplayPromptAsync(
                "Change Reps",
                $"How many reps? (Current: {exercise.Reps})",
                "Save",
                "Cancel",
                keyboard: Keyboard.Numeric,
                initialValue: exercise.Reps.ToString()
            );

            if (int.TryParse(result, out int newReps) && newReps > 0)
            {
                exercise.Reps = newReps;
                await SaveWorkoutToServer();   // שמירת השינוי בשרת
                LoadWorkoutDay(currentDayIndex);
            }
        }

        // ============================================================
        // מאפשרת למשתמש לשנות את זמן המנוחה בין סטים
        // ============================================================
        private async Task ChangeRestTime(int dayIndex, int exerciseIndex)
        {
            var exercise = DataHolder.Workouts[dayIndex].Excercises[exerciseIndex];

            string result = await DisplayPromptAsync(
                "Change Rest Time",
                $"How many seconds rest? (Current: {exercise.RestTime})",
                "Save",
                "Cancel",
                keyboard: Keyboard.Numeric,
                initialValue: exercise.RestTime.ToString()
            );

            if (int.TryParse(result, out int newRestTime) && newRestTime > 0)
            {
                exercise.RestTime = newRestTime;
                await SaveWorkoutToServer();   // שמירת השינוי בשרת
                LoadWorkoutDay(currentDayIndex);
            }
        }

        // ============================================================
        // שומרת את התוכנית הנוכחית (מהזיכרון) בחזרה לשרת
        // נקראת אחרי כל שינוי של סטים/חזרות/מנוחה/תרגיל כדי שהשינוי יישמר
        // ============================================================
        private async Task SaveWorkoutToServer()
        {
            try
            {
                if (DataHolder.Workouts == null || DataHolder.Workouts.Length == 0)
                    return;

                string url = $"{BaseUrl}/save-workout?userId={UserSession.UserId}";

                // ממירים את התוכנית למבנה ה-JSON שהשרת מצפה לו (excercises עם שני e)
                var payload = DataHolder.Workouts.Select(w => new
                {
                    name = w.Name,
                    excercises = w.Excercises.Select(ex => new
                    {
                        name = ex.Name,
                        sets = ex.Sets,
                        reps = ex.Reps,
                        restTime = ex.RestTime,
                        videoLink = ex.VideoLink
                    })
                });

                string jsonContent = JsonConvert.SerializeObject(payload);
                await _apiService.PostJsonAsync(url, jsonContent);

                Console.WriteLine("[SAVE] Workout changes sent to server");
            }
            catch (Exception ex)
            {
                // שמירה שקטה - לא מטרידים את המשתמש בהודעת שגיאה על כל שינוי קטן
                Console.WriteLine($"[SAVE] Error saving workout: {ex.Message}");
            }
        }

        // ============================================================
        // יוצרת פריים קטן להצגת פרט אחד של תרגיל (סטים/מנוחה/חזרות)
        // label - שם הפרט, value - הערך להצגה
        // ============================================================
        private Frame CreateDetailFrame(string label, string value)
        {
            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 10,
                Padding = 15,
                HasShadow = true
            };

            var stack = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 5
            };

            stack.Children.Add(new Label
            {
                Text = label,
                FontSize = 12,
                TextColor = Color.FromArgb("#718096"),
                HorizontalTextAlignment = TextAlignment.Center
            });

            stack.Children.Add(new Label
            {
                Text = value,
                FontSize = 28,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#667eea"),
                HorizontalTextAlignment = TextAlignment.Center
            });

            frame.Content = stack;
            return frame;
        }
    }
}
