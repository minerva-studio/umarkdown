using System.Collections.Generic;
using Markdig.Extensions.Tables;
using UMarkdown.Parsing;
using UnityEngine.UIElements;

namespace UMarkdown.VisualElements {
    [UxmlObject]
    public abstract partial class MarkdownVisualElementFactory {
        public abstract VisualElement Compose(RichTextRenderer renderer, IReadOnlyList<VisualElement> blocks);
        public abstract VisualElement TextBlock(RichTextRenderer renderer, string richText);
        public abstract VisualElement CodeBlock(RichTextRenderer renderer, string richText);

        public abstract VisualElement IndentContainer(RichTextRenderer renderer, List<VisualElement> blocks,
            string prefix = null);

        public abstract VisualElement VerticalSpacer(RichTextRenderer renderer);

        /// <summary>Creates the element for a Markdown image. Return a fallback when it cannot be loaded.</summary>
        public abstract VisualElement ImageBlock(RichTextRenderer renderer, string url, string alt);

        /// <summary>Creates the element for a pipe table. Cell strings contain Unity rich text.</summary>
        public abstract VisualElement Table(RichTextRenderer renderer, IReadOnlyList<IReadOnlyList<string>> rows,
            IReadOnlyList<TableColumnAlign> alignments);
    }
}
