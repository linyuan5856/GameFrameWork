using Pandora;
using UnityEngine;
using UnityEngine.UI;

public class UI_Template : UIWindow
{
    private const int MININDEX = 0;
    private const int MAXINDEX = 9;
    private const string SLIDERDES = "Slider Can Change Image";

    //demo用的资源
    [SerializeField] private AssetForTemplateDemo m_asset;
    [SerializeField] private Button btn_close;
    [SerializeField] private Button btn_send;
    [SerializeField] private Text text_name;
    [SerializeField] private Image image_bg;
    [SerializeField] private Slider slider_changeBg;
    [SerializeField] private ScrollRect scroll_dialog;
    [SerializeField] private InputField input_dialog;
    [SerializeField] private UIToggle toggle_left;
    [SerializeField] private UIToggle toggle_right;
    
    private bool m_IsClosed;
    [SerializeField]
    private Vector3 m_PosR;
    [SerializeField]
    private Vector3 m_PosL;

    protected override void OnInitWindow()
    {
        base.OnInitWindow();     
        this.text_name.text = SLIDERDES;     

        this.slider_changeBg.minValue = MININDEX;
        this.slider_changeBg.maxValue = MAXINDEX;
        this.slider_changeBg.wholeNumbers = true;

        UGUI_EventListener.Get(this.btn_close.gameObject).onClick = this.OnClick;
        UGUI_EventListener.Get(this.btn_send.gameObject).onClick = this.OnClick;
        this.toggle_left.onValueChanged.AddListener(isToggle => this.OnToggleValueChanged(this.toggle_left, isToggle));
        this.toggle_right.onValueChanged.AddListener(isToggle => this.OnToggleValueChanged(this.toggle_right, isToggle));
        this.slider_changeBg.onValueChanged.AddListener(this.OnSliderValueChanged);
    }

    protected override void OnOpenWindow()
    {
        base.OnOpenWindow();

        this.UpdateBgImage();


        this.m_IsClosed = false;
    }

    protected override void OnCloseWindow()
    {
        base.OnCloseWindow();
        this.m_IsClosed = true;
    }


    private void UpdateBgImage()
    {
        int index = (int)this.slider_changeBg.value;
        this.image_bg.overrideSprite = m_asset.m_sprites[index];
    }

    private void UpdateInput()
    {
        string content = this.input_dialog.text;

        if (!string.IsNullOrEmpty(content))
        {

            if (this.scroll_dialog.content == null)
            {
                GameLogger.LogError("Scroll Rect Content is Null");
                return;
            }
            var asset = Resources.Load(GameDefine.Widget_UITemplate_Dialog);
            var prefab = (GameObject)GameObject.Instantiate(asset, this.scroll_dialog.content);
            prefab.GetComponent<Text>().text = (Random.value > 0.5 ? "A Say: " : "B Say: ") + content;
            this.input_dialog.text = string.Empty;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        this.UpdateBgImage();
    }

    private void OnClick(GameObject go)
    {
        if (go == this.btn_close.gameObject)
        {
            UIManager.Instance.CloseWindow<UI_Template>();
        }
        else if (go == this.btn_send.gameObject)
        {
            this.UpdateInput();
        }
    }

    private void OnToggleValueChanged(UIToggle toggle, bool onSelect)
    {
        if (onSelect)
        {
            if (toggle == this.toggle_right)
            {
                this.scroll_dialog.transform.localPosition = this.m_PosL;
                this.image_bg.transform.localPosition = this.m_PosR;

            }
            else if (toggle == this.toggle_left)
            {
                this.scroll_dialog.transform.localPosition = this.m_PosR;
                this.image_bg.transform.localPosition = this.m_PosL;
            }
        }
    }

    void OnGUI()
    {
        if (m_IsClosed)
        {
            if (GUI.Button(
                new Rect(Screen.width * 0.1f, Screen.height * 0.2f, Screen.width * 0.2f, Screen.height * 0.1f),
                "再次打开界面"))
            {
                UIManager.Instance.OpenWindow<UI_Template>();
            }         
        }
    }
}