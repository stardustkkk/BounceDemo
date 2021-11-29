using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Its actually a segment not a ray
public struct BabyRay
{
    private Vector2 _origin;
    private Vector2 _dir;
    static float Epsilon = 0.0001f;

    public Vector2 dir 
    {
        get => _dir;
    }

    public Vector2 origin 
    {
        get => _origin;
    }

    public BabyRay(Vector2 origin, Vector2 dir) 
    {
        this._origin = origin;
        this._dir = dir;
    }
    
    public Vector2? IntersectWithRay(BabyRay ray, bool IsSeg=false) 
    {        
        Vector2 origin2 = ray.origin;
        Vector2 dir2 = ray.dir;
        
        float determinant = dir2.x*dir.y - dir.x*dir2.y;

        if (Mathf.Approximately(determinant, 0))
        {
            // Debug.Log($"d is zero {dir} {dir2}");
            return null;
        }

        float d = 1.0f / determinant;

        float t = d * (dir2.y * (origin.x - origin2.x) - dir2.x * (origin.y - origin2.y));
        float t2 = d * (dir.y * (origin.x - origin2.x) - dir.x * (origin.y - origin2.y));

        if (!IsSeg) 
        {
            // Ray
            return origin + dir * (float)t;
        }
    
        // !!!!! This comparions has precision problem, it cannot pass when t == 0 and t2 == 1 in some case
        else if (0f - Epsilon <= t && t <= 1f + Epsilon && 0f - Epsilon <= t2 && t2 <= 1f + Epsilon) 
        {
            // segment
            return origin + dir * (float)t;
        }

        // Debug.Log($"iiiiiiiiii {t.ToString()} {t2.ToString()}");
        return null;
    }

    public static void TestBabyRay()
    {
        BabyRay[] Rays = {
            new BabyRay(new Vector2(0, 0), new Vector2(1, 1)), 
            new BabyRay(new Vector2(0, 0), new Vector2(1, 1)),
            new BabyRay(new Vector2(3, 9), new Vector2(6, 7.5f)),
            new BabyRay(new Vector2(2, 1), new Vector2(12, 9.76f)),            
            new BabyRay(new Vector2(34.8742916231f, 48.8240082724f), new Vector2(1.93746064573f, 2.71244490402f)),
            new BabyRay(new Vector2(-10, -40), new Vector2(0, -40)-new Vector2(-10, -40)),            
        };

        BabyRay[] Ray2s = {
            new BabyRay(new Vector2(0, 1), new Vector2(1, -1)), 
            new BabyRay(new Vector2(1, 1), new Vector2(2, 2)),
            new BabyRay(new Vector2(8, 9.9f), new Vector2(13f, 3.0f)),
            new BabyRay(new Vector2(-1, -100), new Vector2(9, 11f)),
            new BabyRay(new Vector2(0, 0), new Vector2(50, 0)),
            new BabyRay(new Vector2(0f, -40f), new Vector2(-1.7f, -40.7f)- new Vector2(-0, -40)),            
        };

        for (int i = 0 ; i < Rays.Length; i++ )
        {
            Debug.Log($"================={Rays[i].IntersectWithRay(Ray2s[i], true)}");
        }                
    }
}