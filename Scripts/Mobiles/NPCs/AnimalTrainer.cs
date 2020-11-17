#region References
using Server.ContextMenus;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Mobiles
{
    public class AnimalTrainer : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public AnimalTrainer()
            : base("the animal trainer")
        {
            SetSkill(SkillName.AnimalLore, 64.0, 100.0);
            SetSkill(SkillName.AnimalTaming, 90.0, 100.0);
            SetSkill(SkillName.Veterinary, 65.0, 88.0);
        }

        public AnimalTrainer(Serial serial)
            : base(serial)
        { }

        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override VendorShoeType ShoeType => Female ? VendorShoeType.ThighBoots : VendorShoeType.Boots;

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBAnimalTrainer());
        }

        public override int GetShoeHue()
        {
            return 0;
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(Utility.RandomBool() ? new QuarterStaff() : (Item)new ShepherdsCrook());
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)
            {
                list.Add(new StableEntry(this, from));

                if (from.Stabled.Count > 0)
                {
                    list.Add(new ClaimAllEntry(this, from));
                }
            }

            base.AddCustomContextEntries(from, list);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072269); // Quest Giver
        }

        private DateTime _NextTalk;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Alive && !m.Hidden && m is PlayerMobile && InLOS(m) && InRange(m, 8) && !InRange(oldLocation, 8) && DateTime.UtcNow >= _NextTalk)
            {
                if (Utility.Random(100) < 50)
                        Say(1157526); // Such an exciting time to be an Animal Trainer! New taming techniques have been discovered!

                _NextTalk = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            }
        }

        private readonly Type[] _Quests = { typeof(TamingPetQuest), typeof(UsingAnimalLoreQuest), typeof(LeadingIntoBattleQuest), typeof(TeachingSomethingNewQuest) };

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(Location, 5))
            {
                CheckQuest((PlayerMobile)m);
            }
        }

        public bool CheckQuest(PlayerMobile player)
        {
            for (int i = 0; i < _Quests.Length; i++)
            {
                BaseQuest quest = player.Quests.FirstOrDefault(q => q.GetType() == _Quests[i]);

                if (quest != null)
                {
                    if (quest.Completed)
                    {
                        if (quest.GetType() != typeof(TeachingSomethingNewQuest))
                        {
                            quest.GiveRewards();
                        }
                        else
                        {
                            player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));
                        }

                        return true;
                    }

                    player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.InProgress, false));
                    quest.InProgress();

                    return false;
                }
            }

            BaseQuest questt = new TamingPetQuest
            {
                Owner = player,
                Quester = this
            };
            player.CloseGump(typeof(MondainQuestGump));
            player.SendGump(new MondainQuestGump(questt));

            return true;
        }

        public static int GetMaxStabled(Mobile from)
        {
            double taming = from.Skills[SkillName.AnimalTaming].Value;
            double anlore = from.Skills[SkillName.AnimalLore].Value;
            double vetern = from.Skills[SkillName.Veterinary].Value;
            double sklsum = taming + anlore + vetern;

            int max = from is PlayerMobile ? ((PlayerMobile)from).RewardStableSlots : 0;

            if (sklsum >= 240.0)
            {
                max += 5;
            }
            else if (sklsum >= 200.0)
            {
                max += 4;
            }
            else if (sklsum >= 160.0)
            {
                max += 3;
            }
            else
            {
                max += 2;
            }

            max += 4;

            if (taming >= 100.0)
            {
                max += (int)((taming - 90.0) / 10);
            }

            if (anlore >= 100.0)
            {
                max += (int)((anlore - 90.0) / 10);
            }

            if (vetern >= 100.0)
            {
                max += (int)((vetern - 90.0) / 10);
            }

            return max + Spells.SkillMasteries.MasteryInfo.BoardingSlotIncrease(from);
        }

        private void CloseClaimList(Mobile from)
        {
            from.CloseGump(typeof(ClaimListGump));
        }

        public void BeginClaimList(Mobile from)
        {
            if (Deleted || !from.CheckAlive())
            {
                return;
            }

            List<BaseCreature> list = new List<BaseCreature>();

            for (int i = 0; i < from.Stabled.Count; ++i)
            {
                BaseCreature pet = from.Stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    if (pet != null)
                    {
                        pet.IsStabled = false;
                        pet.StabledBy = null;
                    }

                    from.Stabled.RemoveAt(i--);
                    continue;
                }

                list.Add(pet);
            }

            if (list.Count > 0)
            {
                from.SendGump(new ClaimListGump(this, from, list));
            }
            else
            {
                SayTo(from, 502671); // But I have no animals stabled with me at the moment!
            }
        }

        public void EndClaimList(Mobile from, BaseCreature pet)
        {
            if (pet == null || pet.Deleted || from.Map != Map || !from.Stabled.Contains(pet) || !from.CheckAlive())
            {
                return;
            }

            if (!from.InRange(this, 14))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (CanClaim(from, pet))
            {
                DoClaim(from, pet);

                from.Stabled.Remove(pet);

                if (from is PlayerMobile)
                {
                    ((PlayerMobile)from).AutoStabled.Remove(pet);
                }
            }
            else
            {
                SayTo(from, 1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
            }
        }

        public void BeginStable(Mobile from)
        {
            if (Deleted || !from.CheckAlive())
            {
                return;
            }

            /* 
			 * I charge 30 gold per pet for a real week's stable time.
			 * I will withdraw it from thy bank account.
			 * Which animal wouldst thou like to stable here?
			 */
            from.SendLocalizedMessage(1042558);

            from.Target = new StableTarget(this);
        }

        public void EndStable(Mobile from, BaseCreature pet)
        {
            if (Deleted || !from.CheckAlive())
            {
                return;
            }

            if (pet.Body.IsHuman)
            {
                SayTo(from, 502672); // HA HA HA! Sorry, I am not an inn.
            }
            else if (!pet.Controlled)
            {
                SayTo(from, 1048053); // You can't stable that!
            }
            else if (pet.ControlMaster != from)
            {
                SayTo(from, 1042562); // You do not own that pet!
            }
            else if (pet.IsDeadPet)
            {
                SayTo(from, 1049668); // Living pets only, please.
            }
            else if (pet.Summoned)
            {
                SayTo(from, 502673); // I can not stable summoned creatures.
            }
            else if (pet.Allured)
            {
                SayTo(from, 1048053); // You can't stable that!
            }
            else if ((pet is PackLlama || pet is PackHorse || pet is Beetle) && pet.Backpack != null && pet.Backpack.Items.Count > 0)
            {
                SayTo(from, 1042563); // You need to unload your pet.
            }
            else if (pet.Combatant != null && pet.InRange(pet.Combatant, 12) && pet.Map == pet.Combatant.Map)
            {
                SayTo(from, 1042564); // I'm sorry.  Your pet seems to be busy.
            }
            else if (from.Stabled.Count >= GetMaxStabled(from))
            {
                SayTo(from, 1042565); // You have too many pets in the stables!
            }
            else
            {
                pet.ControlTarget = null;
                pet.ControlOrder = OrderType.Stay;
                pet.Internalize();

                pet.SetControlMaster(null);
                pet.SummonMaster = null;

                pet.IsStabled = true;
                pet.StabledBy = from;

                pet.Loyalty = MaxLoyalty; // Wonderfully happy

                from.Stabled.Add(pet);

                SayTo(from, 1049677); // Your pet has been stabled.
            }
        }

        public void Claim(Mobile from)
        {
            Claim(from, null);
        }

        public void Claim(Mobile from, string petName)
        {
            if (Deleted || !from.CheckAlive())
            {
                return;
            }

            bool claimed = false;
            int stabled = 0;

            bool claimByName = (petName != null);

            for (int i = 0; i < from.Stabled.Count; ++i)
            {
                BaseCreature pet = from.Stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    if (pet != null)
                    {
                        pet.IsStabled = false;
                        pet.StabledBy = null;
                    }

                    from.Stabled.RemoveAt(i--);
                    continue;
                }

                ++stabled;

                if (claimByName && !Insensitive.Equals(pet.Name, petName))
                {
                    continue;
                }

                if (CanClaim(from, pet))
                {
                    DoClaim(from, pet);

                    from.Stabled.RemoveAt(i);

                    if (from is PlayerMobile)
                    {
                        ((PlayerMobile)from).AutoStabled.Remove(pet);
                    }

                    --i;

                    claimed = true;
                }
                else
                {
                    SayTo(from, 1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
                }
            }

            if (claimed)
            {
                SayTo(from, 1042559); // Here you go... and good day to you!
            }
            else if (stabled == 0)
            {
                SayTo(from, 502671); // But I have no animals stabled with me at the moment!
            }
            else if (claimByName)
            {
                BeginClaimList(from);
            }
        }

        public bool CanClaim(Mobile from, BaseCreature pet)
        {
            return from.Followers + pet.ControlSlots <= from.FollowersMax;
        }

        private void DoClaim(Mobile from, BaseCreature pet)
        {
            pet.SetControlMaster(from);

            if (pet.Summoned)
            {
                pet.SummonMaster = from;
            }

            pet.ControlTarget = from;
            pet.ControlOrder = OrderType.Follow;

            pet.MoveToWorld(from.Location, from.Map);

            pet.IsStabled = false;
            pet.StabledBy = null;

            pet.Loyalty = MaxLoyalty; // Wonderfully Happy
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return true;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Mobile.Map.Rules != MapRules.FeluccaRules && !CheckVendorAccess(e.Mobile))
            {
                return;
            }

            if (!e.Handled && e.HasKeyword(0x0008)) // *stable*
            {
                e.Handled = true;

                CloseClaimList(e.Mobile);
                BeginStable(e.Mobile);
            }
            else if (!e.Handled && e.HasKeyword(0x0009)) // *claim*
            {
                e.Handled = true;

                CloseClaimList(e.Mobile);

                int index = e.Speech.IndexOf(' ');

                if (index != -1)
                {
                    Claim(e.Mobile, e.Speech.Substring(index).Trim());
                }
                else
                {
                    Claim(e.Mobile);
                }
            }
            else if (!e.Handled && e.Speech.ToLower().IndexOf("stablecount") >= 0)
            {
                IPooledEnumerable eable = e.Mobile.Map.GetMobilesInRange(e.Mobile.Location, 8);
                e.Handled = true;

                foreach (Mobile m in eable)
                {
                    if (m is AnimalTrainer)
                    {
                        e.Mobile.SendLocalizedMessage(1071250, string.Format("{0}\t{1}", e.Mobile.Stabled.Count.ToString(), GetMaxStabled(e.Mobile).ToString())); // ~1_USED~/~2_MAX~ stable stalls used.
                        break;
                    }
                }

                eable.Free();
            }
            else
            {
                base.OnSpeech(e);
            }
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

        private class StableEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly AnimalTrainer m_Trainer;

            public StableEntry(AnimalTrainer trainer, Mobile from)
                : base(6126, 12)
            {
                m_Trainer = trainer;
                m_From = from;

                Enabled = from.Map.Rules == MapRules.FeluccaRules || trainer.CheckVendorAccess(from);
            }

            public override void OnClick()
            {
                m_Trainer.BeginStable(m_From);
            }
        }

        private class ClaimListGump : Gump
        {
            private readonly Mobile m_From;
            private readonly List<BaseCreature> m_List;
            private readonly AnimalTrainer m_Trainer;

            public ClaimListGump(AnimalTrainer trainer, Mobile from, List<BaseCreature> list)
                : base(50, 50)
            {
                m_Trainer = trainer;
                m_From = from;
                m_List = list;

                from.CloseGump(typeof(ClaimListGump));

                AddPage(0);

                AddBackground(0, 0, 325, 50 + (list.Count * 20), 9250);
                AddAlphaRegion(5, 5, 315, 40 + (list.Count * 20));

                AddHtml(
                    15,
                    15,
                    275,
                    20,
                    "<BASEFONT COLOR=#000008>Select a pet to retrieve from the stables:</BASEFONT>",
                    false,
                    false);

                for (int i = 0; i < list.Count; ++i)
                {
                    BaseCreature pet = list[i];

                    if (pet == null || pet.Deleted)
                    {
                        continue;
                    }

                    AddButton(15, 39 + (i * 20), 10006, 10006, i + 1, GumpButtonType.Reply, 0);
                    AddHtml(
                        32,
                        35 + (i * 20),
                        275,
                        18,
                        string.Format("<BASEFONT COLOR=#C6C6EF>{0}</BASEFONT>", pet.Name),
                        false,
                        false);
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int index = info.ButtonID - 1;

                if (index >= 0 && index < m_List.Count)
                {
                    m_Trainer.EndClaimList(m_From, m_List[index]);
                }
            }
        }

        private class ClaimAllEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly AnimalTrainer m_Trainer;

            public ClaimAllEntry(AnimalTrainer trainer, Mobile from)
                : base(6127, 12)
            {
                m_Trainer = trainer;
                m_From = from;

                Enabled = from.Map.Rules == MapRules.FeluccaRules || trainer.CheckVendorAccess(from);
            }

            public override void OnClick()
            {
                m_Trainer.Claim(m_From);
            }
        }

        private class StableTarget : Target
        {
            private readonly AnimalTrainer m_Trainer;

            public StableTarget(AnimalTrainer trainer)
                : base(12, false, TargetFlags.None)
            {
                m_Trainer = trainer;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature)
                {
                    m_Trainer.EndStable(from, (BaseCreature)targeted);
                }
                else if (targeted == from)
                {
                    m_Trainer.SayTo(from, 502672); // HA HA HA! Sorry, I am not an inn.
                }
                else
                {
                    m_Trainer.SayTo(from, 1048053); // You can't stable that!
                }
            }
        }
    }
}
