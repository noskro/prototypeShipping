using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    /// <summary>
    /// Finds path from given start point to end point. Returns an empty list if the path couldn't be found.
    /// </summary>
    /// <param name="startPoint">Start PathfindingTileNode.</param>
    /// <param name="endPoint">Destination PathfindingTileNode.</param>
    public static List<PathfindingTileNode> FindPath(PathfindingTileNode startPoint, PathfindingTileNode endPoint)
    {
        List<PathfindingTileNode> openPathTiles = new List<PathfindingTileNode>();
        List<PathfindingTileNode> closedPathTiles = new List<PathfindingTileNode>();

        // Prepare the start PathfindingTileNode.
        PathfindingTileNode currentTile = startPoint;

        currentTile.G = 0;
        currentTile.H = GetEstimatedPathCost(startPoint.cubePosition, endPoint.cubePosition);

        // Add the start PathfindingTileNode to the open list.
        openPathTiles.Add(currentTile);

        while (openPathTiles.Count != 0)
        {
            // Sorting the open list to get the PathfindingTileNode with the lowest F.
            openPathTiles = openPathTiles.OrderBy(x => x.F).ThenByDescending(x => x.G).ToList();
            currentTile = openPathTiles[0];

            // Removing the current PathfindingTileNode from the open list and adding it to the closed list.
            openPathTiles.Remove(currentTile);
            closedPathTiles.Add(currentTile);

            int g = currentTile.G + 1;

            // If there is a target PathfindingTileNode in the closed list, we have found a path.
            if (closedPathTiles.Contains(endPoint))
            {
                break;
            }

            // Investigating each adjacent PathfindingTileNode of the current PathfindingTileNode.
            foreach (PathfindingTileNode adjacentTile in currentTile.adjacentTiles)
            {
                // Ignore not walkable adjacent tiles.
                if (!adjacentTile.canGoThere)
                {
                    continue;
                }

                // Ignore the PathfindingTileNode if it's already in the closed list.
                if (closedPathTiles.Contains(adjacentTile))
                {
                    continue;
                }

                // If it's not in the open list - add it and compute G and H.
                if (!(openPathTiles.Contains(adjacentTile)))
                {
                    adjacentTile.G = g;
                    adjacentTile.H = GetEstimatedPathCost(adjacentTile.cubePosition, endPoint.cubePosition);
                    openPathTiles.Add(adjacentTile);
                }
                // Otherwise check if using current G we can get a lower value of F, if so update it's value.
                else if (adjacentTile.F > g + adjacentTile.H)
                {
                    adjacentTile.G = g;
                }
            }
        }

        List<PathfindingTileNode> finalPathTiles = new List<PathfindingTileNode>();

        // Backtracking - setting the final path.
        if (closedPathTiles.Contains(endPoint))
        {
            currentTile = endPoint;
            finalPathTiles.Add(currentTile);

            for (int i = endPoint.G - 1; i >= 0; i--)
            {
                currentTile = closedPathTiles.Find(x => x.G == i && currentTile.adjacentTiles.Contains(x));
                finalPathTiles.Add(currentTile);
            }

            finalPathTiles.Reverse();
        }

        return finalPathTiles;
    }

    /// <summary>
    /// Returns estimated path cost from given start position to target position of hex PathfindingTileNode using Manhattan distance.
    /// </summary>
    /// <param name="startPosition">Start position.</param>
    /// <param name="targetPosition">Destination position.</param>
    protected static int GetEstimatedPathCost(Vector3Int startPosition, Vector3Int targetPosition)
    {
        return Mathf.Max(Mathf.Abs(startPosition.z - targetPosition.z), Mathf.Max(Mathf.Abs(startPosition.x - targetPosition.x), Mathf.Abs(startPosition.y - targetPosition.y)));
    }
}

public class PathfindingTileNode
{
    public int G;
    public int H;

    public int F => G + H;

    /// <summary>
    /// PathfindingTileNode's coordinates.
    /// </summary>
    public Vector3Int cubePosition;
    public Vector2Int cellPosition => StaticTileDataContainer.CubeToUnityCell(cubePosition);

    /// <summary>
    /// References to all adjacent tiles.
    /// </summary>
    public List<PathfindingTileNode> adjacentTiles = new List<PathfindingTileNode>();

    public PathfindingTileNode()
    {
        isMoveable = false;
        isDiscovered = false;
    }

    internal void setHexPosition(Vector2Int cell)
    {
        this.cubePosition = UnityCellToCube(cell);
    }

    /// <summary>
    /// If true - PathfindingTileNode is an obstacle impossible to pass.
    /// </summary>
    public bool isMoveable;
    public bool isDiscovered;

    public bool canGoThere => isMoveable && isDiscovered;

    private Vector3Int UnityCellToCube(Vector2Int cell)
    {
        var yCell = cell.x;
        var xCell = cell.y;
        var x = yCell - (xCell - (xCell & 1)) / 2;
        var z = xCell;
        var y = -x - z;
        return new Vector3Int(x, y, z);
    }
}
