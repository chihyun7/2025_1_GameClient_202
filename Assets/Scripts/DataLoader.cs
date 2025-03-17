using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

public class DataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "items";  // resources 폴더에서 가져올 json 파일 이름

    private List<ItemData> itemList;

    void Start()
    {
        LoadItemData();
    }

    // 한글 이코딩을 위한 헬퍼 함수
    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        byte[] bytes = Encoding.Default.GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    }

    void LoadItemData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile != null)
        {
            // 원본 텍스트에서 utf-8로 변환 처리
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correntText = Encoding.UTF8.GetString(bytes);

            // 변환된 텍스트 사용
            itemList = JsonConvert.DeserializeObject<List<ItemData>>(correntText);

            Debug.Log($"로드된 아이템 수 : {itemList.Count}");

            foreach (var item in itemList)
            {
                Debug.Log($"아이템: {EncodeKorean(item.itemName)}, 설명 : {EncodeKorean(item.description)}");
            }
        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다. : {jsonFileName}");
        }
    }
}

