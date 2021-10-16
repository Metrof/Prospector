using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string suit;//����� ����� (C,D,H ��� S)
    public int rank;//����������� ����� (1-14)
    public Color color = Color.black;//���� �������
    public string colS = "Black";// or "Red".Color name
    //���� ������ ������ ��� ������� ������� Decoration
    public List<GameObject> decoGOs = new List<GameObject>();
    //���� ������ ������ ��� ������� ������� Pip
    public List<GameObject> pipGOs = new List<GameObject>();

    public GameObject back;//������� ������ ������� �����
    public CardDefinition def;//����������� �� DeckXML.xml

    //������ ����������� SpriteRenderer ����� � ��������� � ���� ������� ��������
    public SpriteRenderer[] spriteRenderers;
    private void Start()
    {
        SetSortOrder(0);//��������� ���������� ���������� ����
    }
    //���� spriteRenderers �� ���������, ��� ������� ��������� ���
    public void PopulateSpriteRenderers()
    {
        //���� spriteRenderers �������� null ��� ������ ������
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            //�������� ���������� SpriteRenderer ����� �������� ������� � ��������� � ���� ������� ��������
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }
    //�������������� ���� sorrtinglayerName �� ���� ����������� SpriteRenderer
    public void SetSortinglayerName(string tSLN)
    {
        PopulateSpriteRenderers();
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            tSR.sortingLayerName = tSLN;
        }
    }
    //�������������� ���� sortingOrder ���� ����������� SpriteRenderer
    public void SetSortOrder(int sOrd)
    {
        PopulateSpriteRenderers();
        //��������� ����� ���� ��������� � ������ spriteRenderers
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            if (tSR.gameObject == gameObject)
            {
                //���� ��������� ����������� �������� �������� ����������, ��� ���
                tSR.sortingOrder = sOrd;//���������� ���������� ����� ��� ���������� � sOrd
                continue;//� ������� � ��������� �������� �����
            }
            //������ �������� ������� ������� ����� ���
            //���������� ���������� ����� ��� ����������, � ����������� �� �����
            switch (tSR.gameObject.name)
            {
                case "back"://���� ��� back, �� ���������� ���������� ���������� ����� ��� ����������� ������ ������ ��������
                    tSR.sortingOrder = sOrd + 2;
                    break;
                case "face"://���� ��� face
                default://��� �� ������,���������� ������������� ���������� ����� ��� ����������� ������ ����
                    tSR.sortingOrder = sOrd + 1;
                    break;
            }
        }
    }
    public bool faceUP
    {
        get { return !back.activeSelf; }
        set { back.SetActive(!value); }
    }
    //����������� ������ ����� ��������������� � ���������� ������������ ������� � ���� �� �������
    virtual public void OnMouseUpAsButton()
    {
        print(name);//�� ������ ��� ������ ������� ��� �����
    }
}

[System.Serializable]//������������� ����� �������� ��� ������ � ����������
public class Decorator
{//���� ����� ������ ���������� �� DeckXML � ������ ������ �� �����
    public string type;//������ ������������ ����������� �����
    public Vector3 loc;//�������������� ������� �� �����
    public bool flip = false;//������� ���������� ������� �� ���������
    public float scale = 1f;//������� �������
}

[System.Serializable]
public class CardDefinition
{//���� ����� ������ ���������� � ����������� �����
    public string face;//������, ������������ ������� ������� �����
    public int rank;//����������� �����(1-13)
    public List<Decorator> pips = new List<Decorator>();//������
}
