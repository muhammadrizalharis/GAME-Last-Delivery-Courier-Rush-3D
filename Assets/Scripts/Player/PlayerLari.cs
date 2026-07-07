using UnityEngine;
using UnityEngine.InputSystem;

// Player lari maju terus, pindah kiri/kanan pakai A/D atau panah
public class PlayerLari : MonoBehaviour
{
    public float kecepatanMaju = 8f;
    public float kecepatanGeser = 10f;
    public float batasX = 7f;
    public float lompat = 7f;
    public float tinggiTanah = 1.15f; // tinggi pemain saat berdiri di jalan (biar tak jatuh menembus)
    public float percepatan = 0f;     // >0 = makin cepat seiring waktu (dipakai Level 2)
    public float kecepatanMaks = 18f;
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
        if (percepatan > 0f)
            kecepatanMaju = Mathf.Min(kecepatanMaks, kecepatanMaju + percepatan * Time.fixedDeltaTime);

        float geser = 0f;
        bool menunduk = false;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) geser = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) geser = 1f;
            menunduk = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed || Keyboard.current.leftCtrlKey.isPressed;
        }

        // MENUNDUK: badan memendek + turun, biar bisa lewat di bawah palang tinggi
        transform.localScale = new Vector3(1f, menunduk ? 0.5f : 1f, 1f);
        float tanah = menunduk ? tinggiTanah - 0.5f : tinggiTanah;

        Vector3 p = transform.position;
        // kalau posisi rusak (NaN) akibat tabrakan aneh -> kembalikan ke jalan
        if (float.IsNaN(p.x) || float.IsNaN(p.y) || float.IsNaN(p.z))
            p = new Vector3(0f, tanah, p.z);

        // di tanah dicek dari posisi (lebih aman daripada dari kecepatan)
        bool diTanah = p.y <= tanah + 0.05f;
        float vy = rb.linearVelocity.y;
        if (float.IsNaN(vy) || float.IsInfinity(vy)) vy = 0f;
        if (mintaLompat && diTanah && !menunduk) vy = lompat; // tak bisa lompat sambil menunduk
        mintaLompat = false;

        rb.linearVelocity = new Vector3(geser * kecepatanGeser, vy, kecepatanMaju);

        // jaga pemain tetap di atas jalan: tak boleh jatuh/menembus, dan tak keluar samping
        p.x = Mathf.Clamp(p.x, -batasX, batasX);
        if (p.y < tanah)
        {
            p.y = tanah;
            Vector3 v = rb.linearVelocity;
            if (v.y < 0f) v.y = 0f;
            rb.linearVelocity = v;
        }
        transform.position = p;
    }
}
