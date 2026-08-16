using UnityEngine;

// Menyebar KERUMUNAN pasar: PENJAGA di tiap warung (kiri-kanan) + pengunjung yang melihat dagangan
// dan yang berjalan dari warung ke warung. Dekorasi Play-only di luar jalur pemain (tak menabrak).
public class KerumunanPasar : MonoBehaviour
{
    public int jumlahPengunjung = 14;
    public float mulaiZ = -20f;
    public float akhirZ = 320f;
    public float jarakStan = 18f;

    static readonly Color[] baju = {
        new Color(0.85f, 0.3f, 0.3f), new Color(0.3f, 0.4f, 0.75f), new Color(0.35f, 0.6f, 0.4f),
        new Color(0.85f, 0.7f, 0.3f), new Color(0.6f, 0.4f, 0.7f), new Color(0.7f, 0.5f, 0.35f),
        new Color(0.9f, 0.85f, 0.8f), new Color(0.4f, 0.4f, 0.45f)
    };

    void Start()
    {
        if (!Application.isPlaying) return;

        // penjaga di tiap warung kiri & kanan (menghadap ke tengah/pembeli)
        for (float z = mulaiZ; z <= akhirZ; z += jarakStan)
        {
            if (z < 12f) continue;
            Spawn(6.2f, z, 2, -90f);
            Spawn(-6.2f, z, 2, 90f);
        }

        // pengunjung: KEBANYAKAN berlalu-lalang warung ke warung, sebagian berhenti melihat dagangan
        for (int i = 0; i < jumlahPengunjung; i++)
        {
            float sisi = (i % 2 == 0) ? 1f : -1f;
            float z = Mathf.Lerp(18f, akhirZ - 10f, (i + 0.5f) / jumlahPengunjung) + Random.Range(-5f, 5f);
            if (Random.value < 0.62f)
                Spawn(sisi * Random.Range(4.9f, 5.7f), z, 1, 0f, Random.Range(9f, 22f));   // berlalu-lalang (jarak beragam)
            else
                Spawn(sisi * 5.1f, z, 0, sisi > 0f ? 90f : -90f);                          // berhenti lihat dagangan
        }

        // pedagang mendorong gerobak keliling pasar
        for (int i = 0; i < 4; i++)
        {
            float sisi = (i % 2 == 0) ? 1f : -1f;
            float z = Mathf.Lerp(30f, akhirZ - 20f, (i + 0.5f) / 4f);
            GameObject g = new GameObject("GerobakDorong");
            g.SetActive(false);
            g.transform.position = new Vector3(sisi * 4.6f, 0f, z);
            RakitOrang ro = g.AddComponent<RakitOrang>();
            ro.warna = baju[Random.Range(0, baju.Length)];
            RakitGerobak rg = g.AddComponent<RakitGerobak>();
            rg.warna = baju[Random.Range(0, baju.Length)];
            PendorongGerobak pd = g.AddComponent<PendorongGerobak>();
            pd.mulaiZ = 15f;
            pd.akhirZ = akhirZ;
            pd.kecepatan = Random.Range(1f, 1.6f);
            g.SetActive(true);
        }
    }

    void Spawn(float x, float z, int mode, float hadap, float jarak = 12f)
    {
        GameObject g = new GameObject("Warga");
        g.SetActive(false);
        g.transform.position = new Vector3(x, 0f, z);
        RakitOrang ro = g.AddComponent<RakitOrang>();
        ro.warna = baju[Random.Range(0, baju.Length)];
        WargaPasar w = g.AddComponent<WargaPasar>();
        w.mode = mode;
        w.hadap = hadap;
        w.jarak = jarak;
        g.SetActive(true);
    }
}
