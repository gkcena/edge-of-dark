// PickupInteractor.cs
// Handles looking at objects under a parent named "Weapons" (or tagged as "Weapons");
// highlights them while looked at; press E to teleport the object to `holdPoint` (make kinematic);
// press Q to drop the held object in front of the player so it falls with physics.

using UnityEngine;

public class PickupInteractor : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera; // if null, Camera.main will be used
    public Transform holdPoint; // assign: player's hand or camera child

    [Header("Interaction")]
    public float maxDistance = 4f;
    public Color highlightColor = Color.yellow;
    public LayerMask interactLayers = ~0; // which layers to raycast against

    [Header("Debug")]
    public bool debugMode = false; // show ray and log hits when true

    // Colliders on player and held object to avoid physics interference while holding
    Collider[] playerColliders;
    Collider[] heldColliders;
    bool[] heldCollidersPrevTrigger;

    GameObject highlighted;
    Renderer[] highlightedRenderers;
    Material[] originalMaterials;
    Color[] originalColors;

    GameObject heldObject;
    Rigidbody heldRb;

    void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;

        // Cache colliders on the player (including inactive children) to ignore collisions with held objects
        playerColliders = GetComponentsInChildren<Collider>(true);

        if (holdPoint == null && playerCamera != null)
        {
            // default hold point roughly in front of camera
            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(playerCamera.transform, false);
            hp.transform.localPosition = new Vector3(0f, -0.25f, 0.6f);
            hp.transform.localRotation = Quaternion.identity;
            holdPoint = hp.transform;
        }
    }

    void Update()
    {
        if (playerCamera == null) return;

        UpdateHighlight();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropHeldObject();
        }
    }

    void UpdateHighlight()
    {
        // clear previous highlight if any
        if (highlighted != null && highlighted != heldObject)
        {
            RestoreHighlight();
        }

        // Cast ray from the center of the camera's viewport so it aligns with what's visible on screen
        Ray ray = (playerCamera != null)
            ? playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f))
            : new Ray(transform.position, transform.forward);

        if (debugMode)
        {
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.cyan);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactLayers))
        {
            GameObject candidate = hit.collider.gameObject;

            if (debugMode) Debug.Log($"PickupInteractor: Ray hit '{candidate.name}' tag='{candidate.tag}' root='{candidate.transform.root.name}' distance={hit.distance}");

            // If the object is part of a root named "Weapons" or has tag "Weapons", consider it pickable
            bool isWeaponGroup = false;
            Transform root = candidate.transform.root;
            if (root != null && root.name == "Weapons") isWeaponGroup = true;
            // safe tag check (avoids CompareTag exception when tag not defined)
            if (candidate != null && candidate.tag == "Weapons") isWeaponGroup = true;

            if (isWeaponGroup && candidate != heldObject)
            {
                // set highlight
                if (highlighted != candidate)
                {
                    highlighted = candidate;
                    highlightedRenderers = highlighted.GetComponentsInChildren<Renderer>();
                    CacheAndApplyHighlight();
                }
                return;
            }
            else
            {
                if (debugMode) Debug.Log("PickupInteractor: Hit object is not in Weapons group/tag.");
            }
        }
        else
        {
            if (debugMode) Debug.Log("PickupInteractor: Raycast did not hit anything.");
        }

        // nothing valid hit
        if (highlighted != null && highlighted != heldObject)
            RestoreHighlight();

        highlighted = null;
    }

    void CacheAndApplyHighlight()
    {
        if (highlightedRenderers == null || highlightedRenderers.Length == 0)
        {
            if (debugMode) Debug.Log("PickupInteractor: No renderers found to highlight.");
            return;
        }

        originalMaterials = new Material[highlightedRenderers.Length];
        originalColors = new Color[highlightedRenderers.Length];

        if (debugMode) Debug.Log($"PickupInteractor: Applying highlight to {highlightedRenderers.Length} renderers.");

        for (int i = 0; i < highlightedRenderers.Length; i++)
        {
            Renderer r = highlightedRenderers[i];
            if (r == null) continue;

            // store reference to the instantiated material (accessing r.material creates an instance)
            originalMaterials[i] = r.material;
            originalColors[i] = r.material.HasProperty("_Color") ? r.material.color : Color.white;

            // try to enable emission if available
            if (r.material.HasProperty("_EmissionColor"))
            {
                r.material.EnableKeyword("_EMISSION");
                r.material.SetColor("_EmissionColor", highlightColor * 1.2f);
                if (debugMode) Debug.Log($"PickupInteractor: Set _EmissionColor on '{r.gameObject.name}' to {highlightColor * 1.2f}");
            }
            // fallback: tint main color (works for many shaders)
            if (r.material.HasProperty("_Color"))
            {
                r.material.color = Color.Lerp(originalColors[i], highlightColor, 0.9f);
                if (debugMode) Debug.Log($"PickupInteractor: Tinted _Color on '{r.gameObject.name}' to {r.material.color}");
            }
        }
    }

    void RestoreHighlight()
    {
        if (highlightedRenderers == null) return;

        if (debugMode) Debug.Log("PickupInteractor: Restoring highlight materials.");

        for (int i = 0; i < highlightedRenderers.Length; i++)
        {
            Renderer r = highlightedRenderers[i];
            if (r == null) continue;

            if (r.material != null)
            {
                if (r.material.HasProperty("_EmissionColor"))
                {
                    r.material.SetColor("_EmissionColor", Color.black);
                    r.material.DisableKeyword("_EMISSION");
                }
                if (r.material.HasProperty("_Color") && originalColors != null && i < originalColors.Length)
                {
                    r.material.color = originalColors[i];
                }
            }
        }

        highlighted = null;
        highlightedRenderers = null;
        originalMaterials = null;
        originalColors = null;
    }

    void TryPickup()
    {
        if (heldObject != null)
        {
            if (debugMode) Debug.Log("PickupInteractor: Already holding an object, cannot pick up another.");
            return; // already holding
        }

        Ray ray = (playerCamera != null)
            ? playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f))
            : new Ray(transform.position, transform.forward);

        if (debugMode) Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green, 0.5f);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactLayers))
        {
            GameObject candidate = hit.collider.gameObject;
            if (debugMode) Debug.Log($"PickupInteractor: TryPickup hit '{candidate.name}' tag='{candidate.tag}' root='{candidate.transform.root.name}'");

            Transform root = candidate.transform.root;
            bool isWeaponGroup = (root != null && root.name == "Weapons") || (candidate != null && candidate.tag == "Weapons");

            if (!isWeaponGroup)
            {
                if (debugMode) Debug.Log("PickupInteractor: Hit object is not pickable (not in Weapons group/tag).");
                return;
            }

            // find the topmost pickable object (we'll pick the hit object's root under Weapons if any)
            GameObject pick = candidate;

            // get rigidbody
            Rigidbody rb = pick.GetComponent<Rigidbody>();
            if (rb == null)
            {
                // try to find on parent
                rb = pick.GetComponentInParent<Rigidbody>();
                if (rb != null) pick = rb.gameObject;
            }

            if (rb == null)
            {
                Debug.Log("PickupInteractor: No Rigidbody found on candidate. Make sure props have a Rigidbody.");
                if (debugMode) Debug.Log($"PickupInteractor: Candidate '{candidate.name}' has no Rigidbody (checked self and parents). Root: {candidate.transform.root.name}");
                return;
            }

            if (debugMode) Debug.Log($"PickupInteractor: Picking up '{pick.name}' (Rigidbody on '{rb.gameObject.name}').");
            PickupObject(pick, rb);
        }
        else
        {
            if (debugMode) Debug.Log("PickupInteractor: TryPickup raycast missed.");
        }
    }

    void PickupObject(GameObject obj, Rigidbody rb)
    {
        heldObject = obj;
        heldRb = rb;

        // clear highlight (we're holding it now)
        if (highlighted == heldObject) RestoreHighlight();

        // make kinematic so it follows the holdPoint without physics interfering
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;
        heldRb.isKinematic = true;
        heldRb.useGravity = false;

        // parent and snap to holdPoint
        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        if (debugMode) Debug.Log($"PickupInteractor: Held '{heldObject.name}'.");
    }

    void DropHeldObject()
    {
        if (heldObject == null || heldRb == null) return;

        // unparent
        heldObject.transform.SetParent(null);

        // enable physics
        heldRb.isKinematic = false;
        heldRb.useGravity = true;

        // give a small forward impulse so it drops in front of the player
        Vector3 forward = (playerCamera != null) ? playerCamera.transform.forward : transform.forward;
        Vector3 dropPoint = (playerCamera != null ? playerCamera.transform.position : transform.position) + forward * 1f + Vector3.down * 0.2f;
        heldObject.transform.position = dropPoint;

        float throwForce = 2f;
        heldRb.linearVelocity = forward * throwForce + Vector3.up * 0.5f;

        if (debugMode) Debug.Log($"PickupInteractor: Dropped '{heldObject.name}' at {dropPoint}.");

        // clear held references
        heldObject = null;
        heldRb = null;
    }

    void OnDisable()
    {
        RestoreHighlight();
    }
}
