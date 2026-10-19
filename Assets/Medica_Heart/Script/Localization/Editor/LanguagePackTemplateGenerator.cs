using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Creates a Mandarin <see cref="LanguagePack"/> pre-filled with every key the
/// experience uses, plus the English line-by-line timings as a starting point,
/// so translation is just overwriting text and dropping in the Chinese clips.
/// </summary>
public static class LanguagePackTemplateGenerator
{
    private const string TargetFolder = "Assets/Medica_Heart/Localization";
    private const string AssetPath = TargetFolder + "/LanguagePack_Mandarin.asset";

    [MenuItem("Medica Heart/Create or Refresh Mandarin Language Pack")]
    public static void CreateOrRefresh()
    {
        if (!AssetDatabase.IsValidFolder(TargetFolder))
            AssetDatabase.CreateFolder("Assets/Medica_Heart", "Localization");

        LanguagePack pack = AssetDatabase.LoadAssetAtPath<LanguagePack>(AssetPath);
        bool isNew = pack == null;

        if (isNew)
        {
            pack = ScriptableObject.CreateInstance<LanguagePack>();
            pack.language = GameLanguage.Mandarin;
            pack.subtitleCharsPerLine = 22; // CJK glyphs are double width
            AssetDatabase.CreateAsset(pack, AssetPath);
        }

        AddMissingLabels(pack);
        AddMissingNarrations(pack);

        EditorUtility.SetDirty(pack);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = pack;
        EditorGUIUtility.PingObject(pack);

        Debug.Log(isNew
            ? "[Localization] Created " + AssetPath + ". Fill in the Chinese text, assign a CJK TMP font and drop in the Mandarin clips."
            : "[Localization] Refreshed " + AssetPath + " - existing translations were kept.");
    }

    private static void AddMissingLabels(LanguagePack pack)
    {
        HashSet<string> existing = new HashSet<string>();
        foreach (LocalizedString entry in pack.strings)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.key))
                existing.Add(entry.key);
        }

        foreach (string english in HeartNarration.AllLabels)
        {
            string key = HeartNarration.LabelKey(english);
            if (existing.Contains(key))
                continue;

            // English text is seeded as the value so it is obvious what needs translating.
            pack.strings.Add(new LocalizedString { key = key, value = english });
        }
    }

    private static void AddMissingNarrations(LanguagePack pack)
    {
        HashSet<string> existing = new HashSet<string>();
        foreach (LocalizedNarration entry in pack.narrations)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.key))
                existing.Add(entry.key);
        }

        foreach (string key in HeartNarration.AllKeys)
        {
            if (existing.Contains(key))
                continue;

            LocalizedNarration narration = new LocalizedNarration { key = key };

            // Seed with the English lines and timings; retime them against the
            // Mandarin recording once it exists.
            foreach (SubtitleLine line in HeartNarration.EnglishLines(key))
                narration.lines.Add(line);

            pack.narrations.Add(narration);
        }
    }
}
