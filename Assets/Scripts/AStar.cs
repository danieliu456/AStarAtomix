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
         
        public void calculateAStar()
        {
            startPosition.calculateMovePriority(0, endPosition);

            Debug.Log(startPosition.ToString(Map));

            openList.Enqueue(startPosition);

            while(openList.Length > 0)
            {
                var currPositions = openList.Dequeue();
                closeList.Add(currPositions);

                if (currPositions.compare(endPosition))
                {
                    Console.WriteLine("We solved that :)))");
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
                        
                        if(newG < nextPositions.G || !openList.Exists(nextPositions))
                        {
                            nextPositions.parentPositions = currPositions;
                            nextPositions.calculateMovePriority(newG, endPosition);

                            if (!openList.Exists(nextPositions))
                            {
                                openList.Enqueue(nextPositions);
                            }
                        }
                    }

                }

            }
        }

        public List<Positions> getOtherPositions(Positions currentPosition)
        {
            var otherPosiblePositions = new List<Positions>();
            var mapWithAtoms = currentPosition.ToList(Map);
            Debug.Log($"Expanding state \n {currentPosition.ToString(Map)}");

            foreach (var (atom, i) in currentPosition.Atoms.WithIndex())
            {
                Vector2 startPos = new Vector2(atom.X, atom.Y);
                Debug.Log($"start pos {startPos}");
                foreach (var direction in "UDLR")
                {
                    var directionVector = MoveFactory.getMovementCoordinates(direction);
                    Debug.Log($"Direction {direction}, Vector : {directionVector}");
                    var newAtom = atom.Move(mapWithAtoms, directionVector);
                    var atoms = (Node[])currentPosition.Atoms.Clone();
                    Debug.Log($"Atoms Clone Length: {atoms.Length} , i {i}");
                    atoms[i] = newAtom;

                    Positions position = new Positions(atoms);
                    Debug.Log($"New state \n {position.ToString(Map)}");
                    otherPosiblePositions.Add(position);
                    
                }
            }

            return otherPosiblePositions;
        }



    }
}
