using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using SimpleJSON;
using System;

public class Introscript : MonoBehaviour {

    public GameObject firstimg;
    public GameObject secondimg;
    public GameObject panel1;

    public InputField Uname;
    public InputField pwd;

    string Usernames;
    string pwds;
    public static string id;
    string login;

    public Toggle Remember;

    int loginstatus=0;


    string JSONDataString;

	// Use this for initialization
	void Start ()
    {
        //loginstatus= PlayerPrefs.GetInt("success");
       //secondimg.SetActive(false);
        panel1.SetActive(false);


            Uname.text = PlayerPrefs.GetString("Uname");
            pwd.text = PlayerPrefs.GetString("Pword");
       
		
	}



    public void Submit()
    {
        Usernames = Uname.text;
        pwds = pwd.text;
      




        if (Remember.isOn == true)
        {
            PlayerPrefs.SetString("Uname", Usernames);
            PlayerPrefs.SetString("Pword", pwds);
           
        }
        else if (Remember.isOn == false)
        {


            Uname.text = "";
            pwd.text = "";
            PlayerPrefs.DeleteAll();


        }



        StartCoroutine(UploadLogin());

    }

    IEnumerator UploadLogin()
    {

        string URL = "http://sightica.com/casino/login.php?email=" + Usernames + "&password=" + pwds;
        WWW readjson = new WWW(URL);         yield return readjson;         Debug.Log(readjson.text);


        if (string.IsNullOrEmpty(readjson.error))         {              JSONDataString = readjson.text;         } 

        JSONNode JNode = SimpleJSON.JSON.Parse(JSONDataString);

         login = JNode["success"];
            id = JNode["id"];
        Debug.Log("Jnode values = " + login + " " + id);


        if (login == "success")
        {
           // panel1.SetActive(false);
            //firstimg.SetActive(false);
            firstimg.SetActive(false);
            panel1.SetActive(false);
            PlayerPrefs.SetInt("success", 1);
           // PlayerPrefs.SetString("Uname", Usernames);
            //PlayerPrefs.SetString("Pword", pwds);
            //secondimg.SetActive(true);
            StartCoroutine(LoadNewScene());

           
        }
        else if(login == "fail")
        {
            
            PlayerPrefs.SetInt("success", 0);
            PlayerPrefs.SetString("Uname", "");
            PlayerPrefs.SetString("Pword", "");

        }
        
    }
	// Update is called once per frame
	void Update ()
    {


	}

    public void First()
    {
        firstimg.SetActive(false);
        panel1.SetActive(true);
       // secondimg.SetActive(true);
    }


    public void Second()
    {
       // firstimg.SetActive(false);
       // secondimg.SetActive(false);
       // StartCoroutine(LoadNewScene());
       // SceneManager.LoadScene(1);

    }



    public void ToggleChanged()
    {


    }

    IEnumerator LoadNewScene()
    {
        
        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        yield return new WaitForSeconds(0.25f);

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(1);
    //SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            //firstimg.SetActive(true);
          //  secondimg.SetActive(true);
            yield return null;
        }





        /*
        yield return null;


        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("SampleScene");
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            firstimg.SetActive(true);
            asyncOperation.allowSceneActivation = true;
        }


       

        yield return null;

        */
    }


}
