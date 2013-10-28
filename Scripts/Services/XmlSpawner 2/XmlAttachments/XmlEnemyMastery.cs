using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlEnemyMastery : XmlAttachment
    {
        private int m_Chance = 20;// 20% chance by default
        private int m_PercentIncrease = 50;
        private string m_Enemy;
        private Type m_EnemyType;
        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlEnemyMastery(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlEnemyMastery(string enemy)
        {
            this.Enemy = enemy;
        }

        [Attachable]
        public XmlEnemyMastery(string enemy, int increase)
        {
            this.m_PercentIncrease = increase;
            this.Enemy = enemy;
        }

        [Attachable]
        public XmlEnemyMastery(string enemy, int chance, int increase)
        {
            this.m_Chance = chance;
            this.m_PercentIncrease = increase;
            this.Enemy = enemy;
        }

        [Attachable]
        public XmlEnemyMastery(string enemy, int chance, int increase, double expiresin)
        {
            this.m_Chance = chance;
            this.m_PercentIncrease = increase;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
            this.Enemy = enemy;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Chance
        {
            get
            {
                return this.m_Chance;
            }
            set
            {
                this.m_Chance = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PercentIncrease
        {
            get
            {
                return this.m_PercentIncrease;
            }
            set
            {
                this.m_PercentIncrease = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Enemy 
        { 
            get
            {
                return this.m_Enemy;
            }
            set 
            { 
                this.m_Enemy = value; 
                // look up the type
                this.m_EnemyType = SpawnerType.GetType(this.m_Enemy);
            }
        }
        public override void OnAttach()
        {
            base.OnAttach();

            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                Effects.PlaySound(m, m.Map, 516);
                m.SendMessage(String.Format("You gain the power of Enemy Mastery over {0}", this.Enemy));
            }
        }

        // note that this method will be called when attached to either a mobile or a weapon
        // when attached to a weapon, only that weapon will do additional damage
        // when attached to a mobile, any weapon the mobile wields will do additional damage
        public override void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
        {
            if (this.m_Chance <= 0 || Utility.Random(100) > this.m_Chance)
                return;

            if (defender != null && attacker != null && this.m_EnemyType != null)
            {
                // is the defender the correct type?
                if (defender.GetType() == this.m_EnemyType || defender.GetType().IsSubclassOf(this.m_EnemyType))
                {
                    defender.Damage((int)(damageGiven * this.PercentIncrease / 100), attacker);
                }
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                if (!m.Deleted)
                {
                    Effects.PlaySound(m, m.Map, 958);
                    m.SendMessage(String.Format("Your power of Enemy Mastery over {0} fades..", this.Enemy));
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_PercentIncrease);
            writer.Write(this.m_Chance);
            writer.Write(this.m_Enemy);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_PercentIncrease = reader.ReadInt();
            this.m_Chance = reader.ReadInt();
            this.Enemy = reader.ReadString();
        }

        public override string OnIdentify(Mobile from)
        {
            string msg = null;

            if (this.Expiration > TimeSpan.Zero)
            {
                msg = String.Format("Enemy Mastery : +{3}% damage vs {0}, {1}%, hitchance expires in {2} mins", this.m_Enemy, this.Chance, this.Expiration.TotalMinutes, this.PercentIncrease);
            }
            else
            {
                msg = String.Format("Enemy Mastery : +{2}% damage vs {0}, {1}% hitchance", this.m_Enemy, this.Chance, this.PercentIncrease);
            }

            return msg;
        }
    }
}