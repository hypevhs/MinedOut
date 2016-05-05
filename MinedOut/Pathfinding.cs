using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace MinedOut
{
    internal class DijkstraVertex
    {
        public override string ToString()
        {
            return $"DijkVtx: {X},{Y}";
        }

        public int X { get; }
        public int Y { get; }
        public DijkstraVertex Previous { get; set; }
        public int Dist { get; set; }

        public DijkstraVertex(int x, int y)
        {
            X = x;
            Y = y;
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

        public int DistanceTo(DijkstraVertex dijkstraVertex)
        {
            //taxicab distance
            return
                Math.Abs(X - dijkstraVertex.X) +
                Math.Abs(Y - dijkstraVertex.Y);
        }
            
        public Vector2i ToVector2i()
        {
            return new Vector2i(X, Y);
        }
    }
}
