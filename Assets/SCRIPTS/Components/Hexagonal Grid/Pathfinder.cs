using PlasticGui.WorkspaceWindow.Replication;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGG.Components.HexagonalGrid
{
    public class Pathfinder : MonoBehaviour
    {
        /// <summary>
        /// A* PathFinding
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destinatio"></param>
        /// <returns></returns>
        public static List<HexTile> FindPath(HexTile origin, HexTile destination)
        {
            Dictionary<HexTile, Node> nodesNotEvaluated = new Dictionary<HexTile, Node>();
            Dictionary<HexTile, Node> nodesAlreadyEvaluated = new Dictionary<HexTile, Node>();

            Node startNode = new Node(origin, origin, destination, 0);
            nodesNotEvaluated.Add(origin, startNode);

            bool gotPath = EvaluateNextNode(nodesNotEvaluated, nodesAlreadyEvaluated, origin, destination,
                out List<HexTile> path);

            int iter = 0;
            while (!gotPath && iter < 1000)
            {
                iter++;
                gotPath = EvaluateNextNode(nodesNotEvaluated, nodesAlreadyEvaluated, origin, destination, out path);
            }

            return path;
        }

        public static HexTile GetWalkableRandomNeighbour(HexTile origin)
        {
            int rand = UnityEngine.Random.Range(0, origin.neighbours.Count());
            HexTile tile = origin.neighbours[rand];
            if (tile.tileType == TileType.Mountain)
            {
                tile = GetWalkableRandomNeighbour(origin);
            }
            return tile;
        }

        private static bool EvaluateNextNode(Dictionary<HexTile, Node> nodesNotEvaluated,
            Dictionary<HexTile, Node> nodesAlreadyEvaluated, HexTile origin, HexTile destination,
            out List<HexTile> path)
        {
            Node currentNode = GetCheapestNode(nodesNotEvaluated.Values.ToArray());

            if (currentNode == null)
            {
                path = new List<HexTile>();
                return false;
            }

            nodesNotEvaluated.Remove(currentNode.target);
            nodesAlreadyEvaluated.Add(currentNode.target, currentNode);

            path = new List<HexTile>();

            // If this is our destination then we're done
            if (currentNode.target == destination)
            {
                path.Add(currentNode.target);
                // Build out our path...
                while (currentNode.target != origin)
                {
                    path.Add(currentNode.parent.target);
                    currentNode = currentNode.parent;
                }

                return true;
            }

            // Otherwise, add out neighbours to the list and try to traverse them
            List<Node> neighbours = new List<Node>();
            foreach (HexTile tile in currentNode.target.neighbours)
            {
                Node node = new Node(tile, origin, destination, currentNode.GetCost());

                //If the tile type isn't something we can traverse, then make the cost really high
                if (tile.tileType == TileType.Mountain || tile.selectable == false)
                {
                    node.baseCost = 999999999;
                    // continue
                }

                neighbours.Add(node);
            }

            foreach (Node neighbour in neighbours)
            {
                // if the tile has already been evaluated fully we can ignore it
                if (nodesAlreadyEvaluated.Keys.Contains(neighbour.target))
                {
                    continue;
                }

                // If the cost is lower, or if the tile isn't in the not evaluated dic...
                if (neighbour.GetCost() < currentNode.GetCost() || !nodesNotEvaluated.Keys.Contains(neighbour.target))
                {
                    neighbour.SetParent(currentNode);
                    if (!nodesNotEvaluated.Keys.Contains(neighbour.target))
                    {
                        nodesNotEvaluated.Add(neighbour.target, neighbour);
                    }
                }
            }

            return false;
        }

        private static Node GetCheapestNode(Node[] nodesNotEvaluated)
        {
            if (nodesNotEvaluated.Length == 0)
            {
                return null;
            }

            Node selectedNode = nodesNotEvaluated[0];

            for (int i = 1; i < nodesNotEvaluated.Length; i++)
            {
                var currentNode = nodesNotEvaluated[i];
                if (currentNode.GetCost() < selectedNode.GetCost())
                {
                    selectedNode = currentNode;
                }
                else if (currentNode.GetCost().Equals(selectedNode.GetCost()) &&
                         currentNode.costToDestination < selectedNode.costToDestination)
                {
                    selectedNode = currentNode;
                }
            }

            return selectedNode;
        }
    }
}