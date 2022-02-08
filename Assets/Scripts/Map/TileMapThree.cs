using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapThree : GenericTileMap {
    
    public override void GenerateMapInfo() {
        tiles = new int[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) { tiles[x, y] = 0; }
        }

        // oasis water tiles
        tiles[2, 10] = 1;
        tiles[2, 11] = 1;
        tiles[3, 10] = 1;
        tiles[3, 11] = 1;
        tiles[3, 12] = 1;
        tiles[4, 9] = 1;
        tiles[4, 10] = 1;
        tiles[4, 11] = 1;
        tiles[4, 12] = 1;
        tiles[4, 13] = 1;
        tiles[5, 9] = 1;
        tiles[5, 10] = 1;
        tiles[5, 11] = 1;
        tiles[5, 12] = 1;
        tiles[5, 13] = 1;
        tiles[6, 11] = 1;
        tiles[6, 12] = 1;
        tiles[7, 12] = 1;
        tiles[8, 12] = 1;

        // oasis grass tiles
        tiles[0, 10] = 2;
        tiles[1, 9] = 2;
        tiles[1, 10] = 2;
        tiles[1, 11] = 2;
        tiles[1, 12] = 2;
        tiles[2, 12] = 2;
        tiles[2, 13] = 2;
        tiles[3, 8] = 2;
        tiles[3, 9] = 2;
        tiles[3, 13] = 2;
        tiles[3, 14] = 2;
        tiles[4, 8] = 2;
        tiles[4, 14] = 2;
        tiles[4, 15] = 2;
        tiles[5, 8] = 2;
        tiles[5, 14] = 2;
        tiles[5, 15] = 2;
        tiles[6, 8] = 2;
        tiles[6, 9] = 2;
        tiles[6, 10] = 2;
        tiles[6, 13] = 2;
        tiles[6, 14] = 2;
        tiles[7, 9] = 2;
        tiles[7, 10] = 2;
        tiles[7, 11] = 2;
        tiles[7, 13] = 2;
        tiles[8, 11] = 2;
        tiles[8, 13] = 2;
        tiles[9, 11] = 2;
        tiles[9, 12] = 2;
        tiles[9, 13] = 2;
        tiles[10, 12] = 2;

        // oasis tree tiles
        tiles[2, 9] = 3;
    }
}
