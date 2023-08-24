using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFog : MonoBehaviour
{
    private Vector3 startingposition;
    private float counter;

    // Start is called before the first frame update
    void Start()
    {
        startingposition = transform.position;
        counter = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (counter >= -1)
        {
            transform.position = Vector3.Lerp(startingposition, startingposition + new Vector3(5, 5, 0), Mathf.Abs(counter));
            counter -= (Time.deltaTime / 10);
        }
        else
        {
            counter = 1;
        }
    }
}
