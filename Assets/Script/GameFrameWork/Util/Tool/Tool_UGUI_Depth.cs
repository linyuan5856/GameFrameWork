using UnityEngine;

public class Tool_UGUI_Depth : MonoBehaviour
{
    public int order;
    public bool isUI = true;
    void Start()
    {
        Perform();
    }

    [ContextMenu("Perform")]
    private void Perform()
    {
        if (isUI)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
        }
        else
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>();

            for (int i = 0; i < renders.Length; i++)
            {
                renders[i].sortingOrder = order;
            }          
        }
    }
}
