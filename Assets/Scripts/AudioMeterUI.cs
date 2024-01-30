using UnityEngine;
using UnityEngine.UI;

public class AudioMeterUI : MonoBehaviour
{
    public AudioAnalysis audioAnalysis;
    public GameObject barPrefab; // Prefab for a single bar
    public int numberOfBars = 10; // Default number of bars
    public float margin = 10f; // Margin around the bars
    public float padding = 5f; // Padding between bars
    public Gradient barGradient; // Gradient for coloring bars

    private GameObject[] bars;
    private RectTransform panelRectTransform;

    void Start()
    {
        if (audioAnalysis == null)
        {
            audioAnalysis = GetComponent<AudioAnalysis>();
        }

        panelRectTransform = GetComponent<RectTransform>();
        if (panelRectTransform == null)
        {
            Debug.LogError("RectTransform component not found on " + gameObject.name);
            return; // Exit if no RectTransform is found
        }

        CreateBars();
    }

    void CreateBars()
    {
        // Destroy existing bars
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);

        bool isVertical = panelRectTransform.rect.height > panelRectTransform.rect.width;
        float totalPadding = padding * (numberOfBars - 1);
        float totalMargin = margin * 2;

        float availableWidth = panelRectTransform.rect.width - totalMargin;
        float availableHeight = panelRectTransform.rect.height - totalMargin;

        float barThickness = isVertical ? availableWidth : (availableHeight - totalPadding) / numberOfBars;
        float barLength = isVertical ? (availableHeight - totalPadding) / numberOfBars : availableWidth;

        float totalBarsHeight = barLength * numberOfBars + totalPadding;
        float startOffset = (availableHeight - totalBarsHeight) / 2;

        bars = new GameObject[numberOfBars];
        for (int i = 0; i < numberOfBars; i++)
        {
            GameObject bar = Instantiate(barPrefab, transform);
            RectTransform barRect = bar.GetComponent<RectTransform>();

            if (isVertical)
            {
                barRect.sizeDelta = new Vector2(barThickness, barLength);
                float posY = -availableHeight / 2 + startOffset + (barLength + padding) * i + barLength / 2;
                barRect.anchoredPosition = new Vector2(0, posY);
            }
            else
            {
                // Horizontal layout adjustments if needed
                barRect.sizeDelta = new Vector2(barLength, barThickness);
                float posX = -availableWidth / 2 + margin + (barThickness + padding) * i + barThickness / 2;
                barRect.anchoredPosition = new Vector2(posX, 0);
            }

            Image barImage = bar.GetComponent<Image>();
            barImage.color = barGradient.Evaluate((float)i / numberOfBars);

            bars[i] = bar;
        }
    }




    [ContextMenu("Create Meter in Edit Mode")]
    void CreateMeterInEditMode()
    {
        // Ensure RectTransform is assigned
        if (panelRectTransform == null)
        {
            panelRectTransform = GetComponent<RectTransform>();
        }

        // Check if the component is successfully retrieved
        if (panelRectTransform == null)
        {
            Debug.LogError("RectTransform not found on the GameObject.");
            return;
        }

        // Proceed with bar creation
        CreateBars();
    }

    void Update()
    {
        float volumeLevel = audioAnalysis.NormalizedVolume;
        UpdateMeter(volumeLevel);
    }

    void UpdateMeter(float normalizedVolume)
    {
        int activeBars = Mathf.Clamp((int)(normalizedVolume * numberOfBars), 0, numberOfBars);
        for (int i = 0; i < numberOfBars; i++)
        {
            bars[i].SetActive(i < activeBars);
        }
    }
}