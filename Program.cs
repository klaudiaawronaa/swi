using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

/* ***** Author: Klaudia Wrona **** */
/* ***** Converting file from .xml to .json **** */

namespace swi
{
    class Program
    {
        const string PATH_INPUT = "input.xml";
        const string PATH_OUTPUT = "output.json";
        static XmlSerializer serializer;
        static FileStream fileStream;
        static Objects result;


        static void Main(string[] args)

        {

            /* ***** DESERIALIZATION **** */
            try
            {
                serializer = new XmlSerializer(typeof(Objects));
                fileStream = new FileStream(PATH_INPUT, FileMode.Open);
                result = (Objects)serializer.Deserialize(fileStream);
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }


            /* ***** CONVERTING TO .JSON **** */
            string JSONResult = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
            JSONResult = JSONResult.Replace("{", "");

            foreach (string myString in JSONResult.Split(new string[] { Environment.NewLine }, 
                    StringSplitOptions.RemoveEmptyEntries))
            {
                if (myString.Contains("\"\": ["))
                    JSONResult = JSONResult.Replace(myString, "");
                if (myString.Contains("\":") && !myString.Contains(","))
                    JSONResult = JSONResult.Replace(myString, myString + "{");
                //will remove indents 
                if (myString.Length >= 2)
                    JSONResult = JSONResult.Replace(myString, myString.Substring(2));
                   
            }

            JSONResult = JSONResult.Replace("{{", "[");
            JSONResult = JSONResult.Replace("{", "");
            JSONResult = JSONResult.Replace("[", "{");
            JSONResult = JSONResult.Replace("]", "");
            //^\s+$ will remove everything from the first blank line to the last (in a contiguous block of empty lines)
            //[\r\n]* will remove the last LF -> End Of Line 
            JSONResult = Regex.Replace(JSONResult, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
            JSONResult = JSONResult.Insert(0, "{" + Environment.NewLine);
            //will add first {


            var tw = new StreamWriter(PATH_OUTPUT, false);
                tw.WriteLine(JSONResult.ToString());
                tw.Close();

            Console.WriteLine("Conversion complete, see " + PATH_OUTPUT + " for resulting .JSON.");
            Console.ReadKey();



        }
        

       
    }
}
