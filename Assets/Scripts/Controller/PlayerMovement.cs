using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [ SerializeField] CameraController _camera;
    [SerializeField] Vector3 _cameraOffset; 

    [SerializeField] float _moveSpeed = 5f;

    [SerializeField] private Vector2 _moveVector;

    void Start()
    {
        AttachCameraToPlayer(_camera); 
    }
    void OnEnable()
    {
        InputManager.OnMove += SetMoveVector; 

    }
    void OnDisable()
    {
        InputManager.OnMove -= SetMoveVector;
    }

    /// <summary>
    /// Attach the camera to the players transform plus an offset. 
    /// </summary>
    /// <param name="cam"></param>
    void AttachCameraToPlayer(CameraController cam)
    {
        cam.AttachCameraToTransform(this.transform, _cameraOffset); 

    }

    void SetMoveVector(Vector2 moveVector)
    {
        _moveVector = moveVector; 
    }
    void Update()
    {
        HandleMove(); 
    }

    void HandleMove()
    {
        this.transform.position += new Vector3(_moveVector.x* _moveSpeed * Time.deltaTime, 0 , _moveVector.y* _moveSpeed * Time.deltaTime); 
    }


}
