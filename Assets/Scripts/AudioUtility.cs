using UnityEngine;

public static class AudioUtility
{
    //Root Mean Square or average loudness
    public static float CalculateRMS(float[] audioData)
    {
        float sum = 0f;
        foreach (var sample in audioData)
        {
            sum += sample * sample;
        }
        return Mathf.Sqrt(sum / audioData.Length);
    }
}