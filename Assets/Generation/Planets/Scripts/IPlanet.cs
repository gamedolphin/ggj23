using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface ILit
{
    Vector3 LightSource { get; set; }
    void SetLight(Vector2 pos);
}

public abstract class IPlanet : MonoBehaviour
{

    [SerializeField] public int Pixel = 100;
    [SerializeField] public string Seed = "Seed";
    [SerializeField] public float CalcSeed;
    [SerializeField] public bool GenerateColors = false;

    public abstract void Initialize();

    public abstract void UpdateViaEditor();
}
