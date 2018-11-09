using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TouchPoints : MonoBehaviour {



  

    GameObject insta01;
    //GameObject insta02;
    //GameObject insta03;
    //GameObject insta04;
    //GameObject insta05;
    //GameObject insta06;
    //GameObject insta07;
    //GameObject insta08;
    //GameObject insta09;
    //GameObject insta10;
    //GameObject insta11;
    //GameObject insta12;
    //GameObject insta13;
    //GameObject insta14;
    //GameObject insta15;

    public static List<GameObject> insta1 = new List<GameObject>();
    Transform tt;
    int count,count2;

    int itemlength; 

   

	// Use this for initialization
	void Start () 
    {
        
		
	}
	
	// Update is called once per frame
	void Update () 
    {








        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {

                int listlength = ShapeManager.modelname.Count;


                for (int i = 1; i <= listlength; i++)
                {

                    if (hit.collider.gameObject.name == "Touchpoint" + i)
                    {
                        insta01 = GameObject.Find("SBlue " + i);

                        tt = insta01.transform.GetChild(1);
                        //        Debug.Log("Touch transform" + tt.name);
                        VideoPlayer vv;
                        vv = tt.GetComponent<VideoPlayer>();
                        vv.Play();
                    }

                }


                //    if (hit.collider.gameObject.name == "Touchpoint1")
                //    {
                //        insta01 = GameObject.Find("SBlue 1");
                //       tt= insta01.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();

                //    //for (int i = 0; i <= listlength;i++)
                //    //{

                //    /*
                //        if (hit.collider.gameObject.name == "Touchpoint1"  )
                //        {
                //            Debug.Log("HIT TOUCH POINT2");

                //        insta1.Add(GameObject.Find("SBlue " + 1));
                //        Debug.Log("Insta1 = " + insta1[0].gameObject.name.ToString());

                //            tt = insta1[0].transform;
                //            Debug.Log("tranfomr name==" + tt.name);
                //            VideoPlayer vv;
                //            vv = tt.GetChild(1).GetComponent<VideoPlayer>();
                //            vv.Play();

                //    */




                //   //     if (hit.collider.gameObject.name == "Touchpoint"+(i+1))
                //   //     {
                //   //         Debug.Log("HIT TOUCH POINT2");

                //   //         insta1.Add(GameObject.Find("SBlue " + (i+1)));
                //   //// Debug.Log("Insta1 = " + insta1[i].gameObject.name.ToString());

                //        //tt = insta1[i].transform;
                //        //Debug.Log("tranfomr name==" + tt.name);
                //        //VideoPlayer vv;
                //        //vv = tt.GetChild(1).GetComponent<VideoPlayer>();
                //        //vv.Play();



                //        //}

                //    }



                //    if (hit.collider.gameObject.name == "Touchpoint2")
                //    {
                //        insta02 = GameObject.Find("SBlue 2");
                //        tt = insta02.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }

                //    if (hit.collider.gameObject.name == "Touchpoint3")
                //    {
                //        insta03 = GameObject.Find("SBlue 3");
                //        tt = insta03.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }


                //    if (hit.collider.gameObject.name == "Touchpoint4")
                //    {
                //        insta04 = GameObject.Find("SBlue 4");
                //        tt = insta04.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }

                //    if (hit.collider.gameObject.name == "Touchpoint5")
                //    {
                //        insta05 = GameObject.Find("SBlue 5");
                //        tt = insta05.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }


                //    if (hit.collider.gameObject.name == "Touchpoint6")
                //    {
                //        insta06 = GameObject.Find("SBlue 6");
                //        tt = insta06.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }

                //    if (hit.collider.gameObject.name == "Touchpoint7")
                //    {
                //        insta07 = GameObject.Find("SBlue 7");
                //        tt = insta07.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }

                //    if (hit.collider.gameObject.name == "Touchpoint8")
                //    {
                //        insta08 = GameObject.Find("SBlue 8");
                //        tt = insta08.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }

                //    if (hit.collider.gameObject.name == "Touchpoint9")
                //    {
                //        insta09 = GameObject.Find("SBlue 9");
                //        tt = insta09.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }



                //    if (hit.collider.gameObject.name == "Touchpoint10")
                //    {
                //        insta10 = GameObject.Find("SBlue 10");
                //        tt = insta10.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }

                //    if (hit.collider.gameObject.name == "Touchpoint11")
                //    {
                //        insta11 = GameObject.Find("SBlue 11");
                //        tt = insta11.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }


                //    if (hit.collider.gameObject.name == "Touchpoint12")
                //    {
                //        insta12 = GameObject.Find("SBlue 12");
                //        tt = insta12.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }




                //    if (hit.collider.gameObject.name == "Touchpoint13")
                //    {
                //        insta13 = GameObject.Find("SBlue 13");
                //        tt = insta13.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }




                //    if (hit.collider.gameObject.name == "Touchpoint14")
                //    {
                //        insta14 = GameObject.Find("SBlue 14");
                //        tt = insta14.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }







                //    if (hit.collider.gameObject.name == "Touchpoint15")
                //    {
                //        insta15 = GameObject.Find("SBlue 15");
                //        tt = insta15.transform.GetChild(1);
                //        Debug.Log("Touch transform" + tt.name);
                //        VideoPlayer vv;
                //        vv = tt.GetComponent<VideoPlayer>();
                //        vv.Play();
                //    }


                //}
            }

        }
	}
}
