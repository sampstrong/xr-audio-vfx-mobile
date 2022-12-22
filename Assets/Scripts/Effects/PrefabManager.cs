using Niantic.Experimental.ARDK.SharedAR;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Use class to spawn a prefab at a predefined position
/// </summary>
public class PrefabManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject _prefabToSpawn;
    [SerializeField] private float _yOffset = 5;
    private Vector3 _spawnPos;

    private bool _isInstantiated;


    private void Update()
    {
        _spawnPos = _mainCamera.transform.position + new Vector3(0, _yOffset, 0);
    }
    
    /// <summary>
    /// Spawns prefab at position defined in the inspector. Public access allows for gesture based spawning
    /// while _isInstantiated bool ensures the prefab is only instantiated once.
    /// </summary>
    [Button]
    public void SpawnPrefab()
    {
        Debug.Log($"Prefab Spawned at position: {_spawnPos}");
        if (_isInstantiated) return;
        Instantiate(_prefabToSpawn, _spawnPos, Quaternion.identity);
        _isInstantiated = true;
    }
    
    public void Reset()
    {
        _isInstantiated = false;
    }
}
