/**************************** Steward.cs *******************************************************************************************************
 *
 *					        (C) 2015, by Lokai
 *					        
 * DESCRIPTION FROM http://stratics.com/miscellaneous/the-steward/
 *	
 * Based on the 14th Anniversary reward mannequins, these differ from the traditional vendor or barkeep in that they can be re-deeded. 
 * This difference is vital for the way they function.
 * Deeds are obtained from the Clean Up Officers in exchange for 10,000 clean up points, listed after the Archery Butte in his menu.
 * 
Placing: 
 * The Steward can be placed by an owner or co-owner in public a home and requires 1 vendor slot and 125 lockdowns (regardless of whether the Steward’s pack is full or empty)
 * 
Menu Options:
 * The context menu for the steward contains the following options:

    Open Paperdoll – allows you to dress your Steward as you would a vendor or your character.
    Customize Body – offers a choice of male or female and human, elf or gargoyle.
    Rename – allows a name of up to 20 characters
    Set Keyword – also allows 20 characters and can be a single word or a short phrase.
    Open Backpack – opens the steward’s back pack
    Switch Clothes – works exactly like the same command on a mannequin, exchanging all equipped items between your character and the steward.
    Rotate – turns the steward in a clockwise direction in increments of 45º
    Redeed – converts the steward back into a deed in your back pack along with any equipped items.

How does it Work?
 * Items can be placed in the Steward’s back pack singly or in bags. 
 * Each character using the keyword while in range of the steward will be given one of the items, or one of the bags of items. 
 * (‘An item has been placed in your backpack’).

 * The keyword can be said alone, or included in a sentence.

 * Example: The steward’s pack is filled with books for a scavenger hunt. 
 * The key word has been set as ‘book’. If a participant in the scavenger hunt says to the steward 
 * ‘Give me a book please’ he will receive one of the books from the steward’s pack.

 * The steward will only interact with each character once per day. 
 * Server maintenance or re-deeding will re-set it. Changing the keyword will not.

Possible Uses:
 * There are endless possible uses, but here are a few suggested examples.

    Place the Steward at the entrance to your shop and fill the bag with shop runes. 
 * Name it ‘ask me for a rune’ and set the keyword as ‘rune’. 
 * Each customer can only take one rune.
    Write books with lists of scavenger hunt items to collect for a player/guild event. 
 * Each contestant can take one book
    Fill bags with items to be used in a player event. 
 * Each contestent can take one bag.

 *   
/***********************************************************************************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or (at your option) any later version.
 *
 ***********************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Network;
using Server.Prompts;
using Server.Targeting;

namespace Server.Mobiles
{
    public class Steward : BaseCreature
    {
        public override bool ClickTitle
        {
            get { return false; }
        }

        public override bool NoHouseRestrictions
        {
            get { return true; }
        }

        public override bool AllowEquipFrom(Mobile from)
        {
            return mOwner == from;
        }

        public override bool CheckNonlocalLift(Mobile from, Item item)
        {
            return mOwner == from;
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            if (from==mOwner)
                return true;

            from.SendMessage("Only the Steward's owner may add items to their backpack.");
            return false;
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            return mOwner == from;
        }

        public override bool CanPaperdollBeOpenedBy(Mobile from)
        {
            return true;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (mOwner == from &&
                (dropped is BaseClothing || dropped is BaseArmor || dropped is BaseJewel || dropped is BaseWeapon))
            {
                return EquipItem(dropped);
            }

            if (mOwner == from && dropped is Container)
            {
                Container container = (Container) dropped;
                if (container.Items.Count <= 0)
                {
                    List<Item> stewardItems = new List<Item>();
                    foreach (var item in Items)
                    {
                        if (item is BaseClothing || item is BaseArmor || item is BaseJewel || item is BaseWeapon)
                        {
                            stewardItems.Add(item);
                        }
                    }
                    foreach (var item in stewardItems)
                    {
                        container.DropItem(item);
                    }

                    Say("I put everything I was wearing into that container.");
                    return false;
                }
                else
                {
                    List<Item> containerItems = new List<Item>();
                    foreach (var item in (container.Items))
                    {
                        if (item is BaseClothing || item is BaseArmor || item is BaseJewel || item is BaseWeapon)
                        {
                            containerItems.Add(item);
                        }
                    }
                    foreach (var item in containerItems)
                    {
                        EquipItem(item);
                    }

                    Say("I equipped everything from that container that I could.");
                    return false;
                }
            }

            Say("I don't need that!");
            return false;
        }

        public override bool CheckEquip(Item item)
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i].CheckConflictingLayer(this, item, item.Layer) ||
                    item.CheckConflictingLayer(this, Items[i], Items[i].Layer))
                {
                    Say("I am already wearing something there!");
                    return false;
                }
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == Owner)
            {
                if (SwitchClothes(from))
                {
                    from.FixedParticles(0x376A, 8, 16, 5030, EffectLayer.Waist);
                    FixedParticles(0x376A, 8, 16, 5030, EffectLayer.Waist);
                }
            }
            else
                base.OnDoubleClick(from);
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return from is PlayerMobile;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            PlayerMobile pm = (PlayerMobile) e.Mobile;
            if (pm == null || mKeyword == "") return;

            if (e.Speech.Contains(mKeyword))
            {
                if (Backpack == null || Backpack.Items.Count <= 0)
                {
                    pm.SendMessage("This Steward has nothing to give.");
                    return;
                }

                if (mPlayers.Contains(pm))
                {
                    pm.SendMessage("You have already received an item from this Steward today.");
                    return;
                }

                if (pm.AddToBackpack(Backpack.Items[Utility.Random(Backpack.Items.Count)]))
                {
                    pm.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
                    mPlayers.Add(pm);
                }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from == Owner)
            {
                list.Add(new CustomizeBodyEntry(from, this));
                list.Add(new RenameEntry(from, this));
                list.Add(new SetKeywordEntry(from, this));
                list.Add(new OpenBackpackEntry(from, this));
                list.Add(new SwitchClothesEntry(from, this));
                list.Add(new RotateEntry(from, this));
                list.Add(new RedeedEntry(from, this));
            }
        }

        private class RenameEntry : ContextMenuEntry
        {
            private Mobile from;
            private Steward steward;

            public RenameEntry(Mobile mob, Steward ward)
                : base(1155203, 4)
            {
                from = mob;
                steward = ward;
            }

            public override void OnClick()
            {
                if (from == steward.Owner)
                {
                    from.SendMessage("Enter a new name for this Steward. (3 to 20 characters)");
                    from.Prompt = new RenamePrompt(steward);
                }
            }
        }

        private class RenamePrompt : Prompt
        {
            private readonly Steward steward;

            public RenamePrompt(Steward ward)
            {
                steward = ward;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (text.Length > 20)
                {
                    from.SendLocalizedMessage(501143); // That name is too long.
                }
                else if (text.Length < 3 || ContainsDisallowedSpeech(text))
                {
                    from.SendLocalizedMessage(501144); // That name is not permissible.
                }
                else
                {
                    steward.Name = text;
                    from.SendMessage("The name has been changed.");
                }
            }
        }

        private class SetKeywordEntry : ContextMenuEntry
        {
            private Mobile from;
            private Steward steward;

            public SetKeywordEntry(Mobile mob, Steward ward)
                : base(1153254, 4)
            {
                from = mob;
                steward = ward;
            }

            public override void OnClick()
            {
                if (from == steward.Owner)
                {
                    from.SendMessage("Select the key word or phrase for this Steward. (3 to 20 characters)");
                    from.Prompt = new SetKeywordPrompt(steward);
                }
            }
        }

        private class SetKeywordPrompt : Prompt
        {
            private readonly Steward steward;

            public SetKeywordPrompt(Steward ward)
            {
                steward = ward;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (text.Length > 20)
                {
                    from.SendLocalizedMessage(1153255); // That keyword is too long.
                }
                else if (text.Length < 3 || ContainsDisallowedSpeech(text))
                {
                    from.SendLocalizedMessage(1153256); // That keyword is disallowed.
                }
                else
                {
                    steward.Keyword = text;
                    from.SendLocalizedMessage(1153257); // The keyword has been set.
                }
            }
        }

        private static bool ContainsDisallowedSpeech(string text)
        {
            foreach (var word in ProfanityProtection.Disallowed)
            {
                if (text.Contains(word))
                    return true;
            }
            return false;
        }

        private class OpenBackpackEntry : ContextMenuEntry
        {
            private Mobile from;
            private Steward steward;

            public OpenBackpackEntry(Mobile mob, Steward ward)
                : base(3006145, 4)
            {
                from = mob;
                steward = ward;
            }

            public override void OnClick()
            {
                if (from == steward.Owner)
                {
                    if (steward.Backpack == null)
                        steward.EquipItem(new Backpack());
                    steward.Backpack.DisplayTo(from);
                }
            }
        }


        private class CustomizeBodyEntry : ContextMenuEntry
        {
            private Mobile from;
            private Steward steward;

            public CustomizeBodyEntry(Mobile mob, Steward ward)
                : base(1151585, 4)
            {
                from = mob;
                steward = ward;
            }

            public override void OnClick()
            {
                if (from == steward.Owner)
                {
                    from.SendGump(new StewardGump(from, steward));
                }
            }
        }

        private class SwitchClothesEntry : ContextMenuEntry
        {
            private Mobile from;
            private Steward steward;

            public SwitchClothesEntry(Mobile mob, Steward ward)
                : base(1151606, 4)
            {
                from = mob;
                steward = ward;
            }

            public override void OnClick()
            {
                if (from == steward.Owner)
                {
                    if (steward.SwitchClothes(from))
                    {
                        from.FixedParticles(0x376A, 8, 16, 5030, EffectLayer.Waist);
                        steward.FixedParticles(0x376A, 8, 16, 5030, EffectLayer.Waist);
                    }
                }
            }
        }

        private class RotateEntry : ContextMenuEntry
        {
            private Mobile from;
            private Steward steward;

            public RotateEntry(Mobile mob, Steward ward)
                : base(1151586, 4)
            {
                from = mob;
                steward = ward;
            }

            public override void OnClick()
            {
                if (from == steward.Owner)
                {
                    int direction = (int) steward.Direction;
                    direction++;
                    if (direction > 0x7) direction = 0x0;
                    steward.Direction = (Direction) direction;
                }
            }
        }

        private class RedeedEntry : ContextMenuEntry
        {
            private Mobile from;
            private Steward steward;

            public RedeedEntry(Mobile mob, Steward ward)
                : base(1151601, 4)
            {
                from = mob;
                steward = ward;
            }

            public override void OnClick()
            {
                if (from == steward.Owner)
                {
                    List<Item> stewardItems = new List<Item>();
                    foreach (var item in steward.Items)
                    {
                        if (item is BaseClothing || item is BaseArmor || item is BaseJewel || item is BaseWeapon)
                        {
                            stewardItems.Add(item);
                        }
                    }
                    if (stewardItems.Count > 0)
                    {
                        foreach (var item in stewardItems)
                        {
                            from.AddToBackpack(item);
                        }

                        from.SendMessage("I put everything I was wearing into your Backpack.");
                    }

                    steward.Delete();
                    from.AddToBackpack(new StewardDeed());
                }
            }
        }

        private bool SwitchClothes(Mobile from)
        {
            if (BaseHouse.FindHouseAt(from) == null || !BaseHouse.FindHouseAt(from).IsOwner(from))
            {
                from.SendMessage("You must be in your house to use this.");
                return false;
            }

            if (BaseHouse.FindHouseAt(this) == null || !BaseHouse.FindHouseAt(this).IsOwner(from))
            {
                from.SendMessage("Your Steward must be in your own house to use it.");
                return false;
            }

            if (BaseHouse.FindHouseAt(this) != null && BaseHouse.FindHouseAt(from) != null &&
                BaseHouse.FindHouseAt(this) != BaseHouse.FindHouseAt(from))
            {
                from.SendMessage("You and your Steward must be in the same house to do that!");
                return false;
            }

            if (!CanSee(from) || !from.InLOS(this))
            {
                from.SendMessage("You and your Steward must be able to see eachother to do that.");
                return false;
            }

            List<Item> stewardItems = new List<Item>();
            List<Item> mobileItems = new List<Item>();

            foreach (Item item in Items)
            {
                if (IsEquipped(item) && !(item is Backpack))
                {
                    stewardItems.Add(item);
                }
            }

            foreach (Item item in from.Items)
            {
                if (IsEquipped(item) && !(item is Backpack))
                {
                    mobileItems.Add(item);
                }
            }

            foreach (Item item in mobileItems)
            {
                from.RemoveItem(item);
            }

            foreach (Item item in stewardItems)
            {
                RemoveItem(item);
            }

            foreach (Item item in mobileItems)
            {
                EquipItem(item);
            }

            bool someRemoved = false;
            foreach (Item item in stewardItems)
            {
                if (!from.EquipItem(item))
                {
                    someRemoved = true;
                    if (!from.AddToBackpack(item))
                        item.DropToWorld(from, from.Location);
                }
            }

            if (someRemoved)
            {
                from.SendMessage("You were not able to equip everything.");
                return false;
            }

            return true;
        }

        private bool IsEquipped(Item item)
        {
            return item != null && item.Parent is Mobile && ((Mobile) item.Parent).FindItemOnLayer(item.Layer) == item &&
                   item.Layer != Layer.Mount && item.Layer != Layer.Bank &&
                   item.Layer != Layer.Invalid && item.Layer != Layer.Backpack;
        }

        private bool CanEquip(Item item)
        {
            if (item.Layer != Layer.Invalid)
            {
                if (this.FindItemOnLayer(item.Layer) == null)
                {
                    return true;
                }
                else
                {
                    Say("I am already wearing something on that Layer!");
                }
            }
            else
                Say("The Layer of that Item is Invalid!");

            return false;
        }


        public override bool EquipItem(Item item)
        {
            if (item == null)
            {
                Say("That item is no good!");
                return false;
            }

            if (item.Deleted)
            {
                Say("The item was deleted!");
                return false;
            }

            if (!CanEquip(item))
            {
                return false;
            }

            if (CheckEquip(item) && OnEquip(item) && item.OnEquip(this))
            {
                AddItem(item);
                return true;
            }

            Say("I can't equip that!");
            return false;
        }

        public override bool IsInvulnerable
        {
            get { return true; }
        }

        private Mobile mOwner;

        public Mobile Owner
        {
            get { return mOwner; }
        }

        private string mKeyword;

        public string Keyword
        {
            get { return mKeyword; }
            set { mKeyword = value; }
        }

        private List<PlayerMobile> mPlayers;

        public List<PlayerMobile> Players
        {
            get { return mPlayers; }
            set { mPlayers = value; }
        }

        private MyTimer myTimer;

        private class MyTimer : Timer
        {
            private Steward mSteward;

            public MyTimer(Steward steward) : base(TimeSpan.FromHours(24.0))
            {
                mSteward = steward;
            }

            protected override void OnTick()
            {
                mSteward.Players = new List<PlayerMobile>();
            }
        }

        [Constructable]
        public Steward(Mobile owner)
            : base(AIType.AI_Use_Default, FightMode.None, 1, 1, 0.2, 0.2)
        {
            mOwner = owner;
            mKeyword = "";
            mPlayers = new List<PlayerMobile>();
            myTimer = new MyTimer(this);
            myTimer.Start();
            Female = owner.Female;
            Race = owner.Race;
            Name = "Steward";
            Title = "";
            NameHue = 1150;

            SetBody();

            CantWalk = true;
            Direction = Direction.South;
            Blessed = true;
            if (Backpack == null) EquipItem(new StewardBackpack(this));
        }

        public void SetBody()
        {
            switch (Race.RaceID)
            {
                case 0: // Human
                    Body = Female ? 401 : 400;
                    break;
                case 1: // Elf
                    Body = Female ? 606 : 605;
                    break;
                case 2: // Gargoyle
                    Body = Female ? 667 : 666;
                    break;
            }
            int hairHue = Race.RandomHairHue();
            Hue = Race.RandomSkinHue();
            HairItemID = Race.RandomHair(Female);
            HairHue = hairHue;
            FacialHairItemID = Race.RandomFacialHair(Female);
            FacialHairHue = hairHue;
        }

        public static readonly CustomHuePicker HumanHairColor = new CustomHuePicker(new CustomHueGroup[]
        {
            new CustomHueGroup("Hair Color 0", new int[] {1102, 1103, 1104, 1105, 1106, 1107, 1108, 1109}),
            new CustomHueGroup("Hair Color 1", new int[] {1110, 1111, 1112, 1113, 1114, 1115, 1116, 1117}),
            new CustomHueGroup("Hair Color 2", new int[] {1118, 1119, 1120, 1121, 1122, 1123, 1124, 1125}),
            new CustomHueGroup("Hair Color 3", new int[] {1126, 1127, 1128, 1129, 1130, 1131, 1132, 1133}),
            new CustomHueGroup("Hair Color 4", new int[] {1134, 1135, 1136, 1137, 1138, 1139, 1140, 1141}),
            new CustomHueGroup("Hair Color 5", new int[] {1142, 1143, 1144, 1145, 1146, 1147, 1148, 1149})
        }, false, "Hair Color");

        public static readonly CustomHuePicker HumanSkinColor = new CustomHuePicker(new CustomHueGroup[]
        {
            new CustomHueGroup("Skin Color 0", new int[] {1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009}),
            new CustomHueGroup("Skin Color 1", new int[] {1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017}),
            new CustomHueGroup("Skin Color 2", new int[] {1018, 1019, 1020, 1021, 1022, 1023, 1024, 1025}),
            new CustomHueGroup("Skin Color 3", new int[] {1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033}),
            new CustomHueGroup("Skin Color 4", new int[] {1034, 1035, 1036, 1037, 1038, 1039, 1040, 1041}),
            new CustomHueGroup("Skin Color 5", new int[] {1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049}),
            new CustomHueGroup("Skin Color 6", new int[] {1050, 1051, 1052, 1053, 1054, 1055, 1056, 1057, 1058})
        }, false, "Skin Color");

        public static readonly CustomHuePicker ElfSkinColor = new CustomHuePicker(new CustomHueGroup[]
        {
            new CustomHueGroup("Skin Color 0", new int[] {0x0BF, 0x24D, 0x24E, 0x24F}),
            new CustomHueGroup("Skin Color 1", new int[] {0x353, 0x361, 0x367, 0x374, 0x375, 0x376}),
            new CustomHueGroup("Skin Color 2", new int[] {0x381, 0x382, 0x383, 0x384, 0x385, 0x389}),
            new CustomHueGroup("Skin Color 3", new int[] {0x3DE, 0x3E5, 0x3E6, 0x3E8, 0x3E9, 0x3CB, 0x4A7, 0x4DE}),
            new CustomHueGroup("Skin Color 4", new int[] {0x51D, 0x53F, 0x579, 0x76B, 0x76C, 0x76D, 0x835, 0x903})
        }, false, "Skin Color");

        public static readonly CustomHuePicker ElfHairColor = new CustomHuePicker(new CustomHueGroup[]
        {
            new CustomHueGroup("Hair Color 0", new int[] {0x034, 0x035, 0x036, 0x037, 0x038, 0x039}),
            new CustomHueGroup("Hair Color 1", new int[] {0x058, 0x08E, 0x08F, 0x090, 0x091, 0x092, 0x101}),
            new CustomHueGroup("Hair Color 2", new int[] {0x159, 0x15A, 0x15B, 0x15C, 0x15D, 0x15E}),
            new CustomHueGroup("Hair Color 3", new int[] {0x128, 0x12F, 0x1BD, 0x1E4, 0x1F3}),
            new CustomHueGroup("Hair Color 4", new int[] {0x207, 0x211, 0x239, 0x251, 0x26C, 0x2C3}),
            new CustomHueGroup("Hair Color 5", new int[] {0x2C9, 0x31D, 0x31E, 0x31F}),
            new CustomHueGroup("Hair Color 6", new int[] {0x320, 0x321, 0x322, 0x323, 0x324, 0x325, 0x326}),
            new CustomHueGroup("Hair Color 7", new int[] {0x369, 0x386, 0x387, 0x388, 0x389, 0x38A}),
            new CustomHueGroup("Hair Color 8", new int[] {0x59D, 0x6B8, 0x725, 0x853})
        }, false, "Hair Color");

        public static readonly CustomHuePicker GargoyleHairColor = new CustomHuePicker(new CustomHueGroup[]
        {
            new CustomHueGroup("Hair Color 0", new int[] {0x709, 0x70B, 0x70D, 0x70F, 0x711}),
            new CustomHueGroup("Hair Color 1", new int[] {0x763, 0x765, 0x768, 0x76B}),
            new CustomHueGroup("Hair Color 2", new int[] {0x6F3, 0x6F1, 0x6EF, 0x6E4, 0x6E2, 0x6E0}),
            new CustomHueGroup("Hair Color 3", new int[] {0x709, 0x70B, 0x70D})
        }, false, "Hair Color");

        public static readonly CustomHuePicker GargoyleSkinColor = new CustomHuePicker(new CustomHueGroup[]
        {
            new CustomHueGroup("Skin Color 0", new int[] {0x6DB, 0x6DC, 0x6DD, 0x6DE, 0x6DF}),
            new CustomHueGroup("Skin Color 1", new int[] {0x6E0, 0x6E1, 0x6E2, 0x6E3, 0x6E4}),
            new CustomHueGroup("Skin Color 2", new int[] {0x6E5, 0x6E6, 0x6E7, 0x6E8, 0x6E9}),
            new CustomHueGroup("Skin Color 3", new int[] {0x6EA, 0x6EB, 0x6EC, 0x6ED, 0x6EE}),
            new CustomHueGroup("Skin Color 4", new int[] {0x6EF, 0x6F0, 0x6F1, 0x6F2, 0x6F3})
        }, false, "Skin Color");



        public override void GenerateLoot()
        {
        }

        public Steward(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0); // version

            writer.Write(mOwner);
            writer.Write(mKeyword);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    mOwner = reader.ReadMobile();
                    mKeyword = reader.ReadString();
                    mPlayers = new List<PlayerMobile>();
                    myTimer = new MyTimer(this);
                    myTimer.Start();
                    if (Backpack == null) EquipItem(new StewardBackpack(this));
                    break;
            }
        }

        private class StewardGump : Gump
        {
            private Mobile m_From;
            private Steward m_Steward;

            private void AddBackground()
            {
                AddPage(0);

                AddBackground(0, 0, 500, 400, 3600);

                AddImage(240, 18, 1800);
                AddImage(240, 18, GetLgBodyId(), GetHue(m_Steward.Hue)); // Body
                AddImage(240, 18, GetHairId(m_Steward.HairItemID), GetHue(m_Steward.HairHue)); // Hair
                if (!m_Steward.Female)
                    AddImage(240, 18, GetHairId(m_Steward.FacialHairItemID), GetHue(m_Steward.FacialHairHue));
                        // Facial Hair

                AddLabel(100, 18, 1153, "Steward CUSTOMIZATION MENU");
            }

            private void AddButtons()
            {
                AddButton(20, 80, 4005, 4007, (int) Buttons.RANDOMIZE, GumpButtonType.Reply, 0);
                AddLabel(60, 80, 1152, "Randomize");

                AddButton(20, 120, 4005, 4007, (int) Buttons.CHANGE_RACE, GumpButtonType.Reply, 0);
                AddLabel(60, 120, 1152, "Change Race");

                AddButton(20, 160, 4005, 4007, (int) Buttons.CHANGE_GENDER, GumpButtonType.Reply, 0);
                AddLabel(60, 160, 1152, "Switch Gender");

                AddButton(20, 200, 4005, 4007, (int) Buttons.CHANGE_HAIR, GumpButtonType.Reply, 0);
                AddLabel(60, 200, 1152, "Change Hair Style");

                if (m_Steward.Female)
                    AddImage(20, 240, 4005);
                else
                    AddButton(20, 240, 4005, 4007, (int) Buttons.CHANGE_FACIAL_HAIR, GumpButtonType.Reply, 0);
                AddLabel(60, 240, m_Steward.Female ? 0x20 : 1152, "Change Facial Hair");

                AddButton(20, 280, 4005, 4007, (int) Buttons.CHANGE_HAIR_COLOR, GumpButtonType.Reply, 0);
                AddLabel(60, 280, 1152, "Hair Color");

                AddButton(20, 320, 4005, 4007, (int) Buttons.CHANGE_SKIN_COLOR, GumpButtonType.Reply, 0);
                AddLabel(60, 320, 1152, "Skin Color");

                AddButton(225, 362, 4014, 4016, (int) Buttons.CLOSE, GumpButtonType.Reply, 0);
                AddLabel(265, 362, 1265, "Close");
            }

            private int GetHue(int fromHue)
            {
                int hue = 0;
                switch (m_Steward.Race.Name)
                {
                    case "Human":
                    case "Elf":
                    case "Gargoyle":
                        if (fromHue > 32768)
                            hue = fromHue - 32768;
                        else
                            hue = fromHue;
                        break;
                }
                return hue;
            }

            private int GetLgBodyId()
            {
                switch (m_Steward.Race.Name)
                {
                    case "Human":
                        return m_Steward.Female ? 1888 : 1889;
                    case "Elf":
                        return m_Steward.Female ? 1893 : 1894;
                    case "Gargoyle":
                        return m_Steward.Female ? 1898 : 1899;
                }
                return 1889;
            }

            private int GetHairId(int fromHairId)
            {
                switch (fromHairId)
                {
                    // ELF HAIR
                    case 0x2FC0: //Long Feather
                        return m_Steward.Female ? 1775 : 1785;
                    case 0x2FC1: //Short
                        return m_Steward.Female ? 1776 : 1786;
                    case 0x2FC2: //Mullet
                        return m_Steward.Female ? 1777 : 1787;
                    case 0x2FCE: //Knob
                        return m_Steward.Female ? 1779 : 1790;
                    case 0x2FCF: //Braided
                        return m_Steward.Female ? 1780 : 1791;
                    case 0x2FD1: //Spiked
                        return m_Steward.Female ? 1783 : 1793;
                    case 0x2FCC: //Flower
                        return 1778;
                    case 0x2FBF: //Mid-long
                        return 1784;
                    case 0x2FD0: //Bun
                        return 1782;
                    case 0x2FCD: //Long
                        return 1789;

                    // HUMAN HAIR
                    case 0x203B: //Short
                        return m_Steward.Female ? 1847 : 1875;
                    case 0x203C: //Long
                        return m_Steward.Female ? 1837 : 1876;
                    case 0x203D: //Pony Tail
                        return m_Steward.Female ? 1845 : 1879;
                    case 0x2044: //Mohawk
                        return m_Steward.Female ? 1843 : 1877;
                    case 0x2045: //Pageboy
                        return m_Steward.Female ? 1844 : 1871;
                    case 0x2047: //Curly
                        return m_Steward.Female ? 1839 : 1873;
                    case 0x2049: //Two tails
                        return m_Steward.Female ? 1836 : 1870;
                    case 0x204A: //Top Knot
                        return m_Steward.Female ? 1840 : 1874;
                    case 0x2046: //Buns
                        return 1841;
                    case 0x2048: //Receeding Hair
                        return 1878;

                    // FACIAL HAIR
                    case 0x203E: // Long Beard
                        return m_Steward.Female ? 30088 : 1883;
                    case 0x203F: // Short Beard
                        return m_Steward.Female ? 30088 : 1885;
                    case 0x2040: // Goatee
                        return m_Steward.Female ? 30088 : 1881;
                    case 0x2041: // Moustache
                        return m_Steward.Female ? 30088 : 1884;
                    case 0x204B: // Short Beard and Moustache
                        return m_Steward.Female ? 30088 : 1886;
                    case 0x204C: // Long Beard and Moustache
                        return m_Steward.Female ? 30088 : 1882;
                    case 0x204D: // Vandyke
                        return m_Steward.Female ? 30088 : 1887;

                    // GARGOYLE MALE HAIR-ID HORNS
                    case 0x4258:
                        return 1900;
                    case 0x4259:
                        return 1901;
                    case 0x425A:
                        return 1907;
                    case 0x425B:
                        return 1902;
                    case 0x425C:
                        return 1908;
                    case 0x425D:
                        return 1910;
                    case 0x425E:
                        return 1910;
                    case 0x425F:
                        return 1911;

                    case 0x4261:
                        return 1952;
                    case 0x4262:
                        return 1953;
                    case 0x4273:
                        return 1950;
                    case 0x4274:
                        return 1954;
                    case 0x4275:
                        return 1951;
                    case 0x42B1:
                        return 1918;
                    case 0x42AA:
                        return 1953;
                    case 0x42AB:
                        return 1917;

                    // GARGOYLE FACIAL HORNS
                    case 0x42AD:
                        return m_Steward.Female ? 30088 : 1903;
                    case 0x42AE:
                        return m_Steward.Female ? 30088 : 1904;
                    case 0x42AF:
                        return m_Steward.Female ? 30088 : 1905;
                    case 0x42B0:
                        return m_Steward.Female ? 30088 : 1906;
                }
                return 30088;
            }

            public StewardGump(Mobile from, Steward Steward)
                : base(0, 0)
            {
                m_From = from;
                m_Steward = Steward;

                from.CloseGump(typeof (StewardGump));

                AddBackground();
                AddButtons();
            }

            private enum Buttons
            {
                RANDOMIZE = 1,
                CHANGE_GENDER,
                CHANGE_HAIR_COLOR,
                CHANGE_FACIAL_HAIR,
                CHANGE_HAIR,
                CHANGE_SKIN_COLOR,
                CHANGE_RACE,
                CLOSE = 99
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                int button = info.ButtonID;
                if (button < 1 || button >= 99)
                    return;

                switch (info.ButtonID)
                {
                    case (int) Buttons.RANDOMIZE:
                        // Randomize...
                        m_Steward.Female = Utility.RandomBool();
                        m_Steward.Race = Race.Races[Utility.Random(Race.AllRaces.Count)];
                        m_Steward.SetBody();
                        m_From.SendGump(new StewardGump(m_From, m_Steward));
                        break;
                    case (int) Buttons.CHANGE_GENDER:
                        // Change gender...
                        m_Steward.Female = !m_Steward.Female;
                        UpdateBody();
                        m_From.SendGump(new StewardGump(m_From, m_Steward));
                        break;
                    case (int) Buttons.CHANGE_HAIR_COLOR:
                        // Change Hair Color...
                        switch (m_Steward.Race.RaceID)
                        {
                            case 0: // Human
                                m_From.SendGump(new CustomHuePickerGump(m_Steward, HumanHairColor, HairCallback, m_From));
                                break;
                            case 1: // Elf
                                m_From.SendGump(new CustomHuePickerGump(m_Steward, ElfHairColor, HairCallback, m_From));
                                break;
                            case 2: // Gargoyle
                                m_From.SendGump(new CustomHuePickerGump(m_Steward, GargoyleHairColor, HairCallback,
                                    m_From));
                                break;
                        }
                        break;
                    case (int) Buttons.CHANGE_SKIN_COLOR:
                        // Change Skin Color...
                        switch (m_Steward.Race.RaceID)
                        {
                            case 0: // Human
                                m_From.SendGump(new CustomHuePickerGump(m_Steward, HumanSkinColor, SkinCallback, m_From));
                                break;
                            case 1: // Elf
                                m_From.SendGump(new CustomHuePickerGump(m_Steward, ElfSkinColor, SkinCallback, m_From));
                                break;
                            case 2: // Gargoyle
                                m_From.SendGump(new CustomHuePickerGump(m_Steward, GargoyleSkinColor, SkinCallback,
                                    m_From));
                                break;
                        }
                        break;
                    case (int) Buttons.CHANGE_HAIR:
                        // Change Hairstyle...
                        Utility.AssignRandomHair(m_Steward);
                        m_From.SendGump(new StewardGump(m_From, m_Steward));
                        break;
                    case (int) Buttons.CHANGE_FACIAL_HAIR:
                        // Change Facial Hairstyle...
                        Utility.AssignRandomFacialHair(m_Steward);
                        m_From.SendGump(new StewardGump(m_From, m_Steward));
                        break;
                    case (int) Buttons.CHANGE_RACE:
                        // Change Race...
                        if (m_Steward.Race.RaceID >= Race.AllRaces.Count - 1)
                        {
                            m_Steward.Race = Race.AllRaces[0];
                        }
                        else
                        {
                            m_Steward.Race = Race.AllRaces[m_Steward.Race.RaceID + 1];
                        }
                        UpdateBody();
                        m_From.SendGump(new StewardGump(m_From, m_Steward));
                        break;
                    default:
                        break;
                }

            }

            private void UpdateBody()
            {
                switch (m_Steward.Race.RaceID)
                {
                    case 0: // Human
                        m_Steward.Body = m_Steward.Female ? 401 : 400;
                        break;
                    case 1: // Elf
                        m_Steward.Body = m_Steward.Female ? 606 : 605;
                        break;
                    case 2: // Gargoyle
                        m_Steward.Body = m_Steward.Female ? 667 : 666;
                        break;
                }
                Utility.AssignRandomHair(m_Steward);
                Utility.AssignRandomFacialHair(m_Steward);
                m_Steward.Hue = m_Steward.Race.RandomSkinHue();
                m_Steward.InvalidateProperties();
            }
        }

        private static void HairCallback(Mobile from, object state, int hue)
        {
            from.HairHue = hue;
            if (state is Mobile && from is Steward && ((Mobile) state) == ((Steward) from).Owner)
            {
                ((Steward) from).InvalidateProperties();
                ((Mobile) state).SendGump(new StewardGump(((Mobile) state), ((Steward) from)));
            }
        }

        private static void SkinCallback(Mobile from, object state, int hue)
        {
            from.Hue = hue;
            if (state is Mobile && from is Steward && ((Mobile) state) == ((Steward) from).Owner)
            {
                ((Steward) from).InvalidateProperties();
                ((Mobile) state).SendGump(new StewardGump(((Mobile) state), ((Steward) from)));
            }
        }
    }

    public class StewardBackpack : Backpack
    {
        private Steward mOwner;

        public StewardBackpack(Steward owner)
        {
            mOwner = owner;
        }

        public StewardBackpack(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DummyValue { get { return LiftOverride; } set { LiftOverride = value; } }

        public override bool IsAccessibleTo(Mobile m)
        {
            if (m == mOwner.Owner) return true;

            return base.IsAccessibleTo(m);
        }

        public override void OnSnoop(Mobile from)
        {
            if (from == mOwner.Owner)
            {
                DisplayTo(from);
            }
            else
            {
                base.OnSnoop(from);
            }
        }

        public override bool CheckContentDisplay(Mobile from)
        {
            if (from == mOwner.Owner)
                return true;

            return base.CheckContentDisplay(from);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (from == mOwner.Owner) return true;

            return base.CheckLift(from, item, ref reject);
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            if (from != mOwner.Owner)
                base.OnItemLifted(from, item);
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (from == mOwner.Owner)
            {
                if (!CheckHold(from, dropped, sendFullMessage, true, 0, 0))
                {
                    return false;
                }

                DropItem(dropped);

                ItemFlags.SetTaken(dropped, true);

                return true;
            }
            return base.TryDropItem(from, dropped, sendFullMessage);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (mOwner.Owner == from)
            {
                if (!this.CheckHold(from, item, true, true))
                {
                    return false;
                }

                item.Location = new Point3D(p.X, p.Y, 0);
                this.AddItem(item);

                from.SendSound(this.GetDroppedSound(item), this.GetWorldLocation());

                ItemFlags.SetTaken(item, true);

                return true;
            }
            else
            {
                from.SendMessage("Only the Steward's owner may add items to their backpack.");
            }
            return base.OnDragDropInto(from, item, p);
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (!m.IsStaff())
            {
                int maxItems = MaxItems;

                if (checkItems && maxItems != 0 &&
                    (TotalItems + plusItems + item.TotalItems + (item.IsVirtualItem ? 0 : 1)) > maxItems)
                {
                    if (message)
                    {
                        SendFullItemsMessage(m, item);
                    }

                    return false;
                }
                else
                {
                    int maxWeight = MaxWeight;

                    if (maxWeight != 0 && (TotalWeight + plusWeight + item.TotalWeight + item.PileWeight) > maxWeight)
                    {
                        if (message)
                        {
                            SendFullWeightMessage(m, item);
                        }

                        return false;
                    }
                }
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0); // version
            writer.Write(mOwner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            mOwner = reader.ReadMobile<Steward>();
        }
    }

    [Flipable(0x14F0, 0x14EF)]
    public class StewardDeed : Item
    {
        public override int LabelNumber
        {
            get { return 1153344; }
        }

        [Constructable]
        public StewardDeed()
            : base(0x14F0)
        {
            Name = "Steward Deed";
            LootType = LootType.Blessed;
        }

        public StewardDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (BaseHouse.FindHouseAt(from) != null && BaseHouse.FindHouseAt(from).Owner == from)
                {
                    from.Target = new PlaceStewardTarget(this);
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        private class PlaceStewardTarget : Target
        {
            private StewardDeed mDeed;

            public PlaceStewardTarget(StewardDeed deed)
                : base(10, true, TargetFlags.None)
            {
                mDeed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Point3D p = from.Location;
                bool validtarget = false;
                if (targeted is StaticTarget)
                {
                    p = new Point3D((IPoint3D) targeted);
                    validtarget = true;
                }
                else if (targeted is Item)
                {
                    p = new Point3D(((Item) targeted).Location);
                    validtarget = true;
                }
                if (validtarget)
                {
                    BaseHouse house = BaseHouse.FindHouseAt(p, from.Map, p.Z);
                    // Must be owner or co-owner, and requires 1 vendor slot and 125 lockdowns 
                    if (house != null && house.Owner == from && house.CanPlaceNewVendor() && house.CheckAosStorage(125))
                    {
                        Steward m =
                            (Steward) Activator.CreateInstance(typeof (Steward), new object[] {from});
                        m.MoveToWorld(p, from.Map);
                        m.Direction = Direction.South;
                        mDeed.Delete();
                    }
                    else
                    {
                        from.SendMessage(
                            "You must place this in a house to which you are an owner or co-owner, and have 1 free vendor slot and 125 available lockdowns.");
                    }
                }
            }
        }
    }
}