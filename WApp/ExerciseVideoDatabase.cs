using System;
using System.Collections.Generic;
using System.Linq;

namespace WApp
{
    public static class ExerciseVideoDatabase
    {
        // ============================================================
        // מאגר קישורי סרטוני הדגמה לתרגילים
        // כל קישור נבדק ידנית - אין כפילויות!
        // החיפוש לא תלוי רישיות (case-insensitive)
        // ============================================================
        private static readonly Dictionary<string, string> VideoLinks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // ============================================================
            // חזה - Chest
            // ============================================================
            { "bench press",                        "https://www.youtube.com/watch?v=rT7DgCr-3pg" },
            { "barbell bench press",                "https://www.youtube.com/watch?v=rT7DgCr-3pg" },
            { "incline bench press",                "https://www.youtube.com/watch?v=SrqOu55lrYU" },
            { "incline barbell bench press",        "https://www.youtube.com/watch?v=SrqOu55lrYU" },
            { "decline bench press",                "https://www.youtube.com/watch?v=LfyQBUKR8SE" },
            { "decline barbell bench press",        "https://www.youtube.com/watch?v=LfyQBUKR8SE" },
            { "dumbbell bench press",               "https://www.youtube.com/watch?v=VmB1G1K7v94" },
            { "incline dumbbell bench press",       "https://www.youtube.com/watch?v=8iPEnn-ltC8" },
            { "decline dumbbell bench press",       "https://www.youtube.com/watch?v=LfyQBUKR8SE" },
            { "dumbbell fly",                       "https://www.youtube.com/watch?v=eozdVDA78K0" },
            { "dumbbell flye",                      "https://www.youtube.com/watch?v=eozdVDA78K0" },
            { "chest fly",                          "https://www.youtube.com/watch?v=eozdVDA78K0" },
            { "incline dumbbell fly",               "https://www.youtube.com/watch?v=eozdVDA78K0" },
            { "push up",                            "https://www.youtube.com/watch?v=IODxDxX7oi4" },
            { "pushup",                             "https://www.youtube.com/watch?v=IODxDxX7oi4" },
            { "push-up",                            "https://www.youtube.com/watch?v=IODxDxX7oi4" },
            { "wide grip push up",                  "https://www.youtube.com/watch?v=IODxDxX7oi4" },
            { "diamond push up",                    "https://www.youtube.com/watch?v=J0DnG1_S92I" },
            { "decline push up",                    "https://www.youtube.com/watch?v=SKPab2YC8BE" },
            { "incline push up",                    "https://www.youtube.com/watch?v=cfns5pCERHo" },
            { "cable fly",                          "https://www.youtube.com/watch?v=Iwe6AmxVf7o" },
            { "cable crossover",                    "https://www.youtube.com/watch?v=taI4XduLpTk" },
            { "low cable fly",                      "https://www.youtube.com/watch?v=Iwe6AmxVf7o" },
            { "high cable fly",                     "https://www.youtube.com/watch?v=Iwe6AmxVf7o" },
            { "dips",                               "https://www.youtube.com/watch?v=2z8JmcrW-As" },
            { "chest dips",                         "https://www.youtube.com/watch?v=2z8JmcrW-As" },
            { "pec deck",                           "https://www.youtube.com/watch?v=Z57CtFmRMxA" },
            { "pec deck fly",                       "https://www.youtube.com/watch?v=Z57CtFmRMxA" },
            { "machine chest fly",                  "https://www.youtube.com/watch?v=Z57CtFmRMxA" },
            { "chest press machine",                "https://www.youtube.com/watch?v=xUm0BiZCWlQ" },
            { "machine chest press",                "https://www.youtube.com/watch?v=xUm0BiZCWlQ" },
            { "svend press",                        "https://www.youtube.com/watch?v=5bDkB9kpHHM" },
            { "landmine press",                     "https://www.youtube.com/watch?v=kNDRtDSrMsA" },
            { "smith machine bench press",          "https://www.youtube.com/watch?v=rT7DgCr-3pg" },
            { "floor press",                        "https://www.youtube.com/watch?v=uUGDRwge4F8" },
            { "dumbbell pullover",                  "https://www.youtube.com/watch?v=FK4rHfxTSAk" },

