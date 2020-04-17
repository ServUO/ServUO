using System;

namespace Server.Items
{
    public class BlackrockAutomaton : KotlAutomaton
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public override Type RepairResource => typeof(CrystallineBlackrock); // TODO: Needs to be regular blackrock. THis doesn't exist on ServUO

        [Constructable]
        public BlackrockAutomaton()
        {
            Name = "blackrock automaton";
            Body = 1406;
            BaseSoundID = 541;

            Hue = 0x8497;

            SetStr(800, 900);
            SetDex(67, 74);
            SetInt(255, 263);

            SetHits(774, 876);

            SetDamage(15, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 60);
            SetResistance(ResistanceType.Fire, 55, 60);
            SetResistance(ResistanceType.Cold, 50, 55);
            SetResistance(ResistanceType.Poison, 50, 55);
            SetResistance(ResistanceType.Energy, 45, 50);

            SetSkill(SkillName.Anatomy, 90.3, 99.9);
            SetSkill(SkillName.MagicResist, 121.0, 126.7);
            SetSkill(SkillName.Tactics, 82.0, 94.8);
            SetSkill(SkillName.Wrestling, 94.4, 108.4);

            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
            SetWeaponAbility(WeaponAbility.Disarm);
            SetWeaponAbility(WeaponAbility.ArmorPierce);
        }

        public override double WeaponAbilityChance => 0.45;

        public override void OnResourceChanged()
        {
        }

        public BlackrockAutomaton(Serial serial)
            : base(serial)
        {
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
