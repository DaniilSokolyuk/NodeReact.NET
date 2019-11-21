using System;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace NodeReact.AspNetCore
{
    public class ActionHtmlString : IHtmlContent
    {
        private readonly Action<TextWriter> _textWriter;

        /// <summary>
        /// Constructor IHtmlString or IHtmlString action wrapper implementation
        /// </summary>
        /// <param name="textWriter"></param>
        public ActionHtmlString(Action<TextWriter> textWriter)
        {
            _textWriter = textWriter;
        }

        /// <summary>
        /// Writes the content by encoding it with the specified <paramref name="encoder" />
        /// to the specified <paramref name="writer" />.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to which the content is written.</param>
        /// <param name="encoder">The <see cref="T:System.Text.Encodings.Web.HtmlEncoder" /> which encodes the content to be written.</param>
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            _textWriter(writer);
        }
    }
}
