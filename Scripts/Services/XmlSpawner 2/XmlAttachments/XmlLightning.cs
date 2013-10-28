using System;
using Server.Items;
using Server.Spells;

namespace Server.Engines.XmlSpawner2
{
    public class XmlLightning : XmlAttachment
    {
        private int m_Damage = 0;
        private TimeSpan m_Refractory = TimeSpan.FromSeconds(5);// 5 seconds default time between activations
        private DateTime m_EndTime;
        private int proximityrange = 5;// default movement activation from 5 tiles away

        // a serial constructor is REQUIRED
        public XmlLightning(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlLightning(int damage)
        {
            this.m_Damage = damage;
        }

        [Attachable]
        public XmlLightning(int damage, double refractory)
        {
            this.m_Damage = damage;
            this.Refractory = TimeSpan.FromSeconds(refractory);
        }

        [Attachable]
        public XmlLightning(int damage, double refractory, double expiresin)
        {
            this.m_Damage = damage;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
            this.Refractory = TimeSpan.FromSeconds(refractory);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Damage
        {
            get
            {
                return this.m_Damage;
            }
            set
            {
                this.m_Damage = value;
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
        public int Range
        {
            get
            {
                return this.proximityrange;
            }
            set
            {
                this.proximityrange = value;
            }
        }
        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // restrict the movement detection feature to non-movable items
        public override bool HandlesOnMovement 
        { 
            get 
            { 
                if (this.AttachedTo is Item && !((Item)this.AttachedTo).Movable)
                    return true;
                else
                    return false; 
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

            int damage = 0;

            if (this.m_Damage > 0)
                damage = Utility.Random(this.m_Damage);

            if (defender != null && attacker != null && damage > 0)
            {
                defender.BoltEffect(0);

                SpellHelper.Damage(TimeSpan.Zero, defender, attacker, damage, 0, 0, 0, 0, 100);

                this.m_EndTime = DateTime.UtcNow + this.Refractory;
            }
        }

        //public override bool HandlesOnMovement { get { return true; } }
        public override void OnMovement(MovementEventArgs e)
        {
            base.OnMovement(e);
   
            if (e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player)
                return;

            if (this.AttachedTo is Item && (((Item)this.AttachedTo).Parent == null) && Utility.InRange(e.Mobile.Location, ((Item)this.AttachedTo).Location, this.proximityrange))
            {
                this.OnTrigger(null, e.Mobile);
            }
            else
                return;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
            // version 1
            writer.Write(this.proximityrange);
            // version 0
            writer.Write(this.m_Damage);
            writer.Write(this.m_Refractory);
            writer.Write(this.m_EndTime - DateTime.UtcNow);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch(version)
            {
                case 1:
                    this.proximityrange = reader.ReadInt();
                    goto case 0;
                case 0:
                    // version 0
                    this.m_Damage = reader.ReadInt();
                    this.Refractory = reader.ReadTimeSpan();
                    TimeSpan remaining = reader.ReadTimeSpan();
                    this.m_EndTime = DateTime.UtcNow + remaining;
                    break;
            }
        }

        public override string OnIdentify(Mobile from)
        {
            string msg = null;

            if (this.Expiration > TimeSpan.Zero)
            {
                msg = String.Format("Lightning Damage {0} expires in {1} mins", this.m_Damage, this.Expiration.TotalMinutes);
            }
            else
            {
                msg = String.Format("Lightning Damage {0}", this.m_Damage);
            }

            if (this.Refractory > TimeSpan.Zero)
            {
                return String.Format("{0} - {1} secs between uses", msg, this.Refractory.TotalSeconds);
            }
            else
                return msg;
        }

        public override void OnTrigger(object activator, Mobile m)
        {
            if (m == null)
                return;

            // if it is still refractory then return
            if (DateTime.UtcNow < this.m_EndTime)
                return;

            int damage = 0;

            if (this.m_Damage > 0)
                damage = Utility.Random(this.m_Damage);

            if (damage > 0)
            {
                m.BoltEffect(0);

                SpellHelper.Damage(TimeSpan.Zero, m, damage, 0, 0, 0, 0, 100);
            }

            this.m_EndTime = DateTime.UtcNow + this.Refractory;
        }
    }
}