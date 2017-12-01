using System.Collections.Generic;
using Pandora;

[System.Serializable]
public class ItemCTable : BaseTable<ItemCVO>
{
    public int GetItemCvoByType(int itemType, int smallType)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].ItemType==itemType&&dataList[i].smallType==smallType)
            {
                return dataList[i].Id;
            }
        }
        return -1;
    }

    public List<ItemCVO> GetItemCvoByAttributionPage(int attributionPage)
    {
        List < ItemCVO >infos=new List<ItemCVO>();
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].ItemType == attributionPage )
            {
                infos.Add(dataList[i]);
            }
        }
        return infos;
    }
}

[System.Serializable]
public class ItemCVO : IRow
{
    public int Id; 
    public int ItemType;
    public int smallType;
    public int UseType;
    public string UseEffect1;
    public int ComposeNum;
    public int NextID;
    public string UseCostItem;
    public int AttributionPage;
    public int ItemColour;
    public string ItemIcon;    
    public string ItemName;
    public string ItemDes;

    public string functionDes;
  
    //道具Tips 出处功能使用字段
    public  int JumpUiType;
    public string JumpUi;
    public string JumpValue;
    public string Npc;
    public string JumpDes;  
   

    public List<int> _UseEffect1;
    public MapStruct.Map<int,int> _UseCostItem;
    public MapStruct.MapList<int> _JumpUi;
    public MapStruct.MapList<int> _jumpvalue;
    public List<int> _Npc;
    public MapStruct.MapList<string> _JumpDes;

    public override int getKey()
    {
        return Id;
    }

    public override void FliterData()
    {
       _UseEffect1 = ConvertUtil.ConvertToIntList(UseEffect1, ':');
       _UseCostItem = ConvertUtil.ConvertToMap<int,int>(UseCostItem, ':');
       _JumpUi = ConvertUtil.ConvertToMapList_Int(JumpUi, '|',':');
        _jumpvalue = ConvertUtil.ConvertToMapList_Int(JumpValue, '|', ':');

        _Npc = ConvertUtil.ConvertToIntList(Npc, '|');
       _JumpDes = ConvertUtil.ConvertToMapList_String(JumpDes, '|',':');     
        _Npc = ConvertUtil.ConvertToIntList(Npc, '|');      
    }

}
