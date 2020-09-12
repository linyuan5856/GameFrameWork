using System;
using UnityEngine;
using UnityEngine.U2D;

public partial class LoaderService
{
    #region 图集 精灵

    /// <summary>
    /// Unity官方图集系统加载的回调
    /// https://blog.csdn.net/qq_15559109/article/details/90315106
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="action"></param>
    private void OnAtlasRequested(string tag, Action<SpriteAtlas> action)
    {
        Debug.Log($"Atlas系统请求加载图集 Tag->{tag}");
        LoadAtlasAsync(tag, atlas => { action?.Invoke(atlas); });
    }

    private SpriteAtlas LoadAtlas(string tag)
    {
        var atlas = LoadAsset(AtlasPathPrefix + tag) as UnityEngine.U2D.SpriteAtlas;
        return atlas;
    }

    private void LoadAtlasAsync(string tag, Action<SpriteAtlas> callBack)
    {
        LoadAssetAsync(AtlasPathPrefix + tag,
            asset => { callBack?.Invoke(asset as SpriteAtlas); });
    }

    public Sprite LoadSprite(string path)
    {
        var sprite = _assetLoader.LoadAsset<Sprite>($"{SpritePathPrefix}{path}");
        return sprite;
    }

    public void LoadSpriteAsync(string path, Action<Sprite> callBack)
    {
        LoadAssetAsync<Sprite>($"{SpritePathPrefix}{path}",
            asset => { callBack?.Invoke(asset as Sprite); });
    }

    // public Sprite LoadSprite(int id)
    // {
    //     var data = ConfigUtil.GetRow<IconData>(id);
    //     if (data == null)
    //     {
    //         GameUtil.LogError($"icon id ->{id}  has no config");
    //         return null;
    //     }
    //     var atlas = LoadAtlas(data.Atlas);
    //     var sprite = atlas.GetSprite(data.Name);
    //     Debug.Assert(sprite != null, $"Load Sprite Atlas->{data.Atlas}  Sprite->{data.Name} 不存在");
    //     return sprite;
    // }
    //
    // public void LoadSpriteAsync(int id, Action<Sprite> callBack)
    // {
    //     var data = ConfigUtil.GetRow<IconData>(id);
    //     LoadAtlasAsync(data.Atlas, atlas =>
    //     {
    //         var sprite = atlas.GetSprite(data.Name);
    //         Debug.Assert(sprite != null, $"Load Sprite Atlas->{data.Atlas}  Sprite->{data.Name} 不存在");
    //         callBack?.Invoke(sprite);
    //     });
    // }

    #endregion

    public AudioClip LoadAudioClip(string path)
    {
        return LoadAsset<AudioClip>(path);
    }

    public Material LoadMaterial(string path)
    {
        return LoadAsset<Material>("Materials/"+path);
    }
}