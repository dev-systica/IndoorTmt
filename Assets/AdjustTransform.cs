using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdjustTransform : MonoBehaviour {

    public float rotspeed = 100f;
    private string GoName;


    RaycastHit hit;

   // private RaycastHit vision;
	// Use this for initialization
	void Start () 
    {

       
	}

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


      

        if (Physics.Raycast(ray,out hit,100))
        {

            //thedistance = hits.distance;
          //  Debug.Log(" GO Name "+hit.collider.gameObject.name);
            GoName = hit.collider.gameObject.name;

        }


        //if(  Input.touchCount == 2 && !EventSystem.current.IsPointerOverGameObject() )
        //{
        //    Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
        //    if (hit.transform.gameObject.name == GOName)
        //    {
        //        hit.collider.gameObject.transform.Rotate(Vector3.up, -touchDeltaPosition.x * rotspeed * Time.deltaTime, Space.World);
        //        hit.collider.gameObject.transform.Rotate(Vector3.right, touchDeltaPosition.y * rotspeed * Time.deltaTime, Space.World);
        //    }

        //}

     
    }


  
   
   

}
