using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a shadow knight corpse")]
    public class ShadowKnight : BaseCreature
    {
        public override bool CanStealth { get { return true; } }

        private Timer m_SoundTimer;
        private bool m_HasTeleportedAway;
        [Constructable]
        public ShadowKnight()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("shadow knight");
            this.Title = "the Shadow Knight";
            this.Body = 311;

            this.SetStr(250);
            this.SetDex(100);
            this.SetInt(100);

            this.SetHits(2000);

            this.SetDamage(20, 30);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Cold, 40);

            this.SetResistance(ResistanceType.Physical, 90);
            this.SetResistance(ResistanceType.Fire, 65);
            this.SetResistance(ResistanceType.Cold, 75);
            this.SetResistance(ResistanceType.Poison, 75);
            this.SetResistance(ResistanceType.Energy, 55);

            this.SetSkill(SkillName.Chivalry, 120.0);
            this.SetSkill(SkillName.DetectHidden, 80.0);
            this.SetSkill(SkillName.EvalInt, 100.0);
            this.SetSkill(SkillName.Magery, 100.0);
            this.SetSkill(SkillName.Meditation, 100.0);
            this.SetSkill(SkillName.MagicResist, 120.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);

            this.Fame = 25000;
            this.Karma = -25000;

            this.VirtualArmor = 54;
        }

        public ShadowKnight(Serial serial)
            : base(serial)
        {
        }

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			RevealingAction();
			base.OnDamage(amount, from, willKill);
		}

		public override void OnDamagedBySpell(Mobile from)
		{
			RevealingAction();
			base.OnDamagedBySpell(from);
		}

        public override bool IgnoreYoungProtection
        {
            get
            {
                return Core.ML;
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.SE;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return Core.SE;
            }
        }
        public override bool AreaPeaceImmune
        {
            get
            {
                return Core.SE;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return Utility.RandomBool() ? WeaponAbility.ConcussionBlow : WeaponAbility.CrushingBlow;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
        }

        

        public override int GetIdleSound()
        {
            return 0x2CE;
        }

        public override int GetDeathSound()
        {
            return 0x2C1;
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }

        public override int GetAttackSound()
        {
            return 0x2C8;
        }

        public override void OnCombatantChange()
        {
            base.OnCombatantChange();

            if (this.Hidden && this.Combatant != null)
                this.Combatant = null;
        }

        public virtual void SendTrackingSound()
        {
            if (this.Hidden)
            {
                Effects.PlaySound(this.Location, this.Map, 0x2C8);
                this.Combatant = null;
            }
            else
            {
                this.Frozen = false;

                if (this.m_SoundTimer != null)
                    this.m_SoundTimer.Stop();

                this.m_SoundTimer = null;
            }
        }

        public override void OnThink()
        {
            if (!this.m_HasTeleportedAway && this.Hits < (this.HitsMax / 2))
            {
                Map map = this.Map;

                if (map != null)
                {
                    // try 10 times to find a teleport spot
                    for (int i = 0; i < 10; ++i)
                    {
                        int x = this.X + (Utility.RandomMinMax(5, 10) * (Utility.RandomBool() ? 1 : -1));
                        int y = this.Y + (Utility.RandomMinMax(5, 10) * (Utility.RandomBool() ? 1 : -1));
                        int z = this.Z;

                        if (!map.CanFit(x, y, z, 16, false, false))
                            continue;

                        Point3D from = this.Location;
                        Point3D to = new Point3D(x, y, z);

                        if (!this.InLOS(to))
                            continue;

                        this.Location = to;
                        this.ProcessDelta();
                        this.Hidden = true;
                        this.Combatant = null;

                        Effects.SendLocationParticles(EffectItem.Create(from, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                        Effects.SendLocationParticles(EffectItem.Create(to, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                        Effects.PlaySound(to, map, 0x1FE);

                        this.m_HasTeleportedAway = true;
                        this.m_SoundTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(2.5), new TimerCallback(SendTrackingSound));

                        this.Frozen = true;

                        break;
                    }
                }
            }

            base.OnThink();
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

            if (this.BaseSoundID == 357)
                this.BaseSoundID = -1;
        }
    }
}