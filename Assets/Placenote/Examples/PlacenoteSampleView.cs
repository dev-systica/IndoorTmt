using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.XR.iOS;
using System.Runtime.InteropServices;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


public class PlacenoteSampleView : MonoBehaviour, PlacenoteListener
{
	[SerializeField] GameObject mMapSelectedPanel;
	[SerializeField] GameObject mInitButtonPanel;
	[SerializeField] GameObject mMappingButtonPanel;
	[SerializeField] GameObject mSimulatorAddShapeButton;
	[SerializeField] GameObject mMapListPanel;
	[SerializeField] GameObject mExitButton;
	[SerializeField] GameObject mListElement;
	[SerializeField] RectTransform mListContentParent;
	[SerializeField] ToggleGroup mToggleGroup;
	[SerializeField] GameObject mPlaneDetectionToggle;
	[SerializeField] Text mLabelText;
    [SerializeField] GameObject LabelPanel;
	[SerializeField] PlacenoteARGeneratePlane mPNPlaneManager;
	[SerializeField] Slider mRadiusSlider;
	[SerializeField] float mMaxRadiusSearch;
	[SerializeField] Text mRadiusLabel;

	private UnityARSessionNativeInterface mSession;
	private bool mFrameUpdated = false;
	private UnityARImageFrameData mImage = null;
	private UnityARCamera mARCamera;
	private bool mARKitInit = false;

    public GameObject FeatureText;


    //private List<ShapeInfo> shapeInfoList = new List<ShapeInfo> ();
    //private List<GameObject> shapeObjList = new List<GameObject> ();

    public InputField Mapname;
    public GameObject Mapnamestatus;
    public GameObject textmapname;
    public Text Locinfos;

    private LibPlacenote.MapMetadataSettable mCurrMapDetails;

	private bool mReportDebug = false;

	private LibPlacenote.MapInfo mSelectedMapInfo;
	private string mSelectedMapId {
		get {
			return mSelectedMapInfo != null ? mSelectedMapInfo.placeId : null;
		}
	}
	private string mSaveMapId = null;


	private BoxCollider mBoxColliderDummy;
	private SphereCollider mSphereColliderDummy;
	private CapsuleCollider mCapColliderDummy;


	// Use this for initialization
	void Start ()
	{

        Mapnamestatus.SetActive(false);
        //cloudfeature = new FeaturesVisualizer();
		Input.location.Start ();
        FeatureText.SetActive(true);
        Invoke("HideFeatures",2.5f);


		mMapListPanel.SetActive (false);
        textmapname.SetActive(false);

		mSession = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
		UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
		StartARKit ();
		FeaturesVisualizer.EnablePointcloud ();
        //newly added
        mPlaneDetectionToggle.GetComponent<Toggle>().isOn = false;

		LibPlacenote.Instance.RegisterListener (this);
		ResetSlider ();

		// for simulator
		#if UNITY_EDITOR
		mSimulatorAddShapeButton.SetActive(true);
		mPlaneDetectionToggle.SetActive(false);
		#endif

	}

    public void HideFeatures()
    {
        FeatureText.SetActive(false);

    }


	private void ARFrameUpdated (UnityARCamera camera)
	{
		mFrameUpdated = true;
		mARCamera = camera;
	}


	private void InitARFrameBuffer ()
	{
		mImage = new UnityARImageFrameData ();

		int yBufSize = mARCamera.videoParams.yWidth * mARCamera.videoParams.yHeight;
		mImage.y.data = Marshal.AllocHGlobal (yBufSize);
		mImage.y.width = (ulong)mARCamera.videoParams.yWidth;
		mImage.y.height = (ulong)mARCamera.videoParams.yHeight;
		mImage.y.stride = (ulong)mARCamera.videoParams.yWidth;

		// This does assume the YUV_NV21 format
		int vuBufSize = mARCamera.videoParams.yWidth * mARCamera.videoParams.yWidth/2;
		mImage.vu.data = Marshal.AllocHGlobal (vuBufSize);
		mImage.vu.width = (ulong)mARCamera.videoParams.yWidth/2;
		mImage.vu.height = (ulong)mARCamera.videoParams.yHeight/2;
		mImage.vu.stride = (ulong)mARCamera.videoParams.yWidth;

		mSession.SetCapturePixelData (true, mImage.y.data, mImage.vu.data);
	}


