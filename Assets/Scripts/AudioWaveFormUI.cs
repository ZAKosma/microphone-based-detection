using UnityEngine;
using UnityEngine.UI;

public class AudioWaveformUI : Graphic
{
    public AudioAnalysis audioAnalysis;
    public AudioCapture audioCapture;
    public Gradient loudnessGradient;
    private float[] audioData = new float[1024];

    [SerializeField] private float heightScale = 1.0f; // Height scaling factor
    [SerializeField] private float lineThickness = 2.0f; // Thickness of the waveform line
    [SerializeField] private float dramatizationFactor = 1.0f; // Amplifies the waveform

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (Application.isPlaying == false)
        {
            return;
        }
        vh.Clear();
        UpdateAudioData(); // Ensure audio data is updated

        float loudness = audioAnalysis.NormalizedVolume;
        
        int samplesCount = audioData.Length;
        float width = rectTransform.rect.width;
        float halfWidth = width / 2;
        float height = rectTransform.rect.height * heightScale;

        for (int i = 0; i < samplesCount - 1; i++)
        {
            float x = (-halfWidth) + width * i / samplesCount;
            float nextX = (-halfWidth) + width * (i + 1) / samplesCount;

            // Apply dramatization factor and scale y-values
            float y = ((audioData[i] * dramatizationFactor) + 1) * height / 2;
            float nextY = ((audioData[i + 1] * dramatizationFactor) + 1) * height / 2;

            Color color = loudnessGradient.Evaluate(loudness);

            // Top vertices
            vh.AddVert(new Vector3(x, y, 0), color, new Vector2(0, 1));
            vh.AddVert(new Vector3(nextX, nextY, 0), color, new Vector2(1, 1));

            // Bottom vertices
            vh.AddVert(new Vector3(x, y - lineThickness, 0), color, new Vector2(0, 0));
            vh.AddVert(new Vector3(nextX, nextY - lineThickness, 0), color, new Vector2(1, 0));

            int offset = i * 4;
            vh.AddTriangle(offset, offset + 1, offset + 2);
            vh.AddTriangle(offset + 1, offset + 3, offset + 2);
        }
    }

    void Update()
    {
        SetVerticesDirty(); // Cause OnPopulateMesh to be called again
    }
    
    private void UpdateAudioData()
    {
        audioCapture.GetOutputData(audioData);
    }
}