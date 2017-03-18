/**************************************
*Script Name: Staff Runebook          *
*Author: Joeku                        *
*For use with RunUO 2.0 RC2           *
*Client Tested with: 6.0.9.2          *
*Version: 1.10                        *
*Initial Release: 11/25/07            *
*Revision Date: 02/04/09              *
**************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Joeku.SR
{
    public class SR_Gump : Gump
    {
        public SR_RuneAccount RuneAcc;
        public SR_Gump(SR_RuneAccount runeAcc)
            : base(0, 27)
        {
            this.RuneAcc = runeAcc;

            int count = 0;
            if (this.RuneAcc.ChildRune == null)
                count = this.RuneAcc.Count;
            else
                count = this.RuneAcc.ChildRune.Count;

            int RunebooksH = 0,
            RunebooksW = 0;

            int tier = -1;
            if (this.RuneAcc.ChildRune != null)
                tier = this.RuneAcc.ChildRune.Tier;

            if (tier > -1)
            {
                if (tier == 0)
                {
                    RunebooksH = 42;
                    RunebooksW = 278;
                }
                else
                {
                    RunebooksH = 37 + 42;
                    RunebooksW = 278 + (tier * 5);
                }
            }

            int RunesH = 10 * 2;

            if (count > 10)
                count = 10;
            if (count > 0)
                RunesH += (count * 22);
            if (count > 1)
                RunesH += ((count - 1) * 5);

            this.DisplayHeader();
            if (tier > -1)
                this.DisplayRunebooks(42, RunebooksH, RunebooksW, tier);
            this.DisplayAddNew(42 + RunebooksH + RunesH);
            this.DisplayRunes(42 + RunebooksH, RunesH);
        }

        public static void Send(Mobile mob, SR_RuneAccount runeAcc)
        {
            mob.CloseGump(typeof(SR_Gump));
            mob.SendGump(new SR_Gump(runeAcc));
        }

        public void DisplayHeader()
        {
            this.AddPage(0);
            this.AddBackground(0, 0, 210, 42, 9270); 
            this.AddImageTiled(10, 10, 190, 22, 2624); 
            this.AddAlphaRegion(10, 10, 190, 22); 
            this.AddHtml(0, 11, 210, 20, "<BASEFONT COLOR=#FFFFFF><CENTER><BIG>Joeku's Staff Runebook", false, false);
        }

        public void DisplayRunebooks(int y, int h, int w, int tiers)
        {
            this.AddBackground(0, y, w, h, 9270);
            this.AddImageTiled(10, y + 10, w - 20, h - 20, 2624); 
            this.AddAlphaRegion(10, y + 10, w - 20, h - 20); 
            for (int i = tiers, j = 1; i > 0; i--, j++)
            {
                this.AddBackground(j * 5, y + 37, ((i - 1) * 5) + 278, 42, 9270);
                if (i == 1)
                {
                    this.AddImageTiled((j * 5) + 10, y + 47, ((i - 1) * 5) + 258, 22, 2624); 
                    this.AddAlphaRegion((j * 5) + 10, y + 47, ((i - 1) * 5) + 258, 22); 
                }
            }

            SR_Rune rune = this.RuneAcc.Runes[this.RuneAcc.PageIndex];

            this.AddItem(SR_Utilities.ItemOffsetX(rune), y + SR_Utilities.ItemOffsetY(rune) + 12, SR_Utilities.RunebookID, SR_Utilities.ItemHue(rune)); 
            this.AddLabelCropped(35, y + 12, w - 108, 20, 2100, rune.Name); 
            this.AddButton(w - 70, y + 10, 4014, 4016, 5, GumpButtonType.Reply, 0); 
            this.AddButton(w - 40, y + 10, 4017, 4019, 4, GumpButtonType.Reply, 0); 

            if (tiers > 0)
            {
                rune = this.RuneAcc.ChildRune;
                this.AddItem(SR_Utilities.ItemOffsetX(rune) + tiers * 5, y + SR_Utilities.ItemOffsetY(rune) + 12 + 37, SR_Utilities.RunebookID, SR_Utilities.ItemHue(rune)); 
                this.AddLabelCropped(35 + tiers * 5, y + 12 + 37, 170, 20, 2100, rune.Name); 
                this.AddButton(w - 70, y + 10 + 37, 4014, 4016, 7, GumpButtonType.Reply, 0); 
                this.AddButton(w - 40, y + 10 + 37, 4017, 4019, 6, GumpButtonType.Reply, 0); 
            }
            // AddButton(238, 30 + bgY + 10, 4011, 4013, 0, GumpButtonType.Reply, 0); 
        }

        public void DisplayAddNew(int y)
        { 
            this.AddBackground(0, y, 278, 42, 9270); 
            this.AddImageTiled(10, y + 10, 258, 22, 2624); 
            this.AddAlphaRegion(10, y + 10, 258, 22); 
            this.AddLabel(15, y + 10, 2100, @"New Rune"); 
            this.AddButton(80, y + 10, 4011, 4013, 1, GumpButtonType.Reply, 0); 
            this.AddButton(110, y + 10, 4029, 4031, 2, GumpButtonType.Reply, 0); 
            this.AddLabel(150, y + 10, 2100, @"New Runebook"); 
            this.AddButton(238, y + 10, 4011, 4013, 3, GumpButtonType.Reply, 0); 
        }

        public void DisplayRunes(int y, int h)
        {
            this.AddBackground(0, y, 430/*400*/, h, 9270); 
            this.AddImageTiled(10, y + 10, 410, h - 20, 2624); 
            this.AddAlphaRegion(10, y + 10, 410, h - 20); 

            List<SR_Rune> runes = null;
            int count, runebooks;

            if (this.RuneAcc.ChildRune == null)
            {
                runes = this.RuneAcc.Runes;
                count = this.RuneAcc.Count;
                runebooks = this.RuneAcc.RunebookCount;
            }
            else
            {
                runes = this.RuneAcc.ChildRune.Runes;
                count = this.RuneAcc.ChildRune.Count;
                runebooks = this.RuneAcc.ChildRune.RunebookCount;
            }
			
            this.AddPage(1);
            int pages = (int)Math.Ceiling((double)count / 9.0), temp = 0;
            for (int i = 0, loc = 0, page = 1; i < count; i++, loc++)
            {
                temp = 10 + y + (22 + 5) * loc;

                this.AddItem(SR_Utilities.ItemOffsetX(runes[i]), 2 + SR_Utilities.ItemOffsetY(runes[i]) + temp, runes[i].IsRunebook ? SR_Utilities.RunebookID : SR_Utilities.RuneID, SR_Utilities.ItemHue(runes[i])); 
                if (runes[i].IsRunebook)
                    this.AddLabelCropped(35, 2 + temp, 175, 20, 2100, String.Format("{0}. {1}", i + 1, runes[i].Name)); 
                else
                {
                    this.AddLabelCropped(35, 2 + temp, 175, 20, 2100, String.Format("{0}. {1} ({2})", i + 1 - runebooks, runes[i].Name, runes[i].TargetMap.ToString())); 
                    this.AddLabelCropped(215, 2 + temp, 110, 20, 2100, runes[i].TargetLoc.ToString()); 
                    this.AddButton(360, temp, 4008, 4010, i + 30010, GumpButtonType.Reply, 0); 
                }
                this.AddButton(330 + (runes[i].IsRunebook ? 30 : 0), temp, 4005, 4007, i + 10, GumpButtonType.Reply, 0); 
                //AddButton(340, 40 + ((22+5)*i), 4026, 4028, 0, GumpButtonType.Reply, 0); 
                //AddImage(340, 40 + ((22+5)*i), 4026, 1000); 
                this.AddButton(390, temp, 4017, 4019, i + 60010, GumpButtonType.Reply, 0); // delete

                if (pages > 1 && ((loc == 8 && i < count - 1) || i == count - 1))
                {
                    temp = 10 + y + (22 + 5) * 9;
                    // (430(bg) - 20 (buffer) - 70 (txt/buffer) - 60(buttons)) / 2 = 140
                    if (page > 1)
                        this.AddButton(140, temp, 4014, 4016, 0, GumpButtonType.Page, page - 1);
                    else
                        this.AddImage(140, temp, 4014, 1000);

                    this.AddHtml(170, 2 + temp, 90, 20, String.Format("<BASEFONT COLOR=#FFFFFF><CENTER>Page {0}/{1}", page, pages), false, false);
					
                    if (page < pages)
                        this.AddButton(260, temp, 4005, 4007, 0, GumpButtonType.Page, page + 1);
                    else
                        this.AddImage(260, temp, 4005, 1000);

                    page++;
                    this.AddPage(page);
                    loc = -1;
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int button = info.ButtonID;
            Mobile mob = sender.Mobile;

            switch( button )
            {
                case 0:
                    break;
                case 1:	
                    mob.SendMessage("Enter a description:");
                    mob.Prompt = new SR_NewRunePrompt(this.RuneAcc, mob.Location, mob.Map);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 2:
                    mob.SendMessage("Target a location to mark:");
                    mob.Target = new SR_NewRuneTarget(this.RuneAcc);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 3:
                    mob.SendMessage("Enter a description:");
                    mob.Prompt = new SR_NewRunePrompt(this.RuneAcc);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 4:
                    this.RuneAcc.RemoveRune(this.RuneAcc.PageIndex, true);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 5:
                    this.RuneAcc.ResetPageIndex();
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 6:
                    this.RuneAcc.ChildRune.ParentRune.RemoveRune(this.RuneAcc.ChildRune.ParentRune.PageIndex, true);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 7:
                    this.RuneAcc.ChildRune.ParentRune.ResetPageIndex();
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                default:
                    bool moongate = false;
                    button -= 10;
                    if (button >= 60000)
                    {
                        if (this.RuneAcc.ChildRune == null)
                            this.RuneAcc.RemoveRune(button - 60000);
                        else
                            this.RuneAcc.ChildRune.RemoveRune(button - 60000);
                        Send(mob, SR_Utilities.FetchInfo(mob.Account));
                        break;
                    }

                    if (button >= 30000)
                    {
                        button -= 30000;
                        moongate = true;
                    }
                    SR_Rune rune = null;
                    if (this.RuneAcc.ChildRune == null && button >= 0 && button < RuneAcc.Runes.Count)
                        rune = this.RuneAcc.Runes[button];
                    else if (button >= 0 && button < RuneAcc.ChildRune.Runes.Count)
                        rune = this.RuneAcc.ChildRune.Runes[button];

                    if (rune.IsRunebook)
                    {
                        if (this.RuneAcc.ChildRune == null)
                            this.RuneAcc.PageIndex = button;
                        else
                            this.RuneAcc.ChildRune.PageIndex = button;

                        Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    }
                    else
                    {
                        if (mob.Location == rune.TargetLoc && mob.Map == rune.TargetMap)
                            mob.SendMessage("You are already there.");
                        else if (!moongate)
                        { 
                            mob.PlaySound(0x1FC);
                            mob.MoveToWorld(rune.TargetLoc, rune.TargetMap);
                            mob.PlaySound(0x1FC);
                        }
                        else
                        {
                            if (SR_Utilities.FindItem(typeof(Moongate), mob.Location, mob.Map))
                                mob.SendMessage("You are standing on top of a moongate, please move.");
                            else if (SR_Utilities.FindItem(typeof(Moongate), rune.TargetLoc, rune.TargetMap))
                                mob.SendMessage("There is already a moongate there, sorry.");
                            else
                            {
                                mob.SendLocalizedMessage(501024); // You open a magical gate to another location

                                Effects.PlaySound(mob.Location, mob.Map, 0x20E);

                                SR_RuneGate firstGate = new SR_RuneGate(rune.TargetLoc, rune.TargetMap);
                                firstGate.MoveToWorld(mob.Location, mob.Map);

                                Effects.PlaySound(rune.TargetLoc, rune.TargetMap, 0x20E);

                                SR_RuneGate secondGate = new SR_RuneGate(mob.Location, mob.Map);
                                secondGate.MoveToWorld(rune.TargetLoc, rune.TargetMap);
                            }
                        }
                    }

                    break;
            }
        }
    }
}