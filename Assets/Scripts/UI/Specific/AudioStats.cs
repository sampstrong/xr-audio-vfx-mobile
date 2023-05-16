using TMPro;
using UnityEngine;

public class AudioStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _highTextAreas;
    [SerializeField] private TextMeshProUGUI[] _lowTextAreas;
    
    void Update()
    {
        for (int i = 0; i < AudioSpectrumReader._freqBandHighest.Length; i++)
        {
            _highTextAreas[i].text = AudioSpectrumReader._freqBandHighest[i].ToString("0.0");
        }
    }
}
