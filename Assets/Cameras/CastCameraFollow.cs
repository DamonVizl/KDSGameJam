using UnityEngine;

public class CastCameraFollow : MonoBehaviour
{
    public Transform target;
    public Transform player;
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public float followSpeed = 10f;
    private Vector3 velocity = Vector3.zero;
    public float rotationSpeed = 5f;
    public float transitionDistance = 20f;
    public float blendFactor = 0.3f;

    void LateUpdate()
    {
        if (target == null || player == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / followSpeed);

        float distance = Vector3.Distance(player.position, target.position);
        float t = Mathf.Clamp01(distance / transitionDistance);

        Vector3 lookDirectionA = Vector3.Lerp((player.position - transform.position).normalized, (target.position - transform.position).normalized, t).normalized;
        var castDirection = target.position - player.position;
        Vector3 lookDirectionB = Vector3.Lerp(castDirection, -castDirection, t).normalized;

        Vector3 blendedLookDirection = Vector3.Lerp(lookDirectionA, lookDirectionB, blendFactor).normalized;

        Quaternion desiredRotation = Quaternion.LookRotation(blendedLookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}