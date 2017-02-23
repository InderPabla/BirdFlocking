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

    float sep = 1.25f;
    float attr = 2.1f;
    bool inCollsion = false;
    float chosenAngleOfChange = 0;
    // Update is called once per frame
    public void UpdateOnCall (float fps) {
        
        if (active == true)
        {

            sep = 1f+(0.25f* (1f-(fps/60f)));
            position = transform.position;
            List<Bird> birds = map.GetAllBirdsWithinRange((int)position.x, (int)position.y, attr, transform.position, this);
            //transZ = FixAngle(transform.eulerAngles.z);

            float avgHeading = 0;
            float headingCount = 0;
            Vector3 avgHeadingUnit = Vector3.zero;
            float wallCount = 0;
            Vector3 avgBirdHeadingUnit = Vector3.zero;
            float birdCount = 0;

            //float avgBreakAwayHeading = 0;
            //float breakAwayHeadingCount = 0;
            

            for (int i = 0; i < birds.Count; i++)
            {
                if (birds[i] != this)
                {
                    Vector3 positionOther = birds[i].transform.position;
                    Vector2 dif = position - positionOther;
                    float distance = Vector3.Distance(position, positionOther);
                    float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
                    angle = FixAngle(angle);
                    angle -= 90f;
                    angle = FixAngle(angle);
                    angle -= (transZ);
                    angle = FixAngle(angle);


                    if (birds[i].active == false && !(angle >= (360f - 90f) || angle <= (90f)))
                    {
                        float otherAngle = FixAngle(transZ);
                        if (inCollsion == false) {
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

                        /*if (otherAngle >= 0 && otherAngle < 90)
                            otherAngle = FixAngle(otherAngle - 90f);
                        else if (otherAngle >= 90 && otherAngle < 180)
                            otherAngle = FixAngle(otherAngle + 90f);
                        else if (otherAngle >= 180 && otherAngle < 270)
                            otherAngle = FixAngle(otherAngle + 90f);
                        else if (otherAngle >= 270 && otherAngle <= 360)
                            otherAngle = FixAngle(otherAngle - 90f);*/



                        headingCount += 10000f;
                        avgHeadingUnit += (new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0)* 10000f);

                        wallCount++;
                    }
                    else if (!(angle >= (360f - 40f) || angle <= (40f)))
                    {
                        birdCount++;
                        if (distance < sep)
                        {
                            //breakAwayHeadingCount++;
                            /*float transZ2 = FixAngle(birds[i].transform.eulerAngles.z);
                            Vector2 dif2 = positionOther - position;
                            float angle2 = Mathf.Atan2(dif2.y, dif2.x) * Mathf.Rad2Deg;
                            angle2 -= 90f;
                            angle2 = FixAngle(angle2);
                            angle2 -= (transZ2);
                            angle2 = FixAngle(angle2);

                            avgBreakAwayHeading += FixAngle(angle2);*/

                            headingCount++;

                            float otherAngle = FixAngle(birds[i].transZ);
                            otherAngle -= 180f;
                            otherAngle = FixAngle(otherAngle);

                            avgHeading += otherAngle > 180f ? (360f - otherAngle) : otherAngle;
                            avgHeadingUnit += new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0);
                        }
                        else if (distance < attr)
                        {

                            headingCount++;

                            float otherAngle = FixAngle(birds[i].transZ);
                            //otherAngle -= 90f;
                            otherAngle = FixAngle(otherAngle);

                            avgHeading += otherAngle > 180f ? (360f - otherAngle) : otherAngle; //potential fix?
                            avgHeadingUnit += new Vector3(Mathf.Cos(Mathf.Deg2Rad * otherAngle), Mathf.Sin(Mathf.Deg2Rad * otherAngle), 0);
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


                /*avgHeading = avgHeading / headingCount;
                float thisHeading = transZ > 180f ? (360f - transZ) : transZ;

                if (avgHeading < 0f && thisHeading < 0f)
                {
                    if (avgHeading < thisHeading)
                    {
                        transZ -= 5f;
                    }
                    else if (avgHeading > thisHeading)
                    {
                        transZ += 5f;
                    }
                }
                else if (avgHeading > 0f && thisHeading > 0f)
                {
                    if (avgHeading < thisHeading)
                    {
                        transZ -= 5f;
                    }
                    else if (avgHeading > thisHeading)
                    {
                        transZ += 5f;
                    }
                }
                else if (avgHeading > 0f && thisHeading < 0f)
                {
                    float distanceRight = avgHeading + (thisHeading * -1f);
                    float distanceLeft = (180f- avgHeading) + (thisHeading * 180f);

                    if (distanceRight > distanceLeft)
                    {
                        transZ += 5f;
                    }
                    else if(distanceRight < distanceLeft)
                    {
                        transZ -= 5f;
                    }
                }
                else if (avgHeading < 0f && thisHeading > 0f)
                {
                    float distanceRight = thisHeading + (avgHeading * -1f);
                    float distanceLeft = (180f - thisHeading) + (avgHeading * 180f);

                    if (distanceRight > distanceLeft)
                    {
                        transZ += 5f;
                    }
                    else if (distanceRight < distanceLeft)
                    {
                        transZ -= 5f;
                    }
                }*/

                float turnSpeed = 5f;
                if (wallCount > 0f)
                    turnSpeed = 15f;
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
        
            /*if (breakAwayHeadingCount > 0)
            {
                avgBreakAwayHeading = avgBreakAwayHeading / breakAwayHeadingCount;

                if (avgBreakAwayHeading > transZ)
                {

                    float distanceRight = avgBreakAwayHeading - transZ;
                    float distanceLeft = transZ + (360f - avgBreakAwayHeading);

                    if (distanceLeft > distanceRight)
                    {
                        transZ += 180f;//transZ -= 2f;
                    }
                    else if (distanceLeft < distanceRight)
                    {
                        transZ += 180f;//transZ += 2f;
                    }
                    else
                        transZ += 180f;
                }
                else if (avgBreakAwayHeading < transZ)
                {
                    float distanceLeft = transZ - avgBreakAwayHeading;
                    float distanceRight = avgBreakAwayHeading + (360f - transZ);

                    if (distanceLeft > distanceRight)
                    {
                        transZ += 180f;//transZ -= 2f;
                    }
                    else if (distanceLeft < distanceRight)
                    {
                        transZ += 180f;//transZ += 2f;
                    }
                    else
                        transZ += 180f;
                }
                else
                    transZ += 180f;

                transZ = FixAngle(transZ);
            }*/


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

    public void UpdatePhysics() {

        map.RemoveBirdFromTile((int)oldPosition.x, (int)oldPosition.y, this);

        map.AddBirdToTile((int)position.x, (int)position.y, this);
        oldPosition = transform.position;
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

}
