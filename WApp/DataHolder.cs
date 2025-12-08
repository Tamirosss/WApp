using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WApp
{
    public class Excercise
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sets")]
        public int Sets { get; set; }

        [JsonProperty("reps")]
        public int Reps { get; set; }

        [JsonProperty("restTime")]
        public int RestTime { get; set; }

        [JsonProperty("videoLink")]
        public string VideoLink { get; set; }
    }

    public class Workout
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("excercises")]
        public Excercise[] Excercises { get; set; }
    }

    public static class DataHolder
    {
        public static Workout[] Workouts { get; set; }
    }
}