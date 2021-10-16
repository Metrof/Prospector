using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������������, ������������ ��� ����������, ������� ����� ��������� ��������� ���������������� ��������
public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}
public class CardProspector : Card //CardProspector ������ ��������� Card
{
    [Header("Set Dynamically: CardProspector")]
    public bool itsGold;
    //��� ������������ ������������ eCardState
    public eCardState state = eCardState.drawpile;
    //hiddenBy - ������ ������ ����, �� ����������� ����������� ��� ����� �����
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    //LayoutID ���������� ��� ���� ����� ��� � ���������
    public int layoutID;
    //����� SlotDef ������ ���������� �� �������� <slot> � layoutXML
    public SlotDef slotDef;
    //���������� ������� ���� �� ������ ����
    public override void OnMouseUpAsButton()
    {
        //������� ����� cardClicer �������-�������� Prospector
        Prospector.S.CardClicer(this);
        //� ����� ������ ����� ������ � ������� ������ (Card.cs)
    }
}
