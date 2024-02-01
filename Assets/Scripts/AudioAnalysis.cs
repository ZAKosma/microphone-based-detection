using UnityEngine;

public class AudioAnalysis : MonoBehaviour
{
    [SerializeField] private AudioCapture audioCapture;
    private float normalizedVolume = 0f;

    void Update()
    {
        float[] audioData = audioCapture.GetAudioData();
        normalizedVolume = CalculateNormalizedVolume(audioData);
    }

    private float CalculateNormalizedVolume(float[] data)
    {
        float rmsValue = AudioUtility.CalculateRMS(data);
        
        // Normalize the RMS value against the background noise level and potential max volume
        return Mathf.Clamp01((rmsValue - AudioSettingsManager.BackgroundNoiseLevel) / (AudioSettingsManager.MaxLoudnessLevel - AudioSettingsManager.BackgroundNoiseLevel));
    }
    
    public float NormalizedVolume => normalizedVolume;
}