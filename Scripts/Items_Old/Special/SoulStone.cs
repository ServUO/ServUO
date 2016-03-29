using System;
using Server.Accounting;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class SoulStone : Item, ISecurable
    {
        public override int LabelNumber
        {
            get
            {
                return 1030899;
            }
        }// soulstone

        private int m_ActiveItemID;
        private int m_InactiveItemID;

        private SecureLevel m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int ActiveItemID
        {
            get
            {
                return this.m_ActiveItemID;
            }
            set
            {
                this.m_ActiveItemID = value;

                if (!this.IsEmpty)
                    this.ItemID = this.m_ActiveItemID;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int InactiveItemID
        {
            get
            {
                return this.m_InactiveItemID;
            }
            set
            {
                this.m_InactiveItemID = value;

                if (this.IsEmpty)
                    this.ItemID = this.m_InactiveItemID;
            }
        }

        private string m_Account, m_LastUserName;
        private DateTime m_NextUse; // TODO: unused, it's here not to break serialize/deserialize

        private SkillName m_Skill;
        private double m_SkillValue;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Account
        {
            get
            {
                return this.m_Account;
            }
            set
            {
                this.m_Account = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string LastUserName
        {
            get
            {
                return this.m_LastUserName;
            }
            set
            {
                this.m_LastUserName = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get
            {
                return this.m_Skill;
            }
            set
            {
                this.m_Skill = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double SkillValue
        {
            get
            {
                return this.m_SkillValue;
            }
            set
            {
                this.m_SkillValue = value;

                if (!this.IsEmpty)
                    this.ItemID = this.m_ActiveItemID;
                else
                    this.ItemID = this.m_InactiveItemID;

                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsEmpty
        {
            get
            {
                return this.m_SkillValue <= 0.0;
            }
        }

        [Constructable]
        public SoulStone()
            : this(null)
        {
        }

        [Constructable]
        public SoulStone(string account)
            : this(account, 0x2A93, 0x2A94)
        {
        }

        public SoulStone(string account, int itemID)
            : this(account, itemID, itemID)
        {
        }

        public SoulStone(string account, int inactiveItemID, int activeItemID)
            : base(inactiveItemID)
        {
            this.Light = LightType.Circle300;
            this.LootType = LootType.Blessed;

            this.m_InactiveItemID = inactiveItemID;
            this.m_ActiveItemID = activeItemID;

            this.m_Account = account;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!this.IsEmpty)
                list.Add(1070721, "#{0}\t{1:0.0}", AosSkillBonuses.GetLabel(this.Skill), this.SkillValue); // Skill stored: ~1_skillname~ ~2_skillamount~

            string name = this.LastUserName;

            if (name == null)
                name = String.Format("#{0}", 1074235); // Unknown

            list.Add(1041602, "{0}", name); // Owner: ~1_val~
        }

        private static bool CheckCombat(Mobile m, TimeSpan time)
        {
            for (int i = 0; i < m.Aggressed.Count; ++i)
            {
                AggressorInfo info = m.Aggressed[i];

                if (DateTime.UtcNow - info.LastCombatTime < time)
                    return true;
            }

            return false;
        }

        protected virtual bool CheckUse(Mobile from)
        {
            DateTime now = DateTime.UtcNow;

            PlayerMobile pm = from as PlayerMobile;

            if (this.Deleted || !this.IsAccessibleTo(from))
            {
                return false;
            }
            else if (from.Map != this.Map || !from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return false;
            }
            else if (this.Account != null && (!(from.Account is Account) || from.Account.Username != this.Account))
            {
                from.SendLocalizedMessage(1070714); // This is an Account Bound Soulstone, and your character is not bound to it.  You cannot use this Soulstone.
                return false;
            }
            else if (CheckCombat(from, TimeSpan.FromMinutes(2.0)))
            {
                from.SendLocalizedMessage(1070727); // You must wait two minutes after engaging in combat before you can use a Soulstone.
                return false;
            }
            else if (from.Criminal)
            {
                from.SendLocalizedMessage(1070728); // You must wait two minutes after committing a criminal act before you can use a Soulstone.
                return false;
            }
            else if (from.Region.GetLogoutDelay(from) > TimeSpan.Zero)
            {
                from.SendLocalizedMessage(1070729); // In order to use your Soulstone, you must be in a safe log-out location.
                return false;
            }
            else if (!from.Alive)
            {
                from.SendLocalizedMessage(1070730); // You may not use a Soulstone while your character is dead.
                return false;
            }
            else if (Factions.Sigil.ExistsOn(from))
            {
                from.SendLocalizedMessage(1070731); // You may not use a Soulstone while your character has a faction town sigil.
                return false;
            }
            else if (from.Spell != null && from.Spell.IsCasting)
            {
                from.SendLocalizedMessage(1070733); // You may not use a Soulstone while your character is casting a spell.
                return false;
            }
            else if (from.Poisoned)
            {
                from.SendLocalizedMessage(1070734); // You may not use a Soulstone while your character is poisoned.
                return false;
            }
            else if (from.Paralyzed)
            {
                from.SendLocalizedMessage(1070735); // You may not use a Soulstone while your character is paralyzed.
                return false;
            }

            #region Scroll of Alacrity
            if (pm.AcceleratedStart > DateTime.UtcNow)
            {
                from.SendLocalizedMessage(1078115); // You may not use a soulstone while your character is under the effects of a Scroll of Alacrity.
                return false;
            }
            #endregion
            else
            {
                return true;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.CheckUse(from))
                return;

            from.CloseGump(typeof(SelectSkillGump));
            from.CloseGump(typeof(ConfirmSkillGump));
            from.CloseGump(typeof(ConfirmTransferGump));
            from.CloseGump(typeof(ConfirmRemovalGump));
            from.CloseGump(typeof(ErrorGump));

            if (this.IsEmpty)
                from.SendGump(new SelectSkillGump(this, from));
            else
                from.SendGump(new ConfirmTransferGump(this, from));
        }

        private class SelectSkillGump : Gump
        {
            private readonly SoulStone m_Stone;

            public SelectSkillGump(SoulStone stone, Mobile from)
                : base(50, 50)
            {
                this.m_Stone = stone;

                this.AddPage(0);

                this.AddBackground(0, 0, 520, 440, 0x13BE);

                this.AddImageTiled(10, 10, 500, 20, 0xA40);
                this.AddImageTiled(10, 40, 500, 360, 0xA40);
                this.AddImageTiled(10, 410, 500, 20, 0xA40);

                this.AddAlphaRegion(10, 10, 500, 420);

                this.AddHtmlLocalized(10, 12, 500, 20, 1061087, 0x7FFF, false, false); // Which skill do you wish to transfer to the Soulstone?

                this.AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

                for (int i = 0, n = 0; i < from.Skills.Length; i++)
                {
                    Skill skill = from.Skills[i];

                    if (skill.Base > 0.0)
                    {
                        int p = n % 30;

                        if (p == 0)
                        {
                            int page = n / 30;

                            if (page > 0)
                            {
                                this.AddButton(260, 380, 0xFA5, 0xFA6, 0, GumpButtonType.Page, page + 1);
                                this.AddHtmlLocalized(305, 382, 200, 20, 1011066, 0x7FFF, false, false); // Next page
                            }

                            this.AddPage(page + 1);

                            if (page > 0)
                            {
                                this.AddButton(10, 380, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page);
                                this.AddHtmlLocalized(55, 382, 200, 20, 1011067, 0x7FFF, false, false); // Previous page
                            }
                        }

                        int x = (p % 2 == 0) ? 10 : 260;
                        int y = (p / 2) * 20 + 40;

                        this.AddButton(x, y, 0xFA5, 0xFA6, i + 1, GumpButtonType.Reply, 0);
                        this.AddHtmlLocalized(x + 45, y + 2, 200, 20, AosSkillBonuses.GetLabel(skill.SkillName), 0x7FFF, false, false);

                        n++;
                    }
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0 || !this.m_Stone.IsEmpty)
                    return;

                Mobile from = sender.Mobile;

                int iSkill = info.ButtonID - 1;
                if (iSkill < 0 || iSkill >= from.Skills.Length)
                    return;

                Skill skill = from.Skills[iSkill];
                if (skill.Base <= 0.0)
                    return;

                if (!this.m_Stone.CheckUse(from))
                    return;

                from.SendGump(new ConfirmSkillGump(this.m_Stone, skill));
            }
        }

        private class ConfirmSkillGump : Gump
        {
            private readonly SoulStone m_Stone;
            private readonly Skill m_Skill;

            public ConfirmSkillGump(SoulStone stone, Skill skill)
                : base(50, 50)
            {
                this.m_Stone = stone;
                this.m_Skill = skill;

                this.AddBackground(0, 0, 520, 440, 0x13BE);

                this.AddImageTiled(10, 10, 500, 20, 0xA40);
                this.AddImageTiled(10, 40, 500, 360, 0xA40);
                this.AddImageTiled(10, 410, 500, 20, 0xA40);

                this.AddAlphaRegion(10, 10, 500, 420);

                this.AddHtmlLocalized(10, 12, 500, 20, 1070709, 0x7FFF, false, false); // <CENTER>Confirm Soulstone Transfer</CENTER>

                /* <CENTER>Soulstone</CENTER><BR>
                * You are using a Soulstone.  This powerful artifact allows you to remove skill points
                * from your character and store them in the stone for later retrieval.  In order to use
                * the stone, you must make sure your Skill Lock for the indicated skill is pointed downward.
                * Click the "Skills" button on your Paperdoll to access the Skill List, and double-check
                * your skill lock.<BR><BR>
                *
                * Once you activate the stone, all skill points in the indicated skill will be removed from
                * your character.  These skill points can later be retrieved.  IMPORTANT: When retrieving
                * skill points from a Soulstone, the Soulstone WILL REPLACE any existing skill points
                * already on your character!<BR><BR>
                *
                * This is an Account Bound Soulstone.  Skill pointsstored inside can be retrieved by any
                * character on the same account as the character who placed them into the stone.
                */
                this.AddHtmlLocalized(10, 42, 500, 110, 1061067, 0x7FFF, false, true);

                this.AddHtmlLocalized(10, 200, 390, 20, 1062297, 0x7FFF, false, false); // Skill Chosen:
                this.AddHtmlLocalized(210, 200, 390, 20, AosSkillBonuses.GetLabel(skill.SkillName), 0x7FFF, false, false);

                this.AddHtmlLocalized(10, 220, 390, 20, 1062298, 0x7FFF, false, false); // Current Value:
                this.AddLabel(210, 220, 0x481, skill.Base.ToString("0.0"));

                this.AddHtmlLocalized(10, 240, 390, 20, 1062299, 0x7FFF, false, false); // Current Cap:
                this.AddLabel(210, 240, 0x481, skill.Cap.ToString("0.0"));

                this.AddHtmlLocalized(10, 260, 390, 20, 1062300, 0x7FFF, false, false); // New Value:
                this.AddLabel(210, 260, 0x481, "0.0");

                this.AddButton(10, 360, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 362, 450, 20, 1070720, 0x7FFF, false, false); // Activate the stone.  I am ready to transfer the skill points to it.

                this.AddButton(10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 382, 450, 20, 1062279, 0x7FFF, false, false); // No, let me make another selection.

                this.AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0 || !this.m_Stone.IsEmpty)
                    return;

                Mobile from = sender.Mobile;

                if (!this.m_Stone.CheckUse(from))
                    return;

                if (info.ButtonID == 1) // Is asking for another selection
                {
                    from.SendGump(new SelectSkillGump(this.m_Stone, from));
                    return;
                }

                if (this.m_Skill.Base <= 0.0)
                    return;

                if (this.m_Skill.Lock != SkillLock.Down)
                {
                    // <CENTER>Unable to Transfer Selected Skill to Soulstone</CENTER>
                    /* You cannot transfer the selected skill to the Soulstone at this time. The selected
                    * skill may be locked or set to raise in your skill menu. Click on "Skills" in your
                    * paperdoll menu to check your raise/locked/lower settings and your total skills.
                    * Make any needed adjustments, then click "Continue". If you do not wish to transfer
                    * the selected skill at this time, click "Cancel".
                    */
                    from.SendGump(new ErrorGump(this.m_Stone, 1070710, 1070711));
                    return;
                }

                this.m_Stone.Skill = this.m_Skill.SkillName;
                this.m_Stone.SkillValue = this.m_Skill.Base;

                this.m_Skill.Base = 0.0;

                from.SendLocalizedMessage(1070712); // You have successfully transferred your skill points into the Soulstone.

                this.m_Stone.LastUserName = from.Name;

                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                Effects.PlaySound(from.Location, from.Map, 0x243);

                Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);
            }
        }

        private class ConfirmTransferGump : Gump
        {
            private readonly SoulStone m_Stone;

            public ConfirmTransferGump(SoulStone stone, Mobile from)
                : base(50, 50)
            {
                this.m_Stone = stone;

                this.AddBackground(0, 0, 520, 440, 0x13BE);

                this.AddImageTiled(10, 10, 500, 20, 0xA40);
                this.AddImageTiled(10, 40, 500, 360, 0xA40);
                this.AddImageTiled(10, 410, 500, 20, 0xA40);

                this.AddAlphaRegion(10, 10, 500, 420);

                this.AddHtmlLocalized(10, 12, 500, 20, 1070709, 0x7FFF, false, false); // <CENTER>Confirm Soulstone Transfer</CENTER>

                /* <CENTER>Soulstone</CENTER><BR>
                * You are using a Soulstone.  This powerful artifact allows you to remove skill points
                * from your character and store them in the stone for later retrieval.  In order to use
                * the stone, you must make sure your Skill Lock for the indicated skill is pointed downward.
                * Click the "Skills" button on your Paperdoll to access the Skill List, and double-check
                * your skill lock.<BR><BR>
                *
                * Once you activate the stone, all skill points in the indicated skill will be removed from
                * your character.  These skill points can later be retrieved.  IMPORTANT: When retrieving
                * skill points from a Soulstone, the Soulstone WILL REPLACE any existing skill points
                * already on your character!<BR><BR>
                *
                * This is an Account Bound Soulstone.  Skill pointsstored inside can be retrieved by any
                * character on the same account as the character who placed them into the stone.
                */
                this.AddHtmlLocalized(10, 42, 500, 110, 1061067, 0x7FFF, false, true);

                this.AddHtmlLocalized(10, 200, 390, 20, 1070718, 0x7FFF, false, false); // Skill Stored:
                this.AddHtmlLocalized(210, 200, 390, 20, AosSkillBonuses.GetLabel(stone.Skill), 0x7FFF, false, false);

                Skill fromSkill = from.Skills[stone.Skill];

                this.AddHtmlLocalized(10, 220, 390, 20, 1062298, 0x7FFF, false, false); // Current Value:
                this.AddLabel(210, 220, 0x481, fromSkill.Base.ToString("0.0"));

                this.AddHtmlLocalized(10, 240, 390, 20, 1062299, 0x7FFF, false, false); // Current Cap:
                this.AddLabel(210, 240, 0x481, fromSkill.Cap.ToString("0.0"));

                this.AddHtmlLocalized(10, 260, 390, 20, 1062300, 0x7FFF, false, false); // New Value:
                this.AddLabel(210, 260, 0x481, stone.SkillValue.ToString("0.0"));

                this.AddButton(10, 360, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 362, 450, 20, 1070719, 0x7FFF, false, false); // Activate the stone.  I am ready to retrieve the skill points from it.

                this.AddButton(10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 382, 450, 20, 1070723, 0x7FFF, false, false); // Remove all skill points from this stone and DO NOT absorb them.

                this.AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0 || this.m_Stone.IsEmpty)
                    return;

                Mobile from = sender.Mobile;

                if (!this.m_Stone.CheckUse(from))
                    return;

                if (info.ButtonID == 1) // Remove skill points
                {
                    from.SendGump(new ConfirmRemovalGump(this.m_Stone));
                    return;
                }

                SkillName skill = this.m_Stone.Skill;
                double skillValue = this.m_Stone.SkillValue;
                Skill fromSkill = from.Skills[this.m_Stone.Skill];

                /* If we have, say, 88.4 in our skill and the stone holds 100, we need
                * 11.6 free points. Also, if we're below our skillcap by, say, 8.2 points,
                * we only need 11.6 - 8.2 = 3.4 points.
                */
                int requiredAmount = (int)(skillValue * 10) - fromSkill.BaseFixedPoint - (from.SkillsCap - from.SkillsTotal);

                bool cannotAbsorb = false;

                if (fromSkill.Lock != SkillLock.Up)
                {
                    cannotAbsorb = true;
                }
                else if (requiredAmount > 0)
                {
                    int available = 0;

                    for (int i = 0; i < from.Skills.Length; ++i)
                    {
                        if (from.Skills[i].Lock != SkillLock.Down)
                            continue;

                        available += from.Skills[i].BaseFixedPoint;
                    }

                    if (requiredAmount > available)
                        cannotAbsorb = true;
                }

                if (cannotAbsorb)
                {
                    // <CENTER>Unable to Absorb Selected Skill from Soulstone</CENTER>
                    /* You cannot absorb the selected skill from the Soulstone at this time. The selected
                    * skill may be locked or set to lower in your skill menu. You may also be at your
                    * total skill cap.  Click on "Skills" in your paperdoll menu to check your
                    * raise/locked/lower settings and your total skills.  Make any needed adjustments,
                    * then click "Continue". If you do not wish to transfer the selected skill at this
                    * time, click "Cancel".
                    */
                    from.SendGump(new ErrorGump(this.m_Stone, 1070717, 1070716));
                    return;
                }

                if (skillValue > fromSkill.Cap)
                {
                    // <CENTER>Unable to Absorb Selected Skill from Soulstone</CENTER>
                    /* The amount of skill stored in this stone exceeds your individual skill cap for
                    * that skill.  In order to retrieve the skill points stored in this stone, you must
                    * obtain a Power Scroll of the appropriate type and level in order to increase your
                    * skill cap.  You cannot currently retrieve the skill points stored in this stone.
                    */
                    from.SendGump(new ErrorGump(this.m_Stone, 1070717, 1070715));
                    return;
                }

                if (fromSkill.Base >= skillValue)
                {
                    // <CENTER>Unable to Absorb Selected Skill from Soulstone</CENTER>
                    /* You cannot transfer the selected skill to the Soulstone at this time. The selected
                    * skill has a skill level higher than what is stored in the Soulstone.
                    */
                    // Wrong message?!
                    from.SendGump(new ErrorGump(this.m_Stone, 1070717, 1070802));
                    return;
                }

                #region Scroll of ALacrity
                PlayerMobile pm = from as PlayerMobile;
                if (pm.AcceleratedStart > DateTime.UtcNow)
                {
                    // <CENTER>Unable to Absorb Selected Skill from Soulstone</CENTER>
                    /*You may not use a soulstone while your character is under the effects of a Scroll of Alacrity.*/
                    // Wrong message?!
                    from.SendGump(new ErrorGump(this.m_Stone, 1070717, 1078115));
                    return;
                }
                #endregion

                if (requiredAmount > 0)
                {
                    for (int i = 0; i < from.Skills.Length; ++i)
                    {
                        if (from.Skills[i].Lock != SkillLock.Down)
                            continue;

                        if (requiredAmount >= from.Skills[i].BaseFixedPoint)
                        {
                            requiredAmount -= from.Skills[i].BaseFixedPoint;
                            from.Skills[i].Base = 0.0;
                        }
                        else
                        {
                            from.Skills[i].BaseFixedPoint -= requiredAmount;
                            break;
                        }
                    }
                }

                fromSkill.Base = skillValue;
                this.m_Stone.SkillValue = 0.0;

                from.SendLocalizedMessage(1070713); // You have successfully absorbed the Soulstone's skill points.

                this.m_Stone.LastUserName = from.Name;

                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                Effects.PlaySound(from.Location, from.Map, 0x243);

                Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                if (this.m_Stone is SoulstoneFragment)
                {
                    SoulstoneFragment frag = this.m_Stone as SoulstoneFragment;

                    if (--frag.UsesRemaining <= 0)
                        from.SendLocalizedMessage(1070974); // You have used up your soulstone fragment.
                }
            }
        }

        private class ConfirmRemovalGump : Gump
        {
            private readonly SoulStone m_Stone;

            public ConfirmRemovalGump(SoulStone stone)
                : base(50, 50)
            {
                this.m_Stone = stone;

                this.AddBackground(0, 0, 520, 440, 0x13BE);

                this.AddImageTiled(10, 10, 500, 20, 0xA40);
                this.AddImageTiled(10, 40, 500, 360, 0xA40);
                this.AddImageTiled(10, 410, 500, 20, 0xA40);

                this.AddAlphaRegion(10, 10, 500, 420);

                this.AddHtmlLocalized(10, 12, 500, 20, 1070725, 0x7FFF, false, false); // <CENTER>Confirm Soulstone Skill Removal</CENTER>

                /* WARNING!<BR><BR>
                *
                * You are about to permanently remove all skill points stored in this Soulstone.
                * You WILL NOT absorb these skill points.  They will be DELETED.<BR><BR>
                *
                * Are you sure you wish to do this?  If not, press the Cancel button.
                */
                this.AddHtmlLocalized(10, 42, 500, 110, 1070724, 0x7FFF, false, true);

                this.AddButton(10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 382, 450, 20, 1052072, 0x7FFF, false, false); // Continue

                this.AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0 || this.m_Stone.IsEmpty)
                    return;

                Mobile from = sender.Mobile;

                if (!this.m_Stone.CheckUse(from))
                    return;

                this.m_Stone.SkillValue = 0.0;
                from.SendLocalizedMessage(1070726); // You have successfully deleted the Soulstone's skill points.
            }
        }

        private class ErrorGump : Gump
        {
            private readonly SoulStone m_Stone;

            public ErrorGump(SoulStone stone, int title, int message)
                : base(50, 50)
            {
                this.m_Stone = stone;

                this.AddBackground(0, 0, 520, 440, 0x13BE);

                this.AddImageTiled(10, 10, 500, 20, 0xA40);
                this.AddImageTiled(10, 40, 500, 360, 0xA40);
                this.AddImageTiled(10, 410, 500, 20, 0xA40);

                this.AddAlphaRegion(10, 10, 500, 420);

                this.AddHtmlLocalized(10, 12, 500, 20, title, 0x7FFF, false, false);

                this.AddHtmlLocalized(10, 42, 500, 110, message, 0x7FFF, false, true);

                this.AddButton(10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 382, 450, 20, 1052072, 0x7FFF, false, false); // Continue

                this.AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0)
                    return;

                Mobile from = sender.Mobile;

                if (!this.m_Stone.CheckUse(from))
                    return;

                if (this.m_Stone.IsEmpty)
                    from.SendGump(new SelectSkillGump(this.m_Stone, from));
                else
                    from.SendGump(new ConfirmTransferGump(this.m_Stone, from));
            }
        }

        public SoulStone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(3); // version

            //version 3
            writer.Write((string)this.m_LastUserName);

            //version 2
            writer.Write((int)this.m_Level);

            writer.Write(this.m_ActiveItemID);
            writer.Write(this.m_InactiveItemID);

            writer.Write((string)this.m_Account);
            writer.Write((DateTime)this.m_NextUse); //TODO: delete it in a harmless way

            writer.WriteEncodedInt((int)this.m_Skill);
            writer.Write((double)this.m_SkillValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 3:
                    {
                        this.m_LastUserName = reader.ReadString();
                        goto case 2;
                    }
                case 2:
                    {
                        this.m_Level = (SecureLevel)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_ActiveItemID = reader.ReadInt();
                        this.m_InactiveItemID = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Account = reader.ReadString();
                        this.m_NextUse = reader.ReadDateTime(); //TODO: delete it in a harmless way

                        this.m_Skill = (SkillName)reader.ReadEncodedInt();
                        this.m_SkillValue = reader.ReadDouble();
                        break;
                    }
            }

            if (version == 0)
            {
                this.m_ActiveItemID = 0x2A94;
                this.m_InactiveItemID = 0x2A93;
            }
        }
    }

    public class SoulstoneFragment : SoulStone, IUsesRemaining
    {
        private int m_UsesRemaining;

        public override int LabelNumber
        {
            get
            {
                return 1071000;
            }
        }// soulstone fragment

        [Constructable]
        public SoulstoneFragment()
            : this(5, null)
        {
        }

        [Constructable]
        public SoulstoneFragment(int usesRemaining)
            : this(usesRemaining, null)
        {
        }

        [Constructable]
        public SoulstoneFragment(string account)
            : this(5, account)
        {
        }

        [Constructable]
        public SoulstoneFragment(int usesRemaining, string account)
            : base(account, Utility.Random(0x2AA1, 9))
        {
            this.m_UsesRemaining = usesRemaining;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(2); // version

            writer.WriteEncodedInt(this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_UsesRemaining = reader.ReadEncodedInt();

            if (version <= 1)
            {
                if (this.ItemID == 0x2A93 || this.ItemID == 0x2A94)
                {
                    this.ActiveItemID = Utility.Random(0x2AA1, 9);
                }
                else
                {
                    this.ActiveItemID = this.ItemID;
                }

                this.InactiveItemID = this.ActiveItemID;
            }

            if (version == 0 && this.Weight == 1)
                this.Weight = -1;
        }

        public SoulstoneFragment(Serial serial)
            : base(serial)
        {
        }

        protected override bool CheckUse(Mobile from)
        {
            bool canUse = base.CheckUse(from);

            if (canUse)
            {
                if (this.m_UsesRemaining <= 0)
                {
                    from.SendLocalizedMessage(1070975); // That soulstone fragment has no more uses.
                    return false;
                }
            }

            return canUse;
        }

        public bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
    }

    [Flipable]
    public class BlueSoulstone : SoulStone
    {
        [Constructable]
        public BlueSoulstone()
            : this(null)
        {
        }

        [Constructable]
        public BlueSoulstone(string account)
            : base(account, 0x2ADC, 0x2ADD)
        {
        }

        public BlueSoulstone(Serial serial)
            : base(serial)
        {
        }

        public void Flip()
        {
            switch( this.ItemID )
            {
                case 0x2ADC:
                    this.ItemID = 0x2AEC;
                    break;
                case 0x2ADD:
                    this.ItemID = 0x2AED;
                    break;
                case 0x2AEC:
                    this.ItemID = 0x2ADC;
                    break;
                case 0x2AED:
                    this.ItemID = 0x2ADD;
                    break;
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

    public class RedSoulstone : SoulStone, IRewardItem
    {
        [Constructable]
        public RedSoulstone()
            : this(null)
        {
        }

        [Constructable]
        public RedSoulstone(string account)
            : base(account, 0x32F3, 0x32F4)
        {
        }

        public RedSoulstone(Serial serial)
            : base(serial)
        {
        }

        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_IsRewardItem = reader.ReadBool();
                        break;
                    }
            }
        }
    }
}