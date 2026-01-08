using UnityEngine;
using UnityEngine.UI;

public class SwordHitbox : MonoBehaviour
{
    public float damageAmount = 0.2f;

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
            Destroy(other.gameObject);
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
