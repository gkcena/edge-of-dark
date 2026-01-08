using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public PickupInteractor pickupInteractor;

    [Header("Interaction")]
    public float maxDistance = 4f;
    public LayerMask interactLayers = ~0;

    [Header("Debug")]
    public bool debugMode = false;

    private EnemySpawnMachine currentSpawner;

    void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (pickupInteractor == null)
        {
            pickupInteractor = GetComponent<PickupInteractor>();
        }
    }

    void Update()
    {
        if (playerCamera == null) return;

        CheckSpawnerLookAt();
    }

    void CheckSpawnerLookAt()
    {
        Ray ray = (playerCamera != null)
            ? playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f))
            : new Ray(transform.position, transform.forward);

        EnemySpawnMachine spawner = null;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactLayers))
        {
            spawner = hit.collider.GetComponent<EnemySpawnMachine>();
            if (spawner == null)
            {
                spawner = hit.collider.GetComponentInParent<EnemySpawnMachine>();
            }

            if (debugMode && spawner != null)
            {
                Debug.Log($"PlayerInteractor: Looking at spawner '{spawner.gameObject.name}'");
            }
        }

        if (spawner != currentSpawner)
        {
            if (currentSpawner != null)
            {
                currentSpawner.OnPlayerLookAt(false);
            }

            currentSpawner = spawner;

            if (currentSpawner != null)
            {
                currentSpawner.OnPlayerLookAt(true);
            }
        }
    }

    void OnDisable()
    {
        if (currentSpawner != null)
        {
            currentSpawner.OnPlayerLookAt(false);
            currentSpawner = null;
        }
    }
}
