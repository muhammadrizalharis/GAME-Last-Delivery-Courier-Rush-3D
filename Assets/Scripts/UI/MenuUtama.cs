using UnityEngine;
using UnityEngine.SceneManagement;

// Mengatur tombol di layar judul (Scene Menu)
public class MenuUtama : MonoBehaviour
{
    public string sceneLevel1 = "Level1Scene1";

    public void Mulai()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneLevel1);
    }

    public void Keluar()
    {
        Debug.Log("Keluar game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
