using UnityEngine;

/// <summary>
/// Add this class as component to any object to make it always face the main camera
/// </summary>
public class Billboard : MonoBehaviour
{
    private float _yOffset = 180f;
    
    private void OnWillRenderObject()
    {
        Vector3 targetPos = InteractionManager.Instance.MainCamera.transform.position;
        Vector3 lookDir = (targetPos - transform.position);
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        transform.rotation = lookRot * Quaternion.Euler(0, _yOffset, 0);
    }
}
