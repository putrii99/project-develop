using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Inventory;
using SynapticEffect.Forms;
using System.Linq;

namespace SeisWing
{
    public partial class frm_Setup_Inventory : Form
    {
        private frm_parent parent;
        private List<CL_INVENTORY> Inventories = new List<CL_INVENTORY>();
        private SynapticEffect.Forms.TreeListView treeListView1;
        public CL_INV_SENSOR Sensor = new CL_INV_SENSOR();
        public CL_INV_STATION Station = new CL_INV_STATION();
        public CL_INV_CHANNEL Stream = new CL_INV_CHANNEL();
        public List<string> SelectedFiles = new List<string>();

        public frm_Setup_Inventory(frm_parent _parent)
        {
            this.parent = _parent;
            InitializeComponent();
            InitializeTreeListView();
        }

        private void InitializeTreeListView()
        {
            //this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_ViewPicks));
            SynapticEffect.Forms.ToggleColumnHeader Clm_Tree = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Network = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Station = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Channel = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Description = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Latitude = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Longitude = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Elevation = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Depth = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Azimuth = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Dip = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_SampleRate = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Sensitivity = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_Sensor = new SynapticEffect.Forms.ToggleColumnHeader();
            SynapticEffect.Forms.ToggleColumnHeader Clm_DataLogger = new SynapticEffect.Forms.ToggleColumnHeader();

            Clm_Tree.Hovered = false;
            Clm_Tree.Index = 0;
            Clm_Tree.Pressed = false;
            Clm_Tree.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Tree.Selected = false;
            Clm_Tree.Text = "-=-";
            Clm_Tree.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            //Clm_Tree.Visible = true;
            Clm_Tree.Width = 111;

            Clm_Network.Hovered = false;
            Clm_Network.Index = 0;
            Clm_Network.Pressed = false;
            Clm_Network.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Network.Selected = false;
            Clm_Network.Text = "Network";
            Clm_Network.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            //Clm_Network.Visible = true;
            Clm_Network.Width = 66;

            Clm_Station.Hovered = false;
            Clm_Station.Index = 0;
            Clm_Station.Pressed = false;
            Clm_Station.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Station.Selected = false;
            Clm_Station.Text = "Station";
            Clm_Station.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            //Clm_Station.Visible = true;
            Clm_Station.Width = 66;

            Clm_Channel.Hovered = false;
            Clm_Channel.Image = null;
            Clm_Channel.Index = 0;
            Clm_Channel.Pressed = false;
            Clm_Channel.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Channel.Selected = false;
            Clm_Channel.Text = "Channel";
            Clm_Channel.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            //Clm_Channel.Visible = true;
            Clm_Channel.Width = 66;

            Clm_Description.Hovered = false;
            Clm_Description.Index = 0;
            Clm_Description.Pressed = false;
            Clm_Description.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Description.Selected = false;
            Clm_Description.Text = "Description";
            Clm_Description.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            //Clm_Description.Visible = true;
            Clm_Description.Width = 100;

            Clm_Latitude.Hovered = false;
            Clm_Latitude.Image = null;
            Clm_Latitude.Index = 0;
            Clm_Latitude.Pressed = false;
            Clm_Latitude.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Latitude.Selected = false;
            Clm_Latitude.Text = "Latitude";
            //Clm_Latitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //Clm_Latitude.Visible = true;

            Clm_Longitude.Hovered = false;
            Clm_Longitude.Image = null;
            Clm_Longitude.Index = 0;
            Clm_Longitude.Pressed = false;
            Clm_Longitude.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Longitude.Selected = false;
            Clm_Longitude.Text = "Longitude";
            //Clm_Longitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //Clm_Longitude.Visible = true;

            Clm_Elevation.Hovered = false;
            Clm_Elevation.Image = null;
            Clm_Elevation.Index = 0;
            Clm_Elevation.Pressed = false;
            Clm_Elevation.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Elevation.Selected = false;
            Clm_Elevation.Text = "Elevation";
            //Clm_Elevation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //Clm_Elevation.Visible = true;

            Clm_Depth.Hovered = false;
            Clm_Depth.Image = null;
            Clm_Depth.Index = 0;
            Clm_Depth.Pressed = false;
            Clm_Depth.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Depth.Selected = false;
            Clm_Depth.Text = "Depth";
            //Clm_Depth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //Clm_Depth.Visible = true;

            Clm_Azimuth.Hovered = false;
            Clm_Azimuth.Image = null;
            Clm_Azimuth.Index = 0;
            Clm_Azimuth.Pressed = false;
            Clm_Azimuth.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Azimuth.Selected = false;
            Clm_Azimuth.Text = "Azimuth";
            //Clm_Azimuth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //Clm_Azimuth.Visible = true;

            Clm_Dip.Hovered = false;
            Clm_Dip.Image = null;
            Clm_Dip.Index = 0;
            Clm_Dip.Pressed = false;
            Clm_Dip.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Dip.Selected = false;
            Clm_Dip.Text = "Dip";
            //Clm_Dip.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //Clm_Dip.Visible = true;

            Clm_SampleRate.Hovered = false;
            Clm_SampleRate.Image = null;
            Clm_SampleRate.Index = 0;
            Clm_SampleRate.Pressed = false;
            Clm_SampleRate.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_SampleRate.Selected = false;
            Clm_SampleRate.Text = "Sample Rate";
            //Clm_Gain.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //Clm_Gain.Visible = true;

            Clm_Sensitivity.Hovered = false;
            Clm_Sensitivity.Image = null;
            Clm_Sensitivity.Index = 0;
            Clm_Sensitivity.Pressed = false;
            Clm_Sensitivity.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Sensitivity.Selected = false;
            Clm_Sensitivity.Text = "Sensitivity";
            //Clm_Filename.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;

            Clm_Sensor.Hovered = false;
            Clm_Sensor.Image = null;
            Clm_Sensor.Index = 0;
            Clm_Sensor.Pressed = false;
            Clm_Sensor.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_Sensor.Selected = false;
            Clm_Sensor.Text = "Sensor";
            //Clm_GainFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //Clm_GainFrequency.Visible = true;

            Clm_DataLogger.Hovered = false;
            Clm_DataLogger.Image = null;
            Clm_DataLogger.Index = 0;
            Clm_DataLogger.Pressed = false;
            Clm_DataLogger.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
            Clm_DataLogger.Selected = false;
            Clm_DataLogger.Text = "Data Loger";
            Clm_DataLogger.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            //Clm_GainUnit.Visible = true;

            this.treeListView1 = new SynapticEffect.Forms.TreeListView();
            this.treeListView1.Dock = DockStyle.Fill;
            this.treeListView1.BackColor = System.Drawing.SystemColors.Window;
            this.treeListView1.Columns.AddRange(new SynapticEffect.Forms.ToggleColumnHeader[] {
                Clm_Tree,Clm_Network,
                Clm_Station,
                Clm_Channel,
                Clm_Description,
                Clm_Latitude, Clm_Longitude,
                Clm_Elevation, Clm_Depth, Clm_Azimuth, Clm_Dip,
                Clm_SampleRate, Clm_Sensitivity, Clm_Sensor, Clm_DataLogger
            });
            this.treeListView1.ColumnSortColor = System.Drawing.Color.Gainsboro;
            this.treeListView1.ColumnTrackColor = System.Drawing.Color.WhiteSmoke;
            this.treeListView1.ColumnTracking = true;
            //this.treeListView1.ContextMenu = contextMenu1;
            this.treeListView1.GridLines = true;
            this.treeListView1.GridLineColor = System.Drawing.Color.Gray;
            this.treeListView1.HeaderMenu = null;
            this.treeListView1.ItemHeight = 20;
            //this.treeListView1.ItemMenu = contextTList;
            this.treeListView1.LabelEdit = false;
            this.treeListView1.Location = new System.Drawing.Point(8, 8);
            this.treeListView1.Name = "treeListView1";
            this.treeListView1.RowSelectColor = System.Drawing.SystemColors.Highlight;
            this.treeListView1.RowTrackColor = System.Drawing.Color.WhiteSmoke;
            this.treeListView1.RowTracking = true;
            this.treeListView1.ShowLines = true;
            this.treeListView1.Size = new System.Drawing.Size(694, 226);
            this.treeListView1.SmallImageList = imageList_checkbox;// this.listImages;
            this.treeListView1.StateImageList = null;
            this.treeListView1.TabIndex = 0;
            this.treeListView1.Text = "treeListView1";
            this.panel1.Controls.Add(this.treeListView1);
            this.treeListView1.VisualStyles = true;
            treeListView1.MouseDown += TreeListView1_MouseDown;
            treeListView1.MouseClick += TreeListView1_MouseClick;
            treeListView1.MouseClick += TreeListView1_MouseClick;
        }

