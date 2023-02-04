using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    private void Start()
    {
        var chosen = sprites[Random.Range(0, sprites.Length)];
        GetComponent<SpriteRenderer>().sprite = chosen;
    }
}
