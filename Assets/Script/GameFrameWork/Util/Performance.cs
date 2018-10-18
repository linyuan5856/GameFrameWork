using UnityEngine;
using System.Collections;

public class Performance : MonoBehaviour {
   
    private GUIStyle style;
    private const int M = 1048576;
    void Awake() {
		
        style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.green;
    }

	void Start () {
        StartCoroutine(FPS());
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
        
        string total = ((float) UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()/ M).ToString("f2");
       
        GUI.Label(new Rect(0, temp, 300, 20),
            new GUIContent("Memory: " + total + " MB"), style);
        temp += 20;
  
//        GUI.Label(new Rect(0, temp, 300, 20),
//            new GUIContent("Cache: " + Caching.spaceOccupied/ M + " MB/" + Caching.maximumAvailableDiskSpace / 1048576 +" MB"),
//            style);
    }
}
