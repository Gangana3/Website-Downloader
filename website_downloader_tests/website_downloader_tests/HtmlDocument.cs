using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace website_downloader_tests
{
    /// <summary>
    /// This class enables to retreive data from html code and change it if needed
    /// </summary>
    class HtmlDocument
    {
        public string HtmlCode { get; private set; }

        public HtmlDocument(string htmlCode)
        {
            this.HtmlCode = htmlCode;
        }

        /// <summary>
        /// Return
        /// </summary>
        /// <returns></returns>
        public List<HtmlElement> GetElementsByTagName()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Represents an html element
    /// </summary>
    class HtmlElement
    {
        public string RawCode { get { return this.ToString(); } }           // The element code as is (with changes)
        public Dictionary<string, string> Attributes { get; private set; }  // The element attributes (can be modified)      
        public string TagName { get; private set; }                         // The element's tag name
        public string CodeInside { get; private set; }                      // The code inside the element
        public int Length { get { return this.RawCode.Length; } }           // The element's length

        private string code;        // code to work on

        public HtmlElement(string code)
        {
            this.code = code;

            // Assign the fields
            this.TagName = this.GetTagName();
            this.CodeInside = this.GetCodeInside();
            this.Attributes = this.GetAttributes();
        }

        /// <summary>
        /// Returns the tag name from the code
        /// </summary>
        /// <returns>Tag Name</returns>
        private string GetTagName()
        {
            int firstIndex = this.code.IndexOf('<');    // Start index of the declaration
            int lastIndex = this.code.IndexOf('>');     // End index of the declaration
            
            string declaration = this.code.Slice(firstIndex + 1, lastIndex);    //for example '<link href="">' -->  'link href=""'
            return declaration.Split(' ')[0];
        }


        /// <summary>
        /// Returns the code inside the element (if contains any code, img element for example does 
        /// not contain any inner code).
        /// </summary>
        /// <returns>The code inside the element</returns>
        private string GetCodeInside()
        {
            if (this.code.Count((c) => c == '<') > 1)
                return this.code.Slice(this.code.IndexOf('>') + 1, this.code.LastIndexOf('<'));
            else
                return "";
        }

        /// <summary>
        /// Returns the attributes of the html element
        /// </summary>
        /// <returns>The attributes of the html element</returns>
        private Dictionary<string, string> GetAttributes()
        {
            int firstIndex = this.code.IndexOf('<');    // Start index of the declaration
            int lastIndex = this.code.IndexOf('>');     // End index of the declaration
            string declaration = this.code.Slice(firstIndex + 1, lastIndex);    //for example '<link href="">' -->  'link href=""'
            string rawAttributes = declaration.Slice(declaration.IndexOf(' ') + 1, declaration.Length); // for example: "src='auto:blank' style='width: 100%'"

            // Build the attributes dictionary
            var attributes = new Dictionary<string, string>();
            while (rawAttributes.Contains('"') || rawAttributes.Contains('\''))
            {
                // if raw attributes starts with space, remove it
                rawAttributes = rawAttributes.Strip();

                // get the quatation mark for the attribute for example: "src='auto:blank'" -> "'"
                char quotationMark;
                if (rawAttributes.Contains('"') && rawAttributes.Contains('\''))
                {
                    if (rawAttributes.IndexOf('"') < rawAttributes.IndexOf('\''))
                        quotationMark = '"';
                    else
                        quotationMark = '\'';
                }
                else if (rawAttributes.Contains('"'))
                    quotationMark = '"';
                else
                    quotationMark = '\'';

                // Get the key and the value of the attribute
                int equalsIndex = rawAttributes.IndexOf('=');
                string key = rawAttributes.Slice(0, equalsIndex);   // key of the attribute for example src or href
                int quotationMarkIndex = rawAttributes.IndexOf(quotationMark);
                Console.WriteLine("DEBUG: {0}", rawAttributes);
                int nextQuotationMarkIndex = rawAttributes.IndexOf(quotationMark, quotationMarkIndex + 1);
                string value = rawAttributes.Slice(quotationMarkIndex + 1, nextQuotationMarkIndex);  // the value of the attribute for example 'src="auto:blank"' -> auto:blank

                attributes[key] = value;    // Add the attribute to the attributes dictionary
                rawAttributes = rawAttributes.Slice(nextQuotationMarkIndex + 1, rawAttributes.Length); 
            }

            return attributes;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
