using UnityEngine;

public class AudioAnalysis : MonoBehaviour
{
    [SerializeField] private AudioCapture audioCapture;
    public float NormalizedVolume { get; private set; }

    void Update()
    {
        float[] audioData = audioCapture.GetAudioData();
        NormalizedVolume = CalculateNormalizedVolume(audioData);
    }

    private float CalculateNormalizedVolume(float[] data)
    {
        float rmsValue = AudioUtility.CalculateRMS(data);

        // Calculate the difference between the maximum loudness and background noise levels to define the volume range.
        float topOfVolumeRange = AudioSettingsManager.MaxLoudnessLevel - AudioSettingsManager.BackgroundNoiseLevel;

        // Adjust the RMS value by subtracting the background noise
        float adjustedRMSValue = rmsValue - AudioSettingsManager.BackgroundNoiseLevel;

        // Normalize the volume: Scale the adjusted RMS value to a 0-1 range,
        // with 0 being silent (background noise level) and 1 being the maximum loudness.
        float normalizedVolume = adjustedRMSValue / topOfVolumeRange;

        // Ensure the normalized volume is within the 0-1 range.
        // This step handles cases where the actual RMS value falls outside the calibrated range,
        // either below the background noise level or above the maximum loudness level.
        normalizedVolume = Mathf.Clamp01(normalizedVolume);

        return normalizedVolume;
    }
}