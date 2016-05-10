using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace MinedOut
{
    internal class Pathfinder
    {
        private readonly AiPlayer plr;
        private readonly Minefield field;
        private DrawableTile cachedPathDest;
        private List<DijkstraVertex> cachedPath;

        public Pathfinder(AiPlayer plr, Minefield field)
        {
            this.plr = plr;
            this.field = field;
        }

        private List<DijkstraVertex> Dijkstra(int targetX, int targetY)
        {
            //make target vertex
            DijkstraVertex target = new DijkstraVertex(targetX, targetY);

            //get all dugged tiles (target included) and call it the "graph"
            HashSet<DijkstraVertex> allDug = new HashSet<DijkstraVertex>(PositionsOfAllDuggedTiles().Select(t => new DijkstraVertex(t.X, t.Y)));
            allDug.UnionWith(new HashSet<DijkstraVertex> {target});
            List<DijkstraVertex> graphAsList = allDug.ToList();
            List<DijkstraVertex> graph = new List<DijkstraVertex>(graphAsList);

            //find the source vertex (making the assuming it's dugged)
            DijkstraVertex source = graph.First(v => v.X == plr.X && v.Y == plr.Y);

            //dijkstra implementation
            foreach (DijkstraVertex v in graph)
            {
                v.Dist = 99999;
                v.Previous = null;
            }
            source.Dist = 0;
            var Q = new List<DijkstraVertex>(graph);
            while (Q.Count != 0)
            {
                DijkstraVertex u = Q.OrderBy(t => t.Dist).First();
                Q.Remove(u);
                IEnumerable<DijkstraVertex> vs = GetNeighbors(graph, u);
                foreach (DijkstraVertex v in vs)
                {
                    int alt = u.Dist + u.DistanceTo(v);
                    if (alt < v.Dist)
                    {
                        v.Dist = alt;
                        v.Previous = u;
                    }
                }
            }

            //traverse from the target to the source
            var path = target.GetPathFromSource();
            return path;
        }

        private IEnumerable<DijkstraVertex> GetNeighbors(IEnumerable<DijkstraVertex> graph, DijkstraVertex dijkstraVertex)
        {
            return graph.Where(t => t.DistanceTo(dijkstraVertex) == 1);
        }

        private IEnumerable<Vector2i> PositionsOfAllDuggedTiles()
        {
            var allDugTiles = field.GetAllTiles().Where(t => t.Dug);
            var positions = allDugTiles.Select(t => new Vector2i(t.X, t.Y));
            return positions;
        }

        public Vector2i NextStepInMovingTowards(DrawableTile exploreHere)
        {
            //you're sitting on top of it
            if (exploreHere.X == plr.X && exploreHere.Y == plr.Y)
            {
                return new Vector2i(exploreHere.X, exploreHere.Y);
            }
            if (exploreHere != cachedPathDest)
            {
                //need to regen path
                cachedPath = Dijkstra(exploreHere.X, exploreHere.Y);
                cachedPathDest = exploreHere;
            }
            //now that we've cached the path, pop the first step and return it
            var firstVertex = cachedPath.First();
            cachedPath.Remove(firstVertex);
            return firstVertex.ToVector2i();
        }

        public void Draw(DrawCommandCollection drawCmds)
        {
            if (cachedPath != null)
            {
                var str = string.Join(", ", cachedPath);
                drawCmds.AddRange(DrawCommand.FromString(0, 0, str, Color.Yellow, Color.Black));
            }
        }
    }

    internal class DijkstraVertex : Tile
    {
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
