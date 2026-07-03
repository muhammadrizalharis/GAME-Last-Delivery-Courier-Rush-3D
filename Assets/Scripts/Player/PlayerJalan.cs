using UnityEngine;
using UnityEngine.InputSystem;

// Kurir jalan WASD mengikuti arah hadap kamera (bisa belok)
public class PlayerJalan : MonoBehaviour
{
    public float kecepatan = 7f;
    public float putaran = 12f;
    Rigidbody rb;
    Transform kamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (Camera.main != null) kamera = Camera.main.transform;
    }

    void FixedUpdate()
    {
        float x = 0f, z = 0f;
        var k = Keyboard.current;
        if (k != null)
        {
            if (k.aKey.isPressed || k.leftArrowKey.isPressed) x = -1f;
            if (k.dKey.isPressed || k.rightArrowKey.isPressed) x = 1f;
            if (k.wKey.isPressed || k.upArrowKey.isPressed) z = 1f;
            if (k.sKey.isPressed || k.downArrowKey.isPressed) z = -1f;
        }

        // arah maju/kanan berdasarkan hadap kamera (biar bisa belok)
        Vector3 maju = Vector3.forward, kanan = Vector3.right;
        if (kamera != null)
        {
            maju = kamera.forward; maju.y = 0f; maju.Normalize();
            kanan = kamera.right; kanan.y = 0f; kanan.Normalize();
        }
        Vector3 arah = maju * z + kanan * x;
        if (arah.sqrMagnitude > 1f) arah.Normalize();

        rb.linearVelocity = new Vector3(arah.x * kecepatan, rb.linearVelocity.y, arah.z * kecepatan);

        if (arah.sqrMagnitude > 0.01f)
        {
            Quaternion tujuan = Quaternion.LookRotation(new Vector3(arah.x, 0f, arah.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, tujuan, putaran * Time.fixedDeltaTime);
        }
    }
}
