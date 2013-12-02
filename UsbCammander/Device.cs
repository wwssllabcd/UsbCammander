using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices; //for Import dll, ex: createFile, DeviceIoCtrl
using Microsoft.Win32.SafeHandles;    //SafeHandle

using System.IO; // DirveInfo

namespace EricWang
{
    class Device
    {
        //creareFile
        public const int GENERIC_ALL = 0x10000000;
        public const int GENERIC_EXECUTE = 0x20000000;
        public const uint GENERIC_READ = 0x80000000;
        public const int GENERIC_WRITE = 0x40000000;
        public const int FILE_SHARE_READ = 1;
        public const int FILE_SHARE_WRITE = 2;
        public const int OPEN_EXISTING = 3;


        private const uint IOCTL_SCSI_PASS_THROUGH_DIRECT = 0x4D014;
        private const byte SCSI_IOCTL_DATA_OUT = 0;
        private const byte SCSI_IOCTL_DATA_IN = 1;
        private const byte SCSI_IOCTL_DATA_UNSPECIFIED = 2;

        // SCSI PASS Through
        private const uint METHOD_BUFFERED = 0;
        private const uint FILE_READ_ACCESS = 0x0001;
        private const uint FILE_WRITE_ACCESS = 0x0002;
        private const uint FILE_DEVICE_CONTROLLER = 0x00000004;

        [DllImport( "kernel32.dll", SetLastError = true )]
        static extern void SetLastError( uint dwErrCode );

        [DllImport( "kernel32.dll", SetLastError = true )]
        static extern SafeFileHandle CreateFile(
            String lpFileName,
            UInt32 dwDesiredAccess,
            UInt32 dwShareMode,
            IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes,
            IntPtr hTemplateFile
            );

        [DllImport( "kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto )]
        internal static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped
            );

        [StructLayout( LayoutKind.Sequential, Pack = 4 )]
        struct SCSI_PASS_THROUGH
        {
            public short Length;
            public byte ScsiStatus;
            public byte PathId;
            public byte TargetId;
            public byte Lun;
            public byte CdbLength;
            public byte SenseInfoLength;
            public byte DataIn;
            public int DataTransferLength;
            public int TimeOutValue;
            public IntPtr DataBuffer; // note this is now interpreted as a pointer, not an offset!!
            public uint SenseInfoOffset;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 16 )]
            public byte[] Cdb;
        };

        [StructLayout( LayoutKind.Sequential )]
        class SCSI_PASS_THROUGH_WITH_BUFFERS
        {
            internal SCSI_PASS_THROUGH spt = new SCSI_PASS_THROUGH();
            uint filter;
            // // adapt size to suit your needs!!!!!! 
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            //internal byte[] sense;
            // adapt to suit your needs!!!!!!! 
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 0x24 )]    //changed to match CDB
            internal byte[] data;
        };

        public static uint CTL_CODE( uint DeviceType, uint Function, uint Method, uint Access ) {
            return ( ( ( DeviceType ) << 16 ) | ( ( Access ) << 14 ) | ( ( Function ) << 2 ) | ( Method ) );
        }

        public class MyHandle
        {
            public SafeFileHandle handle;
            public String name;
        }

        public void getDeviceHandle( List<MyHandle> colls ) {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach( DriveInfo drive in allDrives ) {
                if( drive.DriveType != DriveType.Removable ) {
                    continue;
                }
                if( drive.Name == "A:\\" ) {
                    continue;
                }

                char[] deviceName = new char[1];
                deviceName[0] = drive.Name[0];
                String dn = "\\\\.\\" + deviceName[0] + ":";
                SafeFileHandle hDevice = CreateFile
                    (
                    dn,
                    GENERIC_READ | GENERIC_WRITE,
                    FILE_SHARE_READ | FILE_SHARE_WRITE,
                    IntPtr.Zero,
                    OPEN_EXISTING,
                    0,
                    IntPtr.Zero );

                if( hDevice.IsInvalid ) {
                    continue;
                }

                MyHandle tmp = new MyHandle();
                tmp.handle = hDevice;
                tmp.name = deviceName[0].ToString();
                colls.Add( tmp );
            }
        }

        public bool sendScsiCommand( SafeFileHandle sHandle, byte[] cdb, byte[] ioBuffer, ulong dataLen, byte direction ) {
            uint IOCTL_SCSI_PASS_THROUGH = CTL_CODE( FILE_DEVICE_CONTROLLER, 0x0401, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS );
            SCSI_PASS_THROUGH_WITH_BUFFERS sptwb = new SCSI_PASS_THROUGH_WITH_BUFFERS();

            // initilalize the cdb
            sptwb.spt.Cdb = new byte[16];
            sptwb.spt.Cdb = Enumerable.Repeat( (byte)0, 16 ).ToArray();
            sptwb.spt.CdbLength = 12;
            Array.Copy( cdb, sptwb.spt.Cdb, sptwb.spt.CdbLength );

            sptwb.data = new byte[dataLen]; // adapt to suit your needs!!!!!!
            sptwb.spt.DataTransferLength = sptwb.data.Length; // 0x24

            //info.sense = new byte[24]; // adapt to suit your needs!!!!!!
            sptwb.spt.SenseInfoLength = 0;
            sptwb.spt.SenseInfoOffset = 0;

            sptwb.spt.DataBuffer = Marshal.OffsetOf( typeof( SCSI_PASS_THROUGH_WITH_BUFFERS ), "data" );
            sptwb.spt.Length = (short)Marshal.SizeOf( sptwb.spt );

            sptwb.spt.PathId = 0;
            sptwb.spt.TargetId = 0;
            sptwb.spt.Lun = 0;
            sptwb.spt.TimeOutValue = 30;
            sptwb.spt.DataIn = direction;

            IntPtr inBuffer = Marshal.AllocHGlobal( Marshal.SizeOf( sptwb ) );
            Marshal.StructureToPtr( sptwb, inBuffer, false );

            // call DeviceIoControl passing the buffer inpBuffer as inp buffer and/or output buffer depending on the command.
            uint Dummy = 0;
            uint inputBufLen = (uint)Marshal.SizeOf( sptwb.spt );
            uint outputBufLen = (uint)Marshal.SizeOf( sptwb ); // 0x54


            bool ret = DeviceIoControl(
                sHandle.DangerousGetHandle(),
                IOCTL_SCSI_PASS_THROUGH,
                inBuffer,
                inputBufLen,

                inBuffer,
                outputBufLen,

                out  Dummy,
                IntPtr.Zero );


            //Marshal.PtrToStructure( inBuffer, sptwb );

            Array.Copy( sptwb.data, ioBuffer, (int)dataLen );
            return ret;
        }
    }
}
