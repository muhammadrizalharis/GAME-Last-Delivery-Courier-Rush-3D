using UnityEngine;

// Perilaku warga pasar: 0=pengunjung melihat dagangan, 1=jalan menyusuri trotoar (warung ke warung),
// 2=penjaga warung. Yang diam pun bergerak halus (bob + menoleh) supaya tidak kaku.
public class WargaPasar : MonoBehaviour
{
    public int mode;
    public float hadap;   // arah hadap dasar (derajat Y) untuk yang diam
    public float jarak = 12f;   // setengah rentang jalan (mode 1) -> menentukan seberapa jauh berlalu-lalang
    float baseZ, baseY, baseX, kecepatan, arahZ = 1f, fase, faseYaw;

    void Start()
    {
        baseZ = transform.position.z;
        baseY = transform.position.y;
        baseX = transform.position.x;
        kecepatan = Random.Range(0.6f, 1.4f);
        arahZ = Random.value < 0.5f ? 1f : -1f;
        fase = Random.value * 6.28f;
        faseYaw = Random.value * 6.28f;
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        Vector3 p = transform.position;
        if (mode == 1)
        {
            p.z += arahZ * kecepatan * Time.deltaTime;
            p.y = baseY + Mathf.Abs(Mathf.Sin(Time.time * 6f + fase)) * 0.06f;
            p.x = baseX + Mathf.Sin(Time.time * 0.5f + faseYaw) * 0.6f;   // weave dekat/jauh warung -> berlalu-lalang
            transform.position = p;
            transform.rotation = Quaternion.Euler(0f, arahZ > 0f ? 0f : 180f, 0f);
            if (p.z > baseZ + jarak) arahZ = -1f;
            else if (p.z < baseZ - jarak) arahZ = 1f;
        }
        else
        {
            p.y = baseY + Mathf.Abs(Mathf.Sin(Time.time * 2f + fase)) * 0.03f;
            transform.position = p;
            float yaw = hadap + Mathf.Sin(Time.time * 0.8f + faseYaw) * 12f;
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }
}
