using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
    public float Min { get; private set; }      // Minimum elevation of the planet
    public float Max { get; private set; }      // Maximum elevatioon of the planet

    public MinMax() 
    {
        Min = float.MaxValue;
        Max = float.MinValue;
    }

    public void AddValue(float value)
    {
        if(value > Max) Max = value;
        if(value < Min) Min = value;
    }
}
