using System;
using System.Collections;
using Server;
using Server.Commands;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{

    public class MorphDeed : Item
    {
        private bool m_Redyable;

        [Constructable]
        public MorphDeed()
            : base(0x14f0)
        {
            Weight = 0.0;
            Hue = 1199;
            Name = "Exceptional Morph Deed"; //?????
            m_Redyable = false;
            LootType = LootType.Blessed;
        }

        public MorphDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            this.Movable = false;  // Vii edit

            from.CloseGump(typeof(ACGump2));
            from.SendGump(new ACGump2(this, from, null, null, 0, true));  // Vii edit
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
    //?? Menu
    public class ACGump2 : Gump
    {
        public Mobile m_From;
        public bool del;
        public int scaler = 20;           //?????? Major transmog multiples
        public Item o_item;
        public Item t_item;
        public string msg;
        public int m_cost;
        public int m;
        private MorphDeed m_deed;  // Vii edit

        public ACGump2(MorphDeed deed, Mobile from, Item item, Item mar, int mesg, bool dele) : base(100, 80)   // Vii edit
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;

            m_deed = deed;   // Vii edit
            m_From = from;
            o_item = item;
            t_item = mar; ;
            del = dele;
            m = mesg;
            switch (m)
			{
				case 0:
				msg = "Select your item and appearance sample."; //???????????
				break;
				case 1:
				msg = "The items must be in your pack.";//????????
				break;
				case 2:
				msg = "You can't change the appearance of that."; //??????????????
				break;
				case 3:
				msg = "You have not selected all of your items."; //???????
				break;
				case 4:
				msg = "These are the same appearance.";//???????????
				break;
			}
            AddPage(0);
            AddBackground(33, 40, 446, 351, 5054); //(x, y, width, height, stone box)
            AddImage(6, 52, 13); //(x, y, female) 15, 55
            AddImage(46, 12, 201); //(x, y, etched stone border)
            AddImage(7, 50, 50970, 1); //(x, y, Robe, Hue) 16, 53
            AddBackground(171, 174, 60, 60, 3000); //(x, y, width, height, white square box) bottom
            AddLabel(180, 46, 2414, "Change your item's appearance"); //????? //text at the top

            AddButton(246, 115, 4031, 4030, 1, GumpButtonType.Reply, 0); //bottom target button
            AddLabel(288, 115, 2424, "Select the item to change."); //?????? //text after top target button

            AddBackground(171, 109, 60, 60, 3000); //(x, y, width, height, white square box) top
            if (o_item != null)
                AddItem(178, 117, o_item.ItemID, o_item.Hue);
            if (t_item != null)
                AddItem(178, 183, t_item.ItemID, t_item.Hue);
            AddImage(46, 372, 233); //etched stone border??
            AddImage(2, 56, 202);
            AddImage(473, 56, 203);
            AddImage(2, 372, 204);
            AddImage(473, 372, 205);
            AddImage(2, 12, 206);
            AddImage(473, 12, 207);
            AddButton(246, 181, 4031, 4030, 2, GumpButtonType.Reply, 0); // middle target button
            AddLabel(288, 181, 2424, "Select the appearance sample."); 
            AddButton(380, 368, 239, 240, 3, GumpButtonType.Reply, 0);//Apply
            AddLabel(251, 139, 200, "Select the item to change."); 
            AddLabel(240, 205, 937, "<-This equipment will be"); 
            AddLabel(390, 205, 332, "Destroyed"); 
	    AddBackground(60, 284, 400, 32, 3000); //(x, y, width, height, white square box) rectangle
	    AddLabel(70, 289, 1265, msg); // ( X, Y, HUE, "Label" )Text in white rectangle
	    AddLabel(65, 319, 332, "Please note when selecting the appearance of a hat or a hooded");
	    AddLabel(65, 334, 332, "robe, hair may show through on the paperdoll and your toon.");

        }

        public class TransObject : Target
        {
            public Mobile m_From;
            public bool del;
            public Item o_item;
            public Item t_item;
            public string msg;
            public bool obj;
            public int m;
            public MorphDeed tdeed;  // Vii edit

            public int weaponitemID;

            public TransObject(MorphDeed deed, Mobile from, Item item, Item mar, int mesg, bool dele, bool ob)
                : base(2, false, TargetFlags.None)  // Vii edit
            {
                tdeed = deed;  // Vii edit
                m_From = from;
                del = dele;
                m = mesg;
                o_item = item;
                t_item = mar;
                obj = ob;
            }

            protected override void OnTarget(Mobile from, object target)
            {

                if (target is BaseWeapon || target is BaseJewel || target is BaseArmor || target is BaseClothing || target is BaseShield || target is BaseSuit)
                {
                    Item m_item = (Item)target;
                    if (m_item.RootParent == m_From)
                    {
                        if (obj == true)
                            o_item = m_item;
                        else
                            t_item = m_item;
                    }
                    else
                        m = 1;
                }
                else
                    m = 2;
                m_From.CloseGump(typeof(ACGump2));
                m_From.SendGump(new ACGump2(tdeed, m_From, o_item, t_item, m, del));   // Vii edit
                return;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        m_deed.Movable = true;  // Vii edit
                        return;
                    }


                case 1:
                    {
                        from.Target = new TransObject(m_deed, m_From, o_item, t_item, m, del, true);  // Vii edit
                        break;
                    }
                case 2:
                    {
                        from.Target = new TransObject(m_deed, m_From, o_item, t_item, m, del, false);  // Vii edit
                        break;
                    }
                case 3:
                    {

                        if (o_item == null || t_item == null)
                        {
                            m = 3;
                            break;
                        }

                        if (o_item.ItemID == t_item.ItemID)
                        {
                            m = 4;
                            break;
                        }

                        if (!o_item.IsChildOf(from.Backpack))
                        {
                            from.SendMessage("The equipment item must be in your backpack.");
                            return;
                        }
                        if (!t_item.IsChildOf(from.Backpack))
                        {
                            from.SendMessage("The appearance item must be in your backpack.");
                            return;
                        }
                        o_item.ItemID = t_item.ItemID;
                        t_item.Delete();
                        m_From.SendMessage("You changed the appearance of your equipment."); //??? {0} ?????????
                        m_From.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                        m_From.PlaySound(0x1EA);
                        m_deed.Delete();  // Vii edit
                        return;


                    }
                case 4:
                    {
                        del = true;
                        m = 8;
                        break;
                    }
                case 5:
                    {
                        del = false;
                        m = 9;
                        break;
                    }

            }
            m_From.CloseGump(typeof(ACGump2));
            m_From.SendGump(new ACGump2(m_deed, m_From, o_item, t_item, m, del));  // Vii edit
            return;

        }
    }
}