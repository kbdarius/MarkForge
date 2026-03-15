# Markdown Extended – Mermaid Export Fix

This repository contains the patch files needed to fix the "Markdown Extended" (`jebbs.markdown-extended-1.1.4`) VS Code extension so it can successfully export Mermaid diagrams (v10+) to HTML without throwing syntax errors and without leaving them as raw text blocks.

## The Issue
By default, the `jebbs.markdown-extended` HTML exporter misses the `.mermaid` conversions and cannot properly mount the Mermaid ESM library for modern diagram rendering. 

## How to Apply the Patch (Manual Setup)

1. **Copy the Mermaid engine to your user profile:**
   Copy the `patch/mermaid.min.js` file into this exact folder on your PC:
   `C:\Users\<YourUsername>\.vscode\local\mermaid.min.js`

2. **Patch the Extension:**
   Copy `patch/shared.js` and overwrite the existing one inside your installed extension folder:
   `C:\Users\<YourUsername>\.vscode\extensions\jebbs.markdown-extended-1.1.4\out\src\services\exporter\shared.js`

3. **Update VS Code User Settings:**
   Open your global VS Code Settings (JSON) (`Ctrl+Shift+P` -> `Preferences: Open User Settings (JSON)`) and add these two lines:
   ```json
   "markdown-pdf.mermaidServer": "file:///C:/Users/Dariu/.vscode/local/mermaid.min.js",
   "markdownExtended.exportOutDirName": ""
   ```
   *(Note: Change `Dariu` in the path to your Windows username if moving to a completely different PC).*

4. **Restart VS Code:** Run `Developer: Reload Window` for the patched extension file to take effect. 

## Automated Setup Script
You can just run the included `Apply-Patch.ps1` script to do steps 1 & 2 for you automatically on any new machine!
## Troubleshooting Mermaid Syntax Errors (Post-Patch)

Even after applying the patch, Mermaid may show a large **"Syntax error in text"** box when exporting to HTML. That means the rendering engine is **working perfectly**, but it encountered strict formatting errors inside your markdown diagram code.

Here are the most common syntax issues that will break your exported graphs, and how to fix them:

### 1. Quotes inside Database/Cylinder Notations
Mermaid is very strict about shapes and strings. Using strings with quotes inside a cylinder shape directly can break it.
*    **Breaks:** `Node[("Database.json")]` (Causes a parse error)
*    **Fix:** Just use quotes in a standard rectangle instead: `Node["Database.json"]` or avoid quotes in the cylinder: `Node[(Database.json)]`

### 2. Subgraph Titles with Brackets
Using formal bracket structures for subgraph titles can sometimes trip the parser depending on spaces.
*    **Breaks:** `subgraph App ["EmailManage App"]` or `subgraph App [EmailManage]`
*    **Fix:** Just use pure unquoted text for the subgraph name: `subgraph EmailManage App` or wrap the whole title in quotes: `subgraph "EmailManage App"`

### 3. Special Characters Without Quotes
If a label contains parentheses, brackets, or unusual punctuation, you MUST wrap it in double quotes. 
*    **Breaks:** `A[Process User Request (POST)]`
*    **Fix:** `A["Process User Request (POST)"]`

### 4. Arrow Spaces
While modern Mermaid is more forgiving, older versions strongly prefer spaces around arrows for proper tokenization.
*    **Breaks/Buggy:** `A-->B` or `A==>B`
*    **Fix:** `A --> B` or `A ==> B`

### 5. Multi-line Node Text
A common habit with AI-generated diagrams is doing standard HTML line breaks, but syntax rules matter.
*    **Breaks:** Using unquoted HTML tags: `A[Step 1 <br/> Step 2]` 
*    **Fix:** Put it all in double quotes: `A["Step 1 <br/> Step 2"]`

**Quick Tip:** If you see an error say _"Expecting 'SPACE', 'GRAPH', 'DIR'... got 'PS'"_ or something similar, look exactly at the line number mentioned in the error box! It will point straight to the typo in your markdown.
