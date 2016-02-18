using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a slith corpse")]
    public class StoneSlith : BaseCreature
    {
        public static Type[] VArtifacts =
        {
            typeof (StoneSlithClaw)
        };

        [Constructable]
        public StoneSlith()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a stone slith";
            Body = 734;

            SetStr(250, 300);
            SetDex(76, 90);
            SetInt(34, 69);

            SetHits(154, 166);
            SetStam(76, 90);
            SetMana(34, 69);

            SetDamage(6, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 55);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 86.8, 95.1);
            SetSkill(SkillName.Tactics, 82.6, 88.6);
            SetSkill(SkillName.Wrestling, 75.8, 87.4);
            SetSkill(SkillName.Anatomy, 0.0, 2.9);

            PackItem(new DragonBlood(6));

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 65.1;

            QLPoints = 20;
        }

        public StoneSlith(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get { return true; }
        } // fire breath enabled

        public override int Meat
        {
            get { return 1; }
        }

        //public override int DragonBlood{ get{ return 6; } }
        public override int Hides
        {
            get { return 12; }
        }

        public override HideType HideType
        {
            get { return HideType.Spined; }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
            //return WeaponAbility.LowerPhysicalResist;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.05)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        c.DropItem(new SlithTongue());
                        break;
                    case 1:
                        c.DropItem(new SlithEye());
                        break;
                }
            }

            if (Utility.RandomDouble() < 0.25)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        c.DropItem(new AncientPotteryFragments());
                        break;
                    case 1:
                        c.DropItem(new TatteredAncientScroll());
                        break;
                }
            }

            if (c != null && !c.Deleted && c is Corpse)
            {
                var corpse = (Corpse) c;
                if (Utility.RandomDouble() < 0.01 && corpse.Killer != null && !corpse.Killer.Deleted)
                {
                    GiveVArtifactTo(corpse.Killer);
                }
            }
        }

        public static void GiveVArtifactTo(Mobile m)
        {
            var item = (Item) Activator.CreateInstance(VArtifacts[Utility.Random(VArtifacts.Length)]);

            if (m.AddToBackpack(item))
                m.SendLocalizedMessage(1062317);
                    // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
            else
                m.SendMessage("As your backpack is full, your reward has been placed at your feet.");
            {
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
        }
    }
}