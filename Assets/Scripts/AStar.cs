﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class AStar
    {
        Positions startPosition { get; set; }
        Positions endPosition { get; set; }
        PositionsPriorityQueue openList = new PositionsPriorityQueue();
        //HashSet<Positions> closeList = new HashSet<Positions>();
        //Hashtable closeList = new Hashtable();
        Dictionary<int, List<Positions>> closeList = new Dictionary<int, List<Positions>>();
        public List<string> Map = new List<string>();
        private readonly bool log;
        StreamWriter outputFile = File.AppendText("LevelCompletion.txt");

        public List<Positions> Positions { get; set; }

        public AStar(Positions start, Positions end, List<string> map, bool log)
        {
            Map = map;
            this.log = log;
            startPosition = start;
            endPosition = end;
            //calculateAStar(start, end);
        }
         
        public List<Positions> calculateAStar()
        {
            int tempI = 0;
            startPosition.calculateMovePriority(0, endPosition, Map);

            Debug.Log(startPosition.ToString(Map));

            openList.Enqueue(startPosition);

            while(openList.Length > 0)
            {
                tempI++;
                if (tempI > 3000) {
                    Debug.Log($"Could not found solution in {tempI} tries");
                    outputFile.Flush();
                    outputFile.Dispose();
                    return null;
                 }
                var currPositions = openList.Dequeue();

                if (closeList.ContainsKey(currPositions.GetHashCode()))
                {
                    closeList[currPositions.GetHashCode()].Add(currPositions);
                }
                else
                {
                    var list = new List<Positions>();
                    list.Add(currPositions);
                    closeList.Add(currPositions.GetHashCode(), list);
                }

                if(log)
                {
                        outputFile.WriteLine("------------MinPriority------------");
                        string positionsInMap = currPositions.ToString(Map);
                        outputFile.WriteLine(positionsInMap);
                }


                if (currPositions.compare(endPosition))
                {
                    Debug.Log("We solved that :)))");
                    outputFile.Flush();
                    outputFile.Dispose();
                    return calculatePath(currPositions, endPosition);
                }
                var otherPositions = getOtherPositions(currPositions);

                foreach (var (neighbour, index) in otherPositions.WithIndex())
                {
                    var neighbourCostG = currPositions.G + 1;

                    var neighbourInOpen = openList.Exists(neighbour);
                    var neighbourInClose = closeList.TryGetValue(neighbour.GetHashCode(), out var closedHashedList) ? closedHashedList.Find(e => e.Equals(neighbour)) : null;

                    neighbour.calculateMovePriority(neighbourCostG, endPosition, Map);
                    neighbour.parentPositions = currPositions;

                    if (log)
                    {
                        outputFile.WriteLine("------------Expanding------------");
                        string positionsInMap = neighbour.ToString(Map);
                        outputFile.WriteLine(positionsInMap);

                    }

                    if (neighbourInOpen != null)
                    {
                        if (neighbourCostG < neighbourInOpen.G  )
                        {
                            openList.UpdatePriority(neighbour);
                        }
                    } 
                    else
                    {
                        if (neighbourInClose != null)
                        {
                            if (neighbourCostG < neighbourInClose.G)
                            {
                                closeList[neighbour.GetHashCode()].Remove(neighbour);
                                openList.Enqueue(neighbour);
                            }
                        }
                        else
                        {
                            openList.Enqueue(neighbour);
                        }
                    }
                }

            }
            Debug.Log($"Could not found solution Open List Empty");
            outputFile.Flush();
            outputFile.Dispose();
            return null;
        }

        public List<Positions> getOtherPositions(Positions currentPosition)
        {
            var otherPosiblePositions = new List<Positions>();
            var mapWithAtoms = currentPosition.ToList(Map);
            //Debug.Log($"Expanding state \n {currentPosition.ToString(Map)}");



            foreach (var (atom, i) in currentPosition.Atoms.WithIndex())
            {
                Vector2 startPos = new Vector2(atom.X, atom.Y);
                //Debug.Log($"start pos {startPos}");
                foreach (var direction in "UDLR")
                {
                    var directionVector = MoveFactory.getMovementCoordinates(direction);
                    //Debug.Log($"Direction {direction}, Vector : {directionVector}");
                    var newAtom = atom.Move(mapWithAtoms, directionVector);
                    var atoms = (Node[])currentPosition.Atoms.Clone();
                    //Debug.Log($"Atoms Clone Length: {atoms.Length} , i {i}");
                    atoms[i] = newAtom;
                    Positions position = new Positions(atoms);

                    directionVector = new Vector2(directionVector.x, directionVector.y * - 1);
                    position.G = currentPosition.G + 1;
                    position.RoundMove = directionVector;
                    position.MovedNodeuniqueId = atom.uniqueId;
                    position.MovedAtomName = atom.Name;


                    if (currentPosition.Equals(position)) continue;

                    otherPosiblePositions.Add(position);
                    
                }
            }

            return otherPosiblePositions;
        }

        static List<Positions> calculatePath(Positions current, Positions goal)
        {
            var states = new List<Positions>();
            

            if (current.compare(goal))
            {
                while (current != null)
                {
                    states.Add(current);
                    current = current.parentPositions;
                }

                if (states.Count > 0)
                {
                    states.Reverse();
                }
            }

            return states;
        }



    }
}
