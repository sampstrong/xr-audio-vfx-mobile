using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class InstructionPanel : MonoBehaviour
{
    [SerializeField] private Toggle _dontShowAgainToggle;
    [SerializeField] private GameObject _panel;
    [SerializeField] private bool _showOnStart;

    private bool _dontShow;
    private bool _alreadyShown;

    private void Start()
    {
        if (PlayerPrefs.GetInt("dontShow") == 1)
            _dontShow = true;
        else
            _dontShow = false;
        
        if (_showOnStart)
            ShowPanel();
    }

    public void ShowPanel()
    {
        if (_dontShow) return;
        if (_alreadyShown) return;
        
        _panel.SetActive(true);
        _alreadyShown = true;
        TouchManager.DisableTouch();
    }

    public void ClosePanel()
    {
        if (_dontShowAgainToggle.isOn)
            PlayerPrefs.SetInt("dontShow", 1);
        else
            PlayerPrefs.SetInt("dontShow", 0);

        _panel.SetActive(false);
        TouchManager.EnableTouch();
        PlayerPrefs.Save();
    }

    [Button]
    private void Reset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
