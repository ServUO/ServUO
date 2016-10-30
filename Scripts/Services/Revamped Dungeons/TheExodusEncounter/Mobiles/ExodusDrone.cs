using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a exodus drone's corpse")]
    public class ExodusDrone : BaseCreature
    {
        private bool m_FieldActive;
        [Constructable]
        public ExodusDrone() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Exodus Drone";
            this.Body = 0x2F4;
            this.Hue = 0xA92;
            this.SetStr(554, 650);
            this.SetDex(77, 85);
            this.SetInt(64, 85);

            this.SetHits(332, 389);
            this.SetDamage(13, 19);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 40, 60);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 25, 34);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.MagicResist, 83.8, 95.4);
            this.SetSkill(SkillName.Tactics, 82.9, 94.9);
            this.SetSkill(SkillName.Wrestling, 84.7, 95.9);

            this.Fame = 18000;
            this.Karma = -18000;
            this.VirtualArmor = 65;

            this.PackItem(new PowerCrystal());
            this.PackItem(new ArcaneGem());
            this.PackItem(new ClockworkAssembly());

		}
		
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);            

            if (Utility.RandomDouble() < 0.1)
            {
                switch (Utility.Random(4))
                {
                    case 0:
                        PackItem(new ExodusSummoningRite());
                        break;
                    case 1:
                        PackItem(new ExodusSacrificalDagger());
                        break;
                    case 2:
                        PackItem(new RobeofRite());
                        break;
                    case 3:
                        PackItem(new ExodusSummoningAlter());
                        break;
                }
            }

            this.m_FieldActive = this.CanUseField;
        }

        public ExodusDrone(Serial serial) : base(serial)
        {
        }

        public bool FieldActive
        {
            get
            {
                return this.m_FieldActive;
            }
        }
        public bool CanUseField
        {
            get
            {
                return this.Hits >= this.HitsMax * 9 / 10;
            }
        }// TODO: an OSI bug prevents to verify this
        public override bool IsScaredOfScaryThings
        {
            get
            {
                return false;
            }
        }
        public override bool IsScaryToPets
        {
            get
            {
                return true;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }


        public override int GetIdleSound()
        {
            return 0x218;
        }

        public override int GetAngerSound()
        {
            return 0x26C;
        }

        public override int GetDeathSound()
        {
            return 0x211;
        }

        public override int GetAttackSound()
        {
            return 0x232;
        }

        public override int GetHurtSound()
        {
            return 0x140;
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (this.m_FieldActive)
                damage = 0; // no melee damage when the field is up
        }

        public override void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
            if (!this.m_FieldActive)
                damage = 0; // no spell damage when the field is down
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            if (from != null && from.Alive && 0.4 > Utility.RandomDouble())
            {
                this.SendEBolt(from);
            }

            if (!this.m_FieldActive)
            {
                // should there be an effect when spells nullifying is on?
                this.FixedParticles(0, 10, 0, 0x2522, EffectLayer.Waist);
            }
            else if (this.m_FieldActive && !this.CanUseField)
            {
                this.m_FieldActive = false;

                // TODO: message and effect when field turns down; cannot be verified on OSI due to a bug
                this.FixedParticles(0x3735, 1, 30, 0x251F, EffectLayer.Waist);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (this.m_FieldActive)
            {
                this.FixedParticles(0x376A, 20, 10, 0x2530, EffectLayer.Waist);

                this.PlaySound(0x2F4);

                attacker.SendAsciiMessage("Your weapon cannot penetrate the creature's magical barrier");
            }

            if (attacker != null && attacker.Alive && attacker.Weapon is BaseRanged && 0.4 > Utility.RandomDouble())
            {
                this.SendEBolt(attacker);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // TODO: an OSI bug prevents to verify if the field can regenerate or not
            if (!this.m_FieldActive && !this.IsHurt())
                this.m_FieldActive = true;
        }

        public override bool Move(Direction d)
        {
            bool move = base.Move(d);

            if (move && this.m_FieldActive && this.Combatant != null)
                this.FixedParticles(0, 10, 0, 0x2530, EffectLayer.Waist);

            return move;
        }

        public void SendEBolt(Mobile to)
        {
            this.MovingParticles(to, 0x379F, 7, 0, false, true, 0xBE3, 0xFCB, 0x211);
            to.PlaySound(0x229);
            this.DoHarmful(to);
            AOS.Damage(to, this, 50, 0, 0, 0, 0, 100);
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

            this.m_FieldActive = this.CanUseField;

            if (this.Name == "Exodus Drone")
                this.Name = "exodus drone";
        }
    }
}