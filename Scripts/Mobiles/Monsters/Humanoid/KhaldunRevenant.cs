using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    public class KhaldunRevenant : BaseCreature
    {
        private static readonly Hashtable m_Table = new Hashtable();
        private readonly Mobile m_Target;
        private readonly DateTime m_ExpireTime;
        public KhaldunRevenant(Mobile target)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.18, 0.36)
        {
            this.Name = "a revenant";
            this.Body = 0x3CA;
            this.Hue = 0x41CE;

            this.m_Target = target;
            this.m_ExpireTime = DateTime.UtcNow + TimeSpan.FromMinutes(10.0);

            this.SetStr(401, 500);
            this.SetDex(296, 315);
            this.SetInt(101, 200);

            this.SetHits(241, 300);
            this.SetStam(242, 280);

            this.SetDamage(20, 30);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Cold, 50);

            this.SetSkill(SkillName.MagicResist, 100.1, 150.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Swords, 140.1, 150.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.Fame = 0;
            this.Karma = 0;

            this.VirtualArmor = 60;

            Halberd weapon = new Halberd();
            weapon.Hue = 0x41CE;
            weapon.Movable = false;

            this.AddItem(weapon);
        }

        public KhaldunRevenant(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return true;
            }
        }
        public override Mobile ConstantFocus
        {
            get
            {
                return this.m_Target;
            }
        }
        public override bool AlwaysAttackable
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
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public static void Initialize()
        {
            EventSink.PlayerDeath += new PlayerDeathEventHandler(EventSink_PlayerDeath);
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            Mobile m = e.Mobile;
            Mobile lastKiller = m.LastKiller;

            if (lastKiller is BaseCreature)
                lastKiller = ((BaseCreature)lastKiller).GetMaster();

            if (IsInsideKhaldun(m) && IsInsideKhaldun(lastKiller) && lastKiller.Player && !m_Table.Contains(lastKiller))
            {
                foreach (AggressorInfo ai in m.Aggressors)
                {
                    if (ai.Attacker == lastKiller && ai.CanReportMurder)
                    {
                        SummonRevenant(m, lastKiller);
                        break;
                    }
                }
            }
        }

        public static void SummonRevenant(Mobile victim, Mobile killer)
        {
            KhaldunRevenant revenant = new KhaldunRevenant(killer);

            revenant.MoveToWorld(victim.Location, victim.Map);
            revenant.Combatant = killer;
            revenant.FixedParticles(0, 0, 0, 0x13A7, EffectLayer.Waist);
            Effects.PlaySound(revenant.Location, revenant.Map, 0x29);

            m_Table.Add(killer, null);
        }

        public static bool IsInsideKhaldun(Mobile from)
        {
            return from != null && from.Region != null && from.Region.IsPartOf("Khaldun");
        }

        public override void DisplayPaperdollTo(Mobile to)
        {
        }

        public override int GetIdleSound()
        {
            return 0x1BF;
        }

        public override int GetAngerSound()
        {
            return 0x107;
        }

        public override int GetDeathSound()
        {
            return 0xFD;
        }

        public override void OnThink()
        {
            if (!this.m_Target.Alive || DateTime.UtcNow > this.m_ExpireTime)
            {
                this.Delete();
                return;
            }

            //Combatant = m_Target;
            //FocusMob = m_Target;

            if (this.AIObject != null)
                this.AIObject.Action = ActionType.Combat;

            base.OnThink();
        }

        public override bool OnBeforeDeath()
        {
            Effects.SendLocationEffect(this.Location, this.Map, 0x376A, 10, 1);
            return true;
        }

        public override void OnDelete()
        {
            if (this.m_Target != null)
                m_Table.Remove(this.m_Target);

            base.OnDelete();
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

            this.Delete();
        }
    }
}