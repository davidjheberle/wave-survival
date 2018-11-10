using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCameraManager : MonoBehaviour
{
    Camera attachedCamera;

    void Awake()
    {
        attachedCamera = GetComponent<Camera>();
        if (Camera.main != attachedCamera)
        {
            Destroy(gameObject);
        }
    }
}
