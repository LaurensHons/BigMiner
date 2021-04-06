
using System;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
   public static ItemAssets Instance { get; private set; }

   private void Awake()
   {
      Instance = this;
   }

   public Sprite DirtBlockSprite;
   public Sprite StoneBlockSprite;

   public Sprite PickaxeSprite;
   public Sprite HammerSprite;
   
}
