#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class JsonToScriptableConverter : EditorWindow
{
    private string jsonfFilePath = "";    // json 파일 경로 문자값
    private string outputFolder = "Assets/ScriptableObjects/items";  // 출력 so 파일을 경로값
    private bool createDatabase = true;  //  데이터 베이스를 사용할 것인지에 대한 bool 값


    [MenuItem("Tools/Json to Scriptable Objects")]

    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("Json to Scriptable Objects");
    }

    private void ConvertJsonToScriptableobjects()
    {

        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        // json 파일 읽기
        string jsonText = File.ReadAllText(jsonfFilePath);

        try
        {
            // Json 피싱
            List<ItemData> ItemDataList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

            List<ItemSO> createdItems = new List<ItemSO>();

            // 각 아이템 데이터를 스크립터블 오브젝트로 변환
            foreach (var itemData in ItemDataList)
            {
                ItemSO itemSO = ScriptableObject.CreateInstance<ItemSO>();    //ItemSO 파일 생성

                // 데이터 복사
                itemSO.id = itemData.id;
                itemSO.itemName = itemData.itemName;
                itemSO.nameEng = itemData.nameEng;
                itemSO.description = itemData.description;


                // 열거형 변환

                if (System.Enum.TryParse(itemData.itemTypeString, out ItemType parsedType))
                {
                    itemSO.itemtype = parsedType;
                }
                else
                {
                    Debug.LogWarning($"아이템 '{itemData.itemName}'dml 유효하지 않은 아이템 타입 : {itemData.itemTypeString}");
                }
                itemSO.price = itemData.price;
                itemSO.power = itemData.power;
                itemSO.level = itemData.level;
                itemSO.isStackable = itemData.isStackable;


                // 아이콘로드 (경로가 있눈굥유)
                if (!string.IsNullOrEmpty(itemData.iconPath))
                {
                    itemSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{itemData.iconPath}.png");

                    if (itemSO.icon != null)
                    {
                        Debug.LogWarning($"아이템' {itemData.nameEng}'의 아이콘을 찾을 수 없습니다. : {itemData.iconPath}");
                    }
                }


                // 스크립터블 오브젝트 저장 - id를 4자리 숫자로포맷팅
                string assetpath = $"{outputFolder}/Item_{itemData.id.ToString("D4")}_{itemData.nameEng}.asset";
                AssetDatabase.CreateAsset(itemSO, assetpath);

                //에셋 이름 지정
                itemSO.name = $"item{itemData.id.ToString("D4")}+{itemData.nameEng}";
                createdItems.Add(itemSO);

                EditorUtility.SetDirty(itemSO);
            }

            // 데이터 베이스 생성
            if (createDatabase && createdItems.Count > 0)
            {
                ItemDatabaseSO database = ScriptableObject.CreateInstance<ItemDatabaseSO>();
                database.items = createdItems;

                AssetDatabase.CreateAsset(database, $"{outputFolder}/ItemDatabase.asset");
                EditorUtility.SetDirty(database);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdItems.Count} scriptable objects!", "OK");


        }


        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Faild to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON변환오류 : {e}");
        }  
    }
    void OnGUI()
    {
        GUILayout.Label("JSON to scriptable Object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Select JSON File"))
        {
            jsonfFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "JSON");
        }

        EditorGUILayout.LabelField("Selected File : ", jsonfFilePath);
        EditorGUILayout.Space();
        outputFolder = EditorGUILayout.TextField("Output Folder :", outputFolder);
        createDatabase = EditorGUILayout.Toggle("Create Database Aseet", createDatabase);
        EditorGUILayout.Space();

        if (GUILayout.Button("convert to Scriptable Objects"))
        {
            if (string.IsNullOrEmpty(jsonfFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a JSON file firest!", "OK");
                return;
            }
            ConvertJsonToScriptableobjects();
        }

    }

}

    



    





#endif