using System;

namespace Server.Mobiles
{
    [CorpseName("a corporeal brume corpse")]
    public class CorporealBrume : BaseCreature
    {
        [Constructable]
        public CorporealBrume()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a corporeal brume";
            this.Body = 0x104; // TODO: Verify
            this.BaseSoundID = 0x56B;

            this.SetStr(400, 450);
            this.SetDex(100, 150);
            this.SetInt(50, 60);

            this.SetHits(1150, 1250);

            this.SetDamage(21, 25);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 100);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Wrestling, 110.0, 115.0);
            this.SetSkill(SkillName.Tactics, 110.0, 115.0);
            this.SetSkill(SkillName.MagicResist, 80.0, 95.0);
            this.SetSkill(SkillName.Anatomy, 100.0, 110.0);

            this.Fame = 12000;
            this.Karma = -12000;
        }

        public CorporealBrume(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        // TODO: Verify area attack specifics
        public override bool HasAura
        {
            get
            {
                return (this.Combatant != null);
            }
        }
        public override TimeSpan AuraInterval
        {
            get
            {
                return TimeSpan.FromSeconds(20);
            }
        }
        public override int AuraRange
        {
            get
            {
                return 10;
            }
        }
        public override int AuraBaseDamage
        {
            get
            {
                return Utility.RandomMinMax(25, 35);
            }
        }
        public override int AuraFireDamage
        {
            get
            {
                return 0;
            }
        }
        public override int AuraColdDamage
        {
            get
            {
                return 100;
            }
        }

        public override void AuraEffect(Mobile m)
        {
            m.FixedParticles(0x374A, 10, 15, 5038, 1181, 2, EffectLayer.Head);
            m.PlaySound(0x213);

            m.SendLocalizedMessage(1008111, false, this.Name); //  : The intense cold is damaging you!
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}