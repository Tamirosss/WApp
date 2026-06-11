
using System.Text.Json;

namespace WApp
{
    public partial class GymsNearbyPage : ContentPage
    {
        private const string GoogleApiKey = "AIzaSyC3kv3bD1_HjUA3I8DLRHkAWOvLGf5wX4o";

        private const int SearchRadius = 5000; // בדיוק 5 ק"מ

        public GymsNearbyPage()
        {
            InitializeComponent();
            LoadGymsNearby();
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
                Preferences.Remove("UserId");
                Preferences.Remove("Username");

                UserSession.Username = null;
                UserSession.UserId = 0;
                UserSession.IsLoggedIn = false;
                DataHolder.Workouts = null;
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        private async void LoadGymsNearby()
        {
            try
            {
                // בדיקה אם המפתח הוגדר
                if (GoogleApiKey == "YOUR_GOOGLE_API_KEY_HERE")
                {
                    LocationStatusLabel.Text = "⚠️ Google API Key not configured";
                    LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");

                    await DisplayAlert(
                        "Setup Required",
                        "Please add your Google API Key in the code.\n\n" +
                        "You get $200 free every month!",
                        "OK"
                    );
                    return;
                }

                LocationStatusLabel.Text = "📍 Requesting location permission...";

                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status != PermissionStatus.Granted)
                {
                    LocationStatusLabel.Text = "❌ Location permission denied";
                    LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");
                    await DisplayAlert("Permission Required", "Location permission is needed to find gyms near you.", "OK");
                    return;
                }

                LocationStatusLabel.Text = "📍 Getting your location...";

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location == null)
                {
                    LocationStatusLabel.Text = "❌ Could not detect location";
                    LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");
                    await DisplayAlert("Error", "Could not get your location.", "OK");
                    return;
                }

                LocationStatusLabel.Text = $"📍 Location found: {location.Latitude:F4}, {location.Longitude:F4}";

                await SearchGymsNearby(location.Latitude, location.Longitude);
            }
            catch (Exception ex)
            {
                LocationStatusLabel.Text = $"❌ Error: {ex.Message}";
                LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");
                Console.WriteLine($"[LOCATION ERROR] {ex.Message}\n{ex.StackTrace}");
            }
        }

