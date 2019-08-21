
using UnityEngine;
using System.Collections;

public class WeaponHide : MonoBehaviour
{

    [SerializeField]
    Animator animator;

    [SerializeField]
    string weaponTag;

    public bool turned = true;

    SkinnedMeshRenderer weaponRenderer;

    void Start()
    {
        weaponRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    public bool isPlayingAnim(string s)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(s);
    }

    void Update()
    {
        if (!turned) return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag(weaponTag))
        {
            weaponRenderer.enabled = true;
        } else
        {
            weaponRenderer.enabled = false;
        }
    }
}

