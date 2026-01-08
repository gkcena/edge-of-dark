using UnityEngine;

public class AttackAnimations : MonoBehaviour
{
    [Header("References")]
    public SwordHitbox swordHitbox;
    public PickupInteractor pickupInteractor; 
    public Animator animator; 

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

        // 🎬 Animasyon - YENİ BÖLÜM
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 🔊 Ses
        if (swordSwingClip != null)
        {
            SFXManager.Instance.PlaySFX(swordSwingClip);
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

