using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public enum CameraMode { ThirdPerson, FirstPerson }

    [Header("Target")]
    [SerializeField] Transform target;   // Player

    [Header("Mode")]
    [SerializeField] CameraMode currentMode = CameraMode.ThirdPerson;

    [Header("Offsets")]
    [SerializeField] Vector3 thirdPersonOffset = new Vector3(0f, 1.6f, -6f);
    [SerializeField] Vector3 firstPersonOffset = new Vector3(0f, 1.6f, 0f);

    [Header("Rotation")]
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float minPitchTPP = -30f;
    [SerializeField] float maxPitchTPP = 60f;
    [SerializeField] float minPitchFPP = -80f;
    [SerializeField] float maxPitchFPP = 80f;

    float yaw;
    float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Mouse input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        if (currentMode == CameraMode.ThirdPerson)
            pitch = Mathf.Clamp(pitch, minPitchTPP, maxPitchTPP);
        else
            pitch = Mathf.Clamp(pitch, minPitchFPP, maxPitchFPP);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 offset = currentMode == CameraMode.ThirdPerson
            ? thirdPersonOffset
            : firstPersonOffset;

        // Kamera Player’a BAĞLI
        transform.position = target.position + rotation * offset;
        transform.rotation = rotation;
    }

    // UI / test
    public void SetThirdPerson() => currentMode = CameraMode.ThirdPerson;
    public void SetFirstPerson() => currentMode = CameraMode.FirstPerson;
}
