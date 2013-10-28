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
using Server;

namespace Joeku.SR
{
    public class SR_Load
    {
        public static void ReadData(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            Console.WriteLine();
            Console.WriteLine("Joeku's Staff Runebook: Loading...");

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlElement root = doc["StaffRunebook"];
            int version = Utility.ToInt32(root.GetAttribute("Version"));

            if (root.HasChildNodes)
            {
                foreach (XmlElement a in root.GetElementsByTagName("RuneAccount"))
                {
                    try
                    {
                        ReadAccountNode(a);
                    }
                    catch
                    {
                        Console.WriteLine("  Warning: Staff Runebook load failed.");
                    }
                }
            }
            Console.WriteLine();
        }

        public static void ReadAccountNode(XmlElement parent)
        {
            Console.Write("  Account: {0}... ", parent.GetAttribute("Username"));
            try
            {
                SR_RuneAccount acc = new SR_RuneAccount(parent.GetAttribute("Username"));
                if (parent.HasChildNodes)
                {
                    XmlElement child = parent.FirstChild as XmlElement;
                    acc.AddRune(ReadRuneNode(child));
                    while (child.NextSibling != null)
                    {
                        child = child.NextSibling as XmlElement;
                        acc.AddRune(ReadRuneNode(child));
                    }
                    /*foreach( XmlElement child in parent.GetElementsByTagName("Runebook") )
                    if( child != null )
                    acc.AddRune(ReadRunebookNode(child));
                    foreach( XmlElement child in parent.GetElementsByTagName("Rune") )
                    if( child != null )
                    acc.AddRune(ReadRuneNode(child));*/
                }
            }
            catch
            {
                Console.WriteLine("failed.");
            }
            Console.WriteLine("done.");
        }

        public static SR_Rune ReadRuneNode(XmlElement parent)
        {
            if (parent.LocalName == "Runebook")
            {
                SR_Rune runebook = new SR_Rune(parent.GetAttribute("Name"), true);
                if (parent.HasChildNodes)
                {
                    XmlElement child = parent.FirstChild as XmlElement;
                    runebook.AddRune(ReadRuneNode(child));
                    while (child.NextSibling != null)
                    {
                        child = child.NextSibling as XmlElement;
                        runebook.AddRune(ReadRuneNode(child));
                    }
                }
                return runebook;
            }
            //else if( parent.LocalName == "Rune" )

            return new SR_Rune(parent.GetAttribute("Name"), Map.Parse(parent.GetAttribute("TargetMap")), new Point3D(Utility.ToInt32(parent.GetAttribute("X")),Utility.ToInt32(parent.GetAttribute("Y")),Utility.ToInt32(parent.GetAttribute("Z"))));
        }
        /*public static SR_Rune ReadRunebookNode( XmlElement parent )
        {
        SR_Rune rune = new SR_Rune(parent.GetAttribute("Name"), true);
        if( parent.HasChildNodes )
        {
        foreach( XmlElement child in parent.GetElementsByTagName("Runebook") )
        if( child != null )
        rune.AddRune(ReadRunebookNode(child));
        foreach( XmlElement child in parent.GetElementsByTagName("Rune") )
        if( child != null )
        rune.AddRune(ReadRuneNode(child));
        }
        return rune;
        }
        public static SR_Rune ReadRuneNode( XmlElement rune )
        {
        return new SR_Rune(rune.GetAttribute("Name"), Map.Parse(rune.GetAttribute("TargetMap")), new Point3D(Utility.ToInt32(rune.GetAttribute("X")),Utility.ToInt32(rune.GetAttribute("Y")),Utility.ToInt32(rune.GetAttribute("Z"))) );
        }*/
    }
}