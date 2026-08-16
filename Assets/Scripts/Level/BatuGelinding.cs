using UnityEngine;

// Batu besar MENGGELINDING dari DEPAN ke arah pemain (-z) di terowongan.
// Berputar di PUSAT batu (bukan menjungkir di pangkal / di tempat) & searah gerak -> tampak menggelinding nyata.
// Begitu lewat di belakang pemain -> pindah maju lagi ke lane acak. Kena pemain -> RunGame.Kena().
public class BatuGelinding : MonoBehaviour
{
    public float kecepatan = 9f;
    public float jarakUlang = 75f;      // jarak di depan pemain saat di-reset
    public float tinggiDuduk = 0.6f;    // tinggi pusat batu di atas rel
    float cooldown = 0f;
    Transform player, vis;
    static readonly float[] lane = { -3f, 0f, 3f };

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        PusatkanVisual();
        Vector3 pos = transform.position; pos.y = tinggiDuduk; transform.position = pos;
    }

    // geser bongkahan batu supaya PUSAT-nya tepat di titik putar objek -> putaran = menggelinding, bukan menjungkir
    void PusatkanVisual()
    {
        vis = transform.Find("Visual");
        if (vis != null) vis.localPosition = new Vector3(0f, -0.52f, 0f);
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        if (vis == null) PusatkanVisual();

        // datang dari depan (-z) + gelinding searah gerak (sumbu -X) dgn laju putar = v/r
        transform.position += Vector3.back * kecepatan * Time.deltaTime;
        float derajat = kecepatan / 0.65f * Mathf.Rad2Deg * Time.deltaTime;
        transform.Rotate(Vector3.left * derajat, Space.World);

        if (cooldown > 0f) cooldown -= Time.deltaTime;

        // sudah lewat di belakang pemain -> ulang ke depan dengan lane acak
        if (player != null && transform.position.z < player.position.z - 8f)
        {
            Vector3 pos = transform.position;
            pos.z = player.position.z + jarakUlang;
            pos.x = lane[Random.Range(0, lane.Length)];
            pos.y = tinggiDuduk;
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
