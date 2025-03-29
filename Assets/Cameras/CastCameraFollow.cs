using UnityEngine;
using UnityEngine.Serialization;

public class CastCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 5f, -10f);
    [SerializeField] private float _followSpeed = 10f;
    [SerializeField] private Vector3 _velocity = Vector3.zero;
    [SerializeField] private float _rotationSpeed = 5f;

    void LateUpdate()
    {
        if (_target == null)
            return;

        Vector3 desiredPosition = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, 1f / _followSpeed);

        Quaternion desiredRotation = Quaternion.LookRotation(_target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _rotationSpeed * Time.deltaTime);
    }
}