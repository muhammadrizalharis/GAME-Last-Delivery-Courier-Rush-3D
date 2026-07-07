using UnityEngine;
using UnityEngine.UI;

// Dipasang pada Text di Menu: menampilkan rekor skor tertinggi.
public class SkorMenu : MonoBehaviour
{
    void Start()
    {
        Text t = GetComponent<Text>();
        if (t != null) t.text = "Rekor: " + SkorGame.Rekor();
    }
}
