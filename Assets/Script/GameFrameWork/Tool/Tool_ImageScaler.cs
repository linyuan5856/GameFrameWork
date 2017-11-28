using UnityEngine;

public class Tool_ImageScaler : MonoBehaviour
{

    public enum ImageScalerEnum
    {
        RatioThree_Two,
        Ratio1024_768
    }


    private RectTransform rt;
    public ImageScalerEnum ScalerEnum = ImageScalerEnum.RatioThree_Two;

    void Awake()
    {
        rt = this.GetComponent<RectTransform>();


        switch (ScalerEnum)
        {
            case ImageScalerEnum.RatioThree_Two:
                this.ThreeRatioTwo();
                break;
            case ImageScalerEnum.Ratio1024_768:
                this.Ratio1024_768();
                break;
            default:
                return;
        }
    }


    void Ratio1024_768()
    {
        float ro = (float)Screen.width / (float)rt.rect.width + 0.01f;
        rt.localScale = new Vector3(ro, ro, 1);

    }


    void ThreeRatioTwo()
    {
        float r1 = (float)Screen.width / (float)Screen.height;
        float r2 = 1136.0f / 640.0f;

        if (r1 <= r2)
        {
            float ro = (float)Screen.height / (float)rt.rect.height + 0.01f;
            rt.localScale = new Vector3(ro, ro, 1);
        }
        else
        {
            float ro = (float)Screen.width / (float)rt.rect.width + 0.01f;
            rt.localScale = new Vector3(ro, ro, 1);
        }
    }


}
