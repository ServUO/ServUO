using Server.Mobiles;
using Server.Regions;
using System;

namespace Server.Items
{
    public class KhaldunChest : DecorativeBox, IRevealableItem
    {
        public static void Initialize()
        {
            TileData.ItemTable[0x2DF3].Flags = TileFlag.None;
        }

        public override int DefaultGumpID { get { return 0x10C; } }

        private Timer m_Timer;
        private KhaldunChestRegion m_Region;

        public override bool IsDecoContainer { get { return false; } }

        [Constructable]
        public KhaldunChest()
            : base()
        {
            Visible = false;
            Locked = true;
            LockLevel = 90;
            RequiredSkill = 90;
            MaxLockLevel = 100;
            Weight = 0.0;
            Hue = 2745;
            Movable = false;

            TrapType = TrapType.PoisonTrap;
            TrapPower = 100;
            GenerateTreasure();
        }

        public KhaldunChest(Serial serial) : base(serial)
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
                m_Region = new KhaldunChestRegion(this);
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

            for (int i = 0; i < Loot.GemTypes.Length; i++)
            {
                item = Activator.CreateInstance(Loot.GemTypes[i]) as Item;
                item.Amount = Utility.Random(1, 6);
                DropItem(item);
            }

            if (0.30 > Utility.RandomDouble())
            {
                switch (Utility.Random(4))
                {
                    case 0:
                        item = new Bandage(Utility.Random(10, 30)); break;
                    case 1:
                        item = new SmokeBomb(Utility.Random(3, 6)); break;
                    case 2:
                        item = new InvisibilityPotion(Utility.Random(1, 3)); break;
                    case 3:
                        item = new Lockpick(Utility.Random(1, 10)); break;
                    case 4:
                        item = new DreadHornMane(Utility.Random(1, 2)); break;
                    case 5:
                        item = new Corruption(Utility.Random(1, 2)); break;
                    case 6:
                        item = new Taint(Utility.Random(1, 2)); break;
                }

                DropItem(item);
            }

            if (0.25 > Utility.RandomDouble())
            {
                DropItem(new CounterfeitPlatinum());
            }

            if (0.2 > Utility.RandomDouble())
            {
                switch (Utility.Random(3))
                {
                    case 0:
                        item = new ZombiePainting(); break;
                    case 1:
                        item = new SkeletonPortrait(); break;
                    case 2:
                        item = new LichPainting(); break;
                }

                DropItem(item);
            }

            if (0.1 > Utility.RandomDouble())
            {
                item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(this), LootPackEntry.IsMondain(this), LootPackEntry.IsStygian(this));

                if (item != null)
                {
                    int min, max;

                    TreasureMapChest.GetRandomItemStat(out min, out max, 1.0);

                    RunicReforging.GenerateRandomItem(item, null, Utility.RandomMinMax(min, max), 0, ReforgedPrefix.None, ReforgedSuffix.Khaldun, Map);

                    DropItem(item);
                }
            }
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

    public class KhaldunChestRegion : BaseRegion
    {
        public KhaldunChest KhaldunChest { get; set; }

        public KhaldunChestRegion(KhaldunChest chest)
            : base(null, chest.Map, Region.Find(chest.Location, chest.Map), new Rectangle2D(chest.Location.X - 2, chest.Location.Y - 2, 5, 5))
        {
            KhaldunChest = chest;
        }

        public override void OnEnter(Mobile m)
        {
            if (!KhaldunChest.Visible && m is PlayerMobile && m.Skills[SkillName.DetectHidden].Value >= 98.0)
            {
                m.SendLocalizedMessage(1153493); // Your keen senses detect something hidden in the area...
            }
        }
    }
}
