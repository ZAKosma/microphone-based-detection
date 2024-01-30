using UnityEngine;
using System.Collections.Generic;

public class AudioAnalysis : MonoBehaviour
{
    public AudioCapture audioCapture;
    private const int SAMPLE_COUNT = 1024;
    private float[] audioData;
    public float maxVolumeLevel = 10f; // Adjust based on testing
    public float averagingPeriod = 1f; // Time period over which to average, in seconds
    private List<float> volumeHistory;
    private float weightedSum;
    private float weightTotal;
    private int historySize;

    public float NormalizedVolume { get; private set; }

    void Start()
    {
        if (audioCapture == null)
        {
            audioCapture = GetComponent<AudioCapture>();
        }
        audioData = new float[SAMPLE_COUNT];
        volumeHistory = new List<float>();
        historySize = Mathf.FloorToInt(averagingPeriod / Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        float averageVolume = GetAverageVolume();
        UpdateVolumeHistory(averageVolume);
        float smoothedVolume = weightedSum / weightTotal;
        NormalizedVolume = NormalizeVolume(smoothedVolume);
    }

    private float GetAverageVolume()
    {
        audioCapture.GetOutputData(audioData);
        float sum = 0f;
        for (int i = 0; i < audioData.Length; i++)
        {
            sum += audioData[i] * audioData[i];
        }
        return Mathf.Sqrt(sum / SAMPLE_COUNT);
    }

    private void UpdateVolumeHistory(float volume)
    {
        volumeHistory.Add(volume);

        // Update weights and weighted sum
        float weight = 1f; // Initial weight for the newest volume
        weightedSum = 0f;
        weightTotal = 0f;
        for (int i = volumeHistory.Count - 1; i >= 0; i--)
        {
            weightedSum += volumeHistory[i] * weight;
            weightTotal += weight;
            weight *= 0.9f; // Reduce the weight for older volumes
        }

        // Trim the history if it exceeds the size limit
        if (volumeHistory.Count > historySize)
        {
            volumeHistory.RemoveAt(0);
        }
    }

    private float NormalizeVolume(float volume)
    {
        return Mathf.Clamp(volume / maxVolumeLevel, 0f, 1f);
    }
}
