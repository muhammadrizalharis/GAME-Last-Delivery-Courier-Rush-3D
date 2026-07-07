using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// Pengatur Scene 2 (lari di jalan): HP, timer, menang/kalah, HUD
public class RunGame : MonoBehaviour
{
    public int nyawa = 3;
    public float waktu = 60f;
    public GameObject[] hati;      // 3 ikon hati di HUD
    public Text teksWaktu;
    public Text teksPesan;
    public string sceneBerikut = "Menu";
    public int skor = 0;
    public Text teksSkor;

    public static RunGame instance;
    bool selesai = false;
    float sisaKebal = 0f;
    Renderer[] pemainRends;
    Color[] warnaAsli;
    bool kebalTadi = false;

    void Awake() { instance = this; }

    void Start()
    {
        Time.timeScale = 1f;
        if (teksPesan) teksPesan.text = "";
        GameObject pl = GameObject.FindGameObjectWithTag("Player");
        if (pl != null)
        {
            pemainRends = pl.GetComponentsInChildren<Renderer>();
            warnaAsli = new Color[pemainRends.Length];
            for (int i = 0; i < pemainRends.Length; i++)
                warnaAsli[i] = pemainRends[i].material.color;
        }
        UpdateHUD();
    }

    void Update()
    {
        if (selesai)
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return;
        }

        waktu -= Time.deltaTime;
        if (waktu <= 0f) { waktu = 0f; Kalah("WAKTU HABIS"); }

        // perisai / kebal sementara
        bool kebal = sisaKebal > 0f;
        if (kebal)
        {
            sisaKebal -= Time.deltaTime;
            if (pemainRends != null)
            {
                float k = 0.5f + 0.5f * Mathf.Sin(Time.time * 12f);
                Color c = Color.Lerp(new Color(0.25f, 0.9f, 1f), Color.white, k);
                for (int i = 0; i < pemainRends.Length; i++)
                {
                    if (pemainRends[i] == null) continue;
                    pemainRends[i].material.color = c;
                    if (pemainRends[i].material.HasProperty("_BaseColor")) pemainRends[i].material.SetColor("_BaseColor", c);
                }
            }
        }
        else if (kebalTadi && pemainRends != null)
        {
            for (int i = 0; i < pemainRends.Length; i++)
            {
                if (pemainRends[i] == null || warnaAsli == null || i >= warnaAsli.Length) continue;
                pemainRends[i].material.color = warnaAsli[i];
                if (pemainRends[i].material.HasProperty("_BaseColor")) pemainRends[i].material.SetColor("_BaseColor", warnaAsli[i]);
            }
        }
        kebalTadi = kebal;

        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (teksWaktu)
        {
            int t = Mathf.CeilToInt(waktu);
            teksWaktu.text = string.Format("{0:00}:{1:00}", t / 60, t % 60);
        }
        if (teksSkor) teksSkor.text = "Skor: " + skor;
    }

    // dipanggil rintangan saat kena player
    public void Kena()
    {
        if (selesai) return;
        if (sisaKebal > 0f) return; // sedang kebal -> tak kena
        nyawa--;
        if (hati != null && nyawa >= 0 && nyawa < hati.Length && hati[nyawa] != null)
            hati[nyawa].SetActive(false);
        if (nyawa <= 0) { nyawa = 0; Kalah("GAME OVER"); }
    }

    // dipanggil item hati untuk tambah nyawa
    public void TambahNyawa()
    {
        if (selesai) return;
        if (hati != null && nyawa < hati.Length)
        {
            if (hati[nyawa] != null) hati[nyawa].SetActive(true);
            nyawa++;
        }
    }

    // dipanggil item jam untuk tambah waktu
    public void TambahWaktu(float detik)
    {
        if (selesai) return;
        waktu += detik;
    }

    public void TambahSkor(int n)
    {
        if (selesai) return;
        skor += n;
        UpdateHUD();
    }

    public void AktifkanPerisai(float durasi)
    {
        if (selesai) return;
        if (durasi > sisaKebal) sisaKebal = durasi;
    }

    public void Menang()
    {
        if (selesai) return;
        selesai = true;
        if (teksPesan) teksPesan.text = "PAKET TERKIRIM!\nTekan R untuk ulang";
        Time.timeScale = 0f;
    }

    void Kalah(string pesan)
    {
        if (selesai) return;
        selesai = true;
        if (teksPesan) teksPesan.text = pesan + "\nTekan R untuk ulang";
        Time.timeScale = 0f;
    }
}
