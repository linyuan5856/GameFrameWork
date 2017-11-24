using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Performance : MonoBehaviour {

    public static Performance instance;


    private GUIStyle style;
    void Awake() {
		instance = this;
        style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.green;
    }

	void Start () {
        StartCoroutine(FPS());
	}
	
	void Update () {
	
	}

    int mFps;
    IEnumerator FPS()
    {
        while (true)
        {
            float begintime = Time.time;
            int beginframe = Time.frameCount;

            yield return new WaitForSeconds(1.0f);

            float timesp = Time.time - begintime;
            int framesp = Time.frameCount - beginframe;

            mFps = (int)(framesp / (timesp));
        }
    }

    void OnGUI()
    {
        float temp = Screen.height - 80;
        GUI.Label(new Rect(0,temp, 300, 20), new GUIContent("FPS: " + mFps.ToString()),style);
        temp += 20;
        
        string total = ((float) UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory()/1048576).ToString("f2");
       
        GUI.Label(new Rect(0, temp, 300, 20),
            new GUIContent("Memory: " + total + " MB"), style);
        temp += 20;
   
        GUI.Label(new Rect(0, temp, 300, 20),
            new GUIContent("Cache: " + Caching.spaceOccupied/1048576 + " MB/" + Caching.maximumAvailableDiskSpace / 1048576 +" MB"),
            style);
    }
}
