
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pandora
{

    public class UITreeNode : UIComponentGroup
    {
        protected UITreeNodeDataProvider MDataDataProvider;

        protected BetterList<UITreeNode> mNodeList = new BetterList<UITreeNode>();

        [HideInInspector] public UITreeNode parent;
        [HideInInspector] public UITree root;

        public int nodeLevel = 0;
        public float height;
        private float titleHeight;

        private RectTransform mBgRectTrans;
        protected GameObject selectObj;
        protected GameObject unSelectObj;
        protected Transform lockTrans;
        protected Text selectText;
        protected Text unSelectText;

        protected RectTransform mRectTransform;

        //
        private NodeData mNodeData;

        protected virtual void init()
        {
            mRectTransform = GetComponent<RectTransform>();

            selectObj = transform.Find("Checkmark").gameObject;
            mBgRectTrans = selectObj.GetComponent<RectTransform>();
            selectText = transform.Find("Checkmark/text__word").GetComponent<Text>();
            unSelectObj = transform.Find("Background").gameObject;
            unSelectText = transform.Find("Background/text__word").GetComponent<Text>();
            lockTrans = transform.Find("Unlocked");

            UGUI_EventListener.Get(selectObj).onClick = OnClick;
            UGUI_EventListener.Get(unSelectObj).onClick = OnClick;
            isExpand = false;
        }

        override public void SetData(UIDataProvider _data)
        {
            if (mRectTransform == null) init();

            MDataDataProvider = (UITreeNodeDataProvider)_data;

            mNodeData = MDataDataProvider.GetNodeData();

            if (mNodeData != null)
            {
                if (mNodeData._select && hasNoChild)
                {
                    mNodeData._select = false;
                    root.selectNode = this;
                }

                if (mNodeData._expand)
                {
                    mNodeData._expand = false;
                    this.isExpand = true;
                }
            }

            updateDisplay();
        }

        public NodeData GetNodeData()
        {
            return mNodeData;
        }
        public UITreeNodeDataProvider GetData()
        {
            return MDataDataProvider;
        }

        public void Refresh()
        {
            updateDisplay();
        }

        public Vector2 Size
        {
            get { return mRectTransform.sizeDelta; }
        }

        public float TitleHeight
        {
            get { return titleHeight; }
        }

        public float GetPos()
        {
            return mRectTransform.anchoredPosition.y;
        }

        protected virtual void updateDisplay()
        {
            height = 0;

            updateTitle();

            if (isExpand)
            {
                for (int i = 0, len = MDataDataProvider.Size; i < len; i++)
                {
                    UITreeNodeDataProvider data = (UITreeNodeDataProvider)MDataDataProvider.GetItemAt(i);
                    UITreeNode treeNode = getChildNode(i);
                    //
                    treeNode.SetData(data);

                    Vector3 localPos = treeNode.transform.localPosition;
                    localPos.y = -height;
                    treeNode.transform.localPosition = localPos;
                    height += treeNode.Size.y + root.cellSpace;
                }
            }
            else
            {
                for (int i = 0, len = mNodeList.size; i < len; i++)
                {
                    UITreeNode node = mNodeList[i];
                    node.transform.localPosition = Vector3.zero;
                    node.gameObject.SetActive(false);
                }
            }

            resetContentSize();
        }

        protected virtual void updateTitle()
        {
            if (!isRoot)
            {
                //title size;
                titleHeight = mBgRectTrans.sizeDelta.y;
                height += titleHeight + root.cellSpace;

                selectText.text = mNodeData.text;
                unSelectText.text = mNodeData.text;

                if (isSelected)
                {
                    selectObj.SetActive(true);
                    unSelectObj.SetActive(false);
                }
                else
                {
                    selectObj.SetActive(false);
                    unSelectObj.SetActive(true);
                }
                if (lockTrans != null)
                    lockTrans.gameObject.SetActive(mNodeData != null && mNodeData._lock);
            }
        }

        private void resetContentSize()
        {
            Vector2 size = mRectTransform.sizeDelta;
            size.y = height;
            mRectTransform.sizeDelta = size;
        }

        protected UITreeNode getChildNode(int index)
        {
            UITreeNode treeNode;
            if (mNodeList.size > index)
            {
                treeNode = mNodeList[index];
            }
            else
            {
                GameObject ui = root.createItem(nodeLevel + 1);
                ui.SetParent(this.gameObject, false);
                treeNode = ui.GetOrAddComponent<UITreeNode>();
                treeNode.init();
                mNodeList.SetSize(index + 1);
                mNodeList[index] = treeNode;
            }
            treeNode.gameObject.SetActive(true);
            treeNode.parent = this;
            treeNode.root = root;
            treeNode.nodeLevel = nodeLevel + 1;
            return treeNode;
        }


        public virtual bool isExpand
        {
            get; set;
        }

        public virtual bool isSelected
        {
            get { return root.selectNode == this; }
        }

        private void OnClick(GameObject go)
        {
            if (mNodeData != null && mNodeData._lock) return;

            if (root.OnClickPreHandle != null)
            {
                if (!root.OnClickPreHandle(this, mNodeData.param))
                {
                    return;
                }
            }
          

            if (hasNoChild) root.selectNode = this;

            isExpand = !isExpand;
            root.TryMoveUpdate(this);

            if (hasNoChild)
            {
                if (root.OnSelectChange != null)
                    root.OnSelectChange(this, mNodeData.param);
            }
        }

        //protected HMTreeNode getInnerFirstNode()
        //{
        //    if (mNodeList.size == 0)
        //        return this;

        //    HMTreeNode node = mNodeList[0];
        //    return node.getInnerFirstNode();
        //}

        public bool hasNoChild
        {
            get { return MDataDataProvider.Size == 0; }
        }

        protected virtual GameObject createItem(int nodeLevel)
        {
            return null;
        }

        public bool isRoot
        {
            get { return this is UITree; }
        }

    }

}
