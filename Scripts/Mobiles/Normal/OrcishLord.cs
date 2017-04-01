using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class OrcishLord : BaseCreature
    {
        [Constructable]
        public OrcishLord()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an orcish lord";
            Body = 138;
            BaseSoundID = 0x45A;

            SetStr(147, 215);
            SetDex(91, 115);
            SetInt(61, 85);

            SetHits(95, 123);

            SetDamage(4, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 70.1, 85.0);
            SetSkill(SkillName.Swords, 60.1, 85.0);
            SetSkill(SkillName.Tactics, 75.1, 90.0);
            SetSkill(SkillName.Wrestling, 60.1, 85.0);

            Fame = 2500;
            Karma = -2500;

            switch ( Utility.Random(5) )
            {
                case 0:
                    PackItem(new Lockpick());
                    break;
                case 1:
                    PackItem(new MortarPestle());
                    break;
                case 2:
                    PackItem(new Bottle());
                    break;
                case 3:
                    PackItem(new RawRibs());
                    break;
                case 4:
                    PackItem(new Shovel());
                    break;
            }

            PackItem(new RingmailChest());

            if (0.3 > Utility.RandomDouble())
                PackItem(Loot.RandomPossibleReagent());

            if (0.2 > Utility.RandomDouble())
                PackItem(new BolaBall());

            if (0.5 > Utility.RandomDouble())
                PackItem(new Yeast());
        }

        public OrcishLord(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses{ get{ return true; } }
        public override int Meat{ get{ return 1; } }
        public override int TreasureMapLevel{ get{ return 1; } }
        public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }
        public override OppositionType OppositionList{ get{ return OppositionType.Orc; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Average);
            // TODO: evil orc helm
        }

        public override bool IsEnemy(Mobile m)
        {
            if (m.Player && m.FindItemOnLayer(Layer.Helm) is OrcishKinMask)
                return false;

            return base.IsEnemy(m);
        }

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            Item item = aggressor.FindItemOnLayer(Layer.Helm);

            if (item is OrcishKinMask)
            {
                AOS.Damage(aggressor, 50, 0, 100, 0, 0, 0);
                item.Delete();
                aggressor.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                aggressor.PlaySound(0x307);
            }
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
