using Niantic.ARDK.Extensions;
using UnityEngine;
using UnityEngine.UI;

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

    public void ToggleDissolveSensitivity()
    {
        var vfxController = FindObjectOfType<AudioVFXController>();
        if (!vfxController.Dissolve) return;
        vfxController.Rescaled = !vfxController.Rescaled;
        Debug.Log($"Low Dissolve Sensitivity Toggled: {vfxController.Rescaled}");
    }
}
