using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

/// <summary>
/// Unity does not recognise .srt or .vtt as text, so without this they cannot be
/// dropped into a TextAsset field at all. This imports them as TextAssets, which
/// means a subtitle file can be dragged straight into a narration entry.
/// </summary>
[ScriptedImporter(1, new[] { "srt", "vtt" })]
public class SrtImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        string text = File.ReadAllText(ctx.assetPath);
        TextAsset asset = new TextAsset(text);

        ctx.AddObjectToAsset("text", asset);
        ctx.SetMainObject(asset);
    }
}
