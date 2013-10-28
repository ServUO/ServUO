using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlFreeze : XmlAttachment
    {
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        // a serial constructor is REQUIRED
        public XmlFreeze(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlFreeze()
        {
        }

        [Attachable]
        public XmlFreeze(double seconds)
        {
            this.Expiration = TimeSpan.FromSeconds(seconds);
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

        public override string OnIdentify(Mobile from)
        {
            base.OnIdentify(from);

            if (from == null || from.IsPlayer())
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("Freeze expires in {1} secs", this.Expiration.TotalSeconds);
            }
            else
            {
                return String.Format("Frozen");
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            // remove the mod
            if (this.AttachedTo is Mobile)
            {
                ((Mobile)this.AttachedTo).Frozen = false;
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            // apply the mod
            if (this.AttachedTo is Mobile)
            {
                ((Mobile)this.AttachedTo).Frozen = true;
                ((Mobile)this.AttachedTo).ProcessDelta();
            }
            else
                this.Delete();
        }
    }
}