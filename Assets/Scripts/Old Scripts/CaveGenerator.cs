using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : MonoBehaviour
{

    public int width;
    public int height;
    public int smoothCycles;

    public GameObject stone;

    int[,] cavePoints;

    [Range(0, 100)]
    public int randFillPercent;
    [Range(0, 8)]
    public int threshhold;

    void Awake()
    {
        GenerateCave();
    }

    void Start()
    {
        PlaceGrid();
    }

    void GenerateCave()
    {
        cavePoints = new int[width, height];

        int seed = Random.Range(0, 100000);
        System.Random randChoice = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    cavePoints[x, y] = 1;
                }
                else if (randChoice.Next(0, 100) < randFillPercent)
                {
                    cavePoints[x, y] = 1;
                }
                else
                {
                    cavePoints[x, y] = 0;
                }
            }
        }

        for (int i = 0; i < smoothCycles; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int neighboringWalls = GetNeighbors(x, y);

                    if (neighboringWalls > threshhold)
                    {
                        cavePoints[x, y] = 1;
                    }
                    else if (neighboringWalls < threshhold)
                    {
                        cavePoints[x, y] = 0;
                    }
                }
            }
        }
    }

    int GetNeighbors(int pointX, int pointY)
    {
        int wallNeighbors = 0;
        for (int x = pointX - 1; x <= pointX + 1; x++)
        {
            for (int y = pointY - 1; y <= pointY + 1; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    if (x != pointX || y!= pointY)
                    {
                        if (cavePoints[x,y] == 1)
                        {
                            wallNeighbors++;
                        }
                    }
                }
                else
                {
                    wallNeighbors++;
                }
            }
        }
        return wallNeighbors;
    }

    private void PlaceGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cavePoints[x,y] == 1)
                { 
                    Instantiate(stone, new Vector2(x, y), Quaternion.identity);
                }
            }
        }
    }
}
