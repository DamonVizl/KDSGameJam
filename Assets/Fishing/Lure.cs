using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Lure : MonoBehaviour
{
    Scorer _scorer = new Scorer(); //reference to the scorer, used to score the fish caught.
    [SerializeField] public LureData _lureData; //refernece to SO for initial values
    private LureAttach _lureAttach; 
    [SerializeField] private float _magneticMass = 1.0f; // The mass of the lure itself
    [SerializeField] private float _radius = 20f; // The radius of the lure's magnetic field
    public float MagneticMass => _magneticMass; // The mass of the lure itself
    SphereCollider _collider;
    
    List<IAmMagnetic> _magneticObjects = new List<IAmMagnetic>();
    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _radius = _lureData.LureMagneticRadius; 
        _lureAttach = GetComponentInChildren<LureAttach>();
    }
    void FixedUpdate()
    {   
        //iterate through the list and apply force to each of the magnetic objects that are close enough. 
        foreach(var magneticObject in _magneticObjects)
        {
            //magneticObject.GetRigidbody().AddForce(Vector3.up * _magneticMass * 10f, ForceMode.Force);
            Vector3 direction = (transform.position - magneticObject.GetPosition()).normalized;
            float distance = Vector3.Distance(transform.position, magneticObject.GetPosition());
            magneticObject.ApplyForce(direction, distance, _magneticMass);
        }
    }
    /// <summary>
    /// Clear the list of magnetic objects, important to have this for when the player
    /// recalls the line, as OnTriggerExit won't be called for any objects on the disabled lure. 
    /// </summary>
    public void Reset()
    {
        _magneticObjects.Clear();
        _magneticMass = _lureData.LureMagneticMass; //reset the mass of the lure to 1
        _radius = _lureData.LureMagneticRadius; //reset the radius of the lure the SO value
        //reset out the child lure attach too
        _lureAttach.Reset(); 
    }

    void OnTriggerEnter(Collider other)
    {       
        Debug.Log("collision detected   " + other.name);
        var magneticObject = other.GetComponent<IAmMagnetic>();
        if( magneticObject is IAmMagnetic)
        {
            Debug.Log("Magnetic object entered the trigger: " + other.name);
            AddMagneticToNearLure(magneticObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var magneticObject = other.GetComponent<IAmMagnetic>();
        if( magneticObject is IAmMagnetic)
        {
            Debug.Log("Magnetic object entered the trigger: " + other.name);
            RemoveMagneticFromNearLure(magneticObject);
        }
    }

    void AddMagneticToNearLure(IAmMagnetic magneticObject)
    {
        _magneticObjects.Add(magneticObject);
        magneticObject.AddToNearLure(this);
    }
    void RemoveMagneticFromNearLure(IAmMagnetic magneticObject)
    {
        _magneticObjects.Remove(magneticObject);
        magneticObject.RemoveFromNearLure();
    }
    /// <summary>
    /// when the magnetic object is close enough, add it's mass to the lure's mass.
    /// increase the radius of the magnetic field
    /// </summary>
    /// <param name="magneticObject"></param>
    public void AttachMagneticToLure(IAmMagnetic magneticObject)
    {
        if(_magneticObjects.Contains(magneticObject))
        {
            _magneticMass += magneticObject.GetMass();
            UpdateRadius(magneticObject.GetMass());
            magneticObject.SetOnHook(); //set the magnetic object on the hook
            Debug.Log("Magnetic object attached to lure: " + magneticObject + " mass: " + magneticObject.GetMass());
            Debug.Log("New lure mass: " + _magneticMass);
        } 
    }
    /// <summary>
    /// when the magnetic object is far enough
    /// </summary>
    /// <param name="magneticObject"></param>
    public void RemoveMagneticFromLure(IAmMagnetic magneticObject)
    {
        if(_magneticObjects.Contains(magneticObject))
        {
            Debug.Log("Magnetic object removed from lure: " + magneticObject + " mass: " + magneticObject.GetMass());
            magneticObject.RemoveFromHook(); //remove the magnetic object from the hook
             _magneticMass -= magneticObject.GetMass();
             UpdateRadius(-magneticObject.GetMass());
        }
    }

    void UpdateRadius(float mass)
    {
        _radius += mass* 1f; 
        _collider.radius = _radius;
        Debug.Log("New radius: " + _collider.radius);   
    }
    public void Score()
    {
        Debug.Log("trying to score fish");
        _scorer.ScoreAllFish(_magneticObjects);
    }
}
