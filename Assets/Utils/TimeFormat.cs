using UnityEngine;

public static class TimeFormat {
    static float emWidth = 0.65f;
    static string monospaceTag = string.Format("<mspace={0}em>", emWidth);
    static string monospaceEndTag = "</mspace>";

    public static string FormatSeconds(float timeInSeconds) {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds * 1000) % 1000);

        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public static string FormatSecondsMonospace(float timeInSeconds, float emWidth = 0.65f) {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds * 1000) % 1000);

        string minutesString = monospaceTag + string.Format("{0:00}", minutes) + monospaceEndTag;
        string secondsString = monospaceTag + string.Format("{0:00}", seconds) + monospaceEndTag;
        string millisecondsString = monospaceTag + string.Format("{0:000}", milliseconds) + monospaceEndTag;

        return string.Format("{0}:{1}.{2}", minutesString, secondsString, millisecondsString);
    }
}
