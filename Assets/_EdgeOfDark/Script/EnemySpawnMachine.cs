using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnMachine : MonoBehaviour
{
    [Header("Enemy Reference")]
    public GameObject enemyObject;

    [Header("Visual Feedback")]
    public GameObject machineVisualChild;
    public Color highlightColor = Color.yellow;

    [Header("Debug")]
    public bool debugMode = false;

    private bool isPlayerLooking = false;
    private Image enemyHealthBar;
    private Renderer[] machineRenderers;
    private Material[] originalMaterials;
    private Color[] originalColors;
    private Color[] originalEmissionColors;

    void Start()
    {
        if (enemyObject != null)
        {
            enemyHealthBar = FindFilledHealthBar(enemyObject.transform);
            enemyObject.SetActive(false);
        }

        if (machineVisualChild != null)
        {
            machineVisualChild.SetActive(false);
        }

        CacheMachineMaterials();
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

        if (looking)
        {
            ShowMachineVisual();
            ApplyHighlight();
        }
        else
        {
            HideMachineVisual();
            RemoveHighlight();
        }

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

    void ShowMachineVisual()
    {
        if (machineVisualChild != null)
        {
            machineVisualChild.SetActive(true);
        }
    }

    void HideMachineVisual()
    {
        if (machineVisualChild != null)
        {
            machineVisualChild.SetActive(false);
        }
    }

    void CacheMachineMaterials()
    {
        machineRenderers = GetComponentsInChildren<Renderer>();

        if (machineRenderers.Length == 0) return;

        originalMaterials = new Material[machineRenderers.Length];
        originalColors = new Color[machineRenderers.Length];
        originalEmissionColors = new Color[machineRenderers.Length];

        for (int i = 0; i < machineRenderers.Length; i++)
        {
            Renderer r = machineRenderers[i];
            if (r == null) continue;

            originalMaterials[i] = r.material;
            originalColors[i] = r.material.HasProperty("_Color") ? r.material.color : Color.white;
            originalEmissionColors[i] = r.material.HasProperty("_EmissionColor") ? r.material.GetColor("_EmissionColor") : Color.black;
        }
    }

    void ApplyHighlight()
    {
        if (machineRenderers == null || machineRenderers.Length == 0) return;

        for (int i = 0; i < machineRenderers.Length; i++)
        {
            Renderer r = machineRenderers[i];
            if (r == null) continue;

            if (r.material.HasProperty("_EmissionColor"))
            {
                r.material.EnableKeyword("_EMISSION");
                r.material.SetColor("_EmissionColor", highlightColor * 0.5f);
            }

            if (r.material.HasProperty("_Color"))
            {
                r.material.color = Color.Lerp(originalColors[i], highlightColor, 0.3f);
            }
        }

        if (debugMode)
        {
            Debug.Log("EnemySpawnMachine: Highlight applied");
        }
    }

    void RemoveHighlight()
    {
        if (machineRenderers == null || machineRenderers.Length == 0) return;

        for (int i = 0; i < machineRenderers.Length; i++)
        {
            Renderer r = machineRenderers[i];
            if (r == null) continue;

            if (r.material.HasProperty("_EmissionColor"))
            {
                r.material.SetColor("_EmissionColor", originalEmissionColors[i]);
                if (originalEmissionColors[i] == Color.black)
                {
                    r.material.DisableKeyword("_EMISSION");
                }
            }

            if (r.material.HasProperty("_Color"))
            {
                r.material.color = originalColors[i];
            }
        }

        if (debugMode)
        {
            Debug.Log("EnemySpawnMachine: Highlight removed");
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

    void OnDisable()
    {
        RemoveHighlight();
        HideMachineVisual();
    }
}
