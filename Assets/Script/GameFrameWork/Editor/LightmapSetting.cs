using UnityEditor;

public class LightmapSize_256 : EditorWindow
{
    [MenuItem("program/LightmapSize/LightmapSize_256")]
	static void Init()
	{
		LightmapEditorSettings.maxAtlasHeight = 256;
		LightmapEditorSettings.maxAtlasWidth = 256;
	}
}

public class LightmapSize_512 : EditorWindow
{
    [MenuItem("program/LightmapSize/LightmapSize_512")]
	static void Init()
	{
		LightmapEditorSettings.maxAtlasHeight = 512;
		LightmapEditorSettings.maxAtlasWidth = 512;
	}
}

public class LightmapSize_1024 : EditorWindow
{
    [MenuItem("program/LightmapSize/LightmapSize_1024")]
	static void Init()
	{
		LightmapEditorSettings.maxAtlasHeight = 1024;
		LightmapEditorSettings.maxAtlasWidth = 1024;
	}
}

public class LightmapSize_2048 : EditorWindow
{
    [MenuItem("program/LightmapSize/LightmapSize_2048")]
	static void Init()
	{
		LightmapEditorSettings.maxAtlasHeight = 2048;
		LightmapEditorSettings.maxAtlasWidth = 2048;
	}
}

public class LightmapSize_4096 : EditorWindow
{
    [MenuItem("program/LightmapSize/LightmapSize_4096")]
	static void Init()
	{
		LightmapEditorSettings.maxAtlasHeight = 4096;
		LightmapEditorSettings.maxAtlasWidth = 4096;
	}
}