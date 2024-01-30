using System;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public AudioAnalysis audioAnalysis;
    public float loudNoiseThreshold = 0.8f; // Threshold for considering a noise as loud
    private bool playerInTrigger = false;

    void Update()
    {
        // Check if player is in trigger and a loud noise is detected
        if (playerInTrigger && audioAnalysis.NormalizedVolume >= loudNoiseThreshold)
        {
            GameManager.Instance.Victory();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}