            // ============================================================
            // גב - Back
            // ============================================================
            { "pull up",                            "https://www.youtube.com/watch?v=eGo4IYlbE5g" },
            { "pullup",                             "https://www.youtube.com/watch?v=eGo4IYlbE5g" },
            { "pull-up",                            "https://www.youtube.com/watch?v=eGo4IYlbE5g" },
            { "wide grip pull up",                  "https://www.youtube.com/watch?v=eGo4IYlbE5g" },
            { "chin up",                            "https://www.youtube.com/watch?v=brhRXlOhkAM" },
            { "chinup",                             "https://www.youtube.com/watch?v=brhRXlOhkAM" },
            { "chin-up",                            "https://www.youtube.com/watch?v=brhRXlOhkAM" },
            { "neutral grip pull up",               "https://www.youtube.com/watch?v=brhRXlOhkAM" },
            { "deadlift",                           "https://www.youtube.com/watch?v=op9kVnSso6Q" },
            { "conventional deadlift",              "https://www.youtube.com/watch?v=op9kVnSso6Q" },
            { "sumo deadlift",                      "https://www.youtube.com/watch?v=56oNXcRLqhc" },
            { "trap bar deadlift",                  "https://www.youtube.com/watch?v=JZKvZpQCMl0" },
            { "hex bar deadlift",                   "https://www.youtube.com/watch?v=JZKvZpQCMl0" },
            { "barbell row",                        "https://www.youtube.com/watch?v=FWJR5Ve8bnQ" },
            { "bent over row",                      "https://www.youtube.com/watch?v=FWJR5Ve8bnQ" },
            { "bent over barbell row",              "https://www.youtube.com/watch?v=FWJR5Ve8bnQ" },
            { "pendlay row",                        "https://www.youtube.com/watch?v=FWJR5Ve8bnQ" },
            { "lat pulldown",                       "https://www.youtube.com/watch?v=CAwf7n6Luuc" },
            { "wide grip lat pulldown",             "https://www.youtube.com/watch?v=CAwf7n6Luuc" },
            { "close grip lat pulldown",            "https://www.youtube.com/watch?v=CAwf7n6Luuc" },
            { "reverse grip lat pulldown",          "https://www.youtube.com/watch?v=CAwf7n6Luuc" },
            { "seated cable row",                   "https://www.youtube.com/watch?v=GZbfZ033f74" },
            { "cable row",                          "https://www.youtube.com/watch?v=GZbfZ033f74" },
            { "close grip cable row",               "https://www.youtube.com/watch?v=GZbfZ033f74" },
            { "wide grip cable row",                "https://www.youtube.com/watch?v=GZbfZ033f74" },
            { "dumbbell row",                       "https://www.youtube.com/watch?v=roCP6wCXPqo" },
            { "one arm dumbbell row",               "https://www.youtube.com/watch?v=roCP6wCXPqo" },
            { "single arm dumbbell row",            "https://www.youtube.com/watch?v=roCP6wCXPqo" },
            { "t-bar row",                          "https://www.youtube.com/watch?v=j3Igk5dwAkQ" },
            { "t bar row",                          "https://www.youtube.com/watch?v=j3Igk5dwAkQ" },
            { "chest supported row",                "https://www.youtube.com/watch?v=xQNrFHEMhI4" },
            { "chest supported dumbbell row",       "https://www.youtube.com/watch?v=xQNrFHEMhI4" },
            { "machine row",                        "https://www.youtube.com/watch?v=xQNrFHEMhI4" },
            { "straight arm pulldown",              "https://www.youtube.com/watch?v=TFMqHDDFPpI" },
            { "straight arm cable pulldown",        "https://www.youtube.com/watch?v=TFMqHDDFPpI" },
            { "back extension",                     "https://www.youtube.com/watch?v=ph3pddpKzzw" },
            { "hyperextension",                     "https://www.youtube.com/watch?v=ph3pddpKzzw" },
            { "good morning",                       "https://www.youtube.com/watch?v=YA-h3n9L4YU" },
            { "rack pull",                          "https://www.youtube.com/watch?v=PkBxCRFyVqU" },
            { "meadows row",                        "https://www.youtube.com/watch?v=GWDFsLjENrg" },
            { "seal row",                           "https://www.youtube.com/watch?v=xQNrFHEMhI4" },
            { "cable pullover",                     "https://www.youtube.com/watch?v=TFMqHDDFPpI" },
            { "inverted row",                       "https://www.youtube.com/watch?v=LR3-za629j4" },
            { "australian pull up",                 "https://www.youtube.com/watch?v=LR3-za629j4" },

