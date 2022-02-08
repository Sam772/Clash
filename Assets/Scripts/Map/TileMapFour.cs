using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapFour : GenericTileMap {
    
    public override void GenerateMapInfo() {
        tiles = new int[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) { tiles[x, y] = 0; }
        }

        // snow tree tiles
        tiles[3, 8] = 1;
        tiles[14, 5] = 1;
        tiles[11, 11] = 1;
        tiles[6, 13] = 1;
    }
}