        private async Task SearchGymsNearby(double latitude, double longitude)
        {
            try
            {
                LocationStatusLabel.Text = "🔍 Searching for gyms nearby...";

                // חיפוש בשני שלבים - קודם type=gym ואז keyword רחב יותר
                string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
                            $"location={latitude},{longitude}" +
                            $"&radius={SearchRadius}" +
                            $"&keyword=gym+fitness+sport+center+training+athletic+bodybuilding" +
                            $"&language=en" +
                            $"&key={GoogleApiKey}";

                Console.WriteLine($"[GYMS] Searching at: {latitude}, {longitude}, Radius: {SearchRadius}m");

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[GYMS] Response Status: {response.StatusCode}");
                Console.WriteLine($"[GYMS] Response Preview: {json.Substring(0, Math.Min(300, json.Length))}");

                if (!response.IsSuccessStatusCode)
                {
                    LocationStatusLabel.Text = "❌ Search failed";
                    LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");

                    await DisplayAlert(
                        "Error",
                        $"Failed to search for gyms.\n\nStatus: {response.StatusCode}\n\n" +
                        "Make sure:\n" +
                        "1. API Key is valid\n" +
                        "2. Places API is enabled\n" +
                        "3. Billing is set up",
                        "OK"
                    );
                    return;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<GooglePlacesResponse>(json, options);

                // בדיקת שגיאות מה-API
                if (result?.status == "REQUEST_DENIED")
                {
                    LocationStatusLabel.Text = "❌ API Key denied";
                    LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");

                    await DisplayAlert(
                        "API Key Error",
                        $"Google API Key was denied.\n\n{result.error_message}",
                        "OK"
                    );
                    return;
                }

                if (result?.status == "OVER_QUERY_LIMIT")
                {
                    LocationStatusLabel.Text = "❌ Query limit exceeded";
                    LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");
                    await DisplayAlert("Quota", "Query limit exceeded. Try again tomorrow.", "OK");
                    return;
                }

                if (result?.status == "ZERO_RESULTS" || result?.results == null || result.results.Count == 0)
                {
                    LocationStatusLabel.Text = "❌ No gyms found within 5km";
                    LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");
                    await DisplayAlert("No Results", "No gyms found within 5km of your location.", "OK");
                    return;
                }

                // סינון - מסנן מכוני פיזיותרפיה ומקומות לא רלוונטיים
                var filteredGyms = result.results
                    .Where(g => !string.IsNullOrEmpty(g.name))
                    .Where(g => g.geometry?.location != null)
                    .Where(g => IsActualGym(g)) // פילטר חדש!
                    .ToList();

                // חישוב מרחק מדויק
                foreach (var gym in filteredGyms)
                {
                    double distance = CalculateDistance(
                        latitude, longitude,
                        gym.geometry.location.lat,
                        gym.geometry.location.lng
                    );
                    gym.distance = distance;
                }

                // סינון סופי - רק עד 5000 מטר בדיוק
                var gymsInRange = filteredGyms
                    .Where(g => g.distance <= SearchRadius)
                    .OrderBy(g => g.distance)
                    .ToList();

                if (gymsInRange.Count == 0)
                {
                    LocationStatusLabel.Text = "❌ No gyms found within 5km";
                    LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");
                    await DisplayAlert("No Results", "No gyms found within 5km.", "OK");
                    return;
                }

                LocationStatusLabel.Text = $"✅ Found {gymsInRange.Count} gyms within 5km";
                LocationStatusLabel.TextColor = Color.FromArgb("#10b981");

                DisplayGyms(gymsInRange);
            }
            catch (Exception ex)
            {
                LocationStatusLabel.Text = "❌ Search failed";
                LocationStatusLabel.TextColor = Color.FromArgb("#ef4444");
                Console.WriteLine($"[GYMS ERROR] {ex.Message}\n{ex.StackTrace}");
                await DisplayAlert("Error", $"Search failed: {ex.Message}", "OK");
            }
        }

        // פונקציה חדשה - מסננת מכוני פיזיותרפיה ומקומות לא רלוונטיים
        private bool IsActualGym(GooglePlace place)
        {
            string nameLower = place.name?.ToLower() ?? "";
            string vicinityLower = place.vicinity?.ToLower() ?? "";
            string combinedText = nameLower + " " + vicinityLower;

            // רשימת מילות מפתח שמעידות שזה לא חדר כושר אמיתי
            string[] excludeKeywords =
            {
                "physiotherapy", "physical therapy", "physio", "פיזיותרפיה",
                "clinic", "medical center", "doctor", "rehabilitation center",
                "hospital", "health clinic", "קליניקה", "רפואי",
                "beauty salon", "hair salon", "מכון יופי"
            };

            // בדיקה אם יש מילות מפתח לא רלוונטיות - בדיקה מחמירה יותר
            foreach (var keyword in excludeKeywords)
            {
                // רק אם זה ממש מכון פיזיותרפיה/קליניקה - לא סתם מילה בשם
                if (combinedText.Contains(keyword) && !combinedText.Contains("gym") && !combinedText.Contains("fitness"))
                {
                    Console.WriteLine($"[FILTER] Excluding: {place.name} (contains '{keyword}')");
                    return false;
                }
            }

            // מילות מפתח חיוביות - מעידות שזה חדר כושר/מרכז ספורט אמיתי
            string[] includeKeywords =
            {
                "gym", "fitness", "health club", "sport center", "sports center",
                "workout", "bodybuilding", "crossfit", "training center",
                "athletic", "weightlifting", "powerlifting",
                "כושר", "חדר כושר", "מכון כושר", "מרכז ספורט"
            };

            // אם יש מילת מפתח חיובית - זה חדר כושר
            foreach (var keyword in includeKeywords)
            {
                if (combinedText.Contains(keyword))
                {
                    Console.WriteLine($"[FILTER] Including: {place.name} (contains '{keyword}')");
                    return true;
                }
            }

            // אם השם מכיל "sport" או "athletic" - סביר שזה חדר כושר
            if (nameLower.Contains("sport") || nameLower.Contains("athletic"))
            {
                Console.WriteLine($"[FILTER] Including: {place.name} (sport/athletic in name)");
                return true;
            }

            // במקרה של ספק - נכלול (כדי לא לפספס חדרי כושר)
            Console.WriteLine($"[FILTER] Including by default: {place.name}");
            return true;
        }

        private void DisplayGyms(List<GooglePlace> gyms)
        {
            foreach (var gym in gyms)
            {
                var gymFrame = CreateGymCard(gym);
                GymsContainer.Children.Add(gymFrame);
            }
        }

        private Frame CreateGymCard(GooglePlace gym)
        {
            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 15,
                Padding = 20,
                HasShadow = true,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var stack = new VerticalStackLayout
            {
                Spacing = 12
            };

            // שם חדר הכושר
            var nameLabel = new Label
            {
                Text = $"🏋️ {gym.name}",
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2d3748")
            };
            stack.Children.Add(nameLabel);

            // מרחק
            string distanceText = gym.distance < 1000
                ? $"📏 {gym.distance:F0}m away"
                : $"📏 {(gym.distance / 1000):F2}km away";

            var distanceLabel = new Label
            {
                Text = distanceText,
                FontSize = 16,
                TextColor = Color.FromArgb("#667eea"),
                FontAttributes = FontAttributes.Bold
            };
            stack.Children.Add(distanceLabel);

            // כתובת
            if (!string.IsNullOrEmpty(gym.vicinity))
            {
                var addressLabel = new Label
                {
                    Text = $"📍 {gym.vicinity}",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#718096")
                };
                stack.Children.Add(addressLabel);
            }

            // דירוג וכוכבים
            if (gym.rating > 0)
            {
                var ratingGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    ColumnSpacing = 10
                };

                var stars = GetStarsString(gym.rating);
                var starsLabel = new Label
                {
                    Text = stars,
                    FontSize = 20,
                    VerticalOptions = LayoutOptions.Center
                };
                ratingGrid.Children.Add(starsLabel);
                Grid.SetColumn(starsLabel, 0);

                var ratingLabel = new Label
                {
                    Text = $"{gym.rating:F1} ({gym.user_ratings_total ?? 0} reviews)",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#f59e0b"),
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center
                };
                ratingGrid.Children.Add(ratingLabel);
                Grid.SetColumn(ratingLabel, 1);

                stack.Children.Add(ratingGrid);
            }
            else
            {
                var noRatingLabel = new Label
                {
                    Text = "⭐ No ratings yet",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#9ca3af")
                };
                stack.Children.Add(noRatingLabel);
            }

            // סטטוס פתוח/סגור
            if (gym.opening_hours != null)
            {
                var statusLabel = new Label
                {
                    Text = gym.opening_hours.open_now ? "🟢 Open now" : "🔴 Closed",
                    FontSize = 15,
                    TextColor = gym.opening_hours.open_now ?
                        Color.FromArgb("#10b981") : Color.FromArgb("#ef4444"),
                    FontAttributes = FontAttributes.Bold
                };
                stack.Children.Add(statusLabel);
            }

            // קו מפריד
            var separator = new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = Color.FromArgb("#e5e7eb"),
                Margin = new Thickness(0, 10, 0, 10)
            };
            stack.Children.Add(separator);

