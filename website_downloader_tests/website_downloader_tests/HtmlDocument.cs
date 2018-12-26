using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace website_downloader_tests
{
    /// <summary>
    /// Represents an html document. Includes some useful methods
    /// to work with html documents.
    /// </summary>
    class HtmlDocument
    {
        // All the elements that does not contain content
        public static readonly string[] NoContentElements = { "area", "base", "basefont", "br", "col", "frame", "hr", "img", "input", "isindex", "meta", "param" };

        public string HtmlCode { get; private set; }                                            // The Html Code                  
        public string CodeWithoutComments { get { return this.GetCodeWithoutComments(); } }     // The html Code without comments

        public HtmlDocument(string htmlCode)
        {
            this.HtmlCode = htmlCode;
        }

        /// <summary>
        /// Returns all elements with given tag name.
        /// </summary>
        /// <returns>All elements with given tag name.</returns>
        public List<HtmlElement> GetElementsByTagName(string tagName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HtmlElement> GetAllElements()
        {
            throw new NotImplementedException();
        }

        #region Private Methods
        /// <summary>
        /// Returns the code without the comments
        /// </summary>
        /// <returns></returns>
        private string GetCodeWithoutComments()
        {
            throw new NotImplementedException();
        }
        #endregion
    }


    /// <summary>
    /// Represents an html element.
    /// </summary>
    class HtmlElement
    {
        public string RawCode { get { return this.ToString(); } }           // The element code as is (with changes)
        public Dictionary<string, string> Attributes { get; private set; }  // The element attributes (can be modified)      
        public string TagName { get; private set; }                         // The element's tag name
        public string Content { get; private set; }                         // The code inside the element
        public int Length { get { return this.RawCode.Length; } }           // The element's length
        public List<HtmlElement> InnerElements { get { return this.GetInnerElements(); } }

        private string code;        // code to work on

        public HtmlElement(string code)
        {
            this.code = code;

            // Assign the fields
            this.TagName = this.GetTagName();
            this.Content = this.GetCodeInside();
            this.Attributes = this.GetAttributes();
        }

        #region private functions
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
                int nextQuotationMarkIndex = rawAttributes.IndexOf(quotationMark, quotationMarkIndex + 1);
                string value = rawAttributes.Slice(quotationMarkIndex + 1, nextQuotationMarkIndex);  // the value of the attribute for example 'src="auto:blank"' -> auto:blank

                key = key.ToLower();        // Make the key lower-case
                attributes[key] = value;    // Add the attribute to the attributes dictionary 
                rawAttributes = rawAttributes.Slice(nextQuotationMarkIndex + 1, rawAttributes.Length); 
            }
           
            return attributes;
        }

        /// <summary>
        /// Whether the element can contain content or not .
        /// img element for example will return false because cannot
        /// contain content. p will return true.
        /// </summary>
        private bool IsNonContentElement
        {
            get
            {
                return HtmlDocument.NoContentElements.Any(tag => tag == this.TagName);
            }
        }

        /// <summary>
        /// Returns all elements inside the (None recursively)
        /// 
        /// TODO: Make it work with e,pty html elements such as img
        /// </summary>
        /// <returns>All elements inside the (None recursively)</returns>
        private List<HtmlElement> GetInnerElements()
        {
            var elements = new List<HtmlElement>();     // All the inner elements

            string code = this.Content;

            // Find all the elements inside the code
            var tempTagNames = new Stack<string>();     // Used to recognize where the tag starts and ends
            int firstIndex;
            string tagName, tagStart, tagEnd;
            while (code.Contains(">")) 
            {
                firstIndex = code.IndexOf("<");
                // Find the end of the tag name
                int tagNameEnd;
                int spaceIndex = code.IndexOf(" ", firstIndex);
                int smallerCharIndex = code.IndexOf(">", firstIndex);
                if (spaceIndex == -1)
                    tagNameEnd = smallerCharIndex;    // If space does not exists, pick the '<' sign index
                else
                    tagNameEnd = Math.Min(spaceIndex, smallerCharIndex);    // If space exists, pick the closer index to the start
                tagName = code.Slice(firstIndex + 1, tagNameEnd);

                int lastIndex = -1;     // The index of the '>' character that closes the tag
                // In case the element contains no content
                if (HtmlDocument.NoContentElements.Any(tag => tag == tagName))
                {
                    lastIndex = code.IndexOf('>', firstIndex + 1);
                }
                // In case the element can contain content
                else
                {
                    tempTagNames.Push(tagName);
                    tagStart = "<" + tagName;       // Each tag inside should start like this
                    tagEnd = "</" + tagName + ">";  // Each tag inside should end like this

                    // As long as the stack is not empty, keep looking for the end of the tag
                    int currentIndex = firstIndex + tagStart.Length;
                    while (tempTagNames.Count > 0)
                    {
                        int tagStartIndex = code.IndexOf(tagStart, currentIndex);
                        int tagEndIndex = code.IndexOf(tagEnd, currentIndex);
                        if (tagEndIndex == -1 && tagStartIndex == -1)
                            throw new Exception("An Error occured");
                        else if (tagEndIndex == -1)
                            tagEndIndex = int.MaxValue;
                        else if (tagStartIndex == -1)
                            tagStartIndex = int.MaxValue;

                        // In case the next tag is an end tag
                        if (tagEndIndex < tagStartIndex)
                        {
                            tempTagNames.Pop();
                            currentIndex = tagEndIndex + tagEnd.Length;
                        }

                        // In case the next tag is an open tag
                        else
                        {
                            tempTagNames.Push(tagName);
                            currentIndex = tagStartIndex + tagStart.Length;
                        }

                        if (tempTagNames.Count == 0)
                            lastIndex = code.IndexOf(">", tagEndIndex);


                    }
                }

                // Add the element to the list
                var element = new HtmlElement(code.Slice(firstIndex, lastIndex + 1));
                elements.Add(element);

                code = code.Substring(lastIndex + 1);
            }

            return elements;
        }
        #endregion

        /// <summary>
        /// Format code from the element's attributes
        /// </summary>
        /// <returns>The raw html code</returns>
        public override string ToString()
        {
            string declaration = string.Format("<{0}", this.TagName);
            foreach (KeyValuePair<string, string> pair in this.Attributes)
            {
                declaration += string.Format(" {0}=\"{1}\"", pair.Key, pair.Value); 
            }
            if (this.IsNonContentElement)
                return declaration + "/>";

            declaration += ">";
            return string.Format("{0}{1}</{2}>", declaration, this.Content, this.TagName);
            
        }
    }
}
