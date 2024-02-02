
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioCalibrationManager : MonoBehaviour
{
    [SerializeField] private AudioCapture audioCapture;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button startCalibrationButton;
    [SerializeField] private Button testMicrophoneButton;
    [SerializeField] private Button applyButton;
    
    [SerializeField] private int backgroundCalibrationTime = 5;
    [SerializeField] private int loudnessCalibrationTime = 3;
    
    private float backgroundNoiseLevel;
    private float maxLoudnessLevel;

    private void Start()
    {
        audioCapture.InitializeMicrophone();
        SetupUIEvents();
    }

    private void SetupUIEvents()
    {
        startCalibrationButton.onClick.AddListener(() => StartCoroutine(CalibrateBackgroundNoiseLevel(backgroundCalibrationTime)));
        testMicrophoneButton.onClick.AddListener(() => StartCoroutine(CalibrateMaxLoudnessLevel(loudnessCalibrationTime)));
        applyButton.onClick.AddListener(ApplyAndProceed);
        
        startCalibrationButton.gameObject.SetActive(true);
        testMicrophoneButton.gameObject.SetActive(false);
        applyButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    private IEnumerator CalibrateBackgroundNoiseLevel(int duration)
    {
        statusText.text = "Calibrating background noise. Please remain silent...";
        startCalibrationButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);

        yield return StartCoroutine(MeasureAudioLevel(duration, true));

        statusText.text = $"Background noise level calibrated: {backgroundNoiseLevel:F2}";
        testMicrophoneButton.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
    }

    private IEnumerator CalibrateMaxLoudnessLevel(int duration)
    {
        statusText.text = "Calibrating max volume. Please make a loud sound (applaud)...";
        testMicrophoneButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);

        yield return StartCoroutine(MeasureAudioLevel(duration, false));

        statusText.text = $"Max volume calibrated: {maxLoudnessLevel:F2}. Testing microphone...";
        applyButton.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
    }

    private IEnumerator MeasureAudioLevel(int duration, bool isBackgroundNoise)
    {
        List<float> allAudioData = new List<float>();
        for (int i = duration; i > 0; i--)
        {
            timerText.text = i + "s";
            float[] audioData = audioCapture.GetAudioData();
            allAudioData.AddRange(audioData);
            yield return new WaitForSeconds(1);
        }


        if (isBackgroundNoise)
        {
            float rms = AudioUtility.CalculateRMS(allAudioData.ToArray());

            backgroundNoiseLevel = rms;
        }
        else
            maxLoudnessLevel = FindMaxSample(allAudioData);
    }
    
    
    private float FindMaxSample(List<float> samples)
    {
        float maxSample = 0f;
        foreach (float sample in samples)
        {
            float absSample = Mathf.Abs(sample); // Consider the absolute value of each sample
            if (absSample > maxSample)
            {
                maxSample = absSample;
            }
        }
        return maxSample;
    }

    private void ApplyAndProceed()
    {
        Debug.Log($"Applying Calibration: BackgroundNoiseLevel = {backgroundNoiseLevel}, MaxLoudnessLevel = {maxLoudnessLevel}");
        
        AudioSettingsManager.BackgroundNoiseLevel = backgroundNoiseLevel;
        AudioSettingsManager.MaxLoudnessLevel = maxLoudnessLevel;

        SceneManager.LoadScene("Game");
    }
}
