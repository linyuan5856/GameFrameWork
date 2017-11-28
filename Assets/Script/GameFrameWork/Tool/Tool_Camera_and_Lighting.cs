using UnityEngine;

public class Tool_Camera_and_Lighting : MonoBehaviour
{

    public Vector3 camera_position_16_9 = Vector3.zero;
    public Vector3 camera_rotation_16_9 = Vector3.zero;
    
    public UnityEngine.Rendering.AmbientMode ambientMode_16_9 = UnityEngine.Rendering.AmbientMode.Flat;
    public Color ambientColor_16_9 = Color.green;
    public float ambientIntensity_16_9 = 1.0f;

    public bool fog_16_9 = true;
    public Color fogColor_16_9 = Color.green;
    public FogMode fogMode_16_9 = FogMode.Linear;
    public float fogStartDistance_16_9 = 3.0f;
    public float fogEndDistance_16_9 = 30.0f;

    public string cap______________ = "---------------------";
    //===================================================================
    public Vector3 camera_position_3_2 = Vector3.zero;
    public Vector3 camera_rotation_3_2 = Vector3.zero;
    //Quaternion.identity;

    public UnityEngine.Rendering.AmbientMode ambientMode_3_2 = UnityEngine.Rendering.AmbientMode.Flat;
    public Color ambientColor_3_2 = Color.green;
    public float ambientIntensity_3_2 = 1.0f;

    public bool fog_3_2 = true;
    public Color fogColor_3_2 = Color.green;
    public FogMode fogMode_3_2 = FogMode.Linear;
    public float fogStartDistance_3_2 = 3.0f;
    public float fogEndDistance_3_2 = 30.0f;
	public Camera fightCamera=null;



    // Use this for initialization
    private bool inited = false;
    void Init()
    {
        if (inited) return;
        inited = true;
        float r1 = (float)Screen.width / (float)Screen.height;
        float r2 = 960.0f / 640.0f;
        //Debug.Log("Screen.width=" + Screen.width + "Screen.height=" + Screen.height + "|Screen.width / Screen.height=" + r1 + "| 960.0f / 640.0f=" + r2);

        if (r1 <= r2)
        {
            this.cap______________ = "3:2";
			this.transform.localPosition = this.camera_position_3_2;
			this.transform.eulerAngles=this.camera_rotation_3_2;
//            Camera.main.transform.eulerAngles = this.camera_rotation_3_2;

            RenderSettings.ambientMode = this.ambientMode_3_2;
            RenderSettings.ambientSkyColor = this.ambientColor_3_2;
            RenderSettings.ambientIntensity = this.ambientIntensity_3_2;
            RenderSettings.fog = this.fog_3_2;
            RenderSettings.fogColor = this.fogColor_3_2;
            RenderSettings.fogMode = this.fogMode_3_2;
            RenderSettings.fogStartDistance = this.fogStartDistance_3_2;
            RenderSettings.fogEndDistance = this.fogEndDistance_3_2;
        }
        else
        {
            this.cap______________ = "16:9";

			this.transform.localPosition = this.camera_position_16_9;
			this.transform.localRotation= Quaternion.Euler(this.camera_rotation_16_9);//

//			DelayHelper.AfterFrameDo(1,()=>{
//			this.transform.localRotation= Quaternion.Euler(this.camera_rotation_16_9);//
//			Debug.LogError("this.camera_rotation_16_9"+"x"+this.camera_rotation_16_9.x+"y"+this.camera_rotation_16_9.y+"z"+this.camera_rotation_16_9.z);
////            Camera.main.transform.eulerAngles = this.camera_rotation_16_9;
//			});
            RenderSettings.ambientMode = this.ambientMode_16_9;
            RenderSettings.ambientSkyColor = this.ambientColor_16_9;
            RenderSettings.ambientIntensity = this.ambientIntensity_16_9;
            RenderSettings.fog = this.fog_16_9;
            RenderSettings.fogColor = this.fogColor_16_9;
            RenderSettings.fogMode = this.fogMode_16_9;
            RenderSettings.fogStartDistance = this.fogStartDistance_16_9;
            RenderSettings.fogEndDistance = this.fogEndDistance_16_9;
        }
    }

    void Awake()
    {
        this.Init();
    }
    
    void Start()
    {
//        this.Init();
    }

    void OnEnable()
    {
        this.Init();
    }
}
