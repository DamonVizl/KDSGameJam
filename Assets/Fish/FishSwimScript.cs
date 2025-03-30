using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
public class FishSwimScript : MonoBehaviour, IAmMagnetic
{
    public FishType FishType; 
    [SerializeField] float _swimSpeed = 300.0f;
    [SerializeField] float _rotationSpeed = 1000.0f;
    [SerializeField] float _bobSpeed = 3.0f;
    [SerializeField] float _noiseSpeed = 1f;
    [SerializeField] public float mass = 1.0f;

    [SerializeField] float _linearDragCoefficient = 10.0f;
    [SerializeField] float _angularDragCoefficient = 100.0f;

    [SerializeField] float _minDistanceAboveTerrain = 1.0f;
    [SerializeField] float _minDistanceBelowWater = 1.0f;
    
    private Vector3 _forwardDirection;

    private Lure _lure;
    private bool _isOnHook = false;
    private bool _isNearLure = false;

    Transform _tf;
    Rigidbody _rb;   

    private float _noiseOffsetYaw;
    private float _noiseOffsetY;
    private float _noiseOffsetThrust;

    const float WATER_LEVEL = 0f;
    const float TERRAIN_CHECK_DISTANCE = 100f;
    const float VERTICAL_ADJUST_FORCE = 20.0f;
    const float SHORE_PUSH_FORCE = 500.0f;

    void Start()
    {
        _tf = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody>();

        _noiseOffsetYaw = Random.Range(0f, 1000f);
        _noiseOffsetY = Random.Range(0f, 1000f);
        _noiseOffsetThrust = Random.Range(0f, 1000f);

        _rb.maxLinearVelocity = 20.0f;
        _rb.maxAngularVelocity = 2.0f;
    }

    void FixedUpdate()
    {
        MoveFish();

        if (!_isOnHook) {
            StayAboveGround();
            StayUnderWater();
        }
    }

    private void MoveFish()
    {
        float deltaTime = Time.fixedDeltaTime;

        // fish propels itself in model forward direction only
        _forwardDirection = _tf.forward;

        if (!_isOnHook) {
            float time = Time.time * _noiseSpeed;
            float noiseYaw = Mathf.PerlinNoise(time + _noiseOffsetYaw, _noiseOffsetYaw) - 0.5f;
            float noiseY = Mathf.PerlinNoise(time + _noiseOffsetY, _noiseOffsetY) - 0.5f;
            float noiseThrust = Mathf.PerlinNoise(time + _noiseOffsetThrust, _noiseOffsetThrust);

            // rotate
            if (_isNearLure && _lure != null) {
                // turn away from lure
                // float urgencyFactor = lureVector.magnitude > _lureDetectionRadius ? 0.0f : 1.0f - (lureVector.magnitude / _lureDetectionRadius);
                Vector3 lureVector = _lure.transform.position - _tf.position;
                // float lureDistance = lureVector.magnitude;
                float lureAngle = Vector3.Angle(lureVector, _forwardDirection);
                Vector3 lurePlaneNormalizedVector = new Vector3(lureVector.x, 0.0f, lureVector.z).normalized;
                Vector3 directionPlaneNormalizedVector = new Vector3(_forwardDirection.x, 0.0f, _forwardDirection.z).normalized;
                float rotation = Vector3.Cross(lurePlaneNormalizedVector, directionPlaneNormalizedVector).y > 0 ? 1.0f : -1.0f;
                _rb.AddTorque(new Vector3(0.0f, _rotationSpeed * 10.0f * rotation * ((180 - lureAngle) / 180) * deltaTime, 0.0f));
                _rb.AddForce(new Vector3(0.0f, _bobSpeed * noiseY, 0.0f));
            } else {
                // perlin wandering
                _rb.AddTorque(new Vector3(0.0f, _rotationSpeed * noiseYaw * deltaTime, 0.0f));
                _rb.AddForce(new Vector3(0.0f, _bobSpeed * noiseY, 0.0f));
            }

            // apply thrust in forward direction
            _rb.AddForce(_forwardDirection * _swimSpeed * deltaTime * (1 + noiseThrust));

            // apply linear drag
            _rb.AddForce(-1.0f * _linearDragCoefficient * _rb.linearVelocity.sqrMagnitude * deltaTime * _rb.linearVelocity.normalized);

            // apply angular drag
            _rb.AddTorque(-1.0f * _angularDragCoefficient * _rb.angularVelocity.sqrMagnitude * deltaTime * _rb.angularVelocity.normalized);
        }
    }

