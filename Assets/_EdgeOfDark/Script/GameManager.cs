using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Camera Mode")]
    public CameraMode currentCameraMode;

    public Action<CameraMode> OnCameraModeChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {

    }

}
public enum CameraMode
{
    FirstPerson,
    ThirdPerson
}