/// <summary>
/// Class holds basic info for a given band
/// </summary>
public class Band
{
    public float Intensity;
    public bool Triggered;

    public Band(float intensity, bool triggered)
    {
        Intensity = intensity;
        Triggered = triggered;
    }
}
