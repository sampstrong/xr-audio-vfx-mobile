using System;
using System.Collections.Generic;

using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Anchors;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.Utilities.Input.Legacy;
using Niantic.ARDKExamples.Common.Helpers;

using UnityEngine;
using UnityEngine.UI;

public class SpatialAnchorManager : MonoBehaviour
{
    public GameObject CurrentObject => _currentObject;

    public Camera Camera;

    public event Action<GameObject> CurrentObjectSet;
    
    [SerializeField] private float _yOffset = 5;
    [SerializeField] private float _zOffset = 1;
    
    public Text AnchorDisplayText;

    private GameObject _newObject;
    private GameObject _currentObject;

    private IARSession _session = null;
    private Dictionary<Guid, IARAnchor> _addedAnchors = new Dictionary<Guid, IARAnchor>();
    private Dictionary<Guid, GameObject> _placedObjects = new Dictionary<Guid, GameObject>();

    private void Awake()
    {
      // Listen for the ARSession that is created/run by the ARSessionManager component in the scene.
      ARSessionFactory.SessionInitialized += OnARSessionInitialized;
    }

    private void OnARSessionInitialized(AnyARSessionInitializedArgs args)
    {
      _session = args.Session;
      _session.AnchorsAdded += OnAnchorsAdded;
      _session.AnchorsRemoved += OnAnchorsRemoved;
      _session.Deinitialized += _ => _session = null;
    }

    private void Update()
    {
      if (_session == null)
        return;

      // Get the current frame
      var currentFrame = _session.CurrentFrame;
      if (currentFrame == null)
        return;

      // Display the number of anchors
      // AnchorDisplayText.text = "Anchors: " + currentFrame.Anchors.Count;
    }

    /// <summary>
    /// Places anchor at above camera based on Y offset specified in inspector
    /// </summary>
    public void PlaceAnchor(GameObject objToPlace)
    {
      _newObject = objToPlace;

      var cam = Camera.transform;

      var horizontalPos = cam.forward * _zOffset;
      
      var position = cam.position + new Vector3(horizontalPos.x, _yOffset, horizontalPos.z);
      var rotation = Quaternion.Euler(new Vector3(0, cam.rotation.eulerAngles.y, 0));
      
      var anchor = _session.AddAnchor(Matrix4x4.TRS(position, rotation, Vector3.one));
      _addedAnchors.Add(anchor.Identifier, anchor);

      Debug.LogFormat("Created anchor (id: {0}, position: {1} ", anchor.Identifier, position.ToString("F4"));
    }

    private void OnDestroy()
    {
      // OnDestroy being called means the scene was unloaded. So ARSessionManager will
      // dispose the session and we don't have to do any session related cleanup.

      _addedAnchors.Clear();

      ARSessionFactory.SessionInitialized -= OnARSessionInitialized;
    }

    private void OnAnchorsAdded(AnchorsArgs args)
    {
      foreach (var anchor in args.Anchors)
      {
        if (!_addedAnchors.ContainsKey(anchor.Identifier))
        {
          // Plane and image detection are both disabled in this scene, so the only anchors getting
          // surfaced through this callback are the anchors added in HitTestToPlaceAnchor.
          Debug.LogWarningFormat
          (
            "Found anchor (id: {0}) not added by this class. This should not happen.",
            anchor.Identifier
          );

          continue;
        }
        
        // destroy existing effect if there is one before creating a new one
        if (_currentObject != null)
        {
          Destroy(_currentObject);
        }


        // Create the cube object and add a component that will keep it attached to the new anchor.
        var effect =
          Instantiate
          (
            _newObject,
            anchor.Transform.GetPosition(),
            anchor.Transform.rotation
          );

        AttachToAnchor(effect, anchor);

        // Keep track of the anchor objects
        _placedObjects.Add(anchor.Identifier, effect);
        _currentObject = effect;
        
        CurrentObjectSet?.Invoke(_currentObject);
      }
    }

    private void AttachToAnchor(GameObject effectPrefab, IARAnchor anchor)
    {
      var attachment = effectPrefab.AddComponent<ARAnchorAttachment>();
      attachment.AttachedAnchor = anchor;
      
      // designed to shift up to be level with ground
      // not required for floating objects
      // uncomment if trying to place objects on ground
      // var cubeYOffset = _newObject.transform.localScale.y / 2;
      // attachment.Offset = Matrix4x4.Translate(new Vector3(0, cubeYOffset, 0));
    }

    private void OnAnchorsRemoved(AnchorsArgs args)
    {
      foreach (var anchor in args.Anchors)
      {
        if (_addedAnchors.ContainsKey(anchor.Identifier))
        {
          _addedAnchors.Remove(anchor.Identifier);

          Destroy(_placedObjects[anchor.Identifier]);
          _placedObjects.Remove(anchor.Identifier);
        }
      }
    }

    public void ClearAnchors()
    {
      if (_session == null)
        return;

      // Clear out anchors. The OnAnchorsRemoved method should get invoked and handle clearing
      // the placed objects.
      foreach (var anchor in _addedAnchors)
        _session.RemoveAnchor(anchor.Value);
    }
  }

