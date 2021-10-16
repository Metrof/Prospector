using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Prospector : MonoBehaviour
{
    static public Prospector S;

    [Header("Set in Inspector")]
    public TextAsset deckXMl;
    public TextAsset layoutXML;
    public float xOffSet = 3;
    public float YOffSet = -2.5f;
    public Vector3 layoutCenter;
    public Vector2 fsPosMid = new Vector2(0.5f, 0.90f);
    public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);
    public Vector2 fsPosMid2 = new Vector2(0.4f, 1.0f);
    public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
    public float reloadDelay = 2f;//�������� ����� �������� 2 ���
    public Text gameOverText, roundResultText, highScoreText;

    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;
    public List<CardProspector> drawPile;
    public Transform layoutAnchor;
    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;
    public FloatingScore fsRun;

    private void Awake()
    {
        S = this;
        SetUpUITexts();
    }
    void SetUpUITexts()
    {
        //��������� ������ HighScore
        GameObject go = GameObject.Find("HighScore");
        if (go != null)
        {
            highScoreText = go.GetComponent<Text>();
        }
        int highScore = ScoreManager.HIGH_SCORE;
        string hScore = "High Score: " + Utils.AddCommasToNumber(highScore);
        go.GetComponent<Text>().text = hScore;

        //��������� ������� ������������ � ����� ������
        go = GameObject.Find("GameOver");
        if (go != null)
        {
            gameOverText = go.GetComponent<Text>();
        }
        go = GameObject.Find("RoundResult");
        if (go != null)
        {
            roundResultText = go.GetComponent<Text>();
        }
        //������ �������
        ShowResultsUI(false);
    }
    void ShowResultsUI(bool show)
    {
        gameOverText.gameObject.SetActive(show);
        roundResultText.gameObject.SetActive(show);
    }
    private void Start()
    {
        Scoreboard.S.score = ScoreManager.SCORE;
        deck = GetComponent<Deck>();//�������� ��������� Deck
        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);
        deck.InitDeck(deckXMl.text);//�������� ��� DeckXML
        Deck.Shuffle(ref deck.cards);//���������� ������ ������� �� �� ������
        drawPile = ConvertListCardToListCardProspectors(deck.cards);
        LayoutGame();

        //Card c;
        //for (int cNum = 0; cNum < deck.cards.Count; cNum++)//���� ���� ������� ����� � �����, ������������ �������
        //{
        //    c = deck.cards[cNum];
        //    c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        //}
    }

    List<CardProspector> ConvertListCardToListCardProspectors(List<Card> lCD)
    {
        List<CardProspector> lCP = new List<CardProspector>();
        CardProspector tCP;
        foreach (Card tCD in lCD)
        {
            tCP = tCD as CardProspector;
            lCP.Add(tCP);
        }
        return lCP;
    }
    //������� Draw ������� ���� ����� � ������� drawPile � ���������� ��
    CardProspector Draw()
    {
        CardProspector cd = drawPile[0];//����� ������� ����� CardProspector
        drawPile.RemoveAt(0);//������� �� List<> drawPile
        return cd;//� ������� ��
    }
    //LayoutGame() ���������� ����� � ��������� ��������� - "�����"
    void LayoutGame()
    {
        //������� ������ ������� ������, ������� ����� ������� ������� ���������
        if (layoutAnchor == null)
        {
            GameObject tGo = new GameObject("_LayoutAnchor");//��� �� ����� ����� ��� ������, � ������ ������ ������� � ������������
            //������� ������ ������� ������ � ������ _LayoutAnchor � ��������
            layoutAnchor = tGo.transform;//�������� ��� ��������� Transform
            layoutAnchor.transform.position = layoutCenter;//��������� � �����
        }
        CardProspector cp;
        //��������� �����
        foreach (SlotDef tSD in layout.slotDefs)
        {
            //��������� ����� ���� ����������� slotDefs � layout.slotDefs
            cp = Draw();//������� ������ ����� (������) �� ������ drawpile
            cp.faceUP = tSD.faceUp;//���������� �� ������� faceUp � ������������ � ������������ � SlotDef
            cp.transform.parent = layoutAnchor;//��������� layoutAnchor �� ���������
            //��� �������� ������� ����������� ��������: deck.deckAnchor, ������� ����� ������� ���� ������������ � �������� � ������ _Deck
            cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.layerID);
            //���������� localPosition ����� � ������������ � ������������ � SlotDef
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;
            //����� CardProspector � �������� ��������� ����� ��������� CardState.tableau;
            cp.state = eCardState.tableau;
            float setGold = Random.value;
            if (setGold <= 0.1f)
            {
                cp.itsGold = true;
                SpriteRenderer sr = cp.GetComponent<SpriteRenderer>();
                sr.sprite = deck.cardFrontGold;
                SpriteRenderer srBack = cp.back.GetComponent<SpriteRenderer>();
                srBack.sprite = deck.cardBackGold;
            }
            cp.SetSortinglayerName(tSD.layerName);//��������� ���� ����������
            tableau.Add(cp);//��������� ����� � ������ tableau
        }
        //��������� ������ ����, �������� ����������� ������
        foreach (CardProspector tCP in tableau)
        {
            foreach (int hid in tCP.slotDef.hiddenBy)
            {
                cp = FindCardByLayoutID(hid);
                tCP.hiddenBy.Add(cp);
            }
        }
        //������� ��������� ������� �����
        MoveToTarger(Draw());
        //��������� ������ ��������� ����
        UpdateDrawPile();
    }
    //����������� ����� ����� layuotID � ��������� CardProspector � ���� �������
    CardProspector FindCardByLayoutID(int LayoutID)
    {
        foreach (CardProspector tCP in tableau)
        {
            //����� �� ���� ������ ������ tableau
            if (tCP.layoutID == LayoutID)
            {
                //���� ����� ����� ����� ��������� � �������, ������� ��
                return tCP;
            }
        }
        //���� ������ �� �������,������� null
        return null;
    }
    //������������ ����� � �������� ��������� ������� �������� ����� ��� ����
    void SetTableauFaces()
    {
        foreach (CardProspector cd in tableau)
        {
            bool faceUP = true;//������������, ��� ����� ������ ���� ��������� ������� �������� �����
            foreach (CardProspector cover in cd.hiddenBy)
            {
                //���� ����� �� ����,����������� �������, ����������� � �������� ���������
                if (cover.state == eCardState.tableau)
                {
                    faceUP = false;//��������� ������� �������� ����
                }
            }
            cd.faceUP = faceUP;//��������� ����� ��� ��� �����
        }
    }
    //���������� ������� ������� ����� � ������ ���������� ����
    void MoveToDiscard(CardProspector cp)
    {
        //���������� ��������� ����� ��� discard(��������)
        cp.state = eCardState.discard;
        discardPile.Add(cp);
        cp.transform.parent = layoutAnchor;
        cp.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID+0.5f);
        cp.faceUP = true;
        //��������� ������ ������ ��� ���������� �� �������
        cp.SetSortinglayerName(layout.discardPile.layerName);
        cp.SetSortOrder(-100 + discardPile.Count);
    }
    //������ ����� cd ����� ������� ������
    void MoveToTarger(CardProspector cd)
    {
        //���� ������� ����� ����������, ����������� �� � ������ ���������� ����
        if (target != null) MoveToDiscard(target);
        target = cd;//cd-����� ������� �����
        cd.state = eCardState.target;
        cd.transform.parent = layoutAnchor;
        //����������� �� ����� ��� ������� �����
        cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID);
        cd.faceUP = true;//��������� ������� �������� �����
        //��������� ���������� �� �������
        cd.SetSortinglayerName(layout.discardPile.layerName);
        cd.SetSortOrder(0);
    }
    //������������ ������ ��������� ����, ����� ���� �����, ������� ���� ��������
    void UpdateDrawPile()
    {
        CardProspector cd;
        //��������� ����� ���� ���� � drawPile
        for (int i = 0; i < drawPile.Count; i++)
        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;
            //����������� � ������ �������� layout.discardPile.stagger
            Vector2 dpStagger = layout.drawPile.stagger;
            cd.transform.localPosition = new Vector3(layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x), layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y), -layout.drawPile.layerID + 0.1f * i);
            cd.faceUP = false;
            cd.state = eCardState.drawpile;
            //��������� ���������� �� �������
            cd.SetSortinglayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10*i);
        }
    }
    //CardClicer ���������� � ����� �� ������ �� ����� �����
    public void CardClicer(CardProspector cd)
    {
        //������� ������������ ���������� �����
        switch (cd.state)
        {
            case eCardState.drawpile:
                //������ �� ����� ����� � ������ ��������� ���� �������� � ����� ������� �����
                MoveToDiscard(target);//����������� ������� ����� � discardPile
                MoveToTarger(Draw());//����������� ������� ��������� ����� �� ����� �������
                UpdateDrawPile();//�������� ��������� ������ ��������� ����
                ScoreManager.EVENT(eScoreEvent.draw);
                FloatingScoreHandler(eScoreEvent.draw);
                break;
            case eCardState.tableau:
                //��� ����� � �������� ��������� ����������� ����������� �� ����������� �� ����� �������
                bool validMatch = true;
                if (!cd.faceUP)
                {
                    //�����, ���������� ������� �������� ���� �� ����� �����������
                    validMatch = false;
                }
                if (!AdjacentRank(cd, target))
                {
                    //���� ������� ����������� �� �����������, ����� �� ����� �����������
                    validMatch = false;
                }
                if (!validMatch) return;//����� ���� ����� �� ����� �����������
                //�� �����,���! ����� ����� ������������
                tableau.Remove(cd);//������� �� ������ tableau
                MoveToTarger(cd);//������� �� �������
                SetTableauFaces();//��������� ����� � �������� ��������� ����� ��� ����
                if (cd.itsGold)
                {
                    ScoreManager.EVENT(eScoreEvent.mineGold);
                    FloatingScoreHandler(eScoreEvent.mineGold);
                } else
                {
                    ScoreManager.EVENT(eScoreEvent.mine);
                    FloatingScoreHandler(eScoreEvent.mine);
                }
                break;
            case eCardState.target:
                //������ �� ������� ����� ������������
                break;
        }
        //��������� ���������� ����
        CheckForGameOver();
    }
    //��������� ���������� ����
    void CheckForGameOver()
    {
        //���� �������� ��������� ��������, ���� ���������
        if (tableau.Count ==0)
        {
            //������� GameOver() � ��������� ������
            GameOver(true);
        }
        //���� ���� ��� ��������� �����,���� �� �����������
        if (drawPile.Count>0)
        {
            return;
        }
        //��������� ������� ���������� �����
        foreach (CardProspector cd in tableau)
        {
            if (AdjacentRank(cd, target))
            {
                //���� ���� ���������� ���,���� �� �����������
                return;
            }
        }
        //�.�. ���������� ����� ���, ���� �����������
        //������� GameOver � ��������� ���������
        GameOver(false);
    }
    //���������� ����� ���� �����������
    void GameOver(bool won)
    {
        int score = ScoreManager.SCORE;
        if (fsRun != null)
        {
            score += fsRun.score;
        }
        if (won)
        {
            gameOverText.text = "Round Over";
            roundResultText.text = "You won this round!\nHigh score: " + score;
            ShowResultsUI(true);
            ScoreManager.EVENT(eScoreEvent.gameWin);
            FloatingScoreHandler(eScoreEvent.gameWin);
        } else
        {
            gameOverText.text = "Game Over";
            if (ScoreManager.HIGH_SCORE <= score)
            {
                string str = "You got the high score!\nHigh score: " + score;
                roundResultText.text = str;
            } else
            {
                roundResultText.text = "Your final score was: " + score;
            }
            ShowResultsUI(true);
            ScoreManager.EVENT(eScoreEvent.gameLoss);
            FloatingScoreHandler(eScoreEvent.gameLoss);
        }
        //������������� ����� ����� reloadDelay ������
        //��� �������� ����� � ������ �������� �� ����� ����������
        Invoke("ReloadLevel", reloadDelay);
    }
    void ReloadLevel()
    {
        //������������� ����� � �������� ���� � �������� ���������
        SceneManager.LoadScene("__Prospector_Scene_0");
    }
    //���������� true, ���� ��� ����� ������������� ������� ����������� (� ������ ������������ �������� ����������� ����� ����� � �������)
    public bool AdjacentRank(CardProspector c0, CardProspector c1)
    {
        //���� ����� �� ���� ��������� ������� �������� ����, ������� ����������� �� �����������
        if (!c0.faceUP || !c1.faceUP) return false;
        //���� ����������� ���� ���������� �� 1, ������� ����������� �����������
        if (Mathf.Abs(c0.rank - c1.rank) == 1)
        {
            return true;
        }
        //���� ���� ����� ���, � ������ ������, �� ������� �����������
        if (c0.rank == 1 && c1.rank == 13) return true;
        if (c0.rank == 13 && c1.rank == 1) return true;
        //����� ������� false
        return false;
    }

    //������������ �������� FloatingScore
    void FloatingScoreHandler(eScoreEvent evt)
    {
        List<Vector2> fsPts;
        switch (evt)
        {
            //� ������ ������, ��������� � ���������� ���� ����������� ���� � ���� ��������
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLoss:
                //�������� fsRun � Scoreboard
                if (fsRun != null)
                {
                    //������� ����� ������ �����
                    fsPts = new List<Vector2>();
                    fsPts.Add(fsPosRun);
                    fsPts.Add(fsPosMid2);
                    fsPts.Add(fsPosEnd);
                    fsRun.reportFinishTo = Scoreboard.S.gameObject;
                    fsRun.Init(fsPts, 0, 1);
                    //����� �������������� fontSize
                    fsRun.fontSizes = new List<float>(new float[] { 28, 36, 4 });
                    fsRun = null;//�������� fsRun, ����� ������� ������
                }
                break;

            case eScoreEvent.mine:
            case eScoreEvent.mineGold://�������� ����� �� �������� ���������
                //������� FloatingScore ��� ����������� ����� ���������� �����
                FloatingScore fs;
                //����������� �� ������� ��������� ���� mouseposition � fsPosRun
                Vector2 p0 = Input.mousePosition;
                p0.x /= Screen.width;
                p0.y /= Screen.height;
                fsPts = new List<Vector2>();
                fsPts.Add(p0);
                fsPts.Add(fsPosMid);
                fsPts.Add(fsPosRun);
                fs = Scoreboard.S.CreateFloatingScore(ScoreManager.CHAIN, fsPts);
                fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });
                if (fsRun == null)
                {
                    fsRun = fs;
                    fsRun.reportFinishTo = null;
                } else
                {
                    fs.reportFinishTo = fsRun.gameObject;
                }
                break;
        }
    }
}
