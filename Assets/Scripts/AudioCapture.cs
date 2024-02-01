using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioCapture : MonoBehaviour
{
    private AudioSource audioSource;
    private const int SAMPLE_COUNT = 1024;
    private float[] audioData;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioData = new float[SAMPLE_COUNT];
    }

    private void Start()
    {
        InitializeMicrophone();
    }

    public void InitializeMicrophone()
    {
        audioSource.clip = Microphone.Start(null, true, 10, 44100);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) {}
        audioSource.Play();
    }

    public float[] GetAudioData()
    {
        audioSource.GetOutputData(audioData, 0);
        return audioData;
    }
}