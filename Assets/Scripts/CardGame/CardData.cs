using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard" , menuName = "Cards/Card Data")]

public class CardData : ScriptableObject
{
    public enum CardType            // 카드타입 열거형 추가
    {
        Attack,         //공격    
        Heal,           //회복
        Buff,           //버프
        Utility         //유틸리티
    }

    public enum AdditionalEffectType
    {
        None,                      //c추가 효과 없음
        DrawCard,               // 카드 드로우
        DiscardCard,            // 카드 버리기
        GainMana,                   // 마나 획득
        ReduceEnemyMana,        // 적 마나 감소
        ReduceCardCost          // 다음 카드 비용 감소
    }

    //추과 효과 리스트
    public List<AdditionalEffect> additionalEffects = new List< AdditionalEffect > ();

    public string cardName;     //카드이름
    public string description;  //카드설명
    public Sprite artwork;      //카드 이미지
    public int manaCost;        //마나비용
    public int effectAmount;    // 공격력/효과값
    public CardType cardType;   // 카드 타입

    public Color GetCardColor()
    {
        switch(cardType)
        {
            case CardType.Attack:
                return new Color(0.9f, 0.3f, 0.3f);    //빨강
            case CardType.Heal:
                return new Color(0.3f, 0.9f, 0.3f);     //녹색
            case CardType.Buff: 
                return new Color(0.3f, 0.3f, 0.9f);     //파랑
            case CardType.Utility:      
                return new Color(0.9f, 0.9f, 0.3f);     //노랑
            default:
                return Color.white;
        }
    }

    //추가 효과 정보를 문자열로 변환

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
