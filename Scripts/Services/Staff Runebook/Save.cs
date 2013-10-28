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
using System.IO;
using System.Xml;

namespace Joeku.SR
{
    public class SR_Save
    {
        public static void WriteData()
        {
            if (!Directory.Exists(SR_Main.SavePath))
                Directory.CreateDirectory(SR_Main.SavePath);

            string filePath = Path.Combine(SR_Main.SavePath, SR_Main.FileName);

            using (StreamWriter op = new StreamWriter(filePath))
            {
                XmlTextWriter xml = new XmlTextWriter(op);

                xml.Formatting = Formatting.Indented;
                xml.IndentChar = '\t';
                xml.Indentation = 1;

                xml.WriteStartDocument(true);

                xml.WriteStartElement("StaffRunebook");

                xml.WriteAttributeString("Version", SR_Main.Version.ToString());

                for (int i = 0; i < SR_Main.Count; i++)
                    WriteAccountNode(SR_Main.Info[i], xml);

                xml.WriteEndElement();

                xml.Close();
            }
        }

        public static void WriteAccountNode(SR_RuneAccount a, XmlTextWriter xml)
        { 
            xml.WriteStartElement("RuneAccount");

            xml.WriteAttributeString("Username", a.Username);
            for (int i = 0; i < a.Count; i++)
                WriteRuneNode(a.Runes[i], xml);

            xml.WriteEndElement();
        }

        public static void WriteRuneNode(SR_Rune r, XmlTextWriter xml)
        {
            xml.WriteStartElement(r.IsRunebook ? "Runebook" : "Rune");

            xml.WriteAttributeString("Name", r.Name);
            if (!r.IsRunebook)
            {
                xml.WriteAttributeString("TargetMap", r.TargetMap.ToString());
                xml.WriteAttributeString("X", r.TargetLoc.X.ToString());
                xml.WriteAttributeString("Y", r.TargetLoc.Y.ToString());
                xml.WriteAttributeString("Z", r.TargetLoc.Z.ToString());
            }
            else
                for (int i = 0; i < r.Count; i++)
                    WriteRuneNode(r.Runes[i], xml);

            xml.WriteEndElement();
        }
    }
}