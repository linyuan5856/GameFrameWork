using UnityEngine;
using UnityEditor;

public class SceneInfo : EditorWindow {
	private Bounds bounds;
	private bool initialized = false;
	
    [MenuItem("program/Scene Info")]
	static void Init() {
		EditorWindow.GetWindow(typeof(SceneInfo));
	}
	
	void OnGUI() {
        GUILayout.Label(new GUIContent("Calculate Render Bound"));
		if (GUILayout.Button("Calculate")) {
			GetSceneInfo();
		}
		
		if (initialized) {
			Vector3 size = (bounds.max - bounds.min);
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Origin:\t");
			EditorGUILayout.SelectableLabel(bounds.min.ToString());
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Size:\t");
			EditorGUILayout.SelectableLabel(size.ToString());
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("xOrigin:\t");
			EditorGUILayout.SelectableLabel(bounds.min.x.ToString());
			GUILayout.Label("yOrigin:\t");
			EditorGUILayout.SelectableLabel(bounds.min.z.ToString());
			GUILayout.EndHorizontal();
		
			GUILayout.BeginHorizontal();
			GUILayout.Label("xCenter:\t");
			EditorGUILayout.SelectableLabel(bounds.center.x.ToString());
			GUILayout.Label("yCenter:\t");
			EditorGUILayout.SelectableLabel(bounds.center.z.ToString());
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("width:\t");
			EditorGUILayout.SelectableLabel(size.x.ToString());
			GUILayout.Label("height:\t");
			EditorGUILayout.SelectableLabel(size.z.ToString());
			GUILayout.EndHorizontal();
		}
	}
	
	void GetSceneInfo() {
		Renderer[] renderers = Object.FindObjectsOfType(typeof(Renderer)) as Renderer[];
		bounds = new Bounds();
		
		foreach (Renderer renderer in renderers) {
			bounds.Encapsulate(renderer.bounds);
		}
		
		initialized = true;
	}
	
}
