#if UNITY_EDITOR

using UnityEngine;

using UnityEditor;
using UnityEditor.SceneManagement;


[ExecuteInEditMode]
public class FishSpawner : MonoBehaviour
{
    public FishSpawnData[] spawnData;

    [Header("Grid Settings")] 
    public Vector3 gridCenter = Vector3.zero;
    public float gridWidth = 50f;
    public float gridDepth = 50f;

    [Header("Raycast Settings")] 
    public float raycastHeight = 1000f;

    [Header("Terrain Reference (Optional)")]
    public Terrain terrain;

    public void CastRaysAndPlacePrefabs()
    {
        if (spawnData == null || spawnData.Length == 0)
        {
            Debug.LogError("No spawn data assigned! Please assign at least one FishSpawnData object.");
            return;
        }

        for (int fishTypeIndex = 0; fishTypeIndex < spawnData.Length; fishTypeIndex++)
        {
            FishSpawnData selectedData = spawnData[fishTypeIndex];
            for (int i = 0; i < selectedData.fishSpawnAttempts; i++)
            {
                float randomX = Random.Range(-gridWidth / 2f, gridWidth / 2f);
                float randomZ = Random.Range(-gridDepth / 2f, gridDepth / 2f);
                Vector3 randomPosition = gridCenter + new Vector3(randomX, 0, randomZ);

                Vector3 rayOrigin = new Vector3(randomPosition.x, gridCenter.y + raycastHeight, randomPosition.z);
                Ray ray = new Ray(rayOrigin, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, raycastHeight * 2f, LayerMask.GetMask("Terrain")))
                {
                    // Check if the hit point's Y position is within the allowed spawn height range.
                    var fishIsWithinAllowedHeight = 
                        hit.point.y < selectedData.upperSpawnHeight &&
                        hit.point.y > selectedData.lowerSpawnHeight;
                    if (!fishIsWithinAllowedHeight)
                    {
                        Debug.Log(
                            $"Skipping spawn: hit point {hit.point.y} is not " +
                            $"within allowed range ( {selectedData.fishPrefab.name} {selectedData.lowerSpawnHeight}," +
                            $" {selectedData.upperSpawnHeight})");
                        continue;
                    }

                    // If a terrain is assigned, ensure the hit is on that terrain.
                    if (terrain != null && hit.collider.gameObject == terrain.gameObject)
                    {
                        Debug.Log($"Ray {i} hit terrain " +
                                  $"at: {hit.point} " +
                                  $"with prefab: {selectedData.fishPrefab.name}");
                        PlacePrefabAt(hit.point, selectedData);
                    }
                }
                else
                {
                    Debug.Log("Ray " + i + " did not hit any collider.");
                }
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

    }

    private void PlacePrefabAt(Vector3 position, FishSpawnData fishData)
    {

        Debug.Log($"Placing fish {fishData.fishPrefab.name} at position {position}.");

        if (fishData.schoolOfFish)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 schoolPosition = position + Random.insideUnitSphere;
                PlacePrefab(schoolPosition, fishData);
            }
        } else
            PlacePrefab(position, fishData);

    }

    private void PlacePrefab(Vector3 position, FishSpawnData fishData)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fishData.fishPrefab);
        if (instance == null)
        {
            Debug.Log($"Fish {fishData.fishPrefab.name} Missing.");
            return;
        }

        instance.transform.position = position + Vector3.up * 2;
        instance.transform.parent = transform;
        var fishScale = Random.Range(fishData.minFishScale, fishData.maxFishScale);
        instance.transform.localScale = Vector3.one * fishScale;
        var fishSwimScript = instance.GetComponent<FishSwimScript>();
        if (fishSwimScript != null) {
            fishSwimScript.mass = fishData.mass * fishScale;
            fishSwimScript.FishType = fishData.fishType;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gridCenter, new Vector3(gridWidth, 0, gridDepth));
    }
}

#endif