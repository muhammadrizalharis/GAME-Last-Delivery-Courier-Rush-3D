using UnityEngine;

// BOS akhir Scene 2: Truk Besar bergeser kiri-kanan menghalangi jalan. Kena -> HP berkurang.
public class BossTruk : MonoBehaviour
{
    public float ayun = 3.3f;      // jangkauan geser kiri-kanan
    public float kecepatan = 2f;
    float x0;
    bool cooldown = false;

    void Start()
    {
        x0 = transform.position.x;
    }

    void Update()
    {
        Vector3 p = transform.position;
        p.x = x0 + Mathf.Sin(Time.time * kecepatan) * ayun;
        transform.position = p;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cooldown)
        {
            cooldown = true;
            if (RunGame.instance != null) RunGame.instance.Kena();
            Invoke(nameof(ResetCd), 1f);
        }
    }

    void ResetCd() { cooldown = false; }
}
