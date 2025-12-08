namespace WApp
{
    public static class ExerciseVideoDatabase
    {
        private static readonly Dictionary<string, string> VideoLinks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Chest Exercises
            { "bench press", "https://www.youtube.com/watch?v=rT7DgCr-3pg" },
            { "incline bench press", "https://www.youtube.com/watch?v=SrqOu55lrYU" },
            { "decline bench press", "https://www.youtube.com/watch?v=LfyQBUKR8SE" },
            { "dumbbell bench press", "https://www.youtube.com/watch?v=VmB1G1K7v94" },
            { "dumbbell fly", "https://www.youtube.com/watch?v=eozdVDA78K0" },
            { "chest fly", "https://www.youtube.com/watch?v=eozdVDA78K0" },
            { "push up", "https://www.youtube.com/watch?v=IODxDxX7oi4" },
            { "pushup", "https://www.youtube.com/watch?v=IODxDxX7oi4" },
            { "push-up", "https://www.youtube.com/watch?v=IODxDxX7oi4" },
            { "cable fly", "https://www.youtube.com/watch?v=Iwe6AmxVf7o" },
            { "dips", "https://www.youtube.com/watch?v=2z8JmcrW-As" },
            { "chest dips", "https://www.youtube.com/watch?v=2z8JmcrW-As" },

            // Back Exercises
            { "pull up", "https://www.youtube.com/watch?v=eGo4IYlbE5g" },
            { "pullup", "https://www.youtube.com/watch?v=eGo4IYlbE5g" },
            { "pull-up", "https://www.youtube.com/watch?v=eGo4IYlbE5g" },
            { "chin up", "https://www.youtube.com/watch?v=brhRXlOhkAM" },
            { "chinup", "https://www.youtube.com/watch?v=brhRXlOhkAM" },
            { "chin-up", "https://www.youtube.com/watch?v=brhRXlOhkAM" },
            { "deadlift", "https://www.youtube.com/watch?v=op9kVnSso6Q" },
            { "barbell row", "https://www.youtube.com/watch?v=FWJR5Ve8bnQ" },
            { "bent over row", "https://www.youtube.com/watch?v=FWJR5Ve8bnQ" },
            { "lat pulldown", "https://www.youtube.com/watch?v=CAwf7n6Luuc" },
            { "seated cable row", "https://www.youtube.com/watch?v=GZbfZ033f74" },
            { "cable row", "https://www.youtube.com/watch?v=GZbfZ033f74" },
            { "dumbbell row", "https://www.youtube.com/watch?v=roCP6wCXPqo" },
            { "one arm dumbbell row", "https://www.youtube.com/watch?v=roCP6wCXPqo" },
            { "t-bar row", "https://www.youtube.com/watch?v=j3Igk5dwAkQ" },

            // Shoulder Exercises
            { "overhead press", "https://www.youtube.com/watch?v=2yjwXTZQDDI" },
            { "military press", "https://www.youtube.com/watch?v=2yjwXTZQDDI" },
            { "shoulder press", "https://www.youtube.com/watch?v=qEwKCR5JCog" },
            { "dumbbell shoulder press", "https://www.youtube.com/watch?v=qEwKCR5JCog" },
            { "lateral raise", "https://www.youtube.com/watch?v=3VcKaXpzqRo" },
            { "side lateral raise", "https://www.youtube.com/watch?v=3VcKaXpzqRo" },
            { "front raise", "https://www.youtube.com/watch?v=FTDRifWHew4" },
            { "rear delt fly", "https://www.youtube.com/watch?v=EA7u4Q_8HQ0" },
            { "reverse fly", "https://www.youtube.com/watch?v=EA7u4Q_8HQ0" },
            { "upright row", "https://www.youtube.com/watch?v=nPYEz7y8vKQ" },
            { "face pull", "https://www.youtube.com/watch?v=rep-qVOkqgk" },
            { "shrugs", "https://www.youtube.com/watch?v=cJRVVxmytaM" },
            { "dumbbell shrugs", "https://www.youtube.com/watch?v=cJRVVxmytaM" },

            // Leg Exercises
            { "squat", "https://www.youtube.com/watch?v=ultWZbUMPL8" },
            { "back squat", "https://www.youtube.com/watch?v=ultWZbUMPL8" },
            { "front squat", "https://www.youtube.com/watch?v=uYumuL_G_V0" },
            { "leg press", "https://www.youtube.com/watch?v=IZxyjW7MPJQ" },
            { "leg extension", "https://www.youtube.com/watch?v=YyvSfVjQeL0" },
            { "leg curl", "https://www.youtube.com/watch?v=ELOCsoDSmrg" },
            { "hamstring curl", "https://www.youtube.com/watch?v=ELOCsoDSmrg" },
            { "lunge", "https://www.youtube.com/watch?v=QOVaHwm-Q6U" },
            { "walking lunge", "https://www.youtube.com/watch?v=L8fvypPrzzs" },
            { "bulgarian split squat", "https://www.youtube.com/watch?v=2C-uNgKwPLE" },
            { "split squat", "https://www.youtube.com/watch?v=2C-uNgKwPLE" },
            { "calf raise", "https://www.youtube.com/watch?v=JbyjNymZORE" },
            { "standing calf raise", "https://www.youtube.com/watch?v=JbyjNymZORE" },
            { "seated calf raise", "https://www.youtube.com/watch?v=JLFe7U_Z6X4" },
            { "romanian deadlift", "https://www.youtube.com/watch?v=XowKMitOVNc" },
            { "rdl", "https://www.youtube.com/watch?v=XowKMitOVNc" },

            // Arm Exercises - Biceps
            { "bicep curl", "https://www.youtube.com/watch?v=ykJmrZ5v0Oo" },
            { "barbell curl", "https://www.youtube.com/watch?v=ykJmrZ5v0Oo" },
            { "dumbbell curl", "https://www.youtube.com/watch?v=sAq_ocpRh_I" },
            { "hammer curl", "https://www.youtube.com/watch?v=zC3nLlEvin4" },
            { "preacher curl", "https://www.youtube.com/watch?v=fIWP-FRFNU0" },
            { "cable curl", "https://www.youtube.com/watch?v=FY6YkzVB0Oo" },
            { "concentration curl", "https://www.youtube.com/watch?v=Jvj2wV0vOdY" },

            // Arm Exercises - Triceps
            { "tricep extension", "https://www.youtube.com/watch?v=YbX7Wd8jQ-Q" },
            { "overhead tricep extension", "https://www.youtube.com/watch?v=YbX7Wd8jQ-Q" },
            { "tricep pushdown", "https://www.youtube.com/watch?v=2-LAMcpzODU" },
            { "cable pushdown", "https://www.youtube.com/watch?v=2-LAMcpzODU" },
            { "skull crusher", "https://www.youtube.com/watch?v=d_KZxkY_0cM" },
            { "lying tricep extension", "https://www.youtube.com/watch?v=d_KZxkY_0cM" },
            { "close grip bench press", "https://www.youtube.com/watch?v=nEF0bv2FW94" },
            { "tricep dips", "https://www.youtube.com/watch?v=6kALZikXxLc" },

            // Core/Abs Exercises
            { "plank", "https://www.youtube.com/watch?v=ASdvN_XEl_c" },
            { "crunch", "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },
            { "sit up", "https://www.youtube.com/watch?v=1fbU_MkV7NE" },
            { "situp", "https://www.youtube.com/watch?v=1fbU_MkV7NE" },
            { "sit-up", "https://www.youtube.com/watch?v=1fbU_MkV7NE" },
            { "russian twist", "https://www.youtube.com/watch?v=wkD8rjkodUI" },
            { "leg raise", "https://www.youtube.com/watch?v=JB2oyawG9KI" },
            { "hanging leg raise", "https://www.youtube.com/watch?v=JB2oyawG9KI" },
            { "bicycle crunch", "https://www.youtube.com/watch?v=9FGilxCbdz8" },
            { "mountain climber", "https://www.youtube.com/watch?v=nmwgirgXLYM" },
            { "ab wheel", "https://www.youtube.com/watch?v=EXm5BR7XsmA" },
            { "cable crunch", "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },

            // Compound/Full Body
            { "burpee", "https://www.youtube.com/watch?v=TU8QYVW0gDU" },
            { "clean and press", "https://www.youtube.com/watch?v=KwYJTpQ_x5A" },
            { "power clean", "https://www.youtube.com/watch?v=KwYJTpQ_x5A" },
            { "thruster", "https://www.youtube.com/watch?v=L219ltL15zk" },
            { "box jump", "https://www.youtube.com/watch?v=NBY9-kTuHEk" },
            { "jump squat", "https://www.youtube.com/watch?v=A-cFYWvaHr0" },
            { "kettlebell swing", "https://www.youtube.com/watch?v=YSxHifyI6s8" },

            // Glutes
            { "hip thrust", "https://www.youtube.com/watch?v=SEdqd1n0cvg" },
            { "glute bridge", "https://www.youtube.com/watch?v=wPM8icPu6H8" },
            { "cable kickback", "https://www.youtube.com/watch?v=ECV8UcVEKp4" },
            { "donkey kick", "https://www.youtube.com/watch?v=SJ1Xuz9D-ZQ" },

            // Cardio
            { "running", "https://www.youtube.com/watch?v=brFHyOtTwH4" },
            { "jump rope", "https://www.youtube.com/watch?v=hCCNM9kZ8yk" },
            { "jumping jacks", "https://www.youtube.com/watch?v=c4DAnQ6DtF8" },
            { "high knees", "https://www.youtube.com/watch?v=6BvY36TBFuo" },
        };

        public static string GetVideoUrl(string exerciseName)
        {
            if (string.IsNullOrWhiteSpace(exerciseName))
            {
                return GetDefaultSearchUrl("workout tutorial");
            }

            // Try exact match first
            if (VideoLinks.TryGetValue(exerciseName, out string url))
            {
                return url;
            }

            // Try partial match (if exercise name contains key)
            foreach (var kvp in VideoLinks)
            {
                if (exerciseName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase) ||
                    kvp.Key.Contains(exerciseName, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;
                }
            }

            // If no match found, return YouTube search
            return GetDefaultSearchUrl(exerciseName);
        }

        private static string GetDefaultSearchUrl(string exerciseName)
        {
            string searchQuery = Uri.EscapeDataString($"{exerciseName} tutorial proper form");
            return $"https://www.youtube.com/results?search_query={searchQuery}";
        }

        public static int GetTotalExercises()
        {
            return VideoLinks.Count;
        }

        public static bool HasVideo(string exerciseName)
        {
            return VideoLinks.ContainsKey(exerciseName);
        }
    }
}