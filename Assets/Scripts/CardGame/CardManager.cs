using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<CardData> deckCards = new List<CardData>();     //���� �ִ� ī��
    public List<CardData> handCards = new List<CardData>();         // �տ� �ִ� ī��
    public List<CardData> discardCards = new List<CardData>();      // ���� ī�� ����

    public GameObject cardPrefeb;               // ī�� ������
    public Transform deckPosition;             // �� ��ġ
    public Transform handPosition;      //�� �߾� ��ġ
    public Transform disCardPosition;               // ���� ī����� ��ġ

    public List<GameObject> cardObjects = new List<GameObject>();       // ���� ī�� ���� ������Ʈ�� 
    
    
    // Start is called before the first frame update
    void Start()
    {
        ShuffleDeck();                  //���� �� ī�� ����
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.D))        // d Ű�� ������ ī�� ��ο�
        {
            DrawCard();
        }

        if(Input.GetKeyDown(KeyCode.F))             // fŰ�� ������ ���� ī�帣 ���̷� �ǵ����� ����
        {
            ReturnDiscardToDeck();
        }

        ArrangHand();                           // ���� ��ġ ������Ʈ
    }

    // �� ����
    public void ShuffleDeck()
    {
        List<CardData> tempDeck = new List<CardData>(deckCards);    // �ӽ� �����v ī�� ����
        deckCards.Clear();

        while (tempDeck.Count > 0)
        {
            int randIndex = Random.Range(0, tempDeck.Count);
            deckCards.Add(tempDeck[randIndex]);
            tempDeck.RemoveAt(randIndex);
        }

        Debug.Log("���� �������ϴ�. : " + deckCards.Count + "��");
    }

    //ī�� ��ο�
    public void DrawCard()
    {
        if (handCards.Count >= 6)
        {
            Debug.Log("���а� ���� á���ϴ�., ! (�ִ� 6��)");
            return;
        }

        if (deckCards.Count == 0)
        {
            Debug.Log("���� ī�尡 �����ϴ�");
            return;
        }

        // ������ ���� ī�� ��������
        CardData cardData = deckCards[0];
        deckCards.RemoveAt(0);

        // ���п� �߰�
        handCards.Add(cardData);

        //ī�� ���� ������Ʈ ����
        GameObject cardObj = Instantiate(cardPrefeb, deckPosition.position, Quaternion.identity);

        //ī�� ���� ����
        CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
        if (cardDisplay != null)
        {
            cardDisplay.SetupCard(cardData);
            cardDisplay.cardIndex = handCards.Count - 1;
            cardObjects.Add(cardObj);
        }

        // ���� ��ġ ������Ʈ
         ArrangHand();

        Debug.Log("ī�带 ��ο� �߽��ϴ�. : " + cardData.cardName + "(���� : " + handCards.Count + "/6");
    }

    public void ArrangHand()
    {
        if (handCards.Count == 0) return;

        // ���� ��ġ�� ���� ����
        float cardWidth = 1.2f;
        float spacing = cardWidth + 1.8f;
        float totalWidth = (handCards.Count - 1) * spacing;
        float startX = -totalWidth / 2f;

        // �� ī�� ��ġ����
        for (int i = 0; i < cardObjects.Count; i++)
        {
            if (cardObjects[i] != null)
            {
                // �巡�� ���� ī��� �ǳʶ٤��⤿
                CardDisplay display = cardObjects[i].GetComponent<CardDisplay>();
                if (display != null && display.isDragging)
                    continue;

                //��ǥ ��ġ ���
                Vector3 tarPosition = handPosition.position + new Vector3(startX + ( i * spacing), 0, 0);

                //�ε巯�� �̵�
                cardObjects[i].transform.position = Vector3.Lerp(cardObjects[i].transform.position, tarPosition, Time.deltaTime * 10f);
                
                    
                
            }
        }
    }
    public void DiscardCard(int handIndex)    // ī�� ������
    {
        if(handIndex < 0 || handIndex >= handCards.Count)
        {
            Debug.Log("��ȿ���� ���� ī�� �ε��� �Դϴ�!");
            return;
        }

        //���п��� ī�� ��������
        CardData cardData = handCards[handIndex];   
        handCards.RemoveAt(handIndex);

        //���� ī�� ���̿� �߰�
        discardCards.Add(cardData);

        // �ش� ī�� ���� ����Ʈ ����
        if(handIndex < cardObjects.Count)
        {
            Destroy(cardObjects[handIndex]);
            cardObjects.RemoveAt(handIndex);
        }

        // ī�� �ε��� �缳��
        for (int i = 0; i < cardObjects.Count; i++)  // ī�� �ε��� �缳��
        {
            CardDisplay display = cardObjects[i] .GetComponent<CardDisplay>();
            if (display != null) display.cardIndex = i;
        }

        ArrangHand();               // ������ġ ������Ʈ
        Debug.Log("ī�带 ���Ƚ��ϴ�." + cardData.cardName);
    }

    //����ī�带 ������ �ǵ����� ����

    public void ReturnDiscardToDeck()
    {
        if (discardCards.Count == 0)
        {
            Debug.Log("���� ī�� ���̰� ��� �ֽ��ϴ�");
            return;
        }
        deckCards.AddRange(discardCards);                                       // ���� ī�带 ��� ��[�����߰�
        discardCards.Clear();                                            //   ���� ī�� ���� ����
        ShuffleDeck();                                                          // ������

        Debug.Log("����ī�� " + deckCards.Count + " ���� ������ �ǵ���� �������ϴ�.");

    }

}
