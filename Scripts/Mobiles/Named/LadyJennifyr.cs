using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Lady Jennifyr corpse")]
    public class LadyJennifyr : SkeletalKnight
    {
        private static readonly Dictionary<Mobile, ExpireTimer> m_Table = new Dictionary<Mobile, ExpireTimer>();
        [Constructable]
        public LadyJennifyr()
        {
            this.Name = "Lady Jennifyr";
            this.Hue = 0x76D;

            this.SetStr(208, 309);
            this.SetDex(91, 118);
            this.SetInt(44, 101);

            this.SetHits(1113, 1285);

            this.SetDamage(15, 25);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Cold, 60);

            this.SetResistance(ResistanceType.Physical, 56, 65);
            this.SetResistance(ResistanceType.Fire, 41, 49);
            this.SetResistance(ResistanceType.Cold, 71, 80);
            this.SetResistance(ResistanceType.Poison, 41, 50);
            this.SetResistance(ResistanceType.Energy, 50, 58);

            this.SetSkill(SkillName.Wrestling, 127.9, 137.1);
            this.SetSkill(SkillName.Tactics, 128.4, 141.9);
            this.SetSkill(SkillName.MagicResist, 102.1, 119.5);
            this.SetSkill(SkillName.Anatomy, 129.0, 137.5);

            this.Fame = 18000;
            this.Karma = -18000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public LadyJennifyr(Serial serial)
            : base(serial)
        {
        }

		public override bool CanBeParagon { get { return false; } }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.15 )
            c.DropItem( new DisintegratingThesisNotes() );

            if ( Utility.RandomDouble() < 0.1 )
            c.DropItem( new ParrotItem() );
        }
     
        /*public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }*/
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.1)
            {
                ExpireTimer timer;

                if (m_Table.TryGetValue(defender, out timer))
                    timer.DoExpire();

                defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                defender.PlaySound(0x208);
                defender.SendLocalizedMessage(1070833); // The creature fans you with fire, reducing your resistance to fire attacks.

                ResistanceMod mod = new ResistanceMod(ResistanceType.Fire, -10);
                defender.AddResistanceMod(mod);

                m_Table[defender] = timer = new ExpireTimer(defender, mod);
                timer.Start();
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

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly ResistanceMod m_Mod;
            public ExpireTimer(Mobile m, ResistanceMod mod)
                : base(TimeSpan.FromSeconds(10))
            {
                this.m_Mobile = m;
                this.m_Mod = mod;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                this.m_Mobile.RemoveResistanceMod(this.m_Mod);

                this.Stop();
                m_Table.Remove(this.m_Mobile);
            }

            protected override void OnTick()
            {
                this.m_Mobile.SendLocalizedMessage(1070834); // Your resistance to fire attacks has returned.
                this.DoExpire();
            }
        }
    }
}