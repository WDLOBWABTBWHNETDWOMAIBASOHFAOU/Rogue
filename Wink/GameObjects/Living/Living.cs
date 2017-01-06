﻿using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public abstract partial class Living : AnimatedGameObject
    {
        private int timeleft;
        private bool startTimer;
        public bool isTurn;

        protected string idleAnimation, moveAnimation, dieAnimation;
        private string dieSound;

        private readonly GameObjectGrid itemGrid;
        public GameObjectGrid ItemGrid
        {
            get { return itemGrid; }
        }

        private readonly GameObjectList equipmentSlots;
        public GameObjectList EquipmentSlots
        {
            get { return equipmentSlots; }
        }
        EquipmentSlot weapon;
        EquipmentSlot body;
        EquipmentSlot ring1;
        EquipmentSlot ring2;

        public Living(int layer = 0, string id = "", float scale = 1.0f) : base(layer, id, scale)
        {
            SetStats();
            InitAnimation();
            timeleft = 1000;

            itemGrid = new GameObjectGrid(3, 6, 0, "");
            ring1 = new EquipmentSlot(typeof(RingEquipment), id: "ringSlot1");
            ring2 = new EquipmentSlot(typeof(RingEquipment), id: "ringSlot2");
            weapon = new EquipmentSlot(typeof(WeaponEquipment), id: "weaponSlot");
            body = new EquipmentSlot(typeof(BodyEquipment), id: "bodySlot");
            equipmentSlots = new GameObjectList();
            equipmentSlots.Add(weapon);
            equipmentSlots.Add(body);
            equipmentSlots.Add(ring1);
            equipmentSlots.Add(ring2);
        }

        public Living(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            timeleft = info.GetInt32("timeleft");
            startTimer = info.GetBoolean("startTimer");
            isTurn = info.GetBoolean("isTurn");

            idleAnimation = info.GetString("idleAnimation");
            moveAnimation = info.GetString("moveAnimation");
            dieAnimation = info.GetString("dieAnimation");
            dieSound = info.GetString("dieSound");

            manaPoints = info.GetInt32("manaPoints");
            healthPoints = info.GetInt32("healthPoints");
            actionPoints = info.GetInt32("actionPoints");
            baseAttack = info.GetInt32("baseAttack");
            strength = info.GetInt32("strength");
            dexterity = info.GetInt32("dexterity");
            intelligence = info.GetInt32("intelligence");
            creatureLevel = info.GetInt32("creatureLevel");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("timeleft", timeleft);
            info.AddValue("startTimer", startTimer);
            info.AddValue("isTurn", isTurn);

            info.AddValue("idleAnimation", idleAnimation);
            info.AddValue("moveAnimation", moveAnimation);
            info.AddValue("dieAnimation", dieAnimation);
            info.AddValue("dieSound", dieSound);
            
            info.AddValue("manaPoints", manaPoints);
            info.AddValue("healthPoints", healthPoints);
            info.AddValue("actionPoints", actionPoints);
            info.AddValue("baseAttack", baseAttack);
            info.AddValue("strength", strength);
            info.AddValue("dexterity", dexterity);
            info.AddValue("intelligence", intelligence);
            info.AddValue("creatureLevel", creatureLevel);
        }

        protected virtual void InitAnimation(string idleColor = "empty:65:65:10:Magenta")
        {
            //General animations
            idleAnimation = idleColor;
            moveAnimation = "empty:65:65:10:DarkBlue";
            dieAnimation = "empty:65:65:10:LightBlue";
            LoadAnimation(idleAnimation, "idle", true);
            LoadAnimation(moveAnimation, "move", true, 0.05f);
            LoadAnimation(dieAnimation, "die", false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(healthPoints <= 0)
            {
                startTimer = true;
                DeathFeedback("die", dieSound);
                if (startTimer)
                {
                    if (timeleft <= 0)
                        Death();
                    else
                        timeleft -= gameTime.TotalGameTime.Seconds;
                }
            }
        }
        
        public void MoveTo(Tile tile)
        {
            float TileX = (tile.TilePosition.X + 1) * Tile.TileWidth;
            float TileY = (tile.TilePosition.Y + 1) * Tile.TileHeight;

            if (position.X - TileX <= Tile.TileWidth && position.X - TileX >= -Tile.TileWidth*2)
            {
                if (position.Y - TileY <= Tile.TileHeight && position.Y - TileY >= -Tile.TileHeight)
                {
                    position.X = TileX - 0.5f * Tile.TileWidth;
                    position.Y = TileY;
                }
            }
        }
    }
}