    private void StayAboveGround()
    {
        // cast a ray up to check if we're under the terrain
        if (Physics.Raycast(_tf.position, Vector3.up, out var upHit, TERRAIN_CHECK_DISTANCE)) {
            if (upHit.collider.CompareTag("Terrain")) {
                // we're under the terrain - teleport up
                _tf.position = upHit.point + Vector3.up * 0.1f;
                return;
            }
        }

        // cast a ray down to check if we're too close to the terrain
        if (Physics.Raycast(_tf.position + Vector3.up * 0.01f, Vector3.down, out var hit, TERRAIN_CHECK_DISTANCE)) {
            if (!hit.collider) {
                // didn't hit anything? teleport to just below water
                _tf.position = new Vector3(_tf.position.x, WATER_LEVEL - 0.1f, _tf.position.z);
                return;
            }
            
            if (hit.collider.CompareTag("Terrain")) {
                float distanceToTerrain = _tf.position.y - hit.point.y;
                if (_tf.position.y < hit.point.y + _minDistanceAboveTerrain) {
                    // found terrain too close - apply force down
                    float deltaTime = Time.fixedDeltaTime;
                    Vector3 normalVector = hit.normal;
                    Vector3 horizontalVector = new Vector3(normalVector.x, 0.0f, normalVector.z).normalized;
                    float percentageOfWayOfMinDistanceAboveTowardsShore = (_minDistanceAboveTerrain - distanceToTerrain) / _minDistanceAboveTerrain;
                    _rb.AddForce(horizontalVector * SHORE_PUSH_FORCE * (1 / Math.Max(0.01f, 1 - percentageOfWayOfMinDistanceAboveTowardsShore)) * deltaTime);
                    _rb.AddForce(Vector3.up * VERTICAL_ADJUST_FORCE  * deltaTime);
                }
                return;
            }
        }

        _tf.position = new Vector3(_tf.position.x, WATER_LEVEL - 0.1f, _tf.position.z);
    }

    private void StayUnderWater()
    {
        if (_tf.position.y > WATER_LEVEL) {
            // above water, fix immediately by teleporting below water
            _tf.position = new Vector3(_tf.position.x, WATER_LEVEL - 0.1f, _tf.position.z);
        } else if (_tf.position.y > WATER_LEVEL - _minDistanceBelowWater)
        {
            // below water but above minDistance, apply downward force
            float deltaTime = Time.fixedDeltaTime;
            _rb.AddForce(Vector3.up * -VERTICAL_ADJUST_FORCE * (1 + transform.position.y - WATER_LEVEL + _minDistanceBelowWater) * deltaTime);
        } 
    }

    public void ApplyForce(Vector3 direction, float distance, float pullerMass)
    {
        // apply attraction force from lure
        Vector3 attractionVector = 1000.0f * direction.normalized * pullerMass * _rb.mass / direction.sqrMagnitude;
        _rb.AddForce(attractionVector * Time.fixedDeltaTime);
    }

    public float GetMass()
    {
        return mass;
    }

    public Vector3 GetPosition()
    {
        return _tf.position;
    }

    public FishType GetFishType()
    {
        return FishType;
    }

    public void AddToNearLure(Lure lure) {
        _lure = lure;
        _isNearLure = true;
    }

    public void RemoveFromNearLure() {
        _isNearLure = false;
    }

    public void SetOnHook()
    {
        _isOnHook = true;
    }

    public void RemoveFromHook()
    {
        _isOnHook = false;
    }
}
