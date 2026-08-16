using System.Collections.Generic;
using UnityEngine;

// Menulis teks dari KUBUS (huruf balok 3x5) -> geometri solid: TIDAK tembus tembok & TIDAK kebalik.
// Dipakai untuk papan nama gedung & tulisan pada box paket. Teks menghadap sumbu X (jalan kota membujur Z).
public static class HurufBalok
{
    // font 3 kolom x 5 baris ('1' = terisi)
    static readonly Dictionary<char, string[]> font = new Dictionary<char, string[]>
    {
        { 'A', new[] { "010", "101", "111", "101", "101" } },
        { 'B', new[] { "110", "101", "110", "101", "110" } },
        { 'E', new[] { "111", "100", "110", "100", "111" } },
        { 'F', new[] { "111", "100", "110", "100", "100" } },
        { 'H', new[] { "101", "101", "111", "101", "101" } },
        { 'K', new[] { "101", "101", "110", "101", "101" } },
        { 'L', new[] { "100", "100", "100", "100", "111" } },
        { 'N', new[] { "101", "111", "111", "111", "101" } },
        { 'O', new[] { "010", "101", "101", "101", "010" } },
        { 'P', new[] { "110", "101", "110", "100", "100" } },
        { 'R', new[] { "110", "101", "110", "101", "101" } },
        { 'S', new[] { "111", "100", "111", "001", "111" } },
        { 'T', new[] { "111", "010", "010", "010", "010" } },
    };

    // Bangun teks di bawah 'induk'. pusat = titik tengah teks (lokal). tinggi = tinggi huruf.
    // hadap = +1 (teks menghadap +X) atau -1 (menghadap -X). Warna huruf.
    public static void Tulis(Transform induk, string teks, Vector3 pusat, float tinggi, int hadap, Color warna)
    {
        if (string.IsNullOrEmpty(teks) || induk == null) return;
        teks = teks.ToUpper();
        int h = hadap >= 0 ? 1 : -1;
        float sel = tinggi / 5f;
        float majuHuruf = 4f * sel;                       // 3 kolom + 1 spasi
        float total = teks.Length * majuHuruf - sel;
        float mulai = -total * 0.5f;
        float tebal = Mathf.Max(0.05f, sel * 0.7f);

        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material m = new Material(s) { hideFlags = HideFlags.DontSave };
        m.color = warna;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", warna);

        for (int i = 0; i < teks.Length; i++)
        {
            char c = teks[i];
            if (!font.ContainsKey(c)) continue;            // spasi / tak dikenal -> lewati (tetap maju)
            string[] g = font[c];
            float baseR = mulai + i * majuHuruf;
            for (int row = 0; row < 5; row++)
                for (int col = 0; col < 3; col++)
                {
                    if (g[row][col] != '1') continue;
                    float r = baseR + col * sel;            // sepanjang "kanan pembaca"
                    float y = (2 - row) * sel;              // baris 0 = atas
                    GameObject px = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    px.name = "Hrf"; px.hideFlags = HideFlags.DontSave;
                    px.transform.SetParent(induk, false);
                    px.transform.localPosition = pusat + new Vector3(0f, y, h * r);
                    px.transform.localScale = new Vector3(tebal, sel * 0.96f, sel * 0.96f);
                    px.GetComponent<Renderer>().sharedMaterial = m;
                    Collider col2 = px.GetComponent<Collider>();
                    if (col2 != null) { if (Application.isPlaying) Object.Destroy(col2); else Object.DestroyImmediate(col2); }
                }
        }
    }
}
