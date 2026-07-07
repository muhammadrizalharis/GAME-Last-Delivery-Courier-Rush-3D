using UnityEngine;

// Skor global lintas-level + rekor tersimpan (PlayerPrefs). Bukan MonoBehaviour.
public static class SkorGame
{
    public static int total = 0;

    public static void Reset() { total = 0; }

    public static void Tambah(int n) { total += n; }

    public static int Rekor() { return PlayerPrefs.GetInt("RekorSkor", 0); }

    public static void SimpanRekor()
    {
        if (total > Rekor())
        {
            PlayerPrefs.SetInt("RekorSkor", total);
            PlayerPrefs.Save();
        }
    }
}
