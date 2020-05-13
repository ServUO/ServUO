using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Miasma corpse")]
    public class Miasma : Scorpion
    {
        [Constructable]
        public Miasma()
        {

            Name = "Miasma";
            Hue = 0x8FD;

            SetStr(255, 847);
            SetDex(145, 428);
            SetInt(26, 380);

            SetHits(272, 2000);
            SetMana(5, 60);

            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 50, 54);
            SetResistance(ResistanceType.Fire, 40, 45);
            SetResistance(ResistanceType.Cold, 50, 55);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 40, 45);

            SetSkill(SkillName.Wrestling, 64.9, 73.3);
            SetSkill(SkillName.Tactics, 98.4, 110.6);
            SetSkill(SkillName.MagicResist, 74.4, 77.7);
            SetSkill(SkillName.Poisoning, 128.5, 143.6);

            Fame = 21000;
            Karma = -21000;

            Tamable = false;

            SetWeaponAbility(WeaponAbility.MortalStrike);

            ForceActiveSpeed = 0.3;
            ForcePassiveSpeed = 0.6;
        }

        public Miasma(Serial serial)
            : base(serial)
        {
        }
        public override bool CanBeParagon => false;

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Paragon.ChestChance > Utility.RandomDouble())
                c.DropItem(new ParagonChest(Name, 5));
        }

        public override bool GivesMLMinorArtifact => true;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.ArcanistScrolls);
            AddLoot(LootPack.RandomLootItem(new System.Type[]
                {
                    typeof(MyrmidonGloves),typeof(MyrmidonGorget),typeof(MyrmidonLegs),
                    typeof(MyrmidonArms),typeof(PaladinArms),typeof(PaladinGorget),
                    typeof(LeafweaveLegs),typeof(DeathChest),typeof(DeathGloves),
                    typeof(DeathLegs),typeof(GreymistGloves),typeof(GreymistArms),
                    typeof(AssassinChest),typeof(AssassinArms),typeof(HunterGloves),
                    typeof(HunterLegs),typeof(GreymistLegs),typeof(MyrmidonChest)

                }, 2.5, 1, false, false));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
