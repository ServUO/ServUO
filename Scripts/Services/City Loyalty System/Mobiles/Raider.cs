using Server.Items;
using Server.Network;
using System;
using System.Linq;

namespace Server.Mobiles
{
    public class Raider : BaseCreature
    {
        public DateTime TimeToDelete { get; set; }

        public override bool Commandable => false;
        public override bool ReduceSpeedWithDamage => false;
        public override bool AlwaysMurderer => true;

        [Constructable]
        public Raider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Title = "the raider";
            Hue = Utility.RandomSkinHue();
            Utility.AssignRandomHair(this);

            SetStr(150, 200);
            SetDex(125, 150);
            SetInt(95, 110);

            SetHits(400, 650);
            SetDamage(21, 28);

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                EquipItem(new Skirt(Utility.RandomNeutralHue()));
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                EquipItem(new ShortPants(Utility.RandomNeutralHue()));
            }

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.MagicResist, 50.0, 75.5);
            SetSkill(SkillName.Archery, 90.0, 105.5);
            SetSkill(SkillName.Tactics, 90.0, 105.5);
            SetSkill(SkillName.Anatomy, 90.0, 105.5);

            Fame = 7500;
            Karma = -7500;

            SetWearable(new TricorneHat());
            SetWearable(new LeatherArms());
            SetWearable(new FancyShirt());
            SetWearable(new Cutlass());
            SetWearable(new Boots(Utility.RandomNeutralHue()));
            SetWearable(new GoldEarrings());

            Item bow;

            switch (Utility.Random(4))
            {
                default:
                case 0: bow = new CompositeBow(); break;
                case 1: bow = new Crossbow(); break;
                case 2: bow = new Bow(); break;
                case 3: bow = new HeavyCrossbow(); break;
            }

            SetWearable(bow);

            ControlSlots = 0;
        }

        public bool TryArrest(Mobile m)
        {
            if (ControlMaster != null)
            {
                m.SendLocalizedMessage(1152247); // That person is already under arrest.
            }
            else if (m is PlayerMobile && ((PlayerMobile)m).AllFollowers.FirstOrDefault(mob => mob is Raider) != null)
            {
                m.SendLocalizedMessage(1152249); // You already have a prisoner.
            }
            else if (Hits > ((double)HitsMax / 10))
            {
                m.SendLocalizedMessage(1152229); // That person won't sit still for it! A more aggressive approach is in order.
                m.NonlocalOverheadMessage(MessageType.Regular, 0x3B2, 1152237, string.Format("{0}\t{1}", m.Name, Name, "raider"));
            }
            else
            {
                TimeToDelete = DateTime.UtcNow + TimeSpan.FromHours(1);

                SetControlMaster(m);
                IsBonded = false;
                ControlTarget = m;
                ControlOrder = OrderType.Follow;

                m.SendLocalizedMessage(1152236, Name); // You arrest the ~1_name~. Take the criminal to the guard captain.
                m.NonlocalOverheadMessage(MessageType.Regular, 0x3B2, 1152238, string.Format("{0}\t{1}", m.Name, Name));

                return true;
            }

            return false;
        }

        public override void OnThink()
        {
            base.OnThink();

            CheckDelete();
        }

        public void CheckDelete()
        {
            if (TimeToDelete != DateTime.MinValue && DateTime.UtcNow > TimeToDelete)
            {
                if (ControlMaster != null && ControlMaster.NetState != null)
                {
                    ControlMaster.SendLocalizedMessage(1152248); // You did not take your prisoner to the Guard Captain in time.
                }

                Delete();
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.LootItem<Arrow>(25, true));
            AddLoot(LootPack.LootItem<Bolt>(25, true));
        }

        public Raider(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            Timer.DelayCall(TimeSpan.FromSeconds(10), CheckDelete);

            writer.Write(TimeToDelete);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            TimeToDelete = reader.ReadDateTime();
        }
    }
}
