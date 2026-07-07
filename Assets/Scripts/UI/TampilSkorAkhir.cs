using UnityEngine;
using UnityEngine.UI;

// Dipasang pada Text di layar TAMAT: simpan rekor lalu tampilkan skor akhir + rekor.
public class TampilSkorAkhir : MonoBehaviour
{
    public Text teks;

    void Start()
    {
        SkorGame.SimpanRekor();
        if (teks == null) teks = GetComponent<Text>();
        if (teks != null)
            teks.text = "Skor Akhir: " + SkorGame.total + "\nRekor: " + SkorGame.Rekor();
    }
}
