using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a snow elemental corpse")]
    public class SnowElemental : BaseCreature, IAuraCreature
    {
        [Constructable]
        public SnowElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a snow elemental";
            Body = 163;
            BaseSoundID = 263;

            SetStr(326, 355);
            SetDex(166, 185);
            SetInt(71, 95);

            SetHits(196, 213);

            SetDamage(11, 17);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 80);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 10, 15);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 50.1, 65.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);

            Fame = 5000;
            Karma = -5000;

            SetAreaEffect(AreaEffect.AuraDamage);
        }

        public SnowElemental(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 2;

        public void AuraEffect(Mobile m)
        {
            m.FixedParticles(0x374A, 10, 30, 5052, Hue, 0, EffectLayer.Waist);
            m.PlaySound(0x5C6);

            m.SendLocalizedMessage(1008111, false, Name); //  : The intense cold is damaging you!
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.LootItem<BlackPearl>(3, true));
            AddLoot(LootPack.LootItem<IronOre>(3, true));
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
