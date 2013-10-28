using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class CharacterStatueGump : Gump
    {
        private readonly Item m_Maker;
        private readonly CharacterStatue m_Statue;
        private readonly Mobile m_Owner;
        public CharacterStatueGump(Item maker, CharacterStatue statue, Mobile owner)
            : base(60, 36)
        {
            this.m_Maker = maker;
            this.m_Statue = statue;
            this.m_Owner = owner;

            if (this.m_Statue == null)
                return;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);

            this.AddBackground(0, 0, 327, 324, 0x13BE);
            this.AddImageTiled(10, 10, 307, 20, 0xA40);
            this.AddImageTiled(10, 40, 307, 244, 0xA40);
            this.AddImageTiled(10, 294, 307, 20, 0xA40);
            this.AddAlphaRegion(10, 10, 307, 304);
            this.AddHtmlLocalized(14, 12, 327, 20, 1076156, 0x7FFF, false, false); // Character Statue Maker

            // pose
            this.AddHtmlLocalized(133, 41, 120, 20, 1076168, 0x7FFF, false, false); // Choose Pose
            this.AddHtmlLocalized(133, 61, 120, 20, 1076208 + (int)this.m_Statue.Pose, 0x77E, false, false);
            this.AddButton(163, 81, 0xFA5, 0xFA7, (int)Buttons.PoseNext, GumpButtonType.Reply, 0);
            this.AddButton(133, 81, 0xFAE, 0xFB0, (int)Buttons.PosePrev, GumpButtonType.Reply, 0);

            // direction
            this.AddHtmlLocalized(133, 126, 120, 20, 1076170, 0x7FFF, false, false); // Choose Direction
            this.AddHtmlLocalized(133, 146, 120, 20, this.GetDirectionNumber(this.m_Statue.Direction), 0x77E, false, false);
            this.AddButton(163, 167, 0xFA5, 0xFA7, (int)Buttons.DirNext, GumpButtonType.Reply, 0);
            this.AddButton(133, 167, 0xFAE, 0xFB0, (int)Buttons.DirPrev, GumpButtonType.Reply, 0);

            // material
            this.AddHtmlLocalized(133, 211, 120, 20, 1076171, 0x7FFF, false, false); // Choose Material
            this.AddHtmlLocalized(133, 231, 120, 20, this.GetMaterialNumber(this.m_Statue.StatueType, this.m_Statue.Material), 0x77E, false, false);
            this.AddButton(163, 253, 0xFA5, 0xFA7, (int)Buttons.MatNext, GumpButtonType.Reply, 0);
            this.AddButton(133, 253, 0xFAE, 0xFB0, (int)Buttons.MatPrev, GumpButtonType.Reply, 0);

            // cancel
            this.AddButton(10, 294, 0xFB1, 0xFB2, (int)Buttons.Close, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 294, 80, 20, 1006045, 0x7FFF, false, false);	// Cancel

            // sculpt
            this.AddButton(234, 294, 0xFB7, 0xFB9, (int)Buttons.Sculpt, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(269, 294, 80, 20, 1076174, 0x7FFF, false, false); // Sculpt

            // restore
            if (this.m_Maker is CharacterStatueDeed)
            {
                this.AddButton(107, 294, 0xFAB, 0xFAD, (int)Buttons.Restore, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(142, 294, 80, 20, 1076193, 0x7FFF, false, false); // Restore
            }
        }

        private enum Buttons
        {
            Close,
            Sculpt,
            PosePrev,
            PoseNext,
            DirPrev,
            DirNext,
            MatPrev,
            MatNext,
            Restore
        }
        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (this.m_Statue == null || this.m_Statue.Deleted)
                return;

            bool sendGump = false;

            if (info.ButtonID == (int)Buttons.Sculpt)
            {
                if (this.m_Maker is CharacterStatueDeed)
                {
                    CharacterStatue backup = ((CharacterStatueDeed)this.m_Maker).Statue;

                    if (backup != null)
                        backup.Delete();
                }

                if (this.m_Maker != null)
                    this.m_Maker.Delete();

                this.m_Statue.Sculpt(state.Mobile);
            }
            else if (info.ButtonID == (int)Buttons.PosePrev)
            {
                this.m_Statue.Pose = (StatuePose)(((int)this.m_Statue.Pose + 5) % 6);
                sendGump = true;
            }
            else if (info.ButtonID == (int)Buttons.PoseNext)
            {
                this.m_Statue.Pose = (StatuePose)(((int)this.m_Statue.Pose + 1) % 6);
                sendGump = true;
            }
            else if (info.ButtonID == (int)Buttons.DirPrev)
            {
                this.m_Statue.Direction = (Direction)(((int)this.m_Statue.Direction + 7) % 8);
                this.m_Statue.InvalidatePose();
                sendGump = true;
            }
            else if (info.ButtonID == (int)Buttons.DirNext)
            {
                this.m_Statue.Direction = (Direction)(((int)this.m_Statue.Direction + 1) % 8);
                this.m_Statue.InvalidatePose();
                sendGump = true;
            }
            else if (info.ButtonID == (int)Buttons.MatPrev)
            {
                this.m_Statue.Material = (StatueMaterial)(((int)this.m_Statue.Material + 3) % 4);
                sendGump = true;
            }
            else if (info.ButtonID == (int)Buttons.MatNext)
            {
                this.m_Statue.Material = (StatueMaterial)(((int)this.m_Statue.Material + 1) % 4);
                sendGump = true;
            }
            else if (info.ButtonID == (int)Buttons.Restore)
            {
                if (this.m_Maker is CharacterStatueDeed)
                {
                    CharacterStatue backup = ((CharacterStatueDeed)this.m_Maker).Statue;

                    if (backup != null)
                        this.m_Statue.Restore(backup);
                }

                sendGump = true;
            }
            else // Close
            {
                sendGump = !this.m_Statue.Demolish(state.Mobile);
            }

            if (sendGump)
                state.Mobile.SendGump(new CharacterStatueGump(this.m_Maker, this.m_Statue, this.m_Owner));
        }

        private int GetMaterialNumber(StatueType type, StatueMaterial material)
        {
            switch ( material )
            {
                case StatueMaterial.Antique:

                    switch ( type )
                    {
                        case StatueType.Bronze:
                            return 1076187;
                        case StatueType.Jade:
                            return 1076186;
                        case StatueType.Marble:
                            return 1076182;
                    }

                    return 1076187;
                case StatueMaterial.Dark:

                    if (type == StatueType.Marble)
                        return 1076183;

                    return 1076182;
                case StatueMaterial.Medium:
                    return 1076184;
                case StatueMaterial.Light:
                    return 1076185;
                default:
                    return 1076187;
            }
        }

        private int GetDirectionNumber(Direction direction)
        {
            switch ( direction )
            {
                case Direction.North:
                    return 1075389;
                case Direction.Right:
                    return 1075388;
                case Direction.East:
                    return 1075387;
                case Direction.Down:
                    return 1076204;
                case Direction.South:
                    return 1075386;
                case Direction.Left:
                    return 1075391;
                case Direction.West:
                    return 1075390;
                case Direction.Up:
                    return 1076205;
                default:
                    return 1075386;
            }
        }
    }
}