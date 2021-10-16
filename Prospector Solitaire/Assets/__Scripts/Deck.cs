using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Set in Inspector")]
    public bool startFaceUp = false;
    //�����
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;

    public Sprite[] faceSprites;
    public Sprite[] ranksprites;

    public Sprite cardBack;
    public Sprite cardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontGold;
    //�������
    public GameObject prefabCard;
    public GameObject prefabSprite;

    [Header("Set Dynamically")]
    public PT_XMLReader xmlr;
    public List<string> cardNames;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefs;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuits;
    //InitDeck ���������� ����������� Prospector, ����� ����� �����
    public void InitDeck(string deckXMLText)
    {
        //������� ����� �������� ��� ���� ������� �������� Card � ��������
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }
        //���������������� ������� �� ��������� ������� ������
        dictSuits = new Dictionary<string, Sprite>()
        {
            {"C", suitClub },
            {"D", suitDiamond },
            {"H", suitHeart },
            {"S", suitSpade }
        };
        ReadDeck(deckXMLText);//ReadDeck ������ ��������� XML-���� � ������� ������ ����������� CardDefinition
        MakeCards();
    }

    public void ReadDeck(string deckXMLText)
    {
        xmlr = new PT_XMLReader();//������� ����� ��������� PT_XMLReader
        xmlr.Parse(deckXMLText);//������������ ��� ��� ������ DeckXML
        //����� ����������� ������,����� ��������,��� ������������ xmlr
        string s = "xml[0] decorator[0]";
        s += "type=" + xmlr.xml["xml"][0]["decorator"][0].att("type");
        s += " x=" + xmlr.xml["xml"][0]["decorator"][0].att("x");
        s += " y=" + xmlr.xml["xml"][0]["decorator"][0].att("y");
        s += " scale=" + xmlr.xml["xml"][0]["decorator"][0].att("scale");
        //print(s);

        decorators = new List<Decorator>();//��������� ��������  decorator ��� ���� ����
        //������� ������ PT_XMLHashList ���� ��������� decorator �� XML-�����
        PT_XMLHashList xDecos = xmlr.xml["xml"][0]["decorator"];//������ �������� ������
        Decorator deco;//���� ����� ���������� ������
        for (int i = 0; i < xDecos.Count; i++)
        {
            //��� ������� �������� <Decorator> � XML
            deco = new Decorator();
            // ����������� �������� �� <decorators> � Decorator
            deco.type = xDecos[i].att("type");
            //deco.flip ������� �������� true, ���� ������� �������� ����� "1"
            deco.flip = xDecos[i].att("flip") == "1";
            //������� �������� float �� ��������� ���������
            deco.scale = float.Parse(xDecos[i].att("scale"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            //Vector3 loc ���������������� ��� [0,0,0],������� ��� ��������� ������ �������� ���
            deco.loc.x = float.Parse(xDecos[i].att("x"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            deco.loc.y = float.Parse(xDecos[i].att("y"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            deco.loc.z = float.Parse(xDecos[i].att("z"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            //�������� ����� � ������ decorators
            decorators.Add(deco);
        }
        //��������� ���������� ��� �������, ������������ ����������� �����
        cardDefs = new List<CardDefinition>();//���������������� ������ ����
        //������� ������ PT_XMLhashList ���� ��������� <card> �� XML-�����
        PT_XMLHashList xCardDefs = xmlr.xml["xml"][0]["card"];
        for (int i = 0; i < xCardDefs.Count; i++)
        {
            //��� ������� ���������� <card>
            //������� ��������� CardDefinition
            CardDefinition cDef = new CardDefinition();
            //�������� �������� �������� � �������� �� � cDef
            cDef.rank = int.Parse(xCardDefs[i].att("rank"));
            //������� ������ PT_XMLhashList ���� ��������� <pip> ������ ����� �������� <card>
            PT_XMLHashList xPips = xCardDefs[i]["pip"];
            if (xPips != null)
            {
                for (int j = 0; j < xPips.Count; j++)
                {
                    //������ ��� �������� <pip>
                    deco = new Decorator();
                    //�������� <pip> � <card> �������������� ������� Decorator
                    deco.type = "pip";
                    deco.flip = xPips[j].att("flip") == "1";
                    deco.loc.x = float.Parse(xPips[j].att("x"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    deco.loc.y = float.Parse(xPips[j].att("y"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    deco.loc.z = float.Parse(xPips[j].att("z"), System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    if (xPips[j].HasAtt("scale"))
                    {
                        deco.scale = float.Parse(xPips[j].att("scale"));
                    }
                    cDef.pips.Add(deco);
                }
            }
            //����� � ���������� ���� ����������� ����� ������� fase
            if (xCardDefs[i].HasAtt("face"))
            {
                cDef.face = xCardDefs[i].att("face");
            }
            cardDefs.Add(cDef);
        }
    }
    //�������� CardDefinition �� ������ �������� ����������� ( 0� 1 �� 14 - �� ���� �� ������)
    public CardDefinition GetCardDefinitionByRank(int rnk)
    {
        //����� �� ���� ������������ CardDefinition
        foreach (CardDefinition cd in cardDefs)
        {
            //���� ���������� ���������, ������� ��� �����������
            if (cd.rank == rnk)
            {
                return cd;
            }
        }
        return null;
    }
    //������� ������� ������� ����
    public void MakeCards()
    {
        //cardNames ����� ��������� ����� ����������������� ����
        //������ ����� ����� 14 �������� �����������
        // (�������� ��� ���� (Clubs): �� �1 �� �14)
        cardNames = new List<string>();
        string[] letters = new string[] { "C", "D", "H", "S" };
        foreach (string s in letters)
        {
            for (int i = 0; i < 13; i++)
            {
                cardNames.Add(s + (i + 1));
            }
        }
        //������� ������ �� ����� �������
        cards = new List<Card>();
        //������ ��� ������ ��� ��������� ����� ����
        for (int i = 0; i < cardNames.Count; i++)
        {
            //������� ����� � �������� �� � ������
            cards.Add(MakeCard(i));
        }
    }
    private Card MakeCard(int cNum)
    {
        //������� ����� ������� ������ � ������
        GameObject cgo = Instantiate(prefabCard);
        //��������� transform.parent ����� ����� � ������������ � ������ ��������
        cgo.transform.parent = deckAnchor;
        Card card = cgo.GetComponent<Card>();//�������� ��������� Card
        //��� ������ ����������� ����� � ���������� ���
        cgo.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        //��������� �������� ��������� �����
        card.name = cardNames[cNum];
        card.suit = card.name[0].ToString();
        card.rank = int.Parse(card.name.Substring(1));
        if (card.suit == "D" || card.suit == "H")
        {
            card.colS = "Red";
            card.color = Color.red;
        }
        //�������� CardDefinition ��� ���� �����
        card.def = GetCardDefinitionByRank(card.rank);
        AddDecorators(card);
        AddPips(card);
        AddFace(card);
        AddBack(card, cardBack);
        return card;
    }

    //��������� ������� ���������� ������������ ���������������� ��������
    private Sprite _tSp = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSR = null;

    private void AddDecorators(Card card)
    {
        //�������� ����������
        foreach (Decorator deco in decorators)
        {
            if (deco.type == "suit")
            {
                //������� ��������� �������� ������� �������
                _tGO = Instantiate(prefabSprite);
                //�������� ������ �� ��������� SpriteRenderer
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                //���������� ������ �����
                _tSR.sprite = dictSuits[card.suit];
            } else
            {
                _tGO = Instantiate(prefabSprite);
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                //�������� ������ ��� ����������� �����������
                _tSp = ranksprites[card.rank];
                //���������� ������ ����������� � SpriteRenderer
                _tSR.sprite = _tSp;
                //���������� ����� ��������������� �����
                _tSR.color = card.color;
            }
            //��������� ������� ��� ������
            _tSR.sortingOrder = 1;
            //������� ������ �������� �� ��������� � �����
            _tGO.transform.SetParent(card.transform);
            //���������� localPosition, ��� ���������� � xml
            _tGO.transform.localPosition = deco.loc;
            //����������� ������, ���� ����������
            if (deco.flip)
            {
                //������� ������� �� 180 �������� ������������ ��� Z-axis
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            //���������� ������� ����� ��������� ������ �������
            if (deco.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * deco.scale;
            }
            //���� ��� ����� �������� ������� ��� �����������
            _tGO.name = deco.type;
            //�������� ���� ������� ������ � ����������� � ������ card.decoGOs
            card.decoGOs.Add(_tGO);
        }
    }

    private void AddPips(Card card)
    {
        //��� ������� ������ � �����������...
        foreach (Decorator pip in card.def.pips)
        {
            _tGO = Instantiate(prefabSprite);
            //��������� ��������� ������� ������ �����
            _tGO.transform.SetParent(card.transform);
            //���������� LocalPosition, ��� ���������� � XML
            _tGO.transform.localPosition = pip.loc;
            //����������� ���� ����������
            if (pip.flip)
            {
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            //�������������� ���� �����(������ ��� ����)
            if (pip.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }
            //���� ��� �������� �������
            _tGO.name = "pip";
            //�������� ������ �� ��������� SpriteRenderer
            _tSR = _tGO.GetComponent<SpriteRenderer>();
            //���������� ������ �����
            _tSR.sprite = dictSuits[card.suit];
            //���������� sortingOrder, ����� ������ ����������� ��� Card_front
            _tSR.sortingOrder = 1;
            //�������� ���� ������� ������ � ������ �������
            card.pipGOs.Add(_tGO);
        }
    }
    private void AddFace(Card card)
    {
        if (card.def.face == "")
        {
            return;//����� ���� ��� �� ����� � ���������
        }
        _tGO = Instantiate(prefabSprite);
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        //������������� ��� � �������� ��� � GetFace()
        _tSp = GetFace(card.def.face + card.suit);
        _tSR.sprite = _tSp;//���������� ���� ������ � _tSR
        _tSR.sortingOrder = 1;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = "face";
    }
    //������� ������ � ��������� ��� �����
    private Sprite GetFace(string faceS)
    {
        foreach (Sprite _tSp in faceSprites)
        {
            //���� ������ ������ � ��������� ������
            if (_tSp.name == faceS)
            {
                //...������� ���
                return _tSp;
            }
            //���� ������ �� �������, �� ������� null
        }
        return null;
    }
    public void AddBack(Card card, Sprite backCard)
    {
        //�������� �������
        //Card_back ����� ��������� ��� ��������� �� �����
        _tGO = Instantiate(prefabSprite);
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        _tSR.sprite = backCard;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition = Vector3.zero;
        //������ �������� sortingOrder, ��� � ������ ��������
        _tSR.sortingOrder = 2;
        _tGO.name = "back";
        card.back = _tGO;
        //�� ��������� ��������� �����
        card.faceUP = startFaceUp;//������������ �������� FaceUp �����
    }
    //������������� ����� � Deck.cards
    static public void Shuffle(ref List<Card> oCards)
    {
        //������� ��������� ������ ��� �������� ���� � ������������ �������
        List<Card> tCards = new List<Card>();
        int ndx;//����� ������� ������ ������������ �����
        tCards = new List<Card>();//���������������� ��������� ������
        //���������, ���� �� ����� ���������� ��� ����� � �������� ������
        while (oCards.Count > 0)
        {
            //������� ��������� ������ �����
            ndx = Random.Range(0, oCards.Count);
            //�������� ��� ����� �� ��������� ������
            tCards.Add(oCards[ndx]);
            // � ������� ����� �� ��������� ������
            oCards.RemoveAt(ndx);
        }
        //�������� �������� ������ ���������
        oCards = tCards;
        //��� ��� oCards - �������� ������ ref, ������������ ��������, ���������� � �����, ���� ���������
    }
}