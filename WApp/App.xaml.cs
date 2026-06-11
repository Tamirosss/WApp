namespace WApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        // ============================================================
        // נקרא בעת פתיחת האפליקציה - בודק אם המשתמש כבר מחובר
        // אם כן - מדלג ישר לדף הבית, אחרת מציג מסך התחברות
        // ============================================================
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // בדיקה אם יש נתוני משתמש שמורים מהפעם הקודמת
            if (Preferences.ContainsKey("UserId"))
            {
                // שחזור נתוני המשתמש מהזיכרון הקבוע לזיכרון הזמני
                UserSession.UserId = Preferences.Get("UserId", 0);
                UserSession.Username = Preferences.Get("Username", "");
                UserSession.IsLoggedIn = true;

                // מעבר ישיר לדף הבית - המשתמש כבר מחובר
                return new Window(new NavigationPage(new HomePage()));
            }

            // אין משתמש מחובר - פתיחת מסך ההתחברות
            return new Window(new NavigationPage(new LoginPage()));
        }
    }
}