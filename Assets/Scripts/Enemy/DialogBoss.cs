using UnityEngine;
using UnityEngine.UI;

// Dialog boss: saat pemain mendekat, tampilkan teks tantangan boss lalu balasan kurir. Play-only.
public class DialogBoss : MonoBehaviour
{
    public string dialogBoss = "Kau tak akan lewat!";
    public string dialogKurir = "Paket ini harus sampai!";
    public float jarakPicu = 30f;

    Transform player;
    Text teks;
    bool mulai = false;
    float timer = 0f;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        BuatTeks();
    }

    void BuatTeks()
    {
        GameObject cvGo = new GameObject("DialogBossUI");
        Canvas cv = cvGo.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 350;
        CanvasScaler cs = cvGo.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);

        GameObject t = new GameObject("Teks");
        t.transform.SetParent(cvGo.transform, false);
        teks = t.AddComponent<Text>();
        teks.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        teks.fontSize = 40;
        teks.fontStyle = FontStyle.Bold;
        teks.alignment = TextAnchor.MiddleCenter;
        teks.supportRichText = false;
        teks.horizontalOverflow = HorizontalWrapMode.Overflow;
        teks.raycastTarget = false;
        teks.color = Color.white;
        RectTransform rt = teks.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.78f);
        rt.anchorMax = new Vector2(0.5f, 0.78f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(1500, 120);
        teks.text = "";
    }

    void Update()
    {
        if (!Application.isPlaying || player == null || teks == null) return;

        if (!mulai)
        {
            float dz = transform.position.z - player.position.z;
            if (dz <= jarakPicu && dz > -5f) { mulai = true; timer = 0f; }
            return;
        }

        timer += Time.deltaTime;
        if (timer < 2f)
        {
            teks.color = new Color(1f, 0.5f, 0.4f);
            teks.text = "Boss: " + dialogBoss;
        }
        else if (timer < 4f)
        {
            teks.color = new Color(0.5f, 0.9f, 1f);
            teks.text = "Kurir: " + dialogKurir;
        }
        else
        {
            teks.text = "";
        }
    }
}
