using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCameraManager : MonoBehaviour
{
    Camera camera;

    void Awake()
    {
        camera = GetComponent<Camera>();
        if (Camera.main != camera)
        {
            Destroy(gameObject);
        }
    }
}
