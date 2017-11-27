using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GFW
{
    public class RTManager : MonoBehaviour
    {

        static public RTManager Instance;

        static public void AddRenderTexture(GameObject rawImageGO, GameObject threeDModel, RenderTextureType _type)
        {
            if (Instance == null)
            {
                GameObject mRTPoolRoot = new GameObject("RTPoolRoot");
                mRTPoolRoot.transform.position = new Vector3(30000, 0, 0);
                Instance = mRTPoolRoot.AddComponent<RTManager>();
            }

            Instance.addRenderTexture(rawImageGO, threeDModel, _type);
        }

        static public void ClearRenderTexture(GameObject rawImageGO)
        {
            if (Instance != null)
                Instance.clearRenderTexture(rawImageGO);
        }


        [SerializeField] private List<RenderTextureNode> rtList = new List<RenderTextureNode>();

        private void addRenderTexture(GameObject rawImageGO, GameObject threeDModel, RenderTextureType _type)
        {
            RenderTextureNode rtNode = rawImageGO.GetOrAddComponent<RenderTextureNode>();

            if (rtList.IndexOf(rtNode) == -1)
            {
                rtNode.container = new GameObject("rtcontainer");
                rtNode.container.SetParent(this.gameObject, false);
                Camera cam = rtNode.container.AddComponent<Camera>();
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.depth = -100;
                cam.backgroundColor = new Color(1, 1, 1, 0);
                cam.fieldOfView = 30;
                cam.orthographic = false;
                cam.orthographicSize = 0.4f;

                rtNode.rtCam = cam;
                rtNode.heroModel = new GameObject("HeroModel");
                rtNode.heroModel.SetParent(rtNode.container);
                rtNode.rawImage = rawImageGO.GetComponent<RawImage>();
                rtNode.type = _type;


                GameObject heroModelContainer = rawImageGO.transform.Find("HeroModel").gameObject;
                rtNode.heroModel.transform.localPosition = heroModelContainer.transform.localPosition;
                rtNode.heroModel.transform.localScale = heroModelContainer.transform.localScale;
                rtNode.heroModel.transform.localRotation = heroModelContainer.transform.localRotation;

                rtNode.rtCam.targetTexture = GetTexture(rtNode.type);
                rtList.Add(rtNode);
            }

            if (rtNode.type != _type)
            {
                RenderTexture tempTex = rtNode.rtCam.targetTexture;
                if (tempTex != null)
                    RenderTexture.ReleaseTemporary(tempTex);
                rtNode.rtCam.targetTexture = GetTexture(rtNode.type);
            }

            if (!rtNode.container.activeSelf)
                rtNode.container.SetActive(true);
            if (rtNode.model
                && rtNode.model != threeDModel)
            {
                GameObject.Destroy(rtNode.model);
            }

            rtNode.heroModel.AddChild(threeDModel, false);
            rtNode.model = threeDModel;

            rtNode.rawImage.texture = rtNode.rtCam.targetTexture;

        }

        private void clearRenderTexture(GameObject rawImageGO)
        {
            RenderTextureNode rtNode = rawImageGO.GetComponent<RenderTextureNode>();
            if (rtNode != null
                && rtList.Remove(rtNode))
            {
                RenderTexture tempTex = rtNode.rtCam.targetTexture;
                if (tempTex != null)
                    RenderTexture.ReleaseTemporary(tempTex);

                GameObject.Destroy(rtNode.container);
            }
        }

        static public RenderTexture GetTexture(RenderTextureType _type)
        {
            RenderTexture renderTexture = null;

            int width = 512;
            int height = 512;

            switch (_type)
            {
                case RenderTextureType.None:
                    break;
                case RenderTextureType.GuideSystem:
                    break;
                case RenderTextureType.GuideReward:
                    width = 1000;
                    height = 400;
                    break;
                case RenderTextureType.Team:
                    width = 960;
                    height = 540;
                    break;
                case RenderTextureType.Story:
                    break;
                case RenderTextureType.StoryPupInfo:
                    break;
                case RenderTextureType.BattleField:
                    break;
                case RenderTextureType.PlotTalking:
                    width = 960;
                    height = 540;
                    break;
            }
            renderTexture = RenderTexture.GetTemporary(width * 2, height * 2, 24, RenderTextureFormat.Default);
            Logger.LogWarn("!!!!!!GetRenderTextrue  " + _type.ToString());
            return renderTexture;
        }
    }


    public enum RenderTextureType
    {
        None,
        GuideSystem,
        GuideReward,
        Team,
        Story,
        StoryPupInfo,
        BattleField,
        PlotTalking
    }

    public class RenderTextureNode : MonoBehaviour
    {
        void OnDestroy()
        {
            RTManager.ClearRenderTexture(gameObject);
        }

        public GameObject container;
        public GameObject heroModel;
        public GameObject model;
        public RawImage rawImage;
        public Camera rtCam;
        public RenderTextureType type;
    }

}
