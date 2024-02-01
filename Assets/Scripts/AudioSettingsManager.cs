using UnityEngine;

public static class AudioSettingsManager
{
    private static float backgroundNoiseLevel = 0f;
    private static float maxLoudnessLevel = 1f; // Default to 1, adjust based on calibration

    static AudioSettingsManager()
    {
        // Load saved levels on initialization
        backgroundNoiseLevel = PlayerPrefs.GetFloat("BackgroundNoiseLevel", 0f);
        maxLoudnessLevel = PlayerPrefs.GetFloat("MaxLoudnessLevel", 1f);
    }

    public static float BackgroundNoiseLevel
    {
        get => backgroundNoiseLevel;
        set
        {
            backgroundNoiseLevel = value;
            PlayerPrefs.SetFloat("BackgroundNoiseLevel", value);
            PlayerPrefs.Save();
        }
    }

    public static float MaxLoudnessLevel
    {
        get => maxLoudnessLevel;
        set
        {
            maxLoudnessLevel = value;
            PlayerPrefs.SetFloat("MaxLoudnessLevel", value);
            PlayerPrefs.Save();
        }
    }
}