using Server.Mobiles;
using Server.Regions;
using System;

namespace Server.Items
{
    public class ExodusChest : DecorativeBox
    {
        public static void Initialize()
        {
            TileData.ItemTable[0x2DF3].Flags = TileFlag.None;
        }

        public override int DefaultGumpID { get { return 0x10C; } }

        public static Type[] RituelItem { get { return m_RituelItem; } }

        private static readonly Type[] m_RituelItem =
        {
            typeof(ExodusSummoningRite), typeof(ExodusSacrificalDagger), typeof(RobeofRite), typeof(ExodusSummoningAlter)
        };

        private static Type[] m_ImbuingEssenceIngreds =
        {
            typeof(EssencePrecision), typeof(EssenceAchievement), typeof(EssenceBalance), typeof(EssenceControl), typeof(EssenceDiligence),
            typeof(EssenceDirection),   typeof(EssenceFeeling), typeof(EssenceOrder),   typeof(EssencePassion),   typeof(EssencePersistence),
            typeof(EssenceSingularity)
        };

        private Timer m_Timer;
        private ExodusChestRegion m_Region;

        public override bool IsDecoContainer { get { return false; } }        

        [Constructable]
        public ExodusChest() 
            : base()
        {
            this.Visible = false;
            this.Locked = true;
            this.LockLevel = 100;
            this.RequiredSkill = 110;
            this.MaxLockLevel = 130;
            this.Weight = 0.0;
            this.Hue = 2700;
            this.Movable = false;

            this.TrapType = TrapType.PoisonTrap;
            this.TrapPower = 100;
            this.GenerateTreasure();           
        }

        public ExodusChest(Serial serial) : base(serial)
        {
        }

        public void StartDeleteTimer()
        {
            if (Utility.RandomDouble() < 0.2)
            {
                Item item = Activator.CreateInstance(m_RituelItem[Utility.Random(m_RituelItem.Length)]) as Item;
                this.DropItem(item);
            }

            this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(5), new TimerCallback(this.Delete));
            this.m_Timer.Start();
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (this.Deleted)
                return;

            this.UpdateRegion();
        }

        public override void OnMapChange()
        {
            if (this.Deleted)
                return;           

            this.UpdateRegion();
        }

        public void UpdateRegion()
        {
            if (this.m_Region != null)
                this.m_Region.Unregister();

            if (!this.Deleted && this.Map != Map.Internal)
            {
                this.m_Region = new ExodusChestRegion(this);
                this.m_Region.Register();
            }
        }

        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            base.OnAfterDelete();

            this.UpdateRegion();
        }

        protected virtual void GenerateTreasure()
        {
            this.DropItem(new Gold(1500, 3000));           

            Item item = null;

            for (int i = 0 ; i < Loot.GemTypes.Length; i++)
            {               
                item = Activator.CreateInstance(Loot.GemTypes[i]) as Item;
                item.Amount = Utility.Random(1, 6);
                this.DropItem(item);
            }

            if (0.25 > Utility.RandomDouble())
            {
                item = new SmokeBomb(Utility.Random(3, 6));
                this.DropItem(item);
            }

            if (0.25 > Utility.RandomDouble())
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        item = new ParasiticPotion(Utility.Random(1, 3)); break;
                    case 1:
                        item = new InvisibilityPotion(Utility.Random(1, 3)); break;
                }                        

                this.DropItem(item);
            }

            if (0.2 > Utility.RandomDouble())
            {
                item = Activator.CreateInstance(m_ImbuingEssenceIngreds[Utility.Random(m_ImbuingEssenceIngreds.Length)]) as Item;
                item.Amount = Utility.Random(3, 6);
                this.DropItem(item);
            }
            
            if (0.1 > Utility.RandomDouble())
            {
                switch (Utility.Random(4))
                {
                    case 0: this.DropItem(new Taint()); break;
                    case 1: this.DropItem(new Corruption()); break;
                    case 2: this.DropItem(new Blight()); break;
                    case 3: this.DropItem(new LuminescentFungi()); break;
                }
            }
        }

        public static void GiveRituelItem(Mobile m)
        {
            Item item = Activator.CreateInstance(m_RituelItem[Utility.Random(m_RituelItem.Length)]) as Item;
            m.PlaySound(0x5B4);

            if (item == null)
                return;

            m.AddToBackpack(item);
            m.SendLocalizedMessage(1072223); // An item has been placed in your backpack.           
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (!this.Locked)
                this.Delete();

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(UpdateRegion));
        }
    }

    public class ExodusChestRegion : BaseRegion
    {
        private readonly ExodusChest m_Chest;

        public ExodusChest ExodusChest { get { return this.m_Chest; } }

        public ExodusChestRegion(ExodusChest chest)
            : base(null, chest.Map, Region.Find(chest.Location, chest.Map), new Rectangle2D(chest.Location.X, chest.Location.Y, 5, 5) )
        {
            this.m_Chest = chest;
        }

        public override void OnEnter(Mobile m)
        {
            if (!m_Chest.Visible && m is PlayerMobile && m.Skills[SkillName.DetectHidden].Value >= 98.0)
            {
                m.SendLocalizedMessage(1153493); // Your keen senses detect something hidden in the area...
            }
        }
    }
}