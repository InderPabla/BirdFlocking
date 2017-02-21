using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (gameObject.name.Equals("1"))
        {
            Collider2D[] withinRange = Physics2D.OverlapCircleAll(transform.position, 5.1f);

            Vector3 position = transform.position;

            for (int i = 0; i < withinRange.Length; i++)
            {
                if (transform != withinRange[i].transform)
                {
                    Vector3 positionOther = withinRange[i].transform.position;
                    Vector2 dif = position - positionOther;
                    float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;

                    if (angle < 0f)
                    {
                        angle = 360f + angle;
                    }

                    angle -= 90f;


                    if (angle < 0f)
                    {
                        angle = 360f + angle;
                    }

                    angle -= transform.eulerAngles.z;
                    if (angle < 0f)
                    {
                        angle = 360f + angle;
                    }
                    Debug.Log(angle);
                }

            }
        }
    }
}
