using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch : MonoBehaviour {

    List<GameObject> listOfMoveableTiles = new List<GameObject>();
    List<GameObject> listOfMoveableTilesOccupiedBySame = new List<GameObject>();

    //THIS CLASS IS MY IMPLEMENTATION OF FINDING A UNIT'S MOVE RANGE ON A GRAPH OF VARYING TYPES OF TERRAIN
    //THIS IS CALLED BEFORE THE A* PATHFINDING, SO THAT THE A* HAS A LIST OF MOVEABLE TILES TO WORK WITH
    //THE TILES ARE STORED IN A LIST IN ANOTHER SCRIPT, SO ANY TIME THE TERM INDEX
    //OR THE FUNCTION TILEARRAYACTUALFUNCT IS USED, IT IS REFERRING TO THE TILES OR THEIR INDEXES
    //STORED IN THE REFERENCED SCRIPT

    //this function should be initiated with xPos and yPos corresponding to the unit calling it's x and y positions
    //the current move points should be initialized with the unit calling its maximum move points
    public void ShowMovementRange(float xPos, float yPos, int currentMovePoints, List<GameObject> tileList, PlayerMovement playMove, MainGameData mainTong, bool canWalkOnMountains, bool canWalkOnWater)
    {
        int startIndex = mainTong.TileArrayActualFunct(new Vector3(xPos, yPos, 0));
        GameObject startTile = tileList[startIndex];
        bool startWalkable = true;
        bool doorOrNah = false;
        bool isWall = true;
        bool addedThisRun = false;
        
        //another script stores all the tile's movement points and initalizes them all to 100 before this script is called
        //values over 100 mean the tile has been checked and deemed unwalkable to this unit type
        //values under 100 mean the tile has been checked, but if you can visit that tile from a path where you have more move points remaining, it can be checked/overwritten again
        //this allows the function to iterate through all the relevant tiles and easily check the stored values to see if the current tile is both possible to visit/has been visited by a worse/better path
        //higher move points are better, since it means that you are visiting a tile with more spaces left to go from there
        
        int startMove = GameDatabase.database.GetMovementPoints(startTile); //finding the movement points this tile has been visited with
        if (startMove == 100) //100 is the value that tiles that haven't been checked yet are set to
        {
            startWalkable = GameDatabase.database.IsTileTypeWalkableForGroundUnits(mainTong.tileManagerMainGame.typeOfEachTile[startIndex]);
            isWall = GameDatabase.database.IsTileTypeWall(mainTong.tileManagerMainGame.typeOfEachTile[startIndex]);
            
            //checking different permutations of unwalkeable/walkable tile types (for tiles that haven't been checked yet)
            if (((startWalkable == true && canWalkOnMountains == false && canWalkOnWater == false) || (canWalkOnMountains == true && canWalkOnWater == true && isWall == false)))
            {
                doorOrNah = ((mainTong.tileManagerMainGame.typeOfEachTile[startIndex] == (int)(GameDatabase.TileTypesInts.Door) && mainTong.tileManagerMainGame.eachExtraUsedOrNot[mainTong.tileManagerMainGame.ExtraIndexNumGetter(startIndex)]) || mainTong.tileManagerMainGame.typeOfEachTile[startIndex] != (int)(GameDatabase.TileTypesInts.Door));
                if (doorOrNah == true) //add the tile and set it's value to currentMovePoints
                {
                    listOfMoveableTiles.Add(startTile);
                    GameDatabase.database.SetMovementPointsMove(startTile, currentMovePoints);
                    addedThisRun = true;
                }
                else //set tile to unwalkable and move on to the next one
                {
                    GameDatabase.database.SetMovementPointsMove(startTile, 102);
                    return;
                }
            }
            else //set tile to unwalkable and move on to the next one
            {
                GameDatabase.database.SetMovementPointsMove(startTile, 102);
                return;
            }
        }
        else if (startMove > 100)
        {
            return;
        }
        else
        {
            //calling a function that tries to set the current tile's move points to the movePoints passed in to this function
            //if the movePoints we have currently is lower or the same, the function returns false and we don't do anything
            //otherwise, the function returns true and sets the current tile's movePoints to our currentMovePoints
            if (GameDatabase.database.SetMovementPointsMove(startTile, currentMovePoints) == false)
            {
                return;
            }
            startMove = currentMovePoints;
            startWalkable = GameDatabase.database.IsTileTypeWalkableForGroundUnits(mainTong.tileManagerMainGame.typeOfEachTile[startIndex]);
            doorOrNah = ((mainTong.tileManagerMainGame.typeOfEachTile[startIndex] == (int)(GameDatabase.TileTypesInts.Door) && mainTong.tileManagerMainGame.eachExtraUsedOrNot[mainTong.tileManagerMainGame.ExtraIndexNumGetter(startIndex)]) || mainTong.tileManagerMainGame.typeOfEachTile[startIndex] != (int)(GameDatabase.TileTypesInts.Door));
            isWall = GameDatabase.database.IsTileTypeWall(mainTong.tileManagerMainGame.typeOfEachTile[startIndex]);
        }
        BaseCharacterClass toonHere = null;
        if (addedThisRun == true)
        {
            //if the tile has just been checked in this function
            //check to see if its already occupied by another unit
            //if so, add it to the listOfMoveableTilesOccupiedBySame list for reference later
            //since it's technically moveable but you can't actually land there, etc.
            if (playMove.playersAndEnemyPositions.TryGetValue(startTile.transform.position, out toonHere) == true)
            {
                if (this.enemyTing == null)
                {
                    if (toonHere.enemyTing == null)
                    {
                        listOfMoveableTilesOccupiedBySame.Add(startTile);
                    }
                }
                else if (this.enemyTing != null)
                {
                    if (toonHere.enemyTing != null)
                    {
                        listOfMoveableTilesOccupiedBySame.Add(startTile);
                    }
                }
            }
        }
        if (((startWalkable == true && canWalkOnMountains == false && canWalkOnWater == false) || (canWalkOnMountains == true && canWalkOnWater == true && isWall == false) || doorOrNah))
        {
            //getNeighbors is a function that returns a list of all adjacent tiles to the tile passed in
            List<GameObject> tileNeighborrs = GetNeighbors(startTile, startIndex);
            int getmovePoints = 100;
            bool occupiedByOpposite = false;
            int nextMoveCost = currentMovePoints;
            
            //now we iterate through all the neighbors of this tile
            //and if it passes all the checks (hasn't been visited, or is visitable and has a lower movePoints value, etc.)
            //we will call this function recursively on that tile
            //otherwise, we continue on
            for (int i = 0; i < tileNeighborrs.Count; i++)
            {
                int neighborIndex = mainTong.TileArrayActualFunct(tileNeighborrs[i].transform.position);
                nextMoveCost = currentMovePoints - GameDatabase.database.GetTileMovementCost(mainTong.tileManagerMainGame.typeOfEachTile[neighborIndex]);
                if (nextMoveCost >= 0)
                {
                    occupiedByOpposite = false;
                    getmovePoints = GameDatabase.database.GetMovementPoints(tileNeighborrs[i]);
                    if (getmovePoints < 100)
                    {
                        if (nextMoveCost <= getmovePoints)
                        {
                            continue;
                        }
                    }
                    else if (getmovePoints == 100)
                    {
                        if (playMove.playersAndEnemyPositions.TryGetValue(tileNeighborrs[i].transform.position, out toonHere) == true)
                        {
                            if (this.enemyTing == null)
                            {
                                if (toonHere.enemyTing != null)
                                {
                                    occupiedByOpposite = true;
                                    GameDatabase.database.SetMovementPointsMove(tileNeighborrs[i], 102);
                                }
                            }
                            else if (this.enemyTing != null)
                            {
                                if (toonHere.enemyTing == null)
                                {
                                    occupiedByOpposite = true;
                                    GameDatabase.database.SetMovementPointsMove(tileNeighborrs[i], 102);
                                }
                            }
                        }
                    }
                    else if (getmovePoints > 100)
                    {
                        continue;
                    }
                    if (occupiedByOpposite == false)
                    {
                        ShowMovementRange(tileNeighborrs[i].transform.position.x, tileNeighborrs[i].transform.position.y, nextMoveCost, tileList, playMove, mainTong, canWalkOnMountains, canWalkOnWater);
                    }
                }
            }
        }       
    }
}
