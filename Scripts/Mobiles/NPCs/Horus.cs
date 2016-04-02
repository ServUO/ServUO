using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Necro
{
    public class Horus : BaseQuester
    {
        [Constructable]
        public Horus()
            : base("the Guardian")
        {
        }

        public Horus(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83F3;
            this.Body = 0x190;

            this.Name = "Horus";
        }

        public override void InitOutfit()
        {
            this.AddItem(this.SetHue(new PlateLegs(), 0x849));
            this.AddItem(this.SetHue(new PlateChest(), 0x849));
            this.AddItem(this.SetHue(new PlateArms(), 0x849));
            this.AddItem(this.SetHue(new PlateGloves(), 0x849));
            this.AddItem(this.SetHue(new PlateGorget(), 0x849));

            this.AddItem(this.SetHue(new Bardiche(), 0x482));

            this.AddItem(this.SetHue(new Boots(), 0x001));
            this.AddItem(this.SetHue(new Cloak(), 0x482));

            Utility.AssignRandomHair(this, false);
            Utility.AssignRandomFacialHair(this, false);
        }

        public override int GetAutoTalkRange(PlayerMobile m)
        {
            return 3;
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            QuestSystem qs = to.Quest;

            return (qs is DarkTidesQuest && qs.IsObjectiveInProgress(typeof(FindCrystalCaveObjective)));
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is DarkTidesQuest)
            {
                QuestObjective obj = qs.FindObjective(typeof(FindCrystalCaveObjective));

                if (obj != null && !obj.Completed)
                    obj.Complete();
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (this.InRange(m.Location, 2) && !this.InRange(oldLocation, 2) && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;
                QuestSystem qs = pm.Quest;

                if (qs is DarkTidesQuest)
                {
                    QuestObjective obj = qs.FindObjective(typeof(ReturnToCrystalCaveObjective));

                    if (obj != null && !obj.Completed)
                        obj.Complete();
                    else
                    {
                        obj = qs.FindObjective(typeof(FindHorusAboutRewardObjective));

                        if (obj != null && !obj.Completed)
                        {
                            Container cont = GetNewContainer();

                            cont.DropItem(new Gold(500));

                            BaseJewel jewel = new GoldBracelet();
                            if (Core.AOS)
                                BaseRunicTool.ApplyAttributesTo(jewel, 3, 20, 40);
                            cont.DropItem(jewel);

                            if (!pm.PlaceInBackpack(cont))
                            {
                                cont.Delete();
                                pm.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                            }
                            else
                            {
                                obj.Complete();
                            }
                        }
                    }
                }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm != null)
                {
                    QuestSystem qs = pm.Quest;

                    if (qs is DarkTidesQuest)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(SpeakCavePasswordObjective));
                        bool enabled = (obj != null && !obj.Completed);

                        list.Add(new SpeakPasswordEntry(this, pm, enabled));
                    }
                }
            }
        }

        public virtual void OnPasswordSpoken(PlayerMobile from)
        {
            QuestSystem qs = from.Quest;

            if (qs is DarkTidesQuest)
            {
                QuestObjective obj = qs.FindObjective(typeof(SpeakCavePasswordObjective));

                if (obj != null && !obj.Completed)
                {
                    obj.Complete();
                    return;
                }
            }

            from.SendLocalizedMessage(1060185); // Horus ignores you.
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

        private class SpeakPasswordEntry : ContextMenuEntry
        {
            private readonly Horus m_Horus;
            private readonly PlayerMobile m_From;
            public SpeakPasswordEntry(Horus horus, PlayerMobile from, bool enabled)
                : base(6193, 3)
            {
                this.m_Horus = horus;
                this.m_From = from;

                if (!enabled)
                    this.Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (this.m_From.Alive)
                    this.m_Horus.OnPasswordSpoken(this.m_From);
            }
        }
    }
}