using System;

namespace Pandora
{
    public class UITreeNodeDataProvider : UIDataProvider
    {
        public UITreeNodeDataProvider()
        {
            mNodeData = new NodeData(string.Empty);
        }
        public UITreeNodeDataProvider(string _txt, object _param = null, bool _selected = false, bool _expand = false)
        {
            mNodeData = new NodeData(_txt, _param, _selected, _expand);
        }

        public UITreeNodeDataProvider(NodeData nodeData)
        {
            mNodeData = nodeData;
        }


        protected NodeData mNodeData;

        public NodeData GetNodeData()
        {
            return mNodeData;
        }



        protected UITreeNodeDataProvider mParent;

        public void SetParent(UITreeNodeDataProvider parent)
        {
            this.mParent = parent;
        }

        public UITreeNodeDataProvider GetParent()
        {
            return mParent;
        }


        public bool FindNode(Func<UITreeNodeDataProvider, bool> action)
        {
            if (this.Size == 0)
            {
                if (action(this))
                {
                    return true;
                }
            }
            else
            {
                for (int i = 0; i < this.Size; i++)
                {
                    UITreeNodeDataProvider childDataProvider = (UITreeNodeDataProvider)mList[i];
                    if (childDataProvider.FindNode(action))
                    {
                        return true;
                    }
                }
                //
            }
            return false;
        }


        public override void Add(object item)
        {
            UITreeNodeDataProvider nodeData = (UITreeNodeDataProvider)item;
            nodeData.SetParent(this);
            base.Add(item);
        }

        public override void AddAtIndex(int index, object item)
        {
            UITreeNodeDataProvider nodeData = (UITreeNodeDataProvider)item;
            nodeData.SetParent(this);
            base.AddAtIndex(index, item);
        }

        public override void SetItemAt(int i, object item)
        {
            UITreeNodeDataProvider nodeData = (UITreeNodeDataProvider)item;
            nodeData.SetParent(this);
            base.SetItemAt(i, item);
        }

        public void SelectItem(UITreeNodeDataProvider data)
        {
            //  BetterList<int> indexList = new BetterList<int>();

            data.GetNodeData()._select = true;
            data.GetNodeData()._expand = true;

            data = data.GetParent();
            while (data != null)
            {
                data.GetNodeData()._expand = true;
                data = data.GetParent();
            }
        }


        public void SelectFirst()
        {
            SelectItem(0, 0, 0, 0);
        }
        public void SelectItem(params int[] itemIndex)
        {
            UITreeNodeDataProvider currentMenu = this;

            NodeData lastNode = null;
            for (int i = 0; i < itemIndex.Length; i++)
            {
                int index = itemIndex[i];
                currentMenu = (UITreeNodeDataProvider)currentMenu.GetItemAt(index);
                if (currentMenu != null)
                {
                    lastNode = currentMenu.GetNodeData();
                    lastNode._expand = true;
                }
                else
                {
                    break;
                }
            }
            if (lastNode != null)
                lastNode._select = true;
        }
    }


    public class NodeData
    {
        public NodeData(string _txt, object _param = null, bool _selected = false, bool _expand = false, bool _lock = false)
        {
            text = _txt;
            param = _param;
            this._select = _selected;
            this._expand = _expand;
            this._lock = _lock;
        }
        public string text;
        public object param;
        public bool _select;
        public bool _expand;

        public bool _lock;
    }

}
