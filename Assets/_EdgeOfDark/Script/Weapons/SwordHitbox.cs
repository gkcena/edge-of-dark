using UnityEngine;
using UnityEngine.UI;

public class SwordHitbox : MonoBehaviour
{
    [Header("Damage")]
    public float damageAmount = 0.2f;

    [Header("Debug")]
    public bool logHits = false;

    private bool canDealDamage = false;
    private bool hasHitThisSwing = false;

    private void OnTriggerEnter(Collider other)
    {
        // Pickup sistemi için collider hep açık; bu yüzden hasarı state ile kontrol ediyoruz
        if (!canDealDamage) return;
        if (hasHitThisSwing) return;

        if (!other.CompareTag("Enemy")) return;

        // Enemy child'larındaki Filled Image'ı bul (can bar)
        Image filledBar = FindFirstFilledImage(other.gameObject);

        if (filledBar == null)
        {
            if (logHits) Debug.LogWarning("Enemy üzerinde Filled Image bulunamadı.");
            return;
        }

        filledBar.fillAmount = Mathf.Clamp01(filledBar.fillAmount - damageAmount);
        hasHitThisSwing = true;

        if (logHits) Debug.Log($"Hit! New fillAmount = {filledBar.fillAmount}");

        if (filledBar.fillAmount <= 0f)
        {
            Destroy(other.gameObject);
        }
    }

    // AttackAnimations bunu çağıracak
    public void StartDamageWindow()
    {
        canDealDamage = true;
        hasHitThisSwing = false;
    }

    // AttackAnimations bunu çağıracak
    public void StopDamageWindow()
    {
        canDealDamage = false;
    }

    private Image FindFirstFilledImage(GameObject enemyObj)
    {
        // Enemy root + child'larındaki tüm Image'ları tarar
        Image[] images = enemyObj.GetComponentsInChildren<Image>(true);

        foreach (var img in images)
        {
            if (img != null && img.type == Image.Type.Filled)
                return img;
        }

        return null;
    }
}
