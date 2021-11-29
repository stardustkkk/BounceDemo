using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct GridPos: System.IEquatable<GridPos> {
    public int row;
    public int col;    

    public GridPos(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public bool Equals(GridPos pos)
    {
        return pos.row == row && pos.col == col;
    }

    public static bool operator ==(GridPos p1, GridPos p2) => p1.Equals(p2);
    public static bool operator !=(GridPos p1, GridPos p2) => !(p1.Equals(p2));    

    public static GridPos operator +(GridPos p1, GridPos p2) => new GridPos(p1.row + p2.row, p1.col + p2.col);
    public static GridPos operator -(GridPos p1, GridPos p2) => new GridPos(p1.row - p2.row, p1.col - p2.col);    
}

public class GridMap: MonoBehaviour
{    
    public int cellSize;
    private int width, height;
    public Transform cellPrefab;
    
    [Header("HitPoint")]
    public Transform hitPointPrefab;
    public bool drawHitPoint;

    private int rows, cols;
    Vector2 origin;
    private GridCell[,] cells;

    // Start is called before the first frame update
    private void Start() 
    {
        Vector3 minCornor = GetComponent<Renderer>().bounds.min;
        origin = new Vector2(minCornor.x, minCornor.z);
        width = (int)GetComponent<Renderer>().bounds.size.x;
        height = (int)GetComponent<Renderer>().bounds.size.z;        

        this.cols = Mathf.FloorToInt(width/cellSize);
        this.rows = Mathf.FloorToInt(height/cellSize);
        InitCells();
        InitObstacles();
    }

    void InitCells()
    {
        cells = new GridCell[this.rows+2, this.cols+2];
        for (int i = 0; i < rows+2; i++)
        {
            for (int j = 0; j < cols+2; j++)
            {
                Transform cellObj = Instantiate(cellPrefab, new Vector3(origin.x + cellSize * (j - 0.5f), 0.1f, origin.y + cellSize * (i - 0.5f)), transform.rotation);
                cellObj.transform.localScale = new Vector3(cellSize-1, 1, cellSize-1);                
                GridCell cell = cellObj.GetComponent<GridCell>();                         
                cell.SetGridSize(cellSize);
                cells[i, j] = cell;
                if (i == 0 || i==rows+1 || j == 0 || j == rows+1) 
                {
                    cell.SetObstacle(true);
                }
            }
        }
    }

    void InitObstacles()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            if (Child.CompareTag("obstacle")) {
                GridPos gridPos = GetGridPosOfCoordPos(new Vector2(Child.position.x, Child.position.z));
                GridCell cell = cells[gridPos.row, gridPos.col];
                cell.SetObstacle(true);
            }
        }
    }

    public GridPos GetGridPosOfCoordPos(Vector2 pos)
    {
        //Debug.Log($"{pos.y - origin.y} {pos.x - origin.x} {origin} {pos}");
        return new GridPos(
            Mathf.FloorToInt((pos.y-origin.y) / cellSize)+1,
            Mathf.FloorToInt((pos.x-origin.x) / cellSize)+1
        );
    }

    public Vector2 GetNextMoveByRay(BabyRay ray, ref Vector2 velocity, out Vector2? hitP)
    {   
        GridPos oldPos = GetGridPosOfCoordPos(ray.origin);
        GridPos newPos = GetGridPosOfCoordPos(ray.origin + ray.dir);
        hitP = null;
        // Debug.Log($"{oldPos.row} {oldPos.col} {newPos.row} {newPos.col}==========");
        while (oldPos != newPos)
        {
            // Debug.Log($"111111  {oldPos.row} {oldPos.col} {newPos.row} {newPos.col}==========");
            GridCell cell = cells[oldPos.row, oldPos.col];            
            Vector2? hitPoint = cell.IntersectWithRay(ray, out Vector2Int gridOffset);
            if (hitPoint != null)
            {
                // y contribute to column, x contribute to row                
                oldPos += new GridPos(gridOffset.y, gridOffset.x);                
                cell = cells[oldPos.row, oldPos.col];
                // Debug.Log($"111111 next  {oldPos.row} {oldPos.col} {newPos.row} {newPos.col}==========");

                hitP = hitPoint;
                if (drawHitPoint) 
                {
                    Instantiate(hitPointPrefab, new Vector3(hitPoint.Value.x, 0, hitPoint.Value.y) , transform.rotation);                
                }
        
                if (cell.isObstacle) 
                {
                    if (gridOffset.y != 0) 
                    {
                        velocity.y = -velocity.y;
                    }                    

                    if (gridOffset.x != 0)
                    {
                        velocity.x = -velocity.x;
                    }
                    
                    return hitPoint.Value;
                }
            } else
            {
                Debug.LogError($"NO HIT POINT!!!!!!");
                break;
            }
        }
        return ray.origin + ray.dir;
    }

    private void OnDrawGizmosSelected() 
    {
        Vector3 minCornor = GetComponent<Renderer>().bounds.min;
        var origin = new Vector2(minCornor.x, minCornor.z);
        Vector3 size = GetComponent<Renderer>().bounds.size;

        int vLines = Mathf.FloorToInt(size.x/cellSize);
        int hLines = Mathf.FloorToInt(size.z/cellSize); 

        UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.5f);
        for (int i = 0; i < vLines; i++) {
            UnityEditor.Handles.DrawLine(
                new Vector3(origin.x + i * cellSize, 0.1f, origin.y),
                new Vector3(origin.x + i * cellSize, 0.1f, origin.y + size.z)
            );
        }

        for (int i = 0; i < hLines; i++) {
            UnityEditor.Handles.DrawLine(
                new Vector3(origin.x, 1f, origin.y + i * cellSize),
                new Vector3(origin.x+size.x, 1f, origin.y + i * cellSize)
            );
        }
    }
}
