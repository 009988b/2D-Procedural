using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    Gradient grad;
    public GameObject landPrefab;
    public GameObject worldObject;
    private GameObject spawn;
    private int maxX = 64;
    private int maxY = 64;
    float[,] heightMap;
    public int seed;
    // Start is called before the first frame update
    void Awake()
    {
        Draw();
    }
    void Start()
    {
    }
    void Draw()
    {
        float w = landPrefab.transform.lossyScale.x * 2;
        float h = landPrefab.transform.lossyScale.y * 2;
        heightMap = CalcNoise(maxX, maxY, 0.03f, seed);
        for (int xi = 0; xi < heightMap.GetLength(0); xi++)
        {

            for (int yi = 0; yi < heightMap.GetLength(1); yi++)
            {
                spawn = GameObject.Instantiate(landPrefab, new Vector3((float)(xi * w), (float)(yi * h), 0f), new Quaternion(0, 0, 0, 0), worldObject.transform);
                string n = "[" + xi + "," + yi + "]";
                spawn.name = n;
                spawn.GetComponent<Transform>().transform.localScale += 0.25f * spawn.transform.localScale;
                spawn.AddComponent<Terrain>();
                Terrain terrain = spawn.GetComponent<Terrain>();
                terrain.SetElevation(heightMap[xi, yi]);
                terrain.SetCoords(xi, yi);
                terrain.AssignType();
                SpriteRenderer sr = spawn.GetComponent<SpriteRenderer>();
                sr.color = grad.Evaluate(heightMap[xi, yi]);
            }
        }
        UpdateAdjacency();
    }
    void UpdateAdjacency()
    {
        foreach (Transform child in worldObject.transform)
        {
            GameObject g = child.gameObject;
            Terrain terrain = child.GetComponent<Terrain>();
            terrain.findAdjacents(worldObject);
            terrain.checkIfCorner();
        }
    }
    public float[,] CalcNoise(int width, int height, float scale, int seed)
    {
        //Map[x,y] -> noise val.
        //For chunk rendering
        //Calculate same seed just new x,y coords
        float[,] map = new float[width, height];
        for (int xi = 0; xi < width; xi++)
        {
            for (int yi = 0; yi < height; yi++)
            {
                float x = xi * scale;
                float y = yi * scale;
                float noise = Mathf.PerlinNoise(seed + x, seed + y);
                map[xi, yi] = noise;
                string test = x + "," + y + " " + noise;
            }
        }
        return map;
    }
}
