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
   public bool UsTransformTracked;
   public Transform TransformTraked;
   public int DiscoverLength;

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
   public GameObject[] PrefabsStraight;
   public GameObject[] PrefabsLeft;
   public GameObject[] PrefabsRight;

   private TerrainCell[,] _cells;
   private TerrainCell _startCell;
   private TerrainCell _endCell;
   private List<TerrainCell> Path = new List<TerrainCell>();
   private int _currentIndex;

   public void SetCells() {
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

   public Vector2Int GetCordinatesFromWorldPos(Vector3 pos)
   {
      int x = Mathf.RoundToInt(pos.x / 100f);
      int y = Mathf.RoundToInt(pos.z / 100f);
      return new Vector2Int(x, y);
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
      if (UsTransformTracked) TrackPos();

   }

   private void TrackPos() {
      Vector2Int cor = GetCordinatesFromWorldPos(TransformTraked.position);
      TerrainCell cell = GetCell(cor);
      if (Path.Contains(cell)) ChangeCurrentIndex( Path.IndexOf(cell));
   }

   private void ChangeCurrentIndex(int newIndex)
   {
      if (_currentIndex == newIndex) return;
      List<TerrainCell> currentCells = new List<TerrainCell>();
      if(_currentIndex-3>=0) currentCells.Add( Path[_currentIndex-3]);
      if(_currentIndex-2>=0) currentCells.Add( Path[_currentIndex-2]);
      if(_currentIndex-1>=0) currentCells.Add( Path[_currentIndex-1]); 
      currentCells.Add( Path[_currentIndex]);
      if(_currentIndex+3<Path.Count) currentCells.Add( Path[_currentIndex+3]);
      if(_currentIndex+2<Path.Count) currentCells.Add( Path[_currentIndex+2]);
      if(_currentIndex+1<Path.Count) currentCells.Add( Path[_currentIndex+1]);

      _currentIndex = newIndex;
      List<TerrainCell> newList = new List<TerrainCell>();
      if(_currentIndex-3>=0) newList.Add( Path[_currentIndex-3]);
      if(_currentIndex-2>=0) newList.Add( Path[_currentIndex-2]);
      if(_currentIndex-1>=0) newList.Add( Path[_currentIndex-1]); 
      newList.Add( Path[_currentIndex]);
      if(_currentIndex+3<Path.Count) newList.Add( Path[_currentIndex+3]);
      if(_currentIndex+2<Path.Count) newList.Add( Path[_currentIndex+2]);
      if(_currentIndex+1<Path.Count) newList.Add( Path[_currentIndex+1]);
      
      foreach (var cell in currentCells) { 
         if(!newList.Contains(cell)) Destroy(cell.Tile);
      }

      foreach (var cell in newList) {
         if(!currentCells.Contains(cell))GenerateTiles(cell);
      }
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

   private void  SetTileType(List<TerrainCell> cells)
   {
      for (int i = 0; i < cells.Count; i++)
      {
         TerrainCell prev;
         TerrainCell current;
         TerrainCell Next;

         if (i == 0) prev = null;
         else prev = cells[i - 1];
         current = cells[i];
         if (i + 1 >= cells.Count) Next = null;
         else Next = cells[i + 1];

         if (prev == null) {
            current.EnterDirection = Next.Pos - current.Pos;
            current.Type = TerrainCell.RoadType.Start;
            continue;
         }
         
         
         current.EnterDirection = current.Pos - prev.Pos;
         if (Next == null) {
            current.Type = TerrainCell.RoadType.End;
            continue;
         }

         current.Type = GetRoadType(current, Next);
      }
   }

   private TerrainCell.RoadType GetRoadType(TerrainCell from, TerrainCell to)
   {
      Vector2Int dir = to.Pos - from.Pos;
      if (from.EnterDirection == Vector2Int.up) {
         if (dir ==Vector2Int.up) return TerrainCell.RoadType.Straingt;
         if (dir ==Vector2Int.left) return TerrainCell.RoadType.Left;
         if (dir == Vector2Int.right) return TerrainCell.RoadType.Right;
      }

      if (from.EnterDirection == Vector2Int.right)
      {
         if (dir == Vector2Int.right) return TerrainCell.RoadType.Straingt;
         if (dir == Vector2Int.down) return TerrainCell.RoadType.Right;
         if (dir == Vector2Int.up) return TerrainCell.RoadType.Left;
      }

      if (from.EnterDirection == Vector2Int.down)
      {
         if (dir == Vector2Int.down) return TerrainCell.RoadType.Straingt;
         if (dir == Vector2Int.right) return TerrainCell.RoadType.Left;
         if (dir == Vector2Int.left) return TerrainCell.RoadType.Right;
      }

      if (from.EnterDirection == Vector2Int.left)
      {
         if (dir == Vector2Int.left) return TerrainCell.RoadType.Straingt;
         if (dir == Vector2Int.up) return TerrainCell.RoadType.Right;
         if (dir == Vector2Int.down) return TerrainCell.RoadType.Left;
      }

      Debug.LogWarning("dir is Invalide =>"+dir);
      return TerrainCell.RoadType.End;
   }

   [ContextMenu("GenerateTerrain")]
   public void GenerateTerrain()
   {
      SetCells();
      UpdatePerlinNoise();
      PickStartAndEnd();
      List<TerrainCell> tiles = AStart();
      SetTileType(tiles);
      //foreach (var tile in tiles) {
      //   GenerateTiles(tile);
      //}
      //tiles[0].Tile.SetActive(true);
      GenerateTiles(tiles[0]);
      GenerateTiles(tiles[1]);
      GenerateTiles(tiles[2]);
      GenerateTiles(tiles[3]);
      if (TransformTraked) {
         TransformTraked.position = tiles[0].Tile.transform.position+new Vector3(0,5,0);
         TransformTraked.forward = tiles[1].Tile.transform.position - tiles[0].Tile.transform.position;
      }
      _currentIndex = 0;
      Path = tiles;
   }

   private void GenerateTiles(TerrainCell cell) {
      GameObject road = cell.Type switch
      {
         TerrainCell.RoadType.Start => PrefabTile,
         TerrainCell.RoadType.End => PrefabTile,
         TerrainCell.RoadType.Straingt => PrefabsStraight[Random.Range(0, PrefabsStraight.Length)],
         TerrainCell.RoadType.Left => PrefabsLeft[Random.Range(0, PrefabsLeft.Length)],
         TerrainCell.RoadType.Right => PrefabsRight[Random.Range(0, PrefabsRight.Length)],
         _ => throw new ArgumentOutOfRangeException()
      };

      cell.Tile = Instantiate(road, new Vector3(cell.Pos.x * TileScale, 0, cell.Pos.y * TileScale), Quaternion.identity);
      cell.Tile.transform.SetParent(transform);
      cell.Tile.transform.forward = new Vector3(cell.EnterDirection.x, 0, cell.EnterDirection.y);
      
   }
}
