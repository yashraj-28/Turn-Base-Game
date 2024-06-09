using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    private GridPosition gridPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode cameFromPathNode;
    private bool isWalkable = true;


    public PathNode( GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    public override string ToString()
    {
       
        return gridPosition.ToString();
    }
    public int GetGCost()
    {
        return gCost;
    }
    public int GetHCost()
    {
        return hCost;
    }
    public int GetFCost()
    {
        return fCost;
    }

    public void setGCost(int gCost)
    {
        this.gCost = gCost;
    }
    public void setHCost(int hCost)
    {
        this.hCost = hCost;
    }
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public void ResetCameFromPathNode()
    {
        cameFromPathNode = null;
    }
    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    public void SetCameFromPathNode(PathNode pathNode)
    {
        cameFromPathNode = pathNode;
    }
    public PathNode GetCameFromPathNode()
    {
        return cameFromPathNode;
    }
    public bool IsWalkable(){
        return isWalkable;
    }
    public void SetIsWalkable(bool isWalkable){
        this.isWalkable = isWalkable;
    }
}
