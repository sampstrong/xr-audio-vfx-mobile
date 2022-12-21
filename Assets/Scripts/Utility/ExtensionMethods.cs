using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// Remaps float values from one range to another
    /// </summary>
    /// <param name="from"></param>
    /// <param name="fromMin"></param>
    /// <param name="fromMax"></param>
    /// <param name="toMin"></param>
    /// <param name="toMax"></param>
    /// <returns></returns>
    public static float Remap (this float value, float fromMin, float fromMax, float toMin,  float toMax)
    {
        var fromAbs  =  value - fromMin;
        var fromMaxAbs = fromMax - fromMin;      
       
        var normal = fromAbs / fromMaxAbs;
 
        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;
 
        var to = toAbs + toMin;
       
        return to;
    }

    /// <summary>
    /// Maps a 0 to 1 value to a number within a provided range
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float MapValue(this float value, float min, float max)
    {
        return (value * (max - min)) + min;
    }
}
