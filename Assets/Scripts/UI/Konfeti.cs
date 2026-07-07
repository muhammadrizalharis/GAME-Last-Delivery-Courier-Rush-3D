using UnityEngine;

// Konfeti jatuh (efek merayakan). Primitif + kode, hanya aktif saat Play.
public class Konfeti : MonoBehaviour
{
    public int jumlah = 45;
    Transform[] potong;
    Vector3[] kecepatan;
    Vector3[] putar;

    void Start()
    {
        if (!Application.isPlaying) return;

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Color[] warna = {
            new Color(1f, 0.3f, 0.3f), new Color(0.3f, 0.7f, 1f), new Color(1f, 0.85f, 0.2f),
            new Color(0.4f, 0.9f, 0.4f), new Color(0.9f, 0.4f, 0.9f)
        };
        Material[] mat = new Material[warna.Length];
        for (int i = 0; i < warna.Length; i++)
        {
            mat[i] = new Material(s);
            mat[i].color = warna[i];
            if (mat[i].HasProperty("_BaseColor")) mat[i].SetColor("_BaseColor", warna[i]);
        }

        potong = new Transform[jumlah];
        kecepatan = new Vector3[jumlah];
        putar = new Vector3[jumlah];
        for (int i = 0; i < jumlah; i++)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(g.GetComponent<Collider>());
            g.transform.SetParent(transform, false);
            g.transform.localScale = new Vector3(0.13f, 0.13f, 0.02f);
            g.GetComponent<Renderer>().sharedMaterial = mat[Random.Range(0, mat.Length)];
            potong[i] = g.transform;
            Lahir(i, true);
        }
    }

    void Lahir(int i, bool acak)
    {
        float x = Random.Range(-4.5f, 4.5f);
        float y = acak ? Random.Range(1f, 5.5f) : Random.Range(3.5f, 6f);
        float z = Random.Range(-0.6f, 1.6f);
        potong[i].localPosition = new Vector3(x, y, z);
        kecepatan[i] = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-1.3f, -2.4f), 0f);
        putar[i] = new Vector3(Random.Range(-200f, 200f), Random.Range(-200f, 200f), 0f);
    }

    void Update()
    {
        if (potong == null) return;
        for (int i = 0; i < potong.Length; i++)
        {
            potong[i].localPosition += kecepatan[i] * Time.deltaTime;
            potong[i].Rotate(putar[i] * Time.deltaTime, Space.Self);
            if (potong[i].localPosition.y < -3.2f) Lahir(i, false);
        }
    }
}
