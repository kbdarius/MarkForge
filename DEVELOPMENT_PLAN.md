# MarkForge - Development Plan

## 1. Product Name
**MarkForge**

Tagline: *Reliable Markdown to Word conversion with table and diagram fidelity.*

## 2. Product Goal
Build a Windows standalone GUI app that converts `.md` to `.docx` with high fidelity for:
- tables
- diagrams (especially Mermaid)
- headings/styles/layout

Primary delivery target: a distributable `.exe` that users can run on their own PC without using terminal commands.

## 3. Chosen Toolset (and Why)
### Core App
- **Language/Runtime:** C# 12 + .NET 8
- **Desktop UI:** WPF (Windows-native, stable, strong tooling)
- **Architecture:** MVVM (`CommunityToolkit.Mvvm`)

### Conversion Engine
- **Pandoc pipeline** (same proven conversion backbone)
- **Lua filters** for table + diagram fixes
- **Reference DOCX template** for consistent Word styles
- **Open XML SDK** for post-processing (ex: landscape/orientation) without PowerShell dependency

### Packaging/Distribution
- `dotnet publish` self-contained single-file build for `win-x64`
- Bundle required conversion assets with app

### Quality
- **Tests:** xUnit + FluentAssertions
- **Lint/format:** `dotnet format`
- **Logging:** structured log file under `%LOCALAPPDATA%\MarkForge\logs`

## 4. High-Level Architecture
- **UI Layer:** file/folder pickers, options, progress, output logs
- **Application Layer:** conversion job orchestration + validation
- **Conversion Layer:** Pandoc command builder + process runner + filter pipeline
- **Post-Process Layer:** DOCX fixes (orientation, optional cleanup)
- **Settings Layer:** JSON config + reusable conversion profiles

## 5. Phase Plan

### Phase 0 - Foundation and Skeleton
**Objective:** establish project baseline and UI shell.

Deliverables:
- solution scaffold (`MarkForge.sln`)
- WPF app shell (navigation + main conversion panel)
- config model + local settings storage
- placeholder conversion service + log panel

Demo for you:
- launch app
- choose sample files/folders in UI
- save/load basic settings

**Gate:** stop and get your approval before Phase 1.

---

### Phase 1 - Single-File Conversion MVP
**Objective:** replace `.bat` flow with GUI for one-file conversion.

Deliverables:
- choose input `.md` file and output folder
- run Pandoc conversion to `.docx`
- support reference template + Lua filters
- options: TOC on/off, portrait/landscape, highlight style
- conversion status + detailed error log

Demo for you:
- run conversion on your real markdown sample
- verify Word output style and orientation

**Gate:** stop and collect your feedback/fixes before Phase 2.

---

### Phase 2 - Fidelity Improvements (Tables + Diagrams)
**Objective:** target the pain points you called out.

Deliverables:
- robust table style handling via template profiles
- Mermaid diagram conversion path (default online renderer + graceful fallback messaging)
- preflight checks for missing assets/images
- better diagnostics when conversion output is degraded

Demo for you:
- convert test docs with complex tables + Mermaid diagrams
- compare output to source and log issues

**Gate:** stop and iterate based on your review before Phase 3.

---

### Phase 3 - Batch UX and Productivity
**Objective:** make it practical for day-to-day team usage.

Deliverables:
- batch conversion (folder + recursive mode)
- drag/drop support
- preset profiles (save, duplicate, import, export)
- run summary report (success/fail counts + file list)

Demo for you:
- run a real multi-file conversion batch
- validate profile reuse across documents

**Gate:** stop and apply your requested UX changes before Phase 4.

---

### Phase 4 - Standalone Packaging and Sharing
**Objective:** produce shareable executable builds.

Deliverables:
- self-contained `MarkForge.exe` build
- included converter assets (Pandoc + filters + templates as needed)
- first-run validation screen (dependency and permission checks)
- release folder layout for easy sharing

Demo for you:
- copy build to a second machine/user profile
- run conversion with no developer tools installed

**Gate:** stop and confirm portability before Phase 5.

---

### Phase 5 - Hardening and v1.0 Release
**Objective:** stabilize and prepare release candidate.

Deliverables:
- regression test set for representative markdown docs
- improved crash/error recovery
- user quick-start + troubleshooting guide
- `v1.0.0-rc1` build

Demo for you:
- execute full acceptance checklist
- sign-off for production use

**Gate:** final release approval.

## 6. Working Agreement for This Project
For every phase:
1. I implement only that phase scope.
2. I stop and demo to you.
3. You test and request changes.
4. We close that phase before moving forward.

## 7. Key Risks and Mitigations
- **Paths with spaces:** strict argument escaping + automated tests.
- **Diagram rendering edge cases:** fallback paths + explicit warnings.
- **DOCX style drift:** controlled templates + reference test docs.
- **Standalone packaging size:** optimize assets and include only required binaries.

## 8. Immediate Next Step (after your review)
Start **Phase 0** by creating the solution scaffold and a runnable GUI shell.
