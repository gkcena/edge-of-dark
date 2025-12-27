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
        SetCameraMode(GameManager.Instance.currentCameraMode);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnCameraModeChanged += SetCameraMode;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnCameraModeChanged -= SetCameraMode;
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

    public void SetCameraMode(CameraMode mode)
    {
        switch (mode)
        {
            case CameraMode.FirstPerson:
                FpsMode();
                break;
            case CameraMode.ThirdPerson:
                TpsMode();
                break;
        }
    }

    public void FpsMode()
    {
        mainCamera.localPosition = Vector3.zero;
    }

    public void TpsMode()
    {
        mainCamera.localPosition = new Vector3(0f, 0f, -5f);
    }

    void Update()
    {
        // FPP TEST
        if (Input.GetKeyDown(KeyCode.V))
        {
            FpsMode();
        }

        // TPP TEST
        if (Input.GetKeyDown(KeyCode.B))
        {
            TpsMode();
        }
    }
}