using UnityEngine;

// Orang MENYEBERANG jalan seperti pejalan kaki asli: berjalan LURUS satu arah menyeberang jalan,
// setelah sampai seberang (atau dilewati pemain) muncul lagi di depan dari salah satu sisi (arah acak).
public class Penyeberang : MonoBehaviour
{
    public float kecepatan = 3f;    // laju jalan menyamping (unit/detik)
    public float batasX = 5.5f;     // sisi kiri/kanan jalan
    public float jarakUlang = 70f;  // reset ke depan pemain sejauh ini
    int arah = 1;
    float cooldown = 0f;
    Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        UlangPosisi(transform.position.z);
    }

    void UlangPosisi(float z)
    {
        arah = (Random.value < 0.5f) ? 1 : -1;
        Vector3 pos = transform.position;
        pos.x = arah > 0 ? -(batasX + 3f) : (batasX + 3f);
        pos.z = z;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0f, arah > 0f ? 90f : -90f, 0f);
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        Vector3 pos = transform.position;
        pos.x += arah * kecepatan * Time.deltaTime;
        transform.position = pos;

        if (cooldown > 0f) cooldown -= Time.deltaTime;

        bool sampaiSeberang = (arah > 0 && pos.x > batasX + 3f) || (arah < 0 && pos.x < -(batasX + 3f));
        bool dilewati = player != null && player.position.z > pos.z + 6f;
        if (sampaiSeberang || dilewati)
            UlangPosisi((player != null ? player.position.z : pos.z) + jarakUlang);
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
