using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/ItemDatabase", order = 1)]
    public class ItemDatabase : ScriptableObject
    {
        [System.Serializable]
        public class ItemInfo
        {
            public int itemID;
            public GameObject itemPrefabs;
            public Sprite itemSprite;
            public string itemName;
            public bool isObjectSmall;
        }

        public ItemInfo[] items;

        // Method to retrieve an item by its ID
        public ItemInfo GetItemByID(int id)
        {
            foreach (ItemInfo item in items)
            {
                if (item.itemID == id)
                {
                    return item;
                }
            }
            return null; // If no item with the given ID is found
        }
    }
}
