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

    public List<GameObject> allDrops = new List<GameObject>();
    public List<TileBase> allTiles = new List<TileBase>();


    void Awake()
    {

    }

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
        float dropSpawnAdjustmentX = Random.Range(0.2f, 0.3f);
        float dropSpawnAdjustmentY = Random.Range(0f, 0.5f);

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
        Vector3 coordinateVariation = new Vector3((mouseWorldPos.x + dropSpawnAdjustmentX), (mouseWorldPos.y + dropSpawnAdjustmentY));

        if (Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;

            if (timer > waitTime)
            {
                timer = 0;
                RaycastHit2D destroyHit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, Terrain);
                {
                    if (Vector3.Distance(character.position, coordinate) < rangeMine)
                    {
                        if (destroyHit.collider != null)
                        {
                            if (tilemap.GetTile(coordinate) == allTiles[0])
                            {
                                Instantiate(allDrops[0], coordinateVariation, Quaternion.identity);
                            }
                            else if (tilemap.GetTile(coordinate) == allTiles[1])
                            {
                                Instantiate(allDrops[1], coordinateVariation, Quaternion.identity);
                            }
                            else if (tilemap.GetTile(coordinate) == allTiles[2])
                            {
                                Instantiate(allDrops[2], coordinateVariation, Quaternion.identity);
                            }
                            else if (tilemap.GetTile(coordinate) == allTiles[3])
                            {
                                Instantiate(allDrops[3], coordinateVariation, Quaternion.identity);
                            }
                            else if (tilemap.GetTile(coordinate) == allTiles[4])
                            {
                                Instantiate(allDrops[4], coordinateVariation, Quaternion.identity);
                            }
                            else if (tilemap.GetTile(coordinate) == allTiles[5])
                            {
                                Instantiate(allDrops[5], coordinateVariation, Quaternion.identity);
                            }
                            else
                            {
                                Debug.Log("not working");
                            }
                            tilemap.SetTile(coordinate, null);
                        }
                    }
                }
            }
        }
    }
}

public class BlockSystem1_2
{
    public int dropID;
    public string dropName;

    public BlockSystem1_2(int id, string myName)
    {
        dropID = id;
        dropName = myName;
    }
}