using UnityEngine;

// Mobil berjalan terus ke arah pemain (lawan arah), loop / hancur saat lewat. Kena player -> HP berkurang.
public class MobilJalan : MonoBehaviour
{
    public float kecepatan = 12f;
    public bool spawnMode = false;   // true = dibuat spawner (hancur saat lewat)
    Transform player;
    float[] lane = { -3f, 0f, 3f };
    bool cooldown = false;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        transform.position += Vector3.back * kecepatan * Time.deltaTime;
        if (player != null && transform.position.z < player.position.z - 15f)
        {
            if (spawnMode) { Destroy(gameObject); return; }
            Ulang();
        }
    }

    void Ulang()
    {
        if (player == null) return;
        Vector3 p = transform.position;
        p.z = player.position.z + 90f + Random.Range(0f, 40f);
        p.x = lane[Random.Range(0, lane.Length)];
        transform.position = p;
        cooldown = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cooldown)
        {
            cooldown = true;
            if (RunGame.instance != null) RunGame.instance.Kena();
            if (spawnMode) Destroy(gameObject, 0.1f);
            else Ulang();
        }
    }
}
