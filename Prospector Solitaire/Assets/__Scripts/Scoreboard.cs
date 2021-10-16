using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//класс Scoreboard управляет отображением очков игрока
public class Scoreboard : MonoBehaviour
{
    public static Scoreboard S;//одиночка

    [Header("Set in Inspector")]
    public GameObject prefabFloatingScore;

    [Header("Set Dynamically")]
    [SerializeField] private int _score = 0;
    [SerializeField] private string _scoreString;

    private Transform canvasTrans;

    //Свойство score также устанавливает scoreString
    public int score
    {
        get { return _score; }
        set 
        { 
            _score = value;
            scoreString = _score.ToString("N0");
        }
    }

    //свойство scoreString также устанавливает Text.text
    public string scoreString
    {
        get { return _scoreString; }
        set
        {
            _scoreString = value;
            GetComponent<Text>().text = _scoreString;
        }
    }

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        } else
        {
            Debug.LogError("ERROR: Scoreboard.Awake(): S is already set!");
        }
        canvasTrans = transform.parent;
    }

    //Когда вызывается методом SendMessage, прибавляет fs.score к this.score
    public void FSCallback(FloatingScore fs)
    {
        score += fs.score;
    }
    //Создает и инициализирует новый игровой объект FloatingScore
    //Возвращает указатель на созданный экзембляр FloatingScore, чтобы вызывающая функция могла выполнить с ним доп. операции(например определить список fontSizes и т.д.)
    public FloatingScore CreateFloatingScore(int amt, List<Vector2> pts)
    {
        GameObject go = Instantiate(prefabFloatingScore);
        go.transform.SetParent(canvasTrans);
        FloatingScore fs = go.GetComponent<FloatingScore>();
        fs.score = amt;
        fs.reportFinishTo = gameObject;//Настроить обратный вызов
        fs.Init(pts);
        return (fs);
    }
}
