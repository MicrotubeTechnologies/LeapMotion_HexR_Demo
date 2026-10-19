using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Turns a .srt (or .vtt) subtitle file into <see cref="SubtitleLine"/>s.
///
/// This is how the Mandarin timings should be produced: run the Chinese
/// recording through any transcription / alignment tool, drop the resulting
/// file into the narration entry, and the cue times are exact - no measuring
/// anything by hand.
///
/// Only the start of each cue is used. A line stays on screen until the next
/// one begins, so the cue's end time is ignored.
/// </summary>
public static class SrtParser
{
    private const string TimeSeparator = "-->";

    public static SubtitleLine[] Parse(string content)
    {
        List<SubtitleLine> result = new List<SubtitleLine>();

        if (string.IsNullOrEmpty(content))
            return result.ToArray();

        // Normalise line endings and drop a byte order mark if the file has one.
        content = content.Replace("\r\n", "\n").Replace('\r', '\n').TrimStart((char)0xFEFF);

        string[] rows = content.Split('\n');
        float pendingStart = -1f;
        List<string> textParts = new List<string>();

        foreach (string row in rows)
        {
            string line = row.Trim();

            if (line.Length == 0)
            {
                Flush(result, ref pendingStart, textParts);
                continue;
            }

            int separator = line.IndexOf(TimeSeparator);
            if (separator >= 0)
            {
                // A new cue starts here; commit whatever came before it.
                Flush(result, ref pendingStart, textParts);

                if (TryParseTimestamp(line.Substring(0, separator).Trim(), out float start))
                    pendingStart = start;

                continue;
            }

            // Before a timestamp is seen the row is a cue number or a WEBVTT
            // header, neither of which is subtitle text.
            if (pendingStart < 0f)
                continue;

            textParts.Add(line);
        }

        Flush(result, ref pendingStart, textParts);

        // PlayQueue walks the cues in order and waits for each start time, so a
        // file that is out of order would stall it. Cheap to guard against.
        result.Sort((a, b) => a.startTime.CompareTo(b.startTime));
        return result.ToArray();
    }

    private static void Flush(List<SubtitleLine> result, ref float pendingStart, List<string> textParts)
    {
        if (pendingStart >= 0f && textParts.Count > 0)
        {
            // A cue split over several rows is one subtitle; the display wraps it
            // again to fit the box, so join with spaces rather than newlines.
            result.Add(new SubtitleLine
            {
                text = string.Join(" ", textParts),
                startTime = pendingStart
            });
        }

        pendingStart = -1f;
        textParts.Clear();
    }

    /// <summary>Parses "00:01:23,456", "00:01:23.456" or "01:23,456".</summary>
    private static bool TryParseTimestamp(string stamp, out float seconds)
    {
        seconds = 0f;

        if (string.IsNullOrEmpty(stamp))
            return false;

        stamp = stamp.Replace(',', '.');
        string[] parts = stamp.Split(':');

        if (parts.Length < 2 || parts.Length > 3)
            return false;

        float total = 0f;
        for (int i = 0; i < parts.Length; i++)
        {
            if (!float.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                return false;

            total = total * 60f + value;
        }

        seconds = total;
        return true;
    }
}
