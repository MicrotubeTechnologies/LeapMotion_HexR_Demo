using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The English script for the Patient Heart experience, in one place.
///
/// This is the fallback content: whatever the active <see cref="LanguagePack"/>
/// does not translate is played from here, so the experience never goes silent
/// or blank because a translation is missing. It is also what the Mandarin
/// template generator uses to lay out the keys and reference timings.
/// </summary>
public static class HeartNarration
{
    #region Narration keys
    public const string NormalIntro = "narration.normal.1";
    public const string NormalTouch = "narration.normal.2";
    public const string HypertensiveIntro = "narration.hypertensive.1";
    public const string HypertensiveTouch = "narration.hypertensive.2";
    public const string IschemicIntro = "narration.ischemic.1";
    public const string IschemicTouch = "narration.ischemic.2";
    public const string AthleticIntro = "narration.athletic.1";
    public const string AthleticTouch = "narration.athletic.2";
    public const string Completed = "narration.completed";
    public const string Sad = "narration.sad";

    public static readonly string[] AllKeys =
    {
        NormalIntro, NormalTouch,
        HypertensiveIntro, HypertensiveTouch,
        IschemicIntro, IschemicTouch,
        AthleticIntro, AthleticTouch,
        Completed, Sad
    };
    #endregion

    #region Heart labels
    public const string LabelNormal = "Normal";
    public const string LabelHypertensive = "Hypertensive";
    public const string LabelIschemic = "Ischemic";
    public const string LabelAthletic = "Athletic";
    public const string LabelRecap = "Press esc to return to menu";

    public static readonly string[] AllLabels =
    {
        LabelNormal, LabelHypertensive, LabelIschemic, LabelAthletic, LabelRecap
    };

