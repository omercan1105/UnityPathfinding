using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject player;
    public GameObject ground;
    public GameObject obstacle;
    public GameObject goal;
    public GameObject emptyNode;

    public static GameObject[,] grid = new GameObject[30, 30];
    private static Vector3 gridOffset = new Vector3(-14.5f, 0.5f, -14.5f);
    private static GameObject curGoal;
    private static List<GameObject> curPath = new List<GameObject>();
    static int goalX, goalZ; 
    static int playerX = 0; 
    static int playerZ = 0;

    void Start()
    {
        CreatePlayer();
        CreateObstacles();
        CreateEmptyNodes();
        CreateGoal();
    }

    private void CreateEmptyNodes()
    {
        for(int i = 0; i < 30; i++)
        {
            for(int j = 0; j < 30; j++)
            {
                if (grid[i, j] == null)
                {
                    GameObject newNode = Instantiate(emptyNode, new Vector3(i, -0.5f, j) + gridOffset, Quaternion.identity);
                    grid[i, j] = newNode;
                    newNode.GetComponent<Node>().SetCoordinates(i, j);
                }
            }
        }
    }

    private void CreateGoal()
    {
        int xPos, zPos;
        do
        {
            xPos = UnityEngine.Random.Range(0, 30);
            zPos = UnityEngine.Random.Range(0, 30);
        }
        while (grid[xPos, zPos] != null && grid[xPos, zPos].tag != "empty");

        curGoal = Instantiate(goal, new Vector3(xPos, 0, zPos) + gridOffset, Quaternion.identity);
        grid[xPos, zPos] = curGoal;
        goalX = xPos;
        goalZ = zPos;

        foreach(GameObject g in grid)
        {
            if(g.CompareTag("empty"))
            {
                g.GetComponent<Node>().SetGoal(curGoal);
            }
        }
        List<GameObject> newPath = GetPath();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().SetPath(newPath);

    }
    private void CreateObstacles()
    {
        int numObstacles = 100;

        for(int i = 0; i < numObstacles; i++)
        {
            int xPos = UnityEngine.Random.Range(0, 30);
            int zPos = UnityEngine.Random.Range(0, 30);
            if (grid[xPos, zPos] == null)
            {
                grid[xPos, zPos] = Instantiate(obstacle, new Vector3(xPos, 0, zPos) + gridOffset, Quaternion.identity);
            }
            else i--;
        }
    }

    private void CreatePlayer()
    {
        grid[0, 0] = Instantiate(player, new Vector3(0,0,0) + gridOffset, Quaternion.identity);
    }

    public static bool isAvailable(int x, int z)
    {
        if (x >= 30 || z >= 30) return false;
        else if (grid[x, z].tag != "empty") return false;
        else return true;
    }
    public static List<GameObject> GetNeighbours(int x, int z)
    {
        List<GameObject> neighbours = new List<GameObject>();
        if (x < 29)
        {
            GameObject right = grid[x + 1, z];
            if (right != null)
                if (right.tag == "empty" || right.tag == "goal") neighbours.Add(right);
        }
        if (x > 0)
        {
            GameObject left = grid[x - 1, z];
            if (left != null)
                if (left.tag == "empty" || left.tag == "goal") neighbours.Add(left);
        }
        if (z < 29)
        {
            GameObject up = grid[x, z + 1];
            if (up != null)
                if (up.tag == "empty" || up.tag == "goal") neighbours.Add(up);
        }
        if (z > 0)
        {
            GameObject down = grid[x, z - 1];
            if (down != null)
                if (down.tag == "empty" || down.tag == "goal") neighbours.Add(down);
        }
        return neighbours;
    }
    public static void MoveTo(GameObject node, GameObject plyr)
    { 
        Node info = node.GetComponent<Node>();
        grid[playerX, playerZ] = null;
        int x = info.x;
        int z = info.z;
        Debug.Log("XPOS: " + x);
        Debug.Log("ZPOS: " + z);
        grid[x, z] = plyr;
        playerX = x;
        playerZ = z;

        
    }
    public static void MoveToGoal(GameObject plyr)
    {
        playerX = goalX;
        playerZ = goalZ;
    }

    public void SetNewGoal(GameObject newGoal)
    {
        grid[goalX, goalZ] = null;
        curGoal = newGoal;
        int xPos = (int)(newGoal.transform.position.x - gridOffset.x);
        int zPos = (int)(newGoal.transform.position.z - gridOffset.z);

        grid[xPos, zPos] = curGoal;
        curGoal.transform.position = new Vector3 (xPos, 0, zPos) + gridOffset;
        foreach(GameObject g in curPath)
        {
            if(g.CompareTag("empty")) g.GetComponent<Renderer>().enabled = false;
        }
        CreateEmptyNodes();
        List<GameObject> newPath = GetPath();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().SetPath(newPath);
    }

    public void CreateNewGoal()
    {
        grid[goalX, goalZ] = Instantiate(emptyNode, new Vector3(goalX, -0.5f, goalZ) + gridOffset, Quaternion.identity);
        CreateEmptyNodes();
        CreateGoal();
   
    }

    public static List<GameObject> GetPath()
    {
        List<GameObject> path = new List<GameObject>();
        List<GameObject> neighbours = GetNeighbours(playerX, playerZ);
        curPath = path;
        while (true)
        {
            float bestHeuristic = float.MaxValue;
            GameObject bestNeighbour = null;
            foreach (GameObject g in neighbours)
            {
                if (path.Contains(g)) continue;
                if (neighbours.Count == 0) return null;
              
                if(g == curGoal)
                {
                    path.Add(g);
                    return path;
                }
                if (g.GetComponent<Node>().GetHeuristic() < bestHeuristic)
                {
                    bestNeighbour = g;
                    bestHeuristic = g.GetComponent<Node>().GetHeuristic();
                }
            }
            int xpos = bestNeighbour.GetComponent<Node>().x;
            int zpos = bestNeighbour.GetComponent<Node>().z;
            bestNeighbour.GetComponent<MeshRenderer>().enabled = true;
            neighbours = GetNeighbours(xpos, zpos);
            path.Add(bestNeighbour);
        }

    }
}
