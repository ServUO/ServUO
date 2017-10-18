using System;
using Server;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class KotlRegalChest : LockableContainer, IRevealableItem
    {
        [Constructable]
        public KotlRegalChest()
            : base(0x4D0C)
        {
            Movable = false;
            Locked = true;

            Hue = 2591;
            LiftOverride = true;

            RequiredSkill = 90;
            LockLevel =  RequiredSkill - Utility.Random(1, 10);
            MaxLockLevel = RequiredSkill;
            TrapType = TrapType.MagicTrap;
            TrapPower = 100;
            
            Timer.DelayCall(TimeSpan.FromSeconds(1), Fill);
        }

        public virtual void Fill()
        {
            Visible = false;
            Locked = true;
            TrapType = TrapType.MagicTrap;
            TrapPower = 100;

            List<Item> contains = new List<Item>(this.Items);

            foreach (var item in contains)
            {
                item.Delete();
            }

            ColUtility.Free(contains);

            for(int i = 0; i < Utility.RandomMinMax(6, 12); i++)
                DropItem(Loot.RandomGem());

            DropItem(new Gold(Utility.RandomMinMax(800, 1100)));

            if (0.1 > Utility.RandomDouble())
            {
                DropItem(new StasisChamberPowerCore());
            }

            if (0.1 > Utility.RandomDouble())
            {
                DropItem(new CardOfSemidar((CardOfSemidar.CardType)Utility.RandomMinMax(0, 5)));
            }

            if (0.25 > Utility.RandomDouble())
            {
                DropItem(new InoperativeAutomatonHead());
            }

            if (0.1 > Utility.RandomDouble() && Server.Engines.Points.PointsSystem.TreasuresOfKotlCity.Enabled)
            {
                Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(this), LootPackEntry.IsMondain(this), LootPackEntry.IsStygian(this));

                if (item != null)
                {
                    int min, max;

                    TreasureMapChest.GetRandomItemStat(out min, out max, 1.0);

                    RunicReforging.GenerateRandomItem(item, null, Utility.RandomMinMax(min, max), 0, ReforgedPrefix.None, ReforgedSuffix.Kotl, this.Map);

                    DropItem(item);
                }
            }

            if (0.25 > Utility.RandomDouble())
            {
                Item item;

                switch (Utility.Random(8))
                {
                    default:
                    case 0: item = new JournalDrSpector1(); break;
                    case 1: item = new JournalDrSpector2(); break;
                    case 2: item = new JournalDrSpector3(); break;
                    case 3: item = new JournalDrSpector4(); break;
                    case 4: item = new HistoryOfTheGreatWok1(); break;
                    case 5: item = new HistoryOfTheGreatWok2(); break;
                    case 6: item = new HistoryOfTheGreatWok3(); break;
                    case 7: item = new HistoryOfTheGreatWok4(); break;
                }

                DropItem(item);
            }
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
            if (m.InRange(this.Location, 4))
            {
                int skill = (int)m.Skills[SkillName.DetectHidden].Value;

                if (skill >= 80 && Utility.Random(300) < skill)
                    return true;
            }

            return false;
        }

        public override void LockPick(Mobile from)
        {
            Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(4, 6)), Fill);

            base.LockPick(from);
        }

        public KotlRegalChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if(!Locked)
                Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(4, 6)), Fill);
        }
    }
}