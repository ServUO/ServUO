using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class OrcChopper : BaseCreature
    {
        [Constructable]
        public OrcChopper()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an orc chopper";
            Body = 7;
            BaseSoundID = 0x45A;
            Hue = 0x96D;

            SetStr(147, 245);
            SetDex(91, 115);
            SetInt(61, 85);

            SetHits(97, 139);

            SetDamage(4, 13);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 25, 30);

            SetSkill(SkillName.MagicResist, 60.1, 85.0);
            SetSkill(SkillName.Tactics, 75.1, 90.0);
            SetSkill(SkillName.Wrestling, 60.1, 85.0);

            Fame = 4500;
            Karma = -4500;

            SetWeaponAbility(WeaponAbility.WhirlwindAttack);
            SetWeaponAbility(WeaponAbility.CrushingBlow);
        }

        public OrcChopper(Serial serial)
            : base(serial)
        {
        }

        public override InhumanSpeech SpeechType => InhumanSpeech.Orc;
        public override bool CanRummageCorpses => true;
        public override int Meat => 1;

        public override TribeType Tribe => TribeType.Orc;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager, 2);
            AddLoot(LootPack.LootItem<EvilOrcHelm>(10.0));
            AddLoot(LootPack.LootItem<Yeast>(50.0, true));
            AddLoot(LootPack.LootItem<Log>(1, 10, true));
            AddLoot(LootPack.LootItem<Board>(10, 20, true));
            AddLoot(LootPack.LootItem<ExecutionersAxe>());
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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
