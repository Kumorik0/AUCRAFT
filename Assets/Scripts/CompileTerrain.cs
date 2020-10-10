using UnityEngine;
using AccidentalNoise;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

public class CompileTerrain : MonoBehaviour
{

    public TileBase dirtTile;
    public TileBase grassTile;
    public TileBase stoneTile;

    public List<GameObject> fractalLayers = new List<GameObject>();

    public Tilemap grid;
    public int width;
    public int height;

    public float seed;
    public int caveSmoothness = 2;

    void Start()
    {
        grid.ClearAllTiles();

        int touchCount = 0;
        Vector3Int newPos = new Vector3Int(0, 0, 0);
        Vector3Int curPos = new Vector3Int(0, 0, 0);
        double nx, ny;

        ModuleBase combinedTerrain = CavesAndMountains((uint)seed);
        List<Vector3Int> terrainCoords = new List<Vector3Int>();
        HashSet<Vector3Int> containCoords = new HashSet<Vector3Int>();
        SMappingRanges ranges = new SMappingRanges();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nx = (ranges.mapx0 + ((double)x / (double)width) * (ranges.mapx1 - ranges.mapx0)) * 3;
                ny = (ranges.mapy0 + ((double)y / (double)height) * (ranges.mapy1 - ranges.mapy0)) * 3;

                if (combinedTerrain.Get(nx, ny) > 0f)
                {
                    terrainCoords.Add(new Vector3Int(x, height - y, 0));
                    containCoords.Add(new Vector3Int(x, height - y, 0));
                }
            }
        }

        List<Tuple<int, int>> neighbors = new List<Tuple<int, int>>() {Tuple.Create(1, 1), Tuple.Create(-1, -1),
                                                                       Tuple.Create(0, 1), Tuple.Create(1, 0),
                                                                       Tuple.Create(0, -1), Tuple.Create(-1, 0),
                                                                       Tuple.Create(-1, 1), Tuple.Create(1, -1)};
        for (int repeat = 0; repeat < 3; repeat++)
        {
            for (int index = 0; index < terrainCoords.Count; index++)
            {
                if (index == terrainCoords.Count)
                {
                    break;
                }

                touchCount = 0;

                for (int posAdd = 0; posAdd < neighbors.Count && touchCount < 2; posAdd++)
                {
                    newPos.x = terrainCoords[index].x + neighbors[posAdd].Item1;
                    newPos.y = terrainCoords[index].y + neighbors[posAdd].Item2;
                    touchCount += containCoords.Contains(newPos) ? 1 : 0;
                }

                if (touchCount < 2)
                {
                    containCoords.Remove(terrainCoords[index]);
                    terrainCoords.Remove(terrainCoords[index]);
                }

            }
        }

        for (int j = 0; j < caveSmoothness; j++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    curPos.x = x;
                    curPos.y = y;

                    if (!containCoords.Contains(curPos))
                    {
                        touchCount = 0;

                        for (int posAdd = 0; posAdd < neighbors.Count; posAdd++)
                        {
                            newPos.x = x + neighbors[posAdd].Item1;
                            newPos.y = y + neighbors[posAdd].Item2;
                            touchCount += containCoords.Contains(newPos) ? 1 : 0;
                        }

                        if (touchCount >= 5)
                        {
                            terrainCoords.Add(curPos);
                            containCoords.Add(curPos);
                        }

                    }

                }

            }
        }

        terrainCoords.Sort((x, y) => x.x == y.x ? x.y.CompareTo(y.y) : x.x.CompareTo(y.x));
        terrainCoords.Reverse();

        TileBase selectedTile;
        int depth = 0;
        int lastx = 0;
        int lasty = terrainCoords[0].y + 1;

        foreach (Vector3Int blck in terrainCoords)
        {
            depth = blck.x != lastx ? 0 : depth;
            lasty = blck.x != lastx ? blck.y + 1 : lasty;

            selectedTile = depth < 1 ? grassTile : stoneTile;
            selectedTile = 0 < depth && depth < 15 ? dirtTile : selectedTile;

            grid.SetTile(blck, selectedTile);

            lastx = blck.x;
            depth += lasty - blck.y;
            lasty = blck.y;
        }

        int layerNum = 1;
        List<Vector3Int> posList = new List<Vector3Int>();

        foreach (GameObject layer in fractalLayers)
        {
            GetPerlinLayer component = layer.GetComponent<GetPerlinLayer>();

            for (int k = 0; k < 5; k++)
            {
                layerNum++;
                foreach (Vector3Int pos in component.GetFractalCoords(width, height, (uint)(seed * layerNum)))
                    if (grid.GetTile(pos) != null && grid.GetTile(pos) != grassTile && grid.GetTile(pos) != dirtTile)
                    {
                        grid.SetTile(pos, component.defaultTile);
                    }
            }
        }
    }

    public static ModuleBase CavesAndMountains(uint seed)
    {
        AccidentalNoise.Gradient ground_gradient = new AccidentalNoise.Gradient(0, 0, 0, 1);

        // lowlands
        Fractal lowland_shape_fractal = new Fractal(FractalType.BILLOW, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, .25, seed);
        AutoCorrect lowland_autocorrect = new AutoCorrect(lowland_shape_fractal, 0, 1);
        ScaleOffset lowland_scale = new ScaleOffset(0.125, -0.45, lowland_autocorrect);
        ScaleDomain lowland_y_scale = new ScaleDomain(lowland_scale, null, 0);
        TranslatedDomain lowland_terrain = new TranslatedDomain(ground_gradient, null, lowland_y_scale);

        // highlands
        Fractal highland_shape_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, 2, seed);
        AutoCorrect highland_autocorrect = new AutoCorrect(highland_shape_fractal, -1, 1);
        ScaleOffset highland_scale = new ScaleOffset(0.25, 0, highland_autocorrect);
        ScaleDomain highland_y_scale = new ScaleDomain(highland_scale, null, 0);
        TranslatedDomain highland_terrain = new TranslatedDomain(ground_gradient, null, highland_y_scale);

        // mountains
        Fractal mountain_shape_fractal = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 3, 1, seed);
        AutoCorrect mountain_autocorrect = new AutoCorrect(mountain_shape_fractal, -1, 1);
        ScaleOffset mountain_scale = new ScaleOffset(0.3, 0.15, mountain_autocorrect);
        ScaleDomain mountain_y_scale = new ScaleDomain(mountain_scale, null, 0.15);
        TranslatedDomain mountain_terrain = new TranslatedDomain(ground_gradient, null, mountain_y_scale);

        // terrain
        Fractal terrain_type_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 2, 0.125, seed);
        AutoCorrect terrain_autocorrect = new AutoCorrect(terrain_type_fractal, 0, 1);
        ScaleDomain terrain_type_y_scale = new ScaleDomain(terrain_autocorrect, null, 0);
        AccidentalNoise.Cache terrain_type_cache = new AccidentalNoise.Cache(terrain_type_y_scale);
        Select highland_mountain_select = new Select(terrain_type_cache, highland_terrain, mountain_terrain, 1, .5);
        Select highland_lowland_select = new Select(terrain_type_cache, lowland_terrain, highland_mountain_select, 0.25, 0.15);
        AccidentalNoise.Cache highland_lowland_select_cache = new AccidentalNoise.Cache(highland_lowland_select);
        Select ground_select = new Select(highland_lowland_select_cache, 0, 1, 0.5, null);

        // caves
        Fractal cave_shape = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, 4, seed);
        Bias cave_attenuate_bias = new Bias(highland_lowland_select_cache, 0.55);
        Combiner cave_shape_attenuate = new Combiner(CombinerTypes.MULT, cave_shape, cave_attenuate_bias);
        Fractal cave_perturb_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 6, 3, seed);
        ScaleOffset cave_perturb_x_scale = new ScaleOffset(0.75, 100, cave_perturb_fractal);
        ScaleOffset cave_perturb_y_scale = new ScaleOffset(1, 0, cave_perturb_fractal);
        TranslatedDomain cave_perturb = new TranslatedDomain(cave_shape_attenuate, cave_perturb_x_scale, cave_perturb_y_scale);
        Select cave_select = new Select(cave_perturb, 1, 0, 0.85, 0);

        return new Combiner(CombinerTypes.MULT, cave_select, ground_select) as ModuleBase;
    }
}