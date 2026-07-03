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

    public static RunGame instance;
    bool selesai = false;

    void Awake() { instance = this; }

    void Start()
    {
        Time.timeScale = 1f;
        if (teksPesan) teksPesan.text = "";
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
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (teksWaktu)
        {
            int t = Mathf.CeilToInt(waktu);
            teksWaktu.text = string.Format("{0:00}:{1:00}", t / 60, t % 60);
        }
    }

    // dipanggil rintangan saat kena player
    public void Kena()
    {
        if (selesai) return;
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
