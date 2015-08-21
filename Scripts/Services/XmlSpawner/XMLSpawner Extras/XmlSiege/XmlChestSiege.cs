using System;
using Server;
using Server.Items;
using Server.Network;
using System.Collections;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlChestSiege : XmlSiege
	{
		public override int LightDamageEffectID { get { return 14201; }  } // 14201 = sparkle
		public override int MediumDamageEffectID { get { return 14201; } }
		public override int HeavyDamageEffectID { get { return 14201; }  }

		// a serial constructor is REQUIRED
		public XmlChestSiege(ASerial serial)
			: base(serial)
		{
		}

		[Attachable]
		public XmlChestSiege()
		{
			HitsMax = 100;
		}

		[Attachable]
		public XmlChestSiege(int hitsmax)
		{
			HitsMax = hitsmax;
			Hits = HitsMax;
		}

		[Attachable]
		public XmlChestSiege(int hitsmax, int resistfire, int resistphysical)
		{
			HitsMax = hitsmax;
			Hits = HitsMax;
			ResistPhysical = resistphysical;
			ResistFire = resistfire;
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

		public override void OnAttach()
		{
			base.OnAttach();

			// only allow attachment to containers
			if (!(AttachedTo is Container))
			{
				Delete();
			}
		}

		public override void OnDestroyed()
		{
			Container chest = AttachedTo as Container;

			if (chest != null && chest.Map != null && chest.Map != Map.Internal)
			{
				ArrayList movelist = new ArrayList(chest.Items);

				foreach (Item i in movelist)
				{
					// spill the contents out onto the ground
					i.MoveToWorld(chest.Location, chest.Map);
				}

				// and permanently destroy the container
				chest.Delete();
			}
		}

	}
}
