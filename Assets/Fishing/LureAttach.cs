using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LureAttach : MonoBehaviour
{
    Lure _parentLure;
    [SerializeField] float _radius; 
    SphereCollider _collider;

    private void Start()
    {
        _parentLure = GetComponentInParent<Lure>();
        _collider = GetComponent<SphereCollider>();
        _radius = _parentLure._lureData.LureAttachedRadius; 
        _collider.radius = _radius;
        _collider.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        var magneticObject = other.GetComponent<IAmMagnetic>();
        if (magneticObject != null)
        {
            _parentLure.AttachMagneticToLure(magneticObject);
            UpdateRadius(magneticObject.GetMass());
        }
    }
    void OnTriggerExit(Collider other)
    {
        var magneticObject = other.GetComponent<IAmMagnetic>();
        if (magneticObject != null)
        {
            UpdateRadius(-magneticObject.GetMass());
            _parentLure.RemoveMagneticFromLure(magneticObject);
        }
    }

    /// <summary>
    /// Increase or decrease the radius of the lure sphere collider based on the mass of the lure.
    /// Basic implementation just adds the mass to the radius currently. 
    /// </summary>
    /// <param name="mass"></param>
    void UpdateRadius(float mass)
    {   
        _radius += mass * 1f; 
        _collider.radius = _radius;
        Debug.Log("New radius: " + _collider.radius);   
    }

    public void Reset()
    {
        _radius = _parentLure._lureData.LureAttachedRadius; 
        _collider.radius = _radius;
        _collider.isTrigger = true;
    }
}
