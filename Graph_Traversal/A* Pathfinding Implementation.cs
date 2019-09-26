using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour {
    [System.NonSerialized]
    public List<GameObject> path = new List<GameObject>();
    
    GameObject startingTile;
    GameObject endingTile;
    
    //THIS CLASS INTEGRATES WITH MY CUSTOM TILEMANAGER CLASS
    //ANY TIME THE TERM INDEX, OR TILE TYPE IS USED IN THIS SCRIPT
    //IT REFERS TO THE INDEX, AND TILE TYPE (GRASS, FOREST, ETC. TO DETERMINE MOVE COST ON GRID)
    //OF THE ACTUAL OBJECTS REPRESENTING THE TILES, RESPECTIVELY (THEY ARE STORED IN AN ARRAY IN ANOTHER SCRIPT)
    
    //ANOTHER NOTE: TILEARRAYACTUALFUNCT IS A FUNCTION IN THE OTHER SCRIPT
    //THAT RETURNS THE INDEX IN THE ARRAY FOR A GIVEN TILE, BASED ON A GIVEN VECTOR3 ARGUMENT PASSED IN TO THE FUNCTION
    
    //gets a list of all adjacent tiles (checking whether the tile exists or if this tile is on the edge of a graph)
    public List<GameObject> GetNeighbors(GameObject currentTileChoice, int indexOfCurTile)
    {
        List<GameObject> neighbors = new List<GameObject>();
        Transform tileTrans = currentTileChoice.transform;
        //now we grab the tile list that was mentioned in the comments above
        List<GameObject> tilesToPathfindWith = MainGameData.mainGameDataTing.tileManagerMainGame.GetTileList();
        if (tileTrans.position.x + 1 <= MainGameData.mainGameDataTing.FarthestRightXOnGrid)
        {
            neighbors.Add(tilesToPathfindWith[indexOfCurTile + 1]);
        }
        if (tileTrans.position.x - 1 >= MainGameData.mainGameDataTing.FarthestLeftXOnGrid)
        {
            neighbors.Add(tilesToPathfindWith[indexOfCurTile - 1]);
        }
        if (tileTrans.position.y + 1 <= MainGameData.mainGameDataTing.FarthestUpYOnGrid)
        {
            neighbors.Add(tilesToPathfindWith[MainGameData.mainGameDataTing.TileArrayActualFunct(new Vector3(tileTrans.position.x, tileTrans.position.y + 1, 0))]);
        }
        if (tileTrans.position.y - 1 >= MainGameData.mainGameDataTing.FarthestDownYOnGrid)
        {
            neighbors.Add(tilesToPathfindWith[MainGameData.mainGameDataTing.TileArrayActualFunct(new Vector3(tileTrans.position.x, tileTrans.position.y - 1, 0))]);
        }
        return neighbors;
    }

    public void FindPath(double currentXPositionOfPlayer, double currentYPositionOfPlayer, double actualXPositionOfDesiredEndTile, double actualYPositionOfDesiredEndTile)
    {
        path.Clear(); //clear path from prior pathfinding operations
        GameObject mainDataScript = MainGameData.mainGameDataTing.gameObject;

        //get list of tiles to work with
        List<GameObject> currentTileArray = MainGameData.mainGameDataTing.tileManagerMainGame.GetTileList();
        
        //find and assign tiles for beginning and end of path
        startingTile = currentTileArray[mainDataScript.GetComponent<MainGameData>().TileArrayActualFunct(new Vector3((float)(currentXPositionOfPlayer), (float)(currentYPositionOfPlayer), 0))];
        endingTile = currentTileArray[mainDataScript.GetComponent<MainGameData>().TileArrayActualFunct(new Vector3((float)(actualXPositionOfDesiredEndTile), (float)(actualYPositionOfDesiredEndTile), 0))];
        
        //check whether this unit can move over certain types of terrain and cache those values
        bool walkOnMountains = GameDatabase.database.CanUnitMoveOverMountainsOrWater(currentCharacterClass, true);
        bool walkOnWater = GameDatabase.database.CanUnitMoveOverMountainsOrWater(currentCharacterClass, false);
        
        //create an open and closed set of tiles to iterate through
        List<GameObject> openSet = new List<GameObject>();
        HashSet<GameObject> closedSet = new HashSet<GameObject>();
        openSet.Add(startingTile);
        
        while (openSet.Count > 0) //keep repeating this function until we have exhausted all possibilities or found a path
        {
            GameObject currentTile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                GameObject openSetzCurrentTileInfo = openSet[i];
                int fCost = GameDatabase.database.GetFCost(openSetzCurrentTileInfo);
                int curTilefCost = GameDatabase.database.GetFCost(currentTile);
                int hCost = 0;
                int curTileHCost = 0;
                GameDatabase.database.hCost.TryGetValue(openSetzCurrentTileInfo, out hCost);
                GameDatabase.database.hCost.TryGetValue(currentTile, out curTileHCost);
                if (fCost < curTilefCost || fCost == curTilefCost && hCost < curTileHCost)
                {
                    currentTile = openSet[i];
                    break;
                }
            }
            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            //end function and call retrace function when you hit ending tile
            if (currentTile == endingTile)
            {
                RetracePath(startingTile, endingTile);
                return;
            }
            
            int curTileIndex = MainGameData.mainGameDataTing.TileArrayActualFunct(currentTile.transform.position);
            foreach (GameObject neighbor in this.GetNeighbors(currentTile, curTileIndex))
            {
                int neighborIndex = MainGameData.mainGameDataTing.TileArrayActualFunct(neighbor.transform.position);
                
                //moveable tile list and its functionality is implemented in the breadth first search script
                //checking if unit has added this tile to its possible moveable list AND that it is a proper tile type
                //if it fails any of these checks, it isn't looked at as a potential addition to the openSet above
                if ((GameDatabase.database.IsTileTypeWalkableForGroundUnits(MainGameData.mainGameDataTing.tileManagerMainGame.typeOfEachTile[neighborIndex]) == false && walkOnMountains == false && walkOnWater == false) || closedSet.Contains(neighbor) || GameDatabase.database.IsTileTypeWall(MainGameData.mainGameDataTing.tileManagerMainGame.typeOfEachTile[neighborIndex]) == true)
                {
                    continue;
                }
                
                //caching gCosts for reference later/above
                int gCost = 0;
                if (GameDatabase.database.gCost.TryGetValue(neighbor, out gCost) == false)
                {
                    GameDatabase.database.gCost.Add(neighbor, gCost);
                }
                int curGCost = 0;
                if (GameDatabase.database.gCost.TryGetValue(currentTile, out curGCost) == false)
                {
                    GameDatabase.database.gCost.Add(currentTile, curGCost);
                }
                int newMovementCostToNeighbor = curGCost + GetDistance(currentTile, neighbor, curTileIndex, neighborIndex);
                if (newMovementCostToNeighbor < gCost || !openSet.Contains(neighbor) && listOfMoveableTiles.Contains(neighbor))
                {
                    GameDatabase.database.SetGCost(neighbor, newMovementCostToNeighbor);
                    GameDatabase.database.SetHCost(neighbor, GetDistance(neighbor, endingTile, neighborIndex, MainGameData.mainGameDataTing.TileArrayActualFunct(endingTile.transform.position)));
                    GameDatabase.database.SetFCost(neighbor);
                    GameDatabase.database.SetParent(neighbor, currentTile);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

    }
    
    //you have to reverse the path at the end since the function adds tiles in the opposite direction
    public void RetracePath(GameObject startTileNode, GameObject endTileNode)
    {
        List<GameObject> path = new List<GameObject>();
        GameObject currentNode = endTileNode;

        while (currentNode != startTileNode)
        {
            path.Add(currentNode);
            currentNode = GameDatabase.database.tileParents[currentNode];
        }
        path.Reverse();

        this.path = path;
        GameDatabase.database.ClearDatabaseMoveDicts();
    }

    //this function gives a heuristic value to each tile when doing pathfinding, based on the movement cost of a tile
    //and it's distance from the desired end point (lower is preferable)
    public int GetDistance(GameObject tileA, GameObject tileB, int indexA, int indexB)
    {
        double dstX = Mathf.Abs(tileA.transform.position.x - tileB.transform.position.x);
        double dstY = Mathf.Abs(tileA.transform.position.y - tileB.transform.position.y);
        if (dstX > dstY)
        {
            return (int)(14 * dstY + 10 * (dstX - dstY) * GameDatabase.database.GetTileMovementCost(MainGameData.mainGameDataTing.tileManagerMainGame.typeOfEachTile[indexB]));
        }
        else
        {
            return (int)(14 * dstX + 10 * (dstY - dstX) * GameDatabase.database.GetTileMovementCost(MainGameData.mainGameDataTing.tileManagerMainGame.typeOfEachTile[indexB]));
        }
    }
}
