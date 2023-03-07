using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(NetworkController))]
public class NetworkGroup : MonoBehaviour
{
    public List<NetworkObject> NetworkObjects => _networkObjects;

    public event Action AllObjectsCreated; 

    [SerializeField] private NetworkController _networkController;
    [SerializeField] private GameObject _objectPrefab;

    private List<NetworkObject> _networkObjects = new List<NetworkObject>();
    
    // Must subscribe in Awake because event is occuring in Start of NetworkController
    // Event will think nothing is subscribed to it otherwise and not fire because it
    // will return a value of null for objects subscribed to it
    void Awake()
    {
        Assert.IsNotNull(_networkController, "No Network Controller Referenced in the Inspector");
        _networkController.NetworkInitialized += Init;
    }

    private void Init()
    {
        Debug.Log("Network Group Init Triggered");
        
        for (int i = 0; i < _networkController.NetworkSize; i++)
        {
            var newObj = Instantiate(_objectPrefab, _networkController.CurrentPositions[i], Quaternion.identity, transform);
            // var newNetworkObj = newObj.GetComponent(typeof(NetworkObject)) as NetworkObject;
            var newNetworkObj = newObj.GetComponent<NetworkObject>();

            newNetworkObj.Init(i, this, _networkController);
            _networkObjects.Add(newNetworkObj);
        }
        
        AllObjectsCreated?.Invoke();
    }
}
