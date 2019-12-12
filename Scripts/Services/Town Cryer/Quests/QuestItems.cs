using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Network;
using Server.Gumps;
using Server.Services.TownCryer;

namespace Server.Engines.Quests
{
    public class RoyalBritannianGuardOrders : BaseJournal
    {
        public override TextDefinition Title { get { return 1158159; } } // Royal Britannian Guard Orders
        public override TextDefinition Body { get { return 1158160; } }
        /*ROYAL BRITANNIAN GUARD<br>MINISTRY OF PRISONS DETACHMENT<br><br>ORIGINAL ORDERS<br>ROYAL BRITANNIAN GUARD<br>WRONG PRISON
         * DIVISION<br><br>From: COMMAND, RBG Yew<br>To: Lieutenant Bennet Yardley, RBG Yew<br><br>Subject: Wrong Prison Treasure 
         * Expedition<br><br>1. RBG Intelligence has indicated the presence of highly prized cache of weapons stashed within the 
         * deepest confinements of the Prison Dungeon Wrong. Intelligence indicates these weapons were confiscated at the time of 
         * various assassination attempts among the prisoners before the prison was lost.<br><br>2. Official cover story remains in
         * place - no outside interference expected and only official RBG personnel are permitted entry.<br><br>3. Interview with
         * former prison officials indicate weapons under heavy lock and key, suggest specialist assignment of lock picking skills 
         * and tools.<br><br>*The remainder of the document is illegible**/

        public override int LabelNumber { get { return 1158171; } } // orders

        [Constructable]
        public RoyalBritannianGuardOrders()
        {
            LootType = LootType.Blessed;
        }

        public RoyalBritannianGuardOrders(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class CorpseOfBennetYardley : Item, IConditionalVisibility
    {
        public static CorpseOfBennetYardley TramInstance { get; set; }
        public static CorpseOfBennetYardley FelInstance { get; set; }

        public static void Initialize()
        {
            if (Core.TOL)
            {
                if (TramInstance == null)
                {
                    TramInstance = new CorpseOfBennetYardley();
                    TramInstance.MoveToWorld(new Point3D(5688, 653, 0), Map.Trammel);
                }

                if (FelInstance == null)
                {
                    FelInstance = new CorpseOfBennetYardley();
                    FelInstance.MoveToWorld(new Point3D(5688, 653, 0), Map.Felucca);
                }
            }
        }

        public override bool ForceShowProperties { get { return true; } }
        public override int LabelNumber { get { return 1158168; } }

        public CorpseOfBennetYardley()
            : base(Utility.Random(0xECA, 9))
        {
            Movable = false;
        }

        public bool CanBeSeenBy(PlayerMobile pm)
        {
            if (pm.AccessLevel > AccessLevel.Player)
                return true;

            var quest = QuestHelper.GetQuest<RightingWrongQuest4>(pm);

            return quest != null && !quest.Completed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile && from.InRange(Location, 2))
            {
                var quest = QuestHelper.GetQuest<RightingWrongQuest4>((PlayerMobile)from);

                if (from is PlayerMobile && quest != null && !quest.Completed)
                {
                    quest.Objectives[0].CurProgress++;
                    quest.OnCompleted();

                    Visible = false;
                    Visible = true;
                }
            }
        }

        public CorpseOfBennetYardley(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }
            else if (Map == Map.Felucca)
            {
                FelInstance = this;
            }

            if (!Core.TOL)
                Delete();
        }
    }

