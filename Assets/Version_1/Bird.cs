using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour, IEquatable<Bird>{

    private int ID;
    private int pixWidth;
    private int pixHeight;
    private float xOrg;
    private float yOrg;
    private float xScale;
    private float yScale;

    private float speed = 5f;
    private Rigidbody2D rBody;
    private bool active = false;

    private Renderer ren;

    private float height, width;
    private ColliderMap map;

    private Vector3 oldPosition;
    private Vector3 position;
    private float transZ;
    private bool isBorder = false;
    private bool isPredetor = false;
    // Use this for initialization
    void Start () {
        Camera cam = Camera.main;
        height = 2f * cam.orthographicSize;
        width = height * cam.aspect;


        rBody = GetComponent<Rigidbody2D>();
        //rBody.isKinematic = true;
        ren = GetComponent<Renderer>();
        ren.material.color = Color.yellow;
    }

    public float FixAngle(float angle) {
        /*float temp = angle;
        if (temp < 0)
            return (360f + temp);
        else if (temp > 360f)
            return temp - 360f;

        return temp;*/

        return (angle + 360f) % 360f;

    }

    float sep = 1.25f; // 1f+(0.5f* (1f-(fps/60f)));
    float attr = 2.1f; //2.1
    bool inCollsion = false;
    float chosenAngleOfChange = 0;
    bool forceRelocate = false;
    float wallCount = 0;
    public float affect = 1f;

    // Update is called once per frame
    public void UpdateOnCall (float fps) {
        
        if (active == true)
        {
            sep = 1.05f+(0.5f* (1f-(fps/60f)));
            position = transform.position;
            List<Bird> birds = map.GetAllBirdsWithinRange((int)position.x, (int)position.y, attr, transform.position, this);
            //transZ = FixAngle(transform.eulerAngles.z);

            float avgHeading = 0;
            float headingCount = 0;
            Vector3 avgHeadingUnit = Vector3.zero;
            wallCount = 0;
            Vector3 avgBirdHeadingUnit = Vector3.zero;
            //float birdCount = 0;

            //float avgBreakAwayHeading = 0;
            //float breakAwayHeadingCount = 0;

            affect = 1f;

            if (isPredetor == true) {
                for (int i = 0; i < birds.Count; i++)
                {
                    if (birds[i] != this)
                    {
                        Vector3 positionOther = /*birds[i].transform.position*/ ClosestLocation(position, birds[i].transform.position); ;
                        Vector2 dif = position - positionOther;
                        float distance = Vector3.Distance(position, positionOther);
                        float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
                        float otherAffect = birds[i].affect;
                        angle = FixAngle(angle);
                        angle -= 90f;
                        angle = FixAngle(angle);
                        angle -= (transZ);
                        angle = FixAngle(angle);


                        if (birds[i].active == false && !(angle >= (360f - 90f) || angle <= (90f)))
                        {
                            float otherAngle = FixAngle(transZ);
                            if (inCollsion == false)
                            {
                                inCollsion = true;

                                if (otherAngle >= 0 && otherAngle < 90)
                                    chosenAngleOfChange = -10f;
                                else if (otherAngle >= 90 && otherAngle < 180)
                                    chosenAngleOfChange = 10f;
                                else if (otherAngle >= 180 && otherAngle < 270)
                                    chosenAngleOfChange = -10f;
                                else if (otherAngle >= 270 && otherAngle <= 360)
                                    chosenAngleOfChange = 10f;
                            }

                            otherAngle = FixAngle(otherAngle + chosenAngleOfChange);
                            affect = 10f;

                            headingCount += 1000f;
                            avgHeadingUnit += (new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0) * 1000f);

                            wallCount++;
                        }
                        else if (!(angle >= (360f - 40f) || angle <= (40f)) && birds[i].active == true)
                        {
                            //birdCount++;
                            if (distance < sep && birds[i].isPredetor)
                            {


                                headingCount += otherAffect;

                                float otherAngle = FixAngle(birds[i].transZ);
                                otherAngle -= 180f;
                                otherAngle = FixAngle(otherAngle);

                                avgHeadingUnit += new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0) * otherAffect;
                            }
                            else if (distance < attr)
                            {

                                headingCount += 1;

                                float otherAngle = FixAngle(birds[i].transZ);
                                //otherAngle -= 90f;
                                otherAngle = FixAngle(otherAngle);

                                avgHeadingUnit += new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0);
                            }

                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < birds.Count; i++)
                {
                    if (birds[i] != this)
                    {
                        Vector3 positionOther = /*birds[i].transform.position*/ ClosestLocation(position, birds[i].transform.position); ;
                        Vector2 dif = position - positionOther;
                        float distance = Vector3.Distance(position, positionOther);
                        float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
                        float otherAffect = birds[i].affect;
                        angle = FixAngle(angle);
                        angle -= 90f;
                        angle = FixAngle(angle);
                        angle -= (transZ);
                        angle = FixAngle(angle);


                        if (birds[i].active == false && !(angle >= (360f - 90f) || angle <= (90f)) )
                        {
                            float otherAngle = FixAngle(transZ);
                            if (inCollsion == false)
                            {
                                inCollsion = true;

                                if (otherAngle >= 0 && otherAngle < 90)
                                    chosenAngleOfChange = -10f;
                                else if (otherAngle >= 90 && otherAngle < 180)
                                    chosenAngleOfChange = 10f;
                                else if (otherAngle >= 180 && otherAngle < 270)
                                    chosenAngleOfChange = -10f;
                                else if (otherAngle >= 270 && otherAngle <= 360)
                                    chosenAngleOfChange = 10f;
                            }

                            otherAngle = FixAngle(otherAngle + chosenAngleOfChange);
                            affect = 10f;

                            headingCount += 1000f;
                            avgHeadingUnit += (new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0) * 1000f);

                            wallCount++;
                        }
                        else if (!(angle >= (360f - 40f) || angle <= (40f)) && birds[i].active == true)
                        {
                            //birdCount++;
                            if (birds[i].isPredetor == true)
                            {
                                headingCount += 10f;

                                float otherAngle = FixAngle(birds[i].transZ);
                                otherAngle -= 180f;
                                otherAngle = FixAngle(otherAngle);

                                avgHeadingUnit += new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0) * 10f;
                            }
                            else
                            {
                                if (distance < sep)
                                {


                                    headingCount += otherAffect;

                                    float otherAngle = FixAngle(birds[i].transZ);
                                    otherAngle -= 180f;
                                    otherAngle = FixAngle(otherAngle);

                                    avgHeadingUnit += new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0) * otherAffect;
                                }
                                else if (distance < attr)
                                {

                                    headingCount += 1;

                                    float otherAngle = FixAngle(birds[i].transZ);
                                    //otherAngle -= 90f;
                                    otherAngle = FixAngle(otherAngle);

                                    avgHeadingUnit += new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0);
                                }
                            }

                        }
                    }
                }
            }

            

            if (wallCount == 0f)
                inCollsion = false;

            if (headingCount > 0 /*&& breakAwayHeadingCount == 0*/)
            {
                avgHeadingUnit /= headingCount;
                avgHeading = FixAngle(Mathf.Atan2(avgHeadingUnit.y, avgHeadingUnit.x)*Mathf.Rad2Deg);

                float turnSpeed = 5f;
                /*if (wallCount > 0f)
                    turnSpeed = 15f;*/
                if (avgHeading > transZ)
                {

                    float distanceRight = avgHeading - transZ;
                    float distanceLeft = transZ + (360f - avgHeading);

                    if (distanceLeft > distanceRight)
                    {
                        transZ += turnSpeed;
                    }
                    else if (distanceLeft < distanceRight)
                    {
                        transZ -= turnSpeed;
                    }
                    else
                    {
                        transZ = avgHeading;
                    }
                }
                else if (avgHeading < transZ)
                {
                    float distanceLeft = transZ - avgHeading;
                    float distanceRight = avgHeading + (360f - transZ);

                    if (distanceLeft > distanceRight)
                    {
                        transZ += turnSpeed;
                    }
                    else if (distanceLeft < distanceRight)
                    {
                        transZ -= turnSpeed;
                    }
                    else
                    {
                        transZ = avgHeading;
                    }
                }
                else
                {
                    transZ = avgHeading;
                }

                transZ = FixAngle(transZ);

                ren.material.color = Color.green;
            }
            else {
                ren.material.color = Color.red;
            }

            if(isPredetor)
                ren.material.color = Color.blue;

            if (position.x >= width)
            {
                position.x = 0.1f;
            }
            else if (position.x <= 0f)
            {
                position.x = width - 0.1f;
            }

            if (position.y >= height)
            {
                position.y = 0.1f;
            }
            else if (position.y <= 0f)
            {
                position.y = height - 0.1f;
            }
            
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


    public bool IsActive() {
        return active;
    }

    public void Hide() {
        map.RemoveBirdFromTile((int)transform.position.x, (int)transform.position.y, this);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void Show() {
        map.AddBirdToTile((int)transform.position.x, (int)transform.position.y, this);
        GetComponent<Collider2D>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void ForceRelocate() {
        forceRelocate = true;
        Invoke("RemoveForceRelocate",0.1f);
    }

    public void RemoveForceRelocate() {
        forceRelocate = false;
    }

    public void UpdatePhysics() {

        if (wallCount > 0f && forceRelocate == true)
        {
            position = new Vector3(UnityEngine.Random.Range(1f, width - 1f), UnityEngine.Random.Range(1f, height - 1f), 0f);
            forceRelocate = false;
        }
        

        map.RemoveBirdFromTile((int)oldPosition.x, (int)oldPosition.y, this);

        map.AddBirdToTile((int)position.x, (int)position.y, this);
        //oldPosition = transform.position;
        oldPosition = position;
        transform.position = position;
        Vector3 vecAngle = new Vector3(0f, 0f, transZ);
        transform.eulerAngles = vecAngle;

        //vecAngle = vecAngle.normalized;
        vecAngle.z = FixAngle((180f - vecAngle.z)) * Mathf.Deg2Rad;

        vecAngle = new Vector3(Mathf.Sin(vecAngle.z), Mathf.Cos(vecAngle.z), 0);

        rBody.velocity = (vecAngle * -speed);
    }

    public void ActivateWithBirdData(BirdFlocker.BirdActivationData data) {
        transZ = FixAngle(transform.eulerAngles.z);

        pixWidth = data.GetPixelWidth();
        pixHeight = data.GetPixelHeight();
        xOrg = data.GetXOrg();
        yOrg = data.GetYOrg();
        xScale = data.GetXScale();
        yScale = data.GetYScale();


        this.map = data.GetColliderMap();
        oldPosition = transform.position;
        map.AddBirdToTile((int)oldPosition.x,(int)oldPosition.y,this);
        map = data.GetColliderMap();
        GetComponent<Rigidbody2D>().isKinematic = false;

        
        active = true;
    }

    public void SetID(int ID) {
        this.ID = ID;
    }

    public void SetColliderMap(ColliderMap map) {
        this.map = map;
    }

    public bool Equals(Bird other)
    {
        if (other.ID == ID) {
            return true;
        }
        return false;
    }

    public void SetInitialPositionOnMap()
    {
        map.AddBirdToTile((int)transform.position.x, (int)transform.position.y, this);
    }

    public void UpdatePositionOnMap(Vector3 newPosition)
    {
        map.RemoveBirdFromTile((int)transform.position.x, (int)transform.position.y, this);
        transform.position = newPosition;
        map.AddBirdToTile((int)newPosition.x, (int)newPosition.y, this);
    }

    public bool EqualsBird(Bird other)
    {
        if (other.ID == ID)
        {
            return true;
        }
        return false;
    }

    public void RemoveFromWorld() {
        Hide();
        Destroy(this.gameObject);
    }

    public void RemoveAvtiveBirdFromWorld()
    {
        map.RemoveBirdFromTile((int)oldPosition.x, (int)oldPosition.y, this);
        map.RemoveBirdFromTile((int)transform.position.x, (int)transform.position.y, this);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        Destroy(this.gameObject);
    }

    public void SetIsBorder(bool isBorder) {
        this.isBorder = isBorder;
    }

    public bool IsBorder()
    {
        return isBorder;
    }

    public void SetIsPredetor(bool pred) {
        isPredetor = true;
    }

    public bool IsPredetor() {
        return isPredetor;
    }
}
