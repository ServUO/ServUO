using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlFreeze : XmlAttachment
	{

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments
       
		// a serial constructor is REQUIRED
		public XmlFreeze(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlFreeze()
		{
		}
        
		[Attachable]
		public XmlFreeze(double seconds)
		{
			Expiration = TimeSpan.FromSeconds(seconds);
		}
        
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
		
		public override string OnIdentify(Mobile from)
		{
			base.OnIdentify(from);

			if(from == null || from.AccessLevel == AccessLevel.Player) return null;

			if(Expiration > TimeSpan.Zero)
			{
				return String.Format("Freeze expires in {1} secs",Expiration.TotalSeconds);
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
			if(AttachedTo is Mobile)
			{
				((Mobile)AttachedTo).Frozen = false;
			} 
		}

		public override void OnAttach()
		{
			base.OnAttach();

			// apply the mod
			if(AttachedTo is Mobile)
			{
				((Mobile)AttachedTo).Frozen = true;
				((Mobile)AttachedTo).ProcessDelta();
			} 
			else
				Delete();
		}

	}
}
