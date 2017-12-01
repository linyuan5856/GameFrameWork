using System.Collections.Generic;
using Pandora;

public class PreLoadSourceInfo
{
    public string AssetName;
    public AssetType AssetType;

    public PreLoadSourceInfo(string name, AssetType assetType)
    {
        this.AssetName = name;
        this.AssetType = assetType;
    }
}

public static class PreLoadAssetCatalog
{

    public static List<string> TableList = new List<string>()
    {
        "ItemCTable"
    };

    public static List<PreLoadSourceInfo> AssetList = new List<PreLoadSourceInfo>()
    {
        //new PreLoadSourceInfo("HeroScene_Desert", AssetType.MountScene)
    };

    public static List<PreLoadSourceInfo> AssetBundleList = new List<PreLoadSourceInfo>()
    {
        new PreLoadSourceInfo("scene0", AssetType.AssetBundle),
        new PreLoadSourceInfo("scene1", AssetType.AssetBundle),
        new PreLoadSourceInfo("scene2", AssetType.AssetBundle),
        new PreLoadSourceInfo("human", AssetType.AssetBundle),
        new PreLoadSourceInfo("music", AssetType.AssetBundle),
        new PreLoadSourceInfo("effect", AssetType.AssetBundle),
        new PreLoadSourceInfo("orc", AssetType.AssetBundle)
    };
}
