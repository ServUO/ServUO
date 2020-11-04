using Server.Accounting;
using Server.Engines.Craft;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using System;
using System.Linq;


namespace Server.Items
{
    public class SoulStone : Item, ISecurable
    {
        public override int LabelNumber => 1030899;// soulstone

        private int m_ActiveItemID;
        private int m_InactiveItemID;

        private SecureLevel m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int ActiveItemID
        {
            get
            {
                return m_ActiveItemID;
            }
            set
            {
                m_ActiveItemID = value;

                if (!IsEmpty)
                    ItemID = m_ActiveItemID;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int InactiveItemID
        {
            get
            {
                return m_InactiveItemID;
            }
            set
            {
                m_InactiveItemID = value;

                if (IsEmpty)
                    ItemID = m_InactiveItemID;
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
                return m_Account;
            }
            set
            {
                m_Account = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string LastUserName
        {
            get
            {
                return m_LastUserName;
            }
            set
            {
                m_LastUserName = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get
            {
                return m_Skill;
            }
            set
            {
                m_Skill = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double SkillValue
        {
            get
            {
                return m_SkillValue;
            }
            set
            {
                m_SkillValue = value;

                if (!IsEmpty)
                    ItemID = m_ActiveItemID;
                else
                    ItemID = m_InactiveItemID;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsEmpty => m_SkillValue <= 0.0;

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
            Light = LightType.Circle300;
            LootType = LootType.Blessed;

            m_InactiveItemID = inactiveItemID;
            m_ActiveItemID = activeItemID;

            m_Account = account;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!IsEmpty)
                list.Add(1070721, "#{0}\t{1:0.0}", AosSkillBonuses.GetLabel(Skill), SkillValue); // Skill stored: ~1_skillname~ ~2_skillamount~

            string name = LastUserName;

            if (name == null)
                name = string.Format("#{0}", 1074235); // Unknown

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

            if (Deleted || !IsAccessibleTo(from))
            {
                return false;
            }
            else if (from.Map != Map || !from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return false;
            }
            else if (Account != null && (!(from.Account is Account) || from.Account.Username != Account))
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

        public virtual void OnSkillTransfered(Mobile from)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!CheckUse(from))
                return;

            from.CloseGump(typeof(SelectSkillGump));
            from.CloseGump(typeof(ConfirmSkillGump));
            from.CloseGump(typeof(ConfirmTransferGump));
            from.CloseGump(typeof(ConfirmRemovalGump));
            from.CloseGump(typeof(ErrorGump));

            if (IsEmpty)
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
                m_Stone = stone;

                AddPage(0);

                AddBackground(0, 0, 520, 440, 0x13BE);

                AddImageTiled(10, 10, 500, 20, 0xA40);
                AddImageTiled(10, 40, 500, 360, 0xA40);
                AddImageTiled(10, 410, 500, 20, 0xA40);

                AddAlphaRegion(10, 10, 500, 420);

                AddHtmlLocalized(10, 12, 500, 20, 1061087, 0x7FFF, false, false); // Which skill do you wish to transfer to the Soulstone?

                AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

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
                                AddButton(260, 380, 0xFA5, 0xFA6, 0, GumpButtonType.Page, page + 1);
                                AddHtmlLocalized(305, 382, 200, 20, 1011066, 0x7FFF, false, false); // Next page
                            }

                            AddPage(page + 1);

                            if (page > 0)
                            {
                                AddButton(10, 380, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page);
                                AddHtmlLocalized(55, 382, 200, 20, 1011067, 0x7FFF, false, false); // Previous page
                            }
                        }

                        int x = (p % 2 == 0) ? 10 : 260;
                        int y = (p / 2) * 20 + 40;

                        AddButton(x, y, 0xFA5, 0xFA6, i + 1, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(x + 45, y + 2, 200, 20, AosSkillBonuses.GetLabel(skill.SkillName), 0x7FFF, false, false);

                        n++;
                    }
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0 || !m_Stone.IsEmpty)
                    return;

                Mobile from = sender.Mobile;

                int iSkill = info.ButtonID - 1;
                if (iSkill < 0 || iSkill >= from.Skills.Length)
                    return;

                Skill skill = from.Skills[iSkill];
                if (skill.Base <= 0.0)
                    return;

                if (!m_Stone.CheckUse(from))
                    return;

                from.SendGump(new ConfirmSkillGump(m_Stone, skill));
            }
        }

        private class ConfirmSkillGump : Gump
        {
            private readonly SoulStone m_Stone;
            private readonly Skill m_Skill;

            public ConfirmSkillGump(SoulStone stone, Skill skill)
                : base(50, 50)
            {
                m_Stone = stone;
                m_Skill = skill;

                AddBackground(0, 0, 520, 440, 0x13BE);

                AddImageTiled(10, 10, 500, 20, 0xA40);
                AddImageTiled(10, 40, 500, 360, 0xA40);
                AddImageTiled(10, 410, 500, 20, 0xA40);

                AddAlphaRegion(10, 10, 500, 420);

                AddHtmlLocalized(10, 12, 500, 20, 1070709, 0x7FFF, false, false); // <CENTER>Confirm Soulstone Transfer</CENTER>

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
                AddHtmlLocalized(10, 42, 500, 110, 1061067, 0x7FFF, false, true);

                AddHtmlLocalized(10, 200, 390, 20, 1062297, 0x7FFF, false, false); // Skill Chosen:
                AddHtmlLocalized(210, 200, 390, 20, AosSkillBonuses.GetLabel(skill.SkillName), 0x7FFF, false, false);

                AddHtmlLocalized(10, 220, 390, 20, 1062298, 0x7FFF, false, false); // Current Value:
                AddLabel(210, 220, 0x481, skill.Base.ToString("0.0"));

                AddHtmlLocalized(10, 240, 390, 20, 1062299, 0x7FFF, false, false); // Current Cap:
                AddLabel(210, 240, 0x481, skill.Cap.ToString("0.0"));

                AddHtmlLocalized(10, 260, 390, 20, 1062300, 0x7FFF, false, false); // New Value:
                AddLabel(210, 260, 0x481, "0.0");

                AddButton(10, 360, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 362, 450, 20, 1070720, 0x7FFF, false, false); // Activate the stone.  I am ready to transfer the skill points to it.

                AddButton(10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 382, 450, 20, 1062279, 0x7FFF, false, false); // No, let me make another selection.

                AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0 || !m_Stone.IsEmpty)
                    return;

                Mobile from = sender.Mobile;

                if (!m_Stone.CheckUse(from))
                    return;

                if (info.ButtonID == 1) // Is asking for another selection
                {
                    from.SendGump(new SelectSkillGump(m_Stone, from));
                    return;
                }

                if (m_Skill.Base <= 0.0)
                    return;

                if (m_Skill.Lock != SkillLock.Down)
                {
                    // <CENTER>Unable to Transfer Selected Skill to Soulstone</CENTER>
                    /* You cannot transfer the selected skill to the Soulstone at this time. The selected
                    * skill may be locked or set to raise in your skill menu. Click on "Skills" in your
                    * paperdoll menu to check your raise/locked/lower settings and your total skills.
                    * Make any needed adjustments, then click "Continue". If you do not wish to transfer
                    * the selected skill at this time, click "Cancel".
                    */
                    from.SendGump(new ErrorGump(m_Stone, 1070710, 1070711));
                    return;
                }

                m_Stone.Skill = m_Skill.SkillName;
                m_Stone.SkillValue = m_Skill.Base;

                m_Skill.Base = 0.0;

                CheckSkill(from);

                from.SendLocalizedMessage(1070712); // You have successfully transferred your skill points into the Soulstone.

                m_Stone.LastUserName = from.Name;

                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                Effects.PlaySound(from.Location, from.Map, 0x243);

                Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                m_Stone.OnSkillTransfered(from);
            }

            private void CheckSkill(Mobile from)
            {
                if (m_Skill.SkillName == SkillName.AnimalTaming)
                {
                    PlayerMobile owner = from as PlayerMobile;

                    if (owner != null && owner.AllFollowers != null && owner.AllFollowers.Count > 0)
                    {
                        foreach (BaseCreature bc in owner.AllFollowers.OfType<BaseCreature>())
                        {
                            if (bc.CheckControlChance(owner))
                            {
                                bc.ControlTarget = null;
                                bc.ControlOrder = OrderType.None;

                                if (bc is BaseMount && ((BaseMount)bc).Rider == from)
                                {
                                    from.SendLocalizedMessage(1042317); // You may not ride at this time
                                    ((BaseMount)bc).Rider = null;
                                }
                            }
                        }
                    }
                }
            }
        }

        private class ConfirmTransferGump : Gump
        {
            private readonly SoulStone m_Stone;

            public ConfirmTransferGump(SoulStone stone, Mobile from)
                : base(50, 50)
            {
                m_Stone = stone;

                AddBackground(0, 0, 520, 440, 0x13BE);

                AddImageTiled(10, 10, 500, 20, 0xA40);
                AddImageTiled(10, 40, 500, 360, 0xA40);
                AddImageTiled(10, 410, 500, 20, 0xA40);

                AddAlphaRegion(10, 10, 500, 420);

                AddHtmlLocalized(10, 12, 500, 20, 1070709, 0x7FFF, false, false); // <CENTER>Confirm Soulstone Transfer</CENTER>

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
                AddHtmlLocalized(10, 42, 500, 110, 1061067, 0x7FFF, false, true);

                AddHtmlLocalized(10, 200, 390, 20, 1070718, 0x7FFF, false, false); // Skill Stored:
                AddHtmlLocalized(210, 200, 390, 20, AosSkillBonuses.GetLabel(stone.Skill), 0x7FFF, false, false);

                Skill fromSkill = from.Skills[stone.Skill];

                AddHtmlLocalized(10, 220, 390, 20, 1062298, 0x7FFF, false, false); // Current Value:
                AddLabel(210, 220, 0x481, fromSkill.Base.ToString("0.0"));

                AddHtmlLocalized(10, 240, 390, 20, 1062299, 0x7FFF, false, false); // Current Cap:
                AddLabel(210, 240, 0x481, fromSkill.Cap.ToString("0.0"));

                AddHtmlLocalized(10, 260, 390, 20, 1062300, 0x7FFF, false, false); // New Value:
                AddLabel(210, 260, 0x481, stone.SkillValue.ToString("0.0"));

                AddButton(10, 360, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 362, 450, 20, 1070719, 0x7FFF, false, false); // Activate the stone.  I am ready to retrieve the skill points from it.

                AddButton(10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 382, 450, 20, 1070723, 0x7FFF, false, false); // Remove all skill points from this stone and DO NOT absorb them.

                AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0 || m_Stone.IsEmpty)
                    return;

                Mobile from = sender.Mobile;

                if (!m_Stone.CheckUse(from))
                    return;

                if (info.ButtonID == 1) // Remove skill points
                {
                    from.SendGump(new ConfirmRemovalGump(m_Stone));
                    return;
                }

                SkillName skill = m_Stone.Skill;
                double skillValue = m_Stone.SkillValue;
                Skill fromSkill = from.Skills[m_Stone.Skill];

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
                    from.SendGump(new ErrorGump(m_Stone, 1070717, 1070716));
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
                    from.SendGump(new ErrorGump(m_Stone, 1070717, 1070715));
                    return;
                }

                if (fromSkill.Base >= skillValue)
                {
                    // <CENTER>Unable to Absorb Selected Skill from Soulstone</CENTER>
                    /* You cannot transfer the selected skill to the Soulstone at this time. The selected
                    * skill has a skill level higher than what is stored in the Soulstone.
                    */
                    // Wrong message?!
                    from.SendGump(new ErrorGump(m_Stone, 1070717, 1070802));
                    return;
                }

                #region Scroll of ALacrity
                PlayerMobile pm = from as PlayerMobile;
                if (pm.AcceleratedStart > DateTime.UtcNow)
                {
                    // <CENTER>Unable to Absorb Selected Skill from Soulstone</CENTER>
                    /*You may not use a soulstone while your character is under the effects of a Scroll of Alacrity.*/
                    // Wrong message?!
                    from.SendGump(new ErrorGump(m_Stone, 1070717, 1078115));
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
                m_Stone.SkillValue = 0.0;

                from.SendLocalizedMessage(1070713); // You have successfully absorbed the Soulstone's skill points.

                m_Stone.LastUserName = from.Name;

                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                Effects.PlaySound(from.Location, from.Map, 0x243);

                Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                if (m_Stone is SoulstoneFragment)
                {
                    SoulstoneFragment frag = m_Stone as SoulstoneFragment;

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
                m_Stone = stone;

                AddBackground(0, 0, 520, 440, 0x13BE);

                AddImageTiled(10, 10, 500, 20, 0xA40);
                AddImageTiled(10, 40, 500, 360, 0xA40);
                AddImageTiled(10, 410, 500, 20, 0xA40);

                AddAlphaRegion(10, 10, 500, 420);

                AddHtmlLocalized(10, 12, 500, 20, 1070725, 0x7FFF, false, false); // <CENTER>Confirm Soulstone Skill Removal</CENTER>

                /* WARNING!<BR><BR>
                *
                * You are about to permanently remove all skill points stored in this Soulstone.
                * You WILL NOT absorb these skill points.  They will be DELETED.<BR><BR>
                *
                * Are you sure you wish to do this?  If not, press the Cancel button.
                */
                AddHtmlLocalized(10, 42, 500, 110, 1070724, 0x7FFF, false, true);

                AddButton(10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 382, 450, 20, 1052072, 0x7FFF, false, false); // Continue

                AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0 || m_Stone.IsEmpty)
                    return;

                Mobile from = sender.Mobile;

                if (!m_Stone.CheckUse(from))
                    return;

                m_Stone.SkillValue = 0.0;
                from.SendLocalizedMessage(1070726); // You have successfully deleted the Soulstone's skill points.
            }
        }

