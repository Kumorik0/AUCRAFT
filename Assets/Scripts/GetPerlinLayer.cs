using UnityEngine;
using AccidentalNoise;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GetPerlinLayer : MonoBehaviour
{

    public TileBase defaultTile;
    public float threshold = 0.5f;
    public int populateCount = 5;

    public List<Vector3Int> GetFractalCoords(int width, int height, uint seed)
    {
        double nx, ny;

        ModuleBase combinedTerrain = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 6, 2, seed);
        List<Vector3Int> fractalCoords = new List<Vector3Int>();
        SMappingRanges ranges = new SMappingRanges();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nx = (ranges.mapx0 + ((double)x / (double)width) * (ranges.mapx1 - ranges.mapx0)) * 3;
                ny = (ranges.mapy0 + ((double)y / (double)height) * (ranges.mapy1 - ranges.mapy0)) * 3;

                if (combinedTerrain.Get(nx, ny) > threshold)
                {
                    fractalCoords.Add(new Vector3Int(x, height - y, 0));
                }
            }
        }

        return fractalCoords;
    }
}
