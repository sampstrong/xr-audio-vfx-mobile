using Niantic.ARDK.Extensions;
using UnityEngine;

public class ObjectToggle : MonoBehaviour
{
    public void ToggleObject(GameObject objectToToggle)
    {
        if (objectToToggle.activeSelf) objectToToggle.SetActive(false);
        else objectToToggle.SetActive(true);
    }

    public void ToggleOcclusion(ARDepthManager occlusion)
    {
        occlusion.enabled = !occlusion.enabled;
    }

}
