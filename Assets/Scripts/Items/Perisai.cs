using UnityEngine;

// Item PERISAI: diambil pemain -> kebal sementara. [ExecuteAlways] -> tampil di editor.
[ExecuteAlways]
public class Perisai : MonoBehaviour
{
    public float durasi = 5f;
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
        Material biru = new Material(s);
        biru.hideFlags = HideFlags.DontSave;
        Color c = new Color(0.25f, 0.85f, 1f);
        biru.color = c;
        if (biru.HasProperty("_BaseColor")) biru.SetColor("_BaseColor", c);
        if (biru.HasProperty("_EmissionColor")) { biru.EnableKeyword("_EMISSION"); biru.SetColor("_EmissionColor", c * 1.3f); }

        GameObject v = new GameObject("Visual");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        vis = v.transform;

        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(vis, false);
        g.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        g.GetComponent<Renderer>().sharedMaterial = biru;
        Collider col = g.GetComponent<Collider>();
        if (col != null) Hancur(col);
    }

    void Update()
    {
        if (Application.isPlaying) transform.Rotate(0f, 90f * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Application.isPlaying) return;
        if (other.CompareTag("Player"))
        {
            if (RunGame.instance != null) RunGame.instance.AktifkanPerisai(durasi);
            Kilau.Ledak(transform.position, new Color(0.25f, 0.85f, 1f));
            Destroy(gameObject);
        }
    }
}
