using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public ParticleSystem bulletTrail; // Hiệu ứng đuôi đạn
    public GameObject explosionEffect; // Hiệu ứng nổ

    [Networked]
    public Vector3 NetworkPosition { get; set; }

    [Networked]
    public Quaternion NetworkRotation { get; set; }
    private void Start()
    {
        bulletTrail.Play();
        explosionEffect.SetActive(false);
    }
    public override void Spawned()
    {
        // Kiểm tra và chạy hiệu ứng đuôi đạn
        if (bulletTrail != null)
        {
            bulletTrail.Play();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            NetworkPosition = transform.position;
            NetworkRotation = transform.rotation;
        }
        else
        {
            transform.position = NetworkPosition;
            transform.rotation = NetworkRotation;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra tag và kích hoạt hiệu ứng nổ nếu va chạm với Enemy
        if (other.CompareTag("Enemy"))
        {
            // Gọi RPC để kích hoạt hiệu ứng nổ trên tất cả client
            Rpc_Explode();

            // Hủy viên đạn trên server
            if (Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_Explode()
    {
        // Tạo hiệu ứng nổ
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }
    }
}