            // כפתורי ניווט
            var buttonGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                ColumnSpacing = 10
            };

            // Google Maps
            var mapsButton = new Button
            {
                Text = "🗺️ Google Maps",
                BackgroundColor = Color.FromArgb("#34a853"),
                TextColor = Colors.White,
                CornerRadius = 10,
                HeightRequest = 50,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold
            };

            mapsButton.Clicked += async (s, e) =>
            {
                await OpenInGoogleMaps(gym);
            };

            buttonGrid.Children.Add(mapsButton);
            Grid.SetColumn(mapsButton, 0);

            // Waze
            var wazeButton = new Button
            {
                Text = "🚗 Waze",
                BackgroundColor = Color.FromArgb("#33ccff"),
                TextColor = Colors.White,
                CornerRadius = 10,
                HeightRequest = 50,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold
            };

            wazeButton.Clicked += async (s, e) =>
            {
                await OpenInWaze(gym);
            };

            buttonGrid.Children.Add(wazeButton);
            Grid.SetColumn(wazeButton, 1);

            stack.Children.Add(buttonGrid);

            frame.Content = stack;
            return frame;
        }

        private string GetStarsString(double rating)
        {
            int fullStars = (int)Math.Floor(rating);
            bool halfStar = (rating - fullStars) >= 0.5;
            int emptyStars = 5 - fullStars - (halfStar ? 1 : 0);

            string stars = "";
            for (int i = 0; i < fullStars; i++) stars += "⭐";
            if (halfStar) stars += "⭐";
            for (int i = 0; i < emptyStars; i++) stars += "☆";

            return stars;
        }

        private async Task OpenInGoogleMaps(GooglePlace gym)
        {
            try
            {
                if (gym.geometry?.location == null)
                {
                    await DisplayAlert("Error", "No location data available", "OK");
                    return;
                }

                double lat = gym.geometry.location.lat;
                double lon = gym.geometry.location.lng;

                // Google Maps URL עם place_id (הכי מדויק!)
                string url = $"https://www.google.com/maps/search/?api=1&query={lat},{lon}&query_place_id={gym.place_id}";

                Console.WriteLine($"[MAPS] Opening: {url}");

                bool opened = await Launcher.OpenAsync(url);

                if (!opened)
                {
                    await DisplayAlert("Error", "Could not open Google Maps", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MAPS ERROR] {ex.Message}");
                await DisplayAlert("Error", $"Could not open navigation: {ex.Message}", "OK");
            }
        }

        private async Task OpenInWaze(GooglePlace gym)
        {
            try
            {
                if (gym.geometry?.location == null)
                {
                    await DisplayAlert("Error", "No location data available", "OK");
                    return;
                }

                double lat = gym.geometry.location.lat;
                double lon = gym.geometry.location.lng;

                string url = $"https://waze.com/ul?ll={lat},{lon}&navigate=yes&zoom=17";

                Console.WriteLine($"[WAZE] Opening: {url}");

                bool opened = await Launcher.OpenAsync(url);

                if (!opened)
                {
                    await DisplayAlert("Error", "Could not open Waze", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WAZE ERROR] {ex.Message}");
                await DisplayAlert("Error", $"Could not open Waze: {ex.Message}", "OK");
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // נוסחת Haversine - חישוב מרחק מדויק
            const double R = 6371000; // רדיוס כדור הארץ במטרים

            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // מרחק במטרים
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }

    // Google Places API Response Models
    public class GooglePlacesResponse
    {
        public List<GooglePlace> results { get; set; }
        public string status { get; set; }
        public string error_message { get; set; }
        public string next_page_token { get; set; }
    }

    public class GooglePlace
    {
        public string name { get; set; }
        public string vicinity { get; set; }
        public double rating { get; set; }
        public int? user_ratings_total { get; set; }
        public PlaceGeometry geometry { get; set; }
        public OpeningHours opening_hours { get; set; }
        public string place_id { get; set; }
        public double distance { get; set; } // נחשב בעצמנו
    }

    public class PlaceGeometry
    {
        public PlaceLocation location { get; set; }
    }

    public class PlaceLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class OpeningHours
    {
        public bool open_now { get; set; }
    }
}