    /// <summary>
    /// Turns a piece of English UI copy into a stable localisation key, e.g.
    /// "Press esc to return to menu" becomes "label.press_esc_to_return_to_menu".
    /// </summary>
    public static string LabelKey(string english)
    {
        if (string.IsNullOrEmpty(english))
            return "label.empty";

        System.Text.StringBuilder sb = new System.Text.StringBuilder("label.");
        bool lastWasSeparator = true;

        foreach (char c in english)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(char.ToLowerInvariant(c));
                lastWasSeparator = false;
            }
            else if (!lastWasSeparator)
            {
                sb.Append('_');
                lastWasSeparator = true;
            }
        }

        return sb.ToString().TrimEnd('_');
    }
    #endregion

    #region English script
    private static Dictionary<string, SubtitleLine[]> english;

    /// <summary>English subtitles for a narration key, or an empty array if unknown.</summary>
    public static SubtitleLine[] EnglishLines(string key)
    {
        BuildEnglish();
        return english.TryGetValue(key ?? string.Empty, out SubtitleLine[] lines)
            ? lines
            : new SubtitleLine[0];
    }

    private static SubtitleLine Line(string text, float startTime)
    {
        return new SubtitleLine { text = text, startTime = startTime };
    }

    private static void BuildEnglish()
    {
        if (english != null)
            return;

        english = new Dictionary<string, SubtitleLine[]>();

        float delay = 4f;
        english[NormalIntro] = new[]
        {
            Line("Welcome! Today, we're going on a journey to understand the human heart.", delay),
            Line("Think of it like a guided museum tour, where you can see, and even feel how your heart works.", delay + 4f),
            Line("Before we look at different heart problems, let's meet the healthy heart the gold standard that we all want to protect.", delay + 13f),
            Line("Here it is, your heart when it's strong and well. It beats about 100,000 times every single day without resting.", delay + 23f),
            Line("Reach out and touch it.", delay + 35f)
        };

        delay = 5f;
        english[NormalTouch] = new[]
        {
            Line("Do you feel that? That's the rhythm of life - smooth, steady, and strong.", delay),
            Line("A healthy heart is like a reliable pump; every beat pushes blood and oxygen through your body.", delay + 6f),
            Line("Now, here's the big idea for today", delay + 8f),
            Line("the way you live affects whether your heart stays this way or changes for the worse.", delay + 14f),
            Line("Let's see how everyday choices shape your heart.", delay + 23f)
        };

        delay = 4f;
        english[HypertensiveIntro] = new[]
        {
            Line("Look closely - the heart has changed. Its walls are thicker, its glow is weaker", delay),
            Line("and the beat looks more forceful but less flexible. This is the transformation into a hypertensive heart.", delay + 7f),
            Line("An unhealthy diet - too much salt, sugar, and fat - together with obesity, forces the heart to work overtime.", delay + 15f),
            Line("High blood pressure causes the muscles to bulk up, just as lifting heavy weights makes your arms bigger.", delay + 22f),
            Line("But unlike strong arms, a stiff heart isn't good news.", delay + 25f),
            Line("Go ahead, reach out and touch the hypertensive heart.", delay + 28f),
            Line("Notice the difference in how it feels compared to the healthy one.", delay + 31f)
        };

        delay = 6f;
        english[HypertensiveTouch] = new[]
        {
            Line("Do you feel that? Less bounce, more strain. That's the silent burden of high blood pressure.", delay),
            Line("Most people don't even notice it until damage is already happening.", delay + 4f),
            Line("When the heart walls get too thick, the chambers inside shrink, and less blood can be pumped out with each beat.", delay + 13f),
            Line("Over time, this raises the risk of heart failure, stroke, and kidney problems.", delay + 18f),
            Line("The good news: lifestyle changes - less salt, healthier food, regular exercise", delay + 23f),
            Line("can lower blood pressure and help keep your heart flexible and strong.", delay + 29f),
            Line("Now let's try another lifestyle choice, shall we?", delay + 33f)
        };

        delay = 4f;
        english[IschemicIntro] = new[]
        {
            Line("Take a look at how the heart has changed after prolonged smoking.", delay),
            Line("Notice the uneven areas, some parts are dimmer than others.", delay + 3.5f),
            Line("This represents damage from restricted blood flow.", delay + 6.5f),
            Line("This is called an ischemic heart. Smoking damages blood vessels,", delay + 11f),
            Line("promotes plaque build-up, and limits oxygen supply to the heart muscle.", delay + 14.5f),
            Line("Over time, some areas may weaken or even die off, leaving scars.", delay + 18f),
            Line("Go ahead, reach out and touch the ischemic heart to feel the difference yourself", delay + 22f)
        };

        delay = 5.5f;
        english[IschemicTouch] = new[]
        {
            Line("Do you feel that? The heartbeat is uneven, unpredictable, and weaker in some areas.", delay),
            Line("That's the hallmark of an ischemic heart. Ischemic hearts are at risk of chest pain,", delay + 6f),
            Line("heart attacks, and even heart failure if blood flow is not restored.", delay + 9f),
            Line("The good news is that quitting smoking, eating healthy, and exercising can help prevent further damage and improve heart function.", delay + 17.5f),
            Line("Now let's try another lifestyle choice, shall we?", delay + 18f)
        };

        delay = 4.5f;
        english[AthleticIntro] = new[]
        {
            Line("Now let's look at what happens when we care for the heart through regular exercise.", delay),
            Line("Notice how this heart is a little bigger and glowing brightly, pumping with strength and efficiency.", delay + 5f),
            Line("This is sometimes called an athletic heart. Unlike a hypertensive heart that grows stiff from pressure", delay + 11f),
            Line("an athletic heart grows stronger and more efficient.", delay + 14f),
            Line("Go ahead reach out and touch the athletic heart to feel the difference yourself.", delay + 20f)
        };

        delay = 4f;
        english[AthleticTouch] = new[]
        {
            Line("Do you feel that? Stronger, but still smooth and steady.", delay),
            Line("That's the mark of a well-trained, healthy heart.", delay + 3.5f),
            Line("Regular exercise helps the heart pump more efficiently, lowers blood pressure, improves circulation, and protects against heart disease.", delay + 11f),
            Line("Just 30 minutes of activity a day can make a lifetime of difference.", delay + 16f),
            Line("Now let's try another lifestyle choice, shall we?", delay + 18f)
        };

        delay = 4f;
        english[Completed] = new[]
        {
            Line("You've now discovered how lifestyle choices can weaken or strengthen the heart.", delay),
            Line("From here on, you're free to explore. Touch each heart again, compare how they feel,", delay + 4f),
            Line("and reflect on the choices that lead to them.", delay + 7f),
            Line("Remember the heart you take care of today is the one that carries you into tomorrow.", delay + 10f)
        };

        english[Sad] = new[]
        {
            Line("Hey, can you stop throwing my heart away? It's making me sad.", 4f)
        };
    }
    #endregion
}
