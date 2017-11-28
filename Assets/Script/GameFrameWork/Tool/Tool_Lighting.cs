using UnityEngine;

public class Tool_Lighting : MonoBehaviour
{
    public UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
    public Color ambientColor = Color.green;
    public float ambientIntensity = 1.0f;

    public bool fog = true;
    public Color fogColor = Color.green;
    public FogMode fogMode = FogMode.Linear;
    public float fogStartDistance = 3.0f;
    public float fogEndDistance = 30.0f;
	public bool autoRun = true;

    private void Init()
    {
        RenderSettings.ambientMode = this.ambientMode;
        RenderSettings.ambientSkyColor = this.ambientColor;
        RenderSettings.ambientIntensity = this.ambientIntensity;
		RenderSettings.fog = this.fog;
        RenderSettings.fogColor = this.fogColor;
        RenderSettings.fogMode = this.fogMode;
        RenderSettings.fogStartDistance = this.fogStartDistance;
        RenderSettings.fogEndDistance = this.fogEndDistance;
    }

    [ContextMenu("Debug")]
    void Log()
    {
       Debug.LogWarning(string.Format("ambientMode   {0}  ambientSkyColor   {1}   ambientIntensity   {2}  fogColor   {3}    fogMode {4}     fogStartDistance  {5}       fogEndDistance     {6}", RenderSettings.ambientMode, RenderSettings.ambientSkyColor, RenderSettings.ambientIntensity, RenderSettings.fogColor, RenderSettings.fogMode, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance)); 
    }

    void Start()
    {
		if(this.autoRun)
        	this.Init();
    }

	public void OpenFog()
	{
		this.Init();
	}
    
    void OnEnable()
    {
		if(this.autoRun)
			this.Init();
    }

//	void OnGUI()
//	{
//		string str = RenderSettings.fog ? "无效:开" : "雾效:关";
//		if(GUI.Button(new Rect(150,50,150,50), str))
//			RenderSettings.fog = !RenderSettings.fog;
//	}

}
