using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapTwo : GenericTileMap {
    
    public override void GenerateMapInfo() {
        tiles = new int[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) { tiles[x, y] = 0; }
        }

        // mountain tiles
        tiles[11, 6] = 1;
        tiles[11, 7] = 1;
        tiles[11, 8] = 1;
        tiles[12, 6] = 1;
        tiles[12, 7] = 1;
        tiles[12, 8] = 1;
        tiles[13, 6] = 1;
        tiles[13, 7] = 1;
        tiles[13, 8] = 1;
    }

}
