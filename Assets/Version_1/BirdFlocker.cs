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
    public Transform wall;

    private GameObject[] birds;
    private BirdActivationData data;
    private const string ACTIVATE_WITH_BIRD_DATA = "ActivateWithBirdData";
    private ColliderMap map;

    private GameObject[,] walls = new GameObject[4,4];
    // Use this for initialization

    float fps = 60f;
    void Start () {

        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        map = new ColliderMap((int)width,(int)height);

        transform.localScale = new Vector3(width,height,1f);
        transform.position = new Vector3(width/2f,height/2f,10f);
        rend = GetComponent<Renderer>();
        rend.material.color = Color.black;


       /*rend = GetComponent<Renderer>();
       noiseTex = new Texture2D(pixWidth, pixHeight);
       pix = new Color[noiseTex.width * noiseTex.height];
       rend.material.mainTexture = noiseTex;*/


       data = new BirdActivationData(pixWidth, pixHeight, xOrg, yOrg, xScale, yScale,map);
        birds = new GameObject[numberOfBirds];

        for (int i = 0; i < birds.Length; i++) {
            birds[i] = (GameObject) Instantiate(birdPrefab);
            birds[i].transform.eulerAngles = new Vector3(0,0, Random.Range(0f,360f));
            birds[i].transform.position= new Vector3(Random.Range(1f, width-1f), Random.Range(1f,height-1f), 0f);
            birds[i].SendMessage("SetID",i);
            birds[i].SendMessage(ACTIVATE_WITH_BIRD_DATA, data);
        }

        int ids = birds.Length;
        for (int i = 0; i < 4; i++)
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
        }

        for (int i = 0; i < height; i++,ids++)
        {
            GameObject wallBorder = (GameObject)Instantiate(birdPrefab);
            wallBorder.transform.eulerAngles = new Vector3(0, 0, 0f);
            wallBorder.transform.position = new Vector3(0.5f, 0.5f+i, 0f);
            wallBorder.SendMessage("SetID", ids);
            wallBorder.SendMessage("SetColliderMap", map);
            wallBorder.SendMessage("SetInitialPositionOnMap");
        }

        for (int i = 0; i < height; i++, ids++)
        {
            GameObject wallBorder = (GameObject)Instantiate(birdPrefab);
            wallBorder.transform.eulerAngles = new Vector3(0, 0, 0f);
            wallBorder.transform.position = new Vector3(width-0.5f, 0.5f + i, 0f);
            wallBorder.SendMessage("SetID", ids);
            wallBorder.SendMessage("SetColliderMap", map);
            wallBorder.SendMessage("SetInitialPositionOnMap");
        }

        for (int i = 1; i < width-1; i++, ids++)
        {
            GameObject wallBorder = (GameObject)Instantiate(birdPrefab);
            wallBorder.transform.eulerAngles = new Vector3(0, 0, 0f);
            wallBorder.transform.position = new Vector3(0.5f+i, 0.5f, 0f);
            wallBorder.SendMessage("SetID", ids);
            wallBorder.SendMessage("SetColliderMap", map);
            wallBorder.SendMessage("SetInitialPositionOnMap");
        }

        for (int i = 1; i < width - 1; i++, ids++)
        {
            GameObject wallBorder = (GameObject)Instantiate(birdPrefab);
            wallBorder.transform.eulerAngles = new Vector3(0, 0, 0f);
            wallBorder.transform.position = new Vector3(0.5f + i, height-0.5f, 0f);
            wallBorder.SendMessage("SetID", ids);
            wallBorder.SendMessage("SetColliderMap", map);
            wallBorder.SendMessage("SetInitialPositionOnMap");
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
    void Update () {
        //CalcNoise();

        if (Input.GetMouseButtonDown(0)) {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0f;
            
            for (int i = 0; i < 4; i++) {
                for(int j = 0; j < 4; j++) {
                    walls[i,j].SendMessage("UpdatePositionOnMap", new Vector3(j+ position.x, i + position.y, 0));
                }
            }
            //wall.position = position;

        }

        fps = 1.0f / Time.deltaTime;
        //Debug.Log(fps);
    }

    public void FixedUpdate() {
        for (int i = 0; i < birds.Length; i++)
        {
            birds[i].GetComponent<Bird>().UpdateOnCall(fps);
        }

        for (int i = 0; i < birds.Length; i++)
        {
            birds[i].GetComponent<Bird>().UpdatePhysics();
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
