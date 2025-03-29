#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FishSpawner))]
public class FishSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector.
        DrawDefaultInspector();

        // Add a button that calls CastRaysAndPlacePrefabs.
        FishSpawner spawner = (FishSpawner)target;
        if (GUILayout.Button("Spawn Fish"))
        {
            spawner.CastRaysAndPlacePrefabs();
        }

        if (GUILayout.Button("Clear Fish"))
        {
            var childFish = spawner.GetComponentsInChildren<TempFishMovement>();
            foreach (var fish in childFish)
            {
                Undo.DestroyObjectImmediate(fish.gameObject);
            }
            
        }
    }
}
#endif