            // ============================================================
            // כתפיים - Shoulders
            // ============================================================
            { "overhead press",                     "https://www.youtube.com/watch?v=2yjwXTZQDDI" },
            { "barbell overhead press",             "https://www.youtube.com/watch?v=2yjwXTZQDDI" },
            { "military press",                     "https://www.youtube.com/watch?v=2yjwXTZQDDI" },
            { "seated military press",              "https://www.youtube.com/watch?v=2yjwXTZQDDI" },
            { "shoulder press",                     "https://www.youtube.com/watch?v=qEwKCR5JCog" },
            { "dumbbell shoulder press",            "https://www.youtube.com/watch?v=qEwKCR5JCog" },
            { "seated dumbbell shoulder press",     "https://www.youtube.com/watch?v=qEwKCR5JCog" },
            { "arnold press",                       "https://www.youtube.com/watch?v=6Z15_WdXmVw" },
            { "arnold dumbbell press",              "https://www.youtube.com/watch?v=6Z15_WdXmVw" },
            { "lateral raise",                      "https://www.youtube.com/watch?v=3VcKaXpzqRo" },
            { "side lateral raise",                 "https://www.youtube.com/watch?v=3VcKaXpzqRo" },
            { "dumbbell lateral raise",             "https://www.youtube.com/watch?v=3VcKaXpzqRo" },
            { "cable lateral raise",                "https://www.youtube.com/watch?v=3VcKaXpzqRo" },
            { "front raise",                        "https://www.youtube.com/watch?v=FTDRifWHew4" },
            { "dumbbell front raise",               "https://www.youtube.com/watch?v=FTDRifWHew4" },
            { "plate front raise",                  "https://www.youtube.com/watch?v=FTDRifWHew4" },
            { "cable front raise",                  "https://www.youtube.com/watch?v=FTDRifWHew4" },
            { "rear delt fly",                      "https://www.youtube.com/watch?v=EA7u4Q_8HQ0" },
            { "reverse fly",                        "https://www.youtube.com/watch?v=EA7u4Q_8HQ0" },
            { "bent over reverse fly",              "https://www.youtube.com/watch?v=EA7u4Q_8HQ0" },
            { "rear delt raise",                    "https://www.youtube.com/watch?v=EA7u4Q_8HQ0" },
            { "upright row",                        "https://www.youtube.com/watch?v=nPYEz7y8vKQ" },
            { "barbell upright row",                "https://www.youtube.com/watch?v=nPYEz7y8vKQ" },
            { "dumbbell upright row",               "https://www.youtube.com/watch?v=nPYEz7y8vKQ" },
            { "face pull",                          "https://www.youtube.com/watch?v=rep-qVOkqgk" },
            { "cable face pull",                    "https://www.youtube.com/watch?v=rep-qVOkqgk" },
            { "shrugs",                             "https://www.youtube.com/watch?v=cJRVVxmytaM" },
            { "barbell shrugs",                     "https://www.youtube.com/watch?v=cJRVVxmytaM" },
            { "dumbbell shrugs",                    "https://www.youtube.com/watch?v=cJRVVxmytaM" },
            { "machine shoulder press",             "https://www.youtube.com/watch?v=WvLMauqrnK8" },
            { "smith machine shoulder press",       "https://www.youtube.com/watch?v=WvLMauqrnK8" },
            { "push press",                         "https://www.youtube.com/watch?v=iaBVSJm78ko" },
            { "z press",                            "https://www.youtube.com/watch?v=PbkKVrEE6_A" },
            { "behind the neck press",              "https://www.youtube.com/watch?v=2yjwXTZQDDI" },
            { "cable upright row",                  "https://www.youtube.com/watch?v=nPYEz7y8vKQ" },

