using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarTer : MonoBehaviour
{
    public TerrainGeneration TerrainGeneration;
    public PlayerScript PlayerScript;

    public void Start() {
        TerrainGeneration.GenerateTerrain();
        TerrainGeneration.UsTransformTracked = true;
        PlayerScript.SetIsPlaying();
    }
}
