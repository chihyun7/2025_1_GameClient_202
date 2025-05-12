using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard" , menuName = "Cards/Card Data")]

public class CardData : ScriptableObject
{
    public enum CardType            // ī��Ÿ�� ������ �߰�
    {
        Attack,         //����    
        Heal,           //ȸ��
        Buff,           //����
        Utility         //��ƿ��Ƽ
    }

    public enum AdditionalEffectType
    {
        None,                      //c�߰� ȿ�� ����
        DrawCard,               // ī�� ��ο�
        DiscardCard,            // ī�� ������
        GainMana,                   // ���� ȹ��
        ReduceEnemyMana,        // �� ���� ����
        ReduceCardCost          // ���� ī�� ��� ����
    }

    //�߰� ȿ�� ����Ʈ
    public List<AdditionalEffect> additionalEffects = new List< AdditionalEffect > ();

    public string cardName;     //ī���̸�
    public string description;  //ī�弳��
    public Sprite artwork;      //ī�� �̹���
    public int manaCost;        //�������
    public int effectAmount;    // ���ݷ�/ȿ����
    public CardType cardType;   // ī�� Ÿ��

    public Color GetCardColor()
    {
        switch(cardType)
        {
            case CardType.Attack:
                return new Color(0.9f, 0.3f, 0.3f);    //����
            case CardType.Heal:
                return new Color(0.3f, 0.9f, 0.3f);     //���
            case CardType.Buff: 
                return new Color(0.3f, 0.3f, 0.9f);     //�Ķ�
            case CardType.Utility:      
                return new Color(0.9f, 0.9f, 0.3f);     //���
            default:
                return Color.white;
        }
    }

    //�߰� ȿ�� ������ ���ڿ��� ��ȯ

    public string GetAdditionalEffectDescription()
    {
        if (additionalEffects.Count == 0)
            return "";

        string result = "\n";
        foreach(var effect in additionalEffects)
        {
            result += effect.GetDescription() + "\n";
        }

        return result;
    }
}
