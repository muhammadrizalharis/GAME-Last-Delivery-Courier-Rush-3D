using UnityEngine;

// Rintangan di jalan: kena player -> kurangi HP, lalu nonaktif biar tidak dobel
public class RintanganLari : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (RunGame.instance != null) RunGame.instance.Kena();
            Renderer r = GetComponent<Renderer>();
            if (r != null) r.enabled = false;
            Collider c = GetComponent<Collider>();
            if (c != null) c.enabled = false;
        }
    }
}
