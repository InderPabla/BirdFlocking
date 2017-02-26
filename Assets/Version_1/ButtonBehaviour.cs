using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour {

    public string message = "Button";
    public BirdFlocker birdFlocker;
    Vector3 position;
    Vector3 scale;
    Rect rect;
    public bool isToggleType = false;
    bool on = false;

    public ButtonBehaviour linkedOpposite;
    // Use this for initialization
    void Start () {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        if(transform.position.x<0 || transform.position.y<0)
            position = transform.position+new Vector3(width/2f,height/2f,0f);
        else
            position = transform.position ;
        scale = transform.localScale;
        rect = new Rect(position.x-scale.x/2, position.y-scale.y/2,scale.x, scale.y);
        
        //Debug.Log(rect+" "+gameObject.name);
    }

    bool touchOnDown = false;

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0))  {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = 0f;
            //Debug.Log(touchPosition);
            if (rect.Contains(touchPosition)) {
                touchOnDown = true;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = 0f;

            if (rect.Contains(touchPosition) && touchOnDown == true) {
                birdFlocker.ButtonPressed(message);
                if (isToggleType == true)
                {
                    on = !on;
                    if (on)
                    {
                        GetComponent<Renderer>().material.color = Color.blue;
                        if (linkedOpposite != null)
                        {
                            linkedOpposite.GetComponent<Renderer>().material.color = new Color(114f / 255f, 1f, 0);
                            linkedOpposite.on = !on;
                        }
                    }
                    else
                    {
                        GetComponent<Renderer>().material.color = new Color(114f / 255f, 1f, 0);

                        if (linkedOpposite != null)
                        {
                            linkedOpposite.GetComponent<Renderer>().material.color = Color.blue;
                            linkedOpposite.on = !on;
                        }
                    }
                }

                
            }
            touchOnDown = false;
        }
    }
}
