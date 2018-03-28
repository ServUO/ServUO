using System;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Mobiles;
using Server.Network;
using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x14F5, 0x14F6)]
    public class Spyglass : Item, IResource
    {
        private CraftResource _Resource;
        private Mobile _Crafter;
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; _Resource = value; Hue = CraftResources.GetHue(_Resource); InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter { get { return _Crafter; } set { _Crafter = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        public bool PlayerConstructed { get { return true; } }

        [Constructable]
        public Spyglass()
            : base(0x14F5)
        {
            Weight = 3.0;
        }

        public Spyglass(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1008155); // You peer into the heavens, seeking the moons...

            from.Send(new MessageLocalizedAffix(from.NetState, from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 1008146 + (int)Clock.GetMoonPhase(Map.Trammel, from.X, from.Y), "", AffixType.Prepend, "Trammel : ", ""));
            from.Send(new MessageLocalizedAffix(from.NetState, from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 1008146 + (int)Clock.GetMoonPhase(Map.Felucca, from.X, from.Y), "", AffixType.Prepend, "Felucca : ", ""));

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is WitchApprenticeQuest)
                {
                    FindIngredientObjective obj = qs.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

                    if (obj != null && !obj.Completed && obj.Ingredient == Ingredient.StarChart)
                    {
                        int hours, minutes;
                        Clock.GetTime(from.Map, from.X, from.Y, out hours, out minutes);

                        if (hours < 5 || hours > 17)
                        {
                            player.SendLocalizedMessage(1055040); // You gaze up into the glittering night sky.  With great care, you compose a chart of the most prominent star patterns.

                            obj.Complete();
                        }
                        else
                        {
                            player.SendLocalizedMessage(1055039); // You gaze up into the sky, but it is not dark enough to see any stars.
                        }
                    }
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Crafter != null)
            {
                list.Add(1050043, _Crafter.TitleName); // crafted by ~1_NAME~
            }

            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (_Resource > CraftResource.Iron)
            {
                list.Add(1053099, "#{0}\t{1}", CraftResources.GetLocalizationNumber(_Resource), String.Format("#{0}", LabelNumber.ToString())); // ~1_oretype~ ~2_armortype~
            }
            else
            {
                base.AddNameProperty(list);
            }
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)_Resource);
            writer.Write(_Crafter);
            writer.Write((int)_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    _Resource = (CraftResource)reader.ReadInt();
                    _Crafter = reader.ReadMobile();
                    _Quality = (ItemQuality)reader.ReadInt();
                    break;
                case 0:
                    break;
            }
        }
    }
}