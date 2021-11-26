using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManagerExtension
{
    public static bool IsRunning(this GameManager mgr)
        {
            return mgr != null && mgr.Data != null && mgr.Data.IsRunning;
        }
}
