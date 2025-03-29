using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Lure : MonoBehaviour
{

    [SerializeField]
    private float _magneticMass = 1; // The mass of the lure itself
    public float MagneticMass => _magneticMass; // The mass of the lure itself
    
    List<IAmMagnetic> _magneticObjects = new List<IAmMagnetic>();


    void OnTriggerEnter(Collider other)
    {   
        if(other is IAmMagnetic magneticObject)
        {
            AddMagneticObject(magneticObject);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if(other is IAmMagnetic magneticObject)
        {
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
    /// </summary>
    /// <param name="magneticObject"></param>
    public void AttachMagneticToLure(IAmMagnetic magneticObject)
    {
        if(_magneticObjects.Contains(magneticObject)) _magneticMass += magneticObject.GetMass();
    }
    /// <summary>
    /// when the magnetic object is far enough
    /// </summary>
    /// <param name="magneticObject"></param>
    public void RemoveMagneticFromLure(IAmMagnetic magneticObject)
    {
        if(_magneticObjects.Contains(magneticObject)) _magneticMass -= magneticObject.GetMass();
    }
    void Update()
    {
        foreach(var magneticObject in _magneticObjects)
        {
            Vector3 direction = (transform.position - magneticObject.GetPosition()).normalized;
            float distance = Vector3.Distance(transform.position, magneticObject.GetPosition());
            magneticObject.ApplyForce(direction, distance, _magneticMass);
        }
    }


}
