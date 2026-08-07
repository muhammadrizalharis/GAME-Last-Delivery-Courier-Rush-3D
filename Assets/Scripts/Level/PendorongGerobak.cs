using UnityEngine;

// Pendorong gerobak berjalan maju (+z) menyusuri pasar, kembali ke awal saat sampai ujung. Play-only.
public class PendorongGerobak : MonoBehaviour
{
    public float kecepatan = 1.2f;
    public float mulaiZ = 15f;
    public float akhirZ = 330f;
    float baseY, fase;

    void Start()
    {
        baseY = transform.position.y;
        fase = Random.value * 6.28f;
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        Vector3 p = transform.position;
        p.z += kecepatan * Time.deltaTime;
        if (p.z > akhirZ) p.z = mulaiZ;
        p.y = baseY + Mathf.Abs(Mathf.Sin(Time.time * 5f + fase)) * 0.05f;
        transform.position = p;
    }
}
