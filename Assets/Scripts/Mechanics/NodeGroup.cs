using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

/// <summary>
/// Class for creating and controlling a group of of audio reactive nodes
/// </summary>
public class NodeGroup : MonoBehaviour
{
    public List<Node> Nodes { get => _nodes; }
    public int NumberOfNodes { get => _numberOfNodes; }

    public enum NodeState
    {
        Inactive = 0,
        Active = 1,
        Spawning = 2
    }

    public NodeState nodeState;
    
    [Header("Nodes")]
    [SerializeField] private GameObject _nodePrefab;
    [SerializeField] private int _numberOfNodes = 10;
    [SerializeField] private float _minScale = 0.08f;
    [SerializeField] private float _maxScale = 0.2f;
    [SerializeField] private float _speed = 1f;

    [Header("Lines")]
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _distanceThreshold = 0.3f;
    [SerializeField] private float _maxLineWidth = 0.05f;
    [SerializeField] private float _minLineWidth = 0.01f;

    [Header("Bounds")] 
    [SerializeField] private float _xRange = 5f;
    [SerializeField] private float _yRange = 5f;
    [SerializeField] private float _zRange = 5f;

    [Header("Velocity")] 
    [SerializeField] private float _velocityUpperLimit = 5f;
    [SerializeField] private float _velocityLowerLimit = 1f;

    private float _xLowerBounds;
    private float _xUpperBounds;
    private float _yLowerBounds;
    private float _yUpperBounds;
    private float _zLowerBounds;
    private float _zUpperBounds;

    private List<Node> _nodes = new List<Node>();
    
    void Start()
    {
        // Assures chosen node prefab has a rigidbody that we can apply physics to
        Assert.IsNotNull(_nodePrefab.GetComponent<Rigidbody>(), 
            $"Node Prefab: {_nodePrefab.name} needs a rigidbody.");
        
        SetNodeState(NodeState.Active);
        //GetInteractionState();
        ///InteractionManager.Instance.onInteractionStateChanged.AddListener(GetInteractionState);
        SetBounds(_xRange, _yRange, _zRange);
        InitObjects();
    }

    /// <summary>
    /// Initializes node state based on interaction state
    /// </summary>
    private void GetInteractionState()
    {
        switch (InteractionManager.Instance.CurrentInteractionState)
        {
            case InteractionManager.InteractionState.Inactive:
                SetNodeState(nodeState = NodeState.Inactive);
                Debug.Log("Node State: Inactive");
                break;
            case InteractionManager.InteractionState.Active:
                SetNodeState(nodeState = NodeState.Active);
                Debug.Log("Node State: Active");
                break;
            case InteractionManager.InteractionState.Spawning:
                SetNodeState(nodeState = NodeState.Spawning);
                Debug.Log("Node State: Spawning");
                break;
            default:
                SetNodeState(nodeState = NodeState.Active);
                Debug.Log("Node State: Default");
                break;
        }
    }

    /// <summary>
    /// Sets the global node state and initializes each node
    /// </summary>
    /// <param name="state"></param>
    [Button]
    public void SetNodeState(NodeState state)
    {
        nodeState = state;

        foreach (var node in _nodes)
        {
            InitializeNodeState(node);
        }
    }

