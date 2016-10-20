using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    [FlipableAttribute(0x14E7, 0x14E8)]
    public class DungeonHitchingPost : Item
    {
        public override int LabelNumber { get { return 1025351; } }// hitching post

        [Constructable]
        public DungeonHitchingPost()
            : base(0x14E7)
        {
            this.Movable = false;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
            {
                list.Add(new StableEntry(this, from));

                if (from.Stabled.Count > 0)
                {
                    list.Add(new ClaimAllEntry(this, from));
                }
            }
        }

        public DungeonHitchingPost(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
        }

        private class StableEntry : ContextMenuEntry
        {
            private readonly DungeonHitchingPost m_Post;
            private readonly Mobile m_From;

            public StableEntry(DungeonHitchingPost post, Mobile from)
                : base(6126, 12)
            {
                this.m_Post = post;
                this.m_From = from;
            }

            public override void OnClick()
            {
                m_Post.BeginStable(m_From);
            }
        }

        private class ClaimAllEntry : ContextMenuEntry
        {
            private readonly DungeonHitchingPost m_Post;
            private readonly Mobile m_From;

            public ClaimAllEntry(DungeonHitchingPost post, Mobile from)
                : base(6127, 12)
            {
                this.m_Post = post;
                this.m_From = from;
            }

            public override void OnClick()
            {
                m_Post.Claim(m_From);
            }
        }

        public static int GetMaxStabled(Mobile from)
        {
            double taming = from.Skills[SkillName.AnimalTaming].Value;
            double anlore = from.Skills[SkillName.AnimalLore].Value;
            double vetern = from.Skills[SkillName.Veterinary].Value;
            double sklsum = taming + anlore + vetern;

            int max;

            if (sklsum >= 240.0)
                max = 5;
            else if (sklsum >= 200.0)
                max = 4;
            else if (sklsum >= 160.0)
                max = 3;
            else
                max = 2;

            if (taming >= 100.0)
                max += (int)((taming - 90.0) / 10);

            if (anlore >= 100.0)
                max += (int)((anlore - 90.0) / 10);

            if (vetern >= 100.0)
                max += (int)((vetern - 90.0) / 10);

            return max;
        }

        private class StableTarget : Target
        {
            private readonly DungeonHitchingPost m_Post;

            public StableTarget(DungeonHitchingPost post)
                : base(12, false, TargetFlags.None)
            {
                this.m_Post = post;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature)
                    this.m_Post.EndStable(from, (BaseCreature)targeted);
                else if (targeted == from)
                    from.SendLocalizedMessage(502672); // HA HA HA! Sorry, I am not an inn.
                else
                    from.SendLocalizedMessage(1048053); // You can't stable that!
            }
        }

        public void BeginStable(Mobile from)
        {
            if (this.Deleted || !from.CheckAlive())
                return;

            if ((from.Backpack == null || from.Backpack.GetAmount(typeof(Gold)) < 30) && Banker.GetBalance(from) < 30)
            {
                from.SendLocalizedMessage(502677); // But thou hast not the funds in thy bank account!
            }
            else
            {
                from.SendLocalizedMessage(1042558); /* I charge 30 gold per pet for a real week's stable time.
                * I will withdraw it from thy bank account.
                * Which animal wouldst thou like to stable here?
                */

                from.Target = new StableTarget(this);
            }
        }

        public void EndStable(Mobile from, BaseCreature pet)
        {
            if (Deleted || !from.CheckAlive())
            {
                return;
            }

            if (pet.Body.IsHuman)
            {
                from.SendLocalizedMessage(502672); // HA HA HA! Sorry, I am not an inn.
            }
            else if (!pet.Controlled)
            {
                from.SendLocalizedMessage(1048053); // You can't stable that!
            }
            else if (pet.ControlMaster != from)
            {
                from.SendLocalizedMessage(1042562); // You do not own that pet!
            }
            else if (pet.IsDeadPet)
            {
                from.SendLocalizedMessage(1049668); // Living pets only, please.
            }
            else if (pet.Summoned)
            {
                from.SendLocalizedMessage(502673); // I can not stable summoned creatures.
            }
            else if (pet.Allured)
            {
                from.SendLocalizedMessage(1048053); // You can't stable that!
            }
            else if ((pet is PackLlama || pet is PackHorse || pet is Beetle) &&
                     (pet.Backpack != null && pet.Backpack.Items.Count > 0))
            {
                from.SendLocalizedMessage(1042563); // You need to unload your pet.
            }
            else if (pet.Combatant != null && pet.InRange(pet.Combatant, 12) && pet.Map == pet.Combatant.Map)
            {
                from.SendLocalizedMessage(1042564); // I'm sorry.  Your pet seems to be busy.
            }
            else if (from.Stabled.Count >= GetMaxStabled(from))
            {
                from.SendLocalizedMessage(1042565); // You have too many pets in the stables!
            }
            else if ((from.Backpack != null && from.Backpack.ConsumeTotal(typeof(Gold), 30)) || Banker.Withdraw(from, 30))
            {
                pet.ControlTarget = null;
                pet.ControlOrder = OrderType.Stay;
                pet.Internalize();

                pet.SetControlMaster(null);
                pet.SummonMaster = null;

                pet.IsStabled = true;
                pet.StabledBy = from;

                if (Core.SE)
                {
                    pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy
                }

                from.Stabled.Add(pet);

                from.SendLocalizedMessage(Core.AOS ? 1049677 : 502679);
                // [AOS: Your pet has been stabled.] Very well, thy pet is stabled. 
                // Thou mayst recover it by saying 'claim' to me. In one real world week, 
                // I shall sell it off if it is not claimed!
            }
            else
            {
                from.SendLocalizedMessage(502677); // But thou hast not the funds in thy bank account!
            }
        }

        public void Claim(Mobile from)
        {
            if (this.Deleted || !from.CheckAlive())
                return;

            bool claimed = false;
            int stabled = 0;
            List<Mobile> list = new List<Mobile>(from.Stabled);

            foreach (Mobile m in list)
            {
                BaseCreature pet = m as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    pet.IsStabled = false;
                    from.Stabled.Remove(pet);
                }
                else
                {
                    ++stabled;

                    if ((from.Followers + pet.ControlSlots) <= from.FollowersMax)
                    {
                        pet.SetControlMaster(from);

                        if (pet.Summoned)
                            pet.SummonMaster = from;

                        pet.ControlTarget = from;
                        pet.ControlOrder = OrderType.Follow;

                        pet.MoveToWorld(from.Location, from.Map);

                        pet.IsStabled = false;

                        if (Core.SE)
                            pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy

                        from.Stabled.Remove(pet);
                        claimed = true;
                    }
                    else
                    {
                        from.SendLocalizedMessage(1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
                    }
                }
            }

            list.Clear();
            list.TrimExcess();

            if (claimed)
            {
                from.SendLocalizedMessage(1042559); // Here you go... and good day to you!
            }
            else if (stabled == 0)
                from.SendLocalizedMessage(502671); // But I have no animals stabled with me at the moment!            
        }

        public void BeginClaimList(Mobile from)
        {
            if (this.Deleted || !from.CheckAlive())
                return;

            List<BaseCreature> list = new List<BaseCreature>();

            for (int i = 0; i < from.Stabled.Count; ++i)
            {
                BaseCreature pet = from.Stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    pet.IsStabled = false;
                    from.Stabled.RemoveAt(i);
                    --i;
                    continue;
                }

                list.Add(pet);
            }

            if (list.Count > 0)
                from.SendGump(new ClaimListGump(this, from, list));
            else
                from.SendLocalizedMessage(502671); // But I have no animals stabled with me at the moment!
        }

        public void EndClaimList(Mobile from, BaseCreature pet)
        {
            if (pet == null || pet.Deleted || from.Map != this.Map || !from.InRange(this, 14) || !from.Stabled.Contains(pet) || !from.CheckAlive())
                return;

            if ((from.Followers + pet.ControlSlots) <= from.FollowersMax)
            {
                pet.SetControlMaster(from);

                if (pet.Summoned)
                    pet.SummonMaster = from;

                pet.ControlTarget = from;
                pet.ControlOrder = OrderType.Follow;

                pet.MoveToWorld(from.Location, from.Map);

                pet.IsStabled = false;
                from.Stabled.Remove(pet);

                from.SendLocalizedMessage(1042559); // Here you go... and good day to you!
            }
            else
            {
                from.SendLocalizedMessage(1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
            }
        }

        public override bool HandlesOnSpeech
        {
            get
            {
                return true;
            }
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && e.HasKeyword(0x0008))
            {
                e.Handled = true;
                this.BeginStable(e.Mobile);
            }
            else if (!e.Handled && e.HasKeyword(0x0009))
            {
                e.Handled = true;

                if (!Insensitive.Equals(e.Speech, "claim"))
                    this.BeginClaimList(e.Mobile);
                else
                    this.Claim(e.Mobile);
            }
            else
            {
                base.OnSpeech(e);
            }
        }

        private class ClaimListGump : Gump
        {
            private readonly DungeonHitchingPost m_Post;
            private readonly Mobile m_From;
            private readonly List<BaseCreature> m_List;

            public ClaimListGump(DungeonHitchingPost post, Mobile from, List<BaseCreature> list)
                : base(50, 50)
            {
                this.m_Post = post;
                this.m_From = from;
                this.m_List = list;

                from.CloseGump(typeof(ClaimListGump));

                this.AddPage(0);

                this.AddBackground(0, 0, 325, 50 + (list.Count * 20), 9250);
                this.AddAlphaRegion(5, 5, 315, 40 + (list.Count * 20));

                this.AddHtml(15, 15, 275, 20, "<BASEFONT COLOR=#FFFFFF>Select a pet to retrieve from the stables:</BASEFONT>", false, false);

                for (int i = 0; i < list.Count; ++i)
                {
                    BaseCreature pet = list[i];

                    if (pet == null || pet.Deleted)
                        continue;

                    this.AddButton(15, 39 + (i * 20), 10006, 10006, i + 1, GumpButtonType.Reply, 0);
                    this.AddHtml(32, 35 + (i * 20), 275, 18, String.Format("<BASEFONT COLOR=#C0C0EE>{0}</BASEFONT>", pet.Name), false, false);
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int index = info.ButtonID - 1;

                if (index >= 0 && index < this.m_List.Count)
                {
                    this.m_Post.EndClaimList(this.m_From, this.m_List[index]);
                }
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
}