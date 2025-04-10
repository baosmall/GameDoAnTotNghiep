using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class HPMP_ENEMY : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnhealthChanged))]
    public float HP { get; set; }
    public float maxHP { get; set; }

    public Slider HpSlider;

    public NetworkObject networkObject;
    public NetworkRunner networkRunner;

    public GameObject dieEffect; // Hiệu ứng die

    private void Start()
    {
        dieEffect.SetActive(false);
    }
    public override void Spawned()
    {
        maxHP = 100;
        if (Object.HasStateAuthority) HP = maxHP;
        HpSlider.maxValue = maxHP;
        HpSlider.value = HP;
    }

    public void OnhealthChanged()
    {
        HpSlider.value = HP;
    }

    [Networked, OnChangedRender(nameof(OnSpeedChange))]
    public float Speed { get; set; }
    public Animator animator;
    public int speedHash = Animator.StringToHash("Speed");

    public void OnSpeedChange()
    {
        animator.SetFloat(speedHash, Speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            if (Object.HasStateAuthority) HP -= 10;
        }
        if (HP <= 0)
        {
            Destroy(gameObject);
            dieEffect.SetActive(true);
        }
        if (HP <= 0 && Object.HasStateAuthority)
        {
            // Kích hoạt hiệu ứng die
            if (dieEffect != null)
            {
                Instantiate(dieEffect, transform.position, transform.rotation);
            }

            // Phá hủy đối tượng
            networkRunner.Despawn(networkObject);
        }
    }
}