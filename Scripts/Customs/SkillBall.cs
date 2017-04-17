using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Misc;
// COPYRIGHT BY ROMANTHEBRAIN
namespace Server
{
    public class SkilllimPickGump : Gump
    {


        private int switches = 9;
        private SkillBall m_SkillBall;
        private double val = 100;


        public SkilllimPickGump(SkillBall ball, Mobile player)
            : base(0, 0)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = true;
            m_SkillBall = ball;
            this.AddPage(0);
            this.AddBackground(39, 33, 750, 500, 9200);
            this.AddLabel(67, 41, 1153, @"Please select your 9 skills");
            this.AddButton(80, 500, 2071, 2072, (int)Buttons.Close, GumpButtonType.Reply, 0);
            this.AddBackground(52, 60, 720, 430, 9350);
            this.AddPage(1);
            this.AddButton(690, 500, 2311, 2312, (int)Buttons.FinishButton, GumpButtonType.Reply, 0);
            this.AddCheck(55, 65, 210, 211, false, (int)SkillName.Alchemy);
            this.AddCheck(55, 90, 210, 211, false, (int)SkillName.Anatomy);
            this.AddCheck(55, 115, 210, 211, false, (int)SkillName.AnimalLore);
            this.AddCheck(55, 140, 210, 211, false, (int)SkillName.AnimalTaming);
            if (player.Race != Race.Gargoyle)
            {
                this.AddCheck(55, 165, 210, 211, false, (int)SkillName.Archery);
            }
            else
            {
                this.AddCheck(55, 165, 210, 211, false, (int)SkillName.Throwing);
            }
            this.AddCheck(55, 190, 210, 211, false, (int)SkillName.ArmsLore);
            this.AddCheck(55, 215, 210, 211, false, (int)SkillName.Begging);
            this.AddCheck(55, 240, 210, 211, false, (int)SkillName.Blacksmith);
            this.AddCheck(55, 265, 210, 211, false, (int)SkillName.Bushido);
            this.AddCheck(55, 290, 210, 211, false, (int)SkillName.Camping);
            this.AddCheck(55, 315, 210, 211, false, (int)SkillName.Carpentry);
            this.AddCheck(55, 340, 210, 211, false, (int)SkillName.Cartography);
            this.AddCheck(55, 365, 210, 211, false, (int)SkillName.Chivalry);
            this.AddCheck(55, 390, 210, 211, false, (int)SkillName.Cooking);
            this.AddCheck(55, 415, 210, 211, false, (int)SkillName.DetectHidden);
            this.AddCheck(55, 440, 210, 211, false, (int)SkillName.Discordance);
            this.AddCheck(55, 465, 210, 211, false, (int)SkillName.EvalInt);
            this.AddLabel(80, 65, 0, SkillName.Alchemy.ToString());
            this.AddLabel(80, 90, 0, SkillName.Anatomy.ToString());
            this.AddLabel(80, 115, 0, SkillName.AnimalLore.ToString());
            this.AddLabel(80, 140, 0, SkillName.AnimalTaming.ToString());
            if (player.Race != Race.Gargoyle)
            {
                this.AddLabel(80, 165, 0, SkillName.Archery.ToString());
            }
            else
            {
                this.AddLabel(80, 165, 0, SkillName.Throwing.ToString());
            }
            this.AddLabel(80, 190, 0, SkillName.ArmsLore.ToString());
            this.AddLabel(80, 215, 0, SkillName.Begging.ToString());
            this.AddLabel(80, 240, 0, SkillName.Blacksmith.ToString());
            this.AddLabel(80, 265, 0, SkillName.Bushido.ToString());
            this.AddLabel(80, 290, 0, SkillName.Camping.ToString());
            this.AddLabel(80, 315, 0, SkillName.Carpentry.ToString());
            this.AddLabel(80, 340, 0, SkillName.Cartography.ToString());
            this.AddLabel(80, 365, 0, SkillName.Chivalry.ToString());
            this.AddLabel(80, 390, 0, SkillName.Cooking.ToString());
            this.AddLabel(80, 415, 0, SkillName.DetectHidden.ToString());
            this.AddLabel(80, 440, 0, SkillName.Discordance.ToString());
            this.AddLabel(80, 465, 0, SkillName.EvalInt.ToString());
            // ********************************************************
            this.AddCheck(240, 65, 210, 211, false, (int)SkillName.Fencing);
            this.AddCheck(240, 90, 210, 211, false, (int)SkillName.Fishing);
            this.AddCheck(240, 115, 210, 211, false, (int)SkillName.Fletching);
            this.AddCheck(240, 140, 210, 211, false, (int)SkillName.Focus);
            this.AddCheck(240, 165, 210, 211, false, (int)SkillName.Forensics);
            this.AddCheck(240, 190, 210, 211, false, (int)SkillName.Healing);
            this.AddCheck(240, 215, 210, 211, false, (int)SkillName.Herding);
            this.AddCheck(240, 240, 210, 211, false, (int)SkillName.Hiding);
            this.AddCheck(240, 265, 210, 211, false, (int)SkillName.Inscribe);
            this.AddCheck(240, 290, 210, 211, false, (int)SkillName.ItemID);
            this.AddCheck(240, 315, 210, 211, false, (int)SkillName.Lockpicking);
            this.AddCheck(240, 340, 210, 211, false, (int)SkillName.Lumberjacking);
            this.AddCheck(240, 365, 210, 211, false, (int)SkillName.Macing);
            this.AddCheck(240, 390, 210, 211, false, (int)SkillName.Magery);
            this.AddCheck(240, 415, 210, 211, false, (int)SkillName.MagicResist);
            this.AddCheck(240, 440, 210, 211, false, (int)SkillName.Meditation);
            this.AddCheck(240, 465, 210, 211, false, (int)SkillName.Mining);
            this.AddLabel(265, 65, 0, SkillName.Fencing.ToString());
            this.AddLabel(265, 90, 0, SkillName.Fishing.ToString());
            this.AddLabel(265, 115, 0, SkillName.Fletching.ToString());
            this.AddLabel(265, 140, 0, SkillName.Focus.ToString());
            this.AddLabel(265, 165, 0, SkillName.Forensics.ToString());
            this.AddLabel(265, 190, 0, SkillName.Healing.ToString());
            this.AddLabel(265, 215, 0, SkillName.Herding.ToString());
            this.AddLabel(265, 240, 0, SkillName.Hiding.ToString());
            this.AddLabel(265, 265, 0, SkillName.Inscribe.ToString());
            this.AddLabel(265, 290, 0, SkillName.ItemID.ToString());
            this.AddLabel(265, 315, 0, SkillName.Lockpicking.ToString());
            this.AddLabel(265, 340, 0, SkillName.Lumberjacking.ToString());
            this.AddLabel(265, 365, 0, SkillName.Macing.ToString());
            this.AddLabel(265, 390, 0, SkillName.Magery.ToString());
            this.AddLabel(265, 415, 0, SkillName.MagicResist.ToString());
            this.AddLabel(265, 440, 0, SkillName.Meditation.ToString());
            this.AddLabel(265, 465, 0, SkillName.Mining.ToString());
            // ********************************************************
            this.AddCheck(425, 65, 210, 211, false, (int)SkillName.Musicianship);
            this.AddCheck(425, 90, 210, 211, false, (int)SkillName.Necromancy);
            this.AddCheck(425, 115, 210, 211, false, (int)SkillName.Ninjitsu);
            this.AddCheck(425, 140, 210, 211, false, (int)SkillName.Parry);
            this.AddCheck(425, 165, 210, 211, false, (int)SkillName.Peacemaking);
            this.AddCheck(425, 190, 210, 211, false, (int)SkillName.Poisoning);
            this.AddCheck(425, 215, 210, 211, false, (int)SkillName.Provocation);
            this.AddCheck(425, 240, 210, 211, false, (int)SkillName.RemoveTrap);
            this.AddCheck(425, 265, 210, 211, false, (int)SkillName.Snooping);
            this.AddCheck(425, 290, 210, 211, false, (int)SkillName.Spellweaving);
            this.AddCheck(425, 315, 210, 211, false, (int)SkillName.SpiritSpeak);
            this.AddCheck(425, 340, 210, 211, false, (int)SkillName.Stealing);
            this.AddCheck(425, 365, 210, 211, false, (int)SkillName.Stealth);
            this.AddCheck(425, 390, 210, 211, false, (int)SkillName.Swords);
            this.AddCheck(425, 415, 210, 211, false, (int)SkillName.Tactics);
            this.AddCheck(425, 440, 210, 211, false, (int)SkillName.Tailoring);
            this.AddCheck(425, 465, 210, 211, false, (int)SkillName.TasteID);
            this.AddLabel(450, 65, 0, SkillName.Musicianship.ToString());
            this.AddLabel(450, 90, 0, SkillName.Necromancy.ToString());
            this.AddLabel(450, 115, 0, SkillName.Ninjitsu.ToString());
            this.AddLabel(450, 140, 0, SkillName.Parry.ToString());
            this.AddLabel(450, 165, 0, SkillName.Peacemaking.ToString());
            this.AddLabel(450, 190, 0, SkillName.Poisoning.ToString());
            this.AddLabel(450, 215, 0, SkillName.Provocation.ToString());
            this.AddLabel(450, 240, 0, SkillName.RemoveTrap.ToString());
            this.AddLabel(450, 265, 0, SkillName.Snooping.ToString());
            this.AddLabel(450, 290, 0, SkillName.Spellweaving.ToString());
            this.AddLabel(450, 315, 0, SkillName.SpiritSpeak.ToString());
            this.AddLabel(450, 340, 0, SkillName.Stealing.ToString());
            this.AddLabel(450, 365, 0, SkillName.Stealth.ToString());
            this.AddLabel(450, 390, 0, SkillName.Swords.ToString());
            this.AddLabel(450, 415, 0, SkillName.Tactics.ToString());
            this.AddLabel(450, 440, 0, SkillName.Tailoring.ToString());
            this.AddLabel(450, 465, 0, SkillName.TasteID.ToString());
            //**********************************************************
            this.AddCheck(610, 65, 210, 211, false, (int)SkillName.Tinkering);
            this.AddCheck(610, 90, 210, 211, false, (int)SkillName.Tracking);
            this.AddCheck(610, 115, 210, 211, false, (int)SkillName.Veterinary);
            this.AddCheck(610, 140, 210, 211, false, (int)SkillName.Wrestling);
			this.AddCheck(610, 165, 210, 211, false, (int)SkillName.Mysticism);
            this.AddCheck(610, 190, 210, 211, false, (int)SkillName.Imbuing);
            this.AddLabel(635, 65, 0, SkillName.Tinkering.ToString());
            this.AddLabel(635, 90, 0, SkillName.Tracking.ToString());
            this.AddLabel(635, 115, 0, SkillName.Veterinary.ToString());
            this.AddLabel(635, 140, 0, SkillName.Wrestling.ToString());			
			this.AddLabel(635, 165, 0, SkillName.Mysticism.ToString());
            this.AddLabel(635, 190, 0, SkillName.Imbuing.ToString());
            //**********************************************************
        }

