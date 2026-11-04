# Fork notes

Forked from [helightdev/UMarkdown](https://github.com/helightdev/UMarkdown) at `fd73703` (package version 0.0.1).
Upstream had no commits for a year, and we needed three things it does not do.

## Changes

- **Pipe tables**: the pipeline now enables `UsePipeTables`, `TableRenderer` walks `Table` /
  `TableRow` / `TableCell`, and the default factory builds a grid of labels with header
  emphasis and column alignment.
- **Images**: `LinkInline` with `IsImage` becomes an image block instead of a link. The default
  factory resolves absolute paths and `file://` URIs, caches textures, and falls back to a
  label with the alt text when a file is missing. Relative paths resolve against
  `ImageBaseDirectory` on the factory.
- **Links**: element rendering converts `<a href>` to UI Toolkit `<link>` tags. (Click handling
  is not wired yet.)
- **HeadingScale**: new style option, because the default 2.25x heading is too large for a chat
  transcript.

## Not changed

- The bundled Markdig, ColorCode and System.Text.Json DLLs stay as they are; the JSON dependency
  did not conflict under Unity 6000.3.
- Public API otherwise matches upstream.

Maintained for the Minerva Codex Assistant package.
