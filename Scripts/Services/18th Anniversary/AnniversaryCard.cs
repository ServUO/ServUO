using System;
using Server;

namespace Server.Items
{
    [Flipable(0x9C14, 0x9C15)]
	public class AnniversaryCard : Item
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public string Args { get; set; }
		
		private string[] _Staff = new string[] { "CEO", "Snicker7", "Troi", "Dexter", "Phoenix" }; // TODO: Get More

        [Constructable]
        public AnniversaryCard()
            : this(null)
        {
        }

		[Constructable]
		public AnniversaryCard(Mobile m) : base(0x9C14)
		{
            Hue = 124;

			Args = String.Format("{0}\t{1}", _Staff[Utility.Random(_Staff.Length)], m != null ? m.Name : "you");
		}
		
		public override void AddNameProperty(ObjectPropertyList list)
		{
			list.Add(1156145, Args); // A Personally Written Anniversary Card from ~1_name~ to ~2_name~

            list.Add(1062613, "#1156146");
		}

        public AnniversaryCard(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			
			writer.Write(Args);
		}
 
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			
			Args = reader.ReadString();
        }
	}
}