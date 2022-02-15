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
        tiles[18, 5] = 1;
        tiles[7, 11] = 1;
        tiles[19, 13] = 1;

        // stone tiles
        tiles[10, 2] = 2;
        tiles[11, 2] = 2;
        tiles[12, 2] = 2;
        tiles[13, 2] = 2;
        tiles[14, 2] = 2;
        tiles[9, 3] = 2;
        tiles[15, 3] = 2;
        tiles[16, 4] = 2;
        tiles[16, 5] = 2;
        tiles[16, 6] = 2;
        tiles[16, 8] = 2;
        tiles[16, 9] = 2;
        tiles[16, 10] = 2;
        tiles[8, 4] = 2;
        tiles[8, 5] = 2;
        tiles[8, 6] = 2;
        tiles[8, 8] = 2;
        tiles[8, 9] = 2;
        tiles[8, 10] = 2;
        tiles[9, 11] = 2;
        tiles[15, 11] = 2;
        tiles[10, 12] = 2;
        tiles[11, 12] = 2;
        tiles[12, 12] = 2;
        tiles[13, 12] = 2;
        tiles[14, 12] = 2;

        // stone floor tiles
        for (int x = 9; x < 16; x++) {
            for (int y = 4; y < 11; y++) { 
                tiles[x, y] = 3; 
            }
        }

        tiles[8, 7] = 3;
        tiles[16, 7] = 3;
        tiles[10, 3] = 3;
        tiles[11, 3] = 3;
        tiles[12, 3] = 3;
        tiles[13, 3] = 3;
        tiles[14, 3] = 3;
        tiles[10, 11] = 3;
        tiles[11, 11] = 3;
        tiles[12, 11] = 3;
        tiles[13, 11] = 3;
        tiles[14, 11] = 3;
    }
}
