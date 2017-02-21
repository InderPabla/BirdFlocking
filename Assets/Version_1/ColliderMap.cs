

using System.Collections.Generic;
using UnityEngine;

public class ColliderMap {

    public ColliderTile[,] tileMap;
    public int width, height;

    public class ColliderTile {
        
        public List<Bird> colliders;

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
        if(IsValid(x, y))
            tileMap[y, x].colliders.Remove(bird);
    }

    public void AddBirdToTile(int x, int y, Bird bird) {
        if (IsValid(x, y))
            tileMap[y, x].colliders.Add(bird);
    }

    public List<Bird> GetAllBirdsWithinRange(int x , int y, float distance,Vector2 position,Bird bird) {
        List<Bird> birds = new List<Bird>();
        for (int i = y - 3; i <= y + 3; i++) {
            for (int j = x - 3; j <= x + 3; j++) {
                if (IsValid(j, i)) {
                    
                    //if (tileMap[i, j].colliders.Count > 0)
                    //{
                        for (int k = 0; k < tileMap[i, j].colliders.Count; k++)
                        {
                            Bird b = tileMap[i, j].colliders[k];
                            if (!b.EqualsBird(bird) && Vector2.Distance(b.transform.position, position) <= distance)
                            {
                                birds.Add(b);
                            }
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

}
