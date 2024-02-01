using System.Collections;
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
    
    //Time stored in seconds
    [SerializeField] private int backgroundCalibrationTime = 5;
    [SerializeField] private int loudnessCalibrationTime = 3;

    private void Start()
    {
        audioCapture.InitializeMicrophone();
        SetupUIEvents();
    }

    private void SetupUIEvents()
    {
        startCalibrationButton.onClick.AddListener(() => StartCoroutine(CalibrateBackgroundNoiseLevel()));
        testMicrophoneButton.onClick.AddListener(() => StartCoroutine(CalibrateMaxLoudnessLevel()));
        applyButton.onClick.AddListener(ApplyCalibrationAndProceed);
        
        startCalibrationButton.gameObject.SetActive(true);
        testMicrophoneButton.gameObject.SetActive(false);
        applyButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    private IEnumerator CalibrateBackgroundNoiseLevel()
    {
        statusText.text = "Calibrating background noise. Please remain silent...";
        startCalibrationButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);

        yield return MeasureAudioLevel(backgroundCalibrationTime, true);

        AudioSettingsManager.BackgroundNoiseLevel = AudioUtility.CalculateRMS(audioCapture.GetAudioData());
        statusText.text = "Background noise level calibrated. Please make a loud sound for max volume calibration.";
        testMicrophoneButton.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
    }

    private IEnumerator CalibrateMaxLoudnessLevel()
    {
        statusText.text = "Calibrating max volume. Please make a loud sound (applaud)...";
        testMicrophoneButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);

        yield return MeasureAudioLevel(loudnessCalibrationTime, false);

        AudioSettingsManager.MaxLoudnessLevel = AudioUtility.CalculateRMS(audioCapture.GetAudioData());
        statusText.text = "Max volume calibrated. Testing microphone...";
        timerText.gameObject.SetActive(false);
        StartCoroutine(TestMicrophone());
    }

    private IEnumerator MeasureAudioLevel(int duration, bool isBackgroundNoise)
    {
        float sum = 0f;
        int measureTime = isBackgroundNoise ? 5 : 3; // Adjust time based on calibration type
        var audioData = audioCapture.GetAudioData();


        for (int i = duration; i > 0; i--)
        {
            timerText.text = i.ToString(); // Update the countdown timer text
            audioData = audioCapture.GetAudioData();
            foreach (var sample in audioData)
            {
                sum += sample * sample;
            }
            yield return new WaitForSeconds(1);
        }

        for (int i = 0; i < audioData.Length; i++)
        {
            audioData[i] = Mathf.Sqrt(sum / (audioData.Length * measureTime));
        }
    }

    private IEnumerator TestMicrophone()
    {
        yield return new WaitForSeconds(5); // Example delay for testing
        statusText.text = "Microphone test complete. Press Apply to proceed.";
        applyButton.gameObject.SetActive(true);
    }

    private void ApplyCalibrationAndProceed()
    {
        //Settings are set in the previous methods so we can just load the scene.
        SceneManager.LoadScene("Game");
    }
}
