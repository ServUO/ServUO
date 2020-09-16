using Server.ContextMenus;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a plague spawn corpse")]
    public class PlagueSpawn : BaseCreature
    {
        private Mobile m_Owner;
        private DateTime m_ExpireTime;
        [Constructable]
        public PlagueSpawn()
            : this(null)
        {
        }

        public PlagueSpawn(Mobile owner)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Owner = owner;
            m_ExpireTime = DateTime.UtcNow + TimeSpan.FromMinutes(1.0);

            Name = "a plague spawn";
            Hue = Utility.Random(0x11, 15);

            switch (Utility.Random(12))
            {
                case 0: // earth elemental
                    Body = 14;
                    BaseSoundID = 268;
                    break;
                case 1: // headless one
                    Body = 31;
                    BaseSoundID = 0x39D;
                    break;
                case 2: // person
                    Body = Utility.RandomList(400, 401);
                    break;
                case 3: // gorilla
                    Body = 0x1D;
                    BaseSoundID = 0x9E;
                    break;
                case 4: // serpent
                    Body = 0x15;
                    BaseSoundID = 0xDB;
                    break;
                default:
                case 5: // slime
                    Body = 51;
                    BaseSoundID = 456;
                    break;
            }

            SetStr(201, 300);
            SetDex(80);
            SetInt(16, 20);

            SetHits(121, 180);

            SetDamage(11, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 65, 75);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 25.0);
            SetSkill(SkillName.Tactics, 25.0);
            SetSkill(SkillName.Wrestling, 50.0);

            Fame = 1000;
            Karma = -1000;
        }

        public PlagueSpawn(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return m_Owner;
            }
            set
            {
                m_Owner = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ExpireTime
        {
            get
            {
                return m_ExpireTime;
            }
            set
            {
                m_ExpireTime = value;
            }
        }
        public override bool AlwaysMurderer => true;
        public override void DisplayPaperdollTo(Mobile to)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i] is PaperdollEntry)
                    list.RemoveAt(i--);
            }
        }

        public override void OnThink()
        {
            if (m_Owner != null && (DateTime.UtcNow >= m_ExpireTime || m_Owner.Deleted || Map != m_Owner.Map || !InRange(m_Owner, 16)))
            {
                PlaySound(GetIdleSound());
                Delete();
            }
            else
            {
                base.OnThink();
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
            AddLoot(LootPack.Gems);
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
        }
    }
}