        private class ErrorGump : Gump
        {
            private readonly SoulStone m_Stone;

            public ErrorGump(SoulStone stone, int title, int message)
                : base(50, 50)
            {
                m_Stone = stone;

                AddBackground(0, 0, 520, 440, 0x13BE);

                AddImageTiled(10, 10, 500, 20, 0xA40);
                AddImageTiled(10, 40, 500, 360, 0xA40);
                AddImageTiled(10, 410, 500, 20, 0xA40);

                AddAlphaRegion(10, 10, 500, 420);

                AddHtmlLocalized(10, 12, 500, 20, title, 0x7FFF, false, false);

                AddHtmlLocalized(10, 42, 500, 110, message, 0x7FFF, false, true);

                AddButton(10, 380, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 382, 450, 20, 1052072, 0x7FFF, false, false); // Continue

                AddButton(10, 410, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 412, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0)
                    return;

                Mobile from = sender.Mobile;

                if (!m_Stone.CheckUse(from))
                    return;

                if (m_Stone.IsEmpty)
                    from.SendGump(new SelectSkillGump(m_Stone, from));
                else
                    from.SendGump(new ConfirmTransferGump(m_Stone, from));
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
            writer.Write(m_LastUserName);

            //version 2
            writer.Write((int)m_Level);

            writer.Write(m_ActiveItemID);
            writer.Write(m_InactiveItemID);

            writer.Write(m_Account);
            writer.Write(m_NextUse); //TODO: delete it in a harmless way

            writer.WriteEncodedInt((int)m_Skill);
            writer.Write(m_SkillValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 3:
                    {
                        m_LastUserName = reader.ReadString();
                        goto case 2;
                    }
                case 2:
                    {
                        m_Level = (SecureLevel)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_ActiveItemID = reader.ReadInt();
                        m_InactiveItemID = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        m_Account = reader.ReadString();
                        m_NextUse = reader.ReadDateTime(); //TODO: delete it in a harmless way

                        m_Skill = (SkillName)reader.ReadEncodedInt();
                        m_SkillValue = reader.ReadDouble();
                        break;
                    }
            }

            if (version == 0)
            {
                m_ActiveItemID = 0x2A94;
                m_InactiveItemID = 0x2A93;
            }
        }
    }

