using UnityEngine;
using UnityEngine.SceneManagement;

// Tombol navigasi antar scene (dipakai layar Level 2 Unlocked, dll)
public class NavTombol : MonoBehaviour
{
    public void GantiScene(string namaScene)
    {
        Transisi.Pindah(namaScene);
    }

    public void MainLagi()
    {
        SkorGame.Reset();
        Transisi.Pindah("Level1Scene1");
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
