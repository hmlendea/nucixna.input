# Security Policy

NuciXNA.Input is a local input management library for MonoGame/XNA. It runs entirely in-process, has no network capabilities, and processes only transient device state. This policy covers vulnerability reporting for the library code distributed via NuGet and GitHub Releases.

## 📑 Table of Contents

- Supported Versions
- Reporting a Vulnerability
- Scope
- Disclosure Policy
- Safe Harbour
- Recognition

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | NuGet | ✅ |
| Latest version | GitHub Releases | ✅ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/nucixna.input/security/advisories)
- Contact the maintainers directly via GitHub issues

## 📌 Scope

The subsequent report categories are in scope for this repository:
- Vulnerabilities in NuciXNA.Input library code (InputManager, event system, state management)
- Vulnerabilities in the NuGet package or build pipeline

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in MonoGame.Framework.DesktopGL (upstream dependency)
- Vulnerabilities in NuciXNA.Primitives (upstream dependency)
- Vulnerabilities in consuming applications that use this library
- Vulnerabilities in the .NET runtime or SDK
- Denial-of-service via excessive event handler allocation (by design, not a vulnerability)

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.

## 🧾 Safe Harbour

If your research is conducted in good faith, confined to authorised scope, and disclosed responsibly, the maintainers will not pursue action for policy-compliant activity.

## 🙏 Recognition

We appreciate responsible disclosure. Reporters who desire public attribution may be acknowledged in release notes, advisories, or a dedicated acknowledgements section.