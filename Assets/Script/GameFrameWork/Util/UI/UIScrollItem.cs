using UnityEngine;

public class UIScrollItem:MonoBehaviour
{
    public void Init()
    {
       // mScrollDataIndex = -1;
        mRectTransform = GetComponent<RectTransform>();
        OnInit();
    }

    protected virtual void OnInit()
    {
    }

    protected virtual void OnChange()
    {
    }

    [SerializeField]
   // private int mScrollDataIndex;
    private int mScrollIndex;

    public int scrollDataIndex
    {
        get { return mScrollIndex; }
        set
        {
            mScrollIndex = value;
           // mScrollDataIndex = mScrollIndex;
        }
    }

    private object mData;
    private RectTransform mRectTransform;


    public void SetData(object _data)
    {
        mData = _data;
        OnChange();
    }

    public void Refresh()
    {
        OnChange();
    }

    public object GetData()
    {
        return mData;
    }

    public void SetPointXY(float x, float y)
    {
        mRectTransform.anchoredPosition = new Vector2(x, y);
    }

    public virtual float Width
    {
        get { return mRectTransform.rect.width; }
    }

    public virtual float Height
    {
        get { return mRectTransform.rect.height; }
    }

    public virtual void OnRecycle()
    {
        
    }

}
