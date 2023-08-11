using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToObject : MonoBehaviour
{
    [SerializeField] private GameObject target; 
    [SerializeField] private float speed; 

    void Update()
    {
        Vector3 direction = target.transform.position - transform.position;

/*        if (Vector3.Dot(direction, transform.forward) > 0)
        {*/
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        //}
    }
}
