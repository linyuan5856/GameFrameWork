using Pandora;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoaderManager))]
public class LoaderManagerEditor : UnityEditor.Editor
{
    private LoaderManager manager;

    private SerializedProperty _assetCacheDicKey;
    private SerializedProperty _assetCacheDicValue;

    private SerializedProperty _abCacheDicKey;
    //private SerializedProperty _abCacheDicValue;

    private GUIContent btnContent;
    private bool _assetDetail;



    void OnEnable()
    {
        manager = this.target as LoaderManager;

        if (manager == null)
        {
            UnityEngine.Debug.LogError("Manager is Null");
        }

        this._assetCacheDicKey = this.serializedObject.FindProperty("_assetkey");
        this._assetCacheDicValue = this.serializedObject.FindProperty("_assetvalue");

        this._abCacheDicKey = this.serializedObject.FindProperty("_abKey");
        //this._abCacheDicValue = this.serializedObject.FindProperty("_abValue");

        btnContent = new GUIContent("Detail");
        btnContent.tooltip = "详细描述";
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        this.serializedObject.Update();


        EditorGUILayout.LabelField("Asset缓存池", string.Format("当前总数 {0}", this._assetCacheDicKey.arraySize));
        this._assetDetail = EditorGUILayout.ToggleLeft(btnContent, _assetDetail);


        EditorGUILayout.BeginVertical();
        if (_assetDetail)
        {
            for (int i = 0; i < this._assetCacheDicKey.arraySize; i++)
            {
                SerializedProperty value = this._assetCacheDicValue.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(value, true);
            }
        }
        else
        {
            for (int i = 0; i < this._assetCacheDicKey.arraySize; i++)
            {
                SerializedProperty key = this._assetCacheDicKey.GetArrayElementAtIndex(i);
                EditorGUILayout.LabelField(string.Format("{0}. {1}", i+1, key.stringValue));
            }
        }

        EditorGUILayout.LabelField("AssetBundle缓存池", string.Format("当前总数 {0}", this._abCacheDicKey.arraySize));

        for (int i = 0; i < this._abCacheDicKey.arraySize; i++)
        {
            SerializedProperty key = this._abCacheDicKey.GetArrayElementAtIndex(i);
            EditorGUILayout.LabelField(string.Format("{0}. {1}", i+1, key.stringValue));
        }


        EditorGUILayout.EndVertical();

        this.serializedObject.ApplyModifiedProperties();
    }
}
