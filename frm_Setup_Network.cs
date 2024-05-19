using System;
using System.Windows.Forms;
using Inventory;

namespace SeisWing
{
    public partial class frm_Setup_Network : Form
    {
        private frm_parent parent;
        public CL_INV_NETWORK Network;

        public frm_Setup_Network(frm_parent _parent)
        {
            parent = _parent;
            InitializeComponent();
        }

        private void frm_Setup_AddNetwork_Load(object sender, EventArgs e)
        {
            if (Network != null)
            {
                Text = "Setup - Edit Network " + Network.Name;
                Txt_Name.Text = Network.Name;
                dateTimePicker1.Value = Network.TimeBegin;//.ToDateTime();
                Txt_Description .Text= Network.Description;
                Txt_Institution.Text = Network.institutions;
                Txt_Region.Text = Network.region;
                Chk_Restricted.Checked = Network.RestrictedStatus == "restricted";
                Chk_Shared.Checked = Network.shared;
                Txt_LonMin.Text = Double.IsNaN(Network.LonMin) ? "" : Network.LonMin.ToString();
                Txt_LonMax.Text = Double.IsNaN(Network.LonMax) ? "" : Network.LonMax.ToString();
                Txt_LatMin.Text = Double.IsNaN(Network.LatMin) ? "" : Network.LatMin.ToString();
                Txt_LatMax.Text = Double.IsNaN(Network.LatMax) ? "" : Network.LatMax.ToString();
            }
        }

        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Txt_Name.Text))
            {
                return;
            }
            Network = new CL_INV_NETWORK();
            Network.Name = Txt_Name.Text;
            Network.TimeBegin = dateTimePicker1.Value;// DateTimeUing.FromDateTime(dateTimePicker1.Value).ToDateTime();
            Network.Description = Txt_Description.Text;
            Network.institutions = Txt_Institution.Text;
            Network.region = Txt_Region.Text;
            Network.RestrictedStatus = Chk_Restricted.Checked ? "restricted" : "open";
            Network.shared = Chk_Shared.Checked;
            if (!String.IsNullOrEmpty(Txt_LonMin.Text))
            {
                double.TryParse(Txt_LonMin.Text, out Network.LonMin);
            }
            if (!String.IsNullOrEmpty(Txt_LonMax.Text))
            {
                double.TryParse(Txt_LonMax.Text, out Network.LonMax);
            }
            if (!String.IsNullOrEmpty(Txt_LatMin.Text))
            {
                double.TryParse(Txt_LatMin.Text, out Network.LatMin);
            }
            if (!String.IsNullOrEmpty(Txt_LatMax.Text))
            {
                double.TryParse(Txt_LatMax.Text, out Network.LatMax);
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
