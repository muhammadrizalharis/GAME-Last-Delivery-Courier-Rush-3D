using UnityEngine;

// Orang naik-turun tangga: bergerak bolak-balik (PingPong) antara titik bawah & atas mengikuti kemiringan. Play-only.
// Dipakai bersama RakitOrang (kaki otomatis melangkah karena posisi berubah).
public class OrangTangga : MonoBehaviour
{
    public Vector3 bawah, atas;
    public float kecepatan = 1f;
    float t;
    Vector3 prev;

    void Start() { t = Random.value; prev = transform.position; }

    void Update()
    {
        if (!Application.isPlaying) return;
        t += Time.deltaTime * kecepatan * 0.12f;
        float f = Mathf.PingPong(t, 1f);
        Vector3 p = Vector3.Lerp(bawah, atas, f);
        Vector3 d = p - prev;
        prev = p;
        transform.position = p;
        Vector3 datar = new Vector3(d.x, 0f, d.z);
        if (datar.sqrMagnitude > 1e-6f)
            transform.rotation = Quaternion.LookRotation(datar.normalized);
    }
}
