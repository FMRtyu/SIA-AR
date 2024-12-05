using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity.Training
{
    [CreateAssetMenu(fileName = "SpriteDatabase", menuName = "ScriptableObjects/SpriteDatabase", order = 2)]
    public class SpriteDatabase : ScriptableObject
    {
        [System.Serializable]
        public class SpriteIconGroup
        {
            public SpriteIconType spriteType; public Sprite activatedSprite; public Sprite defaultSprite;
        }
        [System.Serializable]
        public class SpriteButtonGroup
        {
            public SpriteButtonType spriteType; public Sprite activatedSprite; public Sprite defaultSprite;
        }

        public SpriteIconGroup[] spriteIconItems;
        public SpriteButtonGroup[] spriteButtonItems;

        public SpriteIconGroup Move => GetIconItemByType(SpriteIconType.Move);
        public SpriteIconGroup Rotate => GetIconItemByType(SpriteIconType.Rotate);
        public SpriteIconGroup Info => GetIconItemByType(SpriteIconType.Info);
        public SpriteIconGroup Reset => GetIconItemByType(SpriteIconType.Reset);
        public SpriteIconGroup Scan => GetIconItemByType(SpriteIconType.Scan);

        public SpriteButtonGroup RoundButton => GetButtonItemByType(SpriteButtonType.Round);
        public SpriteButtonGroup SquareButton => GetButtonItemByType(SpriteButtonType.Square);
        
        // Method to retrieve an icon by its name
        public SpriteIconGroup GetIconItemByType(SpriteIconType spriteType)
        {
            foreach (SpriteIconGroup item in spriteIconItems) 
            { 
                if (item.spriteType == spriteType)
                    return item; 
            }
            Debug.LogError($"Icon type {spriteType} does not exist"); 
            return null;
        }

        // Method to retrieve an button by its name
        public SpriteButtonGroup GetButtonItemByType(SpriteButtonType spriteType)
        {
            foreach (SpriteButtonGroup item in spriteButtonItems) 
            { 
                if (item.spriteType == spriteType)
                    return item;
            }
            Debug.LogError($"Button type {spriteType} does not exist"); 
            return null;
        }
    }
}
