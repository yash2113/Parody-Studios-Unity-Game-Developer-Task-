using UnityEngine;

public class BoxCollectible : MonoBehaviour
{
    public AudioClip collectSound;
    public GameObject collectEffect; // Optional particle effect
    public int scoreValue = 1;

    private bool isCollected = false;

    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;

            // Play sound if assigned
            if (collectSound)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // Instantiate effect if assigned
            if (collectEffect)
                Instantiate(collectEffect, transform.position, Quaternion.identity);

            // Notify score manager or any game manager
            GameManager.Instance?.AddScore(scoreValue);

            // Disable the box
            gameObject.SetActive(false);
        }
    }
}
