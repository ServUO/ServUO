using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class ProfessionalBountyQuest : BaseQuest
    {
        public override object Title => 1116708;
        public override object Description => 1116709;
        public override object Refuse => 1116713;
        public override object Uncomplete => 1116714;
        public override object Complete => 1116715;

        private BaseGalleon m_Galleon;
        private BindingPole m_Pole;
        private BindingRope m_Rope;
        private Mobile m_Captain;
        private readonly List<Mobile> m_Helpers = new List<Mobile>();

        public BaseGalleon Galleon => m_Galleon;
        public BindingPole Pole => m_Pole;
        public BindingRope Rope => m_Rope;
        public Mobile Captain { get { return m_Captain; } set { m_Captain = value; } }

        public ProfessionalBountyQuest()
        {
            AddObjective(new BountyQuestObjective());
        }

        public ProfessionalBountyQuest(BaseGalleon galleon)
        {
            m_Galleon = galleon;

            AddObjective(new BountyQuestObjective());
            AddReward(new BaseReward(1116712)); //The gold listed on the bulletin board and a special reward from the officer if captured alive.
        }

        public override void OnAccept()
        {
            base.OnAccept();

            AddPole();

            if (Owner != null)
            {
                m_Rope = new BindingRope(this);
                Owner.AddToBackpack(m_Rope);
            }
        }

        public void OnBound(BaseCreature captain)
        {
            if (Owner != null)
            {
                m_Captain = captain;
                CompileHelpersList(captain);

                foreach (BaseObjective obj in Objectives)
                {
                    if (obj is BountyQuestObjective)
                    {
                        ((BountyQuestObjective)obj).Captured = true;
                        ((BountyQuestObjective)obj).CapturedCaptain = captain;
                    }
                }
            }
        }

        public void OnPirateDeath(BaseCreature captain)
        {
            m_Captain = captain;
            CompileHelpersList(captain);

            foreach (BaseObjective obj in Objectives)
            {
                if (obj is BountyQuestObjective)
                {
                    ((BountyQuestObjective)obj).Captured = false;
                    ((BountyQuestObjective)obj).CapturedCaptain = null;
                }
            }
        }

        private void CompileHelpersList(BaseCreature pirate)
        {
            if (Owner == null)
                return;

            Party p = Party.Get(Owner);
            List<DamageStore> rights = pirate.GetLootingRights();

            IPooledEnumerable eable = pirate.GetMobilesInRange(19);
            foreach (Mobile mob in eable)
            {
                if (mob == Owner || !(mob is PlayerMobile))
                    continue;

                Party mobParty = Party.Get(mob);

                //Add party memebers regardless of looting rights
                if (p != null && mobParty != null && p == mobParty)
                {
                    m_Helpers.Add(mob);
                    continue;
                }

                // add those with looting rights
                for (int i = rights.Count - 1; i >= 0; --i)
                {
                    DamageStore ds = rights[i];

                    if (ds.m_HasRight && ds.m_Mobile == mob)
                    {
                        m_Helpers.Add(ds.m_Mobile);
                        break;
                    }
                }
            }
            eable.Free();
        }

        public void AddPole()
        {
            if (m_Galleon == null)
                return;

            int dist = m_Galleon.CaptiveOffset;
            int xOffset = 0;
            int yOffset = 0;
            m_Pole = new BindingPole(this);

            switch (m_Galleon.Facing)
            {
                case Direction.North:
                    xOffset = 0;
                    yOffset = dist * -1;
                    break;
                case Direction.South:
                    xOffset = 0;
                    yOffset = dist * 1;
                    break;
                case Direction.East:
                    yOffset = 0;
                    xOffset = dist * 1;
                    break;
                case Direction.West:
                    xOffset = dist * -1;
                    yOffset = 0;
                    break;
            }

            m_Pole.MoveToWorld(new Point3D(m_Galleon.X + xOffset, m_Galleon.Y + yOffset, m_Galleon.ZSurface), m_Galleon.Map);
            m_Galleon.AddFixture(m_Pole);
        }

        public override void GiveRewards()
        {
            bool captured = false;
            foreach (BaseObjective obj in Objectives)
            {
                if (obj is BountyQuestObjective && ((BountyQuestObjective)obj).Captured)
                {
                    BountyQuestObjective o = (BountyQuestObjective)obj;
                    captured = true;

                    if (o.CapturedCaptain != null && o.CapturedCaptain is PirateCaptain)
                    {
                        PirateCaptain p = o.CapturedCaptain as PirateCaptain;
                        p.Quest = null;
                    }

                    o.CapturedCaptain = null;
                    o.Captured = false;
                    break;
                }
            }

            if (Owner == null)
                return;

            m_Helpers.Add(Owner);
            int totalAward = 7523;

            if (m_Captain != null && BountyQuestSpawner.Bounties.ContainsKey(m_Captain))
                totalAward = BountyQuestSpawner.Bounties[m_Captain];

            int eachAward = totalAward;

            if (m_Helpers.Count > 1)
                eachAward = totalAward / m_Helpers.Count;

            foreach (Mobile mob in m_Helpers)
            {
                if (mob.NetState != null || mob == Owner)
                {
                    mob.AddToBackpack(new Gold(eachAward));

                    if (captured)
                    {
                        Item reward = Loot.Construct(m_CapturedRewards[Utility.Random(m_CapturedRewards.Length)]);

                        if (reward != null)
                        {
                            if (reward is RuinedShipPlans)
                                mob.SendLocalizedMessage(1149838); //Here is something special!  It's a salvaged set of orc ship plans.  Parts of it are unreadable, but if you could get another copy you might be able to fill in some of the missing parts...
                            else
                                mob.SendLocalizedMessage(1149840); //Here is some special cannon ammunition.  It's imported!

                            if (reward is HeavyFlameCannonball || reward is LightFlameCannonball || reward is HeavyFrostCannonball || reward is LightFrostCannonball)
                                reward.Amount = Utility.RandomMinMax(5, 10);

                            mob.AddToBackpack(reward);
                        }
                    }

                    mob.SendLocalizedMessage(1149825, string.Format("{0}\t{1}", totalAward, eachAward)); //Here's your share of the ~1_val~ reward money, you get ~2_val~ gold.  You've earned it!
                }
                else
                {
                    foreach (Mobile mobile in m_Helpers)
                    {
                        if (mobile != mob && mobile.NetState != null)
                            mobile.SendLocalizedMessage(1149837, string.Format("{0}\t{1}\t{2}", eachAward, mob.Name, Owner.Name)); //~1_val~ gold is for ~2_val~, I can't find them so I'm giving this to Captain ~3_val~.
                    }

                    Owner.AddToBackpack(new Gold(eachAward));
                }
            }

            if (m_Captain != null && m_Captain.Alive)
                m_Captain.Delete();

            base.GiveRewards();
        }

        public override void RemoveQuest(bool removeChain)
        {
            base.RemoveQuest(removeChain);

            if (m_Rope != null && !m_Rope.Deleted)
            {
                m_Rope.Quest = null;
                m_Rope.Delete();
            }

            if (m_Pole != null && !m_Pole.Deleted)
            {
                m_Pole.Quest = null;
                m_Pole.Delete();
            }

            if (m_Galleon != null)
                m_Galleon.CapturedCaptain = null;
        }

        public override bool RenderObjective(MondainQuestGump g, bool offer)
        {
            if (offer)
                g.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                g.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            g.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            g.AddButton(275, 430, 0x2EE9, 0x2EEB, (int)Buttons.NextPage, GumpButtonType.Reply, 0);

            g.AddHtmlObject(160, 70, 330, 16, Title, BaseQuestGump.DarkGreen, false, false);
            g.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:
            g.AddHtmlLocalized(98, 156, 312, 16, 1072208, 0x2710, false, false); // All of the following	

            int offset = 172;

            g.AddHtmlLocalized(98, offset, 312, 16, 1116710, 0x2710, false, false);  // Capture or kill a pirate listed on the bulletin board.

            offset += 16;

            g.AddHtmlLocalized(98, offset, 312, 32, 1116711, 0x15F90, false, false); //Return to the officer with the pirate or a death certificate for your reward.

            offset += 32;

            g.AddHtmlLocalized(98, offset, 312, 32, 1116712, 0x15F90, false, false); //The gold listed on the bulletin board and a special reward from the officer if captured alive.

            return true;
        }

        private readonly Type[] m_CapturedRewards =
        {
           typeof(RuinedShipPlans),      typeof(RuinedShipPlans),
           typeof(LightFlameCannonball), typeof(HeavyFlameCannonball),
           typeof(LightFrostCannonball), typeof(HeavyFrostCannonball),
           typeof(LightFlameCannonball), typeof(HeavyFlameCannonball),
           typeof(LightFrostCannonball), typeof(HeavyFrostCannonball)
        };

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version

            writer.Write(m_Pole);
            writer.Write(m_Rope);
            writer.Write(m_Captain);
            writer.Write(m_Galleon);

            writer.Write(m_Helpers.Count);
            foreach (Mobile mob in m_Helpers)
                writer.Write(mob);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            m_Pole = reader.ReadItem() as BindingPole;
            m_Rope = reader.ReadItem() as BindingRope;
            m_Captain = reader.ReadMobile();
            m_Galleon = reader.ReadItem() as BaseGalleon;

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile mob = reader.ReadMobile();
                if (mob != null)
                    m_Helpers.Add(mob);
            }

            if (m_Rope != null)
                m_Rope.Quest = this;

            if (m_Pole != null)
                m_Pole.Quest = this;
            else
                AddPole();

            AddReward(new BaseReward(1116712)); //The gold listed on the bulletin board and a special reward from the officer if captured alive.
        }

        public bool HasQuest(PlayerMobile pm)
        {
            if (pm.Quests == null)
                return false;

            for (int i = 0; i < pm.Quests.Count; i++)
            {
                BaseQuest quest = pm.Quests[i];

                if (quest.Quester == this)
                {
                    for (int j = 0; j < quest.Objectives.Count; j++)
                    {
                        if (quest.Objectives[j].Update(pm))
                            quest.Objectives[j].Complete();
                    }

                    if (quest.Completed)
                    {
                        quest.OnCompleted();
                        pm.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));
                    }
                    else
                    {
                        pm.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.InProgress, false));
                        quest.InProgress();
                    }

                    return true;
                }
            }
            return false;

        }
    }
}
