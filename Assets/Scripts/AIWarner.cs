using UnityEngine;

public class AIWarner : MonoBehaviour
{
    public AudioAnalysis audioAnalysis;
    public float maxResponseRadius = 30f; // Maximum radius of AI response

    void Update()
    {
        float normalizedVolume = audioAnalysis.NormalizedVolume;
        float currentResponseRadius = normalizedVolume * maxResponseRadius;
        AlertNearbyEnemies(currentResponseRadius);
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