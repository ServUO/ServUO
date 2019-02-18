using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class KhaldunChest : LockableContainer, IRevealableItem
    {
        private Timer m_Timer;

        public override int DefaultGumpID { get { return 0x49; } }
        public bool CheckWhenHidden { get { return true; } }

        [Constructable]
        public KhaldunChest()
            : base(Utility.RandomList(0xE3C, 0xE3E, 0x9a9))
        {
            Movable = false;
            Locked = true;
            Visible = false;

            Hue = 2745;
            LiftOverride = true;
            Weight = 0.0;

            LockLevel = 90;
            RequiredSkill = 90;
            MaxLockLevel = 100;
            
            TrapType = TrapType.PoisonTrap;
            TrapPower = 100;
            Timer.DelayCall(TimeSpan.FromSeconds(1), Fill);
        }

        public virtual void Fill()
        {
            Reset();

            List<Item> contains = new List<Item>(Items);

            foreach (var i in contains)
            {
                i.Delete();
            }

            ColUtility.Free(contains);

            for (int i = 0; i < Utility.RandomMinMax(6, 12); i++)
                DropItem(Loot.RandomGem());

            DropItem(new Gold(Utility.RandomMinMax(800, 1100)));

            Item item = null;

            if (0.30 > Utility.RandomDouble())
            {
                switch (Utility.Random(7))
                {
                    case 0:
                        item = new Bandage(Utility.Random(10, 30)); break;
                    case 1:
                        item = new SmokeBomb();
                        item.Amount = Utility.Random(3, 6);
                        break;
                    case 2:
                        item = new InvisibilityPotion();
                        item.Amount = Utility.Random(1, 3);
                        break;
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

        public void Reset()
        {
            EndTimer();

            Visible = false;
            Locked = true;

            RequiredSkill = 90;
            LockLevel = RequiredSkill - Utility.Random(1, 10);
            MaxLockLevel = RequiredSkill;

            TrapType = TrapType.MagicTrap;
            TrapPower = 100;
        }

        public virtual bool CheckReveal(Mobile m)
        {
            return m.CheckTargetSkill(SkillName.DetectHidden, this, 80.0, 100.0);
        }

        public virtual void OnRevealed(Mobile m)
        {
            Visible = true;
        }

        public virtual bool CheckPassiveDetect(Mobile m)
        {
            if (m.InRange(Location, 4))
            {
                int skill = (int)m.Skills[SkillName.DetectHidden].Value;

                if (skill >= 80 && Utility.Random(300) < skill)
                    return true;
            }

            return false;
        }

        public override void LockPick(Mobile from)
        {
            TryDelayedLock();

            base.LockPick(from);
        }

        public KhaldunChest(Serial serial) : base(serial)
        {
        }        

        public void TryDelayedLock()
        {
            if (Locked || (m_Timer != null && m_Timer.Running))
                return;

            EndTimer();

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(10, 15)), Fill);
        }

        public void EndTimer()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            TryDelayedLock();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            TryDelayedLock();
        }
    }
}
