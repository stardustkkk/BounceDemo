using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private Vector2 center;
    private int size;
    private bool _isObstacle;

    public bool isObstacle {
        get => _isObstacle;
    }

    Color originalColor;

    private void Start() 
    {
        originalColor = gameObject.GetComponent<Renderer>().material.color;
        this.center = new Vector2(transform.position.x, transform.position.z);
    }

    public void SetObstacle(bool isOb)         
    {
        _isObstacle = isOb;        
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", isOb ? Color.red : originalColor);
    }

    public void SetGridSize(int size)
    {
        this.size = size;
    }

    public Vector2? FindNearestIntersection(BabyRay ray, out Vector2Int gridOffset)
    {
        // use ray instead of segment to find intersection point, and choose the nearer one as the intersection edge
        Vector2 halfX = new Vector2(size / 2.0f, 0);
        Vector2 halfY = new Vector2(0, size / 2.0f);

        gridOffset = new Vector2Int(0, 0);
        int vOffset = 0;
        int hOffset = 0;
        Vector2? resBT = null;
        Vector2? resLR = null;

        if (ray.dir.x > 0) 
        {
            // right
            // Debug.Log($"try right edge {center + halfX + halfY}  {center + halfX - halfY} {ray.origin} {ray.origin + ray.dir}");
            BabyRay rightEdge = new BabyRay(center + halfX + halfY, -halfY * 2);
            resLR = ray.IntersectWithRay(rightEdge);
            if (resLR != null) 
            {   
                hOffset = 1;
            }
        } else if (ray.dir.x < 0)
        {
            // left
            // Debug.Log($"try left edge {center - halfX + halfY}  {center - halfX - halfY} {ray.origin} {ray.origin + ray.dir}");
            BabyRay leftEdge = new BabyRay(center - halfX + halfY, -halfY * 2);
            resLR = ray.IntersectWithRay(leftEdge);
            if (resLR != null) 
            {   
                hOffset = -1;                
            }            
        }

        if (ray.dir.y > 0)
        {
            // top
            // Debug.Log($"try top edge {center - halfX + halfY}  {center + halfX + halfY} {ray.origin} {ray.origin + ray.dir}");
            BabyRay topEdge = new BabyRay(center - halfX + halfY, halfX * 2);
            resBT = ray.IntersectWithRay(topEdge);
            if (resBT != null) 
            {   
                vOffset = 1;
            }
        } else if (ray.dir.y < 0)
        {   
            // bottom
            // Debug.Log($"try bottom edge {center - halfX - halfY}  {center + halfX - halfY} {ray.origin} {ray.origin + ray.dir}");
            BabyRay bottomEdge = new BabyRay(center - halfX - halfY, halfX * 2);
            resBT = ray.IntersectWithRay(bottomEdge);
            if (resBT != null) 
            {   
                vOffset = -1;
            }                        
        }       

        Debug.LogWarning($"cannot find intersection, possibly beacause of float-precision, try ray instead -------------- {resBT} {resLR}");
    
        if (resBT != null && resLR == null)
        {
            gridOffset.y = vOffset;
            return resBT;
        }

        if (resBT == null && resLR != null)
        {
            gridOffset.x = hOffset;
            return resLR;
        }

        if (resBT != null && resLR != null)
        {
            Debug.LogWarning($"-------------- magnitude {(resBT.Value - ray.origin).magnitude} {(resLR.Value - ray.origin).magnitude}");
            if ((resBT.Value - ray.origin).magnitude < (resLR.Value - ray.origin).magnitude)
            {
                gridOffset.y = vOffset;
                return resBT;
            }
            gridOffset.x = hOffset;
            return resLR;
        }
        return null;        
    }

    public Vector2? IntersectWithRay(BabyRay ray, out Vector2Int gridOffset)
    {
        Vector2 halfX = new Vector2(size / 2.0f, 0);
        Vector2 halfY = new Vector2(0, size / 2.0f);

        gridOffset = new Vector2Int(0, 0);

        if (ray.dir.x > 0) 
        {
            // right
            // Debug.Log($"try right edge {center + halfX + halfY}  {center + halfX - halfY} {ray.origin} {ray.origin + ray.dir}");
            BabyRay rightEdge = new BabyRay(center + halfX + halfY, -halfY * 2);
            Vector2? res = ray.IntersectWithRay(rightEdge, true);
            if (res != null) 
            {   
                gridOffset.x = 1; 
                gridOffset.y = 0;
                return res;
            }
        } else if (ray.dir.x < 0)
        {
            // left
            // Debug.Log($"try left edge {center - halfX + halfY}  {center - halfX - halfY} {ray.origin} {ray.origin + ray.dir}");
            BabyRay leftEdge = new BabyRay(center - halfX + halfY, -halfY * 2);
            Vector2? res = ray.IntersectWithRay(leftEdge, true);
            if (res != null) 
            {   
                gridOffset.x = -1;
                gridOffset.y = 0;
                return res;
            }            
        }

        if (ray.dir.y > 0)
        {
            // top
            // Debug.Log($"try top edge {center - halfX + halfY}  {center + halfX + halfY} {ray.origin} {ray.origin + ray.dir}");
            BabyRay topEdge = new BabyRay(center - halfX + halfY, halfX * 2);
            Vector2? res = ray.IntersectWithRay(topEdge, true);
            if (res != null) 
            {   
                gridOffset.x = 0;
                gridOffset.y = 1;
                return res;
            }
        } else if (ray.dir.y < 0)
        {   
            // bottom
            // Debug.Log($"try bottom edge {center - halfX - halfY}  {center + halfX - halfY} {ray.origin} {ray.origin + ray.dir}");
            BabyRay bottomEdge = new BabyRay(center - halfX - halfY, halfX * 2);
            Vector2? res = ray.IntersectWithRay(bottomEdge, true);
            if (res != null) 
            {   
                gridOffset.x = 0;
                gridOffset.y = -1;
                return res;
            }                        
        }

        // handle BabyRay's possible float-precision problem
        return FindNearestIntersection(ray, out gridOffset);
    }
}