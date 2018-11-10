using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace InteropConsole
{
    public class NativeInterop : IDisposable
    {

        [DllImport("nativeloader.dll")]
        private static extern IntPtr Prepare(string filename);

        [DllImport("nativeloader.dll")]
        private static extern void Free(IntPtr handle);


        [DllImport("nativeloader.dll")]
        private static extern void Read(IntPtr handle, [Out] out IntPtr data, [Out] out int length);

        [DllImport("nativeloader.dll")]
        private static extern WavHeader ReadWavHeader(IntPtr handle);

        [DllImport("nativeloader.dll")]
        private static extern unsafe byte* ReadUnsafe(IntPtr handle);


        [DllImport("nativeloader.dll")]
        public static extern int getLibraryVersion();


        private IntPtr _handle;
        public NativeInterop(string filename)
        {
            this.Filename = filename;
            _handle = Prepare(filename);
        }

        public string Filename { get; private set; }

        public void Dispose()
        {
            Free(_handle);
        }

        public void Read([Out] out IntPtr data, [Out] out int length)
        {
            Read(_handle, out data, out length);
        }

        public WavHeader ReadWavHeader()
        {
            return ReadWavHeader(_handle);
        }

        public unsafe byte* ReadUnsafe()
        {
            return ReadUnsafe(_handle);
        }

    }
}
