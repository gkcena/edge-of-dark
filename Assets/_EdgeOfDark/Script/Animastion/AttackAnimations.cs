using UnityEngine;

public class AttackAnimations : MonoBehaviour
{
    [Header("References")]
    public SwordHitbox swordHitbox;

    [Header("Fake Attack Window (no animations yet)")]
    public float damageWindowSeconds = 0.25f;

    private bool isAttacking = false;

    void Update()
    {
        // Sol tık
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartAttackFake();
        }
    }

    private void StartAttackFake()
    {
        if (swordHitbox == null)
        {
            Debug.LogError("AttackAnimations: SwordHitbox referansı boş! Inspector'dan kılıcı sürükle.");
            return;
        }

        isAttacking = true;

        // Hasar penceresini aç
        swordHitbox.StartDamageWindow();

        // Belirli süre sonra kapat
        Invoke(nameof(EndAttackFake), damageWindowSeconds);
    }

    private void EndAttackFake()
    {
        swordHitbox.StopDamageWindow();
        isAttacking = false;
    }
}
