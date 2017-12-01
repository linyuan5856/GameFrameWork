using UnityEngine;
using UnityEngine.UI;

namespace Pandora
{
    public class UICanvasController : MonoBehaviour
    {
        private Canvas _canvas;
        private GraphicRaycaster _graphicRaycaster;     
        private bool _inIt;

        void Init()
        {
            if (_inIt) return;
            _inIt = true;

            this._canvas = this.GetComponent<Canvas>();
            this._graphicRaycaster = this.GetComponent<GraphicRaycaster>();
        }   


        public void SetEnable(bool enable)
        {
            this.Init();
            this._canvas.enabled = enable;
            this._graphicRaycaster.enabled = enable;
        }
    }
}

