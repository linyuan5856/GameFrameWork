using UnityEngine;
using UnityEngine.UI;

public class Tool_CanvasScaler : MonoBehaviour
{
    CanvasScaler canvasScaler = null;

    void Awake()
    {
        this.canvasScaler = this.GetComponent<CanvasScaler>();
        this.canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        this.canvasScaler.referenceResolution = new Vector2(960, 640); 
        this.canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        float r1 = (float)Screen.width / (float)Screen.height;
        float r2 = 960.0f / 640.0f;
        //Debug.Log("Screen.width=" + Screen.width + "Screen.height=" + Screen.height
	//+ "|Screen.width / Screen.height=" + r1 + "| 960.0f / 640.0f=" + r2);

        if (r1 <= r2)
        {     
            //Debug.Log("r<=r2 matchWidthOrHeight=0");
            this.canvasScaler.matchWidthOrHeight = 0;//width
        }
        else
        {
            //Debug.Log("r1>r2 matchWidthOrHeight=1");
            this.canvasScaler.matchWidthOrHeight = 1;//height
        }

    }

}
