using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pandora
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public enum Movement
        {
            Horizontal,
            Vertical,
        }

        public enum DragEffect
        {
            Spring,
            OneByOne,
            PageByPage,
        }

        public enum CellType
        {
            Variable,
            Const
        }

        private class CachePosition
        {
            public float startX;
            public float startY;
            public float endX;
            public float endY;
            public int dataIndex;
           // public float cacheTime;
        }

        public Action<int> actionMoveStart;
        public Action<int> actionMoveComplete;
        public Action<GameObject, object> actionElementClick;


        private LinkedList<UIScrollItem> mScrollItemList = new LinkedList<UIScrollItem>();
        private Queue<UIScrollItem> mCacheItemQueue = new Queue<UIScrollItem>();
        [SerializeField]
        private List<object> mDataList = new List<object>();
        private List<CachePosition> mCacheList = new List<CachePosition>();

        private ScrollRect mScrollRect;
        private RectTransform mScrollTransform;
        private RectTransform mRectTransform;
        //private RectMask2D mRectMask2D;
        private RectTransform mContentTramform;
        //
        private int mCurrentShowIndex = 0;
        private int mScrollToIndex = -1;
        private bool mIsMoving = false;
        private float mSpeed = 6f;
        private Vector2 mCacheContentPos = new Vector2(float.MaxValue, float.MaxValue);

        //
        [SerializeField]
        private GameObject mItemTemplate;
        private RectTransform mTemplateTranform;
        //
        [SerializeField] private DragEffect mDragEffect;
        [SerializeField] private Movement mMovement = Movement.Vertical;
        [SerializeField] private CellType mCellType = CellType.Const;

        [SerializeField] private int cellSpace;
        [SerializeField] private int cellItemCount = -1;
        [SerializeField] private int cellItemSpace = 0;
        [SerializeField] private int dragMoveItemCount = 1;

        private bool mDataChanged = false;

        void Awake()
        {
            tryInit();
        }

        void OnDestory()
        {
            ClearData();
        }

        public int currentShowIndex
        {
            get
            {
                return mCurrentShowIndex;
            }
        }

        private bool inited = false;
        private void tryInit()
        {
            if (inited) return;
            inited = true;
            mScrollRect = GetComponent<ScrollRect>();
            mRectTransform = GetComponent<RectTransform>();
            //mRectMask2D = GetComponent<RectMask2D>();
            mContentTramform = mScrollRect.content;
            SetArchorAndPivot();
        }

        public void ClearData()
        {
            mDataList.Clear();
            mCacheList.Clear();
            clearItem(false);
        }

        public void SetDataList<T>(List<T> _list)
        {
            ClearData();

            for (int i = 0; i < _list.Count; i++)
            {
                mDataList.Add(_list[i]);
            }
            tryDryMoveForward();
            mDataChanged = true;
        }

        public void ResetPosition()
        {
            tryInit();
            clearItem();
            if (mMovement == Movement.Vertical)
                mScrollRect.verticalNormalizedPosition = 1;
            else
                mScrollRect.horizontalNormalizedPosition = 0;

            moveForward();

        }

        public void RefreshData()
        {
            foreach (var item in mScrollItemList)
            {
                item.Refresh();
            }
        }

        public bool MoveToIndex(int index)
        {
            //if (mIsMoving) return false;
            mScrollToIndex = index;
            mCurrentShowIndex = index;
            mIsMoving = true;
            if (actionMoveStart != null)
                actionMoveStart(mScrollToIndex);
            return true;
        }

        public void SkipToLast()
        {
            SkipToIndex(mDataList.Count - 1);
        }
        public void SkipToIndex(int index)
        {
            //
            CachePosition cachePos = getCacheInfo(index);
            if (cachePos == null)
            {
                GameLogger.LogWarn(string.Format("skip to index {0} failed,for cache pos is null", index));
                index = mDataList.Count - 1;
                cachePos = getCacheInfo(index);
                if (cachePos == null) return;
            }
            Vector2 pos = getContentCenterPos(cachePos);
            mContentTramform.anchoredPosition = pos;

            clearItem();
            UIScrollItem scrollItem = getItem();
            object data = mDataList[index];
            scrollItem.scrollDataIndex = index;
            scrollItem.SetData(data);
            mScrollItemList.AddFirst(scrollItem);
            scrollItem.SetPointXY(cachePos.startX, cachePos.startY);

            mCurrentShowIndex = index;

            moveForward();
            moveBack();
        }

        private Vector2 getContentCenterPos(CachePosition cachePos)
        {
            Vector2 pos = mContentTramform.anchoredPosition;

            if (mMovement == Movement.Vertical)
            {
                float maxPos = mContentTramform.rect.height - mRectTransform.rect.height;
                pos.y = -cachePos.startY + (Mathf.Abs(cachePos.endY - cachePos.startY) - mRectTransform.rect.height) / 2;
                if (pos.y < 0) pos.y = 0;
                else if (pos.y > maxPos) pos.y = maxPos;
            }
            else
            {
                float minPos = -(mContentTramform.rect.width - mRectTransform.rect.width);
                pos.x = -cachePos.startX - (Mathf.Abs(cachePos.endX - cachePos.startX) - mRectTransform.rect.width) / 2;
                if (pos.x > 0) pos.x = 0;
                else if (pos.x < minPos) pos.x = minPos;
            }
            return pos;
        }

        private Vector2 getContentStartPos(CachePosition cachePos)
        {
            Vector2 pos = mContentTramform.anchoredPosition;

            if (mMovement == Movement.Vertical)
            {
                //float maxPos = mContentTramform.rect.height - mRectTransform.rect.height;
                pos.y = -cachePos.startY;
                //if (pos.y < 0) pos.y = 0;
                //else if (pos.y > maxPos) pos.y = maxPos;
            }
            else
            {
                //float minPos = -(mContentTramform.rect.width - mRectTransform.rect.width);
                pos.x = -cachePos.startX;
                //if (pos.x > 0) pos.x = 0;
                //else if (pos.x < minPos) pos.x = minPos;
            }
            return pos;
        }

        public T GetScrollItem<T>(int index) where T : UIScrollItem
        {
            foreach (var item in mScrollItemList)
            {
                if (item.scrollDataIndex == index)
                {
                    return (T)item;
                }
            }
            return default(T);
        }

        private UIScrollItem createItem()
        {
            GameObject go = GameObject.Instantiate(getTemplate());
            go.transform.SetParent(mContentTramform.transform);
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
            UIScrollItem item = go.GetComponent<UIScrollItem>();
            item.Init();
            return item;
        }

        private GameObject getTemplate()
        {
            if (mItemTemplate == null && mContentTramform.childCount > 0)
            {
                mItemTemplate = mContentTramform.Find("template").gameObject;
            }
            if (mItemTemplate.activeSelf)
                mItemTemplate.SetActive(false);

            //if (mItemTemplate == null)
            //    mItemTemplate = LoaderManager.Instance.LoadAsset<GameObject>("hmitem");
            return mItemTemplate;
        }

        private RectTransform getTemplateTransform()
        {
            if (mTemplateTranform == null)
            {
                mTemplateTranform = getTemplate().GetComponent<RectTransform>();
            }
            return mTemplateTranform;
        }

        private UIScrollItem getItem()
        {
            UIScrollItem scrollItem = null;

            if (mCacheItemQueue.Count > 0)
                scrollItem = mCacheItemQueue.Dequeue();
            else
            {
                scrollItem = createItem();
            }
            if (!scrollItem.gameObject.activeSelf)
                scrollItem.gameObject.SetActive(true);
            return scrollItem;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (mDragEffect == DragEffect.OneByOne
                || mDragEffect == DragEffect.PageByPage)
            {
                if (mDragEffect == DragEffect.OneByOne) dragMoveItemCount = 1;

                mScrollRect.enabled = false;

                int tempScrollIndex = -1;
                if (mMovement == Movement.Vertical)
                {
                    tempScrollIndex = eventData.delta.y > 0
                        ? mCurrentShowIndex + dragMoveItemCount
                        : mCurrentShowIndex - dragMoveItemCount;
                }
                else
                {
                    tempScrollIndex = eventData.delta.x < 0
                        ? mCurrentShowIndex + dragMoveItemCount
                        : mCurrentShowIndex - dragMoveItemCount;
                }
                //Logger.LogTest("HMScrollView OnBeginDrag：" + tempScrollIndex);
                CachePosition cachePos = getCacheInfo(tempScrollIndex);
                if (cachePos != null)
                {
                    MoveToIndex(tempScrollIndex);
                }
                //
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (mDragEffect == DragEffect.OneByOne)
            {
                mScrollRect.enabled = true;
            }
        }

        private void onValueChange()
        {
            moveForward();
            moveBack();
        }
        void Update()
        {
            //
            if (mScrollToIndex > -1)
            {
                CachePosition cachePos = getCacheInfo(mScrollToIndex);
                if (cachePos == null)
                {
                    mScrollToIndex = -1;
                    return;
                }

                Vector2 targetPos = Vector2.zero;

                if (mDragEffect == DragEffect.OneByOne)
                {
                    targetPos = getContentCenterPos(cachePos);
                }
                else if (mDragEffect == DragEffect.PageByPage)
                {
                    targetPos = getContentStartPos(cachePos);
                }
                Vector2 pos = mContentTramform.anchoredPosition;

                if (mMovement == Movement.Vertical)
                {
                    pos.y = Mathf.Lerp(pos.y, targetPos.y,
                        Time.deltaTime * mSpeed);

                    mContentTramform.anchoredPosition = pos;
                    float distance = Mathf.Abs(targetPos.y - pos.y);
                    if (distance < 1f) mIsMoving = false;
                    if (distance <= 0.1f)
                    {
                        //Logger.LogTest("End ScrollToIndex :" + mScrollToIndex);
                        if (actionMoveComplete != null)
                            actionMoveComplete(mScrollToIndex);

                        mScrollToIndex = -1;
                    }
                }
                else
                {
                    pos.x = Mathf.Lerp(pos.x, targetPos.x,
                        Time.deltaTime * mSpeed);

                    mContentTramform.anchoredPosition = pos;
                    float distance = Mathf.Abs(targetPos.x - pos.x);
                    if (distance < 1f) mIsMoving = false;
                    if (distance <= 0.1f)
                    {
                        // Logger.LogTest("End ScrollToIndex :" + mScrollToIndex);
                        if (actionMoveComplete != null)
                            actionMoveComplete(mScrollToIndex);
                        mScrollToIndex = -1;
                    }
                }
            }
            else
            {
                if (mIsMoving)
                    mIsMoving = false;
            }
            //
            if (!mContentTramform.anchoredPosition.Equals(mCacheContentPos)
                || mDataChanged)
            {
                mCacheContentPos = mContentTramform.anchoredPosition;
                onValueChange();
            }
        }

        private void moveForward()
        {
            mDataChanged = false;
            while (true)
            {
                if (!recycleScrollItem(true)) break;
            }
            int count = 0;
            while (true)
            {
                if (!moveForwardOneStep())
                {
                    break;
                }
                count++;
                if (count > 50)
                {
                    GameLogger.LogWarn("move forward too much item");
                    break;
                }
            }
            //if (count > 0)
            //    Logger.LogTest(string.Format("move forward {0} item", count));
        }
        private void moveBack()
        {
            mDataChanged = false;
            while (true)
            {
                if (!recycleScrollItem(false)) break;
            }
            int count = 0;
            while (true)
            {
                if (!moveBackOneStep())
                {
                    break;
                }
                count++;
                if (count > 50)
                {
                    GameLogger.LogWarn("move back too much item");
                    break;
                }
            }
            //if (count > 0)
            //    Logger.LogTest(string.Format("move back {0} item", count));
        }

        private void resetContentSize()
        {
            CachePosition lastCachePos = getCacheInfo(mCacheList.Count - 1);
            if (lastCachePos != null)
            {
                Vector2 size = mContentTramform.sizeDelta;
                if (mMovement == Movement.Vertical)
                    size.y = Mathf.Abs(lastCachePos.endY);
                else
                    size.x = Mathf.Abs(lastCachePos.endX);

                mContentTramform.sizeDelta = size;
            }
        }

        private bool recycleScrollItem(bool forward)
        {
            if (forward)
            {
                LinkedListNode<UIScrollItem> node = mScrollItemList.First;
                if (node == null) return false;
                UIScrollItem item = node.Value;
                CachePosition cachePos = getCacheInfo(item.scrollDataIndex);

                float startPos = mMovement == Movement.Vertical ? cachePos.endY : cachePos.endX;
                float ViewRangeMin = getContentViewRange().x;

                if ((mMovement == Movement.Vertical && startPos > ViewRangeMin)
                    || (mMovement == Movement.Horizontal && startPos < ViewRangeMin))
                {
                    mScrollItemList.RemoveFirst();
                    recycleOne(item);
                    return true;
                }
            }
            else
            {
                LinkedListNode<UIScrollItem> node = mScrollItemList.Last;
                if (node == null) return false;
                UIScrollItem item = node.Value;
                CachePosition cachePos = getCacheInfo(item.scrollDataIndex);

                float endPos = mMovement == Movement.Vertical ? cachePos.startY : cachePos.startX;
                float ViewRangeMax = getContentViewRange().y;
                if ((mMovement == Movement.Vertical && endPos < ViewRangeMax)
                    || (mMovement == Movement.Horizontal && endPos > ViewRangeMax))
                {
                    mScrollItemList.RemoveLast();
                    recycleOne(item);
                    return true;
                }
            }
            return false;
        }

        private bool moveBackOneStep()
        {
            float ViewRangeMin = getContentViewRange().x;
            int nowStartIndex = getNowStartIndex();

            CachePosition startCache = getCacheInfo(nowStartIndex);

            float startPos = 0;
            if (mMovement == Movement.Vertical)
                startPos = startCache == null ? 0 : startCache.startY;
            else
                startPos = startCache == null ? 0 : startCache.startX;

            //

            if (nowStartIndex <= 0) return false;
            int newStartIndex = nowStartIndex - 1;
            int cellItemIndex = cellItemCount > 1 ? newStartIndex % cellItemCount : 0;

            if (cellItemIndex < cellItemCount - 1
                || (mMovement == Movement.Vertical && startPos < ViewRangeMin)
                || (mMovement == Movement.Horizontal && startPos > ViewRangeMin))
            {
                UIScrollItem scrollItem = getItem();
                object data = mDataList[newStartIndex];
                scrollItem.scrollDataIndex = newStartIndex;
                scrollItem.SetData(data);
                mScrollItemList.AddFirst(scrollItem);

                CachePosition nowCache = getCacheInfo(newStartIndex);
                if (nowCache == null)
                {
                    GameLogger.LogError("HMScrollView move back cannot find cache position");
                }
                scrollItem.SetPointXY(nowCache.startX, nowCache.startY);
                //
                return true;
            }
            return false;
        }



        private bool moveForwardOneStep()
        {
            float ViewRangeMax = getContentViewRange().y;
            int nowEndIndex = getNowEndIndex();
            int nextEndIndex = nowEndIndex + 1;

            CachePosition lastCache = getCacheInfo(nowEndIndex);

            float lastPos = 0;

            if (mMovement == Movement.Vertical)
                lastPos = lastCache == null ? 0 : lastCache.endY - cellSpace;
            else
                lastPos = lastCache == null ? 0 : lastCache.endX + cellSpace;
            //
            if (nextEndIndex >= mDataList.Count) return false;

            int cellItemIndex = cellItemCount > 1 ? nextEndIndex % cellItemCount : 0;

            if (cellItemIndex != 0
                || (mMovement == Movement.Vertical && lastPos > ViewRangeMax)
                || (mMovement == Movement.Horizontal && lastPos < ViewRangeMax))
            {
                UIScrollItem scrollItem = getItem();
                object data = mDataList[nextEndIndex];
                scrollItem.scrollDataIndex = nextEndIndex;
                scrollItem.SetData(data);
                mScrollItemList.AddLast(scrollItem);

                CachePosition nowCache = getCacheInfo(nextEndIndex);
                if (nowCache == null)
                {
                    nowCache = new CachePosition();
                    float h = scrollItem.Height;
                    if (h < 5) h = 5;
                    float w = scrollItem.Width;
                    if (w < 5) w = 5;
                    if (mMovement == Movement.Vertical)
                    {
                        if (cellItemIndex == 0)
                        {
                            nowCache.startY = lastPos;
                            nowCache.endY = nowCache.startY - h;
                            nowCache.startX = 0;
                            nowCache.endX = nowCache.startX + w;
                        }
                        else
                        {
                            nowCache.startY = lastCache.startY;
                            nowCache.endY = lastCache.endY;
                            nowCache.startX = lastCache.endX + cellItemSpace;
                            nowCache.endX = nowCache.startX + w;
                        }
                    }
                    else
                    {
                        if (cellItemIndex == 0)
                        {
                            nowCache.startX = lastPos;
                            nowCache.endX = nowCache.startX + w;
                            nowCache.startY = 0;
                            nowCache.endY = nowCache.startY - h;
                        }
                        else
                        {
                            nowCache.startX = lastCache.startX;
                            nowCache.endX = lastCache.endX;
                            nowCache.startY = lastCache.endY - cellItemSpace;
                            nowCache.endY = nowCache.startY - h;
                        }
                    }
                    nowCache.dataIndex = nextEndIndex;
                    setCacheInfo(nextEndIndex, nowCache);
                }

                scrollItem.SetPointXY(nowCache.startX, nowCache.startY);
                return true;
            }
            return false;
        }
        private void tryDryMoveForward()
        {
            tryInit();
            if (mCellType != CellType.Const) return;

            int startIndex = mCacheList.Count;
            for (int i = startIndex; i < mDataList.Count; i++)
            {
                int nowEndIndex = i - 1;
                int nextEndIndex = i;

                CachePosition lastCache = getCacheInfo(nowEndIndex);

                float lastPos = 0;

                if (mMovement == Movement.Vertical)
                    lastPos = lastCache == null ? 0 : lastCache.endY - cellSpace;
                else
                    lastPos = lastCache == null ? 0 : lastCache.endX + cellSpace;

                CachePosition nowCache = getCacheInfo(nextEndIndex);
                if (nowCache == null)
                {
                    nowCache = new CachePosition();
                    float h = getTemplateTransform().rect.height;
                    if (h < 5) h = 5;
                    float w = getTemplateTransform().rect.width;
                    if (w < 5) w = 5;
                    int cellItemIndex = cellItemCount > 1 ? nextEndIndex % cellItemCount : 0;
                    if (mMovement == Movement.Vertical)
                    {
                        if (cellItemIndex == 0)
                        {
                            nowCache.startY = lastPos;
                            nowCache.endY = nowCache.startY - h;
                            nowCache.startX = 0;
                            nowCache.endX = nowCache.startX + w;
                        }
                        else
                        {
                            nowCache.startY = lastCache.startY;
                            nowCache.endY = lastCache.endY;
                            nowCache.startX = lastCache.endX + cellItemSpace;
                            nowCache.endX = nowCache.startX + w;
                        }
                    }
                    else
                    {
                        if (cellItemIndex == 0)
                        {
                            nowCache.startX = lastPos;
                            nowCache.endX = nowCache.startX + w;
                            nowCache.startY = 0;
                            nowCache.endY = nowCache.startY - h;
                        }
                        else
                        {
                            nowCache.startX = lastCache.startX;
                            nowCache.endX = lastCache.endX;
                            nowCache.startY = lastCache.endY - cellItemSpace;
                            nowCache.endY = nowCache.startY - h;
                        }
                    }
                    nowCache.dataIndex = nextEndIndex;
                    setCacheInfo(nextEndIndex, nowCache);
                }
            }
        }

        //可视区域在Content中的坐标范围
        private Vector2 getContentViewRange()
        {
            if (mMovement == Movement.Vertical)
            {
                float scrollRangeMinY = -mContentTramform.anchoredPosition.y;
                float scrollRangeMaxY = -mContentTramform.anchoredPosition.y - mRectTransform.rect.height;// - 20;
                return new Vector2(scrollRangeMinY, scrollRangeMaxY);
            }
            else
            {
                float scrollRangeMinX = -mContentTramform.anchoredPosition.x;// - 100;
                float scrollRangeMaxX = -mContentTramform.anchoredPosition.x + mRectTransform.rect.width;// + 100;
                return new Vector2(scrollRangeMinX, scrollRangeMaxX);
            }
        }

        private void clearItem(bool active = true)
        {
            foreach (var item in mScrollItemList)
            {
                recycleOne(item, active);
            }
            mScrollItemList.Clear();
            mScrollToIndex = -1;
            mCurrentShowIndex = 0;
        }

        private int getNowStartIndex()
        {
            LinkedListNode<UIScrollItem> scrollItem = mScrollItemList.First;
            if (scrollItem == null) return 0;
            return scrollItem.Value.scrollDataIndex;
        }

        private int getNowEndIndex()
        {
            LinkedListNode<UIScrollItem> scrollItem = mScrollItemList.Last;
            if (scrollItem == null) return -1;
            return scrollItem.Value.scrollDataIndex;
        }

        private void recycleOne(UIScrollItem item, bool active = true)
        {
            item.OnRecycle();
            item.scrollDataIndex = -1;
            item.SetPointXY(10000, 10000);
            if (!active) item.gameObject.SetActive(false);
            mCacheItemQueue.Enqueue(item);
        }

        private CachePosition getCacheInfo(int index)
        {
            if (index >= 0 && index < mCacheList.Count)
                return mCacheList[index];
            return null;
        }

        private void setCacheInfo(int index, CachePosition cachePosition)
        {
            if (index >= 0 && index < mCacheList.Count)
                mCacheList[index] = cachePosition;
            else
            {
                mCacheList.Add(cachePosition);
                resetContentSize();
            }
            ////
        }

        private void OnElementClick(GameObject go, object data)
        {
            if (actionElementClick != null)
                actionElementClick(go, data);
        }


        [ContextMenu("PrintRect")]
        private void PrintRect()
        {
            tryInit();
            //mRectMask2D.PerformClipping();
            //LayoutComplete
            //mScrollRect.LayoutComplete();
            //mScrollRect.GraphicUpdateComplete();

            GameLogger.LogTest("rect :" + mRectTransform.rect.ToString() + mRectTransform.rect.center.ToString());
            GameLogger.LogTest("anchorMin :" + mRectTransform.anchorMin.ToString());
            GameLogger.LogTest("anchorMax :" + mRectTransform.anchorMax.ToString());
            GameLogger.LogTest("anchoredPosition :" + mRectTransform.anchoredPosition.ToString());
            GameLogger.LogTest("anchoredPosition3D :" + mRectTransform.anchoredPosition3D.ToString());
            GameLogger.LogTest("sizeDelta :" + mRectTransform.sizeDelta.ToString());
        }
        [ContextMenu("SetArchorAndPivot")]
        private void SetArchorAndPivot()
        {
            tryInit();

            if (mMovement == Movement.Vertical)
            {
                mContentTramform.anchorMin = new Vector2(0, 1);
                mContentTramform.anchorMax = new Vector2(1, 1);
                mContentTramform.pivot = new Vector2(0, 1);
                //
                RectTransform templateTransform = getTemplate().GetComponent<RectTransform>();
                templateTransform.anchorMin = new Vector2(templateTransform.anchorMin.x, 1);
                templateTransform.anchorMax = new Vector2(templateTransform.anchorMax.x, 1);
                templateTransform.pivot = new Vector2(templateTransform.pivot.x, 1);
            }
            else
            {
                mContentTramform.anchorMin = new Vector2(0, 0);
                mContentTramform.anchorMax = new Vector2(0, 1);
                mContentTramform.pivot = new Vector2(0, 1);
                //
                RectTransform templateTransform = getTemplate().GetComponent<RectTransform>();
                templateTransform.anchorMin = new Vector2(0, templateTransform.anchorMin.y);
                templateTransform.anchorMax = new Vector2(0, templateTransform.anchorMax.y);
                templateTransform.pivot = new Vector2(0, templateTransform.pivot.y);
            }
        }
        [ContextMenu("Reset")]
        private void Reset()
        {
            tryInit();

            mCacheList.Clear();
            clearItem();
            tryDryMoveForward();


            if (mMovement == Movement.Vertical)
                mScrollRect.verticalNormalizedPosition = 1;
            else
                mScrollRect.horizontalNormalizedPosition = 0;

            moveForward();
        }
        /////////
    }

}
