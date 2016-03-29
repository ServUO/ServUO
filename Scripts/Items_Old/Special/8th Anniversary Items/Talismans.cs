using System;
using Server.Mobiles;
using Server.Spells.Ninjitsu;

namespace Server.Items
{
    public enum TalismanForm
    {
        Ferret = 1031672,
        Squirrel = 1031671,
        CuSidhe = 1031670,
        Reptalon = 1075202
    }

    public class BaseFormTalisman : Item, ITokunoDyable
    {
        public BaseFormTalisman()
            : base(0x2F59)
        {
            this.LootType = LootType.Blessed;
            this.Layer = Layer.Talisman;
            this.Weight = 1.0;
        }

        public BaseFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public virtual TalismanForm Form
        {
            get
            {
                return TalismanForm.Squirrel;
            }
        }
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
            list.Add(1075200, String.Format("#{0}", (int)this.Form));
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
        [Constructable]
        public FerretFormTalisman()
            : base()
        {
        }

        public FerretFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public override TalismanForm Form
        {
            get
            {
                return TalismanForm.Ferret;
            }
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
        [Constructable]
        public SquirrelFormTalisman()
            : base()
        {
        }

        public SquirrelFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public override TalismanForm Form
        {
            get
            {
                return TalismanForm.Squirrel;
            }
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
        [Constructable]
        public CuSidheFormTalisman()
            : base()
        {
        }

        public CuSidheFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public override TalismanForm Form
        {
            get
            {
                return TalismanForm.CuSidhe;
            }
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
        [Constructable]
        public ReptalonFormTalisman()
            : base()
        {
        }

        public ReptalonFormTalisman(Serial serial)
            : base(serial)
        {
        }

        public override TalismanForm Form
        {
            get
            {
                return TalismanForm.Reptalon;
            }
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