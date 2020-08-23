namespace NetVips.Interop
{
    internal static class Libraries
    {
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        // We can safely define all these variables as `libvips.so.42` since
        // DLLImport uses dlsym() on Linux. This function also searches for named
        // symbols in the dependencies of the shared library. Therefore, we can
        // provide libvips as a single shared library with all dependencies
        // statically linked without breaking compatibility with shared builds
        // (i.e. what is usually installed via package managers).
        internal const string GLib = "libvips.so.42",
                              GObject = "libvips.so.42",
                              Vips = "libvips.so.42";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        // We can safely define all these variables as `libvips.42.dylib` since
        // DLLImport uses dlsym() on macOS. This function also searches for named
        // symbols in the dependencies of the shared library. Therefore, we can
        // provide libvips as a single shared library with all dependencies
        // statically linked without breaking compatibility with shared builds
        // (i.e. what is usually installed via package managers).
        internal const string GLib = "libvips.42.dylib",
                              GObject = "libvips.42.dylib",
                              Vips = "libvips.42.dylib";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // We cannot define all these variables as `libvips-42.dll` without
        // breaking compatibility with the shared Windows build. Therefore,
        // we always ship at least 3 DLLs.
        internal const string GLib = "libglib-2.0-0.dll",
                              GObject = "libgobject-2.0-0.dll",
                              Vips = "libvips-42.dll";
#else
        #error "Unknown target platform"
#endif
    }
}