using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public Material planetMaterial;
    public Gradient oceanColor;
    public BiomeColorSettings biomeColorSettings;

    [System.Serializable]
    public class BiomeColorSettings
    {
        public Biome[] biomes;
        public NoiseSettings noise;
        public float noiseOffset;
        public float noiseStrength;
        [Range(0f, 1f)]
        public float blendAmount;

        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;
            [Range(0f, 1f)]
            public float startHeight;
            [Range(0f, 1f)]
            public float tintPercent;
        }
    }

    public ColorSettings Clone()
    {
        ColorSettings clone = CreateInstance<ColorSettings>();
        clone.planetMaterial = this.planetMaterial;
        clone.oceanColor = this.oceanColor;
        clone.biomeColorSettings = this.biomeColorSettings;
        return clone;
    }
}
