using GGG.Components.Buildings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // This class exist as a utility class for the pathfinding algorithm

    public Node parent;
    public HexTile target;
    public HexTile destination;
    public HexTile origin;

    public int baseCost;
    public int costFromOrigin;
    public int costToDestination;
    public int pathCost;
    
    public Node(HexTile current, HexTile origin, HexTile destination, int pathCost)
    {
        parent = null;
        this.target = current;
        this.origin = origin;
        this.destination = destination;

        baseCost = 1;
        costFromOrigin = (int)Vector3Int.Distance(current.cubeCoordinate, origin.cubeCoordinate);
        costToDestination = (int)Vector3Int.Distance(current.cubeCoordinate, destination.cubeCoordinate);
        this.pathCost = pathCost;
    }

    /// <summary>
    /// Function that returns the sum of all the costs
    /// </summary>
    public int GetCost()
    {
        return pathCost + baseCost + costFromOrigin + costToDestination;
    }

    /// <summary>
    /// Allows to set another node as this node parents. This enables the capacity to go back on our path
    /// </summary>
    /// <param name="node"></param>
    public void SetParent(Node node)
    {
        this.parent = node;
    }
}
