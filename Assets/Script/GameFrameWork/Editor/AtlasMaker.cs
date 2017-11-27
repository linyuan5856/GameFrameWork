using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

class AtlasMaker
{
   
    [ MenuItem("program/AtlasMaker")]
    static private void MakeAtlas()
    {
	
        string dirOutput = Application.dataPath + "/Resources/Sprites";
        if (!Directory.Exists(dirOutput))
        {
            Directory.CreateDirectory(dirOutput);
        }
       
        string dirInput = EditorUtility.OpenFolderPanel("Select Floder", Application.dataPath + "/Sources/Atlas", "");
      
        if (string.IsNullOrEmpty(dirInput))//直接关闭选择文件窗口
        {
            return;
        }

        DirectoryInfo rootDirInfo = new DirectoryInfo(dirInput);
        if (!Directory.Exists(dirInput))
        {
            Directory.CreateDirectory(dirInput);
        }
		
            foreach (FileInfo pngFile in rootDirInfo. GetFiles ( "*.png" , SearchOption . AllDirectories ))
            {
                string allPath = pngFile.FullName;
                Debug.LogWarning(allPath);

                string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
			
                Sprite sprite = AssetDatabase.LoadAssetAtPath < Sprite >(assetPath);

                if (sprite==null)
                {
                    Debug.LogError(assetPath);
                }
                GameObject go = new GameObject(sprite.name);
			
                go.AddComponent < SpriteRenderer >().sprite = sprite;
			
                allPath = dirOutput + "/" + sprite.name + ".prefab";
			
                string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
			
                PrefabUtility.CreatePrefab(prefabPath, go);
			
                GameObject.DestroyImmediate(go);
			
            }
    }
}
