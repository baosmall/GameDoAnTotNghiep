using UnityEngine;
using UnityEngine.AI;
using Fusion;

public class Enemy : NetworkBehaviour
{
    public NavMeshAgent agent;
    public GameObject BulletPrefab;
    public Transform FirePoint;
    public float FireRate = 1f;
    public float AttackRange = 10f;
    public float bulletSpeed = 20f; // Thêm biến tốc độ đạn

    [Networked]
    private TickTimer NextFireTime { get; set; }

    [Networked]
    private NetworkId TargetId { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            GameObject target = FindNearestPlayer();

            if (target != null)
            {
                TargetId = target.GetComponent<NetworkObject>().Id;
                float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);

                if (distanceToPlayer <= AttackRange)
                {
                    agent.isStopped = true;
                    transform.LookAt(target.transform);

                    if (NextFireTime.ExpiredOrNotRunning(Runner))
                    {
                        RPC_Fire(TargetId);
                        NextFireTime = TickTimer.CreateFromSeconds(Runner, 1f / FireRate);
                    }
                }
                else
                {
                    agent.isStopped = false;
                }
            }
        }
    }

    GameObject FindNearestPlayer()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        if (targets.Length == 0) return null;

        GameObject nearestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (var t in targets)
        {
            float distance = Vector3.Distance(t.transform.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = t;
            }
        }
        return nearestTarget;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Fire(NetworkId targetId)
    {
        NetworkObject targetNetworkObject = Runner.FindObject(targetId);
        if (targetNetworkObject == null) return;
        Vector3 targetPosition = targetNetworkObject.transform.position;

        NetworkObject bullet = Runner.Spawn(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            Vector3 direction = (targetPosition - FirePoint.position).normalized;
            bulletRb.AddForce(direction * bulletSpeed, ForceMode.Impulse); // Sử dụng bulletSpeed
        }
    }
}