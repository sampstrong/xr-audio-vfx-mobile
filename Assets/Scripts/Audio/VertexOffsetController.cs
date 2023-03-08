using System.Collections;
using UnityEngine;

/// <summary>
/// Inherits from BandReceiver.
/// Controls strength of vertex offset for materials with animated,
/// noise based vertex offset shaders based on band triggers
/// </summary>
public class VertexOffsetController : BandReceiver
{
   [SerializeField] private Material _material;
   [SerializeField] private float _min = 0.002f;
   [SerializeField] private float _max = 0.006f;

   protected override void Start()
   {
      base.Start();
      SetStrength(_min);
   }
   
   protected override void OnBandTriggered(int band)
   {
      SetStrength(_max);
   }

   protected override void OnBandReleased(int band)
   {
      StartCoroutine(ReleaseWithDelay(1));
   }
   
   private IEnumerator ReleaseWithDelay(float duration)
   {
      yield return new WaitForSeconds(duration / 2);

      for (float t = 0; t < duration / 2; t += Time.deltaTime)
      {
         float density = Mathf.Lerp(_max, _min, t / (duration / 2));
         SetStrength(density);

         yield return null;
      }
      
      SetStrength(_min);
   }

   private void SetStrength(float value)
   {
      _material.SetFloat("_Strength", value);
   }
}
