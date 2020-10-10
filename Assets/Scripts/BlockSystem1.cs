using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockSystem1 : MonoBehaviour
{

    public GameObject selectObject;
    public Grid grid;
    public Camera cam;

    public Rigidbody2D character;

    public Tilemap tilemap;

    public LayerMask Terrain;

    float waitTime = 0.3f;
    float timer = 0.0f;
    float rangeMine = 8.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float newPosX = Mathf.Round(cam.ScreenToWorldPoint(Input.mousePosition).x);
        float newPosY = Mathf.Round(cam.ScreenToWorldPoint(Input.mousePosition).y);
        selectObject.transform.position = new Vector2(newPosX, newPosY);

        if (Input.GetMouseButton(0))
        {


            timer += Time.deltaTime;

            if (timer > waitTime)
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
                timer = 0;
                RaycastHit2D destroyHit = Physics2D.Raycast(selectObject.transform.position, Vector2.zero, Mathf.Infinity, Terrain);

                if (Vector3.Distance(character.position, coordinate) < rangeMine)
                    {
                    if (destroyHit.collider != null)
                    {
                        tilemap.SetTile(coordinate, null);
                    }
                }
            }
        }
    }
}