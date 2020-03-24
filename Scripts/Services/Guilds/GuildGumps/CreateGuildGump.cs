using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Guilds
{
    public class CreateGuildGump : Gump
    {
        public CreateGuildGump(PlayerMobile pm)
            : this(pm, "Guild Name", "")
        {
        }

        public CreateGuildGump(PlayerMobile pm, string guildName, string guildAbbrev)
            : base(10, 10)
        {
            pm.CloseGump(typeof(CreateGuildGump));
            pm.CloseGump(typeof(BaseGuildGump));

            this.AddPage(0);

            this.AddBackground(0, 0, 500, 300, 0x2422);
            this.AddHtmlLocalized(25, 20, 450, 25, 1062939, 0x0, true, false); // <center>GUILD MENU</center>
            this.AddHtmlLocalized(25, 60, 450, 60, 1062940, 0x0, false, false); // As you are not a member of any guild, you can create your own by providing a unique guild name and paying the standard guild registration fee.
            this.AddHtmlLocalized(25, 135, 120, 25, 1062941, 0x0, false, false); // Registration Fee:
            this.AddLabel(155, 135, 0x481, Guild.RegistrationFee.ToString());
            this.AddHtmlLocalized(25, 165, 120, 25, 1011140, 0x0, false, false); // Enter Guild Name: 
            this.AddBackground(155, 160, 320, 26, 0xBB8);
            this.AddTextEntry(160, 163, 315, 21, 0x481, 5, guildName);
            this.AddHtmlLocalized(25, 191, 120, 26, 1063035, 0x0, false, false); // Abbreviation:
            this.AddBackground(155, 186, 320, 26, 0xBB8);
            this.AddTextEntry(160, 189, 315, 21, 0x481, 6, guildAbbrev);
            this.AddButton(415, 217, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0);
            this.AddButton(345, 217, 0xF2, 0xF1, 0, GumpButtonType.Reply, 0);

            if (pm.AcceptGuildInvites)
                this.AddButton(20, 260, 0xD2, 0xD3, 2, GumpButtonType.Reply, 0);
            else
                this.AddButton(20, 260, 0xD3, 0xD2, 2, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(45, 260, 200, 30, 1062943, 0x0, false, false); // <i>Ignore Guild Invites</i>
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (pm == null || pm.Guild != null)
                return;		//Sanity

            switch( info.ButtonID )
            {
                case 1:
                    {
                        TextRelay tName = info.GetTextEntry(5);
                        TextRelay tAbbrev = info.GetTextEntry(6);

                        string guildName = (tName == null) ? "" : tName.Text;
                        string guildAbbrev = (tAbbrev == null) ? "" : tAbbrev.Text;

                        guildName = Utility.FixHtml(guildName.Trim());
                        guildAbbrev = Utility.FixHtml(guildAbbrev.Trim());

                        if (guildName.Length <= 0)
                            pm.SendLocalizedMessage(1070884); // Guild name cannot be blank.
                        else if (guildAbbrev.Length <= 0)
                            pm.SendLocalizedMessage(1070885); // You must provide a guild abbreviation.
                        else if (guildName.Length > Guild.NameLimit)
                            pm.SendLocalizedMessage(1063036, Guild.NameLimit.ToString()); // A guild name cannot be more than ~1_val~ characters in length.
                        else if (guildAbbrev.Length > Guild.AbbrevLimit)
                            pm.SendLocalizedMessage(1063037, Guild.AbbrevLimit.ToString()); // An abbreviation cannot exceed ~1_val~ characters in length.
                        else if (Guild.FindByAbbrev(guildAbbrev) != null || !BaseGuildGump.CheckProfanity(guildAbbrev))
                            pm.SendLocalizedMessage(501153); // That abbreviation is not available.
                        else if (Guild.FindByName(guildName) != null || !BaseGuildGump.CheckProfanity(guildName))
                            pm.SendLocalizedMessage(1063000); // That guild name is not available.
                        else if (!Banker.Withdraw(pm, Guild.RegistrationFee))
                            pm.SendLocalizedMessage(1063001, Guild.RegistrationFee.ToString()); // You do not possess the ~1_val~ gold piece fee required to create a guild.
                        else
                        {
                            pm.SendLocalizedMessage(1060398, Guild.RegistrationFee.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
                            pm.SendLocalizedMessage(1063238); // Your new guild has been founded.
                            pm.Guild = new Guild(pm, guildName, guildAbbrev);
                        }

                        break;
                    }
                case 2:
                    {
                        pm.AcceptGuildInvites = !pm.AcceptGuildInvites;

                        if (pm.AcceptGuildInvites)
                            pm.SendLocalizedMessage(1070699); // You are now accepting guild invitations.
                        else
                            pm.SendLocalizedMessage(1070698); // You are now ignoring guild invitations.

                        break;
                    }
            }
        }
    }
}