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

        // bottom segment
        for (int x = 0; x < 20; x++) {
            for (int y = 0; y < 2; y++) {
                tiles[x, y] = 1;
            }
        }

        for (int x = 9; x < 18; x++) {
            tiles[x, 2] = 1;
        }

        for (int x = 11; x < 18; x++) {
            tiles[x, 3] = 1;
        }

        for (int x = 13; x < 18; x++) {
            tiles[x, 4] = 1;
        }

        for (int x = 15; x < 18; x++) {
            tiles[x, 5] = 1;
        }

        for (int x = 17; x < 18; x++) {
            tiles[x, 6] = 1;
        }

        // right segment
        for (int x = 18; x < 20; x++) {
            for (int y = 2; y < 15; y++) {
                tiles[x, y] = 1;
            }
        }

        tiles[17, 8] = 1;
        tiles[16, 8] = 1;

        for (int x = 10; x < 18; x++) {
             tiles[x, 9] = 1;
        }

        for (int x = 11; x < 18; x++) {
             tiles[x, 10] = 1;
        }

        for (int x = 12; x < 18; x++) {
             tiles[x, 11] = 1;
        }

        for (int x = 13; x < 18; x++) {
             tiles[x, 12] = 1;
        }

        tiles[16, 13] = 1;
        tiles[17, 13] = 1;

        tiles[18, 15] = 1;
        tiles[19, 15] = 1;
        tiles[19, 16] = 1;
        tiles[19, 17] = 1;
        tiles[18, 17] = 1;

        // left segment
        for (int x = 2; x < 5; x++) {
            tiles[x, 8] = 1;
        }

        for (int x = 1; x < 6; x++) {
            tiles[x, 9] = 1;
        }

        for (int x = 0; x < 7; x++) {
            tiles[x, 10] = 1;
        }

        for (int x = 0; x < 8; x++) {
            tiles[x, 11] = 1;
        }

        for (int x = 0; x < 7; x++) {
            tiles[x, 12] = 1;
        }

        for (int x = 0; x < 7; x++) {
            tiles[x, 13] = 1;
        }

        for (int x = 0; x < 6; x++) {
            tiles[x, 14] = 1;
        }

        for (int x = 0; x < 6; x++) {
            tiles[x, 15] = 1;
        }

        for (int x = 0; x < 5; x++) {
            tiles[x, 16] = 1;
        }

        for (int x = 0; x < 5; x++) {
            tiles[x, 17] = 1;
        }

        for (int x = 0; x < 4; x++) {
            tiles[x, 18] = 1;
        }

        for (int x = 0; x < 4; x++) {
            tiles[x, 19] = 1;
        }

        for (int x = 0; x < 2; x++) {
            for (int y = 13; y < 30; y++) {
                tiles[x, y] = 1;
            }
        }

        // top segment
        for (int x = 2; x < 5; x++) {
            for (int y = 25; y < 30; y++) {
                tiles[x, y] = 1;
            }
        }

        tiles[2, 20] = 1;
        tiles[2, 23] = 1;
        tiles[2, 24] = 1;
        tiles[3, 24] = 1;

        for (int x = 5; x < 10; x++) {
            for (int y = 26; y < 30; y++) {
                tiles[x, y] = 1;
            }
        }

        tiles[6, 25] = 1;
        tiles[7, 25] = 1;
        tiles[7, 24] = 1;
        tiles[8, 25] = 1;
        tiles[8, 24] = 1;
        tiles[8, 23] = 1;

        tiles[10, 28] = 1;
        tiles[10, 29] = 1;
        tiles[11, 29] = 1;
        tiles[12, 28] = 1;
        tiles[12, 29] = 1;

        for (int x = 13; x < 15; x++) {
            for (int y = 16; y < 30; y++) {
                tiles[x, y] = 1;
            }
        }

        tiles[14, 16] = 0;

        tiles[15, 29] = 1;
        tiles[15, 28] = 1;
        tiles[15, 27] = 1;
        tiles[16, 28] = 1;
        tiles[16, 29] = 1;
        tiles[17, 27] = 1;
        tiles[17, 28] = 1;
        tiles[17, 29] = 1;

        for (int x = 18; x < 20; x++) {
            for (int y = 18; y < 30; y++) {
                tiles[x, y] = 1;
            }
        }
    }

}
