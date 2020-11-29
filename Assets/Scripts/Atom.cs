using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;

public class Atom : MonoBehaviour
{
    public GameManager GameManager;
    public int uniqueId;
    public int X;
    public int Y;

    public void setCoordinates(int x = 0, int y = 0, bool current = false)
    {
        if (current)
        {
            var coords = GameManager.convertFromVectorToArrayIndex(transform.position);
            X = coords[0];
            Y = coords[1];
        }
        else
        {
            X = x;
            Y = y;
        }
    }

    public override string ToString()
    {
       return name;
    }

    public override bool Equals(object obj)
    {
        Atom inT = (Atom)obj;

        if (obj == null && inT == null)
        {
            return false;
        }

        return X == inT.X && Y == inT.Y;
    }

    public int distancebetweenAtoms(Atom atom) => Math.Abs(atom.X - this.X) + Math.Abs(atom.Y - this.Y);

    public override int GetHashCode()
    {
        return Tuple.Create(X, Y).GetHashCode();
    }
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

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public int uniqueId { get; set; }
    public string Name { get; set; }

    public Node(Vector2 vector, string name, int uniqueId)
    {
        X = (int)vector.x;
        Y = (int)vector.y;
        this.uniqueId = uniqueId;
        Name = name;
    }

    public Node Move(List<string> map, Vector2 direction)
    {
        var temp = new string[map.Count];
        Vector2 vector = new Vector2(X, Y);

        while (!isBlocked(map, direction, vector))
        {
            vector += direction;
        }

        return new Node(vector, Name, uniqueId);
    }
    private bool isBlocked(List<string> map, Vector2 direction, Vector2 nodeCoord)
    {
        Vector2 tempPos = nodeCoord + direction;
        return map[(int)tempPos.y][(int)tempPos.x] != '.';
    }

    public int distancebetweenAtoms(Atom atom) => Math.Abs(atom.X - this.X) + Math.Abs(atom.Y - this.Y);

    public override int GetHashCode()
    {
        return Tuple.Create(X, Y, uniqueId).GetHashCode();
    }


    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object obj)
    {
        Node inT = (Node)obj;

        if (obj == null && inT == null)
        {
            return false;
        }

        return X == inT.X && Y == inT.Y;
    }
}

public static class MoveFactory{

    public static Vector2 getMovementCoordinates(this char position) {
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