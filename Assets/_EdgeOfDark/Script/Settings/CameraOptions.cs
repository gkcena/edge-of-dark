using UnityEngine;
using UnityEngine.UI;

public class CameraOptions : MonoBehaviour
{
    public static CameraOptions Instance;
    
    public Sprite openImage;
    public Sprite closeImage;

    public Button fpsButton;
    public Button tpsButton;

    private void Start()
    {
        fpsButton.image.sprite = openImage;
        tpsButton.image.sprite = closeImage;
        GameManager.Instance.currentCameraMode = CameraMode.FirstPerson;

        fpsButton.onClick.AddListener(OnFpsButtonPressed);
        tpsButton.onClick.AddListener(OnTpsButtonPressed);
    }

    public void OnFpsButtonPressed()
    {
        fpsButton.image.sprite = openImage;
        tpsButton.image.sprite = closeImage;

        GameManager.Instance.currentCameraMode = CameraMode.FirstPerson;
        GameManager.Instance.OnCameraModeChanged?.Invoke(CameraMode.FirstPerson);
    }

    public void OnTpsButtonPressed()
    {
        fpsButton.image.sprite = closeImage;
        tpsButton.image.sprite = openImage;

        GameManager.Instance.currentCameraMode = CameraMode.ThirdPerson;
        GameManager.Instance.OnCameraModeChanged?.Invoke(CameraMode.ThirdPerson);
    }
}