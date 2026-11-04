using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Markdig.Extensions.Tables;
using UMarkdown.Parsing;
using UnityEngine.UIElements;

namespace UMarkdown.VisualElements {
    public class VisualElementRichTextConsumer : IRichTextConsumer {
        private readonly MarkdownVisualElementFactory _factory;
        private readonly Stack<BlockFrame> _frames = new();

        public VisualElementRichTextConsumer(MarkdownVisualElementFactory factory) {
            _factory = factory;
            _frames.Push(new ComposeBlockFrame());
        }

        private BlockFrame CurrentFrame => _frames.Peek();

        public void AcceptCodeBlock(RichTextRenderer renderer, string richText) {
            var element = _factory.CodeBlock(renderer, richText);
            CurrentFrame.AddBlock(element);
        }

        public void AcceptTextBlock(RichTextRenderer renderer, string richText) {
            var element = _factory.TextBlock(renderer, ToUiToolkitLinks(richText.Trim()));
            CurrentFrame.AddBlock(element);
        }

        public void AcceptImage(RichTextRenderer renderer, string url, string alt) {
            CurrentFrame.AddBlock(_factory.ImageBlock(renderer, url, alt));
        }

        public void AcceptTable(RichTextRenderer renderer, IReadOnlyList<IReadOnlyList<string>> rows,
            IReadOnlyList<TableColumnAlign> alignments) {
            CurrentFrame.AddBlock(_factory.Table(renderer, rows, alignments));
        }

        public void BeginIndent(RichTextRenderer renderer, string prefix = null) {
            _frames.Push(new IndentBlockFrame { Prefix = prefix });
        }

        public void EndIndent(RichTextRenderer renderer) {
            CloseFrame<IndentBlockFrame>(renderer);
        }

        public void EnsureVerticalSpace(RichTextRenderer renderer) {
            var element = _factory.VerticalSpacer(renderer);
            CurrentFrame.AddBlock(element);
        }

        public object Finalize(RichTextRenderer renderer) {
            // Close all remaining frames
            while (_frames.Count > 1) CloseFrame<BlockFrame>(renderer);

            return _factory.Compose(renderer, CurrentFrame.Blocks);
        }

        public bool CloseFrame<T>(RichTextRenderer renderer) {
            if (CurrentFrame is not T || _frames.Count <= 1) return false;
            var frame = _frames.Pop();
            var element = frame.CloseFrame(renderer, _factory);
            CurrentFrame.AddBlock(element);
            return true;
        }

        /// <summary>
        /// UI Toolkit rich text uses &lt;link&gt;, TextMeshPro rich text uses &lt;a href&gt;.
        /// The renderer emits the TMP form; convert it for the element based path.
        /// </summary>
        public static string ToUiToolkitLinks(string richText) {
            if (string.IsNullOrEmpty(richText) || richText.IndexOf("<a href=", StringComparison.Ordinal) < 0)
                return richText;

            return Regex.Replace(richText, "<a href=\"([^\"]*)\">", "<link=\"$1\">")
                .Replace("</a>", "</link>");
        }
    }

    public abstract class BlockFrame {
        public List<VisualElement> Blocks { get; } = new();

        public void AddBlock(VisualElement element) {
            Blocks.Add(element);
        }

        public abstract VisualElement CloseFrame(RichTextRenderer renderer, MarkdownVisualElementFactory factory);
    }

    public class ComposeBlockFrame : BlockFrame {
        public override VisualElement CloseFrame(RichTextRenderer renderer, MarkdownVisualElementFactory factory) {
            return factory.Compose(renderer, Blocks);
        }
    }

    public class IndentBlockFrame : BlockFrame {
        public string Prefix { get; set; }

        public override VisualElement CloseFrame(RichTextRenderer renderer, MarkdownVisualElementFactory factory) {
            return factory.IndentContainer(renderer, Blocks, Prefix);
        }
    }
}
