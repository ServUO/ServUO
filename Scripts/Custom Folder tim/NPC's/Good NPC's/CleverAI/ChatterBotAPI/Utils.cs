using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
//using System.Web.Security;
using System.Xml.XPath;
using System.Security.Cryptography;


/*
    ChatterBotAPI
    Copyright (C) 2011 pierredavidbelanger@gmail.com
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace ChatterBotAPI {
	
	static class Utils {
		
		public static string ParametersToWWWFormURLEncoded(IDictionary<string, string> parameters) {
			string wwwFormUrlEncoded = null;
			foreach (string parameterKey in parameters.Keys) {
				string parameterValue = parameters[parameterKey];
                string parameter = string.Format("{0}={1}", Uri.EscapeUriString(parameterKey).Replace("%20", "+"), Uri.EscapeUriString(parameterValue).Replace("%20", "+"));
				if (wwwFormUrlEncoded == null) {
					wwwFormUrlEncoded = parameter;
				} else {
					wwwFormUrlEncoded = string.Format("{0}&{1}", wwwFormUrlEncoded, parameter);
				}
			}
			return wwwFormUrlEncoded;
		}
		
		public static string MD5_m(string input) {
            //return input;
            return HashPasswordForStoringInConfigFile(input, "MD5");
		}
		
		public static string Post(string url, IDictionary<string, string> parameters) {
			string postData = ParametersToWWWFormURLEncoded(parameters);
			byte[] postDataBytes = Encoding.ASCII.GetBytes(postData);
			
			WebRequest webRequest = WebRequest.Create(url);
			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ContentLength = postDataBytes.Length;
			
			Stream outputStream = webRequest.GetRequestStream();
			outputStream.Write(postDataBytes, 0, postDataBytes.Length);
			outputStream.Close();
			
			WebResponse webResponse = webRequest.GetResponse();
			StreamReader responseStreamReader = new StreamReader(webResponse.GetResponseStream());
			return responseStreamReader.ReadToEnd().Trim();
		}
		
		public static string XPathSearch(string input, string expression) {
			XPathDocument document = new XPathDocument(new MemoryStream(Encoding.ASCII.GetBytes(input)));
			XPathNavigator navigator = document.CreateNavigator();
			return navigator.SelectSingleNode(expression).Value;
		}
		
		public static string StringAtIndex(string[] strings, int index) {
			if (index >= strings.Length) return "";
			return strings[index];
		}

        public static string HashPasswordForStoringInConfigFile(string password, string passwordFormat)
        {

       MD5 algorithm;
        algorithm = MD5.Create(); 
        byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password)); 
        string md5 = ""; 
        for (int i = 0; i < data.Length; i++) 
        { 
            md5 += data[i].ToString("x2").ToLowerInvariant(); 
        } 
        return md5; 

        }
	}
}