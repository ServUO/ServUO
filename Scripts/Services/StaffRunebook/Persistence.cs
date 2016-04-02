/**************************************
*Script Name: Staff Runebook          *
*Author: Joeku                        *
*For use with RunUO 2.0 RC2           *
*Client Tested with: 6.0.9.2          *
*Version: 1.10                        *
*Initial Release: 11/25/07            *
*Revision Date: 02/04/09              *
**************************************/

using System;
using Server;

namespace Joeku.SR
{
    // Legacy... binary serialization only used in v1.00, deserialization preserved to migrate data.
    public class SR_Persistence : Item
    {
        public SR_Persistence()
        {
        }

        public SR_Persistence(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Joeku's Staff Runebook: Loading...");
            Console.WriteLine("  Migrating data from version 1.00... ");
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
                SR_RuneAccount.Deserialize(reader, version);
            Console.WriteLine();

            this.Delete();
        }
    }
}