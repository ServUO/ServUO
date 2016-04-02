using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a solen queen corpse")]
    public class BlackSolenQueen : BaseCreature
    {
        private bool m_BurstSac;
        [Constructable]
        public BlackSolenQueen()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a black solen queen";
            this.Body = 807;
            this.BaseSoundID = 959;
            this.Hue = 0x453;

            this.SetStr(296, 320);
            this.SetDex(121, 145);
            this.SetInt(76, 100);

            this.SetHits(151, 162);

            this.SetDamage(10, 15);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Poison, 30);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 30, 35);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 35, 40);
            this.SetResistance(ResistanceType.Energy, 25, 30);

            this.SetSkill(SkillName.MagicResist, 70.0);
            this.SetSkill(SkillName.Tactics, 90.0);
            this.SetSkill(SkillName.Wrestling, 90.0);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 45;

            SolenHelper.PackPicnicBasket(this);

            this.PackItem(new ZoogiFungus((Utility.RandomDouble() > 0.05) ? 5 : 25));

            if (Utility.RandomDouble() < 0.05)
                this.PackItem(new BallOfSummoning());
        }

        public BlackSolenQueen(Serial serial)
            : base(serial)
        {
        }

        public bool BurstSac
        {
            get
            {
                return this.m_BurstSac;
            }
        }
        public override int GetAngerSound()
        {
            return 0x259;
        }

        public override int GetIdleSound()
        {
            return 0x259;
        }

        public override int GetAttackSound()
        {
            return 0x195;
        }

        public override int GetHurtSound()
        {
            return 0x250;
        }

        public override int GetDeathSound()
        {
            return 0x25B;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
        }

        public override bool IsEnemy(Mobile m)
        {
            if (SolenHelper.CheckBlackFriendship(m))
                return false;
            else
                return base.IsEnemy(m);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            SolenHelper.OnBlackDamage(from);

            if (!willKill)
            {
                if (!this.BurstSac)
                {
                    if (this.Hits < 50)
                    {
                        this.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "* The solen's acid sac is burst open! *");
                        this.m_BurstSac = true;
                    }
                }
                else if (from != null && from != this && this.InRange(from, 1))
                {
                    this.SpillAcid(from, 1);
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public override bool OnBeforeDeath()
        {
            this.SpillAcid(4);

            return base.OnBeforeDeath();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
            writer.Write(this.m_BurstSac);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
			
            switch( version )
            {
                case 1:
                    {
                        this.m_BurstSac = reader.ReadBool();
                        break;
                    }
            }
        }
    }
}