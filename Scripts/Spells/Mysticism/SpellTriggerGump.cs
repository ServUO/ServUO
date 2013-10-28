using System;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class SpellTriggerGump : Gump
    {
        public SpellTriggerGump(Mobile m)
            : base(0, 0)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            int skill = (int)m.Skills[SkillName.Mysticism].Value;

            this.AddPage(0);
            this.AddBackground(0, 0, 170, 400, 9270);
            this.AddAlphaRegion(10, 10, 150, 380);

            // Listing them in skill order would have been easier, all well.

            // Skill required < 45
            this.AddLabel(40, 15, 0, "Animated Weapon");
            this.AddButton(15, 15, 9702, 9703, 683, GumpButtonType.Reply, 0); // Animated Weapon

            if (skill > 58)
            {
                this.AddLabel(40, 40, 0, "Bombard");
                this.AddButton(15, 40, 9702, 9703, 688, GumpButtonType.Reply, 0); // Bombard
            }
            else
            {
                this.AddLabel(40, 40, 995, "Bombard");
                this.AddImage(15, 40, 9702, 995);
            }

            if (skill > 58)
            {
                this.AddLabel(40, 65, 0, "Cleansing Winds");
                this.AddButton(15, 65, 9702, 9703, 687, GumpButtonType.Reply, 0); // Cleansing Winds
            }
            else
            {
                this.AddLabel(40, 65, 995, "Cleansing Winds");
                this.AddImage(15, 65, 9702, 995);
            }

            // Skill required < 45
            this.AddLabel(40, 90, 0, "Eagle Strike");
            this.AddButton(15, 90, 9702, 9703, 682, GumpButtonType.Reply, 0); // Eagle Strike

            // Skill required < 45
            this.AddLabel(40, 115, 0, "Enchant");
            this.AddButton(15, 115, 9702, 9703, 680, GumpButtonType.Reply, 0); // Enchant

            if (skill > 70)
            {
                this.AddLabel(40, 140, 0, "Hail Storm");
                this.AddButton(15, 140, 9702, 9703, 690, GumpButtonType.Reply, 0); // Hail Storm
            }
            else
            {
                this.AddLabel(40, 140, 995, "Hail Storm");
                this.AddImage(15, 140, 9702, 995);
            }

            // No skill required.
            this.AddLabel(40, 165, 0, "Healing Stone");
            this.AddButton(15, 165, 9702, 9703, 678, GumpButtonType.Reply, 0); // Healing Stone

            // Srsly, 0 skill?
            this.AddLabel(40, 190, 0, "Mass Sleep");
            this.AddButton(15, 190, 9702, 9703, 686, GumpButtonType.Reply, 0); // Mass Sleep

            // No skill required.
            this.AddLabel(40, 215, 0, "Nether Bolt");
            this.AddButton(15, 215, 9702, 9703, 677, GumpButtonType.Reply, 0); // Nether Bolt

            if (skill > 83)
            {
                this.AddLabel(40, 240, 0, "Nether Cyclone");
                this.AddButton(15, 240, 9702, 9703, 691, GumpButtonType.Reply, 0); // Nether Cyclone
            }
            else
            {
                this.AddLabel(40, 240, 995, "Nether Cyclone");
                this.AddImage(15, 240, 9702, 995);
            }

            // Skill required < 45
            this.AddLabel(40, 265, 0, "Purge Magic");
            this.AddButton(15, 265, 9702, 9703, 679, GumpButtonType.Reply, 0); // Purge Magic

            if (skill > 83)
            {
                this.AddLabel(40, 290, 0, "Rising Colossus");
                this.AddButton(15, 290, 9702, 9703, 692, GumpButtonType.Reply, 0); // Rising Colossus
            }
            else
            {
                this.AddLabel(40, 290, 995, "Rising Colossus");
                this.AddImage(15, 290, 9702, 995);
            }

            // Skill required < 45
            this.AddLabel(40, 315, 0, "Sleep");
            this.AddButton(15, 315, 9702, 9703, 681, GumpButtonType.Reply, 0); // Sleep

            if (skill > 70)
            {
                this.AddLabel(40, 340, 0, "Spell Plague");
                this.AddButton(15, 340, 9702, 9703, 689, GumpButtonType.Reply, 0); // Spell Plague
            }
            else
            {
                this.AddLabel(40, 340, 995, "Spell Plague");
                this.AddImage(15, 340, 9702, 995);
            }

            // Skill required < 45
            this.AddLabel(40, 365, 0, "Stone Form");
            this.AddButton(15, 365, 9702, 9703, 684, GumpButtonType.Reply, 0); // Stone Form
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (!(info.ButtonID >= 677 && info.ButtonID <= 692))
                from.SendMessage("There was an error in your spell choice, please try again or page if you have.");
            else if (from.Backpack != null)
            {
                Item[] stones = from.Backpack.FindItemsByType(typeof(SpellStone));

                for (int i = 0; i < stones.Length; i++)
                    stones[i].Delete();

                from.PlaySound(0x659);
                from.Backpack.DropItem(new SpellStone(from, info.ButtonID));
                from.SendLocalizedMessage(1080165); // A Spell Stone appears in your backpack
            }
        }
    }
}