using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator>
{
    public EffectManager EffectManager => _effectManager;
    public Camera ARCamera => _aRCamera;
    
    [SerializeField] private EffectManager _effectManager;
    [SerializeField] private Camera _aRCamera;
}