    public class SoulstoneFragment : SoulStone, IUsesRemaining, ICraftable
    {
        private int m_UsesRemaining;

        public override int LabelNumber => 1071000;// soulstone fragment

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
            m_UsesRemaining = usesRemaining;
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return m_UsesRemaining;
            }
            set
            {
                m_UsesRemaining = value;
                InvalidateProperties();
            }
        }

        public override void OnSkillTransfered(Mobile from)
        {
            if (string.IsNullOrEmpty(Account))
            {
                Account = from.Account.Username;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(2); // version

            writer.WriteEncodedInt(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            m_UsesRemaining = reader.ReadEncodedInt();

            if (version <= 1)
            {
                if (ItemID == 0x2A93 || ItemID == 0x2A94)
                {
                    ActiveItemID = Utility.Random(0x2AA1, 9);
                }
                else
                {
                    ActiveItemID = ItemID;
                }

                InactiveItemID = ActiveItemID;
            }

            if (version == 0 && Weight == 1)
                Weight = -1;
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
                if (m_UsesRemaining <= 0)
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

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            UsesRemaining = 1;
            Hue = 1150;
            return quality;
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
            switch (ItemID)
            {
                case 0x2ADC:
                    ItemID = 0x2AEC;
                    break;
                case 0x2ADD:
                    ItemID = 0x2AED;
                    break;
                case 0x2AEC:
                    ItemID = 0x2ADC;
                    break;
                case 0x2AED:
                    ItemID = 0x2ADD;
                    break;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            get { return m_IsRewardItem; }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_IsRewardItem = reader.ReadBool();
                        break;
                    }
            }
        }
    }

