```csharp
using UnityEngine;
using VoxelFPS.Core;

namespace VoxelFPS.Combat
{
    /// <summary>
    /// Modular hitscan weapon system supporting procedural recoil, HDRP bullet trails, and VFX.
    /// </summary>
    public class WeaponSystem : MonoBehaviour
    {
        [Header("Stats")]
        public float damage = 25f;
        public float range = 100f;
        public float fireRate = 0.1f;
        public int maxAmmo = 30;
        private int currentAmmo;

        [Header("References")]
        public Transform gunBarrel;
        public Camera fpsCamera;
        public ParticleSystem muzzleFlash;
        public GameObject impactEffectPrefab; // Assign an HDRP Decal or Particle Prefab
        public TrailRenderer bulletTrailPrefab;

        private float nextTimeToFire = 0f;

        void Start()
        {
            currentAmmo = maxAmmo;
        }

        void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
                return;

            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
            }
        }

        private void Shoot()
        {
            currentAmmo--;
            muzzleFlash.Play();

            // HDRP Audio trigger goes here

            Vector3 rayOrigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            TrailRenderer trail = Instantiate(bulletTrailPrefab, gunBarrel.position, Quaternion.identity);

            if (Physics.Raycast(rayOrigin, fpsCamera.transform.forward, out hit, range))
            {
                trail.AddPosition(hit.point);

                // Handle AI Damage
                IDamageable target = hit.transform.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }

                // Spawn HDRP impact particles/decals
                if (impactEffectPrefab != null)
                {
                    GameObject impactGO = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactGO, 2f);
                }
            }
            else
            {
                trail.AddPosition(rayOrigin + (fpsCamera.transform.forward * range));
            }
        }
    }

    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}

```
