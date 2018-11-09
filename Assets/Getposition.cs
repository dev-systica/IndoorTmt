using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using System.Runtime.InteropServices;

public class Getposition : MonoBehaviour
{



    public Text Locationinformation;
    public static double latitudes;
    public static double longitudes;
    public static double altitudes;
    public static string deviceinfo;
    string upload;

    string JSONDataString;

    public float period = 0.0f;


    private float nextUpdate = 10f;





//#if UNITY_IOS
//    [DllImport("__Internal")]
//    private static extern float UnitySetLastHeading(double timestamp,
//                 float latitude,
//                 float longitude,
//                 float altitude,
//                 float horizontalAccuracy,
//                 float verticalAccuracy);
//#endif


    /*
    //Struct representing coordinates received from the iOS plugin.
    public struct CLLocationCoordinate2D
    {
        double latitude;
        double longitude;
    }

    //Imported function.

    [DllImport("__Internal")]
    private static extern CLLocationCoordinate2D _GetCoordinates();

    //Call the imported `GetCoordinates` when this function is called.
    public static CLLocationCoordinate2D GetCoordinates()
    {
       // return _GetCoordinates();
        return  _GetCoordinates();
    }

    */


    ////Struct representing coordinates received from the iOS plugin.
    //public struct CLLocationCoordinate2D
    //{
    //    double latitude;
    //    double longitude;
    //}

    ////Imported function.
    //[DllImport("__internal")]
    //private static extern CLLocationCoordinate2D _GetCoordinates();

    ////Call the imported `GetCoordinates` when this function is called.
    //public static CLLocationCoordinate2D GetCoordinates()
    //{
    //    return _GetCoordinates();
    //}

  




    // Use this for initialization
    void Start()
    {




        StartCoroutine(Getdata());
        Debug.Log("Next scene loaded");
        //InvokeRepeating("StartCoroutine(Uploaddata())",0,10f);


    }



    // Update is called once per frame



    void Update()
    {
        //if (GetCoordinates().ToString() != null)
        //{
        //    Debug.Log("Getcordinates Plugin " + GetCoordinates().ToString() );
        //}


       // Debug.Log("Getcordinates Plugin " + latitude.ToString() + "---" + longitude.ToString());



        if (period > nextUpdate)
        {
            //Do Stuff
            StartCoroutine(Uploaddata());

            period = 0;
        }
        period += UnityEngine.Time.deltaTime;

    }


    IEnumerator Getdata()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {

            // Access granted and location value could be retrieved

            double aa= Input.location.lastData.latitude;
            Debug.Log("Double Latitude="+aa);

            latitudes = double.Parse(Input.location.lastData.latitude.ToString("R"));
            longitudes = double.Parse(Input.location.lastData.longitude.ToString("R"));
            altitudes = double.Parse(Input.location.lastData.altitude.ToString("R"));
            deviceinfo = SystemInfo.deviceModel;
           // Debug.Log("Device values types = "+SystemInfo.deviceType+" --"+SystemInfo.deviceType);
            Locationinformation.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " - " + Input.location.lastData.horizontalAccuracy + " - " + Input.location.lastData.timestamp;
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }


    IEnumerator Uploaddata()
    {

        Debug.Log("Upload Data loop");

        double lats = latitudes;
        double longs = longitudes;
        double altis = altitudes;
        string devices = deviceinfo;
        string ids = Introscript.id;

       //Debug.Log("Current Position = " + lats + " - " + longs + " - " + altis + "Device Info = " + devices + " IDS "+ids);

        string URL1 = "http://sightica.com/casino/insert-latlon.php?lat=" + lats + "&lon=" + longs + "&alt=" + altis + "&device=" + devices + "&id=" + ids;
        WWW readjson = new WWW(URL1);
        yield return readjson;

         

       

        Debug.Log(readjson.text);


        if (string.IsNullOrEmpty(readjson.error))
        {

            JSONDataString = readjson.text;
        }
        JSONNode JNode = SimpleJSON.JSON.Parse(JSONDataString);
        upload = JNode["success"];
        if (upload == "success")
        {
            Debug.Log("Uploaded successfully");

        }
    }





}
