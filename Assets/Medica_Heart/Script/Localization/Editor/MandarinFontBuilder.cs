using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

/// <summary>
/// Builds the Chinese TMP font asset from Noto Sans SC and drops it into the
/// Mandarin language pack, so the subtitles stop rendering as empty boxes.
///
/// The atlas is Dynamic: glyphs are rasterised on demand at runtime instead of
/// being pre-baked. For CJK that matters a lot - a static atlas covering every
/// Simplified glyph runs to tens of megabytes, while this only ever draws the
/// characters the script actually uses, and keeps working if the translation
/// changes later.
/// </summary>
public static class MandarinFontBuilder
{
    private const string SourceFontPath = "Assets/Medica_Heart/Localization/Fonts/NotoSansSC-Regular.ttf";
    private const string FontAssetPath = "Assets/Medica_Heart/Localization/Fonts/NotoSansSC-Regular SDF.asset";
    private const string PackPath = "Assets/Medica_Heart/Localization/LanguagePack_Mandarin.asset";

    private const int SamplingPointSize = 90;
    private const int AtlasPadding = 9;
    private const int AtlasSize = 1024;

    [MenuItem("Medica Heart/Build Mandarin Font Asset")]
    public static void Build()
    {
        Font source = AssetDatabase.LoadAssetAtPath<Font>(SourceFontPath);
        if (source == null)
        {
            Debug.LogError("[Localization] Could not find the source font at " + SourceFontPath);
            return;
        }

        TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        if (existing != null)
        {
            Debug.Log("[Localization] Font asset already exists at " + FontAssetPath + " - assigning it to the pack.");
            AssignToPack(existing);
            return;
        }

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            source,
            SamplingPointSize,
            AtlasPadding,
            GlyphRenderMode.SDFAA,
            AtlasSize,
            AtlasSize,
            AtlasPopulationMode.Dynamic,
            true);

        if (fontAsset == null)
        {
            Debug.LogError("[Localization] TMP could not build a font asset from " + SourceFontPath);
            return;
        }

        fontAsset.name = "NotoSansSC-Regular SDF";
        AssetDatabase.CreateAsset(fontAsset, FontAssetPath);

        // The atlas texture and material have to live inside the asset, or the
        // font asset loads with a missing material in a build.
        if (fontAsset.atlasTextures != null && fontAsset.atlasTextures.Length > 0)
        {
            fontAsset.atlasTextures[0].name = fontAsset.name + " Atlas";
            AssetDatabase.AddObjectToAsset(fontAsset.atlasTextures[0], fontAsset);
        }

        if (fontAsset.material != null)
        {
            fontAsset.material.name = fontAsset.name + " Material";
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        }

        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();

        AssignToPack(fontAsset);

        Debug.Log("[Localization] Built " + FontAssetPath + " and assigned it to the Mandarin pack.");
    }

    private static void AssignToPack(TMP_FontAsset fontAsset)
    {
        LanguagePack pack = AssetDatabase.LoadAssetAtPath<LanguagePack>(PackPath);
        if (pack == null)
        {
            Debug.LogWarning("[Localization] No language pack at " + PackPath + " - assign the font by hand.");
            return;
        }

        pack.font = fontAsset;
        EditorUtility.SetDirty(pack);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = pack;
        EditorGUIUtility.PingObject(pack);
    }
}