	// Update is called once per frame
	void Update ()
	{
		if (mFrameUpdated) {
			mFrameUpdated = false;
			if (mImage == null) {
				InitARFrameBuffer ();
			}

			if (mARCamera.trackingState == ARTrackingState.ARTrackingStateNotAvailable) {
				// ARKit pose is not yet initialized
				return;
			} else if (!mARKitInit && LibPlacenote.Instance.Initialized()) {
				mARKitInit = true;
				//mLabelText.text = "ARKit Initialized";
                mLabelText.text = "App Initialized";
			}

			Matrix4x4 matrix = mSession.GetCameraPose ();

			Vector3 arkitPosition = PNUtility.MatrixOps.GetPosition (matrix);
			Quaternion arkitQuat = PNUtility.MatrixOps.GetRotation (matrix);

			LibPlacenote.Instance.SendARFrame (mImage, arkitPosition, arkitQuat, mARCamera.videoParams.screenOrientation);
		}
	}


	public void OnListMapClick ()
	{
		if (!LibPlacenote.Instance.Initialized()) {
			Debug.Log ("SDK not yet initialized");
			return;
		}

		foreach (Transform t in mListContentParent.transform) {
			Destroy (t.gameObject);
		}


		mMapListPanel.SetActive (true);
		mInitButtonPanel.SetActive (false);
		mRadiusSlider.gameObject.SetActive (true);
		LibPlacenote.Instance.ListMaps ((mapList) => {
			// render the map list!
			foreach (LibPlacenote.MapInfo mapId in mapList) {
				if (mapId.metadata.userdata != null) {
					Debug.Log(mapId.metadata.userdata.ToString (Formatting.None));
				}
				AddMapToList (mapId);
			}
		});
	}

	public void OnRadiusSelect ()
	{
		Debug.Log ("Map search:" + mRadiusSlider.value.ToString("F2"));
		LocationInfo locationInfo = Input.location.lastData;


		float radiusSearch = mRadiusSlider.value * mMaxRadiusSearch;
		mRadiusLabel.text = "Distance Filter: " + (radiusSearch / 1000.0).ToString ("F2") + " km";

		LibPlacenote.Instance.SearchMaps(locationInfo.latitude, locationInfo.longitude, radiusSearch, 
			(mapList) => {
			foreach (Transform t in mListContentParent.transform) {
				Destroy (t.gameObject);
			}
			// render the map list!
			foreach (LibPlacenote.MapInfo mapId in mapList) {
				if (mapId.metadata.userdata != null) {
					Debug.Log(mapId.metadata.userdata.ToString (Formatting.None));
				}
				AddMapToList (mapId);
			}
		});
	}

	public void ResetSlider() {
		mRadiusSlider.value = 1.0f;
		mRadiusLabel.text = "Distance Filter: Off";
	}

	public void OnCancelClick ()
	{
		mMapSelectedPanel.SetActive (false);
		mMapListPanel.SetActive (false);
		mInitButtonPanel.SetActive (true);
		ResetSlider ();
	}


	public void OnExitClick ()
	{
		mInitButtonPanel.SetActive (true);
		mExitButton.SetActive (false);
		//mPlaneDetectionToggle.SetActive (false);
		mMappingButtonPanel.SetActive (false);

		//clear all existing planes
		mPNPlaneManager.ClearPlanes ();
		mPlaneDetectionToggle.GetComponent<Toggle>().isOn = false;

		LibPlacenote.Instance.StopSession ();
	}


	void AddMapToList (LibPlacenote.MapInfo mapInfo)
	{
		GameObject newElement = Instantiate (mListElement) as GameObject;
		MapInfoElement listElement = newElement.GetComponent<MapInfoElement> ();
		listElement.Initialize (mapInfo, mToggleGroup, mListContentParent, (value) => {
			OnMapSelected (mapInfo);
		});
	}


	void OnMapSelected (LibPlacenote.MapInfo mapInfo)
	{
		mSelectedMapInfo = mapInfo;
		mMapSelectedPanel.SetActive (true);
		mRadiusSlider.gameObject.SetActive (false);
	}


	public void OnLoadMapClicked ()
    {


       
        
		ConfigureSession (false);
        Debug.Log("Inside OnLoadMapClicked ******* ");
		if (!LibPlacenote.Instance.Initialized()) {
			Debug.Log ("SDK not yet initialized");
			return;
		}

		ResetSlider ();
		mLabelText.text = "Loading Map ID: " + mSelectedMapId;
		LibPlacenote.Instance.LoadMap (mSelectedMapId,
			(completed, faulted, percentage) => {
				if (completed) {
					mMapSelectedPanel.SetActive (false);
					mMapListPanel.SetActive (false);
					mInitButtonPanel.SetActive (false);

                //was true chaged to false
					mMappingButtonPanel.SetActive(false);


                //**

                //**
					mExitButton.SetActive (false);
					mPlaneDetectionToggle.SetActive(false);



                FeaturesVisualizer.DisablePointcloud();
                    //LibPlacenote.Instance.MyStopSession();
                    mPNPlaneManager.ClearPlanes();
                    mPlaneDetectionToggle.GetComponent<Toggle>().isOn = false;
                    mMappingButtonPanel.SetActive(false);

					LibPlacenote.Instance.StartSession (true);
                   

					if (mReportDebug) {
						LibPlacenote.Instance.StartRecordDataset (
							(datasetCompleted, datasetFaulted, datasetPercentage) => {

								if (datasetCompleted) {
									mLabelText.text = "Dataset Upload Complete";
                                    

								} else if (datasetFaulted) {
									mLabelText.text = "Dataset Upload Faulted";
								} else {
									mLabelText.text = "Dataset Upload: " + datasetPercentage.ToString ("F2") + "/1.0";
								}
							});
						Debug.Log ("Started Debug Report");
					}

					mLabelText.text = "Loaded ID: " + mSelectedMapId;
				} else if (faulted) {
					mLabelText.text = "Failed to load ID: " + mSelectedMapId;
				} else {
					mLabelText.text = "Map Download: " + percentage.ToString ("F2") + "/1.0";
				}
			}
		);
	}

