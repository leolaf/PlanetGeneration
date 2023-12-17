using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;                     // The settings file we use to get all the informations we need to evaluate a point elevation
    INoiseFilter[] noiseFilters;                // All the noise layers we use to evaluate the elevation
    public MinMax elevationMinMax;              // Instance of MinMax that we will use in the shader to know how strong the elevation is

    /// <summary>
    ///     Setup the shape generator on each settings file change
    /// </summary>
    /// <param name="settings">The setting file</param>
    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        // Create an array of length determined by the number of layer we use
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        // For each of these layers, create an instance of INoiseFilter with the NoiseFilterFactory class
        for(int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
        // Initialize the MinMax instance
        elevationMinMax = new MinMax();
    }

    /// <summary>
    ///     Calculate a point elevation according to its position but not depending on the ocean level
    /// </summary>
    /// <param name="pointOnUnitSphere">The point we want to evaluate</param>
    /// <returns>The elevation not based on the ocean level</returns>
    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (noiseFilters.Length > 0)
        {
            // Evaluate the point position based on the first layer we configured
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                // If the first layer is enabled then store its current elevation
                elevation = firstLayerValue;
            }
        }

        // For each other layers
        for(int i = 1; i < noiseFilters.Length; i++)
        {
            // If it is enabled
            if (settings.noiseLayers[i].enabled)
            {
                // Initialize a coefficient based on the first layer elevation
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                // Add the layer elevation in the current elevation evaluation
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        // Don't forget to store this information in the MinMax instance so the shader can use it
        elevationMinMax.AddValue(elevation);
        return elevation;
    }

    /// <summary>
    ///     Get an elevation according to the ocean level
    /// </summary>
    /// <param name="unscaledElevation">The elevation</param>
    /// <returns>The elevation based on the ocean level</returns>
    public float GetScaledElevation(float unscaledElevation)
    {
        float elevation = Mathf.Max(0, unscaledElevation);
        // Don't forget to take the planet radius into account ;)
        elevation = settings.planetRadius * (1 + elevation);
        return elevation;
    }
}
