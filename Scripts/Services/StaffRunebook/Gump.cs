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
        public SR_RuneAccount RuneAcc { get; set; }

        public SR_Gump(Mobile m, SR_RuneAccount runeAcc)
            : base(0, 27)
        {
            RuneAcc = runeAcc;

            int count = 0;
            if (RuneAcc.ChildRune == null)
                count = RuneAcc.Count;
            else
                count = RuneAcc.ChildRune.Count;

            int RunebooksH = 0,
            RunebooksW = 0;

            int tier = -1;
            if (RuneAcc.ChildRune != null)
                tier = RuneAcc.ChildRune.Tier;

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

            DisplayHeader();

            int labelHue = m != null && m.NetState != null && m.NetState.IsEnhancedClient ? 2101 : 2100;

            if (tier > -1)
                DisplayRunebooks(42, RunebooksH, RunebooksW, tier, labelHue);

            DisplayAddNew(42 + RunebooksH + RunesH, labelHue);
            DisplayRunes(42 + RunebooksH, RunesH, labelHue);
        }

        public static void Send(Mobile mob, SR_RuneAccount runeAcc)
        {
            mob.CloseGump(typeof(SR_Gump));
            mob.SendGump(new SR_Gump(mob, runeAcc));
        }

        public void DisplayHeader()
        {
            AddPage(0);
            AddBackground(0, 0, 210, 42, 9270); 
            AddImageTiled(10, 10, 190, 22, 2624); 
            AddAlphaRegion(10, 10, 190, 22);
            AddHtml(0, 11, 210, 20, "<CENTER><BASEFONT COLOR=#FFFFFF><BIG>Joeku's Staff Runebook</CENTER>", false, false);
        }

        public void DisplayRunebooks(int y, int h, int w, int tiers, int labelHue)
        {
            AddBackground(0, y, w, h, 9270);
            AddImageTiled(10, y + 10, w - 20, h - 20, 2624); 
            AddAlphaRegion(10, y + 10, w - 20, h - 20); 

            for (int i = tiers, j = 1; i > 0; i--, j++)
            {
                AddBackground(j * 5, y + 37, ((i - 1) * 5) + 278, 42, 9270);
                if (i == 1)
                {
                    AddImageTiled((j * 5) + 10, y + 47, ((i - 1) * 5) + 258, 22, 2624); 
                    AddAlphaRegion((j * 5) + 10, y + 47, ((i - 1) * 5) + 258, 22); 
                }
            }

            SR_Rune rune = RuneAcc.Runes[RuneAcc.PageIndex];

            AddItem(SR_Utilities.ItemOffsetX(rune), y + SR_Utilities.ItemOffsetY(rune) + 12, SR_Utilities.RunebookID, SR_Utilities.ItemHue(rune));
            AddLabelCropped(35, y + 12, w - 108, 20, labelHue, rune.Name); 
            AddButton(w - 70, y + 10, 4014, 4016, 5, GumpButtonType.Reply, 0); 
            AddButton(w - 40, y + 10, 4017, 4019, 4, GumpButtonType.Reply, 0); 

            if (tiers > 0)
            {
                rune = RuneAcc.ChildRune;
                AddItem(SR_Utilities.ItemOffsetX(rune) + tiers * 5, y + SR_Utilities.ItemOffsetY(rune) + 12 + 37, SR_Utilities.RunebookID, SR_Utilities.ItemHue(rune));
                AddLabelCropped(35 + tiers * 5, y + 12 + 37, 170, 20, labelHue, rune.Name); 
                AddButton(w - 70, y + 10 + 37, 4014, 4016, 7, GumpButtonType.Reply, 0); 
                AddButton(w - 40, y + 10 + 37, 4017, 4019, 6, GumpButtonType.Reply, 0); 
            }
            // AddButton(238, 30 + bgY + 10, 4011, 4013, 0, GumpButtonType.Reply, 0); 
        }

        public void DisplayAddNew(int y, int labelHue)
        { 
            AddBackground(0, y, 278, 42, 9270); 
            AddImageTiled(10, y + 10, 258, 22, 2624); 
            AddAlphaRegion(10, y + 10, 258, 22);
            AddLabel(15, y + 10, labelHue, @"New Rune"); 
            AddButton(80, y + 10, 4011, 4013, 1, GumpButtonType.Reply, 0); 
            AddButton(110, y + 10, 4029, 4031, 2, GumpButtonType.Reply, 0);
            AddLabel(150, y + 10, labelHue, @"New Runebook"); 
            AddButton(238, y + 10, 4011, 4013, 3, GumpButtonType.Reply, 0); 
        }

        public void DisplayRunes(int y, int h, int labelHue)
        {
            AddBackground(0, y, 430/*400*/, h, 9270); 
            AddImageTiled(10, y + 10, 410, h - 20, 2624); 
            AddAlphaRegion(10, y + 10, 410, h - 20); 

            List<SR_Rune> runes = null;
            int count, runebooks;

            if (RuneAcc.ChildRune == null)
            {
                runes = RuneAcc.Runes;
                count = RuneAcc.Count;
                runebooks = RuneAcc.RunebookCount;
            }
            else
            {
                runes = RuneAcc.ChildRune.Runes;
                count = RuneAcc.ChildRune.Count;
                runebooks = RuneAcc.ChildRune.RunebookCount;
            }
			
            AddPage(1);
            int pages = (int)Math.Ceiling((double)count / 9.0), temp = 0;
            for (int i = 0, loc = 0, page = 1; i < count; i++, loc++)
            {
                temp = 10 + y + (22 + 5) * loc;

                AddItem(SR_Utilities.ItemOffsetX(runes[i]), 2 + SR_Utilities.ItemOffsetY(runes[i]) + temp, runes[i].IsRunebook ? SR_Utilities.RunebookID : SR_Utilities.RuneID, SR_Utilities.ItemHue(runes[i])); 
                if (runes[i].IsRunebook)
                    AddLabelCropped(35, 2 + temp, 175, 20, labelHue, String.Format("{0}. {1}", i + 1, runes[i].Name)); 
                else
                {
                    AddLabelCropped(35, 2 + temp, 175, 20, labelHue, String.Format("{0}. {1} ({2})", i + 1 - runebooks, runes[i].Name, runes[i].TargetMap.ToString()));
                    AddLabelCropped(215, 2 + temp, 110, 20, labelHue, runes[i].TargetLoc.ToString()); 
                    AddButton(360, temp, 4008, 4010, i + 30010, GumpButtonType.Reply, 0); 
                }
                AddButton(330 + (runes[i].IsRunebook ? 30 : 0), temp, 4005, 4007, i + 10, GumpButtonType.Reply, 0); 
                //AddButton(340, 40 + ((22+5)*i), 4026, 4028, 0, GumpButtonType.Reply, 0); 
                //AddImage(340, 40 + ((22+5)*i), 4026, 1000); 
                AddButton(390, temp, 4017, 4019, i + 60010, GumpButtonType.Reply, 0); // delete

                if (pages > 1 && ((loc == 8 && i < count - 1) || i == count - 1))
                {
                    temp = 10 + y + (22 + 5) * 9;
                    // (430(bg) - 20 (buffer) - 70 (txt/buffer) - 60(buttons)) / 2 = 140
                    if (page > 1)
                        AddButton(140, temp, 4014, 4016, 0, GumpButtonType.Page, page - 1);
                    else
                        AddImage(140, temp, 4014, 1000);

                    AddHtml(170, 2 + temp, 90, 20, String.Format("<BASEFONT COLOR=#FFFFFF><CENTER>Page {0}/{1}", page, pages), false, false);
					
                    if (page < pages)
                        AddButton(260, temp, 4005, 4007, 0, GumpButtonType.Page, page + 1);
                    else
                        AddImage(260, temp, 4005, 1000);

                    page++;
                    AddPage(page);
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
                    mob.Prompt = new SR_NewRunePrompt(RuneAcc, mob.Location, mob.Map);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 2:
                    mob.SendMessage("Target a location to mark:");
                    mob.Target = new SR_NewRuneTarget(RuneAcc);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 3:
                    mob.SendMessage("Enter a description:");
                    mob.Prompt = new SR_NewRunePrompt(RuneAcc);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 4:
                    RuneAcc.RemoveRune(RuneAcc.PageIndex, true);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 5:
                    RuneAcc.ResetPageIndex();
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 6:
                    RuneAcc.ChildRune.ParentRune.RemoveRune(RuneAcc.ChildRune.ParentRune.PageIndex, true);
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                case 7:
                    RuneAcc.ChildRune.ParentRune.ResetPageIndex();
                    Send(mob, SR_Utilities.FetchInfo(mob.Account));
                    break;
                default:
                    bool moongate = false;
                    button -= 10;
                    if (button >= 60000)
                    {
                        if (RuneAcc.ChildRune == null)
                            RuneAcc.RemoveRune(button - 60000);
                        else
                            RuneAcc.ChildRune.RemoveRune(button - 60000);
                        Send(mob, SR_Utilities.FetchInfo(mob.Account));
                        break;
                    }

                    if (button >= 30000)
                    {
                        button -= 30000;
                        moongate = true;
                    }
                    SR_Rune rune = null;
                    if (RuneAcc.ChildRune == null && button >= 0 && button < RuneAcc.Runes.Count)
                        rune = RuneAcc.Runes[button];
                    else if (button >= 0 && button < RuneAcc.ChildRune.Runes.Count)
                        rune = RuneAcc.ChildRune.Runes[button];

                    if (rune.IsRunebook)
                    {
                        if (RuneAcc.ChildRune == null)
                            RuneAcc.PageIndex = button;
                        else
                            RuneAcc.ChildRune.PageIndex = button;

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