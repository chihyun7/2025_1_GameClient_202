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
    private string jsonfFilePath = "";    // json ���� ��� ���ڰ�
    private string outputFolder = "Assets/ScriptableObjects/items";  // ��� so ������ ��ΰ�
    private bool createDatabase = true;  //  ������ ���̽��� ����� �������� ���� bool ��


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
        // json ���� �б�
        string jsonText = File.ReadAllText(jsonfFilePath);

        try
        {
            // Json �ǽ�
            List<ItemData> ItemDataList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

            List<ItemSO> createdItems = new List<ItemSO>();

            // �� ������ �����͸� ��ũ���ͺ� ������Ʈ�� ��ȯ
            foreach (var itemData in ItemDataList)
            {
                ItemSO itemSO = ScriptableObject.CreateInstance<ItemSO>();    //ItemSO ���� ����

                // ������ ����
                itemSO.id = itemData.id;
                itemSO.itemName = itemData.itemName;
                itemSO.nameEng = itemData.nameEng;
                itemSO.description = itemData.description;


                // ������ ��ȯ

                if (System.Enum.TryParse(itemData.itemTypeString, out ItemType parsedType))
                {
                    itemSO.itemtype = parsedType;
                }
                else
                {
                    Debug.LogWarning($"������ '{itemData.itemName}'dml ��ȿ���� ���� ������ Ÿ�� : {itemData.itemTypeString}");
                }
                itemSO.price = itemData.price;
                itemSO.power = itemData.power;
                itemSO.level = itemData.level;
                itemSO.isStackable = itemData.isStackable;


                // �����ܷε� (��ΰ� �ִ�����)
                if (!string.IsNullOrEmpty(itemData.iconPath))
                {
                    itemSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{itemData.iconPath}.png");

                    if (itemSO.icon != null)
                    {
                        Debug.LogWarning($"������' {itemData.nameEng}'�� �������� ã�� �� �����ϴ�. : {itemData.iconPath}");
                    }
                }


                // ��ũ���ͺ� ������Ʈ ���� - id�� 4�ڸ� ���ڷ�������
                string assetpath = $"{outputFolder}/Item_{itemData.id.ToString("D4")}_{itemData.nameEng}.asset";
                AssetDatabase.CreateAsset(itemSO, assetpath);

                //���� �̸� ����
                itemSO.name = $"item{itemData.id.ToString("D4")}+{itemData.nameEng}";
                createdItems.Add(itemSO);

                EditorUtility.SetDirty(itemSO);
            }

            // ������ ���̽� ����
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
            Debug.LogError($"JSON��ȯ���� : {e}");
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