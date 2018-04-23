using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;


namespace Server.Items
{
	public class GuildTravelPrompt : Prompt
	{
		private readonly Mobile m_Mobile;
		public GuildTravelPrompt(Mobile from)
		{
			this.m_Mobile = from;
		}
		public override void OnResponse(Mobile from, string name)
		{
			bool Flagged = false;
			foreach (NetState state in NetState.Instances) 
			{
				Mobile m = state.Mobile;
				if (m != null && m.Guild == from.Guild && m.Name == name) 
				{
					if (m.Backpack.FindItemByType(typeof(GuildTravelStone)) != null) 
					{
						if (from.Backpack != null) 
						{
							if (from.Backpack.FindItemByType(typeof(BlackPearl)) != null && from.Backpack.FindItemByType (typeof(Bloodmoss)) != null && from.Backpack.FindItemByType (typeof(MandrakeRoot)) != null) {
								from.Backpack.FindItemByType(typeof(BlackPearl)).Consume();
								from.Backpack.FindItemByType(typeof(Bloodmoss)).Consume();
								from.Backpack.FindItemByType(typeof(MandrakeRoot)).Consume();
								from.PublicOverheadMessage(MessageType.Spell, from.SpeechHue, true, "Kal Ort Por", false);
								from.PlaySound (0x1FC);
								from.SendMessage("Teleporting to guildmate...");
								from.Map = m.Map;
								from.Location = m.Location;
								from.PlaySound(0x1FC);
								Flagged = true; 
							}
							else
							{
								from.SendMessage(0x22, "You do not have enough reagents to cast Recall!");
								Flagged = true;
							}
						} 
						else 
						{
							from.SendMessage(0x22, "We could not find your backpack.");
							Flagged = true;
						}
					}
					else 
					{
						from.SendMessage(0x22, "This guild member does not have a Guild Travel Stone in his backpack!");
						Flagged = true;
					}
				}
				else if (m != null && m.Guild != from.Guild && m.Name == name) 
				{
					from.SendMessage(0x22, "This mobile is not in your guild!");
					Flagged = true;
				}
			}
			if (Flagged == false){
				from.SendMessage(0x22, "We did not find an online member of your guild with that name.");
			}
		}
		public override void OnCancel(Mobile from)
		{
			return;
		}
	}

	public class GuildTravelStone : DecoMagicalCrystal
	{
		private readonly PlayerMobile m_Guildmate;
		[Constructable]
		public GuildTravelStone()
		{
			this.Name = "Guild Travel Stone";
			this.Hue = 0x495;
		}

		public GuildTravelStone(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendMessage("The item needs to be in your pack");
			}
			else
			{
				from.SendMessage("Type the name of the guildmate you wish to teleport too:");
				from.Prompt = new GuildTravelPrompt(from);
			}
		}
			
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}