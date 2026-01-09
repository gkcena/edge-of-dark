using UnityEngine;
using UnityEngine.UI;

public class SwordHitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 0.2f;

    [Header("Sound Effects")]
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound;

    private bool canDealDamage = false;
    private bool hasHitThisSwing = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("SwordHitbox trigger touched: " + other.name);

        if (!canDealDamage) return;
        if (hasHitThisSwing) return;
        if (!other.CompareTag("Enemy")) return;

        Image filledBar = FindFilledHealthBar(other.transform);

        if (filledBar == null)
        {
            Debug.LogWarning("No Filled Image found on enemy!");
            return;
        }

        filledBar.fillAmount = Mathf.Clamp01(filledBar.fillAmount - damageAmount);
        hasHitThisSwing = true;

        if (filledBar.fillAmount <= 0f)
        {
            TriggerEnemyDeathAnimation(other.gameObject);
            PlayEnemyDeathSound();
            other.gameObject.SetActive(false);
        }
        else
        {
            TriggerEnemyHitAnimation(other.gameObject);
            PlayEnemyHitSound();
        }
    }

    private void TriggerEnemyHitAnimation(GameObject enemy)
    {
        Animator enemyAnimator = enemy.GetComponent<Animator>();
        if (enemyAnimator == null)
        {
            enemyAnimator = enemy.GetComponentInChildren<Animator>();
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("GetHit");
            Debug.Log($"Enemy {enemy.name} hit animation triggered!");
        }
        else
        {
            Debug.LogWarning($"No Animator found on enemy {enemy.name}");
        }
    }

    private void TriggerEnemyDeathAnimation(GameObject enemy)
    {
        Animator enemyAnimator = enemy.GetComponent<Animator>();
        if (enemyAnimator == null)
        {
            enemyAnimator = enemy.GetComponentInChildren<Animator>();
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Death");
            Debug.Log($"Enemy {enemy.name} death animation triggered!");
        }
    }

    private void PlayEnemyHitSound()
    {
        if (enemyHitSound != null && SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(enemyHitSound);
            Debug.Log("Enemy hit sound played!");
        }
    }

    private void PlayEnemyDeathSound()
    {
        if (enemyDeathSound != null && SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(enemyDeathSound);
            Debug.Log("Enemy death sound played!");
        }
    }

    public void StartDamageWindow()
    {
        canDealDamage = true;
        hasHitThisSwing = false;
        Debug.Log("Damage window OPEN");
    }

    public void StopDamageWindow()
    {
        canDealDamage = false;
        Debug.Log("Damage window CLOSED");
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
