using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a killer pumpkin corpse")]
    public class PumpkinHead : BaseCreature
    {
        [Constructable]
        public PumpkinHead()
            : base(Utility.RandomBool() ? AIType.AI_Melee : AIType.AI_Mage, FightMode.Closest, 10, 1, 0.05, 0.1)
        {
            Name = "a killer pumpkin";
            Body = 1246 + Utility.Random(2);

            BaseSoundID = 268;

            SetStr(350);
            SetDex(125);
            SetInt(250);

            SetHits(500);
            SetMana(1000);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55);
            SetResistance(ResistanceType.Fire, 50);
            SetResistance(ResistanceType.Cold, 50);
            SetResistance(ResistanceType.Poison, 65);
            SetResistance(ResistanceType.Energy, 80);

            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Necromancy, 100.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);
            SetSkill(SkillName.Magery, 160.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 80.0);

            Fame = 5000;
            Karma = -5000;
        }

        public PumpkinHead(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel => true;
        public override bool BardImmune => true;
        public override bool Unprovokable => true;
        public override bool AreaPeaceImmune => true;

        public override void GenerateLoot()
        {
            if (Utility.RandomDouble() < .05)
            {
                switch (Utility.Random(5))
                {
                    case 0:
                        PackItem(new ObsidianSkull());
                        break;
                    case 1:
                        PackItem(new CrystalSkull());
                        break;
                    case 2:
                        PackItem(new JadeSkull());
                        break;
                    case 3:
                        PackItem(new CarvablePumpkinTall());
                        break;
                    case 4:
                        PackItem(new CarvableGordPumpkinTall());
                        break;
                    default:
                        break;
                }
            }

            PackItem(new WrappedCandy());
            AddLoot(LootPack.UltraRich, 2);
        }

        public virtual void Lifted_Callback(Mobile from)
        {
            if (from != null && !from.Deleted && from is PlayerMobile)
            {
                Combatant = from;

                Warmode = true;
            }
        }

        public override Item NewHarmfulItem()
        {
            Item bad = new AcidSlime(TimeSpan.FromSeconds(10), 25, 30);

            bad.Name = "gooey nasty pumpkin hummus";

            bad.Hue = 144;

            return bad;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Utility.RandomBool())
            {
                if (from != null && from.Map != null && Map != Map.Internal && Map == from.Map && from.InRange(this, 12))
                {
                    SpillAcid((willKill) ? this : from, (willKill) ? 3 : 1);
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
