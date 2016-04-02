using System;
using System.Collections;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a fan dancer corpse")]
    public class FanDancer : BaseCreature
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public FanDancer()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a fan dancer";
            this.Body = 247;
            this.BaseSoundID = 0x372;

            this.SetStr(301, 375);
            this.SetDex(201, 255);
            this.SetInt(21, 25);

            this.SetHits(351, 430);

            this.SetDamage(12, 17);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Fire, 10);
            this.SetDamageType(ResistanceType.Cold, 10);
            this.SetDamageType(ResistanceType.Poison, 10);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 50, 70);
            this.SetResistance(ResistanceType.Cold, 50, 70);
            this.SetResistance(ResistanceType.Poison, 50, 70);
            this.SetResistance(ResistanceType.Energy, 40, 60);

            this.SetSkill(SkillName.MagicResist, 100.1, 110.0);
            this.SetSkill(SkillName.Tactics, 85.1, 95.0);
            this.SetSkill(SkillName.Wrestling, 85.1, 95.0);
            this.SetSkill(SkillName.Anatomy, 85.1, 95.0);

            this.Fame = 9000;
            this.Karma = -9000;
			
            if (Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());
				
            this.AddItem(new Tessen());
			
            if (0.02 >= Utility.RandomDouble())
                this.PackItem(new OrigamiPaper());
        }

        public FanDancer(Serial serial)
            : base(serial)
        {
        }

        public override bool Uncalmable
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Gems, 2);
        }

        /* TODO: Repel Magic
        * 10% chance of repelling a melee attack (why did they call it repel magic anyway?)
        * Cliloc: 1070844
        * Effect: damage is dealt to the attacker, no damage is taken by the fan dancer
        */
        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);

            if (0.8 > Utility.RandomDouble() && !attacker.InRange(this, 1))
            {
                /* Fan Throw
                * Effect: - To: "0x57D4F5B" - ItemId: "0x27A3" - ItemIdName: "Tessen" - FromLocation: "(992 299, 24)" - ToLocation: "(992 308, 22)" - Speed: "10" - Duration: "0" - FixedDirection: "False" - Explode: "False" - Hue: "0x0" - Render: "0x0"
                * Damage: 50-65
                */
                Effects.SendPacket(attacker, attacker.Map, new HuedEffect(EffectType.Moving, Serial.Zero, Serial.Zero, 0x27A3, this.Location, attacker.Location, 10, 0, false, false, 0, 0));
                AOS.Damage(attacker, this, Utility.RandomMinMax(50, 65), 100, 0, 0, 0, 0);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.8 > Utility.RandomDouble() && !attacker.InRange(this, 1))
            {
                /* Fan Throw
                * Effect: - To: "0x57D4F5B" - ItemId: "0x27A3" - ItemIdName: "Tessen" - FromLocation: "(992 299, 24)" - ToLocation: "(992 308, 22)" - Speed: "10" - Duration: "0" - FixedDirection: "False" - Explode: "False" - Hue: "0x0" - Render: "0x0"
                * Damage: 50-65
                */
                Effects.SendPacket(attacker, attacker.Map, new HuedEffect(EffectType.Moving, Serial.Zero, Serial.Zero, 0x27A3, this.Location, attacker.Location, 10, 0, false, false, 0, 0));
                AOS.Damage(attacker, this, Utility.RandomMinMax(50, 65), 100, 0, 0, 0, 0);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (!this.IsFanned(defender) && 0.05 > Utility.RandomDouble())
            {
                /* Fanning Fire
                * Graphic: Type: "3" From: "0x57D4F5B" To: "0x0" ItemId: "0x3709" ItemIdName: "fire column" FromLocation: "(994 325, 16)" ToLocation: "(994 325, 16)" Speed: "10" Duration: "30" FixedDirection: "True" Explode: "False" Hue: "0x0" RenderMode: "0x0" Effect: "0x34" ExplodeEffect: "0x1" ExplodeSound: "0x0" Serial: "0x57D4F5B" Layer: "5" Unknown: "0x0"
                * Sound: 0x208
                * Start cliloc: 1070833
                * Effect: Fire res -10% for 10 seconds
                * Damage: 35-45, 100% fire
                * End cliloc: 1070834
                * Effect does not stack
                */
                defender.SendLocalizedMessage(1070833); // The creature fans you with fire, reducing your resistance to fire attacks.

                int effect = -(defender.FireResistance / 10);

                ResistanceMod mod = new ResistanceMod(ResistanceType.Fire, effect);

                defender.FixedParticles(0x37B9, 10, 30, 0x34, EffectLayer.RightFoot);
                defender.PlaySound(0x208);

                // This should be done in place of the normal attack damage.
                //AOS.Damage( defender, this, Utility.RandomMinMax( 35, 45 ), 0, 100, 0, 0, 0 );

                defender.AddResistanceMod(mod);
		
                ExpireTimer timer = new ExpireTimer(defender, mod, TimeSpan.FromSeconds(10.0));
                timer.Start();
                m_Table[defender] = timer;
            }
        }

        public bool IsFanned(Mobile m)
        {
            return m_Table.Contains(m);
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

            protected override void OnTick()
            {
                this.m_Mobile.SendLocalizedMessage(1070834); // Your resistance to fire attacks has returned.
                this.m_Mobile.RemoveResistanceMod(this.m_Mod);
                this.Stop();
                m_Table.Remove(this.m_Mobile);
            }
        }
    }
}