using Server;
using System;
using Server.Items;

namespace Server.Mobiles
{
    public class SoulboundPirateRaider : BaseCreature
    {
        public override bool ClickTitle { get { return false; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override WeaponAbility GetWeaponAbility()
        {
            Item weapon = FindItemOnLayer(Layer.TwoHanded);

            if (weapon == null)
                return null;

            if (weapon is BaseWeapon)
            {
                if (Utility.RandomBool())
                    return ((BaseWeapon)weapon).PrimaryAbility;
                else
                    return ((BaseWeapon)weapon).SecondaryAbility;
            }
            return null;
        }

        [Constructable]
        public SoulboundPirateRaider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a soulbound pirate raider";
            Body = 0x190;
            Hue = Utility.RandomSkinHue();
            Utility.AssignRandomHair(this);

            SetStr(150, 200);
            SetDex(125, 150);
            SetInt(95, 110);

            SetHits(200, 250);

            SetDamage(15, 25);

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

            Fame = 2000;
            Karma = -2000;

            AddItem(new TricorneHat());
            AddItem(new LeatherArms());
            AddItem(new FancyShirt());
            AddItem(new ShortPants());
            AddItem(new Cutlass());
            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new GoldEarrings());

            Item bow;

            switch (Utility.Random(4))
            {
                default:
                case 0: bow = new CompositeBow(); PackItem(new Arrow(25)); break;
                case 1: bow = new Crossbow(); PackItem(new Bolt(25)); break;
                case 2: bow = new Bow(); PackItem(new Arrow(25)); break;
                case 3: bow = new HeavyCrossbow(); PackItem(new Bolt(25)); break;
            }

            AddItem(bow);
        }


        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public SoulboundPirateRaider(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}