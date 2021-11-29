using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    bool moving = false;
    Vector3 velocity;

    GridMap gridMap;
    public int speed = 10; 
    // Start is called before the first frame update

    private GridPos oldGridPos;

    public void SetDir(Vector3 dir)
    {
        if (moving)
            return;
        Vector3 currentPos = new Vector3(transform.position.x, 0, transform.position.z);
        velocity = (dir - currentPos).normalized * speed;
    }    
    void Start()
    {            
        gridMap = GameObject.FindGameObjectWithTag("gridmap").GetComponent<GridMap>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (velocity.magnitude <= 0)
            return;
        moving = true;

        Vector3 newPos = transform.position + velocity * Time.deltaTime;        
        
        Vector2 v2 = new Vector2(velocity.x, velocity.z);
        
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 destPos = new Vector2(newPos.x, newPos.z);

        Vector2 newPosXZ = gridMap.GetNextMoveByRay(new BabyRay(currentPos, destPos - currentPos), ref v2, out Vector2? hitPoint);
        newPos.x = newPosXZ.x;
        newPos.z = newPosXZ.y;
        velocity.x = v2.x;
        velocity.z = v2.y;

        transform.position = newPos;
    }
}
