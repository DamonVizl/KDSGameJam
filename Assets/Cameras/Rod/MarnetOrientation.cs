using UnityEngine;

public class MarnetOrientation : MonoBehaviour
{
    private Vector3 _lastPosition;
    [SerializeField] Transform _playerTransform = null;
    [SerializeField] float rotationSpeed = 5f; // Adjust this value to control smoothing speed

    void Update()
    {
        // Calculate the distances from the player for current and last positions
        float currentDistance = Vector3.Distance(transform.position, _playerTransform.position);
        float lastDistance = Vector3.Distance(_lastPosition, _playerTransform.position);

        // Only update orientation if the distance has increased
        if (currentDistance > lastDistance)
        {
            Vector3 currentMoveDirection = transform.position - _lastPosition;
            
            // Ensure there is significant movement before updating rotation
            if (currentMoveDirection.sqrMagnitude > 0.0001f)
            {
                // Calculate the target rotation based on movement direction
                Quaternion targetRotation = Quaternion.LookRotation(currentMoveDirection);
                // Smoothly rotate towards the target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        _lastPosition = transform.position;
    }
}