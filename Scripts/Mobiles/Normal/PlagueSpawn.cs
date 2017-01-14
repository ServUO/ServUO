using System;
using System.Collections.Generic;
using Server.ContextMenus;

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
            this.m_Owner = owner;
            this.m_ExpireTime = DateTime.UtcNow + TimeSpan.FromMinutes(1.0);

            this.Name = "a plague spawn";
            this.Hue = Utility.Random(0x11, 15);

            switch ( Utility.Random(12) )
            {
                case 0: // earth elemental
                    this.Body = 14;
                    this.BaseSoundID = 268;
                    break;
                case 1: // headless one
                    this.Body = 31;
                    this.BaseSoundID = 0x39D;
                    break;
                case 2: // person
                    this.Body = Utility.RandomList(400, 401);
                    break;
                case 3: // gorilla
                    this.Body = 0x1D;
                    this.BaseSoundID = 0x9E;
                    break;
                case 4: // serpent
                    this.Body = 0x15;
                    this.BaseSoundID = 0xDB;
                    break;
                default:
                case 5: // slime
                    this.Body = 51;
                    this.BaseSoundID = 456;
                    break;
            }

            this.SetStr(201, 300);
            this.SetDex(80);
            this.SetInt(16, 20);

            this.SetHits(121, 180);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 65, 75);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.MagicResist, 25.0);
            this.SetSkill(SkillName.Tactics, 25.0);
            this.SetSkill(SkillName.Wrestling, 50.0);

            this.Fame = 1000;
            this.Karma = -1000;

            this.VirtualArmor = 20;
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
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ExpireTime
        {
            get
            {
                return this.m_ExpireTime;
            }
            set
            {
                this.m_ExpireTime = value;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override void DisplayPaperdollTo(Mobile to)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i] is ContextMenus.PaperdollEntry)
                    list.RemoveAt(i--);
            }
        }

        public override void OnThink()
        {
            if (this.m_Owner != null && (DateTime.UtcNow >= this.m_ExpireTime || this.m_Owner.Deleted || this.Map != this.m_Owner.Map || !this.InRange(this.m_Owner, 16)))
            {
                this.PlaySound(this.GetIdleSound());
                this.Delete();
            }
            else
            {
                base.OnThink();
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Poor);
            this.AddLoot(LootPack.Gems);
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
        }
    }
}