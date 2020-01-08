using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    /* Probably should have used inheritance here
     */
    private static Texture2D sand;
    private static Sprite[] sand_;
    private static Texture2D shallows;
    private static Sprite[] shallows_;
    private static Texture2D ocean;
    private static Sprite[] ocean_;
    private static Texture2D grass;
    private static Sprite[] grass_;
    private static Texture2D oceanedge;
    private static Sprite[] oceanedge_;
    public string type;
    public float elev;
    int[] coords = new int[2];
    public static GameObject world;
    static SpriteRenderer sr;
    public Dictionary<string, GameObject> adjacents = new Dictionary<string, GameObject>();
    public string adjacentInfo;
    public string shouldBeA;
    public Boolean isCorner = false;
    void Awake() {
        world = GameObject.FindGameObjectWithTag("world");
        sr = gameObject.GetComponent<SpriteRenderer>();
        sand = Resources.Load<Texture2D>("sand");
        sand_ = Resources.LoadAll<Sprite>(sand.name);
        shallows = Resources.Load<Texture2D>("shallows");
        shallows_ = Resources.LoadAll<Sprite>(shallows.name);
        ocean = Resources.Load<Texture2D>("ocean");
        ocean_ = Resources.LoadAll<Sprite>(ocean.name);
        oceanedge = Resources.Load<Texture2D>("oceanedge");
        oceanedge_ = Resources.LoadAll<Sprite>(oceanedge.name);
        grass = Resources.Load<Texture2D>("grass");
        grass_ = Resources.LoadAll<Sprite>(grass.name);
    }
    public Terrain() {
        checkIfCorner();
    }
    public void AssignType() {
        if (elev < 0.648f)
        {
            type = "ocean";
        }
        else if (elev >= 0.648f && elev < 0.7f)
        {
            type = "shallows";
        }
        else if (elev >= 0.7f && elev < 0.75f)
        {
            type = "sand";
        }
        else if (elev >= 0.75f && elev < 1.0f)
        {
            type = "veg";
        }
        else {
            type = "bugged";
        }
    }
    public void SetCoords(int x, int y) {
        this.coords[0] = x;
        this.coords[1] = y;
    }
    public void SetElevation(float e)
    {
        this.elev = e;
    }
    void debug_AdjacentInfo() {
        string s = "";
        try
        {
            foreach (KeyValuePair<string, GameObject> x in adjacents)
            {
                s += x.Value.name + "=" + x.Value.GetComponent<Terrain>().type + "adj: " + x.Key + "corner?"+x.Value.GetComponent<Terrain>().isCorner+"\n";
            }
        }
        catch (KeyNotFoundException e) {
            Debug.Log("Key not found. WTF");
        }
        
        adjacentInfo = s;
    }
    public void findAdjacents(GameObject worldObject) {
        foreach (Transform child in worldObject.transform)
        {
            GameObject go = child.gameObject;
            Terrain terrain = go.GetComponent<Terrain>();
            if (terrain.coords[0] == this.coords[0] - 1 && terrain.coords[1] == this.coords[1])
            {
                adjacents["left"] = go;
            }
            if (terrain.coords[0] == this.coords[0] + 1 && terrain.coords[1] == this.coords[1])
            {
                adjacents["right"] = go;
            }
            if (terrain.coords[0] == this.coords[0] && terrain.coords[1] == this.coords[1] - 1)
            {
                adjacents["bottom"] = go;
            }
            if (terrain.coords[0] == this.coords[0] && terrain.coords[1] == this.coords[1] + 1)
            {
                adjacents["top"] = go;
            }
        }
    }
    public void checkIfCorner() {
        string right = "";
        string left = "";
        string bot = "";
        string top = "";
        try
        {
            right = adjacents["right"].GetComponent<Terrain>().type;
            left = adjacents["left"].GetComponent<Terrain>().type;
            bot = adjacents["bottom"].GetComponent<Terrain>().type;
            top = adjacents["top"].GetComponent<Terrain>().type;
        } catch (KeyNotFoundException e) {
        }
        switch (type) {
            case "veg":
                try
                {
                    if (top == "sand")
                    {
                        if (right == "sand")
                        {
                            isCorner = true;
                            break;
                        }
                        if (left == "sand")
                        {
                            isCorner = true;
                            break;
                        }
                    }
                    if (bot == "sand")
                    {
                        if (right == "sand")
                        {
                            isCorner = true;
                            break;
                        }
                        if (left == "sand")
                        { 
                            isCorner = true;
                            break;
                        }
                    }
                    break;
                }
                catch (NullReferenceException e)
                {
                }
                break;
            case "sand":
                try
                {
                    if (right == "sand")
                    {
                        if (top == "shallows" && left == "shallows")
                        {
                            isCorner = true;
                            break;
                        }
                        if (bot == "shallows" && left == "shallows")
                        {
                            isCorner = true;
                            break;
                        }
                    }
                    if (left == "sand")
                    {
                        if (top == "shallows" && right == "shallows")
                        {
                            isCorner = true;
                            break;
                        }
                        if (bot == "shallows" && right == "shallows")
                        {
                            isCorner = true;
                            break;
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                }
                break;
            case "ocean":
                sr.sprite = ocean_[4];
                shouldBeA = "ocean-waters";
                try
                {
                    if (bot == "shallows" && left == "shallows")
                    {
                        sr.sprite = ocean_[6];
                        shouldBeA = "uprightfacing";
                        isCorner = true;
                        break;
                    }
                    if (bot == "shallows" && right == "shallows")
                    {
                        sr.sprite = ocean_[8];
                        shouldBeA = "upleftfacing";
                        isCorner = true;
                        break;
                    }
                    if (top == "shallows" && left == "shallows")
                    {
                        isCorner = true;
                        break;
                    }
                    if (top == "shallows" && right == "shallows")
                    {
                        isCorner = true;
                        break;
                    }
                }
                catch (NullReferenceException e)
                {
                }
                break;
        }
    }
    public void ApplyTexture()
    {
        string right = "";
        string left = "";
        string bot = "";
        string top = "";
        GameObject[] vertically_adj = new GameObject[2];
        GameObject[] horizontal_adj = new GameObject[2];
        sr = gameObject.GetComponent<SpriteRenderer>();
        try
        {
            right = adjacents["right"].GetComponent<Terrain>().type;
            left = adjacents["left"].GetComponent<Terrain>().type;
            bot = adjacents["bottom"].GetComponent<Terrain>().type;
            top = adjacents["top"].GetComponent<Terrain>().type;
            vertically_adj[0] = adjacents["top"];
            vertically_adj[1] = adjacents["bot"];
            horizontal_adj[0] = adjacents["right"];
            horizontal_adj[1] = adjacents["left"];
        }
        catch (KeyNotFoundException ex) {
        }
        switch (type)
        {
            case "veg":
                try
                {
                    if (top == "sand")
                    {
                        if (right == "sand")
                        {
                            sr.sprite = grass_[2];
                            break;
                        }
                        if (left == "sand")
                        {
                            sr.sprite = grass_[0];
                            break;
                        }
                    }
                    if (bot == "sand")
                    {
                        if (right == "sand")
                        {
                            sr.sprite = grass_[8];
                            break;
                        }
                        if (left == "sand")
                        {
                            sr.sprite = grass_[6];
                            break;
                        }
                    }
                    if (top == "veg" && bot == "veg")
                    {
                        if (right == "sand")
                        {
                            sr.sprite = grass_[5];
                            break;
                        }
                        if (left == "sand")
                        {
                            sr.sprite = grass_[3];
                            break;
                        }
                    }
                    if (left == "veg" && right == "veg")
                    {
                        if (top == "sand")
                        {
                            sr.sprite = grass_[1];
                            break;
                        }
                        if (bot == "sand")
                        {
                            sr.sprite = grass_[7];
                            break;
                        }
                    }
                    sr.sprite = grass_[4];
                    break;
                }
                catch (NullReferenceException e) { 
                }
                break;
            case "sand":
                try
                {
                    if (right == "sand")
                    {
                        //Top left corner
                        if (top == "shallows" && left == "shallows")
                        {
                            sr.sprite = sand_[0];
                            shouldBeA = "corner";
                            break;
                        }
                        //Top
                        if (left == "sand" && top == "shallows")
                        {
                            sr.sprite = sand_[7];
                            shouldBeA = "Top";
                            break;
                        }
                        //Bottom
                        if (left == "sand" && bot == "shallows")
                        {
                            sr.sprite = sand_[4];
                            shouldBeA = "Bottom";
                            break;
                        }
                        //Left
                        if (bot == "sand" && top == "sand")
                        {
                            sr.sprite = sand_[4];
                            shouldBeA = "Left";
                            break;
                        }
                        //Bottom left corner
                        if (bot == "shallows" && left == "shallows")
                        {
                            sr.sprite = sand_[6];
                            shouldBeA = "corner";
                            break;
                        }
                    }
                    if (left == "sand")
                    {
                        //Top right corner
                        if (top == "shallows" && right == "shallows")
                        {
                            sr.sprite = sand_[2];
                            shouldBeA = "corner";
                            break;
                        }
                        //Bottom right corner
                        if (bot == "shallows" && right == "shallows")
                        {
                            shouldBeA = "corner";
                            sr.sprite = sand_[8];
                            break;
                        }
                        //Right
                        if (bot == "sand" && top == "sand")
                        {
                            shouldBeA = "Right";
                            sr.sprite = sand_[5];
                            break;
                        }
                    }
                    shouldBeA = "inland";
                    sr.sprite = sand_[3];
                    break;
                }
                catch (NullReferenceException e)
                {
                    Debug.Log("Setting base sand texture because adjacent tiles are null..");
                    gameObject.GetComponent<SpriteRenderer>().sprite = sand_[3];
                }
                break;
            case "shallows":
                if (right == "sand")
                {
                    if (!adjacents["right"].GetComponent<Terrain>().isCorner) {
                        sr.sprite = shallows_[5];
                        shouldBeA = "right";
                        break;
                    }
                }
                else if (left == "sand")
                {
                    if (!adjacents["left"].GetComponent<Terrain>().isCorner)
                    {
                        sr.sprite = shallows_[3];
                        shouldBeA = "left";
                        break;
                    }
                }
                if (top == "sand")
                {
                    if (!adjacents["top"].GetComponent<Terrain>().isCorner)
                    {
                        sr.sprite = shallows_[1];
                        shouldBeA = "top";
                        break;
                    }
                }
                else if (bot == "sand")
                {
                    if (!adjacents["bottom"].GetComponent<Terrain>().isCorner)
                    {
                        sr.sprite = shallows_[7];
                        shouldBeA = "bot";
                        break;
                    }
                }
                sr.sprite = shallows_[4];
                shouldBeA = "default-shallows";
                break;
            case "ocean":
                sr.sprite = ocean_[4];
                shouldBeA = "ocean-waters";
                try
                {
                    if (bot == "shallows" && left == "shallows")
                    {
                        sr.sprite = ocean_[6];
                        shouldBeA = "uprightfacing";
                        break;
                    }
                    if (bot == "shallows" && right == "shallows")
                    {
                        sr.sprite = ocean_[8];
                        shouldBeA = "upleftfacing";
                        break;
                    }
                    if (top == "shallows" && left == "shallows")
                    {
                        sr.sprite = ocean_[0];
                        shouldBeA = "downrightfacing";
                        break;
                    }
                    if (top == "shallows" && right == "shallows")
                    {
                        sr.sprite = ocean_[2];
                        shouldBeA = "downleftfacing";
                        break;
                    }
                    //if not corner
                    if (!gameObject.GetComponent<Terrain>().isCorner)
                    {
                        if (right == "shallows")
                        {
                            sr.sprite = ocean_[5];
                            break;
                        }
                        if (left == "shallows")
                        {
                            sr.sprite = ocean_[3];
                            break;
                        }
                        if (top == "shallows")
                        {
                            sr.sprite = ocean_[1];
                            break;
                        }
                        if (bot == "shallows")
                        {
                            sr.sprite = ocean_[7];
                            break;
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                    sr.sprite = ocean_[4];
                    shouldBeA = "ocean-waters";
                    break;
                }
                //edge cases lol pun
                if (adjacents["left"].GetComponent<Terrain>().isCorner & adjacents["left"].GetComponent<Terrain>().shouldBeA == "uprightfacing") {
                    sr.sprite = oceanedge_[1];
                    break;
                }
                if (adjacents["left"].GetComponent<Terrain>().isCorner & adjacents["left"].GetComponent<Terrain>().shouldBeA == "downrightfacing")
                {
                    sr.sprite = oceanedge_[3];
                    break;
                }
                if (adjacents["right"].GetComponent<Terrain>().isCorner & adjacents["right"].GetComponent<Terrain>().shouldBeA == "upleftfacing")
                {
                    sr.sprite = oceanedge_[0];
                    break;
                }
                if (adjacents["right"].GetComponent<Terrain>().isCorner & adjacents["right"].GetComponent<Terrain>().shouldBeA == "downleftfacing")
                {
                    sr.sprite = oceanedge_[2];
                    break;
                }
                if (adjacents["bottom"].GetComponent<Terrain>().shouldBeA == "uprightfacing")
                {
                    sr.sprite = oceanedge_[1];
                    break;
                }
                if (adjacents["bottom"].GetComponent<Terrain>().shouldBeA == "upleftfacing")
                {
                    sr.sprite = oceanedge_[0];
                    break;
                }
                if (adjacents["top"].GetComponent<Terrain>().shouldBeA == "downleftfacing")
                {
                    sr.sprite = oceanedge_[2];
                    break;
                }
                if (adjacents["bottom"].GetComponent<Terrain>().shouldBeA == "downrightfacing")
                {
                    sr.sprite = oceanedge_[3];
                    break;
                }
                break;
        }
    }
    void Start()
    {
        ApplyTexture();
    }
}
