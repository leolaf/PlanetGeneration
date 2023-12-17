using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INoiseFilter
{
    /// <summary>
    ///     Evaluate a point elevation based on the current noise filter
    /// </summary>
    /// <param name="point">The point to evaluate</param>
    /// <returns>The elevation based on the current noise filter implementation</returns>
    float Evaluate(Vector3 point);
}
