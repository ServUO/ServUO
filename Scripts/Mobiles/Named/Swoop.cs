using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Swoop corpse")]
    public class Swoop : Eagle
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public Swoop()
        {

            this.Name = "Swoop";
            this.Hue = 0xE0;

            this.AI = AIType.AI_Melee;

            this.SetStr(100, 150);
            this.SetDex(400, 500);
            this.SetInt(80, 90);

            this.SetHits(1500, 2000);

            this.SetDamage(20, 30);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 75, 90);
            this.SetResistance(ResistanceType.Fire, 60, 77);
            this.SetResistance(ResistanceType.Cold, 70, 85);
            this.SetResistance(ResistanceType.Poison, 55, 85);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.Wrestling, 120.0, 140.0);
            this.SetSkill(SkillName.Tactics, 120.0, 140.0);
            this.SetSkill(SkillName.MagicResist, 95.0, 105.0);

            this.Fame = 18000;
            this.Karma = 0;

            this.PackReg(4);

            Tamable = false;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

		public override bool CanBeParagon { get { return false; } }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.025 )
            {
                switch ( Utility.Random( 18 ) )
                {
                    case 0: c.DropItem( new AssassinChest() ); break;
                    case 1: c.DropItem( new AssassinArms() ); break;
                    case 2: c.DropItem( new DeathChest() ); break;
                    case 3: c.DropItem( new MyrmidonArms() ); break;
                    case 4: c.DropItem( new MyrmidonLegs() ); break;
                    case 5: c.DropItem( new MyrmidonGorget() ); break;
                    case 6: c.DropItem( new LeafweaveGloves() ); break;
                    case 7: c.DropItem( new LeafweaveLegs() ); break;
                    case 8: c.DropItem( new LeafweavePauldrons() ); break;
                    case 9: c.DropItem( new PaladinGloves() ); break;
                    case 10: c.DropItem( new PaladinGorget() ); break;
                    case 11: c.DropItem( new PaladinArms() ); break;
                    case 12: c.DropItem( new HunterArms() ); break;
                    case 13: c.DropItem( new HunterGloves() ); break;
                    case 14: c.DropItem( new HunterLegs() ); break;
                    case 15: c.DropItem( new HunterChest() ); break;
                    case 16: c.DropItem( new GreymistArms() ); break;
                    case 17: c.DropItem( new GreymistGloves() ); break;
                }
            }

            if ( Utility.RandomDouble() < 0.1 )
            c.DropItem( new ParrotItem() );
        }
        
        public Swoop(Serial serial)
            : base(serial)
        {
        }

        public override bool CanFly
        {
            get
            {
                return true;
            }
        }
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override int Feathers
        {
            get
            {
                return 72;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.HighScrolls);
        }

        // TODO: Put this attack shared with Hiryu and Lesser Hiryu in one place
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble())
            {
                ExpireTimer timer = (ExpireTimer)m_Table[defender];

                if (timer != null)
                {
                    timer.DoExpire();
                    defender.SendLocalizedMessage(1070837); // The creature lands another blow in your weakened state.
                }
                else
                    defender.SendLocalizedMessage(1070836); // The blow from the creature's claws has made you more susceptible to physical attacks.

                int effect = -(defender.PhysicalResistance * 15 / 100);

                ResistanceMod mod = new ResistanceMod(ResistanceType.Physical, effect);

                defender.FixedEffect(0x37B9, 10, 5);
                defender.AddResistanceMod(mod);

                timer = new ExpireTimer(defender, mod, TimeSpan.FromSeconds(5.0));
                timer.Start();
                m_Table[defender] = timer;
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
            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay)
                : base(delay)
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
                this.m_Mobile.SendLocalizedMessage(1070838); // Your resistance to physical attacks has returned.
                this.DoExpire();
            }
        }
    }
}