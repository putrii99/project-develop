using System;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;
using Inventory;

namespace SeisWing
{
    public partial class frm_Setup_Station : Form
    {
        private frm_parent parent;
        public String NetworkName;
        public String StationName;
        public CL_INVENTORY Inventory;
        private CL_INV_STATION Inventory_Station;
        public CL_INV_SENSOR SensorLocation;
        public CL_INV_CHANNEL Stream;
        private double longitude = double.NaN;
        private double latitude = double.NaN;
        private double elevation = double.NaN;
        public Boolean Changed = false;
        private ComboBox Cbb_LocationFormat;
        //public new CL_INV_STATION Station { get; set; }

        public frm_Setup_Station(frm_parent _parent)
        {
            this.Load += new System.EventHandler(this.frm_Setup_AddStation_Load);
            this.parent = _parent;
            InitializeComponent();

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void frm_Setup_AddStation_Load(object sender, EventArgs e)
        {
            if (NetworkName != null && !string.IsNullOrEmpty(NetworkName))
            {
                Txt_NetworkName.Text = NetworkName;
            }

            if (Inventory.Network != null && Inventory.Network.Stations != null)
            {
                Inventory_Station = Inventory.Network.Stations.Find(q => q.Name == StationName);

                if (Inventory_Station != null)
                {
                    Text = "Setup - Edit Station";
                    Txt_Name.Text = Inventory_Station.Name;
                    dateTimePicker1.Value = Inventory_Station.TimeBegin;

                    if (Inventory_Station.TimeEnd > dateTimePicker2.MinDate && Inventory_Station.TimeEnd < dateTimePicker2.MaxDate)
                    {
                        dateTimePicker2.Value = Inventory_Station.TimeEnd;
                    }
                    else
                    {
                        dateTimePicker2.Value = dateTimePicker2.MaxDate;
                    }

                    Txt_Description.Text = Inventory_Station.Description;
                    Txt_Latitude.Text = double.IsNaN(Inventory_Station.Latitude) ? "" : Inventory_Station.Latitude.ToString();
                    Txt_Longitude.Text = double.IsNaN(Inventory_Station.Longitude) ? "" : Inventory_Station.Longitude.ToString();
                    Txt_Elevation.Text = double.IsNaN(Inventory_Station.Elevation) ? "" : Inventory_Station.Elevation.ToString();
                    Txt_WaterLevel.Text = double.IsNaN(Inventory_Station.WaterLevel) ? "" : Inventory_Station.WaterLevel.ToString();

                    if (Inventory_Station.Site != null)
                    {
                        Txt_SiteName.Text = Inventory_Station.Site.Name;
                        Txt_SiteCountry.Text = Inventory_Station.Site.Country;
                    }

                    ////eusian channel
                    UpdateChannels();
                }

                Changed = false;
            }
        }

        private void UpdateChannels()
        {
            dataGridView1.Rows.Clear();
            if (Inventory_Station.Site != null && Inventory_Station.Channels != null)
            {
                foreach (var channel in Inventory_Station.Channels)
                {
                    int irow = dataGridView1.Rows.Add(1);
                    dataGridView1.Rows[irow].Cells["Clm_Code"].Value = channel.Name;
                    dataGridView1.Rows[irow].Cells["Clm_SampleRate"].Value = channel.SampleRate;
                    if (channel.Sensor != null)
                    {
                        dataGridView1.Rows[irow].Cells["CLm_Sensor"].Value = channel.Sensor.Description;
                    }
                    if (channel.DataLogger != null)
                    {
                        dataGridView1.Rows[irow].Cells["Clm_Datalogger"].Value = channel.DataLogger.Description;
                    }
                    if (channel.Response != null && channel.Response.InstrumentSensitivity != null)
                    {
                        dataGridView1.Rows[irow].Cells["Clm_Sensitivity"].Value = Double.IsNaN(channel.Response.InstrumentSensitivity.Value) ? "" : channel.Response.InstrumentSensitivity.Value.ToString();
                        dataGridView1.Rows[irow].Cells["Clm_Frequency"].Value = Double.IsNaN(channel.Response.InstrumentSensitivity.Frequency) ? "" : channel.Response.InstrumentSensitivity.Frequency.ToString();
                    }
                }
            }
        }

        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            if (Changed)
            {
                if (Inventory_Station == null)
                {
                    Inventory_Station = new CL_INV_STATION();
                }
                Inventory_Station.Name = Txt_Name.Text;
                Inventory_Station.TimeBegin = dateTimePicker1.Value;
                Inventory_Station.TimeEnd = dateTimePicker2.Value;
                Inventory_Station.Description = Txt_Description.Text;

               if(Inventory_Station.Site == null)
               {
                    Inventory_Station.Site=new CL_INV_SITE();
                    
               }
                //Station.shared = checkBox2.Checked;
                //Station.LocationFormat = Cbb_LocationFormat.SelectedIndex;
                Inventory_Station.Site.Name = Txt_SiteName.Text;
                Inventory_Station.Site.Country = Txt_SiteCountry.Text;

                Inventory_Station.LocationFormat = Cbb_LocationFormat.SelectedIndex;

                if (!String.IsNullOrEmpty(Txt_Latitude.Text))
                {
                    double.TryParse(Txt_Latitude.Text, out Inventory_Station.Latitude);
                }
                if (!String.IsNullOrEmpty(Txt_Longitude.Text))
                {
                    double.TryParse(Txt_Longitude.Text, out Inventory_Station.Longitude);
                }
                if (!String.IsNullOrEmpty(Txt_Elevation.Text))
                {
                    double.TryParse(Txt_Elevation.Text, out Inventory_Station.Elevation);
                }

                // save heula
                Inventory.Save(parent.FolderRepoInventory);

                foreach (var location in Inventory_Station.sensorLocation)
                {
                    location.Elevation = Inventory_Station.Elevation;
                    location.Latitude = Inventory_Station.Latitude;
                    location.Longitude = Inventory_Station.Longitude;
                    location.TimeBegin = Inventory_Station.TimeBegin;

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
                        var stream = location.Streams.Find(q => q.Name == stream_name);
                        if (stream == null)
                        {
                            stream = new CL_INV_CHANNEL();
                            stream.Name = stream_name;
                            location.Streams.Add(stream);
                        }
                        if (dataGridView1.Rows[i].Cells["Clm_SampleRate"].Value != null)
                        {
                            double.TryParse(dataGridView1.Rows[i].Cells["Clm_SampleRate"].Value.ToString(), out stream.SampleRateNumerator);
                        }
                        if (dataGridView1.Rows[i].Cells["CLm_Sensor"].Value != null)
                        {
                            //stream.Sensor = dataGridView1.Rows[i].Cells["CLm_Sensor"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["Clm_SerialNumber"].Value != null)
                        {
                            stream.SensorSerialNumber = dataGridView1.Rows[i].Cells["Clm_SerialNumber"].Value.ToString();
                        }
                        if (dataGridView1.Rows[i].Cells["Clm_Gain"].Value != null)
                        {
                            double.TryParse(dataGridView1.Rows[i].Cells["Clm_Gain"].Value.ToString(), out stream.Gain);
                        }
                        if (dataGridView1.Rows[i].Cells["Clm_GainFrequency"].Value != null)
                        {
                            double.TryParse(dataGridView1.Rows[i].Cells["Clm_GainFrequency"].Value.ToString(), out stream.GainFrequency);
                        }
                        if (dataGridView1.Rows[i].Cells["Clm_GainUnit"].Value != null)
                        {
                            stream.GainUnit = dataGridView1.Rows[i].Cells["Clm_GainUnit"].Value.ToString();
                        }
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
            frm_Setup_Channel f_addchannel = new frm_Setup_Channel(this.parent);
            f_addchannel.StationName = Txt_Name.Text;
            if (f_addchannel.ShowDialog() == DialogResult.OK)
            {
                if (Inventory_Station == null)
                {
                    Inventory_Station = new CL_INV_STATION();
                }

                var new_channel = f_addchannel.Channel;
                int idx = dataGridView1.Rows.Add(1);

                dataGridView1.Rows[idx].Cells["Clm_Code"].Value = new_channel.Name;
                dataGridView1.Rows[idx].Cells["Clm_SampleRate"].Value = new_channel.SampleRate;
                if (new_channel.Sensor != null)
                {
                    dataGridView1.Rows[idx].Cells["CLm_Sensor"].Value = new_channel.Sensor.Description;
                }
                if (new_channel.DataLogger != null)
                {
                    dataGridView1.Rows[idx].Cells["Clm_Datalogger"].Value = new_channel.DataLogger.Description;
                }
                if (new_channel.Response != null && new_channel.Response.InstrumentSensitivity != null)
                {
                    dataGridView1.Rows[idx].Cells["Clm_Sensitivity"].Value = Double.IsNaN(new_channel.Response.InstrumentSensitivity.Value) ? "" : new_channel.Response.InstrumentSensitivity.Value.ToString();
                    dataGridView1.Rows[idx].Cells["Clm_Frequency"].Value = Double.IsNaN(new_channel.Response.InstrumentSensitivity.Frequency) ? "" : new_channel.Response.InstrumentSensitivity.ToString();
                }

                //var location_code = String.IsNullOrEmpty(new_channel.locationCode) ? "" : new_channel.locationCode;
                var location = Station.sensorLocation.Find(q => q.Name == new_channel.LocationCode);
                if (location == null)
                {
                    location = new CL_INV_STATION.SensorLocation();
                    location.Name = new_channel.LocationCode;
                    Station.sensorLocation.Add(location);
                    if (!String.IsNullOrEmpty(Txt_Latitude.Text))
                    {
                        double latitude;
                        double.TryParse(Txt_Latitude.Text, out latitude);
                        {
                            location.Latitude = latitude;
                        }
                    }
                    if (!String.IsNullOrEmpty(Txt_Longitude.Text))
                    {
                        double longitude;
                        double.TryParse(Txt_Longitude.Text, out longitude);
                        {
                            location.Longitude = longitude;
                        }
                    }
                    if (!String.IsNullOrEmpty(Txt_Elevation.Text))
                    {
                        double.TryParse(Txt_Elevation.Text, out elevation);
                        location.Elevation = elevation;
                    }
                }
                var channel = location.Streams.Find(q => q.Name == new_channel.Name);
                if (channel == null)
                {
                    channel = new CL_INV_CHANNEL();
                    channel.Name = new_channel.Name;
                    location.Streams.Add(channel);
                }
                channel.SampleRateDenominator = new_channel.SampleRateDenominator;
                channel.SampleRateNumerator = new_channel.SampleRateNumerator;
                channel.Azimuth = new_channel.Azimuth;
                channel.Depth = new_channel.Depth;
                channel.Dip = new_channel.Dip;
                channel.Gain = new_channel.Gain;
                channel.GainFrequency = new_channel.GainFrequency;
                channel.GainUnit = new_channel.GainUnit;
                channel.RestrictedStatus = new_channel.RestrictedStatus;
                channel.Shared = new_channel.Shared;
                channel.TimeBegin = DateTime.Now;
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
            var channel = Inventory_Station.Channels.Find(q => q.Name == row.Cells[0].Value.ToString());
            if (channel == null)
            {
                return;
            }
            frm_Setup_Channel f_addchannel = new frm_Setup_Channel(this.parent);
            f_addchannel.StationName = Txt_Name.Text;
            f_addchannel.Channel = channel;
            if (f_addchannel.ShowDialog() == DialogResult.OK)
            {
                var new_channel = f_addchannel.Channel;

                row.Cells["Clm_Code"].Value = new_channel.Name;
                row.Cells["Clm_SampleRate"].Value = new_channel.SampleRate;
                if (channel.Sensor != null)
                {
                    row.Cells["CLm_Sensor"].Value = new_channel.Sensor.Description;
                }
                if (channel.DataLogger != null)
                {
                    row.Cells["Clm_Datalogger"].Value = new_channel.DataLogger.Description;
                }
                if (new_channel.Response != null && new_channel.Response.InstrumentSensitivity != null)
                {
                    row.Cells["Clm_Sensitivity"].Value = Double.IsNaN(new_channel.Response.InstrumentSensitivity.Value) ? "" : new_channel.Response.InstrumentSensitivity.Value.ToString();
                    row.Cells["Clm_Frequency"].Value = Double.IsNaN(new_channel.Response.InstrumentSensitivity.Frequency) ? "" : new_channel.Response.InstrumentSensitivity.ToString();
                }

                channel.SampleRateDenominator = new_channel.SampleRateDenominator;
                channel.SampleRateNumerator = new_channel.SampleRateNumerator;
                channel.Azimuth = new_channel.Azimuth;
                channel.Depth = new_channel.Depth;
                channel.Dip = new_channel.Dip;
                channel.RestrictedStatus = new_channel.RestrictedStatus;
                channel.Sensor = new_channel.Sensor;
                channel.SensorSerialNumber = new_channel.SensorSerialNumber;
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

        private void Btn_AddResponses_Click(object sender, EventArgs e)
        {
            frm_Setup_Inventory_AddResponse f_Setup_NetworkStationImportFsdnXml = new frm_Setup_Inventory_AddResponse(this.parent);
            f_Setup_NetworkStationImportFsdnXml.StartPosition = FormStartPosition.CenterParent;
            f_Setup_NetworkStationImportFsdnXml.Updated += F_Setup_NetworkStationImportFsdnXml_Updated;
            if (f_Setup_NetworkStationImportFsdnXml.ShowDialog() == DialogResult.OK)
            {
                var Response = f_Setup_NetworkStationImportFsdnXml.Response;
                foreach (var channel in Inventory_Station.Channels)
                {
                    channel.Response = Response;
                }
                UpdateChannels();
                Changed = true;
            }
        }

        private void F_Setup_NetworkStationImportFsdnXml_Updated(object sender, EventArgs e)
        {
        }

        private void Txt_Name_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void Txt_Latitude_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void Txt_Longitude_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void Txt_Elevation_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void Txt_Description_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }
    }
}