using UnityEngine;

public interface IAmMagnetic 
{
    void ApplyForce(Vector3 direction, float distance, float pullerMass); 
    float GetMass();
    Vector3 GetPosition();
    public FishType GetFishType();
    public void AddToNearLure(Lure lure);
    public void RemoveFromNearLure();
    public void SetOnHook();
    public void RemoveFromHook();
}

