using UnityEngine;

// Batu besar MENGGELINDING ke arah pemain (-z) di terowongan. Berputar seperti roda.
// Begitu lewat di belakang pemain -> pindah maju lagi ke lane acak (ancaman terus-menerus).
// Kena pemain -> RunGame.Kena().
public class BatuGelinding : MonoBehaviour
{
    public float kecepatan = 9f;
    public float jarakUlang = 75f;   // jarak di depan pemain saat di-reset
    float cooldown = 0f;
    Transform player;
    static readonly float[] lane = { -3f, 0f, 3f };

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        // maju ke pemain + berputar seperti menggelinding
        transform.position += Vector3.back * kecepatan * Time.deltaTime;
        transform.Rotate(Vector3.right * kecepatan * 45f * Time.deltaTime, Space.World);

        if (cooldown > 0f) cooldown -= Time.deltaTime;

        // sudah lewat di belakang pemain -> ulang ke depan dengan lane acak
        if (player != null && transform.position.z < player.position.z - 8f)
        {
            Vector3 pos = transform.position;
            pos.z = player.position.z + jarakUlang;
            pos.x = lane[Random.Range(0, lane.Length)];
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
