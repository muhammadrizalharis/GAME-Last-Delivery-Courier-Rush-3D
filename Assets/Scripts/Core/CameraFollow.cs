using UnityEngine;

// Kamera mengikuti player dari belakang-atas
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 6, -9);
    public float kehalusan = 5f;
    float getar = 0f;

    void Start()
    {
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
        if (target != null)
            transform.position = target.position + offset;
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 tujuan = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, tujuan, kehalusan * Time.deltaTime);

        Vector3 arah = (target.position + Vector3.up * 1.5f) - transform.position;
        float mag = arah.magnitude;
        if (mag > 0.001f && !float.IsNaN(mag) && !float.IsInfinity(mag))
        {
            Quaternion tujuanRot = Quaternion.LookRotation(arah / mag);
            transform.rotation = Quaternion.Slerp(transform.rotation, tujuanRot, kehalusan * Time.deltaTime);
        }

        if (getar > 0f)
        {
            getar -= Time.deltaTime * 2.5f;
            if (getar < 0f) getar = 0f;
            transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * getar;
        }
    }

    public void Getar(float kekuatan)
    {
        if (kekuatan > getar) getar = kekuatan;
    }
}
