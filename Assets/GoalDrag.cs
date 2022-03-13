using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDrag : MonoBehaviour
{
    private Color mouseOverColor = Color.red;
    private Color originalColor;
    private bool dragging = false;
    private float distance;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }
    void OnMouseEnter()
    {
        rend.material.color = mouseOverColor;
    }

    void OnMouseExit()
    {
        rend.material.color = originalColor;
    }

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }

    void OnMouseUp()
    {
        dragging = false;
        GameObject.Find("Manager").GetComponent<Manager>().SetNewGoal(gameObject);
    }

    void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = new Vector3(rayPoint.x, transform.position.y, rayPoint.z);
        }
    }
}
