using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public Transform grid; 
    public GameObject bulletPrefab;

    public Vector3 initPos;
    GridMap gridMap;
    // Start is called before the first frame update
    void Start()
    {

        BabyRay.TestBabyRay();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
         GUI.Label(new Rect(0, 0, 100, 100), ((int)(1.0f / Time.smoothDeltaTime)).ToString(), style);
    }
    private void SetDir() 
    {
        Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        var bullet = Instantiate(bulletPrefab, initPos, transform.rotation);
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity)) {
            bullet.GetComponent<Bullet>().SetDir(new Vector3(hit.point.x, 0, hit.point.z));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            SetDir();
        }       
    }
}
