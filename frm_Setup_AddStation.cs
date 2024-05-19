using System;
using System.Windows.Forms;

namespace SeisWing
{
    public partial class frm_Setup_AddStation : Form
    {
        private frm_parent parent;
        public String NetworkName;
        public Class_Network Network;
        public Class_Station Station;
        public SensorLocationPtr SensorLocation;
        public StreamPtr Stream;
        private double longitude = double.NaN;
        private double latitude = double.NaN;
        private double elevation = double.NaN;

        public frm_Setup_AddStation(frm_parent _parent)
        {
            parent = _parent;
            InitializeComponent();

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void frm_Setup_AddStation_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(NetworkName))
            {
                Txt_NetworkName.Text = NetworkName;
            }

            if (Station != null)
            {
                Text = "Setup - Edit Station";
                Txt_Name.Text = Station.code;
                dateTimePicker1.Value = Station.start.ToDateTime();
                Txt_Description.Text = Station.description;
                checkBox1.Checked = Station.restricted;
                checkBox2.Checked = Station.shared;
                Cbb_LocationFormat.SelectedIndex = Station.locationFormat;
                Txt_Latitude.Text = Double.IsNaN(Station.latitude) ? "" : Station.latitude.ToString();
                Txt_Longitude.Text = Double.IsNaN(Station.longitude) ? "" : Station.longitude.ToString();
                Txt_Elevation.Text = Double.IsNaN(Station.elevation) ? "" : Station.elevation.ToString();

                //eusian channel
                foreach (var location in Station.sensorLocation)
                {
                    foreach (var channel in location._streams)
                    {
                        int irow = dataGridView1.Rows.Add(1);
                        dataGridView1.Rows[irow].Cells["Clm_Code"].Value = channel.code;
                        dataGridView1.Rows[irow].Cells["Clm_SampleRate"].Value = (channel.sampleRateNumerator * channel.sampleRateDenominator);
                        dataGridView1.Rows[irow].Cells["CLm_Sensor"].Value = channel.sensor;
                        dataGridView1.Rows[irow].Cells["Clm_SerialNumber"].Value = channel.sensorSerialNumber;
                        dataGridView1.Rows[irow].Cells["Clm_Gain"].Value = Double.IsNaN(channel.gain) ? "" : channel.gain.ToString();
                        dataGridView1.Rows[irow].Cells["Clm_GainFrequency"].Value = Double.IsNaN(channel.gainFrequency) ? "" : channel.gainFrequency.ToString();
                        dataGridView1.Rows[irow].Cells["Clm_GainUnit"].Value = channel.gainUnit;
                    }
                }
            }
        }

