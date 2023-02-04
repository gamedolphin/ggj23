using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private SpriteRenderer stars;
    [SerializeField] private SpriteRenderer nebulae;
    [SerializeField] private Star starPrefab;

    private List<Transform> starChildren = new List<Transform>();

    public void Setup(int seed, float radius)
    {
        Reset();

        transform.localScale = Vector3.one * radius * 4;

        var seedRange = math.remap(0, 1000, 1, 10, seed);
        stars.material.SetFloat("_Seed", seedRange);

        stars.material.SetFloat("_Pixels", 500);

        var aspect = new Vector2(1, 1);

        stars.material.SetVector("_UV_Correct", aspect);

        nebulae.material.SetFloat("_Seed", seedRange);

        nebulae.material.SetFloat("_Pixels", 500);

        nebulae.material.SetVector("_UV_Correct", aspect);
    }

    private void Reset()
    {
    }
}
