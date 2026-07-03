using UnityEngine;
using UnityEngine.InputSystem;

// Player lari maju terus, pindah kiri/kanan pakai A/D atau panah
public class PlayerLari : MonoBehaviour
{
    public float kecepatanMaju = 8f;
    public float kecepatanGeser = 10f;
    public float batasX = 7f;
    public float lompat = 7f;
    Rigidbody rb;
    bool mintaLompat = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame))
            mintaLompat = true;
    }

    void FixedUpdate()
    {
        float geser = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) geser = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) geser = 1f;
        }

        float vy = rb.linearVelocity.y;
        bool diTanah = Mathf.Abs(vy) < 0.2f;
        if (mintaLompat && diTanah) vy = lompat;
        mintaLompat = false;

        rb.linearVelocity = new Vector3(geser * kecepatanGeser, vy, kecepatanMaju);

        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, -batasX, batasX);
        transform.position = p;
    }
}