        private void Btn_AddNetwork_Click(object sender, EventArgs e)
        {

        }

        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Txt_Name.Text))
            {
                MessageBox.Show("Station Name Empty.");
                return;
            }
            if (String.IsNullOrEmpty(Txt_Longitude.Text))
            {
                MessageBox.Show("Station Longitude Empty.");
                return;
            }
            if (String.IsNullOrEmpty(Txt_Latitude.Text))
            {
                MessageBox.Show("Station Latitude Empty.");
                return;
            }
            if (String.IsNullOrEmpty(Txt_Elevation.Text))
            {
                MessageBox.Show("Station Elevation Empty.");
                return;
            }

            NetworkName = Txt_Name.Text;

            if (Station == null)
            {
                Station = new Class_Station();
            }
            Station.code = Txt_Name.Text;
            Station.start = DateTimeUing.FromDateTime(dateTimePicker1.Value);
            Station.description = Txt_Description.Text;
            Station.restricted = checkBox1.Checked;
            Station.shared = checkBox2.Checked;
            Station.locationFormat = Cbb_LocationFormat.SelectedIndex;
            double.TryParse(Txt_Latitude.Text, out Station.latitude);
            double.TryParse(Txt_Longitude.Text, out Station.longitude);
            double.TryParse(Txt_Elevation.Text, out Station.elevation);

            foreach (var location in Station.sensorLocation)
            {
                location._elevation = Station.elevation;
                location._latitude = Station.latitude;
                location._longitude = Station.longitude;
                location.start = Station.start;

                // ayeuna datagridview
                int numrows = dataGridView1.RowCount;
                string stream_name = string.Empty;
                for (int i = 0; i < numrows; i++)
                {
                    if (dataGridView1.Rows[i].Cells["Clm_Code"].Value != null)
                    {
                        stream_name = dataGridView1.Rows[i].Cells["Clm_Code"].Value.ToString();
                    }
                    else
                    {
                        continue;
                    }
                    var stream = location._streams.Find(q => q.code == stream_name);
                    if (stream == null)
                    {
                        stream = new StreamPtr();
                        stream.code = stream_name;
                        location._streams.Add(stream);
                    }
                    if (dataGridView1.Rows[i].Cells["Clm_SampleRate"].Value != null)
                    {
                        double.TryParse(dataGridView1.Rows[i].Cells["Clm_SampleRate"].Value.ToString(), out stream.sampleRateNumerator);
                    }
                    if (dataGridView1.Rows[i].Cells["CLm_Sensor"].Value != null)
                    {
                        stream.sensor = dataGridView1.Rows[i].Cells["CLm_Sensor"].Value.ToString();
                    }
                    if (dataGridView1.Rows[i].Cells["Clm_SerialNumber"].Value != null)
                    {
                        stream.sensorSerialNumber = dataGridView1.Rows[i].Cells["Clm_SerialNumber"].Value.ToString();
                    }
                    if (dataGridView1.Rows[i].Cells["Clm_Gain"].Value != null)
                    {
                        double.TryParse(dataGridView1.Rows[i].Cells["Clm_Gain"].Value.ToString(), out stream.gain);
                    }
                    if (dataGridView1.Rows[i].Cells["Clm_GainFrequency"].Value != null)
                    {
                        double.TryParse(dataGridView1.Rows[i].Cells["Clm_GainFrequency"].Value.ToString(), out stream.gainFrequency);
                    }
                    if (dataGridView1.Rows[i].Cells["Clm_GainUnit"].Value != null)
                    {
                        stream.gainUnit = dataGridView1.Rows[i].Cells["Clm_GainUnit"].Value.ToString();
                    }
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CheckLonLat()
        {
            if (Double.IsNaN(longitude) && !String.IsNullOrEmpty(Txt_Longitude.Text))
            {
                longitude = Convert.ToDouble(Txt_Longitude.Text);
            }
            if (Double.IsNaN(latitude) && !String.IsNullOrEmpty(Txt_Latitude.Text))
            {
                latitude = Convert.ToDouble(Txt_Latitude.Text);
            }
            if (Double.IsNaN(elevation) && !String.IsNullOrEmpty(Txt_Elevation.Text))
            {
                elevation = Convert.ToDouble(Txt_Elevation.Text);
            }
        }

        private void Btn_AddChannel_Click(object sender, EventArgs e)
        {
            frm_Setup_AddChannel f_addchannel = new frm_Setup_AddChannel(this.parent);
            f_addchannel.StationName = Txt_Name.Text;
            if (f_addchannel.ShowDialog() == DialogResult.OK)
            {
                if (Station == null)
                {
                    Station = new Class_Station();
                }

                var new_channel = f_addchannel.Stream;
                int idx = dataGridView1.Rows.Add(1);
                dataGridView1.Rows[idx].Cells["Clm_Code"].Value = new_channel.code;
                dataGridView1.Rows[idx].Cells["Clm_SampleRate"].Value = (new_channel.sampleRateNumerator * new_channel.sampleRateDenominator);
                dataGridView1.Rows[idx].Cells["Clm_Sensor"].Value = new_channel.sensor;
                dataGridView1.Rows[idx].Cells["Clm_SerialNumber"].Value = new_channel.sensorSerialNumber;
                dataGridView1.Rows[idx].Cells["Clm_Gain"].Value = Double.IsNaN(new_channel.gain) ? "" : new_channel.gain.ToString();
                dataGridView1.Rows[idx].Cells["Clm_GainFrequency"].Value = Double.IsNaN(new_channel.gainFrequency) ? "" : new_channel.gainFrequency.ToString();
                dataGridView1.Rows[idx].Cells["Clm_GainUnit"].Value = new_channel.gainUnit;

                //var location_code = String.IsNullOrEmpty(new_channel.locationCode) ? "" : new_channel.locationCode;
                var location = Station.sensorLocation.Find(q => q._code == new_channel.locationCode);
                if (location == null)
                {
                    location = new SensorLocationPtr();
                    location._code = new_channel.locationCode;
                    Station.sensorLocation.Add(location);
                    if (!String.IsNullOrEmpty(Txt_Latitude.Text))
                    {
                        double.TryParse(Txt_Latitude.Text, out location._latitude);
                    }
                    if (!String.IsNullOrEmpty(Txt_Longitude.Text))
                    {
                        double.TryParse(Txt_Longitude.Text, out location._longitude);
                    }
                    if (!String.IsNullOrEmpty(Txt_Elevation.Text))
                    {
                        double.TryParse(Txt_Elevation.Text, out location._elevation);
                    }
                }
                var channel = location._streams.Find(q => q.code == new_channel.code);
                if (channel == null)
                {
                    channel = new StreamPtr();
                    channel.code = new_channel.code;
                    location._streams.Add(channel);
                }
                channel.sampleRateDenominator = new_channel.sampleRateDenominator;
                channel.sampleRateNumerator = new_channel.sampleRateNumerator;
                channel.azimuth = new_channel.azimuth;
                channel.depth = new_channel.depth;
                channel.dip = new_channel.dip;
                channel.gain = new_channel.gain;
                channel.gainFrequency = new_channel.gainFrequency;
                channel.gainUnit = new_channel.gainUnit;
                channel.restricted = new_channel.restricted;
                channel.shared = new_channel.shared;
                channel.start = DateTimeUing.FromDateTime(DateTime.Now);
            }
        }

        private void Btn_EditChannel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                return;
            }
            var row = dataGridView1.SelectedRows[0];
            if (row.Cells["Clm_Code"].Value == null)
            {
                return;
            }
            var channel = Station.sensorLocation[0]._streams.Find(q => q.code == row.Cells[0].Value.ToString());
            if (channel == null)
            {
                return;
            }
            frm_Setup_AddChannel f_addchannel = new frm_Setup_AddChannel(this.parent);
            f_addchannel.StationName = Txt_Name.Text;
            f_addchannel.Stream = channel;
            if (f_addchannel.ShowDialog() == DialogResult.OK)
            {
                var new_channel = f_addchannel.Stream;

                row.Cells["Clm_Code"].Value = new_channel.code;
                row.Cells["Clm_Sensor"].Value = new_channel.sensor;
                row.Cells["Clm_SerialNumber"].Value = new_channel.sensorSerialNumber;
                row.Cells["Clm_SampleRate"].Value = (new_channel.sampleRateNumerator * new_channel.sampleRateDenominator);
                row.Cells["Clm_Gain"].Value = Double.IsNaN(new_channel.gain) ? "" : new_channel.gain.ToString();
                row.Cells["Clm_GainFrequency"].Value = Double.IsNaN(new_channel.gainFrequency) ? "" : new_channel.gainFrequency.ToString();
                row.Cells["Clm_GainUnit"].Value = new_channel.gainUnit;

                channel.sampleRateDenominator = new_channel.sampleRateDenominator;
                channel.sampleRateNumerator = new_channel.sampleRateNumerator;
                channel.azimuth = new_channel.azimuth;
                channel.depth = new_channel.depth;
                channel.dip = new_channel.dip;
                channel.gain = new_channel.gain;
                channel.gainFrequency = new_channel.gainFrequency;
                channel.gainUnit = new_channel.gainUnit;
                channel.restricted = new_channel.restricted;
                channel.shared = new_channel.shared;
                channel.sensor = new_channel.sensor;
                channel.sensorSerialNumber = new_channel.sensorSerialNumber;
            }
        }

        private void Btn_DeleteChannel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                return;
            }
            var row = dataGridView1.SelectedRows[0];
            if (MessageBox.Show("Sure to delete channel " + row.Cells[0].Value + "?", "Delete Channel", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

            }
        }

        private void Cbb_LocationFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Cbb_LocationFormat.SelectedIndex == 0)
            {
                Lbl_X.Text = "Longitude";
                Lbl_Y.Text = "Latitude";
            }
           else if (Cbb_LocationFormat.SelectedIndex == 1)
            {
                Lbl_X.Text = "Easting";
                Lbl_Y.Text = "Northing";
            }
        }
    }
}
