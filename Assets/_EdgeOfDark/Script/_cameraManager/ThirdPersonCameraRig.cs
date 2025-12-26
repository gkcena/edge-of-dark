using UnityEngine;

public class ThirdPersonCameraRig : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float minPitch = -30f;
    [SerializeField] float maxPitch = 60f;

    float yaw;
    float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Mouse input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = target.position;
    }

    [SerializeField] Transform mainCamera;

    void Update()
    {
        // FPP TEST
        if (Input.GetKeyDown(KeyCode.V))
        {
            mainCamera.localPosition = Vector3.zero;
        }

        // TPP TEST
        if (Input.GetKeyDown(KeyCode.B))
        {
            mainCamera.localPosition = new Vector3(0f, 0f, -5f);
        }
    }

}
