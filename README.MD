# Jartisan

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

Jartisan is a .NET-based command-line tool designed to streamline the management of Java/Maven projects. It provides automated project detection, `pom.xml` dependency editing, intelligent scaffolding generation, and directory scanning—all built on a robust Clean Architecture approach.

## ✨ Key Features

- **Smart detection:** detects Java/Maven projects by locating `pom.xml` and `jartisan.json`.
- **Dependency management:** reads and safely updates Maven dependencies in `pom.xml` while preserving XML formatting and avoiding duplicates.
- **Project setup:** initializes `jartisan.json` and prepares template folders.
- **Directory scanning:** scans project structures to build an accurate folder map for configuration sync.
- **Code scaffolding:** generates Java boilerplate and templates based on project configuration.
- **Clean architecture:** decouples business logic from infrastructure using interface-based design.

## 🏗️ Architecture

The solution is organized into four main layers:

- **Jartisan.CLI**: command-line entrypoint for the tool; exposes commands such as `init`, `add`, `list`, `make`, and `scan`.
- **Jartisan.Application**: contains application use cases and business flows; uses interfaces (ports) to keep logic independent from implementation details.
- **Jartisan.Infrastructure**: implements concrete file I/O, atomic XML editing, JSON management, scanning logic, and template processing.
- **Jartisan.Domain**: contains core domain entities and models such as `DependencyInfo`, `FolderMap`, `JavaProjectConfig`, and `MakeOptions`.

## 🛠️ Commands & Usage

### `init`
Initializes the current Java/Maven project, creates or updates `jartisan.json`, and prepares template folders.

```bash
jartisan init
```

### `add`
Adds a dependency to the Maven `pom.xml`.

```bash
jartisan add spring-boot-starter-web:3.4.0
```

### `list`
Lists all dependencies currently defined in `pom.xml`.

```bash
jartisan list
```

### `make`
Generates code scaffolding using predefined templates and project configuration. Supports flags such as `--crud` and `--force`.

```bash
jartisan make controller UserController --crud
```

### `scan`
Scans the project directory structure to build a folder map.

```bash
jartisan scan
```

## 🚀 Installation

### Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/) or higher if you plan to build from source.

### Windows

Run the following command in PowerShell to install Jartisan:

```powershell
Invoke-WebRequest -Uri "https://raw.githubusercontent.com/guiszlima/jartisan-cli/main/install.ps1" -UseBasicParsing | Invoke-Expression
```

### Linux/macOS

Run the following command in your terminal to install Jartisan:

```bash
curl -sSL https://raw.githubusercontent.com/guiszlima/jartisan-cli/main/install.sh | bash
```

### Build from source

If you prefer to build it yourself, clone the repository and run:

```bash
git clone https://github.com/guiszlima/jartisan-cli.git
cd jartisan-cli
dotnet build Jartisan.sln
```

## 📂 Project Structure

- `Jartisan.CLI/` — CLI commands and startup configuration
- `Jartisan.Application/` — application use cases and ports
- `Jartisan.Infrastructure/` — implementations for file I/O, Maven parsing, and templating
- `Jartisan.Domain/` — domain models and core business entities
- `install.ps1` — Windows installer script
- `install.sh` — Linux/macOS installer script

## 💡 Notes

- The project relies heavily on dependency injection and interface-based design to isolate external dependencies from core business rules.
- `IProjectDetector` is used to discover `RootPath` and `PomPath` for the active Maven environment.
- Modifications to `pom.xml` are executed safely using atomic temporary file writes to prevent data corruption.

## 🤝 Contributing

Contributions are welcome. If you want to contribute, please open issues or submit pull requests to propose new commands, improve detection logic, or extend template generation capabilities.
