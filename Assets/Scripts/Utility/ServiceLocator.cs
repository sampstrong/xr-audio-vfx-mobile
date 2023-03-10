using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator>
{
    public EffectManager EffectManager => _effectManager;
    
    [SerializeField] private EffectManager _effectManager;
}
