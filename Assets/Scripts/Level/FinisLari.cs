using UnityEngine;

// Garis finis Scene 2 -> menang
public class FinisLari : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && RunGame.instance != null)
            RunGame.instance.Menang();
    }
}
