using UnityEngine;

[CreateAssetMenu(fileName = "LureData", menuName = "ScriptableObjects/Fishing/LureData", order = 2)]
public class LureData : ScriptableObject
{
    //the starting radius where fish will start to be attracted to the lure
    public float LureMagneticRadius = 10f;
    //the radius at which point fish will become 'attached' and their mass will be added to the lure's mass
    public float LureAttachedRadius = 3f;
    //the starting mass of the lure
    public float LureMagneticMass = 1f;
}
