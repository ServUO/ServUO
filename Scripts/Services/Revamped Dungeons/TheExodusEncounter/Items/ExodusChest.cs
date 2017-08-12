using Server.Mobiles;
using Server.Regions;
using System;

namespace Server.Items
{
    public class ExodusChest : DecorativeBox, IRevealableItem
    {
        public static void Initialize()
        {
            TileData.ItemTable[0x2DF3].Flags = TileFlag.None;
        }

        public override int DefaultGumpID { get { return 0x10C; } }

        public static Type[] RituelItem { get { return m_RituelItem; } }

        private static Type[] m_RituelItem = new Type[]
        {
            typeof(ExodusSummoningRite), typeof(ExodusSacrificalDagger), typeof(RobeofRite), typeof(ExodusSummoningAlter)
        };

        private Timer m_Timer;
        private ExodusChestRegion m_Region;

        public override bool IsDecoContainer { get { return false; } }        

        [Constructable]
        public ExodusChest() 
            : base()
        {
            Visible = false;
            Locked = true;
            LockLevel = 100;
            RequiredSkill = 110;
            MaxLockLevel = 130;
            Weight = 0.0;
            Hue = 2700;
            Movable = false;

            TrapType = TrapType.PoisonTrap;
            TrapPower = 100;
            GenerateTreasure();           
        }

        public ExodusChest(Serial serial) : base(serial)
        {
        }

        public bool CheckReveal(Mobile m)
        {
            if (!m.InRange(Location, 3))
                return false;

            return m.Skills[SkillName.DetectHidden].Value >= 98.0;
        }

        public virtual void OnRevealed(Mobile m)
        {
            Visible = true;
            StartDeleteTimer();
        }

        public virtual bool CheckPassiveDetect(Mobile m)
        {
            if (m.InRange(this.Location, 4))
            {
                int skill = (int)m.Skills[SkillName.DetectHidden].Value;

                if (skill >= 80 && Utility.Random(300) < skill)
                    return true;
            }

            return false;
        }

        public void StartDeleteTimer()
        {
            if (Utility.RandomDouble() < 0.2)
            {
                Item item = Activator.CreateInstance(m_RituelItem[Utility.Random(m_RituelItem.Length)]) as Item;
                DropItem(item);
            }

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(5), new TimerCallback(Delete));
            m_Timer.Start();
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (Deleted)
                return;

            UpdateRegion();
        }

        public override void OnMapChange()
        {
            if (Deleted)
                return;           

            UpdateRegion();
        }

        public void UpdateRegion()
        {
            if (m_Region != null)
                m_Region.Unregister();

            if (!Deleted && Map != Map.Internal)
            {
                m_Region = new ExodusChestRegion(this);
                m_Region.Register();
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            base.OnAfterDelete();

            UpdateRegion();
        }

        protected virtual void GenerateTreasure()
        {
            DropItem(new Gold(1500, 3000));           

            Item item = null;

            for (int i = 0 ; i < Loot.GemTypes.Length; i++)
            {               
                item = Activator.CreateInstance(Loot.GemTypes[i]) as Item;
                item.Amount = Utility.Random(1, 6);
                DropItem(item);
            }

            if (0.25 > Utility.RandomDouble())
            {
                item = new SmokeBomb(Utility.Random(3, 6));
                DropItem(item);
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

                DropItem(item);
            }

            if (0.2 > Utility.RandomDouble())
            {
                item = Loot.RandomEssence();
                item.Amount = Utility.Random(3, 6);
                DropItem(item);
            }
            
            if (0.1 > Utility.RandomDouble())
            {
                switch (Utility.Random(4))
                {
                    case 0: DropItem(new Taint()); break;
                    case 1: DropItem(new Corruption()); break;
                    case 2: DropItem(new Blight()); break;
                    case 3: DropItem(new LuminescentFungi()); break;
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

            if (!Locked)
                Delete();

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(UpdateRegion));
        }
    }

    public class ExodusChestRegion : BaseRegion
    {
        private readonly ExodusChest m_Chest;

        public ExodusChest ExodusChest { get { return m_Chest; } }

        public ExodusChestRegion(ExodusChest chest)
            : base(null, chest.Map, Region.Find(chest.Location, chest.Map), new Rectangle2D(chest.Location.X, chest.Location.Y, 5, 5) )
        {
            m_Chest = chest;
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