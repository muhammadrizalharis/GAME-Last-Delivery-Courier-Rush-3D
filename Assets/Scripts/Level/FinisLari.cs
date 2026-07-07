using UnityEngine;
using UnityEngine.SceneManagement;

// Garis finis Scene 2 -> buka layar Level 2 Unlocked.
// Selesai baik saat MENYENTUH garis maupun saat sudah MELEWATI garis (biar tidak gagal gara-gara jatuh/lompat).
public class FinisLari : MonoBehaviour
{
    public string sceneBerikut = "Level2Unlocked";
    bool sudah = false;
    Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (sudah || player == null) return;
        // sudah sampai / melewati garis finis (berdasarkan posisi maju z)
        if (player.position.z >= transform.position.z) Selesai();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Selesai();
    }

    void Selesai()
    {
        if (sudah) return;
        sudah = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneBerikut);
    }
}
