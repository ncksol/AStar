using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    class Program
    {
        static void Main(string[] args)
        {
            var endGoalFound = false;

            var costs = new[]
            {
                //1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1
                0, 0, 1, 0, 1, 1, 1, 0, 1
            };

            var map = new Map(3, 3, costs);
            
            var start = new Node(2, 0);
            var end = new Node(0, 2);

            var cameFrom = new Dictionary<Node, Node>();
            cameFrom.Add(start, null);

            var costSoFar = new Dictionary<Node, int>();
            costSoFar.Add(start, 0);

            var frontier = new List<Node>();
            frontier.Add(start);
            
            while (frontier.Count > 0)
            {
                var current = frontier.OrderBy(x => x.Priority).First();
                frontier.Remove(current);

                if (Equals(current, end))
                {
                    endGoalFound = true;
                    break;
                }

                var neighbours = map.GetNeighbours(current);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + map.Cost(current, neighbour);
                    if (costSoFar.ContainsKey(neighbour) == false || costSoFar[neighbour] > newCost)
                    {
                        costSoFar[neighbour] = newCost;
                        neighbour.Priority = newCost + EstimateDistance(end, neighbour);
                        frontier.Add(neighbour);
                        cameFrom[neighbour] = current;
                    }
                }
            }

            if(endGoalFound == false)
            {
                Console.WriteLine("No path to end");
                return;
            }

            var path = new List<Node>();
            var pathNode = end;
            while (!Equals(pathNode, start))
            {
                path.Add(pathNode);
                pathNode = cameFrom[pathNode];
            }

            path.Reverse();

            foreach (var node in path)
            {
                Console.WriteLine($"{node.X}, {node.Y}");

                //Console.WriteLine($"{map.Nodes.IndexOf(node)}");
            }

            Console.ReadKey();
        }

        private static int EstimateDistance(Node nodeA, Node nodeB)
        {
            return Math.Abs(nodeA.X - nodeB.X) + Math.Abs(nodeA.Y - nodeB.Y);
        }
    }

    class Map
    {
        internal Map(int width, int height, int[] costs)
        {
            Width = width;
            Height = height;
            Nodes = new List<Node>();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Nodes.Add(new Node(j, i));
                }
            }

            for (int i = 0; i < costs.Length; i++)
            {
                Nodes[i].Cost = costs[i];
            }
        }

        internal int Width { get; set; }
        internal int Height { get; set; }
        internal List<Node> Nodes { get; set; }

        internal List<Node> GetNeighbours(Node node)
        {
            var nn = Nodes.FirstOrDefault(neighbour => neighbour.X == node.X && neighbour.Y == node.Y - 1 && node.Y - 1 >= 0);
            var en = Nodes.FirstOrDefault(neighbour => neighbour.X == node.X + 1 && neighbour.Y == node.Y && node.X + 1 < Width);
            var sn = Nodes.FirstOrDefault(neighbour => neighbour.X == node.X && neighbour.Y == node.Y + 1 && node.Y + 1 < Height);
            var wn = Nodes.FirstOrDefault(neighbour => neighbour.X == node.X - 1 && neighbour.Y == node.Y && node.X - 1 >= 0);

            var neighbours = new List<Node>();

            if(nn != null) neighbours.Add(nn);
            if(en != null) neighbours.Add(en);
            if(sn != null) neighbours.Add(sn);
            if(wn != null) neighbours.Add(wn);

            return neighbours;
        }

        internal int Cost(Node nodeA, Node nodeB)
        {
            //return Math.Abs(nodeA.Cost - nodeB.Cost);
            return nodeB.Cost == 0 ? Int16.MaxValue : 1;
        }        
    }

    class Node
    {
        public Node(int x, int y, int priority = 0, int cost = 0)
        {
            X = x;
            Y = y;
            Priority = priority;
            Cost = cost;
        }

        public int X { get; }
        public int Y { get; }
        public int Priority { get; set; }
        public int Cost { get; set; }

        public override bool Equals(object obj)
        {
            return Equals((Node) obj);
        }

        protected bool Equals(Node other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}
