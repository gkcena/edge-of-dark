using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Staff : MonoBehaviour
{
    [Header("Spawn")]
    public Transform skillSpawnPoint;
    public GameObject skillPrefab;
    public PickupInteractor pickupInteractor;

    [Header("Trajectory")]
    [SerializeField] private float range = 12f;
    [SerializeField] private float travelTime = 0.8f;

    [Header("Damage")]
    [SerializeField] private float damageAmount = 0.2f;
    [SerializeField] private bool destroyOnHit = true;

    [Header("Rate Limit")]
    [SerializeField] private float cooldownSeconds = 0.5f;

    private bool onCooldown;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (pickupInteractor == null || !pickupInteractor.IsHoldingStaff())
                return;

            TryCastSkill();
        }
    }

    private void TryCastSkill()
    {
        if (onCooldown) return;
        if (skillPrefab == null || skillSpawnPoint == null) return;

        onCooldown = true;
        Invoke(nameof(ResetCooldown), cooldownSeconds);

        CastSkill();
    }

    private void ResetCooldown() => onCooldown = false;

    public void CastSkill()
    {
        Camera cam = null;

        if (pickupInteractor != null && pickupInteractor.playerCamera != null) cam = pickupInteractor.playerCamera;
        if (cam == null) cam = Camera.main;

        if (cam == null)
        {
            Debug.LogWarning("Staff: Camera bulunamadı (PickupInteractor.playerCamera / Camera.main).");
            return;
        }

        Vector3 spawnPos = skillSpawnPoint.position;
        Quaternion spawnRot = Quaternion.LookRotation(cam.transform.forward, Vector3.up);

        GameObject skill = Instantiate(skillPrefab, spawnPos, spawnRot);

        var proj = skill.GetComponent<StaffProjectile>();
        if (proj == null) proj = skill.AddComponent<StaffProjectile>();

        proj.Init(damageAmount, destroyOnHit);

        EnsurePhysics(skill);

        Vector3 dir = cam.transform.forward.normalized;
        Vector3 targetPos = spawnPos + dir * range;

        skill.transform.DOKill();

        skill.transform.DOMove(targetPos, travelTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (skill != null) Destroy(skill);
            });
    }

    private void EnsurePhysics(GameObject skill)
    {
        Collider col = skill.GetComponent<Collider>();
        if (col == null)
        {
            var sc = skill.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 0.25f;
        }
        else
        {
            col.isTrigger = true;
        }

        Rigidbody rb = skill.GetComponent<Rigidbody>();
        if (rb == null) rb = skill.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    private class StaffProjectile : MonoBehaviour
    {
        private float damageAmount;
        private bool destroyOnHit;

        private bool hasHit;

        public void Init(float damage, bool destroyOnHit)
        {
            this.damageAmount = damage;
            this.destroyOnHit = destroyOnHit;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;
            if (!other.CompareTag("Enemy")) return;

            Image filledBar = FindFilledHealthBar(other.transform);
            if (filledBar == null)
            {
                Debug.LogWarning("StaffProjectile: Enemy üzerinde Filled Image (health bar) bulunamadı.");
                return;
            }

            filledBar.fillAmount = Mathf.Clamp01(filledBar.fillAmount - damageAmount);
            hasHit = true;

            if (filledBar.fillAmount <= 0f)
            {
                other.gameObject.SetActive(false);
            }

            if (destroyOnHit)
            {
                transform.DOKill();
                Destroy(gameObject);
            }
        }

        private Image FindFilledHealthBar(Transform enemy)
        {
            Image[] images = enemy.GetComponentsInChildren<Image>(true);
            foreach (Image img in images)
            {
                if (img.type == Image.Type.Filled)
                    return img;
            }
            return null;
        }
    }
}