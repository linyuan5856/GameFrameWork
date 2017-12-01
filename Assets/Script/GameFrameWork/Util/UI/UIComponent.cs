using UnityEngine;

namespace GFW
{
    public abstract class UIComponentGroup : UIBaseComponent
    {
        public abstract void SetData(UIDataProvider _data);

        public override void SetData(object _data)
        {
            UIDataProvider dataProvider = (UIDataProvider)_data;

            if (dataProvider != null)
            {
                SetData(dataProvider);
            }
            else
            {
                GameLogger.LogError("IHMComponentGroup Must SetData as HMDataProvider");
            }
        }
    }

    public abstract class UIBaseComponent : MonoBehaviour
    {
        public abstract void SetData(object _data);
    }

}
