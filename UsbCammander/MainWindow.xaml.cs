using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32.SafeHandles;
using EricWang;

using System.IO; // DirveInfo





namespace UsbCammander
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        List<EricWang.Device.MyHandle> m_handleColls;
        List<UsbCmd> m_cmdColls;

        private void Window_Initialized(object sender, EventArgs e) {
            m_cmdColls = new List<UsbCmd>();

            USbCmdUtility ucu = new USbCmdUtility();
            ucu.getAllCmd(m_cmdColls);

            foreach(UsbCmd cmd in m_cmdColls) {
                cboCmdSel.Items.Add(cmd.desc);
            }
        }

        private void btnReFresh_Click( object sender, RoutedEventArgs e ) {
            EricWang.Device device = new EricWang.Device();
            m_handleColls = new List<EricWang.Device.MyHandle>();

            device.getDeviceHandle( m_handleColls );
            foreach( EricWang.Device.MyHandle i in m_handleColls ) {
                cboDeviceSel.Items.Add( i.name );
            }
        }

        private void btnExecute_Click( object sender, RoutedEventArgs e ) {
            int curSel = cboDeviceSel.SelectedIndex;
            EricWang.Device.MyHandle myHandle = m_handleColls[curSel];
            EricWang.Device device = new EricWang.Device();

            UsbCmd cmd = m_cmdColls[cboCmdSel.SelectedIndex];


            byte[] ioBuf = new byte[65535];
            bool ret = device.sendScsiCommand(myHandle.handle, cmd.cdb, ioBuf, cmd.length, cmd.direction);
            EricWang.Utility u = new EricWang.Utility();
            txtMsg.Text = u.makeHexTable(ioBuf, cmd.length);

            txtMsg.Text = u.makeHeader(txtMsg.Text);

            txtAscii.Text = u.makeAsciiTable(ioBuf);
        }

        private void cboCmdSel_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            UsbCmd c = m_cmdColls[cboCmdSel.SelectedIndex ];
            setForm(c);
        }

        private void setForm(UsbCmd cmd) {
            txtCdb_00.Text = cmd.cdb[0].ToString("X2");
            txtCdb_01.Text = cmd.cdb[1].ToString("X2");
            txtCdb_02.Text = cmd.cdb[2].ToString("X2");
            txtCdb_03.Text = cmd.cdb[3].ToString("X2");
            txtCdb_04.Text = cmd.cdb[4].ToString("X2");
            txtCdb_05.Text = cmd.cdb[5].ToString("X2");
            txtCdb_06.Text = cmd.cdb[6].ToString("X2");
            txtCdb_07.Text = cmd.cdb[7].ToString("X2");
            txtCdb_08.Text = cmd.cdb[8].ToString("X2");
            txtCdb_09.Text = cmd.cdb[9].ToString("X2");
            txtCdb_10.Text = cmd.cdb[10].ToString("X2");
            txtCdb_11.Text = cmd.cdb[11].ToString("X2");
            txtLength.Text = cmd.length.ToString("X2");

            rdoDataIn.IsChecked = true;
            if(cmd.direction == 0) {
                rdoDataOut.IsChecked = true;
            } 
        }
    }
}
