

using System.Collections.Generic;
using UnityEngine;

public class ColliderMap {

    public ColliderTile[,] tileMap;
    public int width, height;

    public class ColliderTile {
        
        public List<Bird> colliders;
        public int wallCount = 0;
        public int birdCount = 0;
        public int predetorCount = 0;
        public ColliderTile() {
            colliders = new List<Bird>();
        }


    }

    public ColliderMap(int width, int height) {
        this.width = width;
        this.height = height;
        Debug.Log(width+" "+height);
        tileMap = new ColliderTile[height, width];

        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                tileMap[i,j] = new ColliderTile();
            }
        }
    }


    public bool IsValid(int x, int y) {
        if (x < 0 || x>=width ||y<0 ||y>=height)
            return false;
        return true;
    }

    public void RemoveBirdFromTile(int x, int y, Bird bird) {
        if (IsValid(x, y)) {
            if (bird.IsActive() && bird.IsPredetor() == false)
                tileMap[y, x].birdCount--;
            else if (bird.IsPredetor() == true)
                tileMap[y, x].predetorCount--;
            else
                tileMap[y, x].wallCount--;

            tileMap[y, x].colliders.Remove(bird);
        }
    }

    public void AddBirdToTile(int x, int y, Bird bird) {
        if (IsValid(x, y))
        {
            if (bird.IsActive() && bird.IsPredetor()== false) 
                tileMap[y, x].birdCount++;
            else if(bird.IsPredetor() == true)
                tileMap[y, x].predetorCount++;
            else
                tileMap[y, x].wallCount++;
            tileMap[y, x].colliders.Add(bird);
        }
    }

    
    public Vector3 ClosestLocation(Vector3 p, Vector3 otherPoint) {
        float dX = Mathf.Abs(otherPoint.x - p.x);
        float dY = Mathf.Abs(otherPoint.y - p.y);
        float x = otherPoint.x;
        float y = otherPoint.y;

        // now see if the distance between birds is closer if going off one
        // side of the map and onto the other.
        if (Mathf.Abs(width - otherPoint.x + p.x) < dX)
        {
            dX = width - otherPoint.x + p.x;
            x = otherPoint.x - width;
        }
        if (Mathf.Abs(width - p.x + otherPoint.x) < dX)
        {
            dX = width - p.x + otherPoint.x;
            x = otherPoint.x + width;
        }

        if (Mathf.Abs(height - otherPoint.y + p.y) < dY)
        {
            dY = height - otherPoint.y + p.y;
            y = otherPoint.y - height;
        }
        if (Mathf.Abs(height - p.y + otherPoint.y) < dY)
        {
            dY = height - p.y + otherPoint.y;
            y = otherPoint.y + height;
        }

        return new Vector3(x, y);
    }

    public List<Bird> GetAllBirdsWithinRange(int x , int y, float distance,Vector2 position,Bird bird) {
        List<Bird> birds = new List<Bird>();
        int add = 0;
        for (int i = y - 2; i <= y + 2; i++) {
            for (int j = x - 2; j <= x + 2; j++) {
                if (IsValid(j, i) == true) {

                    //if (tileMap[i, j].colliders.Count > 0)
                    //{
                    int count = tileMap[i, j].birdCount;
                   
                        for (int k = 0; k < tileMap[i, j].colliders.Count; k++)
                        {
                            Bird b = tileMap[i, j].colliders[k];
                            Vector3 otherClosestPosition = ClosestLocation(position, b.transform.position);
                            if (!b.EqualsBird(bird) && Vector2.Distance(otherClosestPosition, position) <= distance)
                            {
                                if (b.IsActive() == false) {
                                    birds.Add(b);
                                }
                                else if(count>5) {
                                    if (add == 0) {
                                        birds.Add(b);
                                        add = 1;
                                    }
                                    else {
                                        add = 0;
                                    }
                                }
                                else {
                                    birds.Add(b);
                                    //add = 0;
                                }
                            }
                        //optimizer
                       

                        /* int c = 0;
                         for (int o = 0; o < birds.Count; o++)
                             if (birds[o].EqualsBird(b))
                                 c++;
                     if (c >= 2)
                         Debug.Log("issues");*/
                    }
                    //}
                }
            }
        }
        
        return birds;
    }

    public List<Bird> GetAllBirdsInTile(int x, int y) {
        List<Bird> birds = new List<Bird>();

        if (IsValid(x,y) == true) {
            birds = tileMap[y, x].colliders;
        }

        return birds;
    }


    public int GetAllBirdsCount(int x, int y) {
        if (IsValid(x, y) == true)
        {
            return tileMap[y, x].birdCount+ tileMap[y, x].predetorCount;
        }

        return 0;
    }

}
