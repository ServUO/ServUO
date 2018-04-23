using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Leprechaun corpse")]
    public class Leprechaun : BaseCreature
    {
        [Constructable]
        public Leprechaun()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "the leprechaun";
            Body = 400;
            Hue = 33770;

            AddItem(new LeprechaunCloak());
            AddItem(new LeprechaunPants());
            AddItem(new LeprechaunBoots());
            AddItem(new LeprechaunShirt());
            AddItem(new LeprechaunHat1());
            AddItem(new GreenGlass());

            SetStr(170, 205);
            SetDex(191, 215);
            SetInt(126, 150);

            SetHits(150, 250);

            SetDamage(24, 30);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 80.2, 100.0);
            SetSkill(SkillName.Magery, 95.1, 100.0);
            SetSkill(SkillName.Meditation, 27.5, 50.0);
            SetSkill(SkillName.MagicResist, 77.5, 100.0);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Wrestling, 20.3, 80.0);

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 20;

            Container pack = new Backpack();
            pack.Movable = false;
            AddItem(pack);
        }

        public override bool BleedImmune { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override int Meat { get { return 1; } }

        public Leprechaun(Serial serial)
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