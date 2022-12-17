using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class TerrainGeneration : MonoBehaviour
{
   public int Height = 30;
   public int Width = 10;

   public bool UsDebugSprite;
   public SpriteRenderer PrefabsSprite;
   [Header("Perlin Noise Setting")] 
   public bool IsUpdate;
   public float Scale=1;
   public Vector2 Offset;
   [Range(0,1)]public float WhiteThreshHold = 0.5f;
   [Header("TerrainGeneration")] 
   public float TileScale = 100;
   public GameObject PrefabTile;

   private TerrainCell[,] _cells;
   private TerrainCell _startCell;
   private TerrainCell _endCell;

   public void Start() {
      _cells = new TerrainCell[Width, Height];
      for (int x = 0; x < Width; x++) {
         for (int z = 0; z < Height; z++)
         {
            _cells[x, z] = new TerrainCell(new Vector2Int(x, z));
            if (UsDebugSprite) {
               _cells[x, z].SpriteRenderer = Instantiate(PrefabsSprite, new Vector3(x, 0, z), Quaternion.identity);
               _cells[x, z].SpriteRenderer.transform.forward = Vector3.up;
            }
         }
      }
   }

   public TerrainCell GetCell(Vector2Int pos) {
      if (pos.x < 0 || pos.x >= Width || pos.y < 0 || pos.y >= Height)
         return null;
      return _cells[pos.x, pos.y];
   }

   public List<TerrainCell> GetCellNeighbour(TerrainCell cell) {
      List<TerrainCell> returnList = new List<TerrainCell>();
      if(GetCell(cell.Pos+new Vector2Int(1,0))!=null)returnList.Add(GetCell(cell.Pos+new Vector2Int(1,0)));
      if(GetCell(cell.Pos+new Vector2Int(0,1))!=null)returnList.Add(GetCell(cell.Pos+new Vector2Int(0,1)));
      if(GetCell(cell.Pos+new Vector2Int(-1,0))!=null)returnList.Add(GetCell(cell.Pos+new Vector2Int(-1,0)));
      if(GetCell(cell.Pos+new Vector2Int(0,-1))!=null)returnList.Add(GetCell(cell.Pos+new Vector2Int(0,-1)));
      return returnList;
   }

   private void Update()
   {
      if(IsUpdate)UpdatePerlinNoise();
      
   }

   private void UpdatePerlinNoise()
   {
      for (int x = 0; x < Width; x++) {
         for (int z = 0; z < Height; z++) {
            if (UsDebugSprite) {
               _cells[x, z].SetValue(Mathf.PerlinNoise((x * Scale) + Offset.x, (z * Scale) + Offset.y));
               if (_cells[x, z].Value > WhiteThreshHold) _cells[x, z].SetValue(1);
            }
            else
            {
               _cells[x, z].Value =(Mathf.PerlinNoise((x * Scale) + Offset.x, (z * Scale) + Offset.y));
               if (_cells[x, z].Value > WhiteThreshHold) _cells[x, z].Value=1;
            }
         }
      }
   }
   [ContextMenu("Pick Start and End")]
   private void PickStartAndEnd()
   {
      _startCell = _cells[Random.Range(0,Width), 0];
      _endCell = _cells[Random.Range(0,Width), Height-1];
      if (UsDebugSprite) {
         _startCell.SpriteRenderer.color = Color.red;
         _endCell.SpriteRenderer.color = Color.red;
      }
   }

   [ContextMenu("CalculatePath")]
   public List<Vector2Int> CalculatePath() {
      if (_endCell == null || _startCell == null) {
         Debug.LogWarning( "No Start Or End For The A Start");
         return null;
      }
      List<TerrainCell> path = AStart();

      if (UsDebugSprite) {
         foreach (var cell in path)
         {
            cell.SpriteRenderer.color = Color.green;
         }
      }
      List<Vector2Int> returnList = new List<Vector2Int>();
      foreach (var cell in path) {
         returnList.Add(cell.Pos);
      }
      return returnList;
   }

   public List<TerrainCell> AStart()
   {
      foreach (var cell in _cells) {
         cell.Gcost = Int32.MaxValue;
         cell.FromCell = null;
      }
      List<TerrainCell> OpenList = new List<TerrainCell>();
      List<TerrainCell> CloseList = new List<TerrainCell>();
      _startCell.Hcost = GetHCost(_startCell.Pos, _endCell.Pos);
      OpenList.Add(_startCell);

      while (OpenList.Count>0)
      {
         TerrainCell current=OpenList[0];
         foreach (var cell in OpenList) {
            if (cell.FVCost<current.FVCost) current = cell;
         }
         OpenList.Remove(current);
         CloseList.Add(current);

         if (current == _endCell) {
            return GetPathFrom(current);
         }

         foreach (var neighbor in GetCellNeighbour(current)) {
            if( CloseList.Contains(neighbor))continue;
            if (neighbor.Gcost > current.Gcost + 10 + current.vCost
               || !OpenList.Contains(neighbor)) {
               neighbor.Gcost = current.Gcost + 10 + current.vCost;
               neighbor.Hcost = GetHCost(neighbor.Pos, _endCell.Pos);
               neighbor.FromCell = current;
               if( !OpenList.Contains(neighbor))OpenList.Add(neighbor);
            }
         }
      }
      return null;
   }

   private int GetHCost(Vector2Int pos1, Vector2Int pos2) {
      return Mathf.Abs(pos1.x - pos2.x)+Mathf.Abs(pos1.y - pos2.y);
   }

   private List<TerrainCell> GetPathFrom(TerrainCell origin) {
      List<TerrainCell> returnList = new List<TerrainCell>();
      returnList.Add(origin);
      TerrainCell current =origin;
      while (current != null) {
         current = current.FromCell;
         if(current!=null)returnList.Add(current);
      }
      return returnList;
   }

   [ContextMenu("GenerateTerrain")]
   private void GenerateTerrain()
   {
      UpdatePerlinNoise();
      PickStartAndEnd();
      List<TerrainCell> tiles = AStart();
      foreach (var tile in tiles) {
         tile.Tile = Instantiate(PrefabTile, new Vector3(tile.Pos.x * TileScale, 0, tile.Pos.y * TileScale), Quaternion.identity);
      }
   }
}
