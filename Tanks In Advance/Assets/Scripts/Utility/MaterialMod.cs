using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialMod
{
    
    //cached property indices
    public static readonly int PropColor = Shader.PropertyToID("_BaseColor");
    public static readonly int PropColor2 = Shader.PropertyToID("_Color");
    public static readonly int PropEmissive = Shader.PropertyToID("_EmissionColor");

    public static void SetOpacity(float opacity, MeshRenderer renderer, MaterialPropertyBlock propertyBlock, int matIndex = 0)
    {
        SetOpacity(opacity, renderer, propertyBlock, PropColor, matIndex);
    }
    
    public static void SetOpacity(float opacity, MeshRenderer renderer, MaterialPropertyBlock propertyBlock, int propColor, int matIndex = 0)
    {
        if (matIndex == 0)
        {
            renderer.GetPropertyBlock(propertyBlock);
            Color originalColor = renderer.sharedMaterial.GetColor(propColor);
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, opacity);
            propertyBlock.SetColor(propColor, newColor);
            renderer.SetPropertyBlock(propertyBlock);
        }
        else
        {
            renderer.GetPropertyBlock(propertyBlock, matIndex);
            Color originalColor = renderer.sharedMaterials[matIndex].GetColor(propColor);
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, opacity);
            propertyBlock.SetColor(propColor, newColor);
            renderer.SetPropertyBlock(propertyBlock, matIndex);
        }
    }

    public static void SetColor(Color newColor, MeshRenderer renderer, MaterialPropertyBlock propertyBlock, int matIndex = 0)
    {
        SetColorProperty(newColor, renderer, propertyBlock, PropColor, matIndex);
    }
    
    public static void SetEmissiveColor(Color newColor, MeshRenderer renderer, MaterialPropertyBlock propertyBlock, int matIndex = 0)
    {
        SetColorProperty(newColor, renderer, propertyBlock, PropEmissive, matIndex);
    }
    
    //Hacky, should just remove matIndex?
    public static void SetColorProperty(Color newColor, MeshRenderer renderer, MaterialPropertyBlock propertyBlock, int prop, int matIndex = 0)
    {
        if (matIndex == 0)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector(prop, newColor);
            renderer.SetPropertyBlock(propertyBlock);
        }
        else
        {
            renderer.GetPropertyBlock(propertyBlock, matIndex);
            propertyBlock.SetVector(prop, newColor);
            renderer.SetPropertyBlock(propertyBlock, matIndex);
        }
    }

    public static Color GetColor(MeshRenderer renderer, MaterialPropertyBlock propertyBlock, int matIndex = 0)
    {
        return GetColorProperty(renderer, propertyBlock, PropColor, matIndex);
    }
    
    public static Color GetEmissiveColor(MeshRenderer renderer, MaterialPropertyBlock propertyBlock, int matIndex = 0)
    {
        return GetColorProperty(renderer, propertyBlock, PropEmissive, matIndex);
    }

    public static Color GetColorProperty(MeshRenderer renderer, MaterialPropertyBlock propertyBlock, int prop, int matIndex = 0)
    {
        if (matIndex == 0)
        {
            renderer.GetPropertyBlock(propertyBlock);
        }
        else
        {
            renderer.GetPropertyBlock(propertyBlock, matIndex);
        }
        return propertyBlock.GetVector(prop);
    }

    public static IEnumerator LerpColor(Color end, float time, MeshRenderer renderer,
        MaterialPropertyBlock propertyBlock, int Prop, int matIndex = 0)
    {
        float timeElapsed = 0f;
            renderer.GetPropertyBlock(propertyBlock);
        Color startColor = propertyBlock.GetColor(Prop);
        while (timeElapsed < time)
        {
            Color newColor = Color.Lerp(startColor, end, timeElapsed / time);
            renderer.GetPropertyBlock(propertyBlock, matIndex);
            propertyBlock.SetColor(Prop, newColor);
            renderer.SetPropertyBlock(propertyBlock, matIndex);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        renderer.GetPropertyBlock(propertyBlock, matIndex);
        propertyBlock.SetColor(Prop, end);
        renderer.SetPropertyBlock(propertyBlock, matIndex);
    }
    
    private static byte k_MaxByteForOverexposedColor = 191; //internal Unity const 
    
    //directly from https://answers.unity.com/questions/1652854/how-to-get-set-hdr-color-intensity.html
    public static Color ChangeHDRColorIntensity(Color subjectColor, float intensityChange)
    {
        // Get color intensity
        float maxColorComponent = Mathf.Max(subjectColor.maxColorComponent, 0.01f);
        float scaleFactorToGetIntensity = k_MaxByteForOverexposedColor / maxColorComponent;
        float currentIntensity = Mathf.Log(255f / scaleFactorToGetIntensity) / Mathf.Log(2f);

        // Get original color with ZERO intensity
        float currentScaleFactor = Mathf.Pow(2, currentIntensity);
        Color originalColorWithoutIntensity = subjectColor / currentScaleFactor;

        // Add color intensity
        float modifiedIntensity = currentIntensity + intensityChange;

        // Set color intensity
        float newScaleFactor = Mathf.Pow(2, modifiedIntensity);
        Color colorToRetun = originalColorWithoutIntensity * newScaleFactor;

        // Return color
        return colorToRetun;
    }
}
