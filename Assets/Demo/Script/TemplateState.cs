using Pandora;

public class TemplateState : BaseState{

    protected override void OnInitStage()
    {
        base.OnInitStage();

       LoaderManager.Instance.LoadSceneAsync(GameDefine.TemplateScene,null,this.OnSceneLoad);     
    }

    private void OnSceneLoad()
    {
        UIManager.Instance.OpenWindow<UI_Template>();
    }

}