            // ============================================================
            // רגליים - Legs
            // ============================================================
            { "squat",                              "https://www.youtube.com/watch?v=ultWZbUMPL8" },
            { "back squat",                         "https://www.youtube.com/watch?v=ultWZbUMPL8" },
            { "barbell squat",                      "https://www.youtube.com/watch?v=ultWZbUMPL8" },
            { "front squat",                        "https://www.youtube.com/watch?v=uYumuL_G_V0" },
            { "goblet squat",                       "https://www.youtube.com/watch?v=MxsFDhcyFyE" },
            { "sumo squat",                         "https://www.youtube.com/watch?v=sSESeQAir2M" },
            { "pause squat",                        "https://www.youtube.com/watch?v=ultWZbUMPL8" },
            { "box squat",                          "https://www.youtube.com/watch?v=8IKNRsxvSzQ" },
            { "hack squat",                         "https://www.youtube.com/watch?v=EdtPTSdRDkM" },
            { "smith machine squat",                "https://www.youtube.com/watch?v=ZtMn_fEBqrA" },
            { "leg press",                          "https://www.youtube.com/watch?v=IZxyjW7MPJQ" },
            { "45 degree leg press",                "https://www.youtube.com/watch?v=IZxyjW7MPJQ" },
            { "single leg press",                   "https://www.youtube.com/watch?v=IZxyjW7MPJQ" },
            { "leg extension",                      "https://www.youtube.com/watch?v=YyvSfVjQeL0" },
            { "machine leg extension",              "https://www.youtube.com/watch?v=YyvSfVjQeL0" },
            { "leg curl",                           "https://www.youtube.com/watch?v=ELOCsoDSmrg" },
            { "lying leg curl",                     "https://www.youtube.com/watch?v=ELOCsoDSmrg" },
            { "seated leg curl",                    "https://www.youtube.com/watch?v=ELOCsoDSmrg" },
            { "hamstring curl",                     "https://www.youtube.com/watch?v=ELOCsoDSmrg" },
            { "nordic curl",                        "https://www.youtube.com/watch?v=d8AAPcAFMOE" },
            { "nordic hamstring curl",              "https://www.youtube.com/watch?v=d8AAPcAFMOE" },
            { "lunge",                              "https://www.youtube.com/watch?v=QOVaHwm-Q6U" },
            { "barbell lunge",                      "https://www.youtube.com/watch?v=QOVaHwm-Q6U" },
            { "dumbbell lunge",                     "https://www.youtube.com/watch?v=QOVaHwm-Q6U" },
            { "reverse lunge",                      "https://www.youtube.com/watch?v=xrPteyQLGAo" },
            { "walking lunge",                      "https://www.youtube.com/watch?v=L8fvypPrzzs" },
            { "lateral lunge",                      "https://www.youtube.com/watch?v=k8JVHVeWUSI" },
            { "side lunge",                         "https://www.youtube.com/watch?v=k8JVHVeWUSI" },
            { "bulgarian split squat",              "https://www.youtube.com/watch?v=2C-uNgKwPLE" },
            { "split squat",                        "https://www.youtube.com/watch?v=2C-uNgKwPLE" },
            { "rear foot elevated split squat",     "https://www.youtube.com/watch?v=2C-uNgKwPLE" },
            { "calf raise",                         "https://www.youtube.com/watch?v=JbyjNymZORE" },
            { "standing calf raise",                "https://www.youtube.com/watch?v=JbyjNymZORE" },
            { "seated calf raise",                  "https://www.youtube.com/watch?v=JLFe7U_Z6X4" },
            { "donkey calf raise",                  "https://www.youtube.com/watch?v=JbyjNymZORE" },
            { "single leg calf raise",              "https://www.youtube.com/watch?v=JbyjNymZORE" },
            { "romanian deadlift",                  "https://www.youtube.com/watch?v=XowKMitOVNc" },
            { "rdl",                                "https://www.youtube.com/watch?v=XowKMitOVNc" },
            { "dumbbell romanian deadlift",         "https://www.youtube.com/watch?v=XowKMitOVNc" },
            { "single leg romanian deadlift",       "https://www.youtube.com/watch?v=XowKMitOVNc" },
            { "stiff leg deadlift",                 "https://www.youtube.com/watch?v=XowKMitOVNc" },
            { "step up",                            "https://www.youtube.com/watch?v=dQqApCGd5Ss" },
            { "dumbbell step up",                   "https://www.youtube.com/watch?v=dQqApCGd5Ss" },
            { "box step up",                        "https://www.youtube.com/watch?v=dQqApCGd5Ss" },
            { "sissy squat",                        "https://www.youtube.com/watch?v=ZBCaG_GXAzk" },
            { "wall sit",                           "https://www.youtube.com/watch?v=y-wV4Venusw" },
            { "leg press calf raise",               "https://www.youtube.com/watch?v=JbyjNymZORE" },
            { "glute ham raise",                    "https://www.youtube.com/watch?v=d8AAPcAFMOE" },
            { "ghr",                                "https://www.youtube.com/watch?v=d8AAPcAFMOE" },

