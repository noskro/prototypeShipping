using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DemoController : MonoBehaviour
{
    public Tilemap tilemapFOW;

    // Start is called before the first frame update
    void Start()
    {
        tilemapFOW.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
