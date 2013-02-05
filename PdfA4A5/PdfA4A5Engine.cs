using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfA4A5
{
    public sealed class ErrorMessages
    {
        public static readonly string IOProblem = "There was a problem during open input file.";
        public static readonly string SavingOutputProblem = "There was a problem during saving output to the specified file.";
    };
    
    public class PdfA4A5Engine
    {
    
        private string fileName;
        private XPdfForm form;
        private string outputPath;
        private int pageCountWithModulo;

        public string ErrorMessage {get;set;}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">path to input file</param>
        /// <param name="outputPath">path to output file</param>
        public PdfA4A5Engine(string path, string outputPath)
        {
            try
            {
                fileName = Path.GetFileName(path);                
            }
            catch (IOException)
            {
                ErrorMessage = ErrorMessages.IOProblem.ToString();
                return;
            }
            form = XPdfForm.FromFile(fileName);
            this.outputPath = outputPath;

            pageCountWithModulo = form.PageCount + (form.PageCount % 4);
        }

        /// <summary>
        /// Do the job
        /// </summary>
        /// <returns></returns>
        public bool DoIt()
        {
            PdfDocument outputDocument = new PdfDocument();
            outputDocument.PageLayout = PdfPageLayout.SinglePage;

            ProcessAllPages(ref outputDocument);
            try
            {
                outputDocument.Save(outputPath);
            }
            catch (Exception)
            {
                ErrorMessage = ErrorMessages.SavingOutputProblem.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Do the joba and separate averse and reverse pages
        /// </summary>
        /// <returns></returns>
        public bool DoItAndSeparate()
        {
            PdfDocument outputDocumentFirstHalf = new PdfDocument();
            outputDocumentFirstHalf.PageLayout = PdfPageLayout.SinglePage;

            PdfDocument outputDocumentSecondHalf = new PdfDocument();
            outputDocumentSecondHalf.PageLayout = PdfPageLayout.SinglePage;

            ProcessAversePages(ref outputDocumentFirstHalf);
            string newFileName = string.Concat(Path.GetFileNameWithoutExtension(outputPath), "_part1", ".pdf");
            string pathWithoutFile = Path.GetDirectoryName(outputPath);
            string newPath = Path.Combine(pathWithoutFile, newFileName);
            try
            {
                outputDocumentFirstHalf.Save(newPath);
            }
            catch (Exception)
            {
                ErrorMessage = ErrorMessages.SavingOutputProblem.ToString();
                return false;
            }

            ProcessReversePages(ref outputDocumentSecondHalf);
            newFileName = string.Concat(Path.GetFileNameWithoutExtension(outputPath), "_part2", ".pdf");
            pathWithoutFile = Path.GetDirectoryName(outputPath);
            newPath = Path.Combine(pathWithoutFile, newFileName);
            try
            {
                outputDocumentSecondHalf.Save(newPath);
            }
            catch (Exception)
            {
                ErrorMessage = ErrorMessages.SavingOutputProblem.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Abbreviated form for processing all pages
        /// </summary>
        /// <param name="outputDocument">ref to output document object</param>
        private void ProcessAllPages(ref PdfDocument outputDocument)
        {
            ProcessPages(ref outputDocument, false);
        }

        /// <summary>
        /// Abbreviated form for processing averse pages
        /// </summary>
        /// <param name="outputDocument">ref to output document object</param>
        private void ProcessAversePages(ref PdfDocument outputDocument)
        {
            ProcessPages(ref outputDocument, true, true);
        }

        /// <summary>
        /// Abbreviated form for processing reverse pages
        /// </summary>
        /// <param name="outputDocument">ref to output document object</param>
        private void ProcessReversePages(ref PdfDocument outputDocument)
        {
            ProcessPages(ref outputDocument, true, false);
        }

        /// <summary>
        /// Main method for processing document
        /// </summary>
        /// <param name="outputDocument">ref to output document object</param>
        /// <param name="separateMode">flag for treat averse and reverse pages separately</param>
        /// <param name="isAverse">flag for processing averse pages</param>
        private void ProcessPages(ref PdfDocument outputDocument, bool separateMode, bool isAverse = false)
        {
            /*XFont font = new XFont("Verdana", 8, XFontStyle.Bold);
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Far;*/
            XGraphics gfx;
            XRect box;

            for (int idx = 0, idxReverse = pageCountWithModulo - 1; (idx < pageCountWithModulo) || (idxReverse >= 0); idx += 2, idxReverse -= 2)
            {
                if (!separateMode || isAverse ^ !(idx % 4 == 0))
                {
                    PdfPage page = outputDocument.AddPage();
                    page.Orientation = PageOrientation.Landscape;
                    double width = page.Width;
                    double height = page.Height;

                    int rotate = page.Elements.GetInteger("/Rotate");

                    gfx = XGraphics.FromPdfPage(page);

                    if (idx < form.PageCount && idxReverse > 0)
                    {
                        // Set page number (which is one-based)
                        form.PageNumber = idx + 1;

                        box = new XRect(width / 2, 0, width / 2, height);
                        // Draw the page identified by the page number like an image
                        gfx.DrawImage(form, box);

                        // Write document file name and page number on each page
                        /*box.Inflate(0, -10);
                        gfx.DrawString(String.Format("- {1} -", fileName, idx + 1),
                          font, XBrushes.Red, box, format);*/
                    }
                    if (idxReverse < form.PageCount && idxReverse > 0)
                    {
                        // Set page number (which is one-based)
                        form.PageNumber = idxReverse + 1;

                        box = new XRect(0, 0, width / 2, height);
                        // Draw the page identified by the page number like an image
                        gfx.DrawImage(form, box);

                        // Write document file name and page number on each page
                        /*box.Inflate(0, -10);
                        gfx.DrawString(String.Format("- {1} -", fileName, idx + 2),
                          font, XBrushes.Red, box, format);*/
                    }
                }
            }
        }

        
    }
}
