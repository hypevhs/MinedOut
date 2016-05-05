using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace MinedOut
{
    internal class DijkstraVertex : Tile
    {
        public override void Draw(DrawCommandCollection drawCmds)
        {
            throw new NotImplementedException();
        }
        
        public DijkstraVertex Previous { get; set; }
        public int Dist { get; set; }

        public DijkstraVertex(int x, int y) : base(x, y)
        {
        }

        public List<DijkstraVertex> GetPathFromSource()
        {
            if (Previous == null) //basecase
            {
                //don't include myself in the path
                return new List<DijkstraVertex>();
            }
            //get the parent path, then add myself
            var pathSoFar = Previous.GetPathFromSource();
            pathSoFar.Add(this);
            return pathSoFar;
        }

        public override string ToString()
        {
            return $"DijkVtx: {X},{Y}";
        }
    }
}
