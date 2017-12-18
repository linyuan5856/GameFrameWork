using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class DebugThreeLight : MonoBehaviour
{

    [SerializeField]
    Material mMaterial;

    private Vector3 mFirstLightDir;
    private Color mFirstLightColor;

    void Start()
    {
        this.mMaterial = this.GetComponent<Material>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        this.DrawLightLine("_FirstLight", "_FirstLightColour");
        this.DrawLightLine("_SecLight", "_SecLightColour");
        this.DrawLightLine("_ThirdLight", "_ThirdLightColour");

    }
#endif

    void DrawLightLine(string lightDir, string lightC)
    {
        if (!HasProperty(lightDir) || !HasProperty(lightC))
            return;

        mFirstLightDir = mMaterial.GetVector(lightDir);
        mFirstLightColor = mMaterial.GetColor(lightC);


        Gizmos.color = mFirstLightColor;
        Gizmos.DrawRay(this.transform.position, mFirstLightDir);
        Handles.color = Color.black;
        Handles.Label(this.transform.position + mFirstLightDir, lightDir);
    }

    bool HasProperty(string name)
    {
        return mMaterial != null && mMaterial.HasProperty(name);
    }

}
