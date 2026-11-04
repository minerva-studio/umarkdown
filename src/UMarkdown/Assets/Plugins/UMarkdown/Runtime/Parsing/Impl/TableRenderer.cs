using System.Collections.Generic;
using System.IO;
using Markdig.Extensions.Tables;
using Markdig.Syntax;

namespace UMarkdown.Parsing.Impl {
    /// <summary>
    /// Renders a pipe table as a grid. Cell contents render to rich text through a
    /// temporary writer so inline formatting survives while block boundaries do not
    /// reach the consumer.
    /// </summary>
    public class TableRenderer : RichTextObjectRenderer<Table> {
        protected override void Write(RichTextRenderer renderer, Table table) {
            renderer.PushAsTextBlock();

            var rows = new List<IReadOnlyList<string>>();
            foreach (var block in table) {
                if (block is not TableRow row) continue;

                var cells = new List<string>();
                foreach (var cellBlock in row) {
                    if (cellBlock is TableCell cell) cells.Add(RenderCell(renderer, cell));
                }

                rows.Add(cells);
            }

            var alignments = new List<TableColumnAlign>();
            if (table.ColumnDefinitions != null) {
                foreach (var definition in table.ColumnDefinitions)
                    alignments.Add(definition.Alignment ?? TableColumnAlign.Left);
            }

            renderer.PushTable(rows, alignments);
        }

        private static string RenderCell(RichTextRenderer renderer, TableCell cell) {
            var writer = new StringWriter();
            var previous = renderer.Writer;
            renderer.Writer = writer;
            try {
                foreach (var block in cell) {
                    if (block is LeafBlock leaf) renderer.WriteLeafInline(leaf);
                }
            } finally {
                renderer.Writer = previous;
            }

            return writer.ToString().Trim();
        }
    }
}
