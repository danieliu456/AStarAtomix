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

        public int F { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public Node[] Atoms { get; set; }

        public Positions parentPositions { get; set; }

        public int heuristic(Positions goalPosition)
        {
            int h = 0;
            Node firstAtom = Atoms[0];
            Node firstGoalAtom = goalPosition.Atoms[0];

            for (int i = 1; i < Atoms.Length; i++)
            {
                var distanceX = Atoms[i].X - firstAtom.X;
                var distanceY = Atoms[i].Y - firstAtom.Y;

                var distanceGoalX = goalPosition.Atoms[i].X - firstGoalAtom.X;
                var distanceGoalY = goalPosition.Atoms[i].Y - firstGoalAtom.Y;

                //var distance = Math.Abs(firstAtom.X - Atoms[i].X) + Math.Abs(firstAtom.Y - Atoms[i].Y);
                //var distanceGoal = Math.Abs(firstGoalAtom.X - goalPosition.Atoms[i].X) + Math.Abs(firstGoalAtom.Y - goalPosition.Atoms[i].Y);
                h += Math.Abs(distanceGoalX - distanceX) + Math.Abs(distanceGoalY - distanceY);
                //h += Math.Abs(distance - distanceGoal);
            }

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
                    temp[atom.Y] = temp[atom.Y].Remove(atom.X, 1).Insert(atom.X, atom.Name);
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

            stringBuilder.AppendLine($"F: {F}  G: {G}  H: {H}");

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
                        temp[atom.Y] = temp[atom.Y].Remove(atom.X, 1).Insert(atom.X, atom.Name);
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



        public void calculateMovePriority(int g, Positions end)
        {
            H = heuristic(end);
            F = g + H;
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
