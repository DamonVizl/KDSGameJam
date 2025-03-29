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
        _collider.radius = _radius;
        _collider.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other is IAmMagnetic magneticObject)
        {
            _parentLure.AttachMagneticToLure(magneticObject);
            UpdateRadius(_parentLure.MagneticMass);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other is IAmMagnetic magneticObject)
        {
            _parentLure.RemoveMagneticFromLure(magneticObject);
            UpdateRadius(_parentLure.MagneticMass);
        }
    }

    /// <summary>
    /// Increase or decrease the radius of the lure sphere collider based on the mass of the lure.
    /// Basic implementation just adds the mass to the radius currently. 
    /// </summary>
    /// <param name="mass"></param>
    void UpdateRadius(float mass)
    {   
        _radius += mass; 
        _collider.radius = _radius;
    }
}
