using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using PdfA4A5;

namespace PdfA4A5Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Contains("-h") || args.Contains("/?") || args.Contains("--help"))
            {
                ShowHelpUsage();
                return;
            }

            string inputFile = args[0];
            string outputfile = args[1];
            bool separate = args.Length>2 ? args[2].Contains("-separate"): false;

            Console.WriteLine("Processing {0} file started...", inputFile);

            PdfA4A5Engine page = new PdfA4A5Engine(inputFile,outputfile);
            bool result = false;
            if (separate)
            {
                result = page.DoItAndSeparate();
            }
            else
            {
                result = page.DoIt();
            }
            if (result)
                Console.WriteLine("File successfully processed.");
            else
                Console.WriteLine("Something went wrong. An error message is hereunder.\nERROR: {0}", page.ErrorMessage);
        }

        private static void ShowHelpUsage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\n\nPdfA44A5.Console");
            sb.AppendLine("(c) Copyright 2013 by Robert Skarżycki.\n\n\n");
            sb.AppendLine("Usage:\nPdfA4A5.Console <input-file> <output-file> [-separate]\n");
            sb.AppendLine("\tOption -separate isn't required and tell that you want to have two pdf files on output.\n\nEnjoy!");
            Console.WriteLine(sb.ToString());
        }

        
    }
}
