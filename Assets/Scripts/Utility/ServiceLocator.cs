using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator>
{
    public TouchManager TouchManager => _touchManager;
    public EffectManager EffectManager => _effectManager;
    public Camera ARCamera => _aRCamera;

    [SerializeField] private TouchManager _touchManager;
    [SerializeField] private EffectManager _effectManager;
    [SerializeField] private Camera _aRCamera;
}
