using UnityEngine;
using UnityEngine.InputSystem;

// Kamera third-person: ikut player + putar pakai mouse + zoom scroll
public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float sensitivitas = 0.15f;
    public float jarak = 7f;
    public float jarakMin = 2.5f;
    public float jarakMax = 16f;
    public float kecepatanZoom = 1.5f;
    public float tinggi = 2f;
    public float pitchMin = -15f;
    public float pitchMax = 60f;

    float yaw = 0f;
    float pitch = 15f;

    void Start()
    {
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw += delta.x * sensitivitas;
            pitch -= delta.y * sensitivitas;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
                jarak -= Mathf.Sign(scroll) * kecepatanZoom;
            jarak = Mathf.Clamp(jarak, jarakMin, jarakMax);
        }

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 fokus = target.position + Vector3.up * tinggi;
        transform.position = fokus - (rot * Vector3.forward) * jarak;
        transform.rotation = rot;
    }

    public float Yaw { get { return yaw; } }
}
