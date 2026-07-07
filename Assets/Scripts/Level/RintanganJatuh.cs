using UnityEngine;

// RINTANGAN MENDADAK: bongkahan batu menggantung di langit-langit, JATUH saat pemain mendekat.
// [ExecuteAlways] biar tampil di editor. Kena pemain -> HP berkurang.
[ExecuteAlways]
public class RintanganJatuh : MonoBehaviour
{
    public float jarakPicu = 22f;      // jarak pemain sebelum batu jatuh
    public float kecepatanJatuh = 16f;
    public float tinggiTanah = 0.6f;   // y saat sudah di lantai
    Transform player, vis;
    bool jatuh, kena;

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

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material batu = Buat(s, new Color(0.3f, 0.28f, 0.26f));

        GameObject v = new GameObject("Visual");
        v.hideFlags = HideFlags.DontSave;
        v.transform.SetParent(transform, false);
        vis = v.transform;

        Bongkah(new Vector3(0f, 0f, 0f), new Vector3(1.4f, 1.2f, 1.4f), new Vector3(15f, 20f, 10f), batu);
        Bongkah(new Vector3(-0.6f, 0.1f, 0.3f), new Vector3(0.9f, 0.8f, 0.9f), new Vector3(-20f, 40f, 10f), batu);
        Bongkah(new Vector3(0.6f, -0.1f, -0.2f), new Vector3(0.8f, 0.7f, 0.8f), new Vector3(25f, -15f, -12f), batu);
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (!Application.isPlaying || kena) return;
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform; else return;
        }

        if (!jatuh)
        {
            float beda = transform.position.z - player.position.z;
            if (beda < jarakPicu && beda > -3f) jatuh = true;
        }
        else
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.MoveTowards(pos.y, tinggiTanah, kecepatanJatuh * Time.deltaTime);
            transform.position = pos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Application.isPlaying || kena) return;
        if (other.CompareTag("Player"))
        {
            kena = true;
            if (RunGame.instance != null) RunGame.instance.Kena();
            Renderer[] rs = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rs) r.enabled = false;
            Collider c = GetComponent<Collider>();
            if (c != null) c.enabled = false;
        }
    }

    Material Buat(Shader s, Color c)
    {
        Material m = new Material(s);
        m.hideFlags = HideFlags.DontSave;
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    void Bongkah(Vector3 lp, Vector3 ls, Vector3 rot, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.hideFlags = HideFlags.DontSave;
        g.transform.SetParent(vis, false);
        g.transform.localPosition = lp;
        g.transform.localScale = ls;
        g.transform.localRotation = Quaternion.Euler(rot);
        g.GetComponent<Renderer>().sharedMaterial = m;
        Collider c = g.GetComponent<Collider>();
        if (c != null) Hancur(c);
    }
}
