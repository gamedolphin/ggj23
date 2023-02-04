using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private SpriteRenderer stars;

    private void Start()
    {
        stars.material.SetFloat("_Seed", Random.Range(0, 1));

        var rect_size = stars.bounds.size;

        stars.material.SetFloat("_Pixels", rect_size.x);

        var aspect = new Vector2(1, 1);
        if (rect_size.x > rect_size.y)
        {
            aspect = new Vector2(rect_size.x / rect_size.y, 1.0f);
        }
        else
        {
            aspect = new Vector2(1.0f, rect_size.y / rect_size.x);
        }

        stars.material.SetVector("_UV_Correct", aspect);
    }

    private void Update()
    {

    }
}
