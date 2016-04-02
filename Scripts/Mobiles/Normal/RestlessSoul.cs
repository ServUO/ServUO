using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Quests;
using Server.Engines.Quests.Haven;

namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class RestlessSoul : BaseCreature
    {
        [Constructable]
        public RestlessSoul()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            this.Name = "restless soul";
            this.Body = 0x3CA;
            this.Hue = 0x453;

            this.SetStr(26, 40);
            this.SetDex(26, 40);
            this.SetInt(26, 40);

            this.SetHits(16, 24);

            this.SetDamage(1, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 25);
            this.SetResistance(ResistanceType.Fire, 5, 15);
            this.SetResistance(ResistanceType.Cold, 25, 40);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 20.1, 30.0);
            this.SetSkill(SkillName.Swords, 20.1, 30.0);
            this.SetSkill(SkillName.Tactics, 20.1, 30.0);
            this.SetSkill(SkillName.Wrestling, 20.1, 30.0);

            this.Fame = 500;
            this.Karma = -500;

            this.VirtualArmor = 6;
        }

        public RestlessSoul(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysAttackable
        {
            get
            {
                return true;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Poor);
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

        public override int GetIdleSound()
        {
            return 0x107;
        }

        public override int GetAngerSound()
        {
            return 0x1BF;
        }

        public override int GetDeathSound()
        {
            return 0xFD;
        }

        public override bool IsEnemy(Mobile m)
        {
            PlayerMobile player = m as PlayerMobile;

            if (player != null && this.Map == Map.Trammel && this.X >= 5199 && this.X <= 5271 && this.Y >= 1812 && this.Y <= 1865) // Schmendrick's cave
            {
                QuestSystem qs = player.Quest;

                if (qs is UzeraanTurmoilQuest && qs.IsObjectiveInProgress(typeof(FindSchmendrickObjective)))
                {
                    return false;
                }
            }

            return base.IsEnemy(m);
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