    /// <summary>
    /// Sets appropriate physics for each node depending on the current node state
    /// </summary>
    /// <param name="node"></param>
    private void InitializeNodeState(Node node)
    {
        if (nodeState == NodeState.Inactive)
        {
            node.rb.useGravity = true;
            node.rb.isKinematic = false;
        }
        else if (nodeState == NodeState.Active)
        {
            node.rb.useGravity = false;
            node.rb.isKinematic = false;
            node.rb.velocity = SetVelocity();
            if (node.obj.transform.position.y > _yLowerBounds + 0.5f)
            {
                node.rb.velocity += new Vector3(0, 2.5f, 0);
            }
        }
        else if (nodeState == NodeState.Spawning)
        {
            node.rb.useGravity = false;
            node.rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Creates a set of nodes at random positions based on the number of nodes
    /// and range specified in the inspector. Also adds a line renderer to each node
    /// for each other node for desired visual connection effect
    /// </summary>
    private void InitObjects()
    {
        for (int i = 0; i < _numberOfNodes; i++)
        {
            // Initialize Nodes
            GameObject nodeObject = Instantiate(_nodePrefab, GetRandomPosition() / 2, Quaternion.identity, gameObject.transform);
            nodeObject.transform.localScale = Vector3.one * Random.Range(_minScale, _maxScale);

            // Initialize Rigidbodies
            Rigidbody rb = nodeObject.GetComponent<Rigidbody>();
            
            // Initialize Renderers
            Renderer rend = nodeObject.GetComponentInChildren<OuterSphere>().GetComponent<Renderer>();

            // Initialize Node Object
            Node node = new Node();
            node.obj = nodeObject;
            node.rb = rb;
            node.rend = rend;
            
            InitializeNodeState(node);

            // Initialize Lines
            for (int j = 0; j < _numberOfNodes; j++)
            {
                GameObject child = Instantiate(new GameObject($"Line {j}"), node.obj.transform);
                LineRenderer line = child.AddComponent<LineRenderer>();
                line.material = _lineMaterial;
                line.positionCount = 0;

                node.children.Add(child);
                node.lines.Add(line);
            }

            // Add node to list
            _nodes.Add(node);
            
            // if spawning, run animation coroutine here
        }
    }

    /// <summary>
    /// Update handles change in line appearance and node position based on bounds
    /// </summary>
    private void Update()
    {
        DrawLines();
        if (nodeState == NodeState.Inactive || nodeState == NodeState.Spawning) return;
        EnforceBounds();
    }

    /// <summary>
    /// Fixed update handles changes in velocity for more predictable physics behavior
    /// </summary>
    private void FixedUpdate()
    {
        if (nodeState == NodeState.Inactive || nodeState == NodeState.Spawning) return;
        RegulateVelocity();
    }

    /// <summary>
    /// Sets the three-dimensional boundary for the nodes based on values assigned
    /// in the inspector.
    /// </summary>
    /// <param name="xRange"></param>
    /// <param name="yRange"></param>
    /// <param name="zRange"></param>
    private void SetBounds(float xRange, float yRange, float zRange)
    {
        Vector3 currentPos = transform.position;

        _xLowerBounds = currentPos.x - xRange;
        _xUpperBounds = currentPos.x + xRange;
        _yLowerBounds = currentPos.y - yRange;
        _yUpperBounds = currentPos.y + yRange;
        _zLowerBounds = currentPos.z - zRange;
        _zUpperBounds = currentPos.z + zRange;
    }

    /// <summary>
    /// Returns a random x, y, and z position within predefined bounds
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomPosition()
    {
        float xPos = Random.Range(_xLowerBounds, _xUpperBounds);
        float yPos = Random.Range(_yLowerBounds, _yUpperBounds);
        float zPos = Random.Range(_zLowerBounds, _zUpperBounds);

        return new Vector3(xPos, yPos, zPos);
    }

    /// <summary>
    /// Returns a random velocity. Both speed and direction will be randomized.
    /// </summary>
    /// <returns></returns>
    public Vector3 SetVelocity()
    {
        var value = Random.value;
        var randomSpeed = _speed;
        if (value < 0.5) randomSpeed *= -1;
        Vector3 velocity = GetRandomPosition() * randomSpeed;
        return velocity;
    }

    /// <summary>
    /// Sets the velocity for each node on first frame when switching from inactive
    /// or spawning states. Keeps nodes moving within velocity limits assigned in inspector
    /// </summary>
    private void RegulateVelocity()
    {
        foreach (var node in _nodes)
        {
            if (node.rb.velocity.magnitude == 0)
            {
                node.rb.velocity = SetVelocity();
                return;
            }
            
            // get current direction for each node to maintain direction when assigning velocities below
            var direction = node.rb.velocity / node.rb.velocity.magnitude;
            
            // set velocity to lower limit if velocity goes below that limit
            if (node.rb.velocity.magnitude < _velocityLowerLimit)
            {
                node.rb.velocity = direction * _velocityLowerLimit;
            }
            
            // set velocity to upper limit if velocity goes above that limit
            else if (node.rb.velocity.magnitude > _velocityUpperLimit)
            {
                node.rb.velocity = direction * _velocityUpperLimit;
            }
        }
    }

    /// <summary>
    /// Uses the defined bounds to reverse the velocity for each node when the position
    /// goes past the boundary on each axis. Results in a bouncing effect when nodes hit boundary.
    /// </summary>
    private void EnforceBounds()
    {
        foreach (var node in _nodes)
        {
            Vector3 position = node.rb.position;
            Vector3 velocity = node.rb.velocity;
            
            if (position.x < _xLowerBounds)
            {
                node.rb.position = new Vector3(_xLowerBounds, position.y, position.z);
                node.rb.velocity = new Vector3(-velocity.x, velocity.y, velocity.z);
            }
            else if (position.x > _xUpperBounds)
            {
                node.rb.position = new Vector3(_xUpperBounds, position.y, position.z);
                node.rb.velocity = new Vector3(-velocity.x, velocity.y, velocity.z);
            }

            if (position.y < _yLowerBounds)
            {
                node.rb.position = new Vector3(position.x, _yLowerBounds, position.z);
                node.rb.velocity = new Vector3(velocity.x, -velocity.y, velocity.z);
            }
            else if (position.y > _yUpperBounds)
            {
                node.rb.position = new Vector3(position.x, _yUpperBounds, position.z);
                node.rb.velocity = new Vector3(velocity.x, -velocity.y, velocity.z);
            }

            if (position.z < _zLowerBounds)
            {
                node.rb.position = new Vector3(position.x, position.y, _zLowerBounds);
                node.rb.velocity = new Vector3(velocity.x, velocity.y, -velocity.z);
            }
            else if (position.z > _zUpperBounds)
            {
                node.rb.position = new Vector3(position.x, position.y, _zUpperBounds);
                node.rb.velocity = new Vector3(velocity.x, velocity.y, -velocity.z);
            }
        }
    }

    /// <summary>
    /// Controls line visibility between nodes depending on state and distance threshold
    /// set in the inspector
    /// </summary>
    private void DrawLines()
    {
        // turn off lines for inactive and spawning states
        if (nodeState == NodeState.Inactive || nodeState == NodeState.Spawning)
        {
            foreach (var node in _nodes)
            {
                foreach (var line in node.lines)
                {
                    line.positionCount = 0;
                }
            }

            return;
        }
        
        // sets up line renderers to connect to other nearby nodes when they are closer than the threshold
        for (int i = 0; i < _nodes.Count; i++)
        {
            for (int j = i + 1; j < _nodes.Count; j++)
            {
                float distance = Vector3.Distance(_nodes[j].obj.transform.position, _nodes[i].obj.transform.position);
                
                if (distance < _distanceThreshold)
                {
                    _nodes[i].lines[j].positionCount = 2;
                    _nodes[i].lines[j].SetPosition(0, _nodes[i].obj.transform.position);
                    _nodes[i].lines[j].SetPosition(1, _nodes[j].obj.transform.position);
                    _nodes[i].lines[j].startWidth = 1 / distance.Remap(0, 3, _minLineWidth, _maxLineWidth);
                    _nodes[i].lines[j].endWidth = 1 / distance.Remap(0, 3, _minLineWidth, _maxLineWidth);
                }
                else
                {
                    _nodes[i].lines[j].positionCount = 0;
                }
            }
        }
    }
}

/// <summary>
/// Class holding data for each individual node
/// </summary>
public class Node
{
    public GameObject obj;
    public Rigidbody rb;
    public Renderer rend;
    public List<GameObject> children = new List<GameObject>();
    public List<LineRenderer> lines = new List<LineRenderer>();
}


