using System.Reflection;

namespace Pandora
{
    public class SVO : EventDispatcher<object>
    {
        protected bool _locked;
        protected bool _changed;


        public void setProperty(string fieldName, object value)
        {
            FieldInfo fieldInfo = this.GetType().GetField(fieldName);
            if (fieldInfo == null)
            {
                GameLogger.LogWarn("can not find field on " + this + " " + this.GetType().Name);
                return;
            }
            try
            {
                fieldInfo.SetValue(this, value);
            }
            catch (System.Exception ex)
            {
                GameLogger.LogError("set value failed: " + this.GetType().Name + " " + fieldInfo.Name + ex.Message);
            }
            _changed = true;
            if (_locked) return;
            DispatchEvent(DataEvent.Event_ProChange, null);
        }

        public void unlock()
        {
            _locked = false;
            if (_changed)
            {
                _changed = false;
                DispatchEvent(DataEvent.Event_ProChange, null);
            }
        }

        public void lockTable()
        {
            _locked = true;
        }
    }

    public class DataEvent
    {
        public const int Event_ProChange = 1;
        public const int Event_AddChild = 2;
        public const int Event_RemoveChild = 3;
    }
}