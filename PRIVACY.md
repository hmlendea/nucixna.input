# Privacy and Personal Data

NuciXNA.Input is a local input management library for MonoGame/XNA applications. It runs entirely within the host application process, processes input device state locally, and does not collect, transmit, or store any personal data.

**Information reviewed:** 2026-10-07

## 📑 Table of Contents

- What This Document Covers
- Data We Handle
- Processing and Use
- Storage, Retention, and Deletion
- External Processing and Integrations
- Document Changes
- Contact

## 🔎 What This Document Covers

This document describes how NuciXNA.Input at https://github.com/hmlendea/nucixna.input handles personal data. It covers the library behaviour and verified integrations described below. NuciXNA.Input is a .NET library distributed via NuGet; it is not a deployed service. The library is compiled into the consuming application and runs entirely on the end user's device.

## 📥 Data We Handle

### Data Provided to the Application

No personal data is requested, collected, or provided to the library by users, administrators, or connected systems.

### Data Generated or Collected by the Application

No personal data is generated or collected automatically. The library reads keyboard, mouse, and gamepad device state from the operating system via MonoGame APIs each frame. This input state is transient, exists only in memory during the frame, and is not logged, persisted, or transmitted.

### Data Received from Integrations

No personal data is received from integrations or third parties. The library has no network capabilities and no external service integrations.

## 🧭 Processing and Use

The library processes input device state for these verified functions:
- Keyboard key state tracking (pressed, released, held) — transient keyboard device state
- Mouse button state tracking (pressed, released, held) and position — transient mouse device state
- Gamepad button state tracking (pressed, released, held) for up to four controllers — transient gamepad device state
- Event dispatch to subscriber callbacks — transient input state passed to application code

All processing occurs in-memory during the host application's `Update` loop. No data leaves the process.

## 🗄️ Storage, Retention, and Deletion

No personal data is stored, retained, or deleted by the library. Input state exists only as transient variables within a single frame's `Update` call and is overwritten on the next frame. The library does not write to disk, databases, logs, caches, or backups.

## 🔗 External Processing and Integrations

The application has no built-in external data transfer. The library depends on:
- MonoGame.Framework.DesktopGL — provides `Keyboard`, `Mouse`, `GamePad`, `GameWindow` APIs (local device access only)
- NuciXNA.Primitives — provides `Point2D` struct (local computation only)

Neither dependency transmits data externally.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| MonoGame.Framework.DesktopGL | Local input device access via OS APIs | Transient device state (keyboard, mouse, gamepad) | https://github.com/MonoGame/MonoGame |
| NuciXNA.Primitives | Shared `Point2D` struct for coordinates | None (local computation) | https://github.com/hmlendea/nucixna.primitives |

## 🛡️ Data Protection and Security

The library performs no network I/O, no file I/O, and no inter-process communication. All input processing is in-process and in-memory. The consuming application controls the runtime environment, permissions, and any data it chooses to derive from input events.

## 🔄 Document Changes

Update this document when library data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/nucixna.input/blob/master/PRIVACY.md.

## 📬 Contact

For questions about library data handling, contact the project maintainers via GitHub issues at https://github.com/hmlendea/nucixna.input/issues. Do not send passwords, access tokens, or other secrets.