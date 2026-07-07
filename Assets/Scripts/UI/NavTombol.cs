using UnityEngine;
using UnityEngine.SceneManagement;

// Tombol navigasi antar scene (dipakai layar Level 2 Unlocked, dll)
public class NavTombol : MonoBehaviour
{
    public void GantiScene(string namaScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(namaScene);
    }

    public void Keluar()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
