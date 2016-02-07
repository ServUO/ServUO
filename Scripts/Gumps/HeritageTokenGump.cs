using System;
using System.Collections.Generic;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class HeritageTokenGump : Gump
    {
        private readonly HeritageToken m_Token;
		private readonly Mobile m_User;
        public HeritageTokenGump(HeritageToken token, Mobile from)
            : base(60, 36)
        {
            this.m_Token = token;
			this.m_User = from;

            this.AddPage(0);

            this.AddBackground(0, 0, 520, 404, 0x13BE);
            this.AddImageTiled(10, 10, 500, 20, 0xA40);
            this.AddImageTiled(10, 40, 500, 324, 0xA40);
            this.AddImageTiled(10, 374, 500, 20, 0xA40);
            this.AddAlphaRegion(10, 10, 500, 384);
            this.AddButton(10, 374, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 376, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            this.AddHtmlLocalized(14, 12, 500, 20, 1075576, 0x7FFF, false, false); // Choose your item from the following pages

            this.AddPage(1);

            this.AddImageTiledButton(14, 44, 0x918, 0x919, 0x64, GumpButtonType.Reply, 0, 0x1411, 0x2C, 18, 8);
            this.AddTooltip(1062912);
            this.AddHtmlLocalized(98, 44, 250, 60, 1078147, 0x7FFF, false, false); // Royal Leggings of Embers
            this.AddImageTiledButton(264, 44, 0x918, 0x919, 0x65, GumpButtonType.Reply, 0, 0x234D, 0x0, 18, 12);
            this.AddTooltip(1062914);
            this.AddHtmlLocalized(348, 44, 250, 60, 1062913, 0x7FFF, false, false); // Rose of Trinsic
            this.AddImageTiledButton(14, 108, 0x918, 0x919, 0x66, GumpButtonType.Reply, 0, 0x26C3, 0x504, 18, 8);
            this.AddTooltip(1062916);
            this.AddHtmlLocalized(98, 108, 250, 60, 1062915, 0x7FFF, false, false); // Shamino’s Best Crossbow
            this.AddImageTiledButton(264, 108, 0x918, 0x919, 0x67, GumpButtonType.Reply, 0, 0x3F1D, 0x0, 18, 8);
            this.AddTooltip(1062918);
            this.AddHtmlLocalized(348, 108, 250, 60, 1062917, 0x7FFF, false, false); // The Tapestry of Sosaria
            this.AddImageTiledButton(14, 172, 0x918, 0x919, 0x68, GumpButtonType.Reply, 0, 0x3F14, 0x0, 18, 8);
            this.AddTooltip(1062920);
            this.AddHtmlLocalized(98, 172, 250, 60, 1062919, 0x7FFF, false, false); // Hearth of the Home Fire
            this.AddImageTiledButton(264, 172, 0x918, 0x919, 0x69, GumpButtonType.Reply, 0, 0xF60, 0x482, -1, 10);
            this.AddTooltip(1062922);
            this.AddHtmlLocalized(348, 172, 250, 60, 1062921, 0x7FFF, false, false); // The Holy Sword
            this.AddImageTiledButton(14, 236, 0x918, 0x919, 0x6A, GumpButtonType.Reply, 0, 0x236C, 0x0, 18, 6);
            this.AddTooltip(1062924);
            this.AddHtmlLocalized(98, 236, 250, 60, 1062923, 0x7FFF, false, false); // Ancient Samurai Helm
            this.AddImageTiledButton(264, 236, 0x918, 0x919, 0x6B, GumpButtonType.Reply, 0, 0x2B10, 0x226, 18, 11);
            this.AddTooltip(1075223);
            this.AddHtmlLocalized(348, 236, 250, 60, 1075188, 0x7FFF, false, false); // Helm of Spirituality
            this.AddImageTiledButton(14, 300, 0x918, 0x919, 0x6C, GumpButtonType.Reply, 0, 0x2B0C, 0x226, 18, 15);
            this.AddTooltip(1075224);
            this.AddHtmlLocalized(98, 300, 250, 60, 1075192, 0x7FFF, false, false); // Gauntlets of Valor
            this.AddImageTiledButton(264, 300, 0x918, 0x919, 0x6D, GumpButtonType.Reply, 0, 0x2B01, 0x0, 18, 9);
            this.AddTooltip(1075225);
            this.AddHtmlLocalized(348, 300, 250, 60, 1075196, 0x7FFF, false, false); // Dupre’s Shield
            this.AddButton(400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 2);
            this.AddHtmlLocalized(440, 376, 60, 20, 1043353, 0x7FFF, false, false); // Next

            this.AddPage(2);

            this.AddButton(300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);
            this.AddHtmlLocalized(340, 376, 60, 20, 1011393, 0x7FFF, false, false); // Back
            this.AddImageTiledButton(14, 44, 0x918, 0x919, 0x6E, GumpButtonType.Reply, 0, 0x2AC6, 0x0, 29, 0);
            this.AddTooltip(1075226);
            this.AddHtmlLocalized(98, 44, 250, 60, 1075197, 0x7FFF, false, false); // Fountain of Life
            this.AddImageTiledButton(264, 44, 0x918, 0x919, 0x6F, GumpButtonType.Reply, 0, 0x2AF9, 0x0, -4, -5);
            this.AddTooltip(1075227);
            this.AddHtmlLocalized(348, 44, 250, 60, 1075198, 0x7FFF, false, false); // Dawn’s Music Box
            this.AddImageTiledButton(14, 108, 0x918, 0x919, 0x70, GumpButtonType.Reply, 0, 0x2253, 0x0, 18, 12);
            this.AddTooltip(1075228);
            this.AddHtmlLocalized(98, 108, 250, 60, 1078148, 0x7FFF, false, false); // Ossian Grimoire
            this.AddImageTiledButton(264, 108, 0x918, 0x919, 0x71, GumpButtonType.Reply, 0, 0x2D98, 0x0, 19, 13);
            this.AddTooltip(1078527);
            this.AddHtmlLocalized(348, 108, 250, 60, 1078142, 0x7FFF, false, false); // Talisman of the Fey:<br>Ferret
            this.AddImageTiledButton(14, 172, 0x918, 0x919, 0x72, GumpButtonType.Reply, 0, 0x2D97, 0x0, 19, 13);
            this.AddTooltip(1078528);
            this.AddHtmlLocalized(98, 172, 250, 60, 1078143, 0x7FFF, false, false); // Talisman of the Fey:<br>Squirrel
            this.AddImageTiledButton(264, 172, 0x918, 0x919, 0x73, GumpButtonType.Reply, 0, 0x2D96, 0x0, 19, 8);
            this.AddTooltip(1078529);
            this.AddHtmlLocalized(348, 172, 250, 60, 1078144, 0x7FFF, false, false); // Talisman of the Fey:<br>Cu Sidhe
            this.AddImageTiledButton(14, 236, 0x918, 0x919, 0x74, GumpButtonType.Reply, 0, 0x2D95, 0x0, -4, 2);
            this.AddTooltip(1078530);
            this.AddHtmlLocalized(98, 236, 250, 60, 1078145, 0x7FFF, false, false); // Talisman of the Fey:<br>Reptalon
            this.AddImageTiledButton(264, 236, 0x918, 0x919, 0x75, GumpButtonType.Reply, 0, 0x2B02, 0x0, -2, 9);
            this.AddTooltip(1078526);
            this.AddHtmlLocalized(348, 236, 250, 60, 1075201, 0x7FFF, false, false); // Quiver of Infinity
            this.AddImageTiledButton(14, 300, 0x918, 0x919, 0x76, GumpButtonType.Reply, 0, 0x2A91, 0x0, 25, 5);
            this.AddTooltip(1075986);
            this.AddHtmlLocalized(98, 300, 250, 60, 1074797, 0x7FFF, false, false); // Bone Throne, Bone Couch<br>and Bone Table
            this.AddImageTiledButton(264, 300, 0x918, 0x919, 0x77, GumpButtonType.Reply, 0, 0x2A99, 0x0, 18, 1);
            this.AddTooltip(1075987);
            this.AddHtmlLocalized(348, 300, 250, 60, 1078146, 0x7FFF, false, false); // Creepy Portraits
            this.AddButton(400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 3);
            this.AddHtmlLocalized(440, 376, 60, 20, 1043353, 0x7FFF, false, false); // Next

            this.AddPage(3);

            this.AddButton(300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 2);
            this.AddHtmlLocalized(340, 376, 60, 20, 1011393, 0x7FFF, false, false); // Back
            this.AddImageTiledButton(14, 44, 0x918, 0x919, 0x78, GumpButtonType.Reply, 0, 0x2A71, 0x0, 13, 5);
            this.AddTooltip(1075988);
            this.AddHtmlLocalized(98, 44, 250, 60, 1074799, 0x7FFF, false, false); // Mounted Pixies (5)
            this.AddImageTiledButton(264, 44, 0x918, 0x919, 0x79, GumpButtonType.Reply, 0, 0x2A98, 0x0, 26, 1);
            this.AddTooltip(1075990);
            this.AddHtmlLocalized(348, 44, 250, 60, 1074800, 0x7FFF, false, false); // Haunted Mirror
            this.AddImageTiledButton(14, 108, 0x918, 0x919, 0x7A, GumpButtonType.Reply, 0, 0x2A92, 0x0, 18, 1);
            this.AddTooltip(1075989);
            this.AddHtmlLocalized(98, 108, 250, 60, 1074801, 0x7FFF, false, false); // Bed of Nails
            this.AddImageTiledButton(264, 108, 0x918, 0x919, 0x7B, GumpButtonType.Reply, 0, 0x2AB8, 0x0, 18, 1);
            this.AddTooltip(1075991);
            this.AddHtmlLocalized(348, 108, 250, 60, 1074818, 0x7FFF, false, false); // Sacrificial Altar
            this.AddImageTiledButton(14, 172, 0x918, 0x919, 0x7C, GumpButtonType.Reply, 0, 0x3F26, 0x0, 18, 8);
            this.AddTooltip(1076610);
            this.AddHtmlLocalized(98, 172, 250, 60, 1076257, 0x7FFF, false, false); // Broken Covered Chair
            this.AddImageTiledButton(264, 172, 0x918, 0x919, 0x7D, GumpButtonType.Reply, 0, 0x3F22, 0x0, 18, 8);
            this.AddTooltip(1076610);
            this.AddHtmlLocalized(348, 172, 250, 60, 1076258, 0x7FFF, false, false); // Broken Bookcase
            this.AddImageTiledButton(14, 236, 0x918, 0x919, 0x7E, GumpButtonType.Reply, 0, 0x3F24, 0x0, 18, 8);
            this.AddTooltip(1076610);
            this.AddHtmlLocalized(98, 236, 250, 60, 1076259, 0x7FFF, false, false); // Standing Broken Chair
            this.AddImageTiledButton(264, 236, 0x918, 0x919, 0x7F, GumpButtonType.Reply, 0, 0x3F25, 0x0, 18, 8);
            this.AddTooltip(1076610);
            this.AddHtmlLocalized(348, 236, 250, 60, 1076260, 0x7FFF, false, false); // Broken Vanity
            this.AddImageTiledButton(14, 300, 0x918, 0x919, 0x80, GumpButtonType.Reply, 0, 0x3F23, 0x0, 18, 8);
            this.AddTooltip(1076610);
            this.AddHtmlLocalized(98, 300, 250, 60, 1076261, 0x7FFF, false, false); // Broken Chest of Drawers
            this.AddImageTiledButton(264, 300, 0x918, 0x919, 0x81, GumpButtonType.Reply, 0, 0x3F21, 0x0, 18, 8);
            this.AddTooltip(1076610);
            this.AddHtmlLocalized(348, 300, 250, 60, 1076262, 0x7FFF, false, false); // Broken Armoire
            this.AddButton(400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 4);
            this.AddHtmlLocalized(440, 376, 60, 20, 1043353, 0x7FFF, false, false); // Next

            this.AddPage(4);

            this.AddButton(300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 3);
            this.AddHtmlLocalized(340, 376, 60, 20, 1011393, 0x7FFF, false, false); // Back
            this.AddImageTiledButton(14, 44, 0x918, 0x919, 0x82, GumpButtonType.Reply, 0, 0x3F0B, 0x0, 18, 8);
            this.AddTooltip(1076610);
            this.AddHtmlLocalized(98, 44, 250, 60, 1076263, 0x7FFF, false, false); // Broken Bed
            this.AddImageTiledButton(264, 44, 0x918, 0x919, 0x83, GumpButtonType.Reply, 0, 0xC19, 0x0, 13, 8);
            this.AddTooltip(1076610);
            this.AddHtmlLocalized(348, 44, 250, 60, 1076264, 0x7FFF, false, false); // Broken Fallen Chair
            this.AddImageTiledButton(14, 108, 0x918, 0x919, 0x84, GumpButtonType.Reply, 0, 0x3DAA, 0x0, 20, -3);
            this.AddTooltip(1076611);
            this.AddHtmlLocalized(98, 108, 250, 60, 1076265, 0x7FFF, false, false); // Suit of Gold Armor
            this.AddImageTiledButton(264, 108, 0x918, 0x919, 0x85, GumpButtonType.Reply, 0, 0x151C, 0x0, -20, -3);
            this.AddTooltip(1076612);
            this.AddHtmlLocalized(348, 108, 250, 60, 1076266, 0x7FFF, false, false); // Suit of Silver Armor
            this.AddImageTiledButton(14, 172, 0x918, 0x919, 0x86, GumpButtonType.Reply, 0, 0x3DB1, 0x0, 18, 8);
            this.AddTooltip(1076613);
            this.AddHtmlLocalized(98, 172, 250, 60, 1076267, 0x7FFF, false, false); // Boiling Cauldron
            this.AddImageTiledButton(264, 172, 0x918, 0x919, 0x87, GumpButtonType.Reply, 0, 0x3F27, 0x0, 18, 8);
            this.AddTooltip(1076614);
            this.AddHtmlLocalized(348, 172, 250, 60, 1024656, 0x7FFF, false, false); // Guillotine
            this.AddImageTiledButton(14, 236, 0x918, 0x919, 0x88, GumpButtonType.Reply, 0, 0x3F0C, 0x0, 18, 8);
            this.AddTooltip(1076615);
            this.AddHtmlLocalized(98, 236, 250, 60, 1076268, 0x7FFF, false, false); // Cherry Blossom Tree
            this.AddImageTiledButton(264, 236, 0x918, 0x919, 0x89, GumpButtonType.Reply, 0, 0x3F07, 0x0, 18, 8);
            this.AddTooltip(1076616);
            this.AddHtmlLocalized(348, 236, 250, 60, 1076269, 0x7FFF, false, false); // Apple Tree
            this.AddImageTiledButton(14, 300, 0x918, 0x919, 0x8A, GumpButtonType.Reply, 0, 0x3F16, 0x0, 18, 8);
            this.AddTooltip(1076617);
            this.AddHtmlLocalized(98, 300, 250, 60, 1076270, 0x7FFF, false, false); // Peach Tree
            this.AddImageTiledButton(264, 300, 0x918, 0x919, 0x8B, GumpButtonType.Reply, 0, 0x3F12, 0x0, 18, 8);
            this.AddTooltip(1076618);
            this.AddHtmlLocalized(348, 300, 250, 60, 1076271, 0x7FFF, false, false); // Hanging Axes
            this.AddButton(400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 5);
            this.AddHtmlLocalized(440, 376, 60, 20, 1043353, 0x7FFF, false, false); // Next

            this.AddPage(5);

            this.AddButton(300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 4);
            this.AddHtmlLocalized(340, 376, 60, 20, 1011393, 0x7FFF, false, false); // Back
            this.AddImageTiledButton(14, 44, 0x918, 0x919, 0x8C, GumpButtonType.Reply, 0, 0x3F13, 0x0, 18, 8);
            this.AddTooltip(1076619);
            this.AddHtmlLocalized(98, 44, 250, 60, 1076272, 0x7FFF, false, false); // Hanging Swords
            this.AddImageTiledButton(264, 44, 0x918, 0x919, 0x8D, GumpButtonType.Reply, 0, 0x3F09, 0x0, 18, 8);
            this.AddTooltip(1076620);
            this.AddHtmlLocalized(348, 44, 250, 60, 1076273, 0x7FFF, false, false); // Blue fancy rug
            this.AddImageTiledButton(14, 108, 0x918, 0x919, 0x8E, GumpButtonType.Reply, 0, 0x3F0E, 0x0, 18, 8);
            this.AddTooltip(1076621);
            this.AddHtmlLocalized(98, 108, 250, 60, 1076274, 0x7FFF, false, false); // Coffin
            this.AddImageTiledButton(264, 108, 0x918, 0x919, 0x8F, GumpButtonType.Reply, 0, 0x3F1F, 0x0, 18, 8);
            this.AddTooltip(1076623);
            this.AddHtmlLocalized(348, 108, 250, 60, 1074027, 0x7FFF, false, false); // Vanity
            this.AddImageTiledButton(14, 172, 0x918, 0x919, 0x90, GumpButtonType.Reply, 0, 0x118B, 0x0, -4, -9);
            this.AddTooltip(1076624);
            this.AddHtmlLocalized(98, 172, 250, 60, 1076635, 0x7FFF, false, false); // Table With A Purple<br>Tablecloth
            this.AddImageTiledButton(264, 172, 0x918, 0x919, 0x91, GumpButtonType.Reply, 0, 0x118C, 0x0, -4, -9);
            this.AddTooltip(1076624);
            this.AddHtmlLocalized(348, 172, 250, 60, 1076636, 0x7FFF, false, false); // Table With A Blue<br>Tablecloth
            this.AddImageTiledButton(14, 236, 0x918, 0x919, 0x92, GumpButtonType.Reply, 0, 0x118D, 0x0, -4, -9);
            this.AddTooltip(1076624);
            this.AddHtmlLocalized(98, 236, 250, 60, 1076637, 0x7FFF, false, false); // Table With A Red<br>Tablecloth
            this.AddImageTiledButton(264, 236, 0x918, 0x919, 0x93, GumpButtonType.Reply, 0, 0x118E, 0x0, -4, -9);
            this.AddTooltip(1076624);
            this.AddHtmlLocalized(348, 236, 250, 60, 1076638, 0x7FFF, false, false); // Table With An Orange<br>Tablecloth
            this.AddImageTiledButton(14, 300, 0x918, 0x919, 0x94, GumpButtonType.Reply, 0, 0x3F1E, 0x0, 18, 8);
            this.AddTooltip(1076625);
            this.AddHtmlLocalized(98, 300, 250, 60, 1076279, 0x7FFF, false, false); // Unmade Bed
            this.AddImageTiledButton(264, 300, 0x918, 0x919, 0x95, GumpButtonType.Reply, 0, 0x3F0F, 0x0, 18, 8);
            this.AddTooltip(1076626);
            this.AddHtmlLocalized(348, 300, 250, 60, 1076280, 0x7FFF, false, false); // Curtains
            this.AddButton(400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 6);
            this.AddHtmlLocalized(440, 376, 60, 20, 1043353, 0x7FFF, false, false); // Next

            this.AddPage(6);

            this.AddButton(300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 5);
            this.AddHtmlLocalized(340, 376, 60, 20, 1011393, 0x7FFF, false, false); // Back
            this.AddImageTiledButton(14, 44, 0x918, 0x919, 0x96, GumpButtonType.Reply, 0, 0x1E34, 0x0, 18, -17);
            this.AddTooltip(1076627);
            this.AddHtmlLocalized(98, 44, 250, 60, 1076281, 0x7FFF, false, false); // Scarecrow
            this.AddImageTiledButton(264, 44, 0x918, 0x919, 0x97, GumpButtonType.Reply, 0, 0xA0C, 0x0, 18, 8);
            this.AddTooltip(1076628);
            this.AddHtmlLocalized(348, 44, 250, 60, 1076282, 0x7FFF, false, false); // Wall Torch
            this.AddImageTiledButton(14, 108, 0x918, 0x919, 0x98, GumpButtonType.Reply, 0, 0x3F10, 0x0, 18, 9);
            this.AddTooltip(1076629);
            this.AddHtmlLocalized(98, 108, 250, 60, 1076283, 0x7FFF, false, false); // Fountain
            this.AddImageTiledButton(264, 108, 0x918, 0x919, 0x99, GumpButtonType.Reply, 0, 0x3F19, 0x0, 18, 8);
            this.AddTooltip(1076630);
            this.AddHtmlLocalized(348, 108, 250, 60, 1076284, 0x7FFF, false, false); // Statue
            this.AddImageTiledButton(14, 172, 0x918, 0x919, 0x9A, GumpButtonType.Reply, 0, 0x1EA5, 0x0, 5, -25);
            this.AddTooltip(1076631);
            this.AddHtmlLocalized(98, 172, 250, 60, 1076285, 0x7FFF, false, false); // Large Fish Net
            this.AddImageTiledButton(264, 172, 0x918, 0x919, 0x9B, GumpButtonType.Reply, 0, 0x1EA3, 0x0, 18, -27);
            this.AddTooltip(1076632);
            this.AddHtmlLocalized(348, 172, 250, 60, 1076286, 0x7FFF, false, false); // Small Fish Net
            this.AddImageTiledButton(14, 236, 0x918, 0x919, 0x9C, GumpButtonType.Reply, 0, 0x2FDF, 0x0, 18, -36);
            this.AddTooltip(1076633);
            this.AddHtmlLocalized(98, 236, 250, 60, 1076287, 0x7FFF, false, false); // Ladder
            this.AddImageTiledButton(264, 236, 0x918, 0x919, 0x9D, GumpButtonType.Reply, 0, 0x3F15, 0x0, 18, 8);
            this.AddTooltip(1076622);
            this.AddHtmlLocalized(348, 236, 250, 60, 1076288, 0x7FFF, false, false); // Iron Maiden
            this.AddImageTiledButton(14, 300, 0x918, 0x919, 0x9E, GumpButtonType.Reply, 0, 0x3F0A, 0x0, 18, 8);
            this.AddTooltip(1076620);
            this.AddHtmlLocalized(98, 300, 250, 60, 1076585, 0x7FFF, false, false); // Blue plain rug
            this.AddImageTiledButton(264, 300, 0x918, 0x919, 0x9F, GumpButtonType.Reply, 0, 0x3F11, 0x0, 18, 8);
            this.AddTooltip(1076620);
            this.AddHtmlLocalized(348, 300, 250, 60, 1076586, 0x7FFF, false, false); // Golden decorative rug
            this.AddButton(400, 374, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 7);
            this.AddHtmlLocalized(440, 376, 60, 20, 1043353, 0x7FFF, false, false); // Next

            this.AddPage(7);

            this.AddButton(300, 374, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 6);
            this.AddHtmlLocalized(340, 376, 60, 20, 1011393, 0x7FFF, false, false); // Back
            this.AddImageTiledButton(14, 44, 0x918, 0x919, 0xA0, GumpButtonType.Reply, 0, 0x3F0D, 0x0, 18, 8);
            this.AddTooltip(1076620);
            this.AddHtmlLocalized(98, 44, 250, 60, 1076587, 0x7FFF, false, false); // Cinnamon fancy rug
            this.AddImageTiledButton(264, 44, 0x918, 0x919, 0xA1, GumpButtonType.Reply, 0, 0x3F18, 0x0, 18, 8);
            this.AddTooltip(1076620);
            this.AddHtmlLocalized(348, 44, 250, 60, 1076588, 0x7FFF, false, false); // Red plain rug
            this.AddImageTiledButton(14, 108, 0x918, 0x919, 0xA2, GumpButtonType.Reply, 0, 0x3F08, 0x0, 18, 8);
            this.AddTooltip(1076620);
            this.AddHtmlLocalized(98, 108, 250, 60, 1076589, 0x7FFF, false, false); // Blue decorative rug
            this.AddImageTiledButton(264, 108, 0x918, 0x919, 0xA3, GumpButtonType.Reply, 0, 0x3F17, 0x0, 18, 8);
            this.AddTooltip(1076620);
            this.AddHtmlLocalized(348, 108, 250, 60, 1076590, 0x7FFF, false, false); // Pink fancy rug
            this.AddImageTiledButton(14, 172, 0x918, 0x919, 0xA4, GumpButtonType.Reply, 0, 0x312A, 0x0, 18, 8);
            this.AddTooltip(1076615);
            this.AddHtmlLocalized(98, 172, 250, 60, 1076784, 0x7FFF, false, false); // Cherry Blossom Trunk
            this.AddImageTiledButton(264, 172, 0x918, 0x919, 0xA5, GumpButtonType.Reply, 0, 0x3128, 0x0, 18, 8);
            this.AddTooltip(1076616);
            this.AddHtmlLocalized(348, 172, 250, 60, 1076785, 0x7FFF, false, false); // Apple Trunk
            this.AddImageTiledButton(14, 236, 0x918, 0x919, 0xA6, GumpButtonType.Reply, 0, 0x3129, 0x0, 18, 8);
            this.AddTooltip(1076617);
            this.AddHtmlLocalized(98, 236, 250, 60, 1076786, 0x7FFF, false, false); // Peach Trunk
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Token == null || this.m_Token.Deleted || info.ButtonID == 0 ||
				this.m_User == null || this.m_User.Deleted)
                return;

			if (!this.m_Token.IsChildOf(this.m_User.Backpack))
			{
				sender.Mobile.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
				return;
			}

            List<Type> types = new List<Type>();
            int cliloc = 0;

            switch ( info.ButtonID )
            {
                // 7th anniversary
                case 0x64:
                    types.Add(typeof(LeggingsOfEmbers));
                    cliloc = 1078147;
                    break;
                case 0x65:
                    types.Add(typeof(RoseOfTrinsic));
                    cliloc = 1062913;
                    break;
                case 0x66:
                    types.Add(typeof(ShaminoCrossbow));
                    cliloc = 1062915;
                    break;
                case 0x67:
                    types.Add(typeof(TapestryOfSosaria));
                    cliloc = 1062917;
                    break;
                case 0x68:
                    types.Add(typeof(HearthOfHomeFireDeed));
                    cliloc = 1062919;
                    break;
                case 0x69:
                    types.Add(typeof(HolySword));
                    cliloc = 1062921;
                    break;
                case 0x6A:
                    types.Add(typeof(SamuraiHelm));
                    cliloc = 1062923;
                    break;
                    // 8th anniversary
                    /*case 0x6B: types.Add( typeof( SpiritualityHelm ) ); cliloc = 1075188; break;
                    case 0x6C: types.Add( typeof( ValorGauntlets ) ); cliloc = 1075192; break;*/
                case 0x6D:
                    types.Add(typeof(DupresShield));
                    cliloc = 1075196;
                    break;
                case 0x6E:
                    types.Add(typeof(FountainOfLifeDeed));
                    cliloc = 1075197;
                    break;
                case 0x6F:
                    types.Add(typeof(DawnsMusicBox));
                    cliloc = 1075198;
                    break;
                case 0x70:
                    types.Add(typeof(OssianGrimoire));
                    cliloc = 1078148;
                    break;
                case 0x71:
                    types.Add(typeof(FerretFormTalisman));
                    cliloc = 1078142;
                    break;
                case 0x72:
                    types.Add(typeof(SquirrelFormTalisman));
                    cliloc = 1078143;
                    break;
                case 0x73:
                    types.Add(typeof(CuSidheFormTalisman));
                    cliloc = 1078144;
                    break;
                case 0x74:
                    types.Add(typeof(ReptalonFormTalisman));
                    cliloc = 1078145;
                    break;
                case 0x75:
                    types.Add(typeof(QuiverOfInfinity));
                    cliloc = 1075201;
                    break;
                    // evil home decor
                case 0x76:
                    types.Add(typeof(BoneThroneDeed));
                    types.Add(typeof(BoneCouchDeed));
                    types.Add(typeof(BoneTableDeed));
                    cliloc = 1074797;
                    break;
                case 0x77:
                    types.Add(typeof(CreepyPortraitDeed));
                    types.Add(typeof(DisturbingPortraitDeed));
                    types.Add(typeof(UnsettlingPortraitDeed));
                    cliloc = 1078146;
                    break;
                case 0x78:
                    types.Add(typeof(MountedPixieBlueDeed));
                    types.Add(typeof(MountedPixieGreenDeed));
                    types.Add(typeof(MountedPixieLimeDeed));
                    types.Add(typeof(MountedPixieOrangeDeed));
                    types.Add(typeof(MountedPixieWhiteDeed));
                    cliloc = 1074799;
                    break;
                case 0x79:
                    types.Add(typeof(HaunterMirrorDeed));
                    cliloc = 1074800;
                    break;
                case 0x7A:
                    types.Add(typeof(BedOfNailsDeed));
                    cliloc = 1074801;
                    break;
                case 0x7B:
                    types.Add(typeof(SacrificialAltarDeed));
                    cliloc = 1074818;
                    break;
                    // broken furniture
                case 0x7C:
                    types.Add(typeof(BrokenCoveredChairDeed));
                    cliloc = 1076257;
                    break;
                case 0x7D:
                    types.Add(typeof(BrokenBookcaseDeed));
                    cliloc = 1076258;
                    break;
                case 0x7E:
                    types.Add(typeof(StandingBrokenChairDeed));
                    cliloc = 1076259;
                    break;
                case 0x7F:
                    types.Add(typeof(BrokenVanityDeed));
                    cliloc = 1076260;
                    break;
                case 0x80:
                    types.Add(typeof(BrokenChestOfDrawersDeed));
                    cliloc = 1076261;
                    break;
                case 0x81:
                    types.Add(typeof(BrokenArmoireDeed));
                    cliloc = 1076262;
                    break;
                case 0x82:
                    types.Add(typeof(BrokenBedDeed));
                    cliloc = 1076263;
                    break;
                case 0x83:
                    types.Add(typeof(BrokenFallenChairDeed));
                    cliloc = 1076264;
                    break;
                    // other
                case 0x84:
                    types.Add(typeof(SuitOfGoldArmorDeed));
                    cliloc = 1076265;
                    break;
                case 0x85:
                    types.Add(typeof(SuitOfSilverArmorDeed));
                    cliloc = 1076266;
                    break;
                case 0x86:
                    types.Add(typeof(BoilingCauldronDeed));
                    cliloc = 1076267;
                    break;
                case 0x87:
                    types.Add(typeof(GuillotineDeed));
                    cliloc = 1024656;
                    break;
                case 0x88:
                    types.Add(typeof(CherryBlossomTreeDeed));
                    cliloc = 1076268;
                    break;
                case 0x89:
                    types.Add(typeof(AppleTreeDeed));
                    cliloc = 1076269;
                    break;
                case 0x8A:
                    types.Add(typeof(PeachTreeDeed));
                    cliloc = 1076270;
                    break;
                case 0x8B:
                    types.Add(typeof(HangingAxesDeed));
                    cliloc = 1076271;
                    break;
                case 0x8C:
                    types.Add(typeof(HangingSwordsDeed));
                    cliloc = 1076272;
                    break;
                case 0x8D:
                    types.Add(typeof(BlueFancyRugDeed));
                    cliloc = 1076273;
                    break;
                case 0x8E:
                    types.Add(typeof(WoodenCoffinDeed));
                    cliloc = 1076274;
                    break;
                case 0x8F:
                    types.Add(typeof(VanityDeed));
                    cliloc = 1074027;
                    break;
                case 0x90:
                    types.Add(typeof(TableWithPurpleClothDeed));
                    cliloc = 1076635;
                    break;
                case 0x91:
                    types.Add(typeof(TableWithBlueClothDeed));
                    cliloc = 1076636;
                    break;
                case 0x92:
                    types.Add(typeof(TableWithRedClothDeed));
                    cliloc = 1076637;
                    break;
                case 0x93:
                    types.Add(typeof(TableWithOrangeClothDeed));
                    cliloc = 1076638;
                    break;
                case 0x94:
                    types.Add(typeof(UnmadeBedDeed));
                    cliloc = 1076279;
                    break;
                case 0x95:
                    types.Add(typeof(CurtainsDeed));
                    cliloc = 1076280;
                    break;
                case 0x96:
                    types.Add(typeof(ScarecrowDeed));
                    cliloc = 1076281;
                    break;
                case 0x97:
                    types.Add(typeof(WallTorchDeed));
                    cliloc = 1076282;
                    break;
                case 0x98:
                    types.Add(typeof(FountainDeed));
                    cliloc = 1076283;
                    break;
                case 0x99:
                    types.Add(typeof(StoneStatueDeed));
                    cliloc = 1076284;
                    break;
                case 0x9A:
                    types.Add(typeof(LargeFishingNetDeed));
                    cliloc = 1076285;
                    break;
                case 0x9B:
                    types.Add(typeof(SmallFishingNetDeed));
                    cliloc = 1076286;
                    break;
                case 0x9C:
                    types.Add(typeof(HouseLadderDeed));
                    cliloc = 1076287;
                    break;
                case 0x9D:
                    types.Add(typeof(IronMaidenDeed));
                    cliloc = 1076288;
                    break;
                case 0x9E:
                    types.Add(typeof(BluePlainRugDeed));
                    cliloc = 1076585;
                    break;
                case 0x9F:
                    types.Add(typeof(GoldenDecorativeRugDeed));
                    cliloc = 1076586;
                    break;
                case 0xA0:
                    types.Add(typeof(CinnamonFancyRugDeed));
                    cliloc = 1076587;
                    break;
                case 0xA1:
                    types.Add(typeof(RedPlainRugDeed));
                    cliloc = 1076588;
                    break;
                case 0xA2:
                    types.Add(typeof(BlueDecorativeRugDeed));
                    cliloc = 1076589;
                    break;
                case 0xA3:
                    types.Add(typeof(PinkFancyRugDeed));
                    cliloc = 1076590;
                    break;
                case 0xA4:
                    types.Add(typeof(CherryBlossomTrunkDeed));
                    cliloc = 1076784;
                    break;
                case 0xA5:
                    types.Add(typeof(AppleTrunkDeed));
                    cliloc = 1076785;
                    break;
                case 0xA6:
                    types.Add(typeof(PeachTrunkDeed));
                    cliloc = 1076786;
                    break;
            }

            if (types.Count > 0 && cliloc > 0)
            {
                sender.Mobile.CloseGump(typeof(ConfirmHeritageGump));
                sender.Mobile.SendGump(new ConfirmHeritageGump(this.m_Token, types.ToArray(), cliloc, this.m_User));
            }
            else
                sender.Mobile.SendLocalizedMessage(501311); // This option is currently disabled, while we evaluate it for game balance.
        }
    }
}