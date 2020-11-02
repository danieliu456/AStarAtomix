using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class MapHelper
    {
        private readonly List<string> Map;
        private readonly List<string> Answers;

        public MapHelper(List<string> map, List<string> answers)
        {
            Map = map;
            Answers = answers;
        }

        public List<string> convertToEmptyMap()
        {
            List<string> map = new List<string>();
            foreach (var row in Map)
            {
                string levelRow = row;
                foreach (var answer in Answers)
                {
                    foreach(var atom in answer)
                    {
                        if (row.Contains(atom))
                        {
                            levelRow = levelRow.Replace(atom, '.');
                        }
                    }
                }
                map.Add(levelRow);
            }

            return map;
        }
    }
}
