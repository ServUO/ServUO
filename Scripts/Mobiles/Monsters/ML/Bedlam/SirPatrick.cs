using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Sir Patrick corpse")]
    public class SirPatrick : SkeletalKnight
    {
        [Constructable]
        public SirPatrick()
        {

            this.Name = "Sir Patrick";
            this.Hue = 0x47E;

            this.SetStr(208, 319);
            this.SetDex(98, 132);
            this.SetInt(45, 91);

            this.SetHits(616, 884);

            this.SetDamage(15, 25);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Cold, 60);

            this.SetResistance(ResistanceType.Physical, 55, 62);
            this.SetResistance(ResistanceType.Fire, 40, 48);
            this.SetResistance(ResistanceType.Cold, 71, 80);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.Wrestling, 126.3, 136.5);
            this.SetSkill(SkillName.Tactics, 128.5, 143.8);
            this.SetSkill(SkillName.MagicResist, 102.8, 117.9);
            this.SetSkill(SkillName.Anatomy, 127.5, 137.2);

            this.Fame = 18000;
            this.Karma = -18000;
        }

        public SirPatrick(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.15 )
            c.DropItem( new DisintegratingThesisNotes() );

            if ( Utility.RandomDouble() < 0.05 )
            c.DropItem( new AssassinChest() );
        }

        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.1)
                this.DrainLife();
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Utility.RandomDouble() < 0.1)
                this.DrainLife();
        }

        public virtual void DrainLife()
        {
            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in this.GetMobilesInRange(2))
            {
                if (m == this || !this.CanBeHarmful(m, false) || (Core.AOS && !this.InLOS(m)))
                    continue;

                if (m is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)m;

                    if (bc.Controlled || bc.Summoned || bc.Team != this.Team)
                        list.Add(m);
                }
                else if (m.Player)
                {
                    list.Add(m);
                }
            }

            foreach (Mobile m in list)
            {
                this.DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x455, 0, EffectLayer.Waist);
                m.PlaySound(0x1EA);

                int drain = Utility.RandomMinMax(14, 30);

                this.Hits += drain;
                m.Damage(drain, this);
            }
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