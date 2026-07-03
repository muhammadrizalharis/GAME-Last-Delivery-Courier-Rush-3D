using UnityEngine;

// Kamera mengikuti player dari belakang-atas
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 6, -9);
    public float kehalusan = 5f;

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
        if (arah.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(arah), kehalusan * Time.deltaTime);
    }
}
