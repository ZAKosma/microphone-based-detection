using UnityEngine;

public class AIWarner : MonoBehaviour
{
    public AudioAnalysis audioAnalysis;
    public float maxResponseRadius = 30f; // Maximum radius of AI response
    public GameObject responseRadiusIndicator; // Reference to the sphere GameObject
    
    void Update()
    {
        float normalizedVolume = audioAnalysis.NormalizedVolume;
        float currentResponseRadius = normalizedVolume * maxResponseRadius;
        AlertNearbyEnemies(currentResponseRadius);
        
        if (responseRadiusIndicator != null)
        {
            // Update sphere scale to match the response radius
            responseRadiusIndicator.transform.localScale = new Vector3(currentResponseRadius, currentResponseRadius, currentResponseRadius) * 2; // Multiply by 2 because the scale is diameter
        }
    }

    void AlertNearbyEnemies(float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            EnemyAI enemy = hitCollider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.OnAlert();
            }
        }
    }
}