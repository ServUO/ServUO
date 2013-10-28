using System;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlDeathAction : XmlAttachment
    {
        private string m_Action;// action string
        private string m_Condition;// condition string

        // a serial constructor is REQUIRED
        public XmlDeathAction(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlDeathAction(string action)
        {
            this.Action = action;
        }

        [Attachable]
        public XmlDeathAction()
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Action
        {
            get
            {
                return this.m_Action;
            }
            set
            {
                this.m_Action = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Condition
        {
            get
            {
                return this.m_Condition;
            }
            set
            {
                this.m_Condition = value;
            }
        }
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override bool HandlesOnKilled
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
            // version 1
            writer.Write(this.m_Condition);
            // version 0
            writer.Write(this.m_Action);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    this.m_Condition = reader.ReadString();
                    goto case 0;
                case 0:
                    this.m_Action = reader.ReadString();
                    break;
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if (this.AttachedTo is Item)
            {
                // dont allow item attachments
                this.Delete();
            }
        }

        public override void OnKilled(Mobile killed, Mobile killer)
        {
            base.OnKilled(killed, killer);

            if (killed == null)
                return;

            // now check for any conditions as well
            // check for any condition that must be met for this entry to be processed
            if (this.Condition != null)
            {
                string status_str;

                if (!BaseXmlSpawner.CheckPropertyString(null, killed, this.Condition, killer, out status_str))
                {
                    return;
                }
            }

            this.ExecuteDeathActions(killed.Corpse, killer, this.Action);
        }

        private static void ExecuteDeathAction(Item corpse, Mobile killer, string action)
        {
            if (action == null || action.Length <= 0 || corpse == null)
                return;

            string status_str = null;
            Server.Mobiles.XmlSpawner.SpawnObject TheSpawn = new Server.Mobiles.XmlSpawner.SpawnObject(null, 0);

            TheSpawn.TypeName = action;
            string substitutedtypeName = BaseXmlSpawner.ApplySubstitution(null, corpse, killer, action);
            string typeName = BaseXmlSpawner.ParseObjectType(substitutedtypeName);

            Point3D loc = corpse.Location;
            Map map = corpse.Map;

            if (BaseXmlSpawner.IsTypeOrItemKeyword(typeName))
            {
                BaseXmlSpawner.SpawnTypeKeyword(corpse, TheSpawn, typeName, substitutedtypeName, true, killer, loc, map, out status_str);
            }
            else
            {
                // its a regular type descriptor so find out what it is
                Type type = SpawnerType.GetType(typeName);
                try
                {
                    string[] arglist = BaseXmlSpawner.ParseString(substitutedtypeName, 3, "/");
                    object o = Server.Mobiles.XmlSpawner.CreateObject(type, arglist[0]);

                    if (o == null)
                    {
                        status_str = "invalid type specification: " + arglist[0];
                    }
                    else if (o is Mobile)
                    {
                        Mobile m = (Mobile)o;
                        if (m is BaseCreature)
                        {
                            BaseCreature c = (BaseCreature)m;
                            c.Home = loc; // Spawners location is the home point
                        }

                        m.Location = loc;
                        m.Map = map;

                        BaseXmlSpawner.ApplyObjectStringProperties(null, substitutedtypeName, m, killer, corpse, out status_str);
                    }
                    else if (o is Item)
                    {
                        Item item = (Item)o;
                        BaseXmlSpawner.AddSpawnItem(null, corpse, TheSpawn, item, loc, map, killer, false, substitutedtypeName, out status_str);
                    }
                }
                catch
                {
                }
            }
        }

        private void ExecuteDeathActions(Item corpse, Mobile killer, string actions)
        {
            if (actions == null || actions.Length <= 0)
                return;
            // execute any action associated with it
            // allow for multiple action strings on a single line separated by a semicolon

            string[] args = actions.Split(';');

            for (int j = 0; j < args.Length; j++)
            {
                ExecuteDeathAction(corpse, killer, args[j]);
            }
        }
    }
}