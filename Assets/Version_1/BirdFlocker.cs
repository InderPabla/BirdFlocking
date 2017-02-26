using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BirdFlocker : MonoBehaviour {

    private int pixWidth = 170;
    private int pixHeight = 100;
    private float xOrg = 0;
    private float yOrg = 0;
    private float xScale = 10f;
    private float yScale = 10f;
    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;




    public GameObject birdPrefab;
    public int numberOfBirds = 100;

    //private GameObject[] birds;
    private List<GameObject> birds;
    private BirdActivationData data;
    private const string ACTIVATE_WITH_BIRD_DATA = "ActivateWithBirdData";
    private ColliderMap map;

    //private GameObject[,] walls = new GameObject[4,4];
    // Use this for initialization

    float fps = 60f;
    private bool hideWall = true;

    List<GameObject> wallBorderList;
    List<Bird> wallList;

    int idCounter = 0;
    private bool mouseDown = false;
    private bool intensityHeatMap = false;

    private float width, height;
    bool makeWall = false;
    bool spawnPrey = false;
    bool heat = false;
    bool direction = true;
    void Start () {

        Camera cam = Camera.main;
        height = 2f * cam.orthographicSize;
        width = height * cam.aspect;
        map = new ColliderMap((int)width,(int)height);

        cam.transform.position = new Vector3(width/2f,height/2f,-10f);

        transform.localScale = new Vector3(width,height,1f);
        transform.position = new Vector3(width/2f,height/2f,10f);
        /*rend = GetComponent<Renderer>();
        rend.material.color = Color.black;*/


       rend = GetComponent<Renderer>();
       noiseTex = new Texture2D((int)width, (int)height);
       pix = new Color[noiseTex.width * noiseTex.height];
       rend.material.mainTexture = noiseTex;
        float y = 0.0F;
        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {

                pix[(int)(y * noiseTex.width + x)] = new Color(0f, 0f, 0f);
                x++;
            }
            y++;
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();


        data = new BirdActivationData(pixWidth, pixHeight, xOrg, yOrg, xScale, yScale,map);
        //birds = new GameObject[numberOfBirds];
        birds = new List<GameObject>();

        /*for (int i = 0; i < numberOfBirds; i++, idCounter++) {
            GameObject birdO = (GameObject)Instantiate(birdPrefab);
            birdO = (GameObject)Instantiate(birdPrefab);
            birdO.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
            birdO.transform.position = new Vector3(Random.Range(1f, width - 1f), Random.Range(1f, height - 1f), 0f);
            birdO.SendMessage("SetID", idCounter);
            birdO.SendMessage(ACTIVATE_WITH_BIRD_DATA, data);
            birds.Add(birdO);*/
            /*birds[i] = (GameObject) Instantiate(birdPrefab);
            birds[i].transform.eulerAngles = new Vector3(0,0, Random.Range(0f,360f));
            birds[i].transform.position= new Vector3(Random.Range(1f, width-1f), Random.Range(1f,height-1f), 0f);
            birds[i].SendMessage("SetID", idCounter);
            birds[i].SendMessage(ACTIVATE_WITH_BIRD_DATA, data);*/
            /*if (i > birds.Count * 0.9f) {
                birds[i].SendMessage("SetIsPredetor",true);*/

        
            //birds[i].GetComponent<Renderer>().enabled = false;
        //}

        /*for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++, ids++)
            {
                walls[i,j] = (GameObject)Instantiate(birdPrefab);
                walls[i, j].transform.eulerAngles = new Vector3(0, 0, 0f);
                walls[i, j].transform.position = new Vector3(5+j, 5 + i, 0f);
                walls[i, j].SendMessage("SetID", ids);
                walls[i, j].SendMessage("SetColliderMap", map);
                walls[i, j].SendMessage("SetInitialPositionOnMap");
            }
        }*/

        wallBorderList = new List<GameObject>();
        wallList = new List<Bird>();
        for (int i = 0; i < height; i++, idCounter++)
        {
            GameObject wallBorder = (GameObject)Instantiate(birdPrefab);
            wallBorder.transform.eulerAngles = new Vector3(0, 0, 0f);
            wallBorder.transform.position = new Vector3(0.5f, 0.5f+i, 0f);
            wallBorder.SendMessage("SetID", idCounter);
            wallBorder.SendMessage("SetColliderMap", map);
            wallBorder.SendMessage("SetInitialPositionOnMap");
            wallBorder.SendMessage("SetIsBorder", true);
            wallBorder.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            wallBorderList.Add(wallBorder);
        }

        for (int i = 0; i < height; i++, idCounter++)
        {
            GameObject wallBorder = (GameObject)Instantiate(birdPrefab);
            wallBorder.transform.eulerAngles = new Vector3(0, 0, 0f);
            wallBorder.transform.position = new Vector3(width-0.5f, 0.5f + i, 0f);
            wallBorder.SendMessage("SetID", idCounter);
            wallBorder.SendMessage("SetColliderMap", map);
            wallBorder.SendMessage("SetInitialPositionOnMap");
            wallBorder.SendMessage("SetIsBorder", true);
            wallBorder.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            wallBorderList.Add(wallBorder);
        }

        for (int i = 1; i < width-1; i++, idCounter++)
        {
            GameObject wallBorder = (GameObject)Instantiate(birdPrefab);
            wallBorder.transform.eulerAngles = new Vector3(0, 0, 0f);
            wallBorder.transform.position = new Vector3(0.5f+i, 0.5f, 0f);
            wallBorder.SendMessage("SetID", idCounter);
            wallBorder.SendMessage("SetColliderMap", map);
            wallBorder.SendMessage("SetInitialPositionOnMap");
            wallBorder.SendMessage("SetIsBorder", true);
            wallBorder.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            wallBorderList.Add(wallBorder);
        }

        for (int i = 1; i < width - 1; i++, idCounter++)
        {
            GameObject wallBorder = (GameObject)Instantiate(birdPrefab);
            wallBorder.transform.eulerAngles = new Vector3(0, 0, 0f);
            wallBorder.transform.position = new Vector3(0.5f + i, height-0.5f, 0f);
            wallBorder.SendMessage("SetID", idCounter);
            wallBorder.SendMessage("SetColliderMap", map);
            wallBorder.SendMessage("SetInitialPositionOnMap");
            wallBorder.SendMessage("SetIsBorder", true);
            wallBorder.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            wallBorderList.Add(wallBorder);

        }

        /*for (int i = 0; i < width; i++)
        {
            GameObject bird1 = (GameObject)Instantiate(birdPrefab);
            bird1.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
            Vector3 bird1Pos = new Vector3(i + 0.5f, 0.5f, 0f);
            bird1.transform.position = bird1Pos;
            bird1.SendMessage("SetID", birds.Length+i);

            GameObject bird2 = (GameObject)Instantiate(birdPrefab);
            bird2.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
            Vector3 bird2Pos = new Vector3(i + 0.5f, height - 0.5f, 0f);
            bird2.transform.position = bird2Pos;
            bird2.SendMessage("SetID", birds.Length + i+width);
            map.AddBirdToTile((int)bird1Pos.x, (int)bird1Pos.y,bird1.GetComponent<Bird>());
            map.AddBirdToTile((int)bird2Pos.x, (int)bird2Pos.y,bird2.GetComponent<Bird>());
        }*/



        //CalcNoise();
    }

    /*void CalcNoise()
    {
        float y = 0.0F;
        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / pixWidth * xScale;
                float yCoord = yOrg + y / pixHeight * yScale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)(y * noiseTex.width + x)] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }*/


    // Update is called once per frame

    public Color GetColorBetweenTwoColor(float value, Color c1, Color c2) {
        return new Color((((c2.r-c1.r)* value)+c1.r), (((c2.g - c1.g) * value) + c1.g), (((c2.b - c1.b) * value) + c1.b));
    }

    void LateUpdate () {
        //CalcNoise();

        /*if (Input.GetMouseButtonDown(0)) {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0f;
            
            for (int i = 0; i < 4; i++) {
                for(int j = 0; j < 4; j++) {
                    walls[i,j].SendMessage("UpdatePositionOnMap", new Vector3(j+ position.x, i + position.y, 0));
                }
            }
        }*/

        if (intensityHeatMap == true)
        {
            float[,] count = new float[noiseTex.height, noiseTex.width];
            for (int i = 0; i < noiseTex.height; i++)
            {
                for (int j = 0; j < noiseTex.width; j++)
                {
                    count[i, j] = map.GetAllBirdsCount(j, i);
                }
            }

            float maxIntesity = 0;
            float[,] intensity = new float[noiseTex.height, noiseTex.width];
            for (int i = 0; i < noiseTex.height; i++)
            {
                for (int j = 0; j < noiseTex.width; j++)
                {
                    if (count[i, j] > 0)
                    {
                        int minY = i - 3;
                        int maxY = i + 3;
                        int minX = j - 3;
                        int maxX = j + 3;

                        if (minY < 0)
                            minY = 0;
                        if (maxY >= noiseTex.height)
                            maxY = noiseTex.height - 1;
                        if (minX < 0)
                            minX = 0;
                        if (maxX >= noiseTex.width)
                            maxX = noiseTex.width - 1;

                        
                        float cx = j;
                        float cy = i;

                        for (int col = minY; col <= maxY; col++)
                        {
                            for (int row = minX; row < maxX; row++)
                            {
                                float d = Mathf.Pow(col - cy, 2) + Mathf.Pow(row - cx,2);
                                if (9f >= (d))
                                {
                                    intensity[col, row]++;
                                    //intensity[col, row] = intensity[col, row] * 1.25f;
                                    if (maxIntesity < intensity[col, row])
                                        maxIntesity = intensity[col, row];
                                }
                            }
                        }
                    }
                }
            }

            //Debug.Log(maxIntesity);
            float y = 0.0F;
            while (y < noiseTex.height)
            {
                float x = 0.0F;
                while (x < noiseTex.width)
                {
                    float it = intensity[(int)y, (int)x] / 25f;
                    if (it > 1)
                        it = 1f;
                    //Color color = new Color(it > 1f ? 1f : it, 0.5f * (1f - (it > 1f ? 1f : it)), 1f - (it > 1f ? 1f : it));
                    //Color color = Color.black;



                    pix[(int)(y * noiseTex.width + x)] = HeatMapColor(it);
                    x++;
                }
                y++;
            }
            noiseTex.SetPixels(pix);
            noiseTex.Apply();
        }
        if (Input.GetMouseButtonDown(0)) {
            mouseDown = true;

        }
        if (Input.GetMouseButtonUp(0)) {
            mouseDown = false;
        }

        if (mouseDown == true) {

            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0f;

            List<Bird> mapTileList = map.GetAllBirdsInTile((int)position.x, (int)position.y);

            if (makeWall == true) {
                bool found = false;
                for (int i = 0; i < mapTileList.Count; i++)
                {
                    if (mapTileList[i].IsActive() == false)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    GameObject wall = (GameObject)Instantiate(birdPrefab);
                    wall.transform.eulerAngles = new Vector3(0, 0, 0f);
                    wall.transform.position = new Vector3((int)position.x + 0.5f, (int)position.y + 0.5f, 0);
                    wall.SendMessage("SetID", idCounter);
                    wall.SendMessage("SetColliderMap", map);
                    wall.SendMessage("SetInitialPositionOnMap");
                    wall.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

                    wallList.Add(wall.GetComponent<Bird>());
                    idCounter++;
                }
            }
            else {
                for (int i = 0; i < mapTileList.Count; i++)
                {
                    if (mapTileList[i].IsActive() == false && mapTileList[i].IsBorder() == false)
                    {
                       
                        wallList.Remove(mapTileList[i]);
                        mapTileList[i].RemoveFromWorld();
                        break;
                    }
                }
            }
        }

        fps = 1.0f / Time.deltaTime;
        //Debug.Log(fps);
    }

    public Color HeatMapColor(float it) {
        if (heat == true)
        {
            //Heated Metal
            if (it >= 0 && it < 0.4f)
            {
                Color low = Color.black;
                Color high = new Color(128f / 255f, 0, 128f / 255f);
                return GetColorBetweenTwoColor((it) / 0.4f, low, high);
            }
            else if (it >= 0.4 && it < 0.6f)
            {
                Color low = new Color(128f / 255f, 0, 128f / 255f);
                Color high = Color.red;
                return GetColorBetweenTwoColor((0.2f - (0.6f - it)) / 0.2f, low, high);
            }
            else if (it >= 0.6 && it < 0.8)
            {
                Color low = Color.red;
                Color high = Color.yellow;
                return GetColorBetweenTwoColor((0.2f - (0.8f - it)) / 0.2f, low, high);
            }
            else if (it >= 0.8f && it <= 1f)
            {
                Color low = Color.yellow;
                Color high = Color.white;
                return GetColorBetweenTwoColor((0.2f - (1f - it)) / 0.2f, low, high);
            }
        }
        else {
            //Incandescent
            if (it >= 0 && it < 0.33f)
            {
                Color low = Color.black;
                Color high = new Color(139f / 255f, 0f, 0f);
                return GetColorBetweenTwoColor(it / 0.33f, low, high);
            }
            else if (it >= 0.33 && it < 0.66f)
            {
                Color low = new Color(139f / 255f, 0f, 0f);
                Color high = Color.yellow;
                return GetColorBetweenTwoColor((0.33f - (0.66f - it)) / 0.33f, low, high);
            }
            else if (it >= 0.66f && it <= 1f)
            {
                Color low = Color.yellow;
                Color high = Color.white;
                return GetColorBetweenTwoColor((0.33f - (1f - it)) / 0.33f, low, high);
            }
        }

        


        return Color.black;
    }

    
    public void FixedUpdate() {
        for (int i = 0; i < birds.Count; i++)
        {
            birds[i].GetComponent<Bird>().UpdateOnCall(fps);
        }

        for (int i = 0; i < birds.Count; i++)
        {
            birds[i].GetComponent<Bird>().UpdatePhysics();
        }      
    }

    public void ButtonBorderAction() {
        if (hideWall == true) {
            for (int i = 0; i < wallBorderList.Count; i++) {
                wallBorderList[i].GetComponent<Bird>().Hide();

            }
        }
        else {
            for (int i = 0; i < wallBorderList.Count; i++) {
                wallBorderList[i].GetComponent<Bird>().Show();
            }

            for (int i = 0; i < birds.Count; i++) {
                birds[i].GetComponent<Bird>().ForceRelocate();
            }
        }

        if (wallList.Count > 0)
        {
            Bird o = wallList[wallList.Count - 1];
            wallList.RemoveAt(wallList.Count - 1);
            o.RemoveFromWorld();

        }
        hideWall = !hideWall;
    }

   
    public void ButtonWallAction() {
        if (makeWall == true) {
            if (wallList.Count > 0)
            {
                Bird o = wallList[wallList.Count - 1];
                wallList.RemoveAt(wallList.Count - 1);
                o.RemoveFromWorld();

            }
        }
        makeWall = true;
    }

    public void ButtonEraseAction()
    {
        if (makeWall == true) {
            if (wallList.Count > 0)
            {
                Bird o = wallList[wallList.Count - 1];
                wallList.RemoveAt(wallList.Count - 1);
                o.RemoveFromWorld();

            }
        }
        makeWall = false;
    }

    public void ButtonIntensityAction() {
        if (makeWall == true)
        {
            if (wallList.Count > 0)
            {
                Bird o = wallList[wallList.Count - 1];
                wallList.RemoveAt(wallList.Count - 1);
                o.RemoveFromWorld();

            }
        }

        intensityHeatMap = !intensityHeatMap;
        if (intensityHeatMap == false)
        {
            for (int i = 0; i < birds.Count; i++)
            {
                birds[i].GetComponent<Renderer>().enabled = true;
                
            }

            float y = 0.0F;
            while (y < noiseTex.height)
            {
                float x = 0.0F;
                while (x < noiseTex.width)
                {

                    pix[(int)(y * noiseTex.width + x)] = new Color(0f, 0f, 0f);
                    x++;
                }
                y++;
            }
            noiseTex.SetPixels(pix);
            noiseTex.Apply();
        }
        else
        {
            for (int i = 0; i < birds.Count; i++)
            {
                birds[i].GetComponent<Renderer>().enabled = false;
                //birds[i].transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            }
        }
    }

    public void AddBirds(int num) {
        if (makeWall == true)
        {
            if (wallList.Count > 0)
            {
                Bird o = wallList[wallList.Count - 1];
                wallList.RemoveAt(wallList.Count - 1);
                o.RemoveFromWorld();

            }
        }

        if (num + birds.Count > numberOfBirds) {
            num = numberOfBirds - birds.Count;
        }

        for (int i = 0; i < num; i++) {
            GameObject birdO = (GameObject)Instantiate(birdPrefab);
            birdO = (GameObject)Instantiate(birdPrefab);
            birdO.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
            birdO.transform.position = new Vector3(Random.Range(1f, width - 1f), Random.Range(1f, height - 1f), 0f);
            birdO.SendMessage("SetID", idCounter);
            birdO.SendMessage(ACTIVATE_WITH_BIRD_DATA, data);
            birds.Add(birdO);

            //if (intensityHeatMap == true)
           // {
                birdO.GetComponent<Renderer>().enabled = !intensityHeatMap;
            birdO.transform.GetChild(0).GetComponent<Renderer>().enabled = direction;

            // }

            if (spawnPrey == true) {
                birdO.SendMessage("SetIsPredetor", true);
            }

            idCounter++;
        }
    }

    public void RemoveBirds(int num)
    {
        if (makeWall == true)
        {
            if (wallList.Count > 0)
            {
                Bird o = wallList[wallList.Count - 1];
                wallList.RemoveAt(wallList.Count - 1);
                o.RemoveFromWorld();

            }
        }

        if (birds.Count-num <0)
        {
            num = birds.Count;
        }

        for (int i = 0; i < num; i++)
        {
            GameObject o = birds[0];
            o.GetComponent<Bird>().RemoveAvtiveBirdFromWorld();
            birds.RemoveAt(0);
        }
    }

    public void ButtonTypeAction() {
        if (makeWall == true)
        {
            if (wallList.Count > 0)
            {
                Bird o = wallList[wallList.Count - 1];
                wallList.RemoveAt(wallList.Count - 1);
                o.RemoveFromWorld();

            }
        }
        spawnPrey = !spawnPrey;
    }

    public void ButtonHeatAction() {
        if (makeWall == true)
        {
            if (wallList.Count > 0)
            {
                Bird o = wallList[wallList.Count - 1];
                wallList.RemoveAt(wallList.Count - 1);
                o.RemoveFromWorld();

            }
        }
        heat = !heat;
    }

    public void ButtonDirectionAction()
    {
        if (makeWall == true)
        {
            if (wallList.Count > 0)
            {
                Bird o = wallList[wallList.Count - 1];
                wallList.RemoveAt(wallList.Count - 1);
                o.RemoveFromWorld();

            }
        }

        direction = !direction;

        for (int i = 0; i < birds.Count; i++)
        {
            birds[i].transform.GetChild(0).GetComponent<Renderer>().enabled = direction;

        }
    }

    public void ButtonPressed(string message) {
        if (message.Equals("Border"))
        {
            ButtonBorderAction();
        }
        else if (message.Equals("Wall"))
        {
            ButtonWallAction();
        }
        else if (message.Equals("Erase"))
        {
            ButtonEraseAction();
        }
        else if (message.Equals("Intensity"))
        {
            ButtonIntensityAction();
        }
        else if (message.Equals("+10"))
        {
            AddBirds(10);
        }
        else if (message.Equals("-10"))
        {
            RemoveBirds(10);
        }
        else if (message.Equals("+25"))
        {
            AddBirds(25);
        }
        else if (message.Equals("-25"))
        {
            RemoveBirds(25);
        }
        else if (message.Equals("Type"))
        {
            ButtonTypeAction();
        }
        else if (message.Equals("Heat")) {
            ButtonHeatAction();
        }
        else if (message.Equals("Direction"))
        {
            ButtonDirectionAction();
        }
    }

    public class BirdActivationData {
        private int pixWidth;
        private int pixHeight;
        private float xOrg;
        private float yOrg;
        private float xScale;
        private float yScale;
        private ColliderMap map;
        public BirdActivationData( int pixWidth, int pixHeight, float xOrg, float yOrg, float xScale, float yScale, ColliderMap map) {
            this.pixWidth = pixWidth;
            this.pixHeight = pixHeight;
            this.xOrg = xOrg;
            this.yOrg = yOrg;
            this.xScale = xScale;
            this.yScale = yScale;
            this.map = map;
        }

        public int GetPixelWidth() {
            return pixWidth;
        }

        public int GetPixelHeight() {
            return pixHeight;
        }

        public float GetXOrg() {
            return xOrg;
        }

        public float GetYOrg() {
            return yOrg;
        }

        public float GetXScale() {
            return xScale;
        }

        public float GetYScale()
        {
            return yScale;
        }

        public ColliderMap GetColliderMap() {
            return map;
        }
    }
}
