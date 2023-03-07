using UnityEngine;

public abstract class NetworkObject : MonoBehaviour
{
    public enum StrobeState
    {
        None = 0,
        Synchronized = 1,
        Randomized = 2,
    }

    public StrobeState _strobeState { get; set; }

    public enum VisibilityState
    {
        Off = 0,
        On = 1
    }
    
    public VisibilityState _visibilityState { get; set; }

    public abstract void Init(int index, NetworkGroup group, NetworkController controller);

    public abstract void ControlVis(VisibilityState state);

    public abstract void Strobe(StrobeState state);

}
