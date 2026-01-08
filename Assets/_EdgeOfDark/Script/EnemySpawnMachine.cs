using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnMachine : MonoBehaviour
{
    [Header("Enemy Reference")]
    public GameObject enemyObject;

    [Header("Debug")]
    public bool debugMode = false;

    private bool isPlayerLooking = false;
    private Image enemyHealthBar;

    void Start()
    {
        if (enemyObject != null)
        {
            enemyHealthBar = FindFilledHealthBar(enemyObject.transform);
            enemyObject.SetActive(false);
        }
    }

    void Update()
    {
        bool canSpawn = enemyObject != null && !enemyObject.activeInHierarchy;

        if (Input.GetKeyDown(KeyCode.E) && isPlayerLooking && canSpawn)
        {
            ActivateEnemy();
        }
    }

    public void OnPlayerLookAt(bool looking)
    {
        isPlayerLooking = looking;
        if (debugMode)
        {
            Debug.Log($"EnemySpawnMachine: Player looking = {looking}");
        }
    }

    void ActivateEnemy()
    {
        if (enemyObject == null)
        {
            Debug.LogError("EnemySpawnMachine: Enemy object is not assigned!");
            return;
        }

        if (enemyHealthBar != null)
        {
            enemyHealthBar.fillAmount = 1f;
        }

        enemyObject.SetActive(true);

        if (debugMode)
        {
            Debug.Log("EnemySpawnMachine: Enemy activated with full health!");
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
