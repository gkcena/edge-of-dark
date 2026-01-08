using UnityEngine;

public class AttackAnimations : MonoBehaviour
{
    [Header("References")]
    public SwordHitbox swordHitbox;
    public PickupInteractor pickupInteractor;

    [Header("Fake Attack Window")]
    public float damageWindowSeconds = 0.25f;

    [Header("Audio")]
    public AudioClip swordSwingClip;
    [Range(0f, 1f)]
    public float volume = 0.8f;

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            // 🔴 KILIÇ ELDE DEĞİLSE → HİÇBİR ŞEY YAPMA
            if (pickupInteractor == null || !pickupInteractor.IsHoldingSword())
                return;

            StartAttackFake();
        }
    }

    private void StartAttackFake()
    {
        if (swordHitbox == null) return;

        isAttacking = true;

        // 🔊 Ses
        if (swordSwingClip != null)
        {
            AudioSource.PlayClipAtPoint(
                swordSwingClip,
                transform.position,
                volume
            );
        }

        swordHitbox.StartDamageWindow();
        Invoke(nameof(EndAttackFake), damageWindowSeconds);
    }

    private void EndAttackFake()
    {
        swordHitbox.StopDamageWindow();
        isAttacking = false;
    }
}
