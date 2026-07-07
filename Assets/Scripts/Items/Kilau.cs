using UnityEngine;

// Efek KILAU/percikan saat ambil item. Spawn kubus-kubus kecil terbang lalu mengecil. Play-only.
public class Kilau : MonoBehaviour
{
    Transform[] part;
    Vector3[] vel;
    float umur;
    const float LAMA = 0.6f;

    public static void Ledak(Vector3 pos, Color warna)
    {
        if (!Application.isPlaying) return;
        GameObject go = new GameObject("Kilau");
        go.transform.position = pos;
        go.AddComponent<Kilau>().Mulai(warna);
    }

    void Mulai(Color warna)
    {
        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material m = new Material(s);
        m.color = warna;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", warna);
        if (m.HasProperty("_EmissionColor"))
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", warna * 2.5f);
        }

        int n = 12;
        part = new Transform[n];
        vel = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Collider c = g.GetComponent<Collider>();
            if (c != null) Destroy(c);
            g.transform.SetParent(transform, false);
            g.transform.localScale = Vector3.one * 0.16f;
            g.GetComponent<Renderer>().sharedMaterial = m;
            part[i] = g.transform;
            vel[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(0.6f, 2f), Random.Range(-1f, 1f)) * 3.5f;
        }
        Destroy(gameObject, LAMA);
    }

    void Update()
    {
        if (part == null) return;
        umur += Time.deltaTime;
        float skala = Mathf.Max(0f, 0.16f * (1f - umur / LAMA));
        for (int i = 0; i < part.Length; i++)
        {
            if (part[i] == null) continue;
            part[i].position += vel[i] * Time.deltaTime;
            vel[i] += Vector3.down * 6f * Time.deltaTime;
            part[i].localScale = Vector3.one * skala;
            part[i].Rotate(120f * Time.deltaTime, 90f * Time.deltaTime, 0f);
        }
    }
}
