using System;
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
        HashSet<Positions> closeList = new HashSet<Positions>();
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
            startPosition.calculateMovePriority(0, endPosition);

            Debug.Log(startPosition.ToString(Map));

            openList.Enqueue(startPosition);

            while(openList.Length > 0)
            {
                tempI++;
                if (tempI > 5000) {
                    Debug.Log($"Could not found solution in {tempI} tries");
                    outputFile.Dispose();
                    return null;
                 }
                var currPositions = openList.Dequeue();
                closeList.Add(currPositions);
                //Debug.Log($"CurrentPosition \n{currPositions.ToString(Map)}");

                if(log)
                {
                        outputFile.WriteLine("------------MinPriority------------");
                        string positionsInMap = currPositions.ToString(Map);
                        outputFile.WriteLine(positionsInMap);
                }


                if (currPositions.compare(endPosition))
                {
                    Debug.Log("We solved that :)))");
                    outputFile.Dispose();
                    return calculatePath(currPositions, endPosition);
                }
                var otherPositions = getOtherPositions(currPositions);

                foreach (var (neighbour, index) in otherPositions.WithIndex())
                {
                    var neighbourCostG = currPositions.G + 1;
                    var neighbourExistInOpen = openList.Exists(neighbour);
                    var neighbourExistsInCloseList = closeList.Contains(neighbour);


                    if (neighbourExistInOpen)
                    {
                        if (neighbour.G <= neighbourCostG)
                        {
                            continue;
                        }
                    } 
                    else if (neighbourExistsInCloseList) 
                    {
                        if (neighbour.G <= neighbourCostG)
                        {
                            continue;
                        }

                        closeList.Remove(neighbour);
                        neighbour.parentPositions = currPositions;
                        neighbour.calculateMovePriority(neighbourCostG, endPosition);
                        openList.Enqueue(neighbour);
                    }
                    else
                    {
                        neighbour.parentPositions = currPositions;
                        neighbour.calculateMovePriority(neighbourCostG, endPosition);
                        openList.Enqueue(neighbour);
                    }




                    //if (closeList.Contains(nextPositions))
                    //{
                    //    Debug.Log("Already have this state :)");
                    //}
                    //else
                    //{
                    //    var newG = currPositions.G + currPositions.heuristic(nextPositions);
                    //    var existsInQueue = openList.Exists(nextPositions);

                    //    if (newG < nextPositions.G || !existsInQueue)
                    //    {
                    //        nextPositions.parentPositions = currPositions;
                    //        nextPositions.calculateMovePriority(newG, endPosition);

                    //        if (!existsInQueue)
                    //        {
                    //            openList.Enqueue(nextPositions);
                    //        }
                    //    }
                    //}

                }

            }
            Debug.Log($"Could not found solution Open List Empty");
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
                    //Debug.Log($"New state \n {position.ToString(Map)}");

                    if (log)
                    {
                            outputFile.WriteLine("------------Expanding------------");
                            string positionsInMap = position.ToString(Map);
                            outputFile.WriteLine(positionsInMap);
                        
                    }

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
