#define ServUO
#if(ServUO)
using System;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlFollow : XmlAttachment
    {
        private int m_DataValue;
        // a serial constructor is REQUIRED
        public XmlFollow(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlFollow(int distance)
        {
            this.Distance = distance;
        }

        [Attachable]
        public XmlFollow(int distance, double expiresin)
        {
            this.Distance = distance;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Distance
        {
            get
            {
                return this.m_DataValue;
            }
            set
            {
                this.m_DataValue = value;
                if (this.AttachedTo is BaseCreature)
                {
                    ((BaseCreature)this.AttachedTo).FollowRange = this.m_DataValue;
                }
            }
        }
        // These are the various ways in which the message attachment can be constructed. ?
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_DataValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_DataValue = reader.ReadInt();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            // remove the mod
            if (this.AttachedTo is BaseCreature)
            {
                ((BaseCreature)this.AttachedTo).FollowRange = -1;
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            // apply the mod immediately if attached to a mob
            if (this.AttachedTo is BaseCreature)
            {
                ((BaseCreature)this.AttachedTo).FollowRange = this.Distance;
            }
        }

        public override void OnReattach()
        {
            base.OnReattach();

            // reapply the mod if attached to a mob
            if (this.AttachedTo is BaseCreature)
            {
                ((BaseCreature)this.AttachedTo).FollowRange = this.Distance;
            }
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.IsPlayer() || !(this.AttachedTo is BaseCreature))
                return null;

            BaseCreature b = this.AttachedTo as BaseCreature;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("Following {0} at Distance {1} expires in {2} mins", b.SummonMaster, this.Distance, this.Expiration.TotalMinutes);
            }
            else
            {
                return String.Format("Following {0} at Distance {1}", b.SummonMaster, this.Distance);
            }
        }
    }
}
#endif