using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemSO> items = new List<ItemSO>();

    // 캐싱을 위한 사전
    private Dictionary<int, ItemSO> itemsByld;
    private Dictionary<string, ItemSO> itemsByName;

    public void Initalize()
    {
        itemsByld = new Dictionary<int, ItemSO>();
        itemsByName = new Dictionary<string, ItemSO>();

        foreach (var item in items)
        {
            itemsByld[item.id] = item;
            itemsByName[item.itemName] = item;
        }
    }

    public ItemSO GetItemByld(int id)
    {
        if (itemsByld == null)
        {
            Initalize();
        }
        if (itemsByld.TryGetValue(id, out var item))
            return item;

        return null;
    }

    public ItemSO GetItemByName(string name)
    {
        if (itemsByName == null)
        {
            Initalize();
        }
        if (itemsByName.TryGetValue(name, out var item))
            return item;

        return null;
    }

    public List<ItemSO> GetItemByType(ItemType type)
    {
        return items.FindAll(item => item.itemtype == type);
    }


        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
