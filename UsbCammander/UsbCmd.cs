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
    }
}
