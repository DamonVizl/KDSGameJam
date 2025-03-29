using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
public class FishSwimScript : MonoBehaviour
{
    [SerializeField] float _swimSpeed = 300.0f;
    [SerializeField] float _rotationSpeed = 1000.0f;
    [SerializeField] float _noiseSpeed = 1f;

    [SerializeField] float _linearDragCoefficient = 10.0f;
    [SerializeField] float _angularDragCoefficient = 100.0f;
    [SerializeField] float _lureDetectionRadius = 5.0f;

    [SerializeField] float _minDistanceAboveTerrain = 1.0f;
    [SerializeField] float _minDistanceBelowWater = 1.0f;
    
    private Vector3 _forwardDirection;

    private Vector3 lurePoint = new Vector3(50.0f, -2.0f, -60.0f);
    private float lureMass = 400.0f;
    private float lureCollisionDistance = 0.2f;

    private bool isConnected = false;

    Transform _tf;
    Rigidbody _rb;   

    private float _noiseOffsetYaw;
    private float _noiseOffsetPitch;
    private float _noiseOffsetThrust;

    const float WATER_LEVEL = 0f;
    const float TERRAIN_CHECK_DISTANCE = 100f;
    const float VERTICAL_ADJUST_FORCE = 10.0f;
    const float SHORE_PUSH_FORCE = 500.0f;

    void Start()
    {
        _tf = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody>();

        _noiseOffsetYaw = Random.Range(0f, 1000f);
        _noiseOffsetPitch = Random.Range(0f, 1000f);
        _noiseOffsetThrust = Random.Range(0f, 1000f);

        _rb.maxLinearVelocity = 20.0f;
        _rb.maxAngularVelocity = 2.0f;
    }

    void FixedUpdate()
    {
        MoveFish();
        StayAboveGround();
        StayUnderWater();
    }

    private void MoveFish()
    {
        float deltaTime = Time.fixedDeltaTime;

        // fish propels itself in model forward direction only
        _forwardDirection = _tf.forward;

        if (!isConnected) {
            // get distance from lure
            Vector3 lureVector = lurePoint - _tf.position;
            float lureDistance = lureVector.magnitude;

            // disconnect from physics if attached to lure
            if (lureDistance < lureCollisionDistance) {
                isConnected = true;
                _rb.isKinematic = true;
                _rb.detectCollisions = false;
                return;
            }

            float time = Time.time * _noiseSpeed;
            float noiseYaw = Mathf.PerlinNoise(time + _noiseOffsetYaw, _noiseOffsetYaw) - 0.5f;
            float noisePitch = Mathf.PerlinNoise(time + _noiseOffsetPitch, _noiseOffsetPitch) - 0.5f;
            float noiseThrust = Mathf.PerlinNoise(time + _noiseOffsetThrust, _noiseOffsetThrust);

            // apply attraction force from lure
            Vector3 attractionVector = lureVector.normalized * lureMass * _rb.mass / lureVector.sqrMagnitude;
            _rb.AddForce(attractionVector * deltaTime);

            float urgencyFactor = lureVector.magnitude > _lureDetectionRadius ? 0.0f : 1.0f - (lureVector.magnitude / _lureDetectionRadius);

            // rotate
            if (lureVector.magnitude > _lureDetectionRadius) {
                // perlin wandering
                _rb.AddTorque(new Vector3(0.0f, _rotationSpeed * noiseYaw * deltaTime, 0.0f));
            } else {
                if (Random.Range(0, _lureDetectionRadius) > lureVector.magnitude) {
                    // TODO: turn away from lure
                    _rb.AddTorque(new Vector3(0.0f, 3 * _rotationSpeed * deltaTime, 0.0f));
                } else {
                    // perlin wandering
                    _rb.AddTorque(new Vector3(0.0f, _rotationSpeed * noiseYaw * deltaTime, 0.0f));
                }
            }

            // apply thrust in forward direction
            _rb.AddForce(_forwardDirection * _swimSpeed * deltaTime * (1 + urgencyFactor + noiseThrust));

            // apply linear drag
            _rb.AddForce(-1.0f * _linearDragCoefficient * _rb.linearVelocity.sqrMagnitude * deltaTime * _rb.linearVelocity.normalized);

            // apply angular drag
            _rb.AddTorque(-1.0f * _angularDragCoefficient * _rb.angularVelocity.sqrMagnitude * deltaTime * _rb.angularVelocity.normalized);
        }
    }

    private void StayAboveGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up * _minDistanceAboveTerrain, Vector3.down, out var hit, TERRAIN_CHECK_DISTANCE)) {
            if (!hit.collider) {
                // this probably means the fish has ended up below the terrain. To avoid issues, just destroy.
                Destroy(this);
            } else if (hit.collider.CompareTag("Terrain")) {
                float targetY = hit.point.y + _minDistanceAboveTerrain;
                if (transform.position.y < targetY) {
                    float deltaTime = Time.fixedDeltaTime;
                    Vector3 normalVector = hit.normal;
                    Vector3 horizontalVector = new Vector3(normalVector.x, 0.0f, normalVector.z).normalized;
                    _rb.AddForce(horizontalVector * SHORE_PUSH_FORCE * deltaTime);
                    _rb.AddForce(Vector3.up * VERTICAL_ADJUST_FORCE * deltaTime);
                }
            }
        }
    }

    private void StayUnderWater()
    {
        if (transform.position.y > WATER_LEVEL - _minDistanceBelowWater)
        {
            float deltaTime = Time.fixedDeltaTime;
            _rb.AddForce(Vector3.up * -VERTICAL_ADJUST_FORCE * deltaTime);
        } 
    }
}
