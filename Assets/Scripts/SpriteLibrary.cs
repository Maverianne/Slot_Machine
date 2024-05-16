using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteLibrary", menuName = "ScriptableObjects/SpriteLibrary", order = 1)]

public class SpriteLibrary : ScriptableObject
{
    [SerializeField] private SpriteData[] spriteLibrary;
    [SerializeField] private Sprite defaultSpr;
    
    
    public Sprite GetSprite(string spriteTag)
    {
        foreach (var data in spriteLibrary)
        {
            if(data.tag != spriteTag) continue;
            return data.sprite;
        }

        return defaultSpr;
    }
    [Serializable]
    public struct SpriteData
    {
        public string tag;
        public Sprite sprite;
    }
}