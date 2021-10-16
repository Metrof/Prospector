using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string suit;//Масть карты (C,D,H или S)
    public int rank;//Достоинство карты (1-14)
    public Color color = Color.black;//цвет значков
    public string colS = "Black";// or "Red".Color name
    //Этот список хранит все игровые объекты Decoration
    public List<GameObject> decoGOs = new List<GameObject>();
    //Этот список хранит все игровые объекты Pip
    public List<GameObject> pipGOs = new List<GameObject>();

    public GameObject back;//Игровой объект рубашки карты
    public CardDefinition def;//Извлекается из DeckXML.xml

    //Список компонентов SpriteRenderer этого и вложенных в него игровых объектов
    public SpriteRenderer[] spriteRenderers;
    private void Start()
    {
        SetSortOrder(0);//Обеспечит правильную сортировку карт
    }
    //Если spriteRenderers не определен, эта функция определит его
    public void PopulateSpriteRenderers()
    {
        //Если spriteRenderers содержит null или пустой список
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            //Получить компоненты SpriteRenderer этого игрового объекта и вложенных в него игровых объектов
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }
    //Инициализирует поле sorrtinglayerName во всех компонентах SpriteRenderer
    public void SetSortinglayerName(string tSLN)
    {
        PopulateSpriteRenderers();
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            tSR.sortingLayerName = tSLN;
        }
    }
    //Инициализирует поле sortingOrder всех компонентов SpriteRenderer
    public void SetSortOrder(int sOrd)
    {
        PopulateSpriteRenderers();
        //Выполнить обход всех элементов в списке spriteRenderers
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            if (tSR.gameObject == gameObject)
            {
                //Если компонент принадлежит текущему игровому компоненту, это фон
                tSR.sortingOrder = sOrd;//Установить порядковый номер для сортировки в sOrd
                continue;//И перейти к следующей итерации цикла
            }
            //Каждый дочерний игровой элемент имеет имя
            //Установить порядковый номер для сортировки, в зависимости от имени
            switch (tSR.gameObject.name)
            {
                case "back"://Если имя back, то установить наибольший порядковый номер для отображения поверх других спрайтов
                    tSR.sortingOrder = sOrd + 2;
                    break;
                case "face"://если имя face
                default://Или же другое,установить промежуточный порядковый номер для отображения поверх фона
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
    //Виртуальные методы могут переопределятся в подклассах определением методов с теми же именами
    virtual public void OnMouseUpAsButton()
    {
        print(name);//По щелчку эта строка выведет имя карты
    }
}

[System.Serializable]//Сериализуемый класс доступен для правки в инспекторе
public class Decorator
{//Этот класс хранит информацию из DeckXML о каждом значке на карте
    public string type;//Значок определяющий достоинство карты
    public Vector3 loc;//Местоположение спрайта на карте
    public bool flip = false;//Признак переворота спрайта по вертикали
    public float scale = 1f;//Масштаб спрайта
}

[System.Serializable]
public class CardDefinition
{//Этот класс хранит информацию о достоинстве карты
    public string face;//Спрайт, изображающий лицевую сторону карты
    public int rank;//Достоинство карты(1-13)
    public List<Decorator> pips = new List<Decorator>();//Значки
}
