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