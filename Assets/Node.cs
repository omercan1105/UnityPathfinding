using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int x, z;
    public GameObject goal;
    public float GetHeuristic()
    {
        if (goal != null) return Vector3.Distance(transform.position, goal.transform.position);
        else return float.MaxValue;
    }
    void Start()
    {
       
    }

    public void SetGoal(GameObject g)
    {
        goal = g;
    }
    public void SetCoordinates(int xPos, int zPos)
    {
        x = xPos;
        z = zPos;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
