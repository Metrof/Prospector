using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс SlotDef не наследует MonoBehaviour, поэтому для него не требуется создавать отдельный файл на С#
[System.Serializable]//Сделает экзембляры SlotDef видимыми в инспекторе Unity
public class SlotDef
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stagger;
}
public class Layout : MonoBehaviour
{
    public PT_XMLReader xmlr;//Так же, как Deck, имеет PT_XMLReader
    public PT_XMLHashtable xml;//Используется для ускорения доступа к xml
    public Vector2 multiplier;//Смещение от центра раскладки
    //Ссылки SlotDef
    public List<SlotDef> slotDefs;//Все экзембляры SlotDef для рядов 0-3
    public SlotDef drawPile;
    public SlotDef discardPile;
    //Хранит имена всех рядов
    public string[] sortingLayerNames = new string[] { "Row0", "Row1", "Row2", "Row3", "Discard", "Draw" };
    //Эта функция вызыватеся для чтения файла LauoutXML.xml
    public void  ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);//Загрузить XML
        xml = xmlr.xml["xml"][0];//И определяется xml для ускорения доступа к XML
        //Прочитать множители, определяющие расстояние между картами
        multiplier.x = float.Parse(xml["multiplier"][0].att("x"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        //Прочитать слоты
        SlotDef tSD;
        //slotsX используется для ускорения доступа к элементам <slot>
        PT_XMLHashList slotsX = xml["slot"];
        for (int i = 0; i < slotsX.Count; i++)
        {
            tSD = new SlotDef();//Создать новый экзембляр SlotDef
            if (slotsX[i].HasAtt("type"))
            {
                tSD.type = slotsX[i].att("type");
            } else
            {
                tSD.type = "slot";
            }
            //Преобразовать некоторые атрибуты в числовые значения
            tSD.x = float.Parse(slotsX[i].att("x"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            tSD.y = float.Parse(slotsX[i].att("y"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            tSD.layerID = int.Parse(slotsX[i].att("layer"));
            //Преобразовать номер ряда LayerID в тект layerName
            tSD.layerName = sortingLayerNames[tSD.layerID];
            switch (tSD.type)
            {
                //Прочитать доп. атрибуды, опираясь на тип слота
                case "slot":
                    tSD.faceUp = (slotsX[i].att("faceup") == "1");
                    tSD.id = int.Parse(slotsX[i].att("id"));
                    if (slotsX[i].HasAtt("hiddenby"))
                    {
                        string[] hiding = slotsX[i].att("hiddenby").Split(',');
                        foreach (string s in hiding)
                        {
                            tSD.hiddenBy.Add(int.Parse(s));
                        }
                    }
                    slotDefs.Add(tSD);
                    break;
                case "drawpile":
                    tSD.stagger.x = float.Parse(slotsX[i].att("xstagger"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    drawPile = tSD;
                    break;
                case "discardpile":
                    discardPile = tSD;
                    break;
            }
        }
    }
}
