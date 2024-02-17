using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerVisual : MonoBehaviour
{
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float rotateSpeed = 180;
    private Transform target;

    private void Start()
    {
        walkSpeed += Random.Range(-0.3f, 0.3f);
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        if (!IsClose())
        {
            Vector3 delta = Vector3.MoveTowards(transform.position, new Vector3(target.position.x, transform.position.y, target.position.z), walkSpeed * Time.deltaTime);
            transform.position = delta;
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z), Vector3.up);
        }
        else
        {
            float rotationDiff = target.eulerAngles.y - transform.eulerAngles.y;
            if (Mathf.Abs(rotationDiff) >= 0.05f)
            {
                transform.Rotate(Vector3.up, Mathf.Clamp(rotationDiff, -1, 1) * rotateSpeed * Time.deltaTime);
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
        return dist <= 0.05f;
    }
}
