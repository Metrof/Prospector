using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Перечисление, определяющее тип переменной, которая может принимать несколько предопределенных значений
public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}
public class CardProspector : Card //CardProspector должен расширять Card
{
    [Header("Set Dynamically: CardProspector")]
    public bool itsGold;
    //Так используется перечисление eCardState
    public eCardState state = eCardState.drawpile;
    //hiddenBy - список других карт, не позволяющих перевернуть эту лицом вверх
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    //LayoutID определяет для этой карты ряд в раскладке
    public int layoutID;
    //Класс SlotDef хранит информацию из элемента <slot> в layoutXML
    public SlotDef slotDef;
    //Определить реакцию карт на щелчок мыши
    public override void OnMouseUpAsButton()
    {
        //вызвать метод cardClicer обьекта-одиночки Prospector
        Prospector.S.CardClicer(this);
        //а также версию этого метода в базовом классе (Card.cs)
    }
}
