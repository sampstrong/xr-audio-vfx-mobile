using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class VerticalShaftGroup : MonoBehaviour
{
    public List<VerticalShaft> Shafts => _shafts;
    
    [SerializeField] private NetworkController networkController;
    [SerializeField] private GameObject _shaftPrefab;

    private List<VerticalShaft> _shafts = new List<VerticalShaft>();
    
    void Awake()
    {
        networkController.NetworkInitialized += Init;
    }

    private void Init()
    {
        for (int i = 0; i < networkController.NetworkSize; i++)
        {
            var newShaft = Instantiate(_shaftPrefab, networkController.CurrentPositions[i], Quaternion.identity, transform).GetComponent<VerticalShaft>();
            newShaft.Init(i, this, networkController);
            _shafts.Add(newShaft);
        }
    }

   
}
