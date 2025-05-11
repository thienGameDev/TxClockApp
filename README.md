# Clock Application

## Overview

This repository hosts the Windows PC implementation of the Clock Application, a business‑focused tool for measuring work operation time. It features:

* **Clock**: Displays current time in the user’s timezone.
* **Timer**: Start, stop, pause, and reset. Plays a sound on completion.
* **Stopwatch**: Start, pause, stop, and reset. Shows elapsed time.
* **Multi‑function view**: Access current time while using Timer or Stopwatch.

This codebase uses UniRx for reactive programming and Zenject for dependency injection, with both Edit Mode and Play Mode tests to ensure quality.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Features](#features)
3. [Architecture](#architecture)
4. [Installation](#installation)
5. [Usage](#usage)
6. [Testing](#testing)
7. [Future Improvements](#future-improvements)
8. [iOS/iPad UI Considerations](#iosipad-ui-considerations)
9. [Refactoring](#refactoring--roadmap)
10. [VR Support Considerations](#vr-support-considerations)
11. [Total Effort Hrs](#total-effort-hrs)

## Getting Started

Clone the repository and open in Unity (2021.3.40f1 LTS):

```bash
git clone https://github.com/thienGameDev/TxClockApp.git
cd TxClockApp
```

## Features

1. **LocalClock**

   * Shows local current time.
2. **Stopwatch**

   * Start, Pause, Stop, Reset controls.
3. **Timer**

   * Start, Pause, Stop, Reset controls.
   * Plays a configurable finish sound.

4. **Multi‑tasking**

   * Users can switch between Clock, Stopwatch, and Timer without stopping any running function.

## Architecture

* **Dependencies**: [UniRx](https://github.com/neuecc/UniRx), [Zenject](https://github.com/modesttree/Zenject)
* **Design**:

  * **IU Inspired** by Clock App UI on iOS.
  * **MVC‑style** components for UI and logic separation.
  * **Interfaces** defined under `ILocalClockService`, `IStopwatchService`, and `ITimerService` for future integration.
  * **Dependency Injection** via Zenject installers.

## Installation

1. Open Unity Hub and add the project.
2. In the Package Manager, install UniRx and Zenject via Git URL.
3. Verify that `Assets/Plugins/UniRx` and `Assets/Plugins/Zenject` are present.

## Usage

1. Press **Play** in the Unity Editor.
2. Navigate the UI:

   * **LocalClock** tab: View current time.
   * **Stopwatch** tab: Press Start to begin measuring.
   * **Timer** tab: Configure duration by input hours/min/sec value and press Start.

## Testing

* **Edit Mode Tests**: Validate service logic and interface contracts using NUnit under `Tests/EditMode`.
* **Play Mode Tests**: Simulate user interactions on UI components and validate service logic with realtime update under `Tests/PlayMode`.

## Future Improvements

* Localization support (i18n).
* Theming (light/dark modes).
* Enhanced notification system (system tray alerts on Windows).

## iOS/iPad UI Considerations

* **Touch Targets**: Ensure buttons meet minimum touch size (44×44pt) for accessibility.
* **Adaptive Layouts**: Use safe areas and Auto Layout constraints to handle various screen sizes and orientations.
* **Platform-specific Controls**: Adopt `UIDatePicker`‑style spinners for time selection to match iOS conventions.

## Refactoring
* **My approach for refactoring the code and/or project post-release**
  After a project is released, my refactoring process focuses on improving stability, maintainability, and scalability without disrupting the user experience. I break it down into two categories: 'must-happen' changes that directly impact quality and 'nice-to-have' improvements that enhance the developer experience or future flexibility.

  **Must-Happen changes (High Priority)** 
  1. Fix Critical Bugs and Crashes
  * Use analytics, crash logs, user feedback to patch crash-prone areas or memory leaks immediately.
  * Prioritize hot paths and unstable areas of the code that need to be cleaned up.
  
  2. Address Tech-Debt in Core Systems
  * Refactor poorly written or overly complex systems that are hard to extend or frequently touched by new features
  * Isolate platfrom-specific code (e.g., VR, iOS, Android) for easier testing and extending.

  3. Stabilize Performance Bottlenecks
  * Profile the app to detect bottlenecks (CPU, GPU, memory, disk IO)
  * Refactor inefficient loops, reduce allocations, and optimize memory pooling if applicable
  * Refactor code to be more testable
  * Split scripts or classes into smaller, reusable modules.

  4. Clean-up deprecated or prototype code
  * Remove old feature flags, debugging tools, or placeholder logic used during development.
  * Use feature toggles or remote configuration if post-release experimentation is ongoing.

  5. Improve build and developement pipelines
  * Refactor build scripts or CI/CD pipelines to reduce manual steps.
  * Improve logging for deployment steps.

  **Nice to Have**:
  1. Improve documentation, comments and naming
  * Refactor ambiguous or outdated naming
  * Remove redundant comments and add meaningful documentation for public APIs and core systems.
  2. Folder Structure, and Asset Organization
  * Standardize naming conventions and reorganize folders (especially in Unity or Xcode projects) for better clarity.
  3. Polish UX and Edge Case Handling
  * Improve non-critical user flows, edge cases, or polish animations/sounds that weren't prioritized pre-launch.

## VR Support Considerations

* **Input & Interaction**: Replace 2D buttons with gaze‑or controller‑driven UI (use World Space canvases).
* **Spatial Audio**: Position timer/alarm sounds in 3D space for immersion.
* **User Comfort**: Avoid rapid UI movements and minimize latency in updates.

## Total Effort Hrs

* Read documents: 30 mins 
* Make test: 6 hours 
* Implement: 6 hours 
* Refactor: 3 hour 
* UI/UX Polishing: 2 hour
* Documemtation: 1 hour
