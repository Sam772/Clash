using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LogTerrain : GenericTerrain {
    
    // for this example we treat this terrain as a barrier
    public override IEnumerator TerrainDestroy() {
        yield return new WaitForEndOfFrame();
    }
}
