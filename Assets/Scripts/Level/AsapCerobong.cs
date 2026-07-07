using UnityEngine;
using System.Collections.Generic;

// Kepulan asap dari cerobong lokomotif. Hanya aktif saat Play. Primitif + kode.
public class AsapCerobong : MonoBehaviour
{
    public float interval = 0.3f;
    public Vector3 offset = new Vector3(0f, 1.95f, -1.0f); // posisi cerobong (lokal ke kereta)
    float timer;
    Material mat;
    readonly List<Transform> puff = new List<Transform>();
    readonly List<float> umur = new List<float>();

    void Start()
    {
        if (!Application.isPlaying) return;
        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        mat = new Material(s);
        Color c = new Color(0.85f, 0.85f, 0.88f);
        mat.color = c;
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Collider col = g.GetComponent<Collider>();
            if (col != null) Destroy(col);
            g.transform.position = transform.TransformPoint(offset);
            g.transform.localScale = Vector3.one * 0.4f;
            g.GetComponent<Renderer>().sharedMaterial = mat;
            puff.Add(g.transform);
            umur.Add(0f);
        }

        for (int i = puff.Count - 1; i >= 0; i--)
        {
            if (puff[i] == null) { puff.RemoveAt(i); umur.RemoveAt(i); continue; }
            umur[i] += Time.deltaTime;
            puff[i].position += new Vector3(0.15f, 1.3f, 0f) * Time.deltaTime;
            puff[i].localScale += Vector3.one * (0.6f * Time.deltaTime);
            if (umur[i] > 1.2f)
            {
                Destroy(puff[i].gameObject);
                puff.RemoveAt(i);
                umur.RemoveAt(i);
            }
        }
    }
}
