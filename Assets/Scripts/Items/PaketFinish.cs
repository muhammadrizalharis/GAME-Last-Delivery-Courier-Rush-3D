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
            if (RunGame.instance != null) RunGame.instance.TambahSkor(50);
            Transisi.Pindah("Level1Scene2");
        }
    }
}
