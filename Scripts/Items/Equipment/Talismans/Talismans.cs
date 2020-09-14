using Server.Mobiles;
using Server.Spells.Ninjitsu;
using System;

namespace Server.Items
{
    public enum TalismanForm
    {
        Ferret = 1031672,
        Squirrel = 1031671,
        CuSidhe = 1031670,
        Reptalon = 1075202
    }

    public class BaseFormTalisman : Item
    {
        public BaseFormTalisman()
            : base(0x2F59)
        {
            LootType = LootType.Blessed;
            Layer = Layer.Talisman;
            Weight = 1.0;
        }

        public BaseFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public virtual TalismanForm Form => TalismanForm.Squirrel;

        public static bool EntryEnabled(Mobile m, Type type)
        {
            if (type == typeof(Squirrel))
                return m.Talisman is SquirrelFormTalisman;
            else if (type == typeof(Ferret))
                return m.Talisman is FerretFormTalisman;
            else if (type == typeof(CuSidhe))
                return m.Talisman is CuSidheFormTalisman;
            else if (type == typeof(Reptalon))
                return m.Talisman is ReptalonFormTalisman;

            return true;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1075200, string.Format("#{0}", (int)Form));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                Mobile m = (Mobile)parent;

                AnimalForm.RemoveContext(m, true);
            }
        }
    }

    public class FerretFormTalisman : BaseFormTalisman
    {
        public override TalismanForm Form => TalismanForm.Ferret;

        [Constructable]
        public FerretFormTalisman()
            : base()
        {
        }

        public FerretFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class SquirrelFormTalisman : BaseFormTalisman
    {
        public override TalismanForm Form => TalismanForm.Squirrel;

        [Constructable]
        public SquirrelFormTalisman()
            : base()
        {
        }

        public SquirrelFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class CuSidheFormTalisman : BaseFormTalisman
    {
        public override TalismanForm Form => TalismanForm.CuSidhe;

        [Constructable]
        public CuSidheFormTalisman()
            : base()
        {
        }

        public CuSidheFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class ReptalonFormTalisman : BaseFormTalisman
    {
        public override TalismanForm Form => TalismanForm.Reptalon;

        [Constructable]
        public ReptalonFormTalisman()
            : base()
        {
        }

        public ReptalonFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}