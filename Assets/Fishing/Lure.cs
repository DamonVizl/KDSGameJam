using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider))]
public class Lure : MonoBehaviour
{

    [SerializeField]
    private float _magneticMass = 1; // The mass of the lure itself
    [SerializeField] private float _radius = 20f; // The radius of the lure's magnetic field
    public float MagneticMass => _magneticMass; // The mass of the lure itself
    SphereCollider _collider;
    
    List<IAmMagnetic> _magneticObjects = new List<IAmMagnetic>();
    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.radius = _radius;
    }

    void OnTriggerEnter(Collider other)
    {       
        Debug.Log("collision detected   " + other.name);
        var magneticObject = other.GetComponent<IAmMagnetic>();
        if( magneticObject is IAmMagnetic)
        {
            Debug.Log("Magnetic object entered the trigger: " + other.name);
            AddMagneticObject(magneticObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var magneticObject = other.GetComponent<IAmMagnetic>();
        if( magneticObject is IAmMagnetic)
        {
            Debug.Log("Magnetic object entered the trigger: " + other.name);
            RemoveMagneticObject(magneticObject);
        }
    }

    void AddMagneticObject(IAmMagnetic magneticObject)
    {
        _magneticObjects.Add(magneticObject);
    }
    void RemoveMagneticObject(IAmMagnetic magneticObject)
    {
        _magneticObjects.Remove(magneticObject);
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
                        

    void Update()
    {
        foreach(var magneticObject in _magneticObjects)
        {
            //magneticObject.GetRigidbody().AddForce(Vector3.up * _magneticMass * 10f, ForceMode.Force);
            Vector3 direction = (transform.position - magneticObject.GetPosition()).normalized;
            float distance = Vector3.Distance(transform.position, magneticObject.GetPosition());
            magneticObject.ApplyForce(direction, distance, _magneticMass);
        }
    }


}
