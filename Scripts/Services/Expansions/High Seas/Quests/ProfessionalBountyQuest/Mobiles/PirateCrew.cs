using Server;
using System;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;
using Server.Engines.Quests;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class PirateCrew : BaseCreature
    {
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

        public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }
        
        [Constructable]
        public PirateCrew() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = NameList.RandomName("orc");
            Body = Utility.RandomList(17, 182, 7, 138, 140);

            bool mage = 0.33 > Utility.RandomDouble();

            SetStr(100, 125);
            SetDex(125, 150);
            SetInt(250, 400);
            SetHits(250, 400);
            SetDamage(15, 25);

            if (mage)
            {
                ChangeAIType(AIType.AI_Mage);
                SetSkill(SkillName.Magery, 100.0, 120.0);
                SetSkill(SkillName.EvalInt, 100.0, 120.0);
                SetSkill(SkillName.Meditation, 100.0, 120.0);
                SetSkill(SkillName.MagicResist, 100.0, 120.0);
            }

            SetSkill(SkillName.Archery, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);
            SetSkill(SkillName.Anatomy, 100.0, 120.0);

            SetDamageType(ResistanceType.Physical, 70);
            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 45, 55);

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

            Fame = 8000;
            Karma = -8000;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
        }

        public PirateCrew(Serial serial)
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