	public void OnDeleteMapClicked ()
	{
		if (!LibPlacenote.Instance.Initialized()) {
			Debug.Log ("SDK not yet initialized");
			return;
		}

		mLabelText.text = "Deleting Map ID: " + mSelectedMapId;
		LibPlacenote.Instance.DeleteMap (mSelectedMapId, (deleted, errMsg) => {
			if (deleted) {
				mMapSelectedPanel.SetActive (false);
				mLabelText.text = "Deleted ID: " + mSelectedMapId;
				OnListMapClick();
			} else {
				mLabelText.text = "Failed to delete ID: " + mSelectedMapId;
			}
		});
	}



	public void OnNewMapClick ()
	{
		ConfigureSession (false);

		if (!LibPlacenote.Instance.Initialized()) {
			Debug.Log ("SDK not yet initialized");
			return;
		}

		mInitButtonPanel.SetActive (false);
		mMappingButtonPanel.SetActive (true);
        mPlaneDetectionToggle.GetComponent<Toggle>().isOn = true;

		//mPlaneDetectionToggle.SetActive (true);

		Debug.Log ("Started Session");
		LibPlacenote.Instance.StartSession ();

		if (mReportDebug) {
			LibPlacenote.Instance.StartRecordDataset (
				(completed, faulted, percentage) => {
					if (completed) {
						mLabelText.text = "Dataset Upload Complete";
					} else if (faulted) {
						mLabelText.text = "Dataset Upload Faulted";
					} else {
						mLabelText.text = "Dataset Upload: (" + percentage.ToString ("F2") + "/1.0)";
					}
				});
			Debug.Log ("Started Debug Report");
		}

	}

	public void OnTogglePlaneDetection() {
		ConfigureSession (true);
	}

	private void StartARKit ()
	{
		#if !UNITY_EDITOR
		//mLabelText.text = "Initializing ARKit";
        mLabelText.text = "Initializing App";
		Application.targetFrameRate = 60;
		ConfigureSession (false);
		#endif
	}


	private void ConfigureSession(bool clearPlanes) {
 		#if !UNITY_EDITOR
		ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration ();

		if (mPlaneDetectionToggle.GetComponent<Toggle>().isOn) {
			if (UnityARSessionNativeInterface.IsARKit_1_5_Supported ()) {
				//config.planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
                  config.planeDetection = UnityARPlaneDetection.Horizontal;
			} else {
				config.planeDetection = UnityARPlaneDetection.Horizontal;
			}
			mPNPlaneManager.StartPlaneDetection ();
		} else {
			config.planeDetection = UnityARPlaneDetection.None;
			if (clearPlanes) {
				mPNPlaneManager.ClearPlanes ();
			}
		}

		config.alignment = UnityARAlignment.UnityARAlignmentGravity;
		config.getPointCloudData = true;
		config.enableLightEstimation = true;
		mSession.RunWithConfig (config);
 		#endif
	}




    public void PickLatLong()
    {
        bool useLocation = Input.location.status == LocationServiceStatus.Running;
        LocationInfo locationInfo = Input.location.lastData;


        string lats = locationInfo.latitude.ToString();
        string longs = locationInfo.longitude.ToString();
        string alti = locationInfo.altitude.ToString();

        Locinfos.text = lats + " - " + longs + " - " + alti;
        Debug.Log("Loc Info"+lats + " - " + longs + " - " + alti);


        //if (useLocation)
        //{
        //    metadata.location = new LibPlacenote.MapLocation();
        //    metadata.location.latitude = locationInfo.latitude;
        //    metadata.location.longitude = locationInfo.longitude;
        //    metadata.location.altitude = locationInfo.altitude;
        //}
    }

