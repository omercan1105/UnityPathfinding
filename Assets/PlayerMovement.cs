using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    float moveSpeed = 3f;
    Vector3 targetPosition;
    bool moving = false;
    bool goingToGoal = false;
    bool paused = false;
    GameObject target;

    List<GameObject> path = new List<GameObject>();
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
       
        if (!paused)
        {
            if (!moving)
            {
                if (path != null && path.Count > 0)
                {
                    target = path[0];
                    targetPosition = path[0].transform.position;
                    moving = true;
                }
            }
            if (moving)
            {
                moveTowardsTarget();
            }
        }
    }


    private void moveTowardsTarget()
    {   if (target == null) return;
        if (Vector3.Distance(transform.position, targetPosition) > 0.51f && moving)
        {
            Vector3 dir = new Vector3 (targetPosition.x - transform.position.x, 0f, targetPosition.z - transform.position.z).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else if (moving)
        {
            transform.position = new Vector3(targetPosition.x, 0.5f, targetPosition.z);
            
            if (path.Count == 0)
            {
                StartCoroutine(Wait());
                moving = false;
                Manager.MoveToGoal(gameObject);
                GameObject.Find("Manager").GetComponent<Manager>().CreateNewGoal();
                return;
            }
            Manager.MoveTo(target, gameObject);
            path[0].GetComponent<MeshRenderer>().enabled = false;
            path.RemoveAt(0);
            if (path.Count > 0)
            {
                target = path[0];
                targetPosition = path[0].transform.position;
            }

        }
    }
    public void SetPath(List<GameObject> newpath)
    {
        path = newpath;
    }
    IEnumerator Wait()
    {
        paused = true;
        yield return new WaitForSeconds(2);
        paused = false;
    }
}