    public class VioletSoulstone : SoulStone
    {
        [Constructable]
        public VioletSoulstone()
            : this(null)
        {
        }

        [Constructable]
        public VioletSoulstone(string account)
            : base(account)
        {
            Hue = 2598;
        }

        public VioletSoulstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class OrangeSoulstone : SoulStone
    {
        [Constructable]
        public OrangeSoulstone()
            : this(null)
        {
        }

        [Constructable]
        public OrangeSoulstone(string account)
            : base(account)
        {
            Hue = 43;
        }

        public OrangeSoulstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class YellowSoulstone : SoulStone
    {
        [Constructable]
        public YellowSoulstone()
            : this(null)
        {
        }

        [Constructable]
        public YellowSoulstone(string account)
            : base(account)
        {
            Hue = 53;
        }

        public YellowSoulstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class WhiteSoulstone : SoulStone
    {
        [Constructable]
        public WhiteSoulstone()
            : this(null)
        {
        }

        [Constructable]
        public WhiteSoulstone(string account)
            : base(account)
        {
            Hue = 1150;
        }

        public WhiteSoulstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class BlackSoulstone : SoulStone
    {
        [Constructable]
        public BlackSoulstone()
            : this(null)
        {
        }

        [Constructable]
        public BlackSoulstone(string account)
            : base(account)
        {
            Hue = 1106;
        }

        public BlackSoulstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
