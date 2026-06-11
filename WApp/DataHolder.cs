using Newtonsoft.Json;

namespace WApp
{
    // ============================================================
    // מייצג תרגיל בודד בתוכנית האימון
    // ============================================================
    public class Excercise
    {
        [JsonProperty("name")]
        public string Name { get; set; }        // שם התרגיל

        [JsonProperty("sets")]
        public int Sets { get; set; }           // מספר סטים

        [JsonProperty("reps")]
        public int Reps { get; set; }           // מספר חזרות בכל סט

        [JsonProperty("restTime")]
        public int RestTime { get; set; }       // זמן מנוחה בשניות בין סטים

        [JsonProperty("videoLink")]
        public string VideoLink { get; set; }   // שם התרגיל לחיפוש סרטון
    }

    // ============================================================
    // מייצג יום אימון אחד המכיל רשימת תרגילים
    // ============================================================
    public class Workout
    {
        [JsonProperty("name")]
        public string Name { get; set; }                // שם יום האימון (למשל: "Push Day")

        [JsonProperty("excercises")]
        public Excercise[] Excercises { get; set; }     // מערך התרגילים של היום
    }

    // ============================================================
    // מחזיק סטטי - שומר את תוכנית האימון הנוכחית בזיכרון
    // נגיש מכל מקום באפליקציה ללא צורך להעביר פרמטרים
    // ============================================================
    public static class DataHolder
    {
        // מערך כל ימי האימון בתוכנית הנוכחית
        public static Workout[] Workouts { get; set; }
    }
}
