using UnityEngine;

// Batu penghalang PADAT: collider solid -> pemain benar-benar tertahan (tak bisa menembus),
// harus pindah jalur atau melompatinya. Menyenggol SISI batu -> HP berkurang (ada jeda),
// tapi berdiri di ATAS batu tidak kena. Batu tetap tampak (tidak hilang seperti rintangan biasa).
[RequireComponent(typeof(BoxCollider))]
public class BatuHalang : MonoBehaviour
{
    public int damage = 20;      // HP berkurang tiap menyenggol
    public float jeda = 1.1f;    // jeda antar pengurangan HP saat mentok terus
    float cooldown;

    void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;
    }

    void OnCollisionEnter(Collision c) { Senggol(c); }
    void OnCollisionStay(Collision c) { Senggol(c); }

    void Senggol(Collision c)
    {
        if (!Application.isPlaying || cooldown > 0f) return;
        if (!c.collider.CompareTag("Player")) return;

        // hanya kena kalau menabrak SISI batu (normal mendatar), bukan saat berdiri di atasnya
        bool sisi = false;
        for (int i = 0; i < c.contactCount; i++)
        {
            if (Mathf.Abs(c.GetContact(i).normal.y) < 0.6f) { sisi = true; break; }
        }
        if (!sisi) return;

        cooldown = jeda;
        if (RunGame.instance != null) RunGame.instance.Kena(damage);
    }
}
