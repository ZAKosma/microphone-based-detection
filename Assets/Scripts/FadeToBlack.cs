using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeToBlack : MonoBehaviour
{
    public Image fadePanel;
    public GameObject defeatScreenContainer;
    public GameObject victoryScreenContainer;
    public GameObject entryScreenContainer;
    public float entryFadeDuration = 2f;  // Duration for the entry fade
    public float endGameFadeDuration = 2f;  // Duration for the end-game fade
    
    void Start()
    {
        // Initialize screen containers
        defeatScreenContainer.SetActive(false);
        victoryScreenContainer.SetActive(false);
        entryScreenContainer.SetActive(true); // Assuming you start with entry screen

        FadeFromBlack();
    }

    public void FadeFromBlack()
    {
        StartCoroutine(FadePanelCoroutine(1, 0, entryFadeDuration, () => entryScreenContainer.SetActive(false)));
    }

    public void FadeToBlackForVictory()
    {
        StartCoroutine(FadePanelCoroutine(0, 1, endGameFadeDuration, () => victoryScreenContainer.SetActive(true)));
    }

    public void FadeToBlackForDefeat()
    {
        StartCoroutine(FadePanelCoroutine(0, 1, endGameFadeDuration, () => defeatScreenContainer.SetActive(true)));
    }

    private IEnumerator FadePanelCoroutine(float startAlpha, float endAlpha, float duration, System.Action onFadeComplete)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            if (fadePanel != null)
            {
                fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, Mathf.SmoothStep(startAlpha, endAlpha, elapsedTime / duration));
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, endAlpha);
        onFadeComplete?.Invoke();
    }
}