using UnityEngine;

// Membuat teks selalu menghadap kamera (tidak kebalik dari sudut manapun)
public class Billboard : MonoBehaviour
{
    Transform kamera;

    void Start()
    {
        if (Camera.main != null) kamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (kamera == null)
        {
            if (Camera.main != null) kamera = Camera.main.transform;
            else return;
        }
        Vector3 arah = transform.position - kamera.position;
        arah.y = 0f;
        if (arah.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(arah);
    }
}