            // ============================================================
            // יד קדמית - Biceps
            // ============================================================
            { "bicep curl",                         "https://www.youtube.com/watch?v=ykJmrZ5v0Oo" },
            { "biceps curl",                        "https://www.youtube.com/watch?v=ykJmrZ5v0Oo" },
            { "barbell curl",                       "https://www.youtube.com/watch?v=ykJmrZ5v0Oo" },
            { "ez bar curl",                        "https://www.youtube.com/watch?v=zG2NXrbflFQ" },
            { "ez curl",                            "https://www.youtube.com/watch?v=zG2NXrbflFQ" },
            { "dumbbell curl",                      "https://www.youtube.com/watch?v=sAq_ocpRh_I" },
            { "alternating dumbbell curl",          "https://www.youtube.com/watch?v=sAq_ocpRh_I" },
            { "hammer curl",                        "https://www.youtube.com/watch?v=zC3nLlEvin4" },
            { "dumbbell hammer curl",               "https://www.youtube.com/watch?v=zC3nLlEvin4" },
            { "cross body hammer curl",             "https://www.youtube.com/watch?v=zC3nLlEvin4" },
            { "preacher curl",                      "https://www.youtube.com/watch?v=fIWP-FRFNU0" },
            { "ez bar preacher curl",               "https://www.youtube.com/watch?v=fIWP-FRFNU0" },
            { "dumbbell preacher curl",             "https://www.youtube.com/watch?v=fIWP-FRFNU0" },
            { "cable curl",                         "https://www.youtube.com/watch?v=FY6YkzVB0Oo" },
            { "cable bicep curl",                   "https://www.youtube.com/watch?v=FY6YkzVB0Oo" },
            { "rope cable curl",                    "https://www.youtube.com/watch?v=FY6YkzVB0Oo" },
            { "concentration curl",                 "https://www.youtube.com/watch?v=Jvj2wV0vOdY" },
            { "incline dumbbell curl",              "https://www.youtube.com/watch?v=soxrZlIl35U" },
            { "spider curl",                        "https://www.youtube.com/watch?v=fF7KSeMkRL4" },
            { "reverse curl",                       "https://www.youtube.com/watch?v=nkMJZYiZkJk" },
            { "reverse barbell curl",               "https://www.youtube.com/watch?v=nkMJZYiZkJk" },
            { "zottman curl",                       "https://www.youtube.com/watch?v=ZrFZJ8sSQQI" },
            { "machine bicep curl",                 "https://www.youtube.com/watch?v=ykJmrZ5v0Oo" },
            { "high cable curl",                    "https://www.youtube.com/watch?v=FY6YkzVB0Oo" },

            // ============================================================
            // יד אחורית - Triceps
            // ============================================================
            { "tricep extension",                   "https://www.youtube.com/watch?v=YbX7Wd8jQ-Q" },
            { "triceps extension",                  "https://www.youtube.com/watch?v=YbX7Wd8jQ-Q" },
            { "overhead tricep extension",          "https://www.youtube.com/watch?v=YbX7Wd8jQ-Q" },
            { "dumbbell overhead tricep extension", "https://www.youtube.com/watch?v=YbX7Wd8jQ-Q" },
            { "cable overhead tricep extension",    "https://www.youtube.com/watch?v=YbX7Wd8jQ-Q" },
            { "tricep pushdown",                    "https://www.youtube.com/watch?v=2-LAMcpzODU" },
            { "triceps pushdown",                   "https://www.youtube.com/watch?v=2-LAMcpzODU" },
            { "cable pushdown",                     "https://www.youtube.com/watch?v=2-LAMcpzODU" },
            { "rope pushdown",                      "https://www.youtube.com/watch?v=2-LAMcpzODU" },
            { "rope tricep pushdown",               "https://www.youtube.com/watch?v=2-LAMcpzODU" },
            { "v bar pushdown",                     "https://www.youtube.com/watch?v=2-LAMcpzODU" },
            { "skull crusher",                      "https://www.youtube.com/watch?v=d_KZxkY_0cM" },
            { "skullcrusher",                       "https://www.youtube.com/watch?v=d_KZxkY_0cM" },
            { "lying tricep extension",             "https://www.youtube.com/watch?v=d_KZxkY_0cM" },
            { "ez bar skull crusher",               "https://www.youtube.com/watch?v=d_KZxkY_0cM" },
            { "close grip bench press",             "https://www.youtube.com/watch?v=nEF0bv2FW94" },
            { "close grip barbell bench press",     "https://www.youtube.com/watch?v=nEF0bv2FW94" },
            { "tricep dips",                        "https://www.youtube.com/watch?v=6kALZikXxLc" },
            { "bench dips",                         "https://www.youtube.com/watch?v=6kALZikXxLc" },
            { "tricep kickback",                    "https://www.youtube.com/watch?v=PQdLuBAiA0I" },
            { "dumbbell tricep kickback",           "https://www.youtube.com/watch?v=PQdLuBAiA0I" },
            { "cable tricep kickback",              "https://www.youtube.com/watch?v=PQdLuBAiA0I" },
            { "jm press",                           "https://www.youtube.com/watch?v=VwJPFFKq1Tg" },
            { "tate press",                         "https://www.youtube.com/watch?v=VwJPFFKq1Tg" },
            { "machine tricep extension",           "https://www.youtube.com/watch?v=YbX7Wd8jQ-Q" },
            { "single arm tricep pushdown",         "https://www.youtube.com/watch?v=2-LAMcpzODU" },

