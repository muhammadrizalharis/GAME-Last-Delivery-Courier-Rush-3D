using UnityEngine;

// Rintangan MENYEBERANG (orang pasar) bergerak KIRI-KANAN memotong jalur.
// Pemain harus melintas saat lane-nya kosong. Setelah dilewati -> pindah maju lagi (recycle).
public class Penyeberang : MonoBehaviour
{
    public float kecepatan = 2.2f;   // laju ayun menyeberang
    public float batasX = 4.5f;      // sejauh mana menyeberang kiri-kanan
    public float jarakUlang = 70f;   // reset ke depan pemain sejauh ini
    float fase;
    float cooldown = 0f;
    Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        fase = Random.Range(0f, 6.28f);
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        fase += kecepatan * Time.deltaTime;
        Vector3 pos = transform.position;
        pos.x = Mathf.Sin(fase) * batasX;
        transform.position = pos;

        // hadapkan ke arah jalan (menyamping)
        float arah = Mathf.Cos(fase);
        if (Mathf.Abs(arah) > 0.01f)
            transform.rotation = Quaternion.Euler(0f, arah > 0f ? 90f : -90f, 0f);

        if (cooldown > 0f) cooldown -= Time.deltaTime;

        // pemain sudah lewat -> pindah maju lagi
        if (player != null && player.position.z > transform.position.z + 6f)
        {
            pos = transform.position;
            pos.z = player.position.z + jarakUlang;
            transform.position = pos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Application.isPlaying) return;
        if (other.CompareTag("Player") && cooldown <= 0f)
        {
            cooldown = 1f;
            if (RunGame.instance != null) RunGame.instance.Kena();
        }
    }
}
