using System;
using System.IO;
using System.Net.Mail;
using Server.Targeting;
using Server.Mobiles;
using System.Globalization;

namespace Server.Commands.Generic
{
    public class GenScriptCommand : BaseCommand
    {
        private static bool m_SendEmail = false;
        // used by m_SendEmail = false
        private static string m_ScriptDirectory = "."; // Directory where to store your newly generated scripts
        // used by m_SendEmail = true
        private static string m_Hostname = "myhostname"; // Hostname of your mailserver
        private static string m_User = "myusername"; // Username for your sending email account
		private static string m_Password = "mypassword"; // password for your sending email account
		private static string m_Sender = "sender@myhostname"; // senders email to use
        private static string m_Recipient = "recipient@myhostname"; // recipient's email

        public static void Initialize()
        {
            Server.Commands.CommandSystem.Register( "genscript",
                                                    AccessLevel.GameMaster,
                                                    new CommandEventHandler( GenScript_OnCommand ) );
        }

        [Usage( "genscript" )]
        [Description( "Generates a script for a targeted item." )]
        public static void GenScript_OnCommand( CommandEventArgs e )
        {
            if ( e.Length != 1 )
            {
                e.Mobile.SendMessage( "Format: genscript <ClassName>" );
                return;
            }

            e.Mobile.Target = new GenScriptTarget( e.GetString( 0 ) );
            e.Mobile.SendMessage( "What item do you want to script?" );
        }

        #region Nested type: GenScriptTarget
        private class GenScriptTarget : Target
        {
            private readonly string ClassName;

            public GenScriptTarget( string classname )
                : base( 15, false, TargetFlags.None )
            {
                ClassName = classname;
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                Item it = null;
                BaseCreature m = null;
                if (targ is Item)
                    it = targ as Item;
                else if (targ is BaseCreature)
                    m = targ as BaseCreature;

                if ( it == null && m == null ) return;

                FileStream m_File = null;
                MemoryStream m_Stream = null;
                StreamWriter op = null;
                try
                {

					if( !m_SendEmail )
					{
                        // Change this part if you want to place your newly created script at some special place
						m_File = new FileStream( String.Format( "{0}/{1}.cs", m_ScriptDirectory, ClassName ), FileMode.Create ); 

                        using ( op = new StreamWriter(m_File))
						{
                            if( it != null )
							    WriteItem(op, it);
                            else if ( m != null )
                                WriteMobile(op, m);
                            op.Close();
							m_File.Close();
						}
					}
					else
					{
                        m_Stream = new MemoryStream();
					
						using ( op = new StreamWriter( m_Stream ) )
						{
                            if (it != null)
                                WriteItem(op, it);
                            else if (m != null)
                                WriteMobile(op, m);

							MailMessage message = new MailMessage( m_Sender,
																m_Recipient,
																String.Format( "Skript {0}", ClassName ),
																"Blub" );

							m_Stream.Position = 0;
							Attachment data = new Attachment( m_Stream, String.Format( "{0}.cs", ClassName ) );
							message.Attachments.Add( data );

							//Send the message.
							SmtpClient client = new SmtpClient( m_Hostname );
							// Add credentials if the SMTP server requires them.
							client.UseDefaultCredentials = false;
							client.Credentials = new System.Net.NetworkCredential( m_User, m_Password );
							client.Send( message );
						}
					}
                    from.SendMessage( "Script generated" );
                }
                catch ( Exception ex )
                {
                    if (op != null)
                        op.Close();
                    if( m_File != null )
                        m_File.Close();
                    if( m_Stream != null )
                        m_Stream.Close();
                    from.SendMessage("Scripting failed");

                    Console.WriteLine( ex );
				}
            }

            public void WriteItem(StreamWriter op, Item it)
            {
                op.WriteLine("using System;");
                op.WriteLine(" ");
                op.WriteLine("namespace Server.Items");
                op.WriteLine("{");
                op.WriteLine(String.Format("    public class {0} : {1}", ClassName, it.GetType().Name));
                op.WriteLine("    {");
                op.WriteLine("        [Constructable]");
                op.WriteLine(String.Format("        public {0}() : base( {1} )", ClassName, it.ItemID.ToString()));
                op.WriteLine("        {");
                op.WriteLine(String.Format("            this.Name = \"{0}\";", it.Name));
                op.WriteLine(String.Format("            this.Hue = {0};", it.Hue.ToString()));
                op.WriteLine("        }");
                op.WriteLine(" ");
                op.WriteLine(String.Format("        public {0}( Serial serial ) : base( serial )", ClassName));
                op.WriteLine("        {");
                op.WriteLine("        }");
                op.WriteLine(" ");
                op.WriteLine("        public override void Serialize( GenericWriter writer )");
                op.WriteLine("        {");
                op.WriteLine("            base.Serialize( writer );");
                op.WriteLine(" ");
                op.WriteLine("            writer.Write( (int) 0 ); // version");
                op.WriteLine("        }");
                op.WriteLine(" ");
                op.WriteLine("        public override void Deserialize( GenericReader reader )");
                op.WriteLine("        {");
                op.WriteLine("            base.Deserialize( reader );");
                op.WriteLine(" ");
                op.WriteLine("            int version = reader.ReadInt();");
                op.WriteLine("        }");
                op.WriteLine("    }");
                op.WriteLine("}");
                op.Flush();
            }
            public void WriteMobile(StreamWriter op, BaseCreature m)
            {
                op.WriteLine("using System;");
                op.WriteLine(" ");
                op.WriteLine("namespace Server.Mobiles");
                op.WriteLine("{");
                op.WriteLine(String.Format("    public class {0} : {1}", ClassName, m.GetType().Name));
                op.WriteLine("    {");
                op.WriteLine("        [Constructable]");
                op.WriteLine(String.Format("        public {0}() : base( AIType.{1}, FightMode.{2}, {3}, {4}, {5}, {6} )", ClassName, m.AI.ToString(), m.FightMode.ToString(), m.RangePerception, m.RangeFight, m.ActiveSpeed.ToString("0.0", CultureInfo.InvariantCulture), m.PassiveSpeed.ToString("0.0", CultureInfo.InvariantCulture)));
                op.WriteLine("        {");
                op.WriteLine(String.Format("            this.Name = \"{0}\";", m.Name));
                op.WriteLine(String.Format("            this.Body = {0};", m.BodyValue));
                op.WriteLine(String.Format("            this.Hue = {0};", m.Hue.ToString()));
                op.WriteLine("        }");
                op.WriteLine(" ");
                op.WriteLine(String.Format("        public {0}( Serial serial ) : base( serial )", ClassName));
                op.WriteLine("        {");
                op.WriteLine("        }");
                op.WriteLine(" ");
                op.WriteLine("        public override void Serialize( GenericWriter writer )");
                op.WriteLine("        {");
                op.WriteLine("            base.Serialize( writer );");
                op.WriteLine(" ");
                op.WriteLine("            writer.Write( (int) 0 ); // version");
                op.WriteLine("        }");
                op.WriteLine(" ");
                op.WriteLine("        public override void Deserialize( GenericReader reader )");
                op.WriteLine("        {");
                op.WriteLine("            base.Deserialize( reader );");
                op.WriteLine(" ");
                op.WriteLine("            int version = reader.ReadInt();");
                op.WriteLine("        }");
                op.WriteLine("    }");
                op.WriteLine("}");
                op.Flush();
            }
        }
		
        #endregion
    }
}