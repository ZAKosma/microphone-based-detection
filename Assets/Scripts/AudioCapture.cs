using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioCapture : MonoBehaviour
{
    private AudioSource audioSource;
    private const int SAMPLE_COUNT = 1024;
    private float[] audioData;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioData = new float[SAMPLE_COUNT];
        StartMicrophone();
    }

    void StartMicrophone()
    {
        audioSource.clip = Microphone.Start(null, true, 10, 44100);
        audioSource.loop = true;
        // Wait until the microphone is ready
        while (!(Microphone.GetPosition(null) > 0)) {}
        audioSource.Play();
    }

    public void GetOutputData(float[] data)
    {
        audioSource.GetOutputData(data, 0);
    }
}