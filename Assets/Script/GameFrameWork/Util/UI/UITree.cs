using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pandora
{
    public class UITree : UITreeNode
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject[] mTemplateList;
        public int cellSpace = 0;

        private int mLastTemplateIndex;
        private UITreeNode mSelectTreeNode;


        public Func<UITreeNode, object, bool> OnClickPreHandle;
        public Action<UITreeNode, object> OnSelectChange;

        protected override void init()
        {
            mLastTemplateIndex = mTemplateList.Length - 1;
            root = this;
            nodeLevel = -1;
            mRectTransform = GetComponent<RectTransform>();

            for (int i = 0, len = mTemplateList.Length; i < len; i++)
            {
                mTemplateList[i].SetActive(false);
            }
        }

        public override void SetData(UIDataProvider _data)
        {
            selectNode = null;
            base.SetData(_data);
        }


        public void TryMoveUpdate(UITreeNode node)
        {
            updateDisplay();
            if (scrollRect != null)
            {
                RectTransform scrollTransform = (RectTransform)scrollRect.transform;
                float scrollRangeMaxY = -mRectTransform.anchoredPosition.y - scrollTransform.rect.height;

                float nodeY = GetRelativePos(node);
                float nodeMaxY = nodeY - node.Size.y;
                if (nodeMaxY < scrollRangeMaxY)
                {
                    Vector2 pos = mRectTransform.anchoredPosition;
                    pos.y = -(nodeMaxY + scrollTransform.rect.height);
                    mRectTransform.anchoredPosition = pos;
                }
            }
        }

        override protected GameObject createItem(int nodeLevel)
        {
            GameObject template = nodeLevel < mTemplateList.Length
                ? mTemplateList[nodeLevel]
                : mTemplateList[mLastTemplateIndex];

            GameObject item = GameObject.Instantiate(template);
            item.SetActive(true);
            return item;
        }

        public UITreeNode selectNode
        {
            get { return mSelectTreeNode; }
            set
            {
                mSelectTreeNode = value;
            }
        }

        public override bool isExpand { get { return true; } }

        public float GetRelativePos(UITreeNode node)
        {
            float pos = 0;
            while (node != null && !node.isRoot)
            {
                pos += node.GetPos();
                node = node.parent;
            }
            return pos;
        }

        public void MoveToItem(float pos)
        {
            Vector3 localPos = mRectTransform.anchoredPosition;
            localPos.y = pos;
            mRectTransform.anchoredPosition = localPos;
        }

    }
}
