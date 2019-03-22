# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - ???
### Added
- Pre-compiled libvips binaries for a few distros ([#21](https://github.com/kleisauke/net-vips/issues/21)):
  - [NetVips.Native.linux-x64](https://www.nuget.org/packages/NetVips.Native.linux-x64) - Linux x64 glibc (Ubuntu, Debian, etc).
  - [NetVips.Native.linux-musl-x64](https://www.nuget.org/packages/NetVips.Native.linux-musl-x64) - Linux x64 musl (Alpine, Gentoo Linux, etc).
  - [NetVips.Native.osx-x64](https://www.nuget.org/packages/NetVips.Native.osx-x64) - macOS x64.

### Changed
- Improve memory management ([#26](https://github.com/kleisauke/net-vips/issues/26)).
- The bundled libvips Windows binaries were split into different NuGet packages:
  - [NetVips.Native.win-x64](https://www.nuget.org/packages/NetVips.Native.win-x64) - Windows 64-bit.
  - [NetVips.Native.win-x86](https://www.nuget.org/packages/NetVips.Native.win-x86) - Windows 32-bit.
- Update the [NetVips.Benchmarks](https://github.com/kleisauke/net-vips/tree/master/tests/NetVips.Benchmarks).
- A statically linked libvips binary is build for Windows. This reduces the number of DLLs from 37 to 3 ([libvips/build-win64#21](https://github.com/libvips/build-win64/issues/21#issuecomment-458112440)).

### Removed
- The `UseGlobalLibvips` property since the bundled libvips binaries are split into different NuGet packages.

## [1.0.7] - 2019-01-18
### Changed
- Update bundled libvips x86/x64 binary to 8.7.4.
- Speed-up `Base.Version` by caching the libvips version as soon as the assembly is loaded.

## [1.0.6] - 2019-01-10
### Added
- The `LibvipsOutputBase` property to specify the subdirectory (within your project's output directory) where the libvips binaries are copied to ([#20](https://github.com/kleisauke/net-vips/issues/20)).

### Changed
- Update bundled libvips x86/x64 binary to 8.7.3.
- No exceptions will be thrown by the `ModuleInitializer` (used to initialize libvips once the assembly is loaded) ([#15](https://github.com/kleisauke/net-vips/issues/15), [#20](https://github.com/kleisauke/net-vips/issues/20)).

### Removed
- The redundant `LibvipsDLLPath` property.

## [1.0.5] - 2018-09-25
### Added
- Bundle pre-compiled libvips binary and its dependencies for 32-bit Windows.

### Fixed
- Fix five small memleaks ([libvips/lua-vips#24](https://github.com/libvips/lua-vips/issues/24)).

### Changed
- Update bundled libvips binary to 8.7.0.

## [1.0.4] - 2018-06-28
### Added
- Add `contains` helper (to check if the image contains an property of metadata).
- Support 32-bit architecture ([#7](https://github.com/kleisauke/net-vips/issues/7)).

### Changed
- Update bundled libvips binary to 8.6.4.

### Fixed
- Fix a bug that freed a string pointer too early ([#9](https://github.com/kleisauke/net-vips/issues/9)).

## [1.0.3] - 2018-06-06
### Added
- Bundle pre-compiled libvips binary and its dependencies for 64-bit Windows ([#3](https://github.com/kleisauke/net-vips/issues/3)).

### Changed
- Target .NET Standard 2.0 instead of .NET Core 2.0 ([#4](https://github.com/kleisauke/net-vips/issues/4)).
- Lower the minimum required .NET Framework version to 4.5 ([#4](https://github.com/kleisauke/net-vips/issues/4)).

## [1.0.2] - 2018-04-23
### Added
- Add missing libvips 8.7 methods.
- Add logging handler to log warnings and debug messages from libvips.

### Fixed
- Fix a bug that tried to reference an empty pointer.
- Fix a bug that causes libvips arguments to be set incorrectly.
- Fix up memory errors and leaks.
- Prevent the GC from unsetting the gvalue and disposing a delegate prematurely.

## [1.0.1] - 2018-04-10
### Fixed
- Fix reference count bug.

## [1.0.0] - 2018-04-08
### Added
- First release!

[1.1.0]: https://github.com/kleisauke/net-vips/compare/v1.0.7...v1.1.0
[1.0.7]: https://github.com/kleisauke/net-vips/compare/v1.0.6...v1.0.7
[1.0.6]: https://github.com/kleisauke/net-vips/compare/v1.0.5...v1.0.6
[1.0.5]: https://github.com/kleisauke/net-vips/compare/v1.0.4...v1.0.5
[1.0.4]: https://github.com/kleisauke/net-vips/compare/v1.0.3...v1.0.4
[1.0.3]: https://github.com/kleisauke/net-vips/compare/v1.0.2...v1.0.3
[1.0.2]: https://github.com/kleisauke/net-vips/compare/v1.0.1...v1.0.2
[1.0.1]: https://github.com/kleisauke/net-vips/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/kleisauke/net-vips/releases/tag/v1.0.0