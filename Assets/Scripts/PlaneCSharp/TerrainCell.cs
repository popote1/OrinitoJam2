using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCell
{

    public enum RoadType {
        Start ,End,Straingt, Left, Right
    }

    public RoadType Type;

    public Vector2Int EnterDirection;
    public Vector2Int Pos;
    public SpriteRenderer SpriteRenderer;
    public float Value;
    public GameObject Tile;

    public TerrainCell(Vector2Int pos) {
        Pos = pos;
    }

    public void SetValue(float value) {
        Value = value;
        Color col =Color.white*value;
        col.a = 1;
        if (SpriteRenderer) SpriteRenderer.color = col;
    }

    public int vCost => Mathf.RoundToInt(Value * 10000);
    public int Gcost;
    public int Hcost;
    public int Fcost => Gcost + Hcost;
    public int FVCost => Fcost + vCost;
    public TerrainCell FromCell;
}
