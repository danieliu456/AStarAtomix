using System;
using System.Collections.Generic;
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
        HashSet<Positions> closeList = new HashSet<Positions>();
        public List<string> Map = new List<string>();

        public List<Positions> Positions { get; set; }

        public AStar(Positions start, Positions end, List<string> map)
        {
            Map = map;
            startPosition = start;
            endPosition = end;
            //calculateAStar(start, end);
        }
         
        public List<Positions> calculateAStar()
        {
            int tempI = 0;
            startPosition.calculateMovePriority(0, endPosition);

            Debug.Log(startPosition.ToString(Map));

            openList.Enqueue(startPosition);

            while(openList.Length > 0)
            {
                tempI++;
                if (tempI > 5000) return null;
                var currPositions = openList.Dequeue();
                closeList.Add(currPositions);
                //Debug.Log($"CurrentPosition \n{currPositions.ToString(Map)}");
                if (currPositions.compare(endPosition))
                {
                    Debug.Log("We solved that :)))");
                    return calculatePath(currPositions, endPosition);
                }
                var otherPositions = getOtherPositions(currPositions);
                Debug.Log("Successfully expanded state");

                foreach (var (nextPositions, index) in otherPositions.WithIndex())
                {
                    if (closeList.Contains(nextPositions))
                    {
                        Debug.Log("Already have this state :)");
                    }
                    else
                    {
                        var newG = currPositions.G + currPositions.heuristic(nextPositions);
                        var existsInQueue = openList.Exists(nextPositions);

                        if (newG < nextPositions.G || !existsInQueue)
                        {
                            nextPositions.parentPositions = currPositions;
                            nextPositions.calculateMovePriority(newG, endPosition);

                            if (!existsInQueue)
                            {
                                openList.Enqueue(nextPositions);
                            }
                        }
                    }

                }

            }
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

                    position.RoundMove = directionVector;
                    position.MovedNodeuniqueId = atom.uniqueId;
                    position.MovedAtomName = atom.Name;


                    if (currentPosition.Equals(position)) continue;
                    //Debug.Log($"New state \n {position.ToString(Map)}");
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
