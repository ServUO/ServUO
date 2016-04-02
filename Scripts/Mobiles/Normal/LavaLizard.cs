using Server.Items;
using Server.Services;

namespace Server.Mobiles
{
    [CorpseName("a lava lizard corpse")]
    [TypeAlias("Server.Mobiles.Lavalizard")]
    public class LavaLizard : BaseCreature
    {
        [Constructable]
        public LavaLizard()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a lava lizard";
            Body = 0xCE;
            Hue = Utility.RandomList(0x647, 0x650, 0x659, 0x662, 0x66B, 0x674);
            BaseSoundID = 0x5A;

            SetStr(126, 150);
            SetDex(56, 75);
            SetInt(11, 20);

            SetHits(76, 90);
            SetMana(0);

            SetDamage(6, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 30, 45);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 55.1, 70.0);
            SetSkill(SkillName.Tactics, 60.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 3000;
            Karma = -3000;

            VirtualArmor = 40;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 80.7;

            QLPoints = 2;

            PackItem(new SulfurousAsh(Utility.Random(4, 10)));
        }

        public LavaLizard(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get { return true; }
        } // fire breath enabled

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
            AddLoot(LootPack.Meager);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            SARegionDrops.GetSADrop(c);
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