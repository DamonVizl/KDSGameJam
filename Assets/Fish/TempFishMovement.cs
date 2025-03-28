using UnityEngine;

public class TempFishMovement : MonoBehaviour
{
    [SerializeField] public float speed = 2f;
    [SerializeField] public float noiseScale = 1f;
    [SerializeField] public float noiseSpeed = 1f;
    [SerializeField] public float rotationSpeed = 5f;
    
    private float _noiseOffsetX;
    private float _noiseOffsetY;
    private float _noiseOffsetZ;

    const float TERRAIN_CHECK_DISTANCE = 100f;
    const float MIN_DISTANCE_ABOVE_TERRAIN = .5f;
    const float VERTICAL_ADJUST_SPEED = 2f;
    const float FALL_SPEED = 9.8f;

    void Start()
    {
        _noiseOffsetX = Random.Range(0f, 1000f);
        _noiseOffsetY = Random.Range(0f, 1000f);
        _noiseOffsetZ = Random.Range(0f, 1000f);
    }

    void Update()
    {
        MoveFish();
        StayAboveGround();
        StayUnderWater();
    }

    private void MoveFish()
    {
        float time = Time.time * noiseSpeed;
        float noiseX = Mathf.PerlinNoise(time + _noiseOffsetX, _noiseOffsetX) - 0.5f;
        float noiseY = Mathf.PerlinNoise(time + _noiseOffsetY, _noiseOffsetY) - 0.5f;
        float noiseZ = Mathf.PerlinNoise(time + _noiseOffsetZ, _noiseOffsetZ) - 0.5f;

        Vector3 direction = new Vector3(noiseX, noiseY, noiseZ).normalized;
        Vector3 movement = direction * speed * noiseScale * Time.deltaTime;

        transform.position += movement;
        FaceForward(movement);
    }

    private void FaceForward(Vector3 movement)
    {
        Quaternion targetRotation = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void StayAboveGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hit, TERRAIN_CHECK_DISTANCE))
            if (hit.collider.CompareTag("Terrain"))
            {
                float targetY = hit.point.y + MIN_DISTANCE_ABOVE_TERRAIN;
                if (transform.position.y < targetY)
                {
                    float newY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * VERTICAL_ADJUST_SPEED);
                    transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                }
            }
    }

    private void StayUnderWater()
    {
        const float WATER_LEVEL = 0f;
        if (transform.position.y > WATER_LEVEL) 
            transform.position = Vector3.up * -FALL_SPEED;
    }
}