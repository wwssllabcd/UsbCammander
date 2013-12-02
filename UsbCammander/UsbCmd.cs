using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EricWang
{
    class UsbCmd
    {
        public byte[] cdb = new byte[12];
        public uint length;
        public byte direction;
        public string desc;
    }

    class USbCmdUtility
    {
        private const byte SCSI_IOCTL_DATA_OUT = 0;
        private const byte SCSI_IOCTL_DATA_IN = 1;
        private const byte SCSI_IOCTL_DATA_UNSPECIFIED = 2;

        public void getAllCmd(List<UsbCmd> colls) {
            colls.Add(inquiry());
            colls.Add(requestSense());

            colls.Add(readCapacity());
            colls.Add(readFormatCapacity());
            colls.Add(testUnitReady());
            colls.Add(read10());
            colls.Add(write10());

        }

        public UsbCmd inquiry() {
            UsbCmd cmd = new UsbCmd();
            cmd.cdb[0] = 0x12;
            cmd.cdb[4] = 0x24;
            cmd.length = 0x24;

            cmd.direction = SCSI_IOCTL_DATA_IN;
            cmd.desc = "UFI: Inquiry";

            return cmd;
        }

        public UsbCmd requestSense() {
            UsbCmd cmd = new UsbCmd();
            cmd.cdb[0] = 0x3;
            cmd.cdb[4] = 0x12;

            cmd.length = 0x12;
            cmd.direction = SCSI_IOCTL_DATA_IN;
            cmd.desc = "UFI: Request Sense";
            return cmd;
        }

        public UsbCmd readCapacity() {
            UsbCmd cmd = new UsbCmd();
            cmd.cdb[0] = 0x25;
            cmd.length = 8;
            cmd.direction = SCSI_IOCTL_DATA_IN;
            cmd.desc = "UFI: Read Capacity";
            return cmd;
        }

        public UsbCmd readFormatCapacity() {
            UsbCmd cmd = new UsbCmd();
            cmd.cdb[0] = 0x23;
            cmd.length = 12;
            cmd.direction = SCSI_IOCTL_DATA_IN;
            cmd.desc = "UFI: Read Format Capacity";
            return cmd;
        }

        public UsbCmd testUnitReady() {
            UsbCmd cmd = new UsbCmd();
            cmd.direction = SCSI_IOCTL_DATA_IN;
            cmd.desc = "UFI: Test Unit Ready";
            return cmd;
        }

        public UsbCmd read10() {
            UsbCmd cmd = new UsbCmd();
            cmd.cdb[0] = 0x28;
            cmd.cdb[8] = 0x01;
            cmd.length = 512;

            cmd.direction = SCSI_IOCTL_DATA_IN;
            cmd.desc = "UFI: Read(10)";
            return cmd;
        }

        public UsbCmd write10() {
            UsbCmd cmd = new UsbCmd();
            cmd.cdb[0] = 0x2A;
            cmd.cdb[8] = 0x01;
            cmd.length = 512;

            cmd.direction = SCSI_IOCTL_DATA_OUT;
            cmd.desc = "UFI: Write(10)";
            return cmd;
        }
    }
}
