using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapOne : GenericTileMap {
    
    public override void GenerateMapInfo() {
        tiles = new int[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) { tiles[x, y] = 0; }
        }
        tiles[1, 4] = 1; tiles[2, 6] = 1; tiles[3, 2] = 1; tiles[4, 5] = 1; 
        tiles[9, 4] = 1; tiles[6, 3] = 1; tiles[7, 7] = 1;
    }
}
