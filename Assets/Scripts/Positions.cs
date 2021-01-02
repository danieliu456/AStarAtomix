using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public class Positions
    {
        public Positions(Node[] atoms)
        {
            Atoms = atoms.OrderBy(atom => atom.Name).ToArray();

        }
        public int MovedNodeuniqueId { get; set; }
        public string MovedAtomName { get; set; }
        public int deep { get; set; } = 0;
        public Vector2 RoundMove { get; set; }
        public int F { get; set; }
        public int G { get; set; }
        public int M { get; set; }




        public int H { get; set; }
        public int Side { get; set; } = 0;
        public int Fit { get; set; } = 0;
        public Node[] Atoms { get; set; }

        public Positions parentPositions { get; set; }

        public int heuristic(Positions goalPosition, List<string> map)
        {
            var mid = Atoms.Length / 2;
            
            int h = 0;
            Node firstAtom = Atoms[0];
            Node firstGoalAtom = goalPosition.Atoms[0];

            //Manheteno atstumas
            for (int i = 1; i < Atoms.Length; i++)
            {
                //if (i == mid) continue;

                var distanceX = Atoms[i].X - firstAtom.X;
                var distanceY = Atoms[i].Y - firstAtom.Y;

                var distanceGoalXx = goalPosition.Atoms[i].X - firstGoalAtom.X;
                var distanceGoalYy = goalPosition.Atoms[i].Y - firstGoalAtom.Y;

                h += Math.Abs(distanceGoalXx - distanceX) + Math.Abs(distanceGoalYy - distanceY);
            }

            //for (int i = 1; i < Atoms.Length; i++)
            //{
            //    int gx, gy;

            //    var distanceGoalX = goalPosition.Atoms[i].X - firstGoalAtom.X;
            //    var distanceGoalY = firstGoalAtom.Y - goalPosition.Atoms[i].Y;

            //    gx = firstAtom.X + distanceGoalX;
            //    gy = firstAtom.Y + distanceGoalY;
            //    //var mapWithAtoms = this.ToList(map);

            //    Vector2 goalVector = new Vector2();
            //    goalVector.x = gx;
            //    goalVector.y = gy;
            //    var (manhattan, deep) = bfs(Atoms, i, goalVector, map);
            //    h += manhattan;
            //    M += deep;
            //}


            // Atoms side 
            Node MidAtom = Atoms[mid];
            Node MidGoalAtom = goalPosition.Atoms[mid];
            for (int i = 0; i < Atoms.Length; i++)
            {
                if (i == mid) continue;

                var distanceX = MidAtom.X - Atoms[i].X;
                var distanceY = MidAtom.Y - Atoms[i].Y;

                var distanceGoalX = MidGoalAtom.X - goalPosition.Atoms[i].X;
                var distanceGoalY = MidGoalAtom.Y - goalPosition.Atoms[i].Y;

                if (distanceGoalY == 0 && distanceX * distanceGoalX <= 0)
                {
                    Side += 3;
                }
                else if (distanceGoalX == 0 && distanceY * distanceGoalY <= 0)
                {
                    Side += 3;
                }
            }

            bool fit = true;
            for (int i = 0; i < Atoms.Length; i++)
            {
                var currAtom = Atoms[i];
                for (int j = 0; j < goalPosition.Atoms.Length; j++)
                {
                    if (i == j ) continue;
                    var currAtomInGoal = goalPosition.Atoms[i];
                    var diffX = goalPosition.Atoms[j].X - currAtomInGoal.X;
                    var diffY = currAtomInGoal.Y - goalPosition.Atoms[j].Y;
                    try
                    {
                        int y = currAtom.Y + diffY;
                        int x = currAtom.X + diffX;

                        if (y >= map.Count || y < 0)
                        {
                            Fit += 4;
                            fit = false;
                            break;
                        }

                        if (x < 0 || x >= map[y].Length)
                        {
                            Fit += 4;
                            fit = false;
                            break;
                        }

                        if (map[y][x] != '.')
                        {
                            Fit += 4;
                            //fit = false;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw;
                    }
                }

                //if (fit)
                //{
                //    break;
                //}
            }

            //if (!fit) Fit += (Atoms.Length - 1) * 3;

            return h;
        }

        public string ToString(List<string> map)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var temp = new string[map.Count];

            map.CopyTo(temp);
             
            foreach (var atom in Atoms)
            {
                try
                {
                    temp[atom.Y] = temp[atom.Y].Remove(atom.X, 1).Insert(atom.X, atom.Name[0].ToString());
                }
                catch (Exception e)
                {
                    Debug.LogError($"x {atom.X}, Y {atom.Y}");
                    Debug.LogError($"Length { temp.Length }");
                    Debug.LogError($"ROW LEngth: {temp[atom.Y].Length}");
                    throw e;
                }
            }

            foreach (var row in temp)
            {
                stringBuilder.AppendLine(row);
            }

            stringBuilder.AppendLine($"F: {F}  G: {G}  H: {H}  S:{Side}  Fit: {Fit} Moves: {M}");
            stringBuilder.AppendLine($"Moved Atom: {MovedAtomName}  MovedAtomId: {MovedNodeuniqueId}  Vector: {RoundMove}");

            return stringBuilder.ToString();
        }

        public List<string> ToList(List<string> map)
        {
            List<string> newMap;
            var temp = new string[map.Count];

            map.CopyTo(temp);


                foreach (var atom in Atoms)
                {
                    try
                    {
                        temp[atom.Y] = temp[atom.Y].Remove(atom.X, 1).Insert(atom.X, atom.Name[0].ToString());
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"x {atom.X}, Y {atom.Y}");
                        Debug.LogError($"Length { temp.Length }");
                        Debug.LogError($"ROW LEngth: {temp[atom.Y].Length}");
                        throw e;
                    }
                }



            newMap = temp.ToList();

            return newMap;
        }


        public (int, int) bfs(Node[] atoms, int atomToMove, Vector2 goalVector, List<string> map)
        {
            var startPosition = new Positions(atoms);
            var visited = new List<Positions>();
            var Queue = new Queue<Positions>();
            int manhattan = 100, deep = 3;

            Queue.Enqueue(startPosition);

            while(Queue.Count > 0)
            {
                var currPosition = Queue.Dequeue();
                var mapWithAtoms = currPosition.ToList(map);
                int temp = (int)(Math.Abs(currPosition.Atoms[atomToMove].X - goalVector.x) + Math.Abs(currPosition.Atoms[atomToMove].Y - goalVector.y));

                if (currPosition.deep > 1) return (manhattan, deep);

                if (temp < manhattan)
                {
                    manhattan = temp;
                    deep = currPosition.deep;
                }

                if (currPosition.Atoms[atomToMove].X == goalVector.x && currPosition.Atoms[atomToMove].Y == goalVector.y)
                {
                    return (manhattan, currPosition.deep);
                }

                foreach (var direction in "UDLR")
                {
                    var directionVector = MoveFactory.getMovementCoordinates(direction);

                    var newAtom = Atoms[atomToMove].Move(mapWithAtoms, directionVector);

                    var atomsClone = (Node[])currPosition.Atoms.Clone();

                    atomsClone[atomToMove] = newAtom;
                    Positions position = new Positions(atomsClone);

                    if (currPosition.Equals(position) || visited.Contains(position)) continue;
                    position.parentPositions = currPosition;
                    position.deep = currPosition.deep + 1;
                    Queue.Enqueue(position);
                }
            }

            return (manhattan, deep);
        }

        public void calculateMovePriority(int g, Positions end, List<string> map)
        {
            H = heuristic(end, map);
            F = g + H + Side + Fit + M;
            G = g;
        }

        public override int GetHashCode()
        {
            return Atoms.Sum((atom) => atom.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            Positions inT;

            if (obj != null && obj is Positions)
            {
                inT = (Positions)obj;
            }
            else
            {
                return false;
            }

            for (int i = 0; i < Atoms.Length; i++)
            {
                if (!Atoms[i].Equals(inT.Atoms[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool compare(Positions goal)
        {
            Node firstAtom = Atoms[0];
            Node firstGoalAtom = goal.Atoms[0];

            for (int i = 1; i < Atoms.Length; i++)
            {
                var distanceX = firstAtom.X - Atoms[i].X;
                var distanceY = firstAtom.Y - Atoms[i].Y;

                var distanceGoalX = firstGoalAtom.X - goal.Atoms[i].X;
                var distanceGoalY = firstGoalAtom.Y - goal.Atoms[i].Y;

                if (distanceX != distanceGoalX || distanceY != distanceGoalY)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
