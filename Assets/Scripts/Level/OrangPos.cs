using UnityEngine;

// Perilaku orang: 0 = diam (mengobrol / melayani -> bob halus + menoleh ke 'hadap'),
// 1 = berjalan bolak-balik pada sumbu 'arah', 2 = berjalan sambil melompat-lompat. Bergerak hanya saat Play.
public class OrangPos : MonoBehaviour
{
    public int mode;                    // 0 diam, 1 jalan, 2 jalan+lompat
    public float hadap;                 // yaw dasar (derajat) untuk yang diam
    public Vector3 arah = Vector3.forward; // arah jalan (mode 1)
    public float jarak = 3.5f;          // setengah rentang bolak-balik
    public float kecepatan = 1f;

    Vector3 basePos;
    float fase, t;

    void Start()
    {
        basePos = transform.position;
        fase = Random.value * 6.283f;
        t = Random.value * 6.283f;
        kecepatan *= Random.Range(0.8f, 1.2f);
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        if (mode == 1 || mode == 2)
        {
            t += Time.deltaTime * kecepatan;
            Vector3 dir = arah.sqrMagnitude > 0.0001f ? arah.normalized : Vector3.forward;
            Vector3 p = basePos + dir * (Mathf.Sin(t) * jarak);
            if (mode == 2)
                p.y = basePos.y + Mathf.Abs(Mathf.Sin(t * 2.2f)) * 0.5f;      // melompat-lompat
            else
                p.y = basePos.y;                                             // langkah ditangani animasi RakitOrang
            transform.position = p;
            Vector3 g = dir * (Mathf.Cos(t) >= 0f ? 1f : -1f);
            transform.rotation = Quaternion.Euler(0f, Mathf.Atan2(g.x, g.z) * Mathf.Rad2Deg, 0f);
        }
        else
        {
            Vector3 p = transform.position;
            p.y = basePos.y + Mathf.Abs(Mathf.Sin(Time.time * 2f + fase)) * 0.03f;
            transform.position = p;
            transform.rotation = Quaternion.Euler(0f, hadap + Mathf.Sin(Time.time * 0.8f + fase) * 10f, 0f);
        }
    }
}
