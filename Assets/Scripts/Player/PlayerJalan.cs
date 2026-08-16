using UnityEngine;
using UnityEngine.InputSystem;

// Kurir jalan WASD mengikuti arah hadap kamera (bisa belok)
public class PlayerJalan : MonoBehaviour
{
    public float kecepatan = 7f;
    public float putaran = 12f;
    public float lompat = 6f;
    Rigidbody rb;
    Transform kamera;
    bool mintaLompat;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (Camera.main != null) kamera = Camera.main.transform;
    }

    void Update()
    {
        var k = Keyboard.current;
        if (k != null && k.spaceKey.wasPressedThisFrame) mintaLompat = true;
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

        float vy = rb.linearVelocity.y;
        bool diTanah = Physics.Raycast(transform.position, Vector3.down, 1.2f, ~0, QueryTriggerInteraction.Ignore);
        if (mintaLompat && diTanah) vy = lompat;   // Space -> lompat bila menginjak tanah/tangga
        mintaLompat = false;
        rb.linearVelocity = new Vector3(arah.x * kecepatan, vy, arah.z * kecepatan);

        if (arah.sqrMagnitude > 0.01f)
        {
            Vector3 hadap = new Vector3(arah.x, 0f, arah.z);
            if (hadap.sqrMagnitude > 0.0001f)
            {
                Quaternion tujuan = Quaternion.LookRotation(hadap.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, tujuan, putaran * Time.fixedDeltaTime);
            }
        }
    }
}
