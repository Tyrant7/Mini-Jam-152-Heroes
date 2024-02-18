using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerVisual : MonoBehaviour
{
    [Header("Animations")]
    Animator anim;
    [SerializeField] string[] flavourAnimationNames;
    [SerializeField] string walkAnimationName;
    [SerializeField] string idleAnimationName;
    [SerializeField] float minFlavourInterval, maxFlavourInterval;

    private float timeSinceLastFlavour = 0;
    private float decidedFlavourInterval;

    [Header("Movement")]
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float rotateSpeed = 180;
    private Transform target;

    private void Start()
    {
        anim = GetComponent<Animator>();
        walkSpeed += Random.Range(-0.3f, 0.3f);
        decidedFlavourInterval = Random.Range(minFlavourInterval, maxFlavourInterval);
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        if (!IsClose())
        {
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z), Vector3.up);

            // Movement is part of animation for some gross reason
            anim.Play(walkAnimationName);
            anim.applyRootMotion = true;
            anim.speed = walkSpeed;
        }
        else
        {
            anim.applyRootMotion = false;
            anim.speed = 1;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(walkAnimationName))
            {
                anim.Play(idleAnimationName);
            }

            float rotationDiff = target.eulerAngles.y - transform.eulerAngles.y;
            if (Mathf.Abs(rotationDiff) >= 2)
            {
                transform.Rotate(Vector3.up, Mathf.Clamp(rotationDiff, -1, 1) * rotateSpeed * Time.deltaTime);
            }
            else
            {
                timeSinceLastFlavour += Time.deltaTime;
                if (timeSinceLastFlavour >= decidedFlavourInterval)
                {
                    timeSinceLastFlavour = 0;
                    decidedFlavourInterval = Random.Range(minFlavourInterval, maxFlavourInterval);
                    anim.Play(flavourAnimationNames[Random.Range(0, flavourAnimationNames.Length)]);
                }
            }
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public bool IsClose()
    {
        Vector2 myPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetPos = new Vector2(target.position.x, target.position.z);
        float dist = Vector2.Distance(myPos, targetPos);
        return dist <= 0.2f;
    }
}
