using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passage_Pathing
{
    public static class Extensions
    {
        public static bool ContainsCave(this IEnumerable<Cave> caves, string name)
        {
            foreach (var cave in caves)
            {
                if (cave.Name == name)
                    return true;
            }

            return false;
        }

        public static Cave GetCave(this IEnumerable<Cave> caves, string name)
        {
            foreach (var cave in caves)
            {
                if (cave.Name == name)
                    return cave;
            }

            return null;
        }
    }
}
