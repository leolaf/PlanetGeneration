using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory 
{
    /// <summary>
    ///     Create a INoiseFilter based on the settings filter type
    /// </summary>
    /// <param name="settings">The noise settings</param>
    /// <returns>A new instance of INoiseFilter of type equivalentof the one specified in the settings</returns>
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
        switch(settings.filterType)
        {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(settings.simpleNoiseSettings);
            case NoiseSettings.FilterType.Rigid:
                return new RigidNoiseFilter(settings.rigidNoiseSettings);
        }
        return null;
    }
}
