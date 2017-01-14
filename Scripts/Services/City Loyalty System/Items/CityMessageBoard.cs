using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Engines.CityLoyalty;
using System.Linq;
using Server.Prompts;
using Server.ContextMenus;

namespace Server.Items
{
	public class CityMessageBoard : Item
	{
        public City City { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltySystem CitySystem { get { return CityLoyaltySystem.GetCityInstance(City); } set { } }

        public override string DefaultName
        {
            get
            {
                if (CitySystem != null)
                    return String.Format("{0} Message Board", CitySystem.Definition.Name);

                return "Message Board";
            }
        }

        [Constructable]
        public CityMessageBoard(City city, int id) : base(id)
        {
            Movable = false;
            City = city;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!CityLoyaltySystem.Enabled || CitySystem == null)
                return;

            if (from.InRange(this.Location, 3))
            {
                from.CloseGump(typeof(CityMessageGump));
                from.SendGump(new CityMessageBoardGump());
            }
            else
                from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, 1019045, from.NetState);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (!CityLoyaltySystem.Enabled || CitySystem == null || !CitySystem.IsGovernor(from))
                return;

            list.Add(new SimpleContextMenuEntry(from, 1154912, m => // Set New Announcement
            {
                if (m.Prompt != null)
                    m.SendLocalizedMessage(1079166); // You already have a text entry request pending.
                else if (CitySystem.Herald != null)
                {
                    m.SendMessage("Enter message board headline:");
                    m.BeginPrompt(
                        (mob, text) =>
                        {
                            if (Server.Guilds.BaseGuildGump.CheckProfanity(text, 150))
                            {
                                CitySystem.Herald.Announcement = text;
                            }
                            else
                                mob.SendLocalizedMessage(1112587); // Invalid entry.
                        },
                        (mob, text) =>
                        {
                            mob.SendMessage("Herald message unchanged.");
                        });
                }
            }, enabled: CitySystem.Herald != null));

            list.Add(new SimpleContextMenuEntry(from, 1154913, m => // Set Headline
            {
                if (m.Prompt != null)
                    m.SendLocalizedMessage(1079166); // You already have a text entry request pending.
                else
                {
                    m.SendMessage("Enter message board headline:");
                    m.BeginPrompt(
                        (mob, text) =>
                        {
                            if (Server.Guilds.BaseGuildGump.CheckProfanity(text, 150))
                            {
                                if (String.IsNullOrEmpty(text))
                                {
                                    CitySystem.Headline = null;
                                    CitySystem.Body = null;
                                    CitySystem.PostedOn = DateTime.Now;
                                    mob.SendMessage("{0} message board headline removed!", CitySystem.Definition.Name);
                                }
                                else
                                {
                                    CitySystem.Headline = text;
                                    CitySystem.PostedOn = DateTime.Now;
                                    mob.SendMessage("{0} message board headline changed!", CitySystem.Definition.Name);
                                }
                            }
                            else
                                mob.SendLocalizedMessage(1112587); // Invalid entry.
                        },
                        (mob, text) =>
                        {
                            mob.SendMessage("Message headline unchanged.");
                        });
                }
            }));

            list.Add(new SimpleContextMenuEntry(from, 1154914, m => // Set Body
            {
                if (m.Prompt != null)
                    m.SendLocalizedMessage(1079166); // You already have a text entry request pending.
                else
                {
                    m.SendMessage("Enter message board body:");
                    m.BeginPrompt(
                        (mob, text) =>
                        {
                            if (Server.Guilds.BaseGuildGump.CheckProfanity(text, 150))
                            {
                                if (String.IsNullOrEmpty(text))
                                {
                                    CitySystem.Body = null;
                                    CitySystem.PostedOn = DateTime.Now;
                                    mob.SendMessage("{0} message board body removed!", CitySystem.Definition.Name);
                                }
                                else
                                {
                                    CitySystem.Body = text;
                                    CitySystem.PostedOn = DateTime.Now;
                                    mob.SendMessage("{0} message board body removed!", CitySystem.Definition.Name);
                                }
                            }
                            else
                                mob.SendLocalizedMessage(1112587); // Invalid entry.
                        },
                        (mob, text) =>
                        {
                            mob.SendMessage("Message body unchanged.");
                        });
                }
            }));
        }

        public CityMessageBoard(Serial serial)
            : base(serial)
        {
        }
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);

            writer.Write((int)City);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

            City = (City)reader.ReadInt();
		}
	}
}