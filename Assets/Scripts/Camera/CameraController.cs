using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttachCameraToTransform(Transform parent, Vector3 offset)
    {
        this.transform.parent = parent;
        this.transform.position += offset; 
    }
}
