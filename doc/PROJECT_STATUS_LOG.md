# MarkForge Project Status Log

This file tracks implementation progress by session so work can resume quickly after interruptions.

## 2026-02-26 - Session 1

- Phase: `Phase 0 - Foundation and Skeleton`
- Status: `Ready for Approval`

### Completed

- Initialized and pushed repository to `kbdarius/MarkForge`.
- Created solution scaffold: `MarkForge.sln`.
- Added WPF app project: `src/MarkForge.App`.
- Implemented MVVM shell with navigation between `Convert` and `Settings` panels.
- Added local JSON settings storage service at `%LOCALAPPDATA%\MarkForge\settings.json`.
- Added placeholder conversion service and run log panel in the UI.
- Added save/load settings actions in the conversion panel.
- Installed `.NET SDK 8.0.418` via `winget`.
- Verified build success with `dotnet build MarkForge.sln` (0 errors, 0 warnings).
- Performed executable launch sanity check (`LAUNCHED_OK`).

### Pending Before Phase 0 Gate

- User demo/approval before starting Phase 1.

### Resume Notes

- Open `MarkForge.sln` in Visual Studio.
- Run `MarkForge.App`.
- Test:
  - browse markdown file and output folder
  - save settings
  - reload settings
  - run placeholder conversion and review log panel

## 2026-02-26 - Session 2

- Phase: `Phase 0 - Foundation and Skeleton`
- Status: `Completed`

### Completed

- Added default output-folder behavior:
  - when input markdown file is selected, output folder auto-populates to the same directory
  - manual output folder overrides are preserved
- Created and integrated application branding assets:
  - app icon: `src/MarkForge.App/Assets/MarkForge.ico`
  - icon preview: `src/MarkForge.App/Assets/MarkForge-icon.png`
  - thumbnail image: `doc/assets/markforge-thumbnail.png`
- Wired icon into WPF app:
  - `ApplicationIcon` in project file
  - `MainWindow` icon for title bar/taskbar thumbnail
- Added repository `.gitignore` for `bin/`, `obj/`, and IDE user files.
- Build verification passed after updates:
  - `dotnet build MarkForge.sln` -> 0 errors, 0 warnings

### Resume Notes

- Current branch target: `main`
- Next planned phase: `Phase 1 - Single-File Conversion MVP`

## 2026-02-26 - Session 3

- Phase: `Phase 1 - Single-File Conversion MVP`
- Status: `Ready for Approval`

### Completed

- Replaced placeholder pipeline with real Pandoc conversion service:
  - `src/MarkForge.App/Services/PandocConversionService.cs`
- Added detailed run logs under `%LOCALAPPDATA%\MarkForge\logs` including:
  - command line
  - selected options
  - stdout/stderr
  - error/exception details
- Added real DOCX orientation post-processing via Open XML SDK:
  - `src/MarkForge.App/Services/OpenXmlDocxOrientationService.cs`
- Added Lua filter support:
  - optional user-selected Lua filter folder in UI/settings
  - bundled default filters copied to output (`ConverterAssets/filters/*.lua`)
- Updated conversion UI for Phase 1:
  - run button now executes real conversion
  - added Lua filter folder picker
  - added last output DOCX path + last detailed log path fields
- Updated settings model and summary for Lua filters and new defaults.
- Verified build:
  - `dotnet build MarkForge.sln` -> 0 errors, 0 warnings
- Verified app startup:
  - executable launch sanity check (`LAUNCHED_OK`)
- Verified end-to-end conversion via temporary harness:
  - generated DOCX successfully (`DOCX_OK`)
  - generated detailed run log successfully (`LOG_OK`)

### Pending Before Phase 1 Gate

- User demo/approval on real markdown sample before starting Phase 2.

### Resume Notes

- Use the `Convert` panel to run a real `.md` -> `.docx` conversion.
- Detailed run logs are written to `%LOCALAPPDATA%\MarkForge\logs`.
- Next planned phase after approval: `Phase 2 - Fidelity Improvements (Tables + Diagrams)`.
