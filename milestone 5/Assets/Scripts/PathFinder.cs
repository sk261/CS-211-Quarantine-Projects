using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class PathFinder
{
    public static TilePath DiscoverPath(Tilemap map, Vector3Int start, Vector3Int end)
    {
        //you will return this path to the user.  It should be the shortest path between
        //the start and end vertices 
        TilePath discoveredPath = new TilePath();

        //TileFactory is how you get information on tiles that exist at a particular vector's
        //coordinates
        TileFactory tileFactory = TileFactory.GetInstance();

        //This is the priority queue of paths that you will use in your implementation of
        //Dijkstra's algorithm
        PriortyQueue<TilePath> pathQueue = new PriortyQueue<TilePath>();

        //You can slightly speed up your algorithm by remembering previously visited tiles.
        //This isn't strictly necessary.
        List<Vector3Int> discoveredTiles = new List<Vector3Int>();

        //quick sanity check
        if (map == null || start == null || end == null)
        {
            return discoveredPath;
        }
        discoveredTiles.Add(start);

        //This is how you get tile information for a particular map location
        //This gets the Unity tile, which contains a coordinate (.Position)
        var startingMapLocation = map.GetTile(start);

        //And this converts the Unity tile into an object model that tracks the
        //cost to visit the tile.
        Tile startingTile = tileFactory.GetTile(startingMapLocation.name);
        startingTile.Position = start;

        // Temporary usage of discoveredPath
        discoveredPath.AddTileToPath(startingTile);

        //This adds the starting tile to the PQ and we start off from there...
        pathQueue.Enqueue(discoveredPath);
        while(pathQueue.IsEmpty() == false)
        {
            TilePath current = pathQueue.Dequeue();
            Vector3Int currentPos = current.GetMostRecentTile().Position;
            if (currentPos == end)
            {
                discoveredPath = current;
                break;
            }
            for (int x = -1; x <= 1; x ++)
                for (int y = Math.Abs(x)-1; y <= 1-Math.Abs(x); y+=2)
                {
                    Vector3Int find = new Vector3Int(currentPos.x + x, currentPos.y + y, currentPos.z);
                    if (!discoveredTiles.Contains(find))
                    {
                        discoveredTiles.Add(find);
                        TilePath next = new TilePath(current);
                        var nextLocation = map.GetTile(find);
                        if (nextLocation == null) continue;
                        Tile nextTile = tileFactory.GetTile(nextLocation.name);
                        nextTile.Position = find;
                        next.AddTileToPath(nextTile);
                        pathQueue.Enqueue(next);
                    }
                }
        }
        return discoveredPath;
    }

}
