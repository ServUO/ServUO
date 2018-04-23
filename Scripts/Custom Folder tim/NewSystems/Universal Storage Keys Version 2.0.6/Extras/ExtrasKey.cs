//uncomment these to activate the optional extras code
//#define FS_ATS_PETS_ACTIVE
//#define ADVANCED_ARCHERY_ACTIVE

using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.BulkOrders;

namespace Server.Items
{
    //item inherited from BaseResourceKey
    public class ExtrasKey : BaseStoreKey
    {
        public override int DisplayColumns { get { return 2; } }

        public override List<StoreEntry> EntryStructure
        {
            get
            {
                List<StoreEntry> entry = base.EntryStructure;

#if (FS_ATS_PETS_ACTIVE)
				{
					entry.Add( new GenericEntry( typeof( PetLeash ), "Charges", "Pet Leash" ) );
				}
#endif

#if (ADVANCED_ARCHERY_ACTIVE)
				{
					entry.Add( new GenericEntry( typeof( ArmorPiercingDipTub ), "Charges", "Armor Piercing" ) );
					entry.Add( new GenericEntry( typeof( ExplosiveDipTub ), "Charges", "Explosive" ) );
					entry.Add( new GenericEntry( typeof( FreezeDipTub ), "Charges", "Freeze" ) );
					entry.Add( new GenericEntry( typeof( LightningDipTub ), "Charges", "Lightning" ) );
					entry.Add( new GenericEntry( typeof( PoisonDipTub ), "Charges", "Poison" ) );
				}
#endif

                return entry;
            }
        }

        [Constructable]
        public ExtrasKey() : base(0)
        {
            ItemID = 6226;
            Name = "Extras Key - Example Only!";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Extras Example Storage";

            store.Dynamic = false;
            store.OfferDeeds = false;
            return store;
        }

        //serial constructor
        public ExtrasKey(Serial serial) : base(serial)
        {
        }

        //events

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}