    public class TreasureHuntingBook : Item
    {
        [Constructable]
        public TreasureHuntingBook()
            : base(0xFBE)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && IsChildOf(m.Backpack))
            {
                m.CloseGump(typeof(InternalGump));
                BaseGump.SendGump(new InternalGump((PlayerMobile)m));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1158253); // Treasure Hunting: A Practical Approach
            list.Add(1154760, "#1158254"); // By: Vespyr Jones
        }

        public class InternalGump : BaseGump
        {
            public InternalGump(PlayerMobile pm)
                : base(pm, 10, 10)
            {
            }

            public override void AddGumpLayout()
            {
                AddImage(0, 0, 0x761C);
                AddImage(112, 40, 0x655);
                AddHtmlLocalized(113, 350, 342, 280, 1158255, C32216(0x080808), false, true);
            }
        }

        public TreasureHuntingBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class BuriedRichesTreasureMap : TreasureMap
    {
        public TreasureMapChest Chest { get; set; }

        public BuriedRichesTreasureMap(int level)
            : base(level, Map.Trammel)
        {
            LootType = LootType.Blessed;
        }

        public override void Decode(Mobile from)
        {
            if (QuestHelper.HasQuest<TheTreasureChaseQuest>((PlayerMobile)from))
            {
                from.CheckSkill(SkillName.Cartography, 0, 100);
                Decoder = from;

                DisplayTo(from);

                from.SendLocalizedMessage(1158243); // Your time studying Treasure Hunting: A Practical Approach helps you decode the map...
            }
            else
            {
                from.PrivateOverheadMessage(MessageType.Regular, 0x21, 1157850, from.NetState); // *You don't make anything of it.*
                //m.PrivateOverheadMessage(MessageType.Regular, 1154, 1158244, m.NetState); // *You decide to visit the Provisioner at the Adventurer's Supplies in Vesper before trying to decode the map...*
            }
        }

        public override void DisplayTo(Mobile m)
        {
            base.DisplayTo(m);

            m.PlaySound(0x41A);
            m.PrivateOverheadMessage(MessageType.Regular, 1154, 1157722, "Cartography", m.NetState); // *Your proficiency in ~1_SKILL~ reveals more about the item*

            if (m is PlayerMobile)
            {
                m.CloseGump(typeof(InternalGump));
                BaseGump.SendGump(new InternalGump((PlayerMobile)m, this));
            }
        }

        public override void OnChestOpened(Mobile from, TreasureMapChest chest)
        {
            if (from is PlayerMobile)
            {
                var quest = QuestHelper.GetQuest<TheTreasureChaseQuest>((PlayerMobile)from);

                if (quest != null)
                {
                    if (Level < 3)
                    {
                        TownCryerSystem.CompleteQuest((PlayerMobile)from, 1158239, 1158251, 0x655);
                        /*Your eyes widen as you pry open the old chest and reveal the treasure within! Even this small cache
                         * excites you as the thought of bigger and better treasure looms on the horizon! The map is covered
                         * in ancient runes and marks the location of another treasure hoard. You carefully furl the map and
                         * set off on your next adventure!*/

                        switch (Level)
                        {
                            default:
                            case 1: from.SendLocalizedMessage(1158245, "", 0x23); // You have found the first zealot treasure! As you dig up the chest a leather bound case appears to contain an additional map. You place it in your backpack for later examination. 
                                break;
                            case 2: from.SendLocalizedMessage(1158246, "", 0x23); // You have found the second zealot treasure! As you dig up the chest a leather bound case appears to contain an additional map. You place it in your backpack for later examination. 
                                break;
                        }
                    }
                    else
                    {
                        quest.CompleteQuest();
                    }
                }
            }
        }

        public override void OnBeginDig(Mobile from)
        {
            if (Completed)
            {
                from.SendLocalizedMessage(503028); // The treasure for this map has already been found.
            }
            else if (Decoder != from)
            {
                from.SendLocalizedMessage(503031); // You did not decode this map and have no clue where to look for the treasure.
            }
            else if (!from.CanBeginAction(typeof(TreasureMap)))
            {
                from.SendLocalizedMessage(503020); // You are already digging treasure.
            }
            else if (from.Map != Facet)
            {
                from.SendLocalizedMessage(1010479); // You seem to be in the right place, but may be on the wrong facet!
            }
            else if (from is PlayerMobile && !QuestHelper.HasQuest<TheTreasureChaseQuest>((PlayerMobile)from))
            {
                from.SendLocalizedMessage(1158257); // You must be on the "The Treasure Chase" quest offered via the Town Cryer to dig up this treasure.
            }
            else
            {
                from.SendLocalizedMessage(503033); // Where do you wish to dig?
                from.Target = new TreasureMap.DigTarget(this);
            }
        }

        protected override bool HasRequiredSkill(Mobile from)
        {
            return true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Level == 1)
            {
                list.Add(1158229); // A mysterious treasure map personally given to you by the Legendary Cartographer in Vesper
            }
            else
            {
                list.Add(1158256); // A mysterious treasure map recovered from a treasure hoard
            }
        }

        private class InternalGump : BaseGump
        {
            public BuriedRichesTreasureMap Map { get; set; }

            public InternalGump(PlayerMobile pm, BuriedRichesTreasureMap map)
                : base(pm, 10, 10)
            {
                Map = map;
            }

            public override void AddGumpLayout()
            {
                AddBackground(0, 0, 454, 400, 9380);
                AddHtmlLocalized(177, 53, 235, 20, CenterLoc, "#1158240", C32216(0xA52A2A), false, false); // A Mysterious Treasure Map
                AddHtmlLocalized(177, 80, 235, 40, CenterLoc, Map.Level == 1 ? "#1158241" : "#1158250", C32216(0xA52A2A), false, false); // Given to you by the Master Cartographer

                /*The Cartographer has given you a mysterious treasure map and offered you some tips on how to go about 
                 * recovering the treasure. As the Cartographer leaned in and handed you the furled parchment, she told
                 * you of the origins of this mysterious document. "Legend has it..." she tells you, "this map is the 
                 * lost treasure of an ancient Sosarian Order of Zealots. I'm told over the centuries they would bury 
                 * small portions of their treasure throughout the Britannian countryside in an effort to thwart any 
                 * attempts to recover the hoard in its entirety." Your eyes widen at the thought of a massive treasure
                 * hoard and you can't wait to find it!*/
                AddHtmlLocalized(177, 122, 235, 228, 1158242, true, true);

                AddItem(85, 120, 0x14EB, 0); 
            }
        }

        public BuriedRichesTreasureMap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(Chest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            Chest = reader.ReadItem() as TreasureMapChest;
        }
    }

    public class TreasureSeekersLockpick : Lockpick
    {
        public override int LabelNumber { get { return 1158258; } }

        public TreasureSeekersLockpick()
        {
            ItemID = 0x14FD;
        }

        protected override void BeginLockpick(Mobile from, ILockpickable item)
        {
            if (from is PlayerMobile && 
                item.Locked &&
                QuestHelper.HasQuest<TheTreasureChaseQuest>((PlayerMobile)from) &&
                item is TreasureMapChest && 
                ((TreasureMapChest)item).TreasureMap is BuriedRichesTreasureMap)
            {
                var chest = (TreasureMapChest)item;

                from.PlaySound(0x241);

                Timer.DelayCall(TimeSpan.FromMilliseconds(200), () =>
                    {
                        if (item.Locked && from.InRange(chest.GetWorldLocation(), 1))
                        {
                            from.CheckTargetSkill(SkillName.Lockpicking, item, 0, 100);

                            // Success! Pick the lock!
                            from.PrivateOverheadMessage(MessageType.Regular, 1154, 1158252, from.NetState); // *Your recent study of Treasure Hunting helps you pick the lock...*
                            chest.SendLocalizedMessageTo(from, 502076); // The lock quickly yields to your skill.
                            from.PlaySound(0x4A);
                            item.LockPick(from);
                        }
                    });
            }
            else
            {
                base.BeginLockpick(from, item);
            }
        }

        public TreasureSeekersLockpick(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class MysteriousPotion : Item
    {
        public override int LabelNumber { get { return 1158286; } } // A Mysterious Potion

        public MysteriousPotion()
            : base(0xF06)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile)
            {
                var pm = m as PlayerMobile;

                if (QuestHelper.HasQuest<AForcedSacraficeQuest2>(pm))
                {
                    if (!TownCryerSystem.UnderMysteriousPotionEffects(pm))
                    {
                        pm.SendGump(new ConfirmCallbackGump(pm, 1158286, 1158287, null, null, confirm: (mob, o) =>
                            {
                                TownCryerSystem.AddMysteriousPotionEffects(mob);

                                mob.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
                                mob.PlaySound(0x1E3);

                                BasePotion.PlayDrinkEffect(mob);

                                Delete();
                            }));
                    }
                    else
                    {
                        pm.SendLocalizedMessage(1158289); // You have already used this.
                    }
                }
                else
                {
                    pm.SendLocalizedMessage(1158285); // You must be on the "A Forced Sacrifice" quest to use this item.
                }
            }
        }

        public MysteriousPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class PaladinCorpse : Container
    {
        public static PaladinCorpse TramInstance { get; set; }
        public static PaladinCorpse FelInstance { get; set; }

        public static void Initialize()
        {
            if (Core.TOL)
            {
                if (TramInstance == null)
                {
                    TramInstance = new PaladinCorpse();
                    TramInstance.MoveToWorld(new Point3D(5396, 118, 0), Map.Trammel);
                }

                if (FelInstance == null)
                {
                    FelInstance = new PaladinCorpse();
                    FelInstance.MoveToWorld(new Point3D(5396, 118, 0), Map.Felucca);
                }
            }
        }

        public override int LabelNumber { get { return 1158135; } } // the remains of a would-be paladin
        public override bool HandlesOnMovement { get { return true; } }
        public override bool IsDecoContainer { get { return false; } }

        public PaladinCorpse()
            : base(0x9F1E)
        {
            DropItem(new WouldBePaladinChronicles());
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (InRange(m.Location, 2) && !InRange(oldLocation, 2))
            {
                PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1158137, m.NetState); // *You notice the skeleton clutching a small journal...*
            }
        }

        public PaladinCorpse(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }
            else if (Map == Map.Felucca)
            {
                FelInstance = this;
            }

            if (!Core.TOL)
                Delete();
        }
    }

    public class WouldBePaladinChronicles : BaseJournal
    {
        public override int LabelNumber { get { return 1094837; } } // a journal

        public override TextDefinition Title { get { return null; } }
        public override TextDefinition Body { get { return 1158138; } }
        /**the text is mostly a journal chronicling the adventures of a man who wished to join the Paladins of Trinsic.  
         * Of particular note is the final entry...*<br><br>This is the most shameful entry I will write...for I have fallen
         * short of my goal. My only hope is my failures will serve to assist those who come after me with the courage to 
         * pursue the truth, and with my notes they will find success. I have found strange crystals on the corpses of the 
         * creatures I slay here. When I touch the crystal, I can feel it absorbed into my being. A growing voice inside me 
         * compels me to altars located throughout the dungeon. When the voice within grew loud enough I could no longer 
         * ignore it, I touched my hand to the altar and before me a grand champion stood! I was quick to react to the
         * crushing blow my newly summoned opponent sought to deliver, and I was victorious! Alas, the deeper into the 
         * dungeon I explored, the more powerful the altar champions become and now I find myself in this dire situation.... 
         * To anyone who reads this...do me the honor and defeat the three champions and slay the unbound energy vortexes that 
         * inhabit the deepest depths of this place...for I feel my time here is shor...*/

        public WouldBePaladinChronicles()
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            m.SendGump(new BaseJournalGump(Title, Body));
        }

        public WouldBePaladinChronicles(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}