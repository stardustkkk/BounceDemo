using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    // Start is called before the first frame update
    public float destoryDelay;
    void Start()
    {
        StartCoroutine("SelfDestruction");
    }

    IEnumerator SelfDestruction()
    {
        yield return new WaitForSeconds(destoryDelay);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
