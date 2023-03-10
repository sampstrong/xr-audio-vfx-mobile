public interface IAdjustable
{
    public void AdjustXPos(float xOffset);

    public void AdjustYPos(float yOffset);
    
    public void AdjustZPos(float zOffset);
    
    public void Recenter();

    public void ResetStartPos();
}