        public enum Buttons
        {
            Close,
            FinishButton,

        }
        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile m = state.Mobile;

            switch (info.ButtonID)
            {
                case 0: { break; }
                case 1:
                    {

                        if (info.Switches.Length < switches)
                        {
                            m.SendGump(new SkilllimPickGump(m_SkillBall, m));
                            m.SendMessage(0, "You must pick {0} more skills.", switches - info.Switches.Length);
                            break;
                        }
                        else if (info.Switches.Length > switches)
                        {
                            m.SendGump(new SkilllimPickGump(m_SkillBall, m));
                            m.SendMessage(0, "Please get rid of {0} skills, you have exceeded the 9 skills that are allowed.", info.Switches.Length - switches);
                            break;

                        }

                        else
                        {

                            Server.Skills skills = m.Skills;

                            for (int i = 0; i < skills.Length; ++i)
                                skills[i].Base = 0;
                            for (int i = 0; i < skills.Length; ++i)
                            {
                                if (info.IsSwitched(i))
                                    m.Skills[i].Base = val;
                            }

                            m_SkillBall.Delete();

                        }

                        break;
                    }

            }

        }

    }

    public class SkillBall : Item
    {
        [Constructable]
        public SkillBall()
            : base(0x2AAA)
        {
            Weight = 1.0;
            Hue = 1153;
            Name = "Use this to set 9 skills to 100";
			LootType = LootType.Blessed;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {

            if (m.Backpack != null && m.Backpack.GetAmount(typeof(SkillBall)) > 0)
            {
                m.SendMessage("Please choose  your 9 skills to set to 7x GM.");
                m.CloseGump(typeof(SkilllimPickGump));
                m.SendGump(new SkilllimPickGump(this, m));
            }
            else
                m.SendMessage(" You need to have the skill ball in your backpack.");

        }

        public SkillBall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

        }

    }
}