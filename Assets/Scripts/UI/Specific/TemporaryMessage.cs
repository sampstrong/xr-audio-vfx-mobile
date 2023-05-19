using System.Collections;
using UnityEngine;

public class TemporaryMessage : MonoBehaviour
{
    [SerializeField] private GameObject _textObject;

    public void ShowMessage()
    {
        _textObject.SetActive(true);
        StartCoroutine(MessageTimeout());
    }

    private IEnumerator MessageTimeout()
    {
        yield return new WaitForSeconds(2f);
        _textObject.SetActive(false);
    }
}
