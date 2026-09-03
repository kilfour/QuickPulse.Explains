### 0.3.18
- Added `DocBarChart` support for rendering numeric tuple data as Mermaid XY bar charts.
- Fixed case-sensitive document links, section anchors, generic anchors, and namespace-prefix matching.
- Made includes recursive, detected include cycles, and preserved `NoHeader` per include occurrence.
- Limited code extraction to referenced examples, preserved blank lines and non-documentation attributes.
- Escaped Markdown table cells and Mermaid chart labels containing syntax-significant characters.

### 0.3.17
- Added a shared link contract for regular links and table links.
- Updated the renderer and writer integration for QuickPulse 0.4.0.

### 0.3.16
- Updated the rendering flows for the newer QuickPulse API.
- Updated the QuickPulse dependency to 0.3.7.

### 0.3.15
- Better Code Parsing: using Roslyn.
- DX improvements:
  - EmptyStringUsedInCodeReplaceAttributeException
  - DocCodeFileNotFoundException
  - DocRawFileNotFoundException
  - CodeExampleNotFoundException

### 0.3.14
- Allow for *partial* inclusion of external file.
- Optional boolean argument on `DocInclude` to omit the file header.

### 0.3.13
- Better DocLinks: navigation outside of docfile containing the link is now possible.
