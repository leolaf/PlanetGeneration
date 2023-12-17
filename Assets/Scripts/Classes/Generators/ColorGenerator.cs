using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;                     // The setting file we use to get all the informations we need to evaluate a point color
    
    // The texture we will use in the shader
    // (height = number of biome, width = resolution*2 (1/2 is ocean and 1/2 is biome color)
    Texture2D texture = null;                   
    
    const int textureResolution = 50;           // The texture resolution
    INoiseFilter biomeNoiseFilter;              // Noise to determine in which biome a point is stuated

    /// <summary>
    ///     Setup the color generator on each settings file change
    /// </summary>
    /// <param name="settings">The setting file</param>
    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if(texture == null || texture.height != settings.biomeColorSettings.biomes.Length)
        {
            texture = new Texture2D(textureResolution * 2, settings.biomeColorSettings.biomes.Length, TextureFormat.RGBA32, false);
        }
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    /// <summary>
    ///     Update the material's shader parameter for it to know the elevation min and max
    /// </summary>
    /// <param name="elevationMinMax">MinMax instance in which the planet elevation informations is stored</param>
    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    /// <summary>
    ///     Get the biome index of a point blended by its distance to the next biome
    /// </summary>
    /// <param name="pointOnUnitSphere">The point on the planet</param>
    /// <returns>The biome index of a point blended by its distance to the next biome</returns>
    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        // Get the point height
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        // Apply biome noise on this height
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;
        
        float biomeIndex = 0;
        int numBiomes = settings.biomeColorSettings.biomes.Length;
        float blendRange = settings.biomeColorSettings.blendAmount / 2f + .001f;

        for (int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);
            biomeIndex *= (1-weight);
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    /// <summary>
    ///     Update the texture use by the material and set its corresponding shader parameter
    /// </summary>
    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];
        int colorIndex = 0;
        foreach(var biome in settings.biomeColorSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                Color gradientCol;
                // Ocean texture
                if (i < textureResolution)
                {
                    gradientCol = settings.oceanColor.Evaluate(i / (textureResolution - 1f));
                }
                // Terrain texture
                else
                {
                    gradientCol = biome.gradient.Evaluate((i-textureResolution) / (textureResolution - 1f));
                    Color tintColor = biome.tint;
                    gradientCol = gradientCol * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
                }
                colors[colorIndex] = gradientCol;
                colorIndex++;
            }
        }
        texture.SetPixels(colors);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }
}
