using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Atom : MonoBehaviour
{
    public GameManager GameManager;

    public void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnMouseUp()
    {
        if (tag == "answer") 
        {
            Debug.LogWarning("Answer click");
        }
        else
        {
            Debug.Log("Changing Atom Reference");
            GameManager.setMainAtom(this);
        }
    }

    public void Move(Vector2 direction)
    {
        // Neleidzia vaikscioti istrizai
        _ = Mathf.Abs(direction.x) < 0.5 ? direction.x = 0 : direction.y = 0;
        // Duoda eiti tik 1 langeli (suapvalina float reiksmes i sveiku vienetus)
        direction.Normalize();

        while (!isBlocked(transform.position, direction))
        {
            transform.Translate(direction);
        }
    }

    private bool isBlocked(Vector3 currenPosition, Vector2 direction)
    {
        Vector2 newPosition = new Vector2(currenPosition.x, currenPosition.y) + direction;
        GameObject[] borders = GameObject.FindGameObjectsWithTag("border");
        GameObject[] atoms = GameObject.FindGameObjectsWithTag("Atom");

        List<GameObject> obstacles = new List<GameObject>(borders);
        obstacles.AddRange(atoms);

        foreach (var obstacle in obstacles)
        {
            if (obstacle.transform.position.y == newPosition.y && obstacle.transform.position.x == newPosition.x) return true;

        }

        return false;
    }
}

public static class MoveFactory{

    public static Vector2 getMovementCordinates(this char position) {
        switch (position)
        {
            case 'U':
                return new Vector2(0, 1);
            case 'D':
                return new Vector2(0, -1);
            case 'L':
                return new Vector2(-1, 0);
            case 'R':
                return new Vector2(1, 0);
            default:
                throw new Exception($"I Dont know what to do with this movement {position}");
        }
    }
}