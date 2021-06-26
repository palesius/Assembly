using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assembly
{
    public partial class frmStatus : Form
    {
        public frmStatus()
        {
            InitializeComponent();
        }

        private void frmStatus_Load(object sender, EventArgs e)
        {

        }

        public void UpdateMapStatus(int mapIdx, int mapCnt, String mapMsg)
        {
            if (mapCnt != pbMap.Maximum) { pbMap.Maximum = mapCnt; }
            if (mapIdx != pbMap.Value) { pbMap.Value = mapIdx; pbMap.Refresh(); }
            if (mapMsg != null && mapMsg.CompareTo(txtMap.Text) != 0) { txtMap.Text = mapMsg; txtMap.Refresh(); }
        }
        public void UpdateTagStatus(int tagIdx, int tagCnt, String tagMsg)
        {
            if (tagCnt != pbTag.Maximum) { pbTag.Maximum = tagCnt; }
            if (tagIdx != pbTag.Value) { pbTag.Value = tagIdx; pbTag.Refresh(); }
            if (tagMsg != null && tagMsg.CompareTo(txtTag.Text) != 0) { txtTag.Text = tagMsg; txtTag.Refresh(); }
        }
    }
}
