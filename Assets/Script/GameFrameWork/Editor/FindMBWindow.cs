using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FindMBWindow:EditorWindow
{


    [MenuItem("program/FindScript")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow<FindMBWindow>().Show();
    }


    private string mbName = "";
    private string mSearchPath = "Assets/Resources/UI";

    private List<string> mResultList = new List<string>();
    private List<string> mLostMBList = new List<string>();
    private bool mShowMissing = true;

    private Vector2 scrollPos;

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        mbName = EditorGUILayout.TextField("MBName:", mbName);
        Rect rect = GUILayoutUtility.GetLastRect();
        if ((Event.current.type == EventType.DragUpdated)
          && rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }
        if (Event.current.type == EventType.DragPerform
            && rect.Contains(Event.current.mousePosition))
        {
            if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
                mbName = DragAndDrop.objectReferences[0].name;
        }

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();

        mSearchPath = EditorGUILayout.TextField("SearchPath", mSearchPath);
        rect = GUILayoutUtility.GetLastRect();

        //mSearchPath = EditorGUILayout.TextField("SearchPath", mSearchPath);
        if ((Event.current.type == EventType.DragUpdated)
          && rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }
        if (Event.current.type == EventType.DragPerform
            && rect.Contains(Event.current.mousePosition))
        {
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                mSearchPath = DragAndDrop.paths[0];
        }
        if (GUILayout.Button("浏览"))
        {
            string path = EditorUtility.OpenFolderPanel("搜索路径", Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                mSearchPath = path.Substring(path.IndexOf("Assets"));
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        mShowMissing = GUILayout.Toggle(mShowMissing, "Missing MonoBehavior");

        if (GUILayout.Button("开始查找"))
        {
            FindScript();
        }

        EditorGUILayout.EndHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < mResultList.Count; i++)
        {
            EditorGUILayout.LabelField(mResultList[i]);
        }

        if (mShowMissing)
        {
            for (int i = 0; i < mLostMBList.Count; i++)
            {
                EditorGUILayout.LabelField(mLostMBList[i]);
            }
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    private void FindScript()
    {
        
        if (string.IsNullOrEmpty(mbName)
            || string.IsNullOrEmpty(mSearchPath))
        {
            EditorUtility.DisplayDialog("出错", "参数错误", "确定");
            return;
        }

        mResultList.Clear();
        mLostMBList.Clear();
        mResultList.Add(string.Format("Search [{0}] in [{1}] prefabs", mbName, mSearchPath));

        string[] paths = Directory.GetFiles(mSearchPath, "*.prefab", SearchOption.AllDirectories);
        foreach (var o in paths)
        {
            GameObject go = (GameObject) AssetDatabase.LoadAssetAtPath(o, typeof(GameObject));
            List<MonoBehaviour> mbList = new List<MonoBehaviour>();
            findScriptInGO(go, mbList);
            for (int i = 0; i < mbList.Count; i++)
            {
                if (mbList[i] == null)
                {
                    mLostMBList.Add(string.Format("missing mb in [{0}]", go.name));
                    continue;
                }
                if (mbList[i].GetType().Name == mbName)
                {
                    mResultList.Add(string.Format("find mb [{1}] in [{0}]  Path : {2}", go.name, mbName,mbList[i].GetPath()));
                }
            }
        }
        mResultList.Add("end.");
        //
    }

    private void findScriptInGO(GameObject go,List<MonoBehaviour> mbList)
    {
        MonoBehaviour[] mbs = go.GetComponents<MonoBehaviour>();
        mbList.AddRange(mbs);

        for (int i = 0; i < go.transform.childCount; i++)
        {
            Transform t = go.transform.GetChild(i);
            findScriptInGO(t.gameObject, mbList);
        }
    }
}