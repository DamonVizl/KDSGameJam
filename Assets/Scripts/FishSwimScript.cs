using UnityEngine;

public class FishSwimScript : MonoBehaviour
{
    [SerializeField] float _swimSpeed = 300.0f;
    [SerializeField] float _rotationSpeed = 20.0f;
    [SerializeField] float _noiseSpeed = 1f;

    [SerializeField] float _linearDragCoefficient = 10.0f;
    [SerializeField] float _angularDragCoefficient = 2.0f;
    [SerializeField] float _lureDetectionRadius = 5.0f;
    
    private Vector3 _forwardDirection;

    private Vector3 attractionPoint = new Vector3(50.0f, -2.0f, -60.0f);
    private float attractionStrength = 400.0f;

    Transform _tf;
    Rigidbody _rb;   

    private float _noiseOffsetYaw;
    private float _noiseOffsetPitch;
    private float _noiseOffsetThrust;

    const float WATER_LEVEL = 0f;
    const float TERRAIN_CHECK_DISTANCE = 100f;
    const float MIN_DISTANCE_ABOVE_TERRAIN = 1.5f;
    const float VERTICAL_ADJUST_SPEED = 2f;
    const float FALL_SPEED = 9.8f;

    void Start()
    {
        _tf = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody>();

        _noiseOffsetYaw = Random.Range(0f, 1000f);
        _noiseOffsetPitch = Random.Range(0f, 1000f);
        _noiseOffsetThrust = Random.Range(0f, 1000f);
    }

    void FixedUpdate()
    {
        MoveFish();
        StayAboveGround();
        StayUnderWater();
    }

    private void MoveFish()
    {
        float _deltaTime = Time.fixedDeltaTime;

        // fish propels itself in model forward direction only
        _forwardDirection = _tf.forward;

        float time = Time.time * _noiseSpeed;
        float noiseYaw = Mathf.PerlinNoise(time + _noiseOffsetYaw, _noiseOffsetYaw) - 0.5f;
        float noisePitch = Mathf.PerlinNoise(time + _noiseOffsetPitch, _noiseOffsetPitch) - 0.5f;
        float noiseThrust = Mathf.PerlinNoise(time + _noiseOffsetThrust, _noiseOffsetThrust);

        // apply attraction force from lure
        Vector3 attractionVector = attractionPoint - _tf.position;
        _rb.AddForce(attractionVector.normalized * attractionStrength / attractionVector.sqrMagnitude);

        float urgencyFactor = attractionVector.magnitude > _lureDetectionRadius ? 0.0f : 1.0f - (attractionVector.magnitude / _lureDetectionRadius);

        // rotate
        if (attractionVector.magnitude > _lureDetectionRadius) {
            // perlin wandering
            _rb.AddTorque(new Vector3(0.0f, _rotationSpeed * noiseYaw, 0.0f));
        } else {
            if (Random.Range(0, _lureDetectionRadius) > attractionVector.magnitude) {
                // TODO: turn away from lure
                _rb.AddTorque(new Vector3(0.0f, 3 * _rotationSpeed * 1.0f, 0.0f));
            } else {
                // perlin wandering
                _rb.AddTorque(new Vector3(0.0f, _rotationSpeed * noiseYaw, 0.0f));
            }
        }

        // apply thrust in forward direction
        _rb.AddForce(_forwardDirection * _swimSpeed * _deltaTime * (1 + urgencyFactor + noiseThrust));

        // apply linear drag
        _rb.AddForce(-1.0f * _linearDragCoefficient * _rb.linearVelocity.sqrMagnitude * _deltaTime * _rb.linearVelocity);

        // apply angular drag
        _rb.AddTorque(-1.0f * _angularDragCoefficient * _rb.angularVelocity);
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
        if (transform.position.y > WATER_LEVEL) 
            transform.position = Vector3.up * -FALL_SPEED;
    }
}
