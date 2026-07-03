using UnityEngine;

// Teks/objek hanya tampil kalau player berada dekat (mis. sudah masuk ruangan)
public class TampilDekat : MonoBehaviour
{
    public float jarakMuncul = 7f;
    Transform player;
    MeshRenderer mr;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;
    }

    void Update()
    {
        if (mr == null || player == null) return;
        float d = Vector3.Distance(transform.position, player.position);
        mr.enabled = d <= jarakMuncul;
    }
}
