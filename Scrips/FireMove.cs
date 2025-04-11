using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class FireMove : NetworkBehaviour
{
    public GameObject BulletPrefab;
    public Transform[] FirePoints;
    public NetworkRunner networkRunner;
    public float FireRate = 0.1f;
    public float BulletSpeed = 20f;
    public float BulletLifetime = 5f;

    // Các trường để gán hiệu ứng từ Editor
    public GameObject[] FireEffects; // Mảng hiệu ứng bắn
   // public GameObject[] AudioEffects; // Mảng GameObject âm thanh

    private float nextFireTime = 0f;
    private bool isFiring = false;
    private void Start()
    {
        StopFiringEffects();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            if (networkRunner is not null && networkRunner.LocalPlayer.IsRealPlayer)
            {
                if (!isFiring)
                {
                    StartFiringEffects();
                }
                isFiring = true;
                FireBullets();
                nextFireTime = Time.time + 1f / FireRate;
            }
        }
        else
        {
            if (isFiring)
            {
                StopFiringEffects();
            }
            isFiring = false;
        }
    }

    private void FireBullets()
    {
        for (int i = 0; i < FirePoints.Length; i++)
        {
            if (FirePoints[i] != null)
            {
                var bullet = networkRunner.Spawn(BulletPrefab, FirePoints[i].position, FirePoints[i].rotation);
                var bulletDirection = FirePoints[i].forward;
                bullet.GetComponent<Rigidbody>().AddForce(bulletDirection * BulletSpeed, ForceMode.Impulse);

                StartCoroutine(DestroyBulletAfterLifetime(bullet));
            }
        }
    }

    private IEnumerator DestroyBulletAfterLifetime(NetworkObject bullet)
    {
        yield return new WaitForSeconds(BulletLifetime);

        if (bullet != null && networkRunner != null)
        {
            networkRunner.Despawn(bullet);
        }
    }

    private void StartFiringEffects()
    {
        for (int i = 0; i < FirePoints.Length; i++)
        {
            if (FirePoints[i] != null)
            {
                // Kích hoạt hiệu ứng bắn nếu có
                if (FireEffects != null && i < FireEffects.Length && FireEffects[i] != null)
                {
                    FireEffects[i].SetActive(true);
                }

                // Phát âm thanh nếu có
                //if (AudioEffects != null && i < AudioEffects.Length && AudioEffects[i] != null)
                //{
                //    AudioSource audioSource = AudioEffects[i].GetComponent<AudioSource>();
                //    if (audioSource != null)
                //    {
                //        audioSource.Play();
                //    }
                //}
            }
        }
    }

    private void StopFiringEffects()
    {
        for (int i = 0; i < FirePoints.Length; i++)
        {
            if (FirePoints[i] != null)
            {
                // Dừng hiệu ứng bắn nếu có
                if (FireEffects != null && i < FireEffects.Length && FireEffects[i] != null)
                {
                    FireEffects[i].SetActive(false);
                }

                // Dừng âm thanh nếu có
                //if (AudioEffects != null && i < AudioEffects.Length && AudioEffects[i] != null)
                //{
                //    AudioSource audioSource = AudioEffects[i].GetComponent<AudioSource>();
                //    if (audioSource != null)
                //    {
                //        audioSource.Stop();
                //    }
                //}
            }
        }
    }
}