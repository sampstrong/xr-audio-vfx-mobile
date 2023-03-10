using UnityEngine;

public abstract class AdjustableEffect : MonoBehaviour
{
    public abstract void AdjustXPos(float xOffset);

    public abstract void AdjustYPos(float yOffset);
    
    public abstract void AdjustZPos(float zOffset);
    public abstract void Recenter();

    public abstract void ResetStartPos();
}
