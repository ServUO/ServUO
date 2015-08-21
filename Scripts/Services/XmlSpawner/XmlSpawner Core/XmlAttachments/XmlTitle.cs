using System;
using System.Collections.Generic;

namespace Server.Engines.XmlSpawner2
{
    public class XmlTitle : XmlAttachment
    {
        private string m_Title = null;// title string

        // a serial constructor is REQUIRED
        public XmlTitle(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlTitle(string name)
        {
            this.Name = name;
            this.Title = String.Empty;
        }

        [Attachable]
        public XmlTitle(string name, string title)
        {
            this.Name = name;
            this.Title = title;
        }

        [Attachable]
        public XmlTitle(string name, string title, double expiresin)
        {
            this.Name = name;
            this.Title = title;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Title
        {
            get
            {
                return this.m_Title;
            }
            set
            { 
                this.m_Title = value;
                // change the title
                if (this.AttachedTo is Mobile)
                {
                    ((Mobile)this.AttachedTo).InvalidateProperties();
                }
                if (this.AttachedTo is Item)
                {
                    ((Item)this.AttachedTo).InvalidateProperties();
                }
            }
        }
        public static void AddTitles(object o, ObjectPropertyList list)
        {
            List<XmlAttachment> alist = XmlAttach.FindAttachments(o, typeof(XmlTitle));

            if (alist != null && alist.Count > 0)
            {
                string titlestring = null;
                bool hastitle = false;
                foreach (XmlTitle t in alist)
                {
                    if (t == null || t.Deleted)
                        continue;

                    if (hastitle)
                    {
                        titlestring += '\n';
                    }
                    titlestring += Utility.FixHtml(t.Title);
                    hastitle = true;
                }
                if (hastitle)
                    list.Add(1070722, "<BASEFONT COLOR=#E6CC80>{0}<BASEFONT COLOR=#FFFFFF>", titlestring);
            }
        }

        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write((string)this.m_Title);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_Title = reader.ReadString();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            // remove the title when deleted
            if (this.AttachedTo is Mobile)
            {
                ((Mobile)this.AttachedTo).InvalidateProperties();
            }
            if (this.AttachedTo is Item)
            {
                ((Item)this.AttachedTo).InvalidateProperties();
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            // apply the title immediately when attached
            if (this.AttachedTo is Mobile)
            {
                ((Mobile)this.AttachedTo).InvalidateProperties();
            }
            if (this.AttachedTo is Item)
            {
                ((Item)this.AttachedTo).InvalidateProperties();
            }
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.AccessLevel < AccessLevel.Counselor)
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("{2}: Title {0} expires in {1} mins", this.Title, this.Expiration.TotalMinutes, this.Name);
            }
            else
            {
                return String.Format("{1}: Title {0}", this.Title, this.Name);
            }
        }
    }
}