using UnityEngine;

public interface IAmMagnetic 
{
    void ApplyForce(Vector3 direction, float distance, float pullerMass); 
    float GetMass();
    Vector3 GetPosition();
}

