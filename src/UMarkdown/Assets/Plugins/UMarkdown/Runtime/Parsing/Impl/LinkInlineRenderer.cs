using System.Text;
using Markdig.Syntax.Inlines;

namespace UMarkdown.Parsing.Impl {
    public class LinkInlineRenderer : RichTextObjectRenderer<LinkInline> {
        protected override void Write(RichTextRenderer renderer, LinkInline obj) {
            if (obj.IsImage) {
                var url = obj.GetDynamicUrl != null ? obj.GetDynamicUrl() : obj.Url;
                renderer.PushImage(url, CollectAltText(obj));
                return;
            }

            renderer.Write("<a href=\"");
            renderer.Write(obj.GetDynamicUrl != null ? obj.GetDynamicUrl() : obj.Url);
            renderer.Write("\">");
            renderer.WriteChildren(obj);
            renderer.Write("</a>");
        }

        private static string CollectAltText(LinkInline link) {
            var builder = new StringBuilder();
            var inline = link.FirstChild;
            while (inline != null) {
                if (inline is LiteralInline literal) builder.Append(literal.Content.ToString());
                inline = inline.NextSibling;
            }

            return builder.ToString();
        }
    }
}
