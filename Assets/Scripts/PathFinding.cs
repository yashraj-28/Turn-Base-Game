using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    [SerializeField] private Transform gridDebugObjectPrefab;
    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;
    public static PathFinding Instance { get; private set; }
    [SerializeField] private LayerMask obstaclesLayerMask;

    private void Awake() {

        if(Instance != null)
        {
            Debug.LogError("There's more than one PathFinding" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

       
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

         gridSystem = new GridSystem<PathNode>(width, height, cellSize, 
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
            
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycatOffsetDistance = 5f;
                if(Physics.Raycast(
                    worldPosition + Vector3.down * raycatOffsetDistance, 
                    Vector3.up,
                    raycatOffsetDistance * 2,
                    obstaclesLayerMask ))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closeList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.setGCost(int.MaxValue);
                pathNode.setHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.setGCost(0);
        startNode.setHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                // reached final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if(closeList .Contains(neighbourNode)){
                    continue;
                }

                if(!neighbourNode.IsWalkable())
                {
                    closeList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = 
                    currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());
                
                if(tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.setGCost(tentativeGCost);
                    neighbourNode.setHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if(!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // no path found 
        pathLength = 0;
        return null;
    }
    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        //int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);

        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for(int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        } 
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if(gridPosition.x - 1 >= 0)
        {
            // left node
        neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0 ));

        if(gridPosition.z - 1 >= 0)
        {
            // left-down node
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1 ));
        
        }
        if(gridPosition.z + 1 < gridSystem.GetHeight())
        {
            // left-up node
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1 ));
        }
        
        }

        if(gridPosition.x + 1  < gridSystem.GetWidth())
        {
            // right node
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0 ));

        if(gridPosition.z - 1 >= 0)
        {

            // right-down node
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1 ));
        }

        if(gridPosition.z + 1 < gridSystem.GetHeight())
        {

            // right-up node
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1 ));
        }
        }
        
        if(gridPosition.z - 1 >= 0)
        {

            // down node
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1 ));
        }

        if(gridPosition.z + 1 < gridSystem.GetHeight())
        {

            // up node
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1 ));
        }
        

        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;

        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach(PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition()); 
        }

        return gridPositionList;
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }
    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
         gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }
    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
       return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }
    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
