using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{
    protected Animator animator;

    public Transform newTarget;
    public float newTargetWeight;
    public Transform rightHandIK;
    public float rightHandweight = 1.0f;


    void Start()
    {
        // TODO: ein Script anhängen mit und mit getComponent danach suchen
        animator = GetComponent<Animator>();
    }

	//a callback for calculating IK
	void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {

            // Set the right hand target position and rotation, if one has been assigned
            if (rightHandIK != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandweight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandweight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, GetCurrentPosition());
                animator.SetIKRotation(AvatarIKGoal.RightHand, GetCurrentRotation());
            }

        }
    }

    public void MoveNewTargetWeight (float weight, float time) {

        newTargetWeight = Mathf.MoveTowards(newTargetWeight, weight, Time.deltaTime * time);
    }



    Vector3 GetCurrentPosition () {
        if (!newTarget)
            return rightHandIK.position;
        return Vector3.Lerp(rightHandIK.position, newTarget.position, newTargetWeight);
    }

    Quaternion GetCurrentRotation() {
        if (!newTarget)
            return rightHandIK.rotation;
        return Quaternion.Lerp(rightHandIK.rotation, newTarget.rotation, newTargetWeight);
    }

    public GameObject FindComponentInChildWithTag(GameObject parent, string tag)
    {
        GameObject[] children = parent.GetComponentsInChildren<GameObject>();
        foreach (GameObject go in children)
        {
            if (go.tag == tag)
            {
                print("found " + go.name + " in children");
                return go;
            }
        }
        return null;
    }

    public GameObject FindComponentInChildWithTag(string tag)
    {
        return FindComponentInChildWithTag(gameObject, tag);

    }
}

