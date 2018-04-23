using System;
using System.Collections.Generic;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
    /*
		This class is a bit of a hack to make the system drag droppable. We are specifically looking to "override" Carve
		(which is not virtual in the first place). To do this, the old corpse handler is deregistered. Instead, we call the old
		CreateCorpseHandler if the corpse should not have a bounty.
	*/

    public class BountyCorpse : Corpse, ICarvable
    {
        [CallPriority(1)]
        public new static void Initialize()
        {
            Mobile.CreateCorpseHandler -= Corpse.Mobile_CreateCorpseHandler;
            Mobile.CreateCorpseHandler += Mobile_CreateCorpseHandler;
        }

        public new static Container Mobile_CreateCorpseHandler(Mobile owner, HairInfo hair, FacialHairInfo facialhair,
            List<Item> initialContent, List<Item> equipItems)
        {
            var pm = owner as PlayerMobile;
            if (pm == null || BountyInformation.GetBounty(pm) <= 0)
                return Corpse.Mobile_CreateCorpseHandler(owner, hair, facialhair, initialContent, equipItems);

            Corpse c = new BountyCorpse(owner, hair, facialhair, equipItems);

            owner.Corpse = c;

            for (var i = 0; i < initialContent.Count; ++i)
            {
                var item = initialContent[i];

                if (Core.AOS && item.Parent == owner.Backpack)
                    c.AddItem(item);
                else
                    c.DropItem(item);

                if (Core.AOS)
                    c.SetRestoreInfo(item, item.Location);
            }

            if (Core.AOS)
            {
                c.RestoreEquip = pm.EquipSnapshot;
            }

            var loc = owner.Location;
            var map = owner.Map;

            if (map == null || map == Map.Internal)
            {
                loc = owner.LogoutLocation;
                map = owner.LogoutMap;
            }

            c.MoveToWorld(loc, map);

            return c;
        }

        public BountyCorpse(Mobile owner, HairInfo hair, FacialHairInfo facialhair, List<Item> equipItems)
            : base(owner, hair, facialhair, equipItems)
        {
        }

        public BountyCorpse(Serial serial) : base(serial)
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

            var version = reader.ReadInt();
        }

        public new void Carve(Mobile from, Item item)
        {
            base.Carve(from, item);

            var charName = Owner.Name;
            var itemsInRange = GetItemsInRange(0);
            Head oldHead = null;
            foreach (var it in itemsInRange)
            {
                var h = it as Head;
                if (h == null || h is BountiedHead || h.PlayerName != charName)
                    continue;

                oldHead = h;
                break;
            }
            /* RunUO 2.2 IPooledEnumerable does not support ToList()?
            var oldHead = GetItemsInRange(0)
                .ToList()
                .FirstOrDefault(x => x is Head && ((Head) x).PlayerName == charName);*/
            if (oldHead == null)
                return;

            var bh = new BountiedHead(charName)
            {
                Player = Owner as PlayerMobile,
                Bounty = BountyInformation.GetBounty(Owner),
                Created = DateTime.UtcNow
            };
            bh.MoveToWorld(oldHead.Location, oldHead.Map);
            oldHead.Delete();
        }
    }
}