            // ============================================================
            // בטן - Core / Abs
            // ============================================================
            { "plank",                              "https://www.youtube.com/watch?v=ASdvN_XEl_c" },
            { "forearm plank",                      "https://www.youtube.com/watch?v=ASdvN_XEl_c" },
            { "side plank",                         "https://www.youtube.com/watch?v=K-CrEi0ymMg" },
            { "plank with shoulder tap",            "https://www.youtube.com/watch?v=LEZq7QlMMqE" },
            { "crunch",                             "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },
            { "crunches",                           "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },
            { "sit up",                             "https://www.youtube.com/watch?v=1fbU_MkV7NE" },
            { "situp",                              "https://www.youtube.com/watch?v=1fbU_MkV7NE" },
            { "sit-up",                             "https://www.youtube.com/watch?v=1fbU_MkV7NE" },
            { "decline sit up",                     "https://www.youtube.com/watch?v=1fbU_MkV7NE" },
            { "russian twist",                      "https://www.youtube.com/watch?v=wkD8rjkodUI" },
            { "weighted russian twist",             "https://www.youtube.com/watch?v=wkD8rjkodUI" },
            { "leg raise",                          "https://www.youtube.com/watch?v=JB2oyawG9KI" },
            { "lying leg raise",                    "https://www.youtube.com/watch?v=JB2oyawG9KI" },
            { "hanging leg raise",                  "https://www.youtube.com/watch?v=JB2oyawG9KI" },
            { "hanging knee raise",                 "https://www.youtube.com/watch?v=JB2oyawG9KI" },
            { "bicycle crunch",                     "https://www.youtube.com/watch?v=9FGilxCbdz8" },
            { "bicycle crunches",                   "https://www.youtube.com/watch?v=9FGilxCbdz8" },
            { "mountain climber",                   "https://www.youtube.com/watch?v=nmwgirgXLYM" },
            { "mountain climbers",                  "https://www.youtube.com/watch?v=nmwgirgXLYM" },
            { "ab wheel",                           "https://www.youtube.com/watch?v=EXm5BR7XsmA" },
            { "ab wheel rollout",                   "https://www.youtube.com/watch?v=EXm5BR7XsmA" },
            { "ab rollout",                         "https://www.youtube.com/watch?v=EXm5BR7XsmA" },
            { "cable crunch",                       "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },
            { "cable ab crunch",                    "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },
            { "dragon flag",                        "https://www.youtube.com/watch?v=njKXkuhY7p8" },
            { "v up",                               "https://www.youtube.com/watch?v=iP2fjvG0g3w" },
            { "v-up",                               "https://www.youtube.com/watch?v=iP2fjvG0g3w" },
            { "toe touch",                          "https://www.youtube.com/watch?v=iP2fjvG0g3w" },
            { "windshield wiper",                   "https://www.youtube.com/watch?v=OO4VrOHqATo" },
            { "windshield wipers",                  "https://www.youtube.com/watch?v=OO4VrOHqATo" },
            { "hollow body hold",                   "https://www.youtube.com/watch?v=LlDNef_Ztsc" },
            { "dead bug",                           "https://www.youtube.com/watch?v=4XLEnwUr1d8" },
            { "pallof press",                       "https://www.youtube.com/watch?v=AaBSByMFOZg" },
            { "reverse crunch",                     "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },
            { "ab crunch machine",                  "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },
            { "flutter kicks",                      "https://www.youtube.com/watch?v=JB2oyawG9KI" },
            { "scissor kicks",                      "https://www.youtube.com/watch?v=JB2oyawG9KI" },

            // ============================================================
            // ישבן - Glutes
            // ============================================================
            { "hip thrust",                         "https://www.youtube.com/watch?v=SEdqd1n0cvg" },
            { "barbell hip thrust",                 "https://www.youtube.com/watch?v=SEdqd1n0cvg" },
            { "dumbbell hip thrust",                "https://www.youtube.com/watch?v=SEdqd1n0cvg" },
            { "banded hip thrust",                  "https://www.youtube.com/watch?v=SEdqd1n0cvg" },
            { "glute bridge",                       "https://www.youtube.com/watch?v=wPM8icPu6H8" },
            { "single leg glute bridge",            "https://www.youtube.com/watch?v=wPM8icPu6H8" },
            { "cable kickback",                     "https://www.youtube.com/watch?v=ECV8UcVEKp4" },
            { "glute kickback",                     "https://www.youtube.com/watch?v=ECV8UcVEKp4" },
            { "machine glute kickback",             "https://www.youtube.com/watch?v=ECV8UcVEKp4" },
            { "donkey kick",                        "https://www.youtube.com/watch?v=SJ1Xuz9D-ZQ" },
            { "donkey kicks",                       "https://www.youtube.com/watch?v=SJ1Xuz9D-ZQ" },
            { "fire hydrant",                       "https://www.youtube.com/watch?v=LA1JlPVOKJg" },
            { "fire hydrants",                      "https://www.youtube.com/watch?v=LA1JlPVOKJg" },
            { "abductor machine",                   "https://www.youtube.com/watch?v=gkr3Yw0wFJM" },
            { "hip abduction machine",              "https://www.youtube.com/watch?v=gkr3Yw0wFJM" },
            { "adductor machine",                   "https://www.youtube.com/watch?v=gkr3Yw0wFJM" },
            { "hip adduction machine",              "https://www.youtube.com/watch?v=gkr3Yw0wFJM" },
            { "frog pump",                          "https://www.youtube.com/watch?v=wPM8icPu6H8" },
            { "cable pull through",                 "https://www.youtube.com/watch?v=pMJBcJt7tGk" },

            // ============================================================
            // תרגילים מורכבים - Compound / Full Body
            // ============================================================
            { "burpee",                             "https://www.youtube.com/watch?v=TU8QYVW0gDU" },
            { "burpees",                            "https://www.youtube.com/watch?v=TU8QYVW0gDU" },
            { "clean and press",                    "https://www.youtube.com/watch?v=KwYJTpQ_x5A" },
            { "clean and jerk",                     "https://www.youtube.com/watch?v=KwYJTpQ_x5A" },
            { "power clean",                        "https://www.youtube.com/watch?v=KwYJTpQ_x5A" },
            { "hang clean",                         "https://www.youtube.com/watch?v=KwYJTpQ_x5A" },
            { "snatch",                             "https://www.youtube.com/watch?v=9xQp2sldyts" },
            { "thruster",                           "https://www.youtube.com/watch?v=L219ltL15zk" },
            { "thrusters",                          "https://www.youtube.com/watch?v=L219ltL15zk" },
            { "box jump",                           "https://www.youtube.com/watch?v=NBY9-kTuHEk" },
            { "box jumps",                          "https://www.youtube.com/watch?v=NBY9-kTuHEk" },
            { "jump squat",                         "https://www.youtube.com/watch?v=A-cFYWvaHr0" },
            { "jump squats",                        "https://www.youtube.com/watch?v=A-cFYWvaHr0" },
            { "kettlebell swing",                   "https://www.youtube.com/watch?v=YSxHifyI6s8" },
            { "kettlebell swings",                  "https://www.youtube.com/watch?v=YSxHifyI6s8" },
            { "kettlebell goblet squat",            "https://www.youtube.com/watch?v=MxsFDhcyFyE" },
            { "kettlebell deadlift",                "https://www.youtube.com/watch?v=YSxHifyI6s8" },
            { "turkish get up",                     "https://www.youtube.com/watch?v=boFNGI_PYzA" },
            { "farmer walk",                        "https://www.youtube.com/watch?v=Fkzk_RqlYig" },
            { "farmers walk",                       "https://www.youtube.com/watch?v=Fkzk_RqlYig" },
            { "farmers carry",                      "https://www.youtube.com/watch?v=Fkzk_RqlYig" },
            { "sled push",                          "https://www.youtube.com/watch?v=G9XAMJJz7TM" },
            { "sled pull",                          "https://www.youtube.com/watch?v=G9XAMJJz7TM" },
            { "battle rope",                        "https://www.youtube.com/watch?v=roHqPJw8Aqo" },
            { "battle ropes",                       "https://www.youtube.com/watch?v=roHqPJw8Aqo" },
            { "medicine ball slam",                 "https://www.youtube.com/watch?v=duFqBECJME8" },
            { "med ball slam",                      "https://www.youtube.com/watch?v=duFqBECJME8" },
            { "medicine ball throw",                "https://www.youtube.com/watch?v=duFqBECJME8" },

            // ============================================================
            // קרדיו - Cardio
            // ============================================================
            { "running",                            "https://www.youtube.com/watch?v=brFHyOtTwH4" },
            { "treadmill running",                  "https://www.youtube.com/watch?v=brFHyOtTwH4" },
            { "jump rope",                          "https://www.youtube.com/watch?v=hCCNM9kZ8yk" },
            { "jump roping",                        "https://www.youtube.com/watch?v=hCCNM9kZ8yk" },
            { "jumping jacks",                      "https://www.youtube.com/watch?v=c4DAnQ6DtF8" },
            { "high knees",                         "https://www.youtube.com/watch?v=6BvY36TBFuo" },
            { "sprint",                             "https://www.youtube.com/watch?v=brFHyOtTwH4" },
            { "sprints",                            "https://www.youtube.com/watch?v=brFHyOtTwH4" },
            { "rowing machine",                     "https://www.youtube.com/watch?v=H0r_D6OhBsI" },
            { "rowing",                             "https://www.youtube.com/watch?v=H0r_D6OhBsI" },
            { "elliptical",                         "https://www.youtube.com/watch?v=brFHyOtTwH4" },
            { "stair climber",                      "https://www.youtube.com/watch?v=brFHyOtTwH4" },
            { "cycling",                            "https://www.youtube.com/watch?v=brFHyOtTwH4" },
            { "stationary bike",                    "https://www.youtube.com/watch?v=brFHyOtTwH4" },

            // ============================================================
            // אמות - Forearms
            // ============================================================
            { "wrist curl",                         "https://www.youtube.com/watch?v=j0IbBbCXFJQ" },
            { "wrist curls",                        "https://www.youtube.com/watch?v=j0IbBbCXFJQ" },
            { "reverse wrist curl",                 "https://www.youtube.com/watch?v=j0IbBbCXFJQ" },
            { "wrist roller",                       "https://www.youtube.com/watch?v=j0IbBbCXFJQ" },
            { "plate pinch",                        "https://www.youtube.com/watch?v=j0IbBbCXFJQ" },
            { "dead hang",                          "https://www.youtube.com/watch?v=eGo4IYlbE5g" },

            // ============================================================
            // יציבה ומתיחה - Mobility / Stretching
            // ============================================================
            { "hip flexor stretch",                 "https://www.youtube.com/watch?v=YQmpO9VT2X4" },
            { "pigeon pose",                        "https://www.youtube.com/watch?v=YQmpO9VT2X4" },
            { "cat cow",                            "https://www.youtube.com/watch?v=kqnua4rHVVA" },
            { "world greatest stretch",             "https://www.youtube.com/watch?v=bsAoRMGDkRM" },
            { "band pull apart",                    "https://www.youtube.com/watch?v=YGEJFqhKkBY" },
            { "thoracic rotation",                  "https://www.youtube.com/watch?v=kqnua4rHVVA" },
        };

        // ============================================================
        // מחזירה את קישור הסרטון לתרגיל לפי שמו
        // אם לא נמצא סרטון - מחזירה null במקום DuckDuckGo
        // ============================================================
        public static string GetVideoUrl(string exerciseName)
        {
            if (string.IsNullOrWhiteSpace(exerciseName))
                return null;

            // 1. חיפוש התאמה מדויקת - הכי מהיר
            if (VideoLinks.TryGetValue(exerciseName, out string url))
                return url;

            // 2. חיפוש חכם - ממיין לפי אורך המפתח למניעת טעויות
            var bestMatch = VideoLinks.Keys
                .Where(k => exerciseName.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                            k.Contains(exerciseName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(k => k.Length)
                .FirstOrDefault();

            if (bestMatch != null)
                return VideoLinks[bestMatch];

            // 3. לא נמצא - מחזירים null
            return null;
        }

        // ============================================================
        // מחזירה את מספר התרגילים הכולל במאגר
        // ============================================================
        public static int GetTotalExercises() => VideoLinks.Count;

        // ============================================================
        // בודקת אם יש סרטון ישיר לתרגיל במאגר
        // ============================================================
        public static bool HasVideo(string exerciseName) => VideoLinks.ContainsKey(exerciseName);
    }
}