        private void TreeListView1_MouseClick(object sender, MouseEventArgs e)
        {
            var tree = sender as SynapticEffect.Forms.TreeListView;
            //var cnode = tree.node
            var nodes = tree.SelectedNodes;
            var nnode = nodes.Count;
            var node = nodes[0];
            var mos = e.Button;
            switch (mos)
            {
                case MouseButtons.Left:
                    if (node != null && node.Tag != null)
                    {
                        Txt_Channel.Text = "";
                        Txt_Network.Text = "";
                        Txt_Station.Text = "";
                        if (node.Tag.ToString() == "node_network")
                        {
                            Txt_Network.Text = node.Text;
                        }
                        else if (node.Tag.ToString() == "node_station")
                        {
                            Txt_Network.Text = node.SubItems[0].Text;
                            Txt_Station.Text = node.Text;
                        }
                        else if (node.Tag.ToString() == "node_channel")
                        {
                            Txt_Network.Text = node.SubItems[0].Text;
                            Txt_Station.Text = node.SubItems[1].Text;
                            Txt_Channel.Text = node.Text;
                        }
                    }
                    break;
            }
        }
        ////private void SelectThisRow(object sender, EventArgs e)
        ////{
        ////    if (sender is MenuItem menuItem && menuItem.Tag is object[] tag && tag.Length > 0 && tag[0] is TreeNode node)
        ////    {
        ////        //
        ////    }
        ////}
        ////private void SelectAll(object sender, EventArgs e)
        ////{
        ////    // 
        ////}

        ////private void UnselectAll(object sender, EventArgs e)
        ////{
        ////    // 
        ////}

        ////private void PlotAll(object sender, EventArgs e)
        ////{
        ////    // 
        ////}

        ////private void PlotSelected(object sender, EventArgs e)
        ////{
        ////    // 
        ////}

        ////private void ProcessHypocenter(object sender, EventArgs e)
        ////{
        ////    //
        ////}
        ////private void InvertSelection(object sender, EventArgs e)
        ////{
        ////    //
        ////}

        private void TreeListView1_MouseDown(object sender, MouseEventArgs e)
        {
            var tree = sender as SynapticEffect.Forms.TreeListView;
            //var cnode = tree.node
            var nodes = tree.SelectedNodes;
            var nnode = nodes.Count;
            var node = nodes[0];
            var mos = e.Button;
            switch (mos)
            {
                case MouseButtons.Left:

                    break;

                case MouseButtons.Right:
                    if (nnode > 0)
                    {
                        int inode = 0;
                        ContextMenu m = new ContextMenu();

                        if (node.ImageIndex == 0)
                        {
                            m.MenuItems.Add(new MenuItem("Select this Pick"));
                           // m.MenuItems[0].Click += new System.EventHandler(this.SelectThisRow);
                            m.MenuItems[0].Tag = new object[] { node };
                        }
                        else
                        {
                            m.MenuItems.Add(new MenuItem("Unselect this Pick"));
                            //m.MenuItems[0].Click += new System.EventHandler(this.SelectThisRow);
                            m.MenuItems[0].Tag = new object[] { node };
                        }
                        m.MenuItems.Add(new MenuItem("-"));

                        m.MenuItems.Add(new MenuItem("Select All Picks"));
                       // m.MenuItems[inode].Click += new System.EventHandler(this.SelectAll);
                        inode++;

                        m.MenuItems.Add(new MenuItem("Unselet All Picks"));
                       // m.MenuItems[inode].Click += new System.EventHandler(this.UnselectAll);
                        inode++;

                        m.MenuItems.Add(new MenuItem("Invert Selection"));
                       // m.MenuItems[inode].Click += new System.EventHandler(this.InvertSelection);
                        inode++;

                        m.MenuItems.Add(new MenuItem("-"));
                        inode++;

                        m.MenuItems.Add(new MenuItem("Plot All Picks on Signals"));
                        //m.MenuItems[inode].Click += new System.EventHandler(this.PlotAll);
                        inode++;

                        m.MenuItems.Add(new MenuItem("Plot Selected Picks on Signals"));
                        m.MenuItems[inode].Tag = node;
                       // m.MenuItems[inode].Click += new System.EventHandler(this.PlotSelected);
                        inode++;

                        m.MenuItems.Add(new MenuItem("-"));
                        inode++;

                        m.MenuItems.Add(new MenuItem("Process Hypocenter Location"));
                        m.MenuItems[inode].Tag = node;
                       // m.MenuItems[inode].Click += new System.EventHandler(this.ProcessHypocenter);
                        inode++;

                        m.Show(treeListView1, new Point(e.X, e.Y));
                    }
                    break;
            }
        }

        private void frm_Setup_Load(object sender, EventArgs e)
        {
            if (parent.Inventories != null)
            {
                foreach (var inventory in parent.Inventories)
                {
                    PrintInventory(inventory);
                }
            }
        }

        public void PrintInventory(CL_INVENTORY inventory)
        {
            
            TreeListNode node_network = new TreeListNode();
            TreeListNode node_station = new TreeListNode();
            TreeListNode node_sensor = new TreeListNode();
            TreeListNode node_channel = new TreeListNode();

            // network heula
            //foreach (var network in inventory.Networks)
            //{
            var network = inventory.Network;
            node_network = treeListView1.FindNodesByText(treeListView1.Nodes, network.Name);
            if (node_network == null)
            {
                node_network = new TreeListNode();
                node_network.Tag = "node_network";
                node_network.Text = network.Name;
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                node_network.SubItems.Add("");
                treeListView1.Nodes.Add(node_network);
            }
            node_network.SubItems[3].Text = network.Description;

            foreach (var station in network.Stations)
            {
                node_station = treeListView1.FindNodesByText(node_network.Nodes, station.Name);
                if (node_station == null)
                {
                    node_station = new TreeListNode();
                    node_station.Tag = "node_station";
                    node_station.Text = station.Name;
                    node_station.SubItems.Add("");
                    node_station.SubItems.Add("");
                    node_station.SubItems.Add("");
                    node_station.SubItems.Add("");
                    node_station.SubItems.Add("");
                    node_station.SubItems.Add("");
                    node_station.SubItems.Add("");
                    node_network.Nodes.Add(node_station);
                }
                node_station.SubItems[0].Text = network.Name;
                node_station.SubItems[1].Text = station.Name;
                node_station.SubItems[3].Text = station.Description;
                node_station.SubItems[4].Text = Double.IsNaN(station.Latitude) ? "" : station.Latitude.ToString();
                node_station.SubItems[5].Text = Double.IsNaN(station.Longitude) ? "" : station.Longitude.ToString();
                node_station.SubItems[6].Text = Double.IsNaN(station.Elevation) ? "" : station.Elevation.ToString();

                if (station.Channels != null)
                {
                    foreach (var channel in station.Channels)
                    {
                        node_channel = treeListView1.FindNodesByText(node_station.Nodes, channel.Name);
                        if (node_channel == null)
                        {
                            node_channel = new TreeListNode();
                            node_channel.Tag = "node_channel";
                            node_channel.Text = channel.Name;
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add(""); //  9 Dip
                            node_channel.SubItems.Add(""); // 10 SampleRate
                            node_channel.SubItems.Add(""); // 11 sensitivity
                            node_channel.SubItems.Add(""); // 12 Sensor.Description
                            node_channel.SubItems.Add(""); // 13 DataLogger.Description
                            node_station.Nodes.Add(node_channel);
                        }
                        node_channel.SubItems[0].Text = network.Name;
                        node_channel.SubItems[1].Text = station.Name;
                        node_channel.SubItems[2].Text = channel.Name;
                        node_channel.SubItems[3].Text = channel.Description;
                        node_channel.SubItems[4].Text = Double.IsNaN(channel.Latitude) ? "" : channel.Latitude.ToString();
                        node_channel.SubItems[5].Text = Double.IsNaN(channel.Longitude) ? "" : channel.Longitude.ToString();
                        node_channel.SubItems[6].Text = Double.IsNaN(channel.Elevation) ? "" : channel.Elevation.ToString();
                        node_channel.SubItems[7].Text = Double.IsNaN(channel.Depth) ? "" : channel.Depth.ToString();
                        node_channel.SubItems[8].Text = Double.IsNaN(channel.Azimuth) ? "" : channel.Azimuth.ToString();
                        node_channel.SubItems[9].Text = Double.IsNaN(channel.Dip) ? "" : channel.Dip.ToString();
                        node_channel.SubItems[10].Text = Double.IsNaN(channel.SampleRate) ? "" : channel.SampleRate.ToString();
                        if (channel.Response != null && channel.Response.InstrumentSensitivity != null)
                        {
                            node_channel.SubItems[11].Text = Double.IsNaN(channel.Response.InstrumentSensitivity.Value) ? "" : channel.Response.InstrumentSensitivity.Value.ToString();
                        }
                        if (channel.Sensor != null)
                        {
                            node_channel.SubItems[12].Text = channel.Sensor.Description;
                        }
                        if (channel.DataLogger != null)
                        {
                            node_channel.SubItems[13].Text = channel.DataLogger.Description;
                        }
                    }
                }
            }
            node_network.Expand();
            //}
        }

        private void Btn_AddNetwork_Click(object sender, EventArgs e)
        {
            frm_Setup_Network f_addnetwork = new frm_Setup_Network(this.parent);
            if (f_addnetwork.ShowDialog() == DialogResult.OK)
            {
                var network = f_addnetwork.Network;

                //teang heula network ieu geus aya can??
                TreeListNode node_network = treeListView1.FindNodesByText(treeListView1.Nodes, network.Name);
                if (node_network == null)
                {
                    node_network = new TreeListNode();
                    node_network.Tag = "node_network";
                    node_network.Text = network.Name;
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add(network.Description);
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    treeListView1.Nodes.Add(node_network);
                }

                // teangan di inventory
                foreach (var Inventory in parent.Inventories)
                {
                    var inv_network = Inventory.Network;// s.Find(q => q.Name == network.Name);
                    if (inv_network == null)
                    {
                        inv_network = new CL_INV_NETWORK();
                        inv_network.Name = network.Name;
                        Inventory.Network = inv_network;// s.Add(inv_network);
                    }
                    inv_network.Description = network.Description;
                    inv_network.institutions = network.institutions;
                    inv_network.LatMax = network.LatMax;
                    inv_network.LatMin = network.LatMin;
                    inv_network.LonMax = network.LonMax;
                    inv_network.LonMin = network.LonMin;
                    inv_network.region = network.region;
                    inv_network.RestrictedStatus = network.RestrictedStatus;
                    inv_network.shared = network.shared;
                    inv_network.TimeBegin = network.TimeBegin;
                    inv_network.Stations = network.Stations;
                    treeListView1.Invalidate();
                }
            }
        }

        private void Btn_AddStation_Click(object sender, EventArgs e)
        {
           
            if (String.IsNullOrEmpty(Txt_Network.Text))
            {
                return;
            }
            if (String.IsNullOrEmpty(Txt_Station.Text))
            {
                return;
            }
            if (String.IsNullOrEmpty(Txt_Network.Text) || String.IsNullOrEmpty(Txt_Station.Text))
            {
                return;
            }
            var network = parent.Inventory.Networks.FirstOrDefault(q => q.Name == Txt_Network.Text);
            if (network == null)
            {
                return;
            }
            var station = network.Stations.FirstOrDefault(q => q.Name == Txt_Station.Text);
            if (station == null)
            {
                station = new CL_INV_STATION();
                station.Name = Txt_Station.Text;
                network.Stations.Add(station);
                return;
            }
            //try
            //{
                frm_Setup_Station f_addstation = new frm_Setup_Station(this.parent);
                f_addstation.NetworkName = network.Name;
                f_addstation.Station = station;

            if (f_addstation.ShowDialog() == DialogResult.OK)
            {
                var node_network = treeListView1.FindNodesByText(treeListView1.Nodes, network.Name);
                if (node_network != null)
                {
                    Console.WriteLine("Network tidak ditemukan" + network.Name);
                    var new_station = f_addstation.Station;
                    var node_station = treeListView1.FindNodesByText(node_network.Nodes, new_station.Name);
                    if (node_station == null)
                    {
                        node_station = new TreeListNode();
                        node_station.Tag = "node_station";
                        node_station.Text = new_station.Name;
                        node_network.Nodes.Add(node_station);
                        node_station.SubItems.Add(network.Name);
                        node_station.SubItems.Add(new_station.Name);
                        node_station.SubItems.Add("");
                        node_station.SubItems.Add("");
                        node_station.SubItems.Add("");
                        node_station.SubItems.Add("");
                        node_station.SubItems.Add("");
                        node_station.SubItems[3].Text = new_station.Description;
                        node_station.SubItems[4].Text = Double.IsNaN(new_station.Latitude) ? "" : new_station.Latitude.ToString();
                        node_station.SubItems[5].Text = Double.IsNaN(new_station.Longitude) ? "" : new_station.Longitude.ToString();
                        node_station.SubItems[6].Text = Double.IsNaN(new_station.Elevation) ? "" : new_station.Elevation.ToString();

                        foreach (var location in new_station.sensorLocation)
                        {
                            foreach (var new_channel in location.Streams)
                            {
                                var node_channel = treeListView1.FindNodesByText(node_station.Nodes, new_channel.Name);
                                if (node_channel == null)
                                {
                                    node_channel = new TreeListNode();
                                    node_channel.Tag = "node_channel";
                                    node_channel.Text = new_channel.Name;
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_channel.SubItems.Add("");
                                    node_station.Nodes.Add(node_channel);
                                }
                                node_channel.SubItems[0].Text = Txt_Network.Text;
                                node_channel.SubItems[1].Text = new_station.Name;
                                node_channel.SubItems[2].Text = new_channel.Name;
                                node_channel.SubItems[7].Text = Double.IsNaN(new_channel.Azimuth) ? "" : new_channel.Azimuth.ToString();
                                node_channel.SubItems[8].Text = Double.IsNaN(new_channel.Depth) ? "" : new_channel.Depth.ToString();
                                node_channel.SubItems[9].Text = Double.IsNaN(new_channel.Dip) ? "" : new_channel.Dip.ToString();
                                node_channel.SubItems[10].Text = Double.IsNaN(new_channel.Gain) ? "" : new_channel.Gain.ToString();
                                node_channel.SubItems[11].Text = Double.IsNaN(new_channel.GainFrequency) ? "" : new_channel.GainFrequency.ToString();
                                node_channel.SubItems[12].Text = new_channel.GainUnit;
                            }
                        }
                        node_station.ExpandAll();
                        treeListView1.Invalidate();
                    }

                    // ayeuna station di inventory
                    foreach (var Inventory in parent.Inventories)
                    {
                        var inv_station = Inventory.Network.Stations.Find(q => q.Name == new_station.Name);
                        if (inv_station == null)
                        {
                            inv_station = new CL_INV_STATION();
                            network.Stations.Add(inv_station);
                            inv_station.Name = new_station.Name;
                        }
                        inv_station.affiliation = new_station.affiliation;
                        inv_station.NetworkName = new_station.NetworkName;
                        inv_station.country = new_station.country;
                        inv_station.Description = new_station.Description;
                        inv_station.Elevation = new_station.Elevation;
                        inv_station.Latitude = new_station.Latitude;
                        inv_station.Longitude = new_station.Longitude;
                        inv_station.RestrictedStatus = new_station.RestrictedStatus;
                        inv_station.sensorLocation = new_station.sensorLocation;
                        inv_station.shared = new_station.shared;
                        inv_station.TimeBegin = new_station.TimeBegin;
                    }
                    treeListView1.Invalidate();
                }
            }
        }
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Exception occurred: " + ex.Message);
            //}

        private void Btn_AddChannel_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Txt_Network.Text))
            {
                return;
            }
            if (String.IsNullOrEmpty(Txt_Station.Text))
            {
                return;
            }
            var network = parent.Inventory.Network;
            if (network == null)
            {
                return;
            }
            var station = network.Stations.Find(q => q.Name == Txt_Station.Text);
            if (station == null)
            {
                return;
            }
            frm_Setup_Channel f_addchannel = new frm_Setup_Channel(this.parent);
            f_addchannel.NetworkName = Txt_Network.Text;
            f_addchannel.StationName = Txt_Station.Text;
            if (f_addchannel.ShowDialog() == DialogResult.OK)
            {
                var node_network = treeListView1.FindNodesByText(treeListView1.Nodes, Txt_Network.Text);
                if (node_network == null)
                {
                    return;
                }
                var node_station = treeListView1.FindNodesByText(node_network.Nodes, Txt_Station.Text);
                if (node_station == null)
                {
                    return;
                }
                var new_channel = f_addchannel.Channel;
                AddChannel(station, node_station, new_channel);
                treeListView1.Invalidate();
            }
        }
        private void Btn_EditNetwork_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Txt_Network.Text))
            {
                return;
            }
            var network = parent.Inventory.Networks.Find(q => q.Name == Txt_Network.Text);
            if (network == null)
            {
                return;
            }
            frm_Setup_Network f_addnetwork = new frm_Setup_Network(this.parent);
            f_addnetwork.Network = network;
            if (f_addnetwork.ShowDialog() == DialogResult.OK)
            {
                var new_network = f_addnetwork.Network;

                // update network in inventory
                network.Name = new_network.Name;
                network.Description = new_network.Description;
                network.institutions = new_network.institutions;
                network.LatMax = new_network.LatMax;
                network.LatMin = new_network.LatMin;
                network.LonMax = new_network.LonMax;
                network.LonMin = new_network.LonMin;
                network.region = new_network.region;
                network.RestrictedStatus = new_network.RestrictedStatus;
                network.shared = new_network.shared;
                network.TimeBegin = new_network.TimeBegin;

                parent.Inventory.Save(parent.FolderRepoInventory);

                //update node network
                var node_network = treeListView1.FindNodesByText(treeListView1.Nodes, network.Name);
                if (node_network == null)
                {
                    node_network = new TreeListNode();
                    node_network.Tag = "node_network";
                    node_network.Text = network.Name;
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add(network.Description);
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    node_network.SubItems.Add("");
                    treeListView1.Nodes.Add(node_network);
                }
                node_network.SubItems[3].Text = network.Description;
                treeListView1.Invalidate();
            }
        }

        private void Btn_EditStation_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Txt_Network.Text))
            {
                return;
            }
            if (String.IsNullOrEmpty(Txt_Station.Text))
            {
                return;
            }
            var inventories = new List<CL_INVENTORY>();
            var inv_networks = new List<CL_INV_NETWORK>();
            var inv_stations = new List<CL_INV_STATION>();
            var selected_inventory = new CL_INVENTORY();
            foreach (var inventory in parent.Inventories)
            {
                if (inventory.Network != null) // s.Find(q=>q.Name==Txt_Network.Text)!=null)
                {
                    selected_inventory = inventory;
                }
                foreach (var network in inventory.Networks)
                {
                    if (network.Name == Txt_Network.Text)
                    {
                        foreach (var station in network.Stations)
                        {
                            if (station.Name == Txt_Station.Text)
                            {
                                inventories.Add(inventory);
                                inv_networks.Add(network);
                                inv_stations.Add(station);
                            }
                        }
                    }
                }
            }

            frm_Setup_Station f_Setup_Station = new frm_Setup_Station(this.parent);
            f_Setup_Station.NetworkName = Txt_Network.Text;
            f_Setup_Station.StationName = Txt_Station.Text;
            //f_Setup_Station.Inventory_Station = inv_stations[0];
            f_Setup_Station.Inventory = selected_inventory;
            if (f_Setup_Station.ShowDialog() == DialogResult.OK)
            {
                if (f_Setup_Station.Changed)
                {
                    selected_inventory = f_Setup_Station.Inventory;
                    var inv_network = selected_inventory.Network;// s[0];
                    var inv_station = inv_network.Stations.Find(q => q.Name == Txt_Station.Text);
                    // save heula 
                    inventories[0].Save(parent.FolderRepoInventory, inventories[0].XmlFilename);

                    var node_network = treeListView1.FindNodesByText(treeListView1.Nodes, Txt_Network.Text);
                    if (node_network != null)
                    {
                        //var new_station = f_Setup_Station.Inventory_Station;
                        var node_station = treeListView1.FindNodesByText(node_network.Nodes, Txt_Station.Text);
                        if (node_station != null)
                        {
                            node_station.SubItems[3].Text = inv_station.Description;
                            node_station.SubItems[4].Text = Double.IsNaN(inv_station.Latitude) ? "" : inv_station.Latitude.ToString();
                            node_station.SubItems[5].Text = Double.IsNaN(inv_station.Longitude) ? "" : inv_station.Longitude.ToString();
                            node_station.SubItems[6].Text = Double.IsNaN(inv_station.Elevation) ? "" : inv_station.Elevation.ToString();

                            //Station.Description = Station.Description;
                            //station.Latitude = new_station.Latitude;
                            //station.Longitude = new_station.Longitude;
                            //station.Elevation = new_station.Elevation;
                            //station.RestrictedStatus = new_station.RestrictedStatus;
                            ////station.shared = new_station.shared;
                            ////station.sensorLocation = new_station.sensorLocation;
                            ////if (station.sensorLocation.Count > 0)
                            ////{
                            ////    foreach (var channel in station.sensorLocation[0].Streams)
                            ////    {
                            ////        AddChannel(station, node_station, channel);
                            ////    }
                            ////}
                            
                            treeListView1.Invalidate();
                        }
                    }
                }
            }
        }

        private void Btn_EditChannel_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Txt_Network.Text))
            {
                return;
            }
            if (String.IsNullOrEmpty(Txt_Station.Text))
            {
                return;
            }
            if (String.IsNullOrEmpty(Txt_Channel.Text))
            {
                return;
            }
            var inventories = new List<CL_INVENTORY>();
            var inv_networks = new List<CL_INV_NETWORK>();
            var inv_stations = new List<CL_INV_STATION>();
            var inv_channels=new List<CL_INV_CHANNEL>();
            foreach (var inventory in parent.Inventories)
            {
                foreach (var network in inventory.Networks)
                {
                    var networks = inventory.Network;
                    if (network.Name == Txt_Network.Text)
                    {
                        foreach (var station in network.Stations)
                        {
                            if (station.Name == Txt_Station.Text)
                            {
                                foreach (var Channel in station.Channels)
                                {
                                    if (Channel.Name == Txt_Channel.Text)
                                    {
                                        inv_channels.Add(Channel);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //var network = parent.Inventory.Networks.Find(q => q.Name == Txt_Network.Text);
            //if (network == null)
            //{
            //    return;
            //}
            //////var station = network.Stations.Find(q => q.Name == Txt_Station.Text);
            //////if (station == null)
            //////{
            //////    return;
            //////}
            var channel = new CL_INV_CHANNEL();
            var tobreak = false;
            foreach (var location in Station.sensorLocation)
            {
                foreach (var stchannream in Station.Channels)
                {
                    if (stchannream.Name == Txt_Channel.Text)
                    {
                        channel = stchannream;
                        tobreak = true;
                        break;
                    }
                }
                if (tobreak)
                {
                    break;
                }
            }
            frm_Setup_Channel f_addChannel = new frm_Setup_Channel(this.parent);
            f_addChannel.NetworkName = Txt_Network.Text;
            f_addChannel.StationName = Txt_Station.Text;
            f_addChannel.Channel = inv_channels[0];
            if (f_addChannel.ShowDialog() == DialogResult.OK)
            {
                var new_channel = f_addChannel.Channel;

                // update heula node tre na
                var node_network = treeListView1.FindNodesByText(treeListView1.Nodes, Station.Name);
                if (node_network != null)
                {
                    var node_station = treeListView1.FindNodesByText(node_network.Nodes, Station.Name);
                    if (node_station != null)
                    {
                        var node_channel = treeListView1.FindNodesByText(node_station.Nodes, new_channel.Name);
                        if (node_channel == null)
                        {
                            node_channel = new TreeListNode();
                            node_channel.Tag = "node_channel";
                            node_channel.Text = new_channel.Name;
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_channel.SubItems.Add("");
                            node_station.Nodes.Add(node_channel);
                        }
                        node_channel.SubItems[0].Text = Txt_Network.Text;
                        node_channel.SubItems[1].Text = Station.Name;
                        node_channel.SubItems[2].Text = new_channel.Name;
                        node_channel.SubItems[7].Text = Double.IsNaN(new_channel.Azimuth) ? "" : new_channel.Azimuth.ToString();
                        node_channel.SubItems[8].Text = Double.IsNaN(new_channel.Depth) ? "" : new_channel.Depth.ToString();
                        node_channel.SubItems[9].Text = Double.IsNaN(new_channel.Dip) ? "" : new_channel.Dip.ToString();
                        node_channel.SubItems[10].Text = Double.IsNaN(new_channel.Gain) ? "" : new_channel.Gain.ToString();
                        node_channel.SubItems[11].Text = Double.IsNaN(new_channel.GainFrequency) ? "" : new_channel.GainFrequency.ToString();
                        node_channel.SubItems[12].Text = new_channel.GainUnit;
                    }
                }

                ////////    // ayeuna update inventory
                ////////    channel.Azimuth = new_channel.Azimuth;
                ////////    channel.Name = new_channel.Name;
                ////////    channel.Depth = new_channel.Depth;
                ////////    channel.Dip = new_channel.Dip;
                ////////    channel.Gain = new_channel.Gain;
                ////////    channel.GainFrequency = new_channel.GainFrequency;
                ////////    channel.GainUnit = new_channel.GainUnit;
                ////////    channel.LocationCode = new_channel.LocationCode;
                ////////    channel.RestrictedStatus = new_channel.RestrictedStatus;
                ////////    channel.SampleRateDenominator = new_channel.SampleRateDenominator;
                ////////    channel.SampleRateNumerator = new_channel.SampleRateNumerator;
                ////////    channel.Sensor = new_channel.Sensor;
                ////////    channel.SensorChannel = new_channel.SensorChannel;
                ////////    channel.SensorSerialNumber = new_channel.SensorSerialNumber;
                ////////    //channel.Shared = new_channel.Shared;
                ////////    channel.TimeBegin = new_channel.TimeBegin;
                ////////    treeListView1.Invalidate();
                ////////}
            }
        }

        private void AddChannel(CL_INV_STATION station, TreeListNode node_station, CL_INV_CHANNEL channel)
        {
            var node_channel = treeListView1.FindNodesByText(node_station.Nodes, channel.Name);
            if (node_channel == null)
            {
                node_channel = new TreeListNode();
                node_channel.Tag = "node_channel";
                node_channel.Text = channel.Name;
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_channel.SubItems.Add("");
                node_station.Nodes.Add(node_channel);
            }
            node_channel.SubItems[0].Text = Txt_Network.Text;
            node_channel.SubItems[1].Text = station.Name;
            node_channel.SubItems[2].Text = channel.Name;
            node_channel.SubItems[7].Text = Double.IsNaN(channel.Azimuth) ? "" : channel.Azimuth.ToString();
            node_channel.SubItems[8].Text = Double.IsNaN(channel.Depth) ? "" : channel.Depth.ToString();
            node_channel.SubItems[9].Text = Double.IsNaN(channel.Dip) ? "" : channel.Dip.ToString();
            node_channel.SubItems[10].Text = Double.IsNaN(channel.Gain) ? "" : channel.Gain.ToString();
            node_channel.SubItems[11].Text = Double.IsNaN(channel.GainFrequency) ? "" : channel.GainFrequency.ToString();
            node_channel.SubItems[12].Text = channel.GainUnit;

            var location = station.sensorLocation.Find(q => q.Name == channel.LocationCode);
            if (location == null)
            {
                CL_INV_STATION.SensorLocation newLocation = new CL_INV_STATION.SensorLocation();
                newLocation.Name = channel.LocationCode;
                newLocation.Latitude = station.Latitude;
                newLocation.Longitude = station.Longitude;
                newLocation.Elevation = station.Elevation;
                station.sensorLocation.Add(newLocation);
            }
            var stream = location.Streams.Find(q => q.Name == channel.Name);
            if (stream == null)
            {
                stream = new CL_INV_CHANNEL();
                stream.Name = channel.Name;
                location.Streams.Add(stream);
            }
            stream.LocationCode = channel.LocationCode;
            stream.Azimuth = channel.Azimuth;
            stream.Depth = channel.Depth;
            stream.Dip = channel.Dip;
            stream.Gain = channel.Gain;
            stream.GainFrequency = channel.GainFrequency;
            stream.GainUnit = channel.GainUnit;
            stream.RestrictedStatus = channel.RestrictedStatus;
            stream.SampleRateDenominator = channel.SampleRateDenominator;
            stream.SampleRateNumerator = channel.SampleRateNumerator;
            stream.Sensor = channel.Sensor;
            stream.SensorChannel = channel.SensorChannel;
            stream.SensorSerialNumber = channel.SensorSerialNumber;
            stream.Shared = channel.Shared;
            stream.TimeBegin = channel.TimeBegin;

            treeListView1.Invalidate();
            node_station.Expand();
        
        }

        private void Btn_DeleteNetwork_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Txt_Network.Text))
            {
                return;
            }
            var network = parent.Inventory.Networks.Find(q => q.Name == Txt_Network.Text);
            if (network == null)
            {
                return;
            }
            if (MessageBox.Show("Sure to delete network " + Txt_Network.Text + "?", "Delete Network", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // hupus heula node network na
                var node_network = treeListView1.FindNodesByText(treeListView1.Nodes, network.Name);
                if (node_network != null)
                {
                    treeListView1.Nodes.Remove(node_network);
                    treeListView1.Invalidate();
                }
                parent.Inventory.Networks.Remove(network);
            }
        }

        private void Btn_DeleteStation_Click(object sender, EventArgs e)
        {
            ////////if (String.IsNullOrEmpty(Txt_Network.Text))
            ////////{
            ////////    return;
            ////////}
            ////////if (String.IsNullOrEmpty(Txt_Station.Text))
            ////////{
            ////////    return;
            ////////}
            ////////var network = parent.Inventory.Networks.Find(q => q.Name == Txt_Network.Text);
            ////////if (network == null)
            ////////{
            ////////    return;
            ////////}
            ////////var station = network.Stations.Find(q => q.Name == Txt_Station.Text);
            ////////if (station == null)
            ////////{
            ////////    return;
            ////////}
            ////////if (MessageBox.Show("Sure to delete station " + Txt_Station.Text + " on network " + Txt_Network.Text + "?", "Delete Station", MessageBoxButtons.YesNo) == DialogResult.Yes)
            ////////{
            ////////    // hupus heula node network na
            ////////    var node_network = treeListView1.FindNodesByText(treeListView1.Nodes, network.Name);
            ////////    if (node_network != null)
            ////////    {
            ////////        var node_station = treeListView1.FindNodesByText(node_network.Nodes, station.Name);
            ////////        if (node_station != null)
            ////////        {
            ////////            node_network.Nodes.Remove(node_station);
            ////////            treeListView1.Invalidate();
            ////////        }
            ////////    }
            ////////    network.Stations.Remove(station);
            ////////}
        }

        private void Btn_DeleteChannel_Click(object sender, EventArgs e)
        {
            ////////if (String.IsNullOrEmpty(Txt_Network.Text))
            ////////{
            ////////    return;
            ////////}
            ////////if (String.IsNullOrEmpty(Txt_Station.Text))
            ////////{
            ////////    return;
            ////////}
            ////////if (String.IsNullOrEmpty(Txt_Channel.Text))
            ////////{
            ////////    return;
            ////////}
            ////////var network = parent.Inventory.Networks.Find(q => q.Name == Txt_Network.Text);
            ////////if (network == null)
            ////////{
            ////////    return;
            ////////}
            ////////var station = network.Stations.Find(q => q.Name == Txt_Station.Text);
            ////////if (station == null)
            ////////{
            ////////    return;
            ////////}
            ////////var channel = new CL_INV_CHANNEL();
            ////////var tobreak = false;
            //////////foreach (var location in station.sensorLocation)
            //////////{
            //////////    foreach (var stream in location.Streams)
            //////////    {
            //////////        if (stream.Name == Txt_Channel.Text)
            //////////        {
            //////////            channel = stream;
            //////////            tobreak = true;
            //////////            break;
            //////////        }
            //////////    }
            //////////    if (tobreak)
            //////////    {
            //////////        break;
            //////////    }
            //////////}
            ////////if (MessageBox.Show("Sure to delete channel " + Txt_Channel.Text + " on station " + Txt_Station.Text + "?", "Delete Channel", MessageBoxButtons.YesNo) == DialogResult.Yes)
            ////////{
            ////////    // hupus heula node network na
            ////////    var node_network = treeListView1.FindNodesByText(treeListView1.Nodes, network.Name);
            ////////    if (node_network != null)
            ////////    {
            ////////        var node_station = treeListView1.FindNodesByText(node_network.Nodes, station.Name);
            ////////        if (node_station != null)
            ////////        {
            ////////            var node_channel = treeListView1.FindNodesByText(node_station.Nodes, channel.Name);
            ////////            if (node_channel != null)
            ////////            {
            ////////                node_station.Nodes.Remove(node_channel);
            ////////                treeListView1.Invalidate();
            ////////            }
            ////////        }
            ////////    }
            ////////    ////var location_code = String.IsNullOrEmpty(channel.locationCode) ? "" : channel.locationCode;
            ////////    //var location = station.sensorLocation.Find(q => q.Name == channel.LocationCode);
            ////////    //if (location != null)
            ////////    //{
            ////////    //    if (location.Streams.Find(q => q.Name == channel.Name) != null)
            ////////    //    {
            ////////    //        location.Streams.Remove(channel);
            ////////    //    }
            ////////    //}
            ////////}
        }

        private void Btn_SaveNetwork_Click(object sender, EventArgs e)
        {
            parent.SaveInventory();
        }

        private void Btn_Export_Click(object sender, EventArgs e)
        {
            ////////if (parent.Inventory == null)
            ////////{
            ////////    return;
            ////////}
            ////////SaveFileDialog dialog = new SaveFileDialog();
            ////////dialog.Filter = "XML File (*.xml)|*.xml";
            ////////if (dialog.ShowDialog() == DialogResult.OK)
            ////////{
            ////////    File.Copy(parent.Inventory.XmlFilename, dialog.FileName);
            ////////}
        }

        private void Btn_Import_Click(object sender, EventArgs e)
        {
            frm_Setup_Inventory_AddFdsnXml f_Setup_NetworkStationImportFsdnXml = new frm_Setup_Inventory_AddFdsnXml(this.parent);
            f_Setup_NetworkStationImportFsdnXml.StartPosition = FormStartPosition.CenterParent;
            f_Setup_NetworkStationImportFsdnXml.Updated += F_Setup_NetworkStationImportFsdnXml_Updated;
            f_Setup_NetworkStationImportFsdnXml.Execute();
            //var fsdn_file = @"D:\GX\INSTRUMENT RESPONSES\FDSNStationXML CHANNEL RESPONSE AXEL # GE_NPW.xml";
            //OpenFileDialog dialog = new OpenFileDialog();
            //if (File.Exists(fsdn_file))
            //{                
            //    dialog.FileName = fsdn_file;
            //}
            //dialog.Multiselect = false;
            //dialog.Filter = "XML File (*.xml)|*.xml";
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    var Inventory = InventoryFunctions.ReadFsdnXml(dialog.FileName);
            //}
        }

        private void F_Setup_NetworkStationImportFsdnXml_Updated(object sender, EventArgs e)
        {
            PrintInventory(sender as CL_INVENTORY);
        }
    }
}