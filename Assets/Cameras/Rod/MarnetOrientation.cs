using UnityEngine;

public class MarnetOrientation : MonoBehaviour
{
    private Vector3 _lastPosition;
    [SerializeField] Transform _playerTransform = null;

    void FixedUpdate()
    {
        // Calculate the distances from the player for current and last positions
        float currentDistance = Vector3.Distance(transform.position, _playerTransform.position);
        float lastDistance = Vector3.Distance(_lastPosition, _playerTransform.position);

        // Only update orientation if the distance has increased
        if (currentDistance > lastDistance)
        {
            var currentMoveDirection = transform.position - _lastPosition;
            transform.LookAt(transform.position + currentMoveDirection);
        }

        _lastPosition = transform.position;
    }
}