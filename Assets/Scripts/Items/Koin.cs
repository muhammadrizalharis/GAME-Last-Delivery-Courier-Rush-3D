using UnityEngine;

// Koin emas berputar. Diambil pemain -> tambah skor. [ExecuteAlways] -> tampil di editor.
[ExecuteAlways]
public class Koin : MonoBehaviour
{
    public int nilai = 10;
    Transform vis;

    void OnEnable() { Bangun(); }

    void Hancur(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void Bangun()
    {
        Transform lama = transform.Find("Visual");
        if (lama != null) Hancur(lama.gameObject);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material emas = new Material(s);
        emas.hideFlags = HideFlags.DontSave;
        Color c = new Color(1f, 0.84f, 0.15f);
        emas.color = c;
        if (emas.HasProperty("_BaseColor")) emas.SetColor("_BaseColor", c);
        if (emas.HasProperty("_Metallic")) emas.SetFloat("_Metallic", 0.7f);
        if (emas.HasProperty("_Smoothness")) emas.SetFloat("_Smoothness", 0.75f);

        GameObject v = new GameObject("Visual");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        vis = v.transform;

        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(vis, false);
        g.transform.localScale = new Vector3(0.7f, 0.08f, 0.7f);
        g.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        g.GetComponent<Renderer>().sharedMaterial = emas;
        Collider col = g.GetComponent<Collider>();
        if (col != null) Hancur(col);
    }

    void Update()
    {
        if (Application.isPlaying) transform.Rotate(0f, 140f * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Application.isPlaying) return;
        if (other.CompareTag("Player"))
        {
            if (RunGame.instance != null) RunGame.instance.TambahSkor(nilai);
            Kilau.Ledak(transform.position, new Color(1f, 0.84f, 0.15f));
            Destroy(gameObject);
        }
    }
}