    public void OnSaveMapClick()
    {
        Mapnamestatus.SetActive(true);
        if(Mapname.text!="")
        {
        if (!LibPlacenote.Instance.Initialized())
        {
            Debug.Log("SDK not yet initialized");
            return;
        }

        Debug.Log("Inside OnSaveMapClick ******* ");
        bool useLocation = Input.location.status == LocationServiceStatus.Running;
        LocationInfo locationInfo = Input.location.lastData;

        mLabelText.text = "Saving...";
        LibPlacenote.Instance.SaveMap(
            (mapId) =>
            {
                LibPlacenote.Instance.StopSession();
                mSaveMapId = mapId;
                mInitButtonPanel.SetActive(true);
                mMappingButtonPanel.SetActive(false);
                mExitButton.SetActive(false);
                //mPlaneDetectionToggle.SetActive (false);

                //clear all existing planes
                mPNPlaneManager.ClearPlanes();
                mPlaneDetectionToggle.GetComponent<Toggle>().isOn = false;

                LibPlacenote.MapMetadataSettable metadata = new LibPlacenote.MapMetadataSettable();
                //metadata.name = RandomName.Get ();
                metadata.name = Mapname.text;
                mLabelText.text = "Saved Map Name: " + metadata.name;

                JObject userdata = new JObject();


                JObject shapeList = GetComponent<ShapeManager>().Shapes2JSON();
                foreach (var eachShapevalue in shapeList)
                {
                    var shapevalue = eachShapevalue.Value;
                    var shapeKey = eachShapevalue.Key;
                    var shapeString = eachShapevalue.ToString();
                    Debug.Log("shape count is " + shapeList.Count.ToString());
                    Debug.Log("Value issssssssssssss : " + shapevalue + ";" + shapeKey + ";" + shapeString);
                }


                userdata["shapeList"] = shapeList;
                metadata.userdata = userdata;

                if (useLocation)
                {
                    metadata.location = new LibPlacenote.MapLocation();
                    metadata.location.latitude = locationInfo.latitude;
                    metadata.location.longitude = locationInfo.longitude;
                    metadata.location.altitude = locationInfo.altitude;
                }
                LibPlacenote.Instance.SetMetadata(mapId, metadata, (success) =>
                {
                    if (success)
                    {
                        Debug.Log("Meta data successfully saved");
                    }
                    else
                    {
                        Debug.Log("Meta data failed to save");
                    }
                });
                mCurrMapDetails = metadata;
            },
            (completed, faulted, percentage) =>
            {
                if (completed)
                {
                    mLabelText.text = "Upload Complete:" + mCurrMapDetails.name;
                }
                else if (faulted)
                {
                    mLabelText.text = "Upload of Map Named: " + mCurrMapDetails.name + "faulted";
                }
                else
                {
                    mLabelText.text = "Uploading Map Named: " + mCurrMapDetails.name + "(" + percentage.ToString("F2") + "/1.0)";
                }
            }
        );
        Debug.Log("Exiting OnSaveMapClick ******* ");


    }

        else 
        {
            Debug.Log("Enter Map Name");

            textmapname.SetActive(true);

            Invoke("BoolGUIs",2f);

        }
	}



    void BoolGUIs()
    {
        textmapname.SetActive(false);
    }


	public void OnPose (Matrix4x4 outputPose, Matrix4x4 arkitPose) {}

	public void OnStatusChange (LibPlacenote.MappingStatus prevStatus, LibPlacenote.MappingStatus currStatus)
	{
		Debug.Log ("prevStatus: " + prevStatus.ToString() + " currStatus: " + currStatus.ToString());
		if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.LOST) {
			mLabelText.text = "Localized";
            //FeaturesVisualizer.DisablePointcloud();
            ////LibPlacenote.Instance.MyStopSession();
            //mPNPlaneManager.ClearPlanes();
            //mPlaneDetectionToggle.GetComponent<Toggle>().isOn = false;
            //mMappingButtonPanel.SetActive(false);
            mLabelText.enabled = false;
            LabelPanel.SetActive(false);

            GetComponent<ShapeManager>().LoadShapesJSON (mSelectedMapInfo.metadata.userdata);
		} else if (currStatus == LibPlacenote.MappingStatus.RUNNING && prevStatus == LibPlacenote.MappingStatus.WAITING) {
			mLabelText.text = "Mapping";
		} else if (currStatus == LibPlacenote.MappingStatus.LOST) {
			mLabelText.text = "Searching for position lock";
		} else if (currStatus == LibPlacenote.MappingStatus.WAITING) {
            if (GetComponent<ShapeManager>().shapeObjList.Count != 0) {
                GetComponent<ShapeManager>().ClearShapes ();
			}
		}
	}

	void OnApplicationQuit()
	{
		LibPlacenote.Instance.Shutdown();
	}



   public void OnQuit()
    {
        System.Environment.Exit(0);
       
    }


}
