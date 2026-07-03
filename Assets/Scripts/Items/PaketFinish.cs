using UnityEngine;
using UnityEngine.SceneManagement;

// Paket di pos. Player ambil = lanjut ke jalan (Scene 2)
public class PaketFinish : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PAKET TERAMBIL - lanjut ke jalan!");
            SceneManager.LoadScene("Level1Scene2");
        }
    }
}
