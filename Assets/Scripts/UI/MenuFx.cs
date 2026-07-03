using UnityEngine;
using UnityEngine.UI;

// Efek sederhana untuk menu: melayang, denyut, kedip. Pakai kode (bukan asset)
public class MenuFx : MonoBehaviour
{
    public enum Efek { Melayang, Denyut, Kedip }
    public Efek efek = Efek.Melayang;
    public float kecepatan = 2f;
    public float kekuatan = 15f;

    RectTransform rt;
    Vector2 posAwal;
    Vector3 skalaAwal;
    Graphic gfx;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        if (rt != null) posAwal = rt.anchoredPosition;
        skalaAwal = transform.localScale;
        gfx = GetComponent<Graphic>();
    }

    void Update()
    {
        float t = Mathf.Sin(Time.unscaledTime * kecepatan);
        if (efek == Efek.Melayang && rt != null)
            rt.anchoredPosition = posAwal + new Vector2(0, t * kekuatan);
        else if (efek == Efek.Denyut)
            transform.localScale = skalaAwal * (1f + t * 0.05f);
        else if (efek == Efek.Kedip && gfx != null)
        {
            Color c = gfx.color;
            c.a = 0.4f + 0.6f * (t * 0.5f + 0.5f);
            gfx.color = c;
        }
    }
}
