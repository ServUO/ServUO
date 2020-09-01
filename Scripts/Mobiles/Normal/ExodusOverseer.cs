using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an overseer's corpse")]
    public class ExodusOverseer : BaseCreature
    {
        private bool m_FieldActive;
        [Constructable]
        public ExodusOverseer()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "exodus overseer";
            Body = 0x2F4;

            SetStr(561, 650);
            SetDex(76, 95);
            SetInt(61, 90);

            SetHits(331, 390);

            SetDamage(13, 19);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 80.2, 98.0);
            SetSkill(SkillName.Tactics, 80.2, 98.0);
            SetSkill(SkillName.Wrestling, 80.2, 98.0);

            Fame = 10000;
            Karma = -10000;

            m_FieldActive = CanUseField;
        }

        public ExodusOverseer(Serial serial)
            : base(serial)
        {
        }

        public bool FieldActive => m_FieldActive;
        public bool CanUseField => Hits >= HitsMax * 9 / 10;// TODO: an OSI bug prevents to verify this
        public override bool IsScaredOfScaryThings => false;
        public override bool IsScaryToPets => true;
        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.RandomLootItem(new System.Type[] { typeof(PowerCrystal), typeof(ArcaneGem) }));
        }

        public override int GetIdleSound()
        {
            return 0xFD;
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
            return 0x23B;
        }

        public override int GetHurtSound()
        {
            return 0x140;
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (m_FieldActive)
                damage = 0; // no melee damage when the field is up
        }

        public override void AlterSpellDamageFrom(Mobile caster, ref int damage)
        {
            if (!m_FieldActive)
                damage = 0; // no spell damage when the field is down
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            if (from != null && from.Alive && 0.4 > Utility.RandomDouble())
            {
                SendEBolt(from);
            }

            if (!m_FieldActive)
            {
                // should there be an effect when spells nullifying is on?
                FixedParticles(0, 10, 0, 0x2522, EffectLayer.Waist);
            }
            else if (m_FieldActive && !CanUseField)
            {
                m_FieldActive = false;

                // TODO: message and effect when field turns down; cannot be verified on OSI due to a bug
                FixedParticles(0x3735, 1, 30, 0x251F, EffectLayer.Waist);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (m_FieldActive)
            {
                FixedParticles(0x376A, 20, 10, 0x2530, EffectLayer.Waist);

                PlaySound(0x2F4);

                attacker.SendAsciiMessage("Your weapon cannot penetrate the creature's magical barrier");
            }

            if (attacker != null && attacker.Alive && attacker.Weapon is BaseRanged && 0.4 > Utility.RandomDouble())
            {
                SendEBolt(attacker);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // TODO: an OSI bug prevents to verify if the field can regenerate or not
            if (!m_FieldActive && !IsHurt())
                m_FieldActive = true;
        }

        public override bool Move(Direction d)
        {
            bool move = base.Move(d);

            if (move && m_FieldActive && Combatant != null)
                FixedParticles(0, 10, 0, 0x2530, EffectLayer.Waist);

            return move;
        }

        public void SendEBolt(Mobile to)
        {
            MovingParticles(to, 0x379F, 7, 0, false, true, 0xBE3, 0xFCB, 0x211);
            to.PlaySound(0x229);
            DoHarmful(to);
            AOS.Damage(to, this, 50, 0, 0, 0, 0, 100);
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

            m_FieldActive = CanUseField;
        }
    }
}
