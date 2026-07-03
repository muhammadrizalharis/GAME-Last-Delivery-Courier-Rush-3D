using UnityEngine;

// Terus memunculkan mobil yang melaju ke arah pemain -> jalan jadi ramai.
public class MobilSpawner : MonoBehaviour
{
    public float interval = 1.2f;
    float t = 1f;
    Transform player;
    Color[] warna = {
        new Color(0.3f, 0.5f, 0.85f), new Color(0.85f, 0.25f, 0.2f),
        new Color(0.9f, 0.7f, 0.3f), new Color(0.4f, 0.7f, 0.45f), new Color(0.9f, 0.9f, 0.95f)
    };
    float[] lane = { -3f, 0f, 3f };

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;
        t -= Time.deltaTime;
        if (t <= 0f) { t = interval; Spawn(); }
    }

    void Spawn()
    {
        GameObject car = new GameObject("MobilLalu");
        car.transform.position = new Vector3(lane[Random.Range(0, lane.Length)], 0.7f, player.position.z + 95f);
        BoxCollider bc = car.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(2.4f, 1.4f, 3.6f);
        RakitMobil rm = car.AddComponent<RakitMobil>();
        rm.warna = warna[Random.Range(0, warna.Length)];
        MobilJalan mj = car.AddComponent<MobilJalan>();
        mj.kecepatan = Random.Range(11f, 17f);
        mj.spawnMode = true;
    }
}
