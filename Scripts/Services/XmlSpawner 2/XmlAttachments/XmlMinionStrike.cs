using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlMinionStrike : XmlAttachment
    {
        private readonly ArrayList MinionList = new ArrayList();
        private int m_Chance = 5;// 5% chance by default
        private TimeSpan m_Refractory = TimeSpan.FromSeconds(5);// 5 seconds default time between activations
        private DateTime m_EndTime;
        private string m_Minion = "Drake";
        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlMinionStrike(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlMinionStrike(string minion)
        {
            this.m_Minion = minion;
            this.Expiration = TimeSpan.FromMinutes(30);
        }

        [Attachable]
        public XmlMinionStrike(string minion, int chance)
        {
            this.m_Chance = chance;
            this.m_Minion = minion;
            this.Expiration = TimeSpan.FromMinutes(30);
        }

        [Attachable]
        public XmlMinionStrike(string minion, int chance, double refractory)
        {
            this.m_Chance = chance;
            this.Refractory = TimeSpan.FromSeconds(refractory);
            this.Expiration = TimeSpan.FromMinutes(30);
            this.m_Minion = minion;
        }

        [Attachable]
        public XmlMinionStrike(string minion, int chance, double refractory, double expiresin)
        {
            this.m_Chance = chance;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
            this.Refractory = TimeSpan.FromSeconds(refractory);
            this.m_Minion = minion;
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
        public TimeSpan Refractory
        {
            get
            {
                return this.m_Refractory;
            }
            set
            {
                this.m_Refractory = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Minion
        {
            get
            {
                return this.m_Minion;
            }
            set
            {
                this.m_Minion = value;
            }
        }
        public override void OnAttach()
        {
            base.OnAttach();

            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                Effects.PlaySound(m, m.Map, 516);
            }
        }

        // note that this method will be called when attached to either a mobile or a weapon
        // when attached to a weapon, only that weapon will do additional damage
        // when attached to a mobile, any weapon the mobile wields will do additional damage
        public override void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
        {
            // if it is still refractory then return
            if (DateTime.UtcNow < this.m_EndTime)
                return;

            if (this.m_Chance <= 0 || Utility.Random(100) > this.m_Chance)
                return;

            if (defender != null && attacker != null)
            {
                // spawn a minion
                object o = null;
                try
                {
                    o = Activator.CreateInstance(SpawnerType.GetType(this.m_Minion));
                }
                catch
                {
                }

                if (o is BaseCreature)
                {
                    BaseCreature b = o as BaseCreature;
                    b.MoveToWorld(attacker.Location, attacker.Map);

                    if (attacker is PlayerMobile)
                    {
                        b.Controlled = true;
                        b.ControlMaster = attacker;
                    }

                    b.Combatant = defender;

                    // add it to the list of controlled mobs
                    this.MinionList.Add(b);
                }
                else
                {
                    if (o is Item)
                        ((Item)o).Delete();
                    if (o is Mobile)
                        ((Mobile)o).Delete();
                    // bad minion specification so delete the attachment
                    this.Delete();
                }

                this.m_EndTime = DateTime.UtcNow + this.Refractory;
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
                }
            }

            // delete the minions
            foreach (BaseCreature b in this.MinionList)
            {
                if (b != null && !b.Deleted)
                    b.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_Chance);
            writer.Write(this.m_Minion);
            writer.Write(this.m_Refractory);
            writer.Write(this.m_EndTime - DateTime.UtcNow);
            writer.Write(this.MinionList.Count);
            foreach (BaseCreature b in this.MinionList)
                writer.Write(b);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_Chance = reader.ReadInt();
            this.m_Minion = reader.ReadString();
            this.Refractory = reader.ReadTimeSpan();
            TimeSpan remaining = reader.ReadTimeSpan();
            this.m_EndTime = DateTime.UtcNow + remaining;
            int nminions = reader.ReadInt();
            for (int i = 0; i < nminions; i++)
            {
                BaseCreature b = (BaseCreature)reader.ReadMobile();
                this.MinionList.Add(b);
            }
        }

        public override string OnIdentify(Mobile from)
        {
            string msg = null;

            if (this.Expiration > TimeSpan.Zero)
            {
                msg = String.Format("Minion : {0} {1}% chance expires in {2} mins", this.m_Minion, this.Chance, this.Expiration.TotalMinutes);
            }
            else
            {
                msg = String.Format("Minion : {0}", this.m_Minion);
            }

            if (this.Refractory > TimeSpan.Zero)
            {
                return String.Format("{0} : {1} secs between uses", msg, this.Refractory.TotalSeconds);
            }
            else
                return msg;
        }
    }
}