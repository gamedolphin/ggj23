using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DestroyOnAnimationEnd : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
}
