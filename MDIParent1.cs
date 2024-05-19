using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
//using lib_hypo71;
using lib_miniseed;
using lib_seedlink;
using lib_seismic;
using TriStateCheckBoxTreeView;
using lib_license;
using Inventory;
//using SeisWingSettings;
using log4net.Repository.Hierarchy;
using log4net.Layout.Pattern;

using SeisWingSettings;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Xml.Serialization.Configuration;

namespace SeisWing
{
    public partial class frm_parent : Form
    {
        private Boolean IsMouseDown = false;
        private Boolean pertamax = true;
        private Boolean StationNameFromData = true;
        private BW_LocalNodes[] msdBgWorkers;
        private BW_Watcher_Local bw_Watcher_Local;
        private BW_Watcher_Remote bw_Watcher_Remote;
        private Int32 childFormNumber = 0;
        private Int32 LastSentThread = 0;
        private Int32 MonitorIndex = -1;
        private Int32 MouseDistance = 0;
        private Int32 SL_BgWorker_Id = 0;
        private List<MiniSeedFileInfo> MSD_FileInfos = new List<MiniSeedFileInfo>();
        private List<String> FilesFeed_Files_Dirs = new List<String>();
        private List<String> FilesFeed_Files_Files = new List<String>();
        private List<String> LocalFeed_Files_Dirs = new List<String>();
        private List<String> LocalFeed_Files_Files = new List<String>();
        private List<String> this_paths = new List<String>();
        private List<WaveformStreamTag> streamIDs = new List<WaveformStreamTag>();
        private Point MousePosition0 = new Point();
        private Point MousePosition1 = new Point();
        private SL_CONNECTION SL_Connection;
        private StreamReader sr;
        private StreamWriter sw;
        private String FileName = String.Empty;
        private String NetworkName = String.Empty;
       // private String Network = String.Empty;
        private String ServerAddress = String.Empty;
        private String StationName = String.Empty;
        private String[] FilesFeed_Files = new string[] { };
        private TreeDisplayStyles TreeDisplayStyle = TreeDisplayStyles.NET_DATE_STA_CHA;
        private TreeNode Node_Files = new TreeNode();
        private TreeNode Node_Local = new TreeNode();
        private TreeNode Node_Remote = new TreeNode();
        public Boolean IsLicensed = false;
        public Boolean IsSeisWingServiceExists = false;
        public Int32 NumberOfProcessors = 0;
        public List<CL_INVENTORY> Inventories;
        //public List<CL_INVENTORY> Inventory;
        public List<MiniSeedFileInfo> MiniSeedFileInfos_FILES = new List<MiniSeedFileInfo>();
        public List<MiniSeedFileInfo> MiniSeedFileInfos_LOCAL = new List<MiniSeedFileInfo>();
        public List<SpectraClass> Spectras;
        public List<SW_MONITOR> SW_Monitors;
        public SeisWingSetting Settings;
        public ServiceController ServiceController_SeisWingProc;
        public ServiceController ServiceController_SeisWingServer;
        public String FileLicense;
        public String FileSettings;
        public String Folder_Datafeed_Files;
        public String Folder_Datafeed_Remote;

        public String FolderHypo71;
        public String FolderHypoDD;
        public String FolderHypos;

        public String FolderResult_Hypocenter;
        //public String FolderResult_Picks;
        //public String FolderResult_PicksCommon;
        //public String FolderResult_PicksRaw;
        //public String FolderResult_Events;

        public String FolderIndiArciving;
        public String FolderIndiProcessing;
        public String FolderLicense;
        public String FolderProcessing;
        public String FolderRepoInventory;
        public String FolderRepoVelmod;
        public String FolderSC;
        public String FolderSCLog;
        public String FolderSCShare;
        public String FolderSCShareTtt;
        public String FolderSeisWing;
        public String FolderSettings;
        public String FolderTemp;
        public String SW_Params_File;
        public String VelocityModelInUseFileName;
        public String VelocityModelInUseName;
        public String VelocityModelSettingFile;
        public SW_LICENSE licenseis = new SW_LICENSE();
        public TraceSaveOptions TraceSaveOption;
        public TriStateTreeView triStateCheckBoxTreeView1;
        public VelocityModel VelocityModelInUse;

        public String FolderTemp_OfflinePhasePicking;
        public String FolderTemp_OfflinePhasePicking_Traces;
        public String FolderTemp_OfflinePhasePicking_Commands;
        public String FolderTemp_OfflinePhasePicking_Progress;
        public String FolderTemp_OfflinePhasePicking_Inventory;
        public String FolderTemp_OfflinePhasePicking_RawPicks;
        public String FolderTemp_OfflinePhasePicking_CommonPicks;
        public String FolderTemp_OfflinePhasePicking_OutPicks;
        public String FolderTemp_OfflinePhasePicking_Events;
        public CL_INVENTORY Inventory { get; set; }

        public frm_parent()
        {
            InitializeComponent();

            Tsm_PeakValues.Visible = false;
            Tsm_SpectralValues.Visible = false;

            Inventories = new List<CL_INVENTORY>();
           // inventory = new CL_INVENTORY();

            // spectraProcessingDataGroup = new List<SpectraProcessingDataClass>();
            Spectras = new List<SpectraClass>();
            SW_Monitors = new List<SW_MONITOR>();

            triStateCheckBoxTreeView1 = new TriStateTreeView();
            triStateCheckBoxTreeView1.Name = "trv_Networks";
            triStateCheckBoxTreeView1.ImageIndex = 0;
            triStateCheckBoxTreeView1.ImageList = imageList1;
            triStateCheckBoxTreeView1.Dock = DockStyle.Fill;
            triStateCheckBoxTreeView1.NodeMouseClick += Trv_Networks_NodeMouseClick;
            triStateCheckBoxTreeView1.CheckBoxes = true;
            grb_Menu.Controls.Add(triStateCheckBoxTreeView1);

            Node_Files = new TreeNode();
            Node_Files.Name = "FILES";
            Node_Files.Tag = "FILES";
            Node_Files.Text = "FILES";
            Node_Files.ImageKey = "folderhejo_24.png";
            Node_Files.SelectedImageKey = Node_Files.ImageKey;
            triStateCheckBoxTreeView1.Nodes.Add(Node_Files);

            Node_Local = new TreeNode();
            Node_Local.Name = "LOCAL";
            Node_Local.Tag = "LOCAL";
            Node_Local.Text = "LOCAL";
            Node_Local.ImageKey = "computer_24.png";
            Node_Local.SelectedImageKey = Node_Local.ImageKey;
            triStateCheckBoxTreeView1.Nodes.Add(Node_Local);

            Node_Remote = new TreeNode();
            Node_Remote.Name = "REMOTE";
            Node_Remote.Tag = "REMOTE";
            Node_Remote.Text = "REMOTE";
            Node_Remote.ImageKey = "remote_24.png";
            Node_Remote.SelectedImageKey = Node_Remote.ImageKey;
            triStateCheckBoxTreeView1.Nodes.Add(Node_Remote);

            // ganti lokalilasi ka en-us heula
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            // neangan jumlah logical processor
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                NumberOfProcessors += Int32.Parse(item["NumberOfLogicalProcessors"].ToString());
            }
            if (NumberOfProcessors == 0)
            {
                NumberOfProcessors = 8;
            }

            FolderSeisWing = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SeisWing\\";
            if (!Directory.Exists(FolderSeisWing))
            {
                Directory.CreateDirectory(FolderSeisWing);
            }

            // SW_Params_File untuk dibaca ku si service menunjukkan di mana folder temp
            SW_Params_File = "C:\\SeisWing\\sw_params.par";
            if (!Directory.Exists(Path.GetDirectoryName(SW_Params_File)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SW_Params_File));
            }
            if (!File.Exists(SW_Params_File))
            {
                sw = new StreamWriter(SW_Params_File);
                sw.WriteLine(FolderSeisWing.TrimEnd('\\'));
                sw.Close();
            }

            FolderLicense = FolderSeisWing + "license\\";
            if (!Directory.Exists(FolderLicense))
            {
                Directory.CreateDirectory(FolderLicense);
            }
            FileLicense = FolderLicense + "\\license.txt";

            FolderIndiArciving = FolderSeisWing + "archiving\\";
            if (!Directory.Exists(FolderIndiArciving))
            {
                Directory.CreateDirectory(FolderIndiArciving);
            }

            FolderIndiProcessing = FolderSeisWing + "processing\\";
            if (!Directory.Exists(FolderIndiProcessing))
            {
                Directory.CreateDirectory(FolderIndiProcessing);
            }

            FolderProcessing = FolderSeisWing + "processing\\";
            if (!Directory.Exists(FolderProcessing))
            {
                Directory.CreateDirectory(FolderProcessing);
            }

            FolderRepoInventory = FolderSeisWing + "inventories\\";
            if (!Directory.Exists(FolderRepoInventory))
            {
                Directory.CreateDirectory(FolderRepoInventory);
            }

            FolderRepoVelmod = FolderSeisWing + "velmod\\";
            if (!Directory.Exists(FolderRepoVelmod))
            {
                Directory.CreateDirectory(FolderRepoVelmod);
            }

            // folder datafeed
            if (!Directory.Exists(FolderSeisWing + "datafeed"))
            {
                Directory.CreateDirectory(FolderSeisWing + "datafeed");
            }
            //data feed files
            Folder_Datafeed_Files = FolderSeisWing + "datafeed\\files\\";
            if (!Directory.Exists(Folder_Datafeed_Files))
            {
                Directory.CreateDirectory(Folder_Datafeed_Files);
            }
            // data feed remote
            Folder_Datafeed_Remote = FolderSeisWing + "datafeed\\remote\\";
            if (!Directory.Exists(Folder_Datafeed_Remote))
            {
                Directory.CreateDirectory(Folder_Datafeed_Remote);
            }

            // folder sc
            FolderSC = FolderSeisWing + "sc\\";
            if (!Directory.Exists(FolderSC))
            {
                Directory.CreateDirectory(FolderSC);
            }
            FolderSCLog = FolderSC + "log\\";
            if (!Directory.Exists(FolderSCLog))
            {
                Directory.CreateDirectory(FolderSCLog);
            }
            FolderSCShare = FolderSC + "share\\";
            if (!Directory.Exists(FolderSCShare))
            {
                Directory.CreateDirectory(FolderSCShare);
            }
            FolderSCShareTtt = FolderSCShare + "ttt\\";
            if (!Directory.Exists(FolderSCShareTtt))
            {
                Directory.CreateDirectory(FolderSCShareTtt);
            }

            // temporary folder
            FolderTemp = Path.GetTempPath() + "~SeisWing\\";
            if (!Directory.Exists(FolderTemp))
            {
                Directory.CreateDirectory(FolderTemp);
            }
            // temporary folder for offline_phase_picking
            FolderTemp_OfflinePhasePicking = FolderTemp + "offline_phase_picking\\";
            FolderTemp_OfflinePhasePicking_Traces = FolderTemp_OfflinePhasePicking + "traces\\";
            FolderTemp_OfflinePhasePicking_Commands = FolderTemp_OfflinePhasePicking + "commands\\";
            FolderTemp_OfflinePhasePicking_Progress = FolderTemp_OfflinePhasePicking + "progress\\";
            FolderTemp_OfflinePhasePicking_Inventory = FolderTemp_OfflinePhasePicking + "inv\\";
            FolderTemp_OfflinePhasePicking_RawPicks = FolderTemp_OfflinePhasePicking + "raw_picks\\";
            FolderTemp_OfflinePhasePicking_CommonPicks = FolderTemp_OfflinePhasePicking + "com_picks\\";
            FolderTemp_OfflinePhasePicking_OutPicks = FolderTemp_OfflinePhasePicking + "out_picks\\";
            FolderTemp_OfflinePhasePicking_Events = FolderTemp_OfflinePhasePicking + "events\\";

            // settings keur processing and result
            FolderSettings = FolderSeisWing + "settings\\";
            if (!Directory.Exists(FolderSettings))
            {
                Directory.CreateDirectory(FolderSettings);
            }
            FileSettings = FolderSettings + "settings.sws";
            if (File.Exists(FileSettings))
            {
                Settings = new SeisWingSetting();
                Settings.FileSettings = FileSettings;
                Settings.FileSettingsRd();
            }
            else
            {
                Settings = new SeisWingSetting();
                Settings.FileSettings = FileSettings;
                Settings.FileSettingsWr0();
            }

            // folder processing result hypocenter
            //FolderResult_Hypocenter = Settings.ProcessingResultFolder.TrimEnd('\\') + "\\Hypo\\";
            //FolderResult_Picks = FolderResult_Hypocenter + "phase_picking\\";
            //FolderResult_PicksCommon = FolderResult_Picks + "common\\";
            //FolderResult_PicksRaw = FolderResult_Picks + "raw\\";
            //FolderResult_Events = FolderResult_Hypocenter + "events\\";

            //////TraceSaveOption = new TraceSaveOptions();
            //////TraceSaveOption.Save_Location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //////TraceSaveOption.Save_LengthInterval = 2;

            //load inventory
            LoadInventory();

            // update node files jeung lokal jeung remote
            UpdateNodes_Files();

            // masukin datafeed remote nu geus aya
            //UpdateNodes_Remote();

            //// remote datafeed watcher
            //StartWatchFilesDatafeedRemote();
        }


        #region form_functions
        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }
        #endregion form_functions

        private Boolean CheckLicense()
        {
            Boolean isLicensed = false;
            if (!File.Exists(FileLicense))
            {
                Frm_License f_license = new Frm_License(this);
                f_license.ShowDialog();
            }
            else
            {
                isLicensed = licenseis.CheckLicense(FileLicense);
            }
            // lamun 1
            Tsm_PeakValues.Visible = licenseis.ValidFor(Enum_ProcessingTypes.PeakValues);
            //toolStripMenuItem5.Visible = licenseis.ValidFor(Enum_ProcessingTypes.PeakValues);
            // lamun 3
            Tsm_SpectralValues.Visible = licenseis.ValidFor(Enum_ProcessingTypes.Spectal);
            //compareSpectraToolStripMenuItem.Visible = licenseis.ValidFor(Enum_ProcessingTypes.Spectal);
            // lamun 2
            automaticEventDetectionToolStripMenuItem.Visible = licenseis.ValidFor(Enum_ProcessingTypes.Hypocenter);
            phasePickingToolStripMenuItem.Visible = licenseis.ValidFor(Enum_ProcessingTypes.Hypocenter);
            eventLocationToolStripMenuItem.Visible = licenseis.ValidFor(Enum_ProcessingTypes.Hypocenter);
            magnitudeCalculationToolStripMenuItem.Visible = licenseis.ValidFor(Enum_ProcessingTypes.Hypocenter);
            return isLicensed;
        }

        private Int32 InstallService(String service_name)
        {
            int res = 0;

            var bat = FolderTemp + "exec.bat";
            sw = new StreamWriter(bat);
            sw.WriteLine("InstallUtil.exe \"" + service_name + ".exe\" > \"" + FolderTemp + "exec.txt\"");
            sw.Close();

            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = " /c " + bat;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            // ayeuna baca hasilna
            var text = File.ReadAllText(FolderTemp + "exec.txt");
            if (text.Contains("failed") || text.Contains("FileNotFoundException"))
            {
                res = 1;
            }

            return res;
        }

        private void frm_parent_Load(object sender, EventArgs e)
        {
            // cek heula lisensi
            IsLicensed = CheckLicense();

            Int32 service_install_error = 0;

            // check if SeisWingServer exists
            ServiceController_SeisWingServer = ServiceController.GetServices().ToList().Find(q => q.ServiceName == "SeisWingServer");

            if (ServiceController_SeisWingServer == null)
            {
                // install service
                service_install_error += InstallService("SeisWingServer");
                ServiceController_SeisWingServer = new ServiceController("SeisWingServer");
            }
            if (service_install_error == 0)
            {

                var status_SeisWingServer = ServiceController_SeisWingServer.Status;
                if ((status_SeisWingServer.Equals(ServiceControllerStatus.Stopped)) || (status_SeisWingServer.Equals(ServiceControllerStatus.StopPending)))
                {
                    try
                    {
                        ServiceController_SeisWingServer.Start();
                    }
                    catch
                    {

                    }
                }
            }

            // check if ServiceController_SeisWingProc exists
            if (licenseis.ValidFor(Enum_ProcessingTypes.PeakValues))
            {
                ServiceController_SeisWingProc = ServiceController.GetServices().ToList().Find(q => q.ServiceName == "SeisWingProcPeaks");
                if (ServiceController_SeisWingProc == null)
                {
                    // install service
                    service_install_error += InstallService("SeisWingProcPeaks");
                    ServiceController_SeisWingProc = new ServiceController("SeisWingProcPeaks");
                }
                if (service_install_error == 0)
                {
                    var status_SeisWingProc = ServiceController_SeisWingProc.Status;
                    if ((status_SeisWingProc.Equals(ServiceControllerStatus.Stopped)) || (status_SeisWingProc.Equals(ServiceControllerStatus.StopPending)))
                    {
                        try
                        {
                            ServiceController_SeisWingProc.Start();
                        }
                        catch
                        {

                        }
                    }
                }
            }

            if (service_install_error > 0)
            {
                MessageBox.Show("Please close and restart as Administrator.");
            }

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_parent_FormClosing);

            ////// load heula data spectra
            //string filename = FolderRepoInventory + "spectral.txt";

            //if (File.Exists(filename))
            //{
            //    var sr = new StreamReader(filename);
            //    sr.ReadLine();
            //    while (!sr.EndOfStream)
            //    {
            //        var lines = sr.ReadLine().Split('\t');
            //        var spectra_class = new SpectraClass();
            //        spectra_class.StationName = lines[0];
            //        spectra_class.Easting = Convert.ToDouble(lines[1]);
            //        spectra_class.Northing = Convert.ToDouble(lines[2]);
            //        spectra_class.VH_1_4 = Convert.ToDouble(lines[3]);
            //        spectra_class.VH_4_10 = Convert.ToDouble(lines[4]);
            //        Spectras.Add(spectra_class);
            //    }
            //    sr.Close();
            //}

            //toolStripButton5_Click(null, null);

            //toolStripButton4_Click(null, null);

            //frm_ViewHeaders f_ViewHeaders = new frm_ViewHeaders(this);
            //f_ViewHeaders.MdiParent = this;
            //f_ViewHeaders.WindowState = FormWindowState.Maximized;
            //f_ViewHeaders.msd_filename = @"D:\SeisWing\olahdata\mseed\eq_banda5_6.mseed";
            //f_ViewHeaders.Show();

            //frm_ViewPicks form = new frm_ViewPicks(this);
            //form.MdiParent = this;
            //form.WindowState = FormWindowState.Maximized;
            //form.Show();

            //frm_Setup f_setup = new frm_Setup(this);
            //f_setup.MdiParent = this;
            //f_setup.WindowState = FormWindowState.Maximized;
            //f_setup.Show();

            //frm_ViewEvents f_viewEvents = new frm_ViewEvents(this);
            //f_viewEvents.MdiParent = this;
            //f_viewEvents.WindowState = FormWindowState.Maximized;
            //f_viewEvents.Show();

            //frm_ViewEvents_Map f_eventMap = new frm_ViewEvents_Map(this);
            //f_eventMap.MdiParent = this;
            //f_eventMap.WindowState = FormWindowState.Maximized;
            //f_eventMap.Show();

            //frm_EventLocation f_eventLocation = new frm_EventLocation(this);
            //f_eventLocation.MdiParent = this;
            //f_eventLocation.WindowState = FormWindowState.Maximized;
            //f_eventLocation.Show();

            //frm_Setup f_setup = new frm_Setup(this);
            //f_setup.MdiParent = this;
            //f_setup.WindowState = FormWindowState.Maximized;
            //f_setup.Show();

            //plotEventsOnMapToolStripMenuItem_Click(null, null);

            ////local watcher
            //bw_Watcher_Local = new BW_Watcher_Local();
            //bw_Watcher_Local.ArchivingFolder = Settings.ArchivingFolder;
            //bw_Watcher_Local.ProgressChanged += Bw_Watcher_Local_ProgressChanged;
            //bw_Watcher_Local.RunWorkerCompleted += Bw_Watcher_Local_RunWorkerCompleted;
            //bw_Watcher_Local.RunWorkerAsync();

            //// remote watcher
            //bw_Watcher_Remote = new BW_Watcher_Remote();
            //bw_Watcher_Remote.Folder_Datafeed_Remote = Folder_Datafeed_Remote;
            //bw_Watcher_Remote.ProgressChanged += Bw_Watcher_Remote_ProgressChanged;
            //bw_Watcher_Remote.RunWorkerAsync();
        }

        private void Splitter1_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
            MousePosition0 = e.Location;
        }

        private void Splitter1_MouseMove(object sender, MouseEventArgs e)
        {
            MousePosition1 = e.Location;
            MouseDistance = MousePosition1.X - MousePosition0.X;
            if (IsMouseDown)
            {
                if (this.Pnl_LeftList.Width < 150 && MouseDistance < 0)
                {
                    return;
                }
                this.Pnl_LeftList.Width += (MouseDistance);
            }
        }

        private void Splitter1_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        private void ToolStripStatusLabel1_TextChanged(object sender, EventArgs e)
        {
            var textwidth = TextRenderer.MeasureText(toolStripStatusLabel1.Text, toolStripStatusLabel1.Font).Width;
            if (textwidth >= statusStrip.ClientSize.Width)
            {
                toolStripStatusLabel1.Text = "";
            }
        }

        private void Btn_AddNetwork_Click(object sender, EventArgs e)
        {
            Menu_item_AddRemoteDataFeed_Click(null, null);
        }

        private void Btn_CheckNetwork_Click(object sender, EventArgs e)
        {
            frm_Dialog_CheckServer form = new frm_Dialog_CheckServer(this);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void Trv_Networks_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ContextMenuStrip submenu1 = new ContextMenuStrip();
            ToolStripMenuItem menu_item = new ToolStripMenuItem();
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                switch (e.Node.Name)
                {
                    #region FILES
                    case "FILES":
                        #region FILES
                        menu_item = new ToolStripMenuItem();
                        menu_item.Name = e.Node.Name;
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Add Folders or Files";
                        menu_item.Image = global::SeisWing.Properties.Resources.folderhejo_add_24;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Remove Folder(s)";
                        menu_item.Image = global::SeisWing.Properties.Resources.edit_delete;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        //menu.Items.Add("-");
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Refresh";
                        //menu_item.Image = global::SeisWing.Properties.Resources.refresh;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);
                        menu.Items.Add("-");
                        submenu1 = new ContextMenuStrip();
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Style 1";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Style 2";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Style 3";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Style 4";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Change Tree Style";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.DropDown = submenu1;
                        menu.Items.Add(menu_item);
                        #endregion FILES
                        break;

                    case "FILES_DIRECTORY":
                        #region FILES_DIRECTORY
                        menu_item = new ToolStripMenuItem();
                        menu_item.Name = e.Node.Name;
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Expand Folder";
                        menu_item.Image = global::SeisWing.Properties.Resources.folder_expand_24;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Name = e.Node.Name;
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Expand Folder and Content";
                        menu_item.Image = global::SeisWing.Properties.Resources.folder_expand_24;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Name = e.Node.Name;
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Collapse Folder";
                        menu_item.Image = global::SeisWing.Properties.Resources.folder_collapse_24;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu.Items.Add("-");
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Refresh";
                        //menu_item.Image = global::SeisWing.Properties.Resources.refresh;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);
                        //menu.Items.Add("-");
                        submenu1 = new ContextMenuStrip();
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Spectral Processing";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        submenu1 = new ContextMenuStrip();
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Hypocenter Location";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Process";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.DropDown = submenu1;
                        menu.Items.Add(menu_item);
                        menu.Items.Add("-");
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Remove Folder(s)";
                        menu_item.Image = global::SeisWing.Properties.Resources.edit_delete;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        #endregion FILES_DIRECTORY
                        break;

                    case "FILES_SUB":
                        #region FILES_SUB
                        if (e.Node.Level == 1)
                        {
                            menu_item = new ToolStripMenuItem();
                            menu_item.Name = e.Node.Name;
                            menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                            menu_item.Text = "Expand All Folder";
                            menu_item.Image = global::SeisWing.Properties.Resources.folder_expand_24;
                            menu_item.Click += this.ProcessTreeNode;
                            menu.Items.Add(menu_item);
                            menu_item = new ToolStripMenuItem();
                            menu_item.Name = e.Node.Name;
                            menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                            menu_item.Text = "Collapse All Folder";
                            menu_item.Image = global::SeisWing.Properties.Resources.folder_collapse_24;
                            menu_item.Click += this.ProcessTreeNode;
                            menu.Items.Add(menu_item);
                            menu.Items.Add("-");
                            menu_item = new ToolStripMenuItem();
                            menu_item.Name = e.Node.Name;
                            menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                            menu_item.Text = "Make Custom Selections";
                            menu_item.Image = global::SeisWing.Properties.Resources.selection_24;
                            menu_item.Click += this.ProcessTreeNode;
                            menu.Items.Add(menu_item);
                            menu.Items.Add("-");
                            menu_item = new ToolStripMenuItem();
                            menu_item.Name = e.Node.Name;
                            menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                            menu_item.Text = "Remove this Folder";
                            menu_item.Image = global::SeisWing.Properties.Resources.folderhejo_min_24;
                            menu_item.Click += this.ProcessTreeNode;
                            menu.Items.Add(menu_item);
                        }
                        #endregion FILES_SUB
                        break;

                    case "FILES_SUBDIR":
                        #region FILES_SUBDIR
                        menu_item = new ToolStripMenuItem();
                        menu_item.Name = e.Node.Name;
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Expand All Folder";
                        menu_item.Image = global::SeisWing.Properties.Resources.folder_expand_24;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Name = e.Node.Name;
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Collapse All Folder";
                        menu_item.Image = global::SeisWing.Properties.Resources.folder_collapse_24;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu.Items.Add("-");
                        submenu1 = new ContextMenuStrip();
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Display Helicorder";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.OpenEditor;
                        //submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "All Files";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.DropDown = submenu1;
                        menu.Items.Add(menu_item);
                        submenu1 = new ContextMenuStrip();
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "selected" };
                        //menu_item.Text = "Display Helicorder";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.OpenEditor;
                        //submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Selected Files";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.DropDown = submenu1;
                        menu.Items.Add(menu_item);
                        #endregion FILES_SUBDIR
                        break;

                    case "FILES_STATION":
                        #region FILES_STATION
                        menu_item = new ToolStripMenuItem();
                        menu_item.Name = e.Node.Name;
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Expand Station";
                        menu_item.Image = global::SeisWing.Properties.Resources.folder_expand_24;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Name = e.Node.Name;
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Collapse Station";
                        menu_item.Image = global::SeisWing.Properties.Resources.folder_collapse_24;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu.Items.Add("-");
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Select and Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        #endregion FILES_STATION
                        break;

                    case "FILES_DATE":
                        #region FILES_DATE
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Name = e.Node.Name;
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //menu_item.Text = "Expand Station";
                        //menu_item.Image = global::SeisWing.Properties.Resources.folder_expand_24;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Name = e.Node.Name;
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //menu_item.Text = "Collapse Station";
                        //menu_item.Image = global::SeisWing.Properties.Resources.folder_collapse_24;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);
                        //menu.Items.Add("-");
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Select and Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        #endregion FILES_DATE
                        break;

                    case "FILES_FILE":
                        #region FILES_FILE
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Select and Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        #endregion FILES_FILE
                        break;
                    #endregion FILES

                    #region LOCAL
                    case "LOCAL":
                        #region LOCAL
                        submenu1 = new ContextMenuStrip();
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Style 1";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Style 2";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Style 3";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Style 4";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        submenu1.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Change Tree Style";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.DropDown = submenu1;
                        menu.Items.Add(menu_item);
                        #endregion LOCAL
                        break;

                    case "LOCAL_NETWORK":
                        #region LOCAL_NETWORK
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Refresh";
                        //menu_item.Image = global::SeisWing.Properties.Resources.refresh;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);

                        ////menu_item = new ToolStripMenuItem();
                        ////menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        ////menu_item.Text = "Display Headers";
                        ////menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        ////menu_item.Click += this.OpenEditor;
                        ////submenu1.Items.Add(menu_item);
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //menu_item.Text = "All Files";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.DropDown = submenu1;
                        ////menu.Items.Add(menu_item);

                        //submenu1 = new ContextMenuStrip();
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Spectral Processing";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.ProcessTreeNode;
                        //submenu1.Items.Add(menu_item);
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //menu_item.Text = "Process";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.DropDown = submenu1;
                        ////menu.Items.Add(menu_item);
                        #endregion LOCAL_NETWORK
                        break;

                    case "LOCAL_DATE":
                        #region LOCAL_DATE
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Select and Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Display Headers";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);
                        menu.Items.Add("-");
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Remove this Dataset";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        #endregion LOCAL_DATE
                        break;

                    case "LOCAL_STATION":
                        #region LOCAL_STATION
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Open Helicorder";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);

                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Select and Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);

                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Display Headers";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);
                        #endregion LOCAL_STATION
                        break;

                    case "LOCAL_CHANNEL":
                        #region LOCAL_CHANNEL
                        //submenu1 = new ContextMenuStrip();
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Select and Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Display Headers";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.OpenEditor;
                        //menu.Items.Add(menu_item);
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //menu_item.Text = "All Files";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.DropDown = submenu1;
                        //menu.Items.Add(menu_item);
                        #endregion LOCAL_CHANNEL
                        break;

                    case "LOCAL_FILE":
                        #region LOCAL_FILE
                        //submenu1 = new ContextMenuStrip();
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu.Items.Add("-");
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Select and Display Waveforms";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        menu_item.Text = "Properties";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name, IsSubmenu = true, Attribute = "all" };
                        //menu_item.Text = "Display Headers";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.OpenEditor;
                        //menu.Items.Add(menu_item);
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //menu_item.Text = "All Files";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.DropDown = submenu1;
                        //menu.Items.Add(menu_item);
                        #endregion LOCAL_FILE
                        break;
                    #endregion LOCAL

                    #region REMOTE
                    case "REMOTE":
                        #region REMOTE
                        menu_item = new ToolStripMenuItem();
                        menu_item.Name = e.Node.Name;
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Add Remote Data Feed";
                        menu_item.Image = global::SeisWing.Properties.Resources.folderhejo_add_24;
                        menu_item.Click += Menu_item_AddRemoteDataFeed_Click;
                        menu.Items.Add(menu_item);
                        #endregion REMOTE
                        break;
                    case "REMOTE_SERVER":
                        #region REMOTE_SERVER
                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //menu_item.Text = "Connect to Server";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);

                        //menu_item = new ToolStripMenuItem();
                        //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //menu_item.Text = "Disconnect from Server";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //menu_item.Click += this.ProcessTreeNode;
                        //menu.Items.Add(menu_item);

                        //menu.Items.Add("-");

                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Remove this Server";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);

                        break;
                    #endregion REMOTE_SERVER
                    case "REMOTE_NETWORK":
                        #region REMOTE_NETWORK
                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Remove this Network";
                        menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);

                        break;
                    #endregion REMOTE_NETWORK
                    case "REMOTE_STATION":

                        if (e.Node.ImageKey.Contains("stop"))
                        {
                            //menu_item = new ToolStripMenuItem();
                            //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                            //menu_item.Text = "Connect to this Station";
                            //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                            //menu_item.Click += this.ProcessTreeNode;
                            //menu.Items.Add(menu_item);
                            ////menu.Items.Add("-");
                        }
                        else if (e.Node.ImageKey.Contains("ok"))
                        {
                            //menu_item = new ToolStripMenuItem();
                            //menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                            //menu_item.Text = "Disconnect this Station";
                            //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                            //menu_item.Click += this.ProcessTreeNode;
                            //menu.Items.Add(menu_item);
                            ////menu.Items.Add("-");
                        }

                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Open Wave Viewer";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu.Items.Add("-");

                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Remove this Station";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);
                        menu.Items.Add("-");

                        //if (e.Node.ImageKey.Contains("ok"))
                        //{
                        //    menu_item = new ToolStripMenuItem();
                        //    menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //    menu_item.Text = "Start Archiving";
                        //    menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //    menu_item.Click += this.ProcessTreeNode;
                        //    menu.Items.Add(menu_item);

                        //    menu.Items.Add("-");
                        //}
                        //else if (e.Node.ImageKey.Contains("archiving"))
                        //{
                        //    menu_item = new ToolStripMenuItem();
                        //    menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        //    menu_item.Text = "Stop Archiving";
                        //    menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        //    menu_item.Click += this.ProcessTreeNode;
                        //    menu.Items.Add(menu_item);

                        //    menu.Items.Add("-");
                        //}

                        menu_item = new ToolStripMenuItem();
                        menu_item.Tag = new TreeMenuTag() { Node = e.Node, Name = e.Node.Name };
                        menu_item.Text = "Properties";
                        //menu_item.Image = global::SeisWing.Properties.Resources.go_next_view_22;
                        menu_item.Click += this.ProcessTreeNode;
                        menu.Items.Add(menu_item);

                        break;
                        #endregion REMOTE
                }
                menu.Show(this.triStateCheckBoxTreeView1, new Point((Int32)e.X, (Int32)e.Y));
                triStateCheckBoxTreeView1.SelectedNode = e.Node;
            }
        }

        private void ProcessTreeNode(Object sender, EventArgs e)
        {
            var menu_tags = new TreeMenuTag();
            var menu_text = String.Empty;
            var node_name = String.Empty;
            var node_node = new TreeNode();
            var node_subs = false;

            frm_WaveViewer_Remote f_waveviewer = null;
            frm_RemoteStation_Properties f_property = null;
            frm_WaveProcessing f_process = null;
            frm_WaveStatus f_status = null;
            SW_MONITOR monitor = null;

            if (sender is ToolStripMenuItem)
            {
                var menu = (sender as ToolStripMenuItem);
                menu_text = menu.Text;
                menu_tags = menu.Tag as TreeMenuTag;
                node_name = menu_tags.Name;
                node_node = menu_tags.Node;
                node_subs = menu_tags.IsSubmenu;
            }
            else if (sender is TreeView)
            {
                node_node = (e as TreeNodeMouseClickEventArgs).Node;
                node_name = node_node.Name;
            }

            List<string> SelectedFiles = new List<string>();

            TreeNode fnode;
            string data_path;

            switch (node_name)
            {
                #region FILES
                case "FILES":
                    if (menu_text == "Add Folders or Files")
                    {
                        var mydir = @"D:\WW\DATA\20190523";
                        mydir = @"D:\WW\DATA\09. September 2020";
                        mydir = @"D:\SARA_LINK\DATA";
                        //FolderBrowserDialog dialog = new FolderBrowserDialog();
                        //if (Directory.Exists(mydir))
                        //{
                        //    dialog.SelectedPath = mydir;
                        //}
                        //if (dialog.ShowDialog() == DialogResult.OK)
                        //{
                        //    AddDataFeed_LOCAL addLocalDataFeed = new AddDataFeed_LOCAL();
                        //    addLocalDataFeed.NumberOfProcessors = NumberOfProcessors;
                        //    addLocalDataFeed.Datafeed_Local_Dir = Folder_Datafeed_Local;
                        //    addLocalDataFeed.ProcessStarted += AddLocalDataFeed_ProcessStarted;
                        //    addLocalDataFeed.ProcessChanged += AddLocalDataFeed_ProcessChanged;
                        //    addLocalDataFeed.ProcessCompleted += AddLocalDataFeed_ProcessCompleted;
                        //    addLocalDataFeed.Populate_Local_Feed(dialog.SelectedPath);
                        //}
                        frm_Dialog_AddDataFeed_Files dialog = new frm_Dialog_AddDataFeed_Files(this);
                        if (Directory.Exists(mydir))
                        {
                            dialog.SelectedPath = mydir;
                        }
                        dialog.StartPosition = FormStartPosition.CenterParent;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            // ayeuna cari heula di folder ieu aya data naon wae
                            if (dialog.FolderIsSelected)
                            {
                                AddDataFeed_FILES addDataFeed_FILES = new AddDataFeed_FILES();
                                addDataFeed_FILES.NumberOfProcessors = NumberOfProcessors;
                                addDataFeed_FILES.Datafeed_Files_Dir = Folder_Datafeed_Files;
                                addDataFeed_FILES.ProcessStarted += AddDataFeed_FILES_ProcessStarted;
                                addDataFeed_FILES.ProcessChanged += AddDataFeed_FILES_ProcessChanged;
                                addDataFeed_FILES.ProcessCompleted += AddDataFeed_FILES_ProcessCompleted;
                                addDataFeed_FILES.StationNameFromData = dialog.StationNameFromData;
                                addDataFeed_FILES.UserDefinedNetworkName = dialog.UserDefinedNetworkName;
                                addDataFeed_FILES.Populate_Files_Feed(dialog.SelectedPath);
                            }
                        }
                    }
                    else if (menu_text == "Remove Folder(s)")
                    {
                        int npcnode = Node_Files.Nodes.Count;
                        for (int i = npcnode - 1; i >= 0; i--)
                        {
                            if (Node_Files.Nodes[i].Checked)
                            {
                                // hupus file-datafeedna
                                var datafeed_file = Folder_Datafeed_Files + Node_Files.Nodes[i].Text.Replace("\\", "@_@").Replace(":", "_@_") + ".xml";
                                if (File.Exists(datafeed_file))
                                {
                                    File.Delete(datafeed_file);
                                }
                                // ayeuna hupus tree-na
                                Node_Files.Nodes.RemoveAt(i);
                            }
                        }
                        triStateCheckBoxTreeView1.Invalidate();
                    }
                    else if (menu_text == "Refresh")
                    {

                    }
                    else if (menu_text == "Style 1")
                    {
                        UpdateNodes(1);
                    }
                    else if (menu_text == "Style 2")
                    {
                        UpdateNodes(2);
                    }
                    else if (menu_text == "Style 3")
                    {
                        UpdateNodes(3);
                    }
                    else if (menu_text == "Style 4")
                    {
                        UpdateNodes(4);
                    }

                    break;

                case "FILES_DIRECTORY":
                    #region FILES_DIRECTORY
                    if (menu_text == "Expand Folder")
                    {
                        node_node.Collapse();
                        node_node.Expand();
                    }
                    if (menu_text == "Expand Folder and Content")
                    {
                        node_node.ExpandAll();
                    }
                    else if (menu_text == "Collapse Folder")
                    {
                        node_node.Collapse();
                    }
                    else if (menu_text == "Save")
                    {
                        var xml_file = Folder_Datafeed_Files + node_node.Text.Replace(":", "_@_").Replace("\\", "@_@") + ".xml";
                        if (File.Exists(xml_file))
                        {
                            var tree = XmlFunctions.Read_TreeNodeFromXml_FILES(xml_file);
                            XmlFunctions.Write_TreeNodeToXml_FILES(xml_file, tree);
                        }
                    }
                    else if (menu_text == "Spectral Processing")
                    {
                        List<String> selectedFiles = new List<string>();
                        GetFilesInsideNode(node_node, true, ref selectedFiles);
                        if (selectedFiles.Count == 0)
                        {
                            GetFilesInsideNode(node_node, false, ref selectedFiles);
                        }

                        streamIDs = new List<WaveformStreamTag>();
                        //foreach (TreeNode node_date in node_node.Nodes)
                        //{
                        foreach (TreeNode node_station in node_node.Nodes)
                        {
                            foreach (TreeNode node_channel in node_station.Nodes)
                            {
                                streamIDs.Add(new WaveformStreamTag() { NetworkName = node_node.Text, StationName = node_station.Text, ChannelName = node_channel.Text });
                            }
                        }
                        //}

                        // stationnamefromdata
                        var tag = node_node.Tag;
                        string manualNetworkName = String.Empty;
                        Boolean stationNameFromData = true;
                        if (tag is object[])
                        {
                            manualNetworkName = (tag as object[])[0].ToString();
                            stationNameFromData = Convert.ToBoolean((tag as object[])[1]);
                        }

                        //frm_Process_Spectra_Home f_Process_Spectra_Home = new frm_Process_Spectra_Home(this);
                        //f_Process_Spectra_Home.StartPosition = FormStartPosition.CenterParent;
                        //f_Process_Spectra_Home.streamIDs = streamIDs;
                        //f_Process_Spectra_Home.selectedFiles = selectedFiles;
                        //f_Process_Spectra_Home.ManualNetworkName = manualNetworkName;
                        //f_Process_Spectra_Home.StationNameFromData = stationNameFromData;
                        //f_Process_Spectra_Home.ShowDialog();
                    }
                    else if (menu_text == "Hypocenter Location")
                    {
                        //pol
                    }
                    else if (menu_text == "Remove Folder(s)")
                    {
                        var tag = node_node.Tag as object[];
                        if (tag.Length > 2)
                        {
                            if (tag[2] != null)
                            {
                                if (File.Exists(tag[2].ToString()))
                                {
                                    File.Delete(tag[2].ToString());
                                }
                            }
                        }
                        Node_Files.Nodes.Remove(node_node);
                        triStateCheckBoxTreeView1.Invalidate();
                    }
                    #endregion FILES_DIRECTORY
                    break;

                case "FILES_SUB":
                    #region FILES_SUB
                    if (menu_text == "Expand All Folder")
                    {
                        node_node.ExpandAll();
                    }
                    else if (menu_text == "Collapse All Folder")
                    {
                        node_node.Collapse();
                    }
                    else if (menu_text == "Make Custom Selections")
                    {
                        frm_Dialog_LocalFilesCustomSelections f_Dialog_LocalFilesCustomSelections = new frm_Dialog_LocalFilesCustomSelections(this);
                        f_Dialog_LocalFilesCustomSelections.SelectedNode = node_node;
                        f_Dialog_LocalFilesCustomSelections.ShowDialog();
                    }
                    else if (menu_text == "Remove this Folder")
                    {
                        sr = new StreamReader(Folder_Datafeed_Files);
                        var eusi_file = sr.ReadToEnd();
                        sr.Close();
                        if (eusi_file.Contains(node_node.Text))
                        {
                            eusi_file = eusi_file.Replace(node_node.Text + "\r\n", "");
                        }
                        sw = new StreamWriter(Folder_Datafeed_Files, false);
                        sw.Write(eusi_file);
                        sw.Close();
                        //UpdateNodes_Local();
                        node_node.Parent.Nodes.Remove(node_node);
                    }
                    #endregion FILES_SUB
                    break;

                case "FILES_SUBDIR":
                    #region FILES_SUBDIR
                    this_paths = new List<string>();
                    this_paths.Add(node_node.Text);
                    fnode = node_node;
                    while (true)
                    {
                        var parent = fnode.Parent;
                        if (parent == null)
                        {
                            break;
                        }
                        if (parent.Text == "Local")
                        {
                            break;
                        }
                        this_paths.Add(parent.Text);
                        fnode = parent;//.Parent;
                    }
                    this_paths.Reverse();
                    data_path = String.Join("\\", this_paths);
                    if (node_subs)
                    {

                    }
                    if (menu_text == "Expand All Folder")
                    {
                        node_node.ExpandAll();
                    }
                    if (menu_text == "Collapse All Folder")
                    {
                        node_node.Collapse();
                    }
                    //else if (node_text == "Display Helicorder")
                    //{
                    //    frm_Helicorder f_helicorder = new frm_Helicorder(this);
                    //    f_helicorder.MdiParent = this;
                    //    f_helicorder.WindowState = FormWindowState.Maximized;
                    //    f_helicorder.DataDirectory = data_path;
                    //    f_helicorder.Show();
                    //}
                    #endregion FILES_SUBDIR
                    break;

                case "FILES_STATION":
                    #region FILES_STATION
                    data_path = node_node.Parent.Text + "\\" + node_node.Text;

                    if (node_subs)
                    {
                        SelectedFiles = GetSelectedFiles(menu_tags.Attribute, data_path, node_node);
                    }

                    if (menu_text == "Expand Station")
                    {
                        node_node.ExpandAll();
                    }
                    else if (menu_text == "Collapse Station")
                    {
                        node_node.Collapse();
                    }
                    else if (menu_text == "Display Waveforms")
                    {
                        FromTreeMenu_DisplayWaveforms(node_node);
                    }
                    else if (menu_text == "Display Headerss")
                    {
                        frm_ViewHeaders f_ViewHeaders = new frm_ViewHeaders(this);
                        f_ViewHeaders.MdiParent = this;
                        f_ViewHeaders.WindowState = FormWindowState.Maximized;
                        f_ViewHeaders.SelectedFiles = SelectedFiles;
                        f_ViewHeaders.Show();
                    }
                    #endregion FILES_STATION
                    break;

                case "FILES_DATE":
                    #region FILES_DATE
                    data_path = node_node.Parent.Text + "\\" + node_node.Text;

                    if (node_subs)
                    {
                        SelectedFiles = GetSelectedFiles(menu_tags.Attribute, data_path, node_node);
                    }

                    if (menu_text == "Expand Station")
                    {
                        node_node.ExpandAll();
                    }
                    else if (menu_text == "Collapse Station")
                    {
                        node_node.Collapse();
                    }
                    else if (menu_text == "Display Waveforms")
                    {
                        FromTreeMenu_DisplayWaveforms(node_node);
                    }
                    else if (menu_text == "Display Headerss")
                    {
                        frm_ViewHeaders f_ViewHeaders = new frm_ViewHeaders(this);
                        f_ViewHeaders.MdiParent = this;
                        f_ViewHeaders.WindowState = FormWindowState.Maximized;
                        f_ViewHeaders.SelectedFiles = SelectedFiles;
                        f_ViewHeaders.Show();
                    }
                    #endregion FILES_DATE
                    break;

                case "FILES_FILE":
                    #region FILES_FILE
                    if (menu_text == "")
                    {

                    }
                    else if (menu_text == "Display Waveforms")
                    {

                    }
                    #endregion FILES_FILE
                    break;
                #endregion FILES

                #region LOCAL
                case "LOCAL":
                    if (menu_text == "Add Local Data Feed")
                    {

                    }
                    else if (menu_text == "Tree Display Style")
                    {
                        frm_Dialog_TreeDisplayStyle f_Dialog_TreeDisplayStyle = new frm_Dialog_TreeDisplayStyle(this);
                        f_Dialog_TreeDisplayStyle.StartPosition = FormStartPosition.CenterParent;
                        f_Dialog_TreeDisplayStyle.ShowDialog();
                    }
                    else if (menu_text == "Style 1")
                    {
                        UpdateNodes(1);
                    }
                    else if (menu_text == "Style 2")
                    {
                        UpdateNodes(2);
                    }
                    else if (menu_text == "Style 3")
                    {
                        UpdateNodes(3);
                    }
                    else if (menu_text == "Style 4")
                    {
                        UpdateNodes(4);
                    }
                    break;

                case "LOCAL_NETWORK":
                    #region LOCAL_NETWORK
                    if (menu_text == "Refresh")
                    {

                    }
                    else if (menu_text == "Spectral Processing")
                    {
                        List<String> selectedFiles = new List<string>();
                        GetFilesInsideNode(node_node, true, ref selectedFiles);
                        if (selectedFiles.Count == 0)
                        {
                            GetFilesInsideNode(node_node, false, ref selectedFiles);
                        }

                        streamIDs = new List<WaveformStreamTag>();
                        foreach (TreeNode node_date in node_node.Nodes)
                        {
                            foreach (TreeNode node_station in node_date.Nodes)
                            {
                                foreach (TreeNode node_channel in node_station.Nodes)
                                {
                                    streamIDs.Add(new WaveformStreamTag() { NetworkName = node_node.Text, StationName = node_station.Text, ChannelName = node_channel.Text });
                                }
                            }
                        }

                        //frm_Process_Spectra_Home f_Process_Spectra_Home = new frm_Process_Spectra_Home(this);
                        //f_Process_Spectra_Home.StartPosition = FormStartPosition.CenterParent;
                        //f_Process_Spectra_Home.streamIDs = streamIDs;
                        //f_Process_Spectra_Home.selectedFiles = selectedFiles;
                        //f_Process_Spectra_Home.ShowDialog();
                    }
                    #endregion LOCAL_NETWORK
                    break;

                case "LOCAL_DATE":
                    #region LOCAL_DATE
                    if (menu_text == "Open Helicorder")
                    {
                        //frm_Helicorder f_helicorder = new frm_Helicorder(this);
                        //f_helicorder.MdiParent = this;
                        //f_helicorder.WindowState = FormWindowState.Maximized;
                        //f_helicorder.Show();
                    }
                    else if (menu_text == "Display Waveforms")
                    {
                        List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
                        GetMiniseedInfosInsideNode(node_node, true, ref miniSeedFileInfos/*, ChannelCodes.Vertical*/);

                        if (miniSeedFileInfos.Count == 0)
                        {
                            GetMiniseedInfosInsideNode(node_node, false, ref miniSeedFileInfos/*, ChannelCodes.Vertical*/);
                        }

                        frm_WaveViewer_Local f_WaveViewer_Local = new frm_WaveViewer_Local(this);
                        f_WaveViewer_Local.MdiParent = this;
                        f_WaveViewer_Local.WindowState = FormWindowState.Maximized;
                        f_WaveViewer_Local.MiniSeedFileInfos_Selected = miniSeedFileInfos;
                        f_WaveViewer_Local.OnlyVerticalComponent = true;
                        f_WaveViewer_Local.Show();
                    }
                    else if (menu_text == "Select and Display Waveforms")
                    {
                        //List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
                        //GetMiniseedInfosInsideNode(node_node, true, ref miniSeedFileInfos/*, ChannelCodes.Vertical*/);

                        //if (miniSeedFileInfos.Count == 0)
                        //{
                        //    GetMiniseedInfosInsideNode(node_node, false, ref miniSeedFileInfos/*, ChannelCodes.Vertical*/);
                        //}

                        //frm_WaveViewer_Selection f_WaveViewer_Selection = new frm_WaveViewer_Selection(this);
                        //f_WaveViewer_Selection.MiniSeedFileInfos = miniSeedFileInfos;
                        //f_WaveViewer_Selection.OnlyVerticalComponent = true;
                        //f_WaveViewer_Selection.ShowDialog();
                    }
                    //else if (menu_text == "Display Headers")
                    //{
                    //    List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
                    //    GetMiniseedInfosInsideNode(node_node, true, ref miniSeedFileInfos/*, ChannelCodes.Vertical*/);

                    //    if (miniSeedFileInfos.Count == 0)
                    //    {
                    //        GetMiniseedInfosInsideNode(node_node, false, ref miniSeedFileInfos/*, ChannelCodes.Vertical*/);
                    //    }

                    //    frm_ViewHeaders f_viewheader = new frm_ViewHeaders(this);
                    //    f_viewheader.MiniSeedFileInfos_Selected = miniSeedFileInfos;
                    //    f_viewheader.MdiParent = this;
                    //    f_viewheader.WindowState = FormWindowState.Maximized;
                    //    f_viewheader.Show();
                    //}
                    else if (menu_text == "Remove this Dataset")
                    {
                        if (MessageBox.Show("Sure to remove Dataset " + node_node.Text + "?", "Remove Dataset", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            //RemoveNode_Local(node_node);
                        }
                    }
                    #endregion LOCAL_DATE
                    break;

                case "LOCAL_STATION":
                    #region LOCAL_STATION
                    if (menu_text == "Open Helicorder")
                    {
                        //List<String> selectedFiles = new List<string>();
                        //List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
                        //GetMiniseedInfosInsideNode(node_node, true, ref miniSeedFileInfos, ChannelCodes.Vertical);
                        //GetFilesInsideNode(node_node, true, ref selectedFiles);

                        //if (miniSeedFileInfos.Count == 0)
                        //{
                        //    GetMiniseedInfosInsideNode(node_node, false, ref miniSeedFileInfos, ChannelCodes.Vertical);
                        //    GetFilesInsideNode(node_node, false, ref selectedFiles);
                        //}

                        //frm_WaveViewer_Local f_WaveViewer_Local = new frm_WaveViewer_Local(this);
                        ////frm_Helicorder f_helicorder = new frm_Helicorder(this);
                        //f_WaveViewer_Local.MdiParent = this;
                        //f_WaveViewer_Local.WindowState = FormWindowState.Maximized;
                        //f_WaveViewer_Local.MiniSeedFileInfos_Selected = miniSeedFileInfos;
                        ////f_helicorder.StreamIDs = streamIDs;
                        //f_WaveViewer_Local.Show();
                    }
                    else if (menu_text == "Display Waveforms")
                    {
                        //List<String> selectedFiles = new List<string>();
                        List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
                        GetMiniseedInfosInsideNode(node_node, true, ref miniSeedFileInfos);
                        //GetFilesInsideNode(node_node, true, ref selectedFiles);

                        if (miniSeedFileInfos.Count == 0)
                        {
                            GetMiniseedInfosInsideNode(node_node, false, ref miniSeedFileInfos);
                            //GetFilesInsideNode(node_node, false, ref selectedFiles);
                        }

                        frm_WaveViewer_Local f_WaveViewer_Local = new frm_WaveViewer_Local(this);
                        f_WaveViewer_Local.MdiParent = this;
                        f_WaveViewer_Local.MiniSeedFileInfos_Selected = miniSeedFileInfos;
                        f_WaveViewer_Local.WindowState = FormWindowState.Maximized;
                        f_WaveViewer_Local.Show();
                    }
                    //else if (menu_text == "Display Headers")
                    //{
                    //    List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
                    //    GetMiniseedInfosInsideNode(node_node, true, ref miniSeedFileInfos/*, ChannelCodes.Vertical*/);

                    //    if (miniSeedFileInfos.Count == 0)
                    //    {
                    //        GetMiniseedInfosInsideNode(node_node, false, ref miniSeedFileInfos/*, ChannelCodes.Vertical*/);
                    //    }

                    //    frm_ViewHeaders f_viewheader = new frm_ViewHeaders(this);
                    //    f_viewheader.MiniSeedFileInfos_Selected = miniSeedFileInfos;
                    //    f_viewheader.MdiParent = this;
                    //    f_viewheader.WindowState = FormWindowState.Maximized;
                    //    f_viewheader.Show();
                    //}
                    #endregion LOCAL_STATION
                    break;

                case "LOCAL_CHANNEL":
                    #region LOCAL_CHANNEL

                    #endregion LOCAL_CHANNEL
                    break;

                case "LOCAL_FILE":
                    #region LOCAL_FILE
                    if (menu_text == "")
                    {

                    }
                    else if (menu_text == "Display Waveforms")
                    {
                        frm_WaveViewer_Local f_WaveViewer_Local = new frm_WaveViewer_Local(this);
                        f_WaveViewer_Local.MdiParent = this;
                        f_WaveViewer_Local.WindowState = FormWindowState.Maximized;
                        f_WaveViewer_Local.MSS_Info = (node_node.Tag as MiniSeedFileInfo);
                        f_WaveViewer_Local.Show();
                    }
                    else if (menu_text == "Properties")
                    {
                        Frm_Properties_MiniseedFile F_Properties_MiniseedFile = new Frm_Properties_MiniseedFile(this);
                        F_Properties_MiniseedFile.StartPosition = FormStartPosition.CenterParent;
                        F_Properties_MiniseedFile.MSFileName = (node_node.Tag as MiniSeedFileInfo).FileName;
                        F_Properties_MiniseedFile.ShowDialog();

                    }
                    #endregion LOCAL_FILE
                    break;
                #endregion LOCAL

                #region REMOTE
                case "REMOTE_SERVER":
                    #region REMOTE_SERVER
                    ServerAddress = node_node.Text; //server name
                    NetworkName = String.Empty; //network name
                    StationName = String.Empty;// node_node.Text; // station name Open Processing
                    //MonitorIndex = (node_node.Tag as TreeNodeTag).MonitorIndex;
                    //RemoveServer(node, ServerName, MonitorIndex);
                    if (menu_text == "Connect to Server")
                    {
                        //frm_Dialog_Properties_Station prop_station = new frm_Dialog_Properties_Station(this);
                        //prop_station.ShowDialog();
                    }
                    if (menu_text == "Disconnect from Server")
                    {
                        //frm_Dialog_Properties_Station prop_station = new frm_Dialog_Properties_Station(this);
                        //prop_station.ShowDialog();
                    }
                    if (menu_text == "Remove this Server")
                    {
                        if (MessageBox.Show("Sure to delete Server " + ServerAddress + "?", "Delete Server", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            RemoveRemoteDatafeed_Server(node_node);
                        }
                    }
                    if (menu_text == "Properties")
                    {
                        frm_Dialog_Properties_RemoteServer prop_server = new frm_Dialog_Properties_RemoteServer(this);
                        prop_server.StartPosition = FormStartPosition.CenterParent;
                        prop_server.ShowDialog();
                    }
                    #endregion REMOTE_SERVER
                    break;

                case "REMOTE_NETWORK":
                    #region REMOTE_NETWORK
                    ServerAddress = node_node.Parent.Text;
                    NetworkName = node_node.Text;
                    StationName = String.Empty;

                    if (menu_text == "Open Wave Viewer")
                    {
                        //monitor.DisplayWaveViewer(ServerAddress, NetworkName, StationName, true);
                    }
                    else if (menu_text == "Remove this Network")
                    {
                        if (MessageBox.Show("Sure to delete Network " + NetworkName + "?", "Delete Network", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            RemoveRemoteDatafeed_Network(node_node);
                        }
                    }
                    else if (menu_text == "Properties")
                    {
                        //monitor.DisplayProperty(/*ServerAddress, NetworkName, StationName, */node_node.ImageKey);
                    }
                    #endregion REMOTE_NETWORK
                    break;

                case "REMOTE_STATION":
                    #region REMOTE_STATION
                    ServerAddress = node_node.Parent.Parent.Text; //server name
                    NetworkName = node_node.Parent.Text; //network name
                    StationName = node_node.Text; // station name Open Processing
                    MonitorIndex = (node_node.Tag as TreeNodeTag).MonitorIndex;
                    monitor = SW_Monitors.Find(q => q.Index == MonitorIndex);
                    //monitor = SW_Monitors.Find(q => q.ServerAddress == ServerAddress && q.NetworkName == NetworkName && q.StationName == StationName);

                    if (menu_text == "Open Wave Viewer")
                    {
                        monitor.DisplayWaveViewer(ServerAddress, NetworkName, StationName, true);
                        //// cari dulu sudah ada belum form ieu
                        //f_waveviewer = monitor.f_waveviewers.Find(q => q.StationName == StationName);
                        //if (f_waveviewer == null)
                        //{
                        //    f_waveviewer = new frm_WaveViewer_Remote(this);
                        //    monitor.f_waveviewers.Add(f_waveviewer);
                        //    //MS_Monitors[RecorderIndex].f_waveviewer = f_waveviewer;
                        //    f_waveviewer.StationName = StationName;
                        //    f_waveviewer.NetworkName = NetworkName;
                        //    f_waveviewer.ServerName = ServerName;
                        //    //f_waveviewer.IsStreaming = true;
                        //    f_waveviewer.MdiParent = this;
                        //    f_waveviewer.WindowState = FormWindowState.Maximized;
                        //    f_waveviewer.Show();
                        //}
                        //else
                        //{
                        //    //f_waveviewer.Show();
                        //    f_waveviewer.WindowState = FormWindowState.Maximized;
                        //    f_waveviewer.BringToFront();
                        //    f_waveviewer.Show();
                        //}
                    }
                    else if (menu_text == "Connect to this Station")
                    {
                        //node_node.ImageKey = "station_connecting_24.png";
                        //node_node.SelectedImageKey = "station_connecting_24.png";
                        //monitor.ConnectToServer();
                    }
                    else if (menu_text == "Disconnect this Station")
                    {
                        //monitor.DisconnectFromServer();
                    }
                    else if (menu_text == "Remove this Station")
                    {
                        if (MessageBox.Show("Sure to delete Station " + StationName + "?", "Delete Station", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            RemoveRemoteDatafeed_Station(node_node);
                        }
                    }
                    else if (menu_text == "Properties")
                    {
                        monitor.DisplayProperty(/*ServerAddress, NetworkName, StationName, */node_node.ImageKey);

                        //frm_Properties_RemoteStation prop_station = new frm_Properties_RemoteStation(this);
                        //prop_station.StartPosition = FormStartPosition.CenterParent;
                        //prop_station.ShowDialog();

                        //// cari dulu sudah ada belum form properties
                        //f_property = monitor.f_properties.Find(q => q.StationName == StationName);
                        //if (f_property == null)
                        //{
                        //    f_property = new frm_WaveStatus(this);
                        //    f_property.StationName = StationName;
                        //    f_property.NetworkName = NetworkName;
                        //    f_property.ServerName = ServerName;
                        //    f_property.RecorderIndex = MonitorIndex;
                        //    f_property.MdiParent = this;
                        //    f_property.WindowState = FormWindowState.Normal;
                        //    f_property.Show();
                        //    monitor.f_wavestatuss.Add(f_property);
                        //}
                        //else
                        //{
                        //    f_property.BringToFront();
                        //    f_property.Show();
                        //}
                    }
                    #endregion REMOTE_STATION
                    break;
                    #endregion REMOTE
            }
        }

        private void FromTreeMenu_DisplayWaveforms(TreeNode node_node)
        {
            List<String> selectedFiles = new List<string>();
            List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
            GetMiniseedInfosInsideNode(node_node, true, ref miniSeedFileInfos);
            GetFilesInsideNode(node_node, true, ref selectedFiles);

            if (miniSeedFileInfos.Count == 0)
            {
                GetMiniseedInfosInsideNode(node_node, false, ref miniSeedFileInfos);
                GetFilesInsideNode(node_node, false, ref selectedFiles);
            }

            // stationnamefromdata
            var tag = node_node.Parent.Tag;
            string UserDefinedNetworkName = String.Empty;
            Boolean stationNameFromData = true;
            if (tag is object[])
            {
                var tatag = tag as object[];
                UserDefinedNetworkName = tatag[0] == null ? String.Empty : tatag[0].ToString();
                stationNameFromData = Convert.ToBoolean(tatag[1]);
            }

            frm_WaveViewer_Local f_WaveViewer_Local = new frm_WaveViewer_Local(this);
            f_WaveViewer_Local.MdiParent = this;
            f_WaveViewer_Local.MiniSeedFileInfos_Selected = miniSeedFileInfos;
            f_WaveViewer_Local.WindowState = FormWindowState.Maximized;
            f_WaveViewer_Local.UserDefinedNetworkName = UserDefinedNetworkName;
            f_WaveViewer_Local.StationNameFromData = stationNameFromData;
            f_WaveViewer_Local.DatafeedLocal = false;
            f_WaveViewer_Local.Show();
        }

        private void RemoveInsideXmlFile(String xml_file, String server_name, String network_name, String station_name)
        {
            if (String.IsNullOrEmpty(xml_file))
            {
                return;
            }
            var slcon = class_seedlink.ReadXmlFile(xml_file);

            if (!String.IsNullOrEmpty(station_name))
            {
                var sl_networks = slcon.Networks.FindAll(q => q.Name == network_name);
                foreach (var sl_network in sl_networks)
                {
                    //var sl_network = slcon.Network;
                    var sl_station = sl_network.Stations.Find(q => q.Name == station_name);
                    if (sl_station != null)
                    {
                        sl_network.Stations.Remove(sl_station);
                    }
                    if (sl_network.Stations.Count == 0)
                    {
                        slcon.Networks.Remove(sl_network);
                    }
                }
            }
            slcon.SaveXmlFile();
        }

        private void SpectraProcessing(object sender, EventArgs e)
        {
            var node_tags = new TreeMenuTag();
            var node_name = String.Empty;
            var node_text = String.Empty;
            var node_node = new TreeNode();
            var node_subs = false;

            frm_Process_Spectra_Home f_Process_Spectra_Home = new frm_Process_Spectra_Home(this);
            f_Process_Spectra_Home.StartPosition = FormStartPosition.CenterParent;
            f_Process_Spectra_Home.ShowDialog();
        }

        private void UpdateNodes(Int32 TreeStyle)
        {
            Node_Files = SeisWingFunction.FindTreeNode(triStateCheckBoxTreeView1.Nodes, "FILES", "FILES");
            Node_Files.Nodes.Clear();
            SeisWingFunction.UpdateNode_FILES(MiniSeedFileInfos_FILES, TreeStyle, ref Node_Files);

            Node_Local = SeisWingFunction.FindTreeNode(triStateCheckBoxTreeView1.Nodes, "LOCAL", "LOCAL");
            Node_Local.Nodes.Clear();
            SeisWingFunction.UpdateNode_LOCAL(MiniSeedFileInfos_LOCAL, TreeStyle, ref Node_Local);
        }

        #region populate_data_feed_files
        private void UpdateNodes_Files()
        {
            FilesFeed_Files = Directory.GetFiles(Folder_Datafeed_Files);
            foreach (var file_file in FilesFeed_Files)
            {
                if (Path.GetExtension(file_file) == ".xml")
                {
                    UpdateNode_Files(file_file);
                }
            }
        }

        private void UpdateNode_Files(String xml_file)
        {
            var tree = XmlFunctions.Read_TreeNodeFromXml_FILES(xml_file);
            foreach (var msinfo in tree.MiniSeedFileInfos)
            {
                if (!MiniSeedFileInfos_FILES.Contains(msinfo))
                {
                    msinfo.StationNameFromData = tree.StationNameFromData;
                    msinfo.UserDefinedNetworkName = tree.UserDefinedNetworkName;
                    msinfo.XML_Datafeed_FileName = tree.XML_Datafeed_FileName;
                    MiniSeedFileInfos_FILES.Add(msinfo);
                }
            }
            SeisWingFunction.UpdateNode_FILES(MiniSeedFileInfos_FILES, xml_file, 4, ref Node_Files);
            Node_Files.Expand();
        }

        private void UpdateNode_Files(List<MiniSeedFileInfo> MSD_FileInfos, TreeNodes_FILES tree)
        {
            foreach (var msinfo in MSD_FileInfos)
            {
                if (!MiniSeedFileInfos_FILES.Contains(msinfo))
                {
                    msinfo.StationNameFromData = tree.StationNameFromData;
                    msinfo.UserDefinedNetworkName = tree.UserDefinedNetworkName;
                    msinfo.XML_Datafeed_FileName = tree.XML_Datafeed_FileName;
                    MiniSeedFileInfos_FILES.Add(msinfo);
                }
            }
            SeisWingFunction.UpdateNode_FILES(MiniSeedFileInfos_FILES, 4, ref Node_Files);
            Node_Files.Expand();
        }

        private void UpdateNode_Files(TreeNodes_FILES tree, Boolean good)
        {
            //TreeNode node_folder = new TreeNode();
            //TreeNode node_date = new TreeNode();
            //TreeNode node_station = new TreeNode();
            //TreeNode node_channel = new TreeNode();
            //TreeNode node_file = new TreeNode();
            //string dateformat = "yyyyMMdd";

            //node_folder = SeisWingFunction.FindTreeNode(Node_Files.Nodes, "FILES_DIRECTORY", tree.Title);
            //if (node_folder == null)
            //{
            //    node_folder = new TreeNode();
            //    node_folder.Name = "FILES_DIRECTORY";
            //    node_folder.Tag = new object[] { tree.Tag, tree.StationNameFromData };
            //    node_folder.Text = tree.Title;
            //    node_folder.ImageKey = "folderhejo_24.png";
            //    node_folder.SelectedImageKey = "folderhejo_24.png";
            //    Node_Files.Nodes.Add(node_folder);
            //}

            //foreach (var file in tree.MiniSeedFileInfos)
            //{
            //    if (!File.Exists(file.FileName))
            //    {
            //        continue;
            //    }
            //    node_station = SeisWingFunction.FindTreeNode(node_folder.Nodes, "FILES_STATION", file.StreamTag.StationName);
            //    if (node_station == null)
            //    {
            //        node_station = new TreeNode();
            //        node_station.Name = "FILES_STATION";
            //        node_station.Tag = file.StreamTag.StationName;
            //        node_station.Text = file.StreamTag.StationName;
            //        node_station.ImageKey = "station_24.png";
            //        node_station.SelectedImageKey = "station_24.png";
            //        node_folder.Nodes.Add(node_station);
            //    }

            //    node_date = SeisWingFunction.FindTreeNode(node_station.Nodes, "FILES_DATE", file.Times.TimeBegin.ToString(dateformat));
            //    if (node_date == null)
            //    {
            //        node_date = new TreeNode();
            //        node_date.Name = "FILES_DATE";
            //        node_date.Tag = file.Times.TimeBegin.ToString(dateformat);
            //        node_date.Text = file.Times.TimeBegin.ToString(dateformat);
            //        node_date.ImageKey = "calendar_24.png";
            //        node_date.SelectedImageKey = "calendar_24.png";
            //        node_station.Nodes.Add(node_date);
            //    }

            //    node_channel = SeisWingFunction.FindTreeNode(node_date.Nodes, "FILES_CHANNEL", file.StreamTag.ChannelName);
            //    if (node_channel == null)
            //    {
            //        node_channel = new TreeNode();
            //        node_channel.Name = "FILES_CHANNEL";
            //        node_channel.Tag = file.StreamTag.ChannelName;
            //        node_channel.Text = file.StreamTag.ChannelName;
            //        node_channel.ImageKey = "channel_24.png";
            //        node_channel.SelectedImageKey = "channel_24.png";
            //        node_date.Nodes.Add(node_channel);
            //    }

            //    node_file = SeisWingFunction.FindTreeNode(node_channel.Nodes, "FILES_FILE", Path.GetFileName(file.FileName));
            //    if (node_file == null)
            //    {
            //        node_file = new TreeNode();
            //        node_file.Name = "FILES_FILE";
            //        node_file.Tag = file;
            //        node_file.Text = Path.GetFileName(file.FileName);
            //        node_file.ImageKey = "msd_24.png";
            //        node_file.SelectedImageKey = "msd_24.png";
            //        node_channel.Nodes.Add(node_file);
            //    }
            //}

            ////foreach (var station in tree.Stations)
            ////{
            ////    node_station = SeisWingFunction.FindTreeNode(node_folder.Nodes, "FILES_STATION", station.Name);
            ////    if (node_station == null)
            ////    {
            ////        node_station = new TreeNode();
            ////        node_station.Name = "FILES_STATION";
            ////        node_station.Tag = station.Tag;
            ////        node_station.Text = station.Name;
            ////        node_station.ImageKey = "station_24.png";
            ////        node_station.SelectedImageKey = "station_24.png";
            ////        node_folder.Nodes.Add(node_station);
            ////    }

            ////    foreach (var channel in station.Channels)
            ////    {
            ////        node_channel = SeisWingFunction.FindTreeNode(node_station.Nodes, "FILES_CHANNEL", channel.Name);
            ////        if (node_channel == null)
            ////        {
            ////            node_channel = new TreeNode();
            ////            node_channel.Name = "FILES_CHANNEL";
            ////            node_channel.Tag = channel.Tag;
            ////            node_channel.Text = channel.Name;
            ////            node_channel.ImageKey = "channel_24.png";
            ////            node_channel.SelectedImageKey = "channel_24.png";
            ////            node_station.Nodes.Add(node_channel);
            ////        }
            ////        foreach (var file in channel.Files)
            ////        {
            ////            node_file = SeisWingFunction.FindTreeNode(node_channel.Nodes, "FILES_FILE", Path.GetFileName(file.Filename));
            ////            if (node_file == null)
            ////            {
            ////                node_file = new TreeNode();
            ////                node_file.Name = "FILES_FILE";
            ////                node_file.Tag = file;
            ////                node_file.Text = Path.GetFileName(file.Filename);
            ////                node_file.ImageKey = "msd_24.png";
            ////                node_file.SelectedImageKey = "msd_24.png";
            ////                node_channel.Nodes.Add(node_file);
            ////            }
            ////        }
            ////    }
            ////}

            //node_folder.Expand();
            //Node_Files.Expand();
            //triStateCheckBoxTreeView1.Invalidate();
        }

        private void Populate_Files_Feed(String files_file)
        {
            if (FilesFeed_Files_Files.Count > 0)
            {
                LastSentThread = 0;
                StationNameFromData = false;
                int num_workers = Math.Min(NumberOfProcessors, FilesFeed_Files_Files.Count);
                msdBgWorkers = new BW_LocalNodes[num_workers];
                for (int i = 0; i < num_workers; i++)
                {
                    msdBgWorkers[i] = new BW_LocalNodes(i);
                    msdBgWorkers[i].ID_Worker = i;
                    msdBgWorkers[i].FileFeed_Directory = files_file;
                    msdBgWorkers[i].StationNameFromData = StationNameFromData;
                    //msdBgWorkers[i].LocalFeed_Folder = FilesFeed_Folder;
                    msdBgWorkers[i].ProgressChanged += BW_FilesNodes_ProgressChanged;
                    msdBgWorkers[i].RunWorkerCompleted += BW_FilesNodes_RunWorkerCompleted;
                }
                toolStripProgressBar1.Maximum = FilesFeed_Files_Files.Count;
                toolStripProgressBar1.Value = 0;
                MSD_FileInfos = new List<MiniSeedFileInfo>();
                BW_FilesNodes_AssignWorkers();
            }
        }

        private void AddDataFeed_FILES_ProcessStarted(object sender, EventArgs e)
        {
            toolStripProgressBar1.Maximum = (sender as AddDataFeed_FILES).LocalFeed_Files_Files.Count;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel2.Text = "Loading Files Started.";
        }

        private void AddDataFeed_FILES_ProcessChanged(object sender, EventArgs e)
        {
            var ngok = (sender as AddDataFeed_FILES);
            UpdateNode_Files(ngok.MSD_FileInfos, ngok.TreeNodes);
            toolStripProgressBar1.Value = ngok.LastSentThread;
            toolStripStatusLabel2.Text = "Loading File: " + ngok.MSD_FileInfos[0].FileName + "...";
        }

        private void AddDataFeed_FILES_ProcessCompleted(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = "Loading Files Completed.";
            System.Threading.Thread.Sleep(1000);
            toolStripStatusLabel2.Text = String.Empty;
        }

        private void BW_FilesNodes_AssignWorkers()
        {
            Boolean ThereAreWorkersWorking = false;
            foreach (BW_LocalNodes WORKER in msdBgWorkers)
            {
                if (!WORKER.IsBusy)
                {
                    if (FilesFeed_Files_Files.Count > LastSentThread)
                    {
                        WORKER.ID_Job = LastSentThread;
                        WORKER.RunWorkerAsync(new Object[] { FilesFeed_Files_Files[LastSentThread] });

                        ThereAreWorkersWorking = true;
                        LastSentThread++;
                    }
                }
                else
                {
                    ThereAreWorkersWorking = true;
                }
            }

            if (!ThereAreWorkersWorking)
            {
                string xml_file = string.Empty;

                #region bikin_nodenyah
                MSD_FileInfos = MSD_FileInfos.OrderBy(q => q.FileName).ToList();
                foreach (var msdfileinfo in MSD_FileInfos)
                {
                    //var station = FileNodesForXml.Stations.Find(q => q.Name == msdfileinfo.StreamID.StationName);
                    //if (station == null)
                    //{
                    //    station = new LocalTreeNodesForXml_Station();
                    //    station.Name = msdfileinfo.StreamID.StationName;
                    //    station.Tag = new NodeStationTag() { NetworkName = msdfileinfo.StreamID.StationName, DateName = msdfileinfo.Times.StartTime.ToString("yyyyMMdd") };
                    //    FileNodesForXml.Stations.Add(station);
                    //}

                    //var channel = station.Channels.Find(q => q.Name == msdfileinfo.StreamID.ChannelName);
                    //if (channel == null)
                    //{
                    //    channel = new LocalTreeNodesForXml_Channel();
                    //    channel.Name = msdfileinfo.StreamID.ChannelName;
                    //    channel.Tag = new NodeChannelTag() { NetworkName = msdfileinfo.StreamID.StationName, DateName = msdfileinfo.Times.StartTime.ToString("yyyyMMdd"), StationName = msdfileinfo.StreamID.StationName };
                    //    station.Channels.Add(channel);
                    //}

                    //var file = channel.Files.Find(q => q.Filename == msdfileinfo.Filename);
                    //if (file == null)
                    //{
                    //    channel.Files.Add(msdfileinfo);
                    //}
                }
                #endregion bikin_nodenyah

                #region bikin_xmlnyah
                ////xml_file = Datafeed_Files_Dir + network.Tag.Replace(":", "").Replace("\\", "") + ".xml";
                //xml_file = Folder_Datafeed_Files + FileNodesForXml.Title.Replace(":", "_@_").Replace("\\", "@_@") + ".xml";
                //string xml_content = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
                //xml_content += "    <Title name=\"" + FileNodesForXml.Title + "\" tag=\"" + FileNodesForXml.Tag + "\">\n";
                //xml_content += "        <StationNameFromData>" + StationNameFromData + "</StationNameFromData>\n";
                //foreach (var station in FileNodesForXml.Stations)
                //{
                //    xml_content += "        <Station name=\"" + station.Name + "\" tag=\"" + station.Tag + "\">\n";
                //    foreach (var channel in station.Channels)
                //    {
                //        xml_content += "            <Channel name=\"" + channel.Name + "\" tag=\"" + channel.Tag + "\">\n";
                //        foreach (var file in channel.Files)
                //        {
                //            xml_content += "                <File>\n";
                //            xml_content += "                    <Filename>" + file.Filename + "</Filename>\n";
                //            xml_content += "                    <Duration>" + file.Duration.TotalMinutes + "</Duration>\n";
                //            xml_content += "                    <TotalSamples>" + file.TotalSamples + "</TotalSamples>\n";
                //            xml_content += "                    <SampleRate>" + file.SampleRate + "</SampleRate>\n";
                //            xml_content += "                    <StartTime>" + file.Times.StartTime.ToStringTZ() + "</StartTime>\n";
                //            xml_content += "                    <EndTime>" + file.Times.EndTime.ToStringTZ() + "</EndTime>\n";
                //            xml_content += "                </File>\n";
                //        }
                //        xml_content += "            </Channel>\n";
                //    }
                //    xml_content += "        </Station>\n";
                //}
                //xml_content += "    </Title>\n";
                //StreamWriter sr = new StreamWriter(xml_file);
                //sr.Write(xml_content);
                //sr.Close();
                #endregion bikin_xmlnyah

                #region cekan_sudah_aya_inventory
                //if (Inventory == null)
                //{
                //    Inventory = new Class_Inventory();
                //}
                //foreach (var network in FileNodesForXml.Networks)
                //{
                //    var inv_network = Inventory.Networks.Find(q => q.code == network.Name);
                //    if (inv_network == null)
                //    {
                //        inv_network = new Class_Network();
                //        inv_network.code = network.Name;
                //        inv_network.start = DateTimeUing.FromDateTime(DateTime.Now);
                //        Inventory.Networks.Add(inv_network);
                //    }
                //    foreach (var date in network.Dates)
                //    {
                //        foreach (var station in date.Stations)
                //        {
                //            var inv_station = inv_network.Stations.Find(q => q.code == station.Name);
                //            if (inv_station == null)
                //            {
                //                inv_station = new Class_Station();
                //                inv_station.code = station.Name;
                //                inv_station.start = DateTimeUing.FromDateTime(DateTime.Now);
                //                inv_network.Stations.Add(inv_station);
                //            }
                //            List<string> sensor_locations = new List<string>();
                //            foreach (var channel in station.Channels)
                //            {
                //                foreach (var file in channel.Files)
                //                {
                //                    sensor_locations.Add(file.StreamID._locationCode);
                //                }
                //            }
                //            sensor_locations = new HashSet<string>(sensor_locations).ToList();
                //            foreach (var sensor_location in sensor_locations)
                //            {
                //                var inv_location = new SensorLocationPtr();
                //                inv_location._code = String.IsNullOrEmpty(sensor_location) ? "" : sensor_location;
                //                inv_location.start = DateTimeUing.FromDateTime(DateTime.Now);
                //                if (inv_station.sensorLocation.Find(q => q._code == inv_location._code) == null)
                //                {
                //                    inv_station.sensorLocation.Add(inv_location);
                //                }
                //                foreach (var channel in station.Channels)
                //                {
                //                    foreach (var file in channel.Files)
                //                    {
                //                        var stream_location = String.IsNullOrEmpty(file.StreamID._locationCode) ? "" : file.StreamID._locationCode;
                //                        var inv_channel = inv_location._streams.Find(q => q.code == file.StreamID.ChannelName);
                //                        if (inv_channel == null)
                //                        {
                //                            inv_channel = new StreamPtr();
                //                            inv_channel.code = file.StreamID.ChannelName;
                //                            inv_channel.sampleRateNumerator = file.SampleRate;
                //                            inv_channel.sampleRateDenominator = 1;
                //                            inv_channel.start = DateTimeUing.FromDateTime(DateTime.Now);
                //                            inv_channel.sensorChannel = 1;
                //                            inv_channel.locationCode = stream_location;
                //                            inv_location._streams.Add(inv_channel);
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                #endregion cekan_sudah_aya_inventory

                UpdateNode_Files(xml_file);
            }
        }

        private void BW_FilesNodes_ProgressChanged(Object sender, ProgressChangedEventArgs e)
        {

        }

        private void BW_FilesNodes_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            //toolStripProgressBar1.Value++;

            var worker = sender as BW_LocalNodes;
            var msd_file_infos = worker.MSD_FileInfos;
            var files_file = worker.FileFeed_Directory;
            MSD_FileInfos.AddRange(msd_file_infos);

            toolStripProgressBar1.Value = worker.ID_Job;

            foreach (var info in msd_file_infos)
            {

            }

            #region bikin_tangkalnyah
            //if (msd_file_infos.Count > 1)
            //{
            //    var abe = 09;
            //}
            ////var data = e.UserState as UserData;
            //if (msd_file_infos == null || msd_file_infos.Count == 0)
            //{
            //    return;
            //}
            //foreach (var msdfileinfo in msd_file_infos)
            //{

            //    //var fsdh = data.header;
            //    //var msfile_info = msdfileinfo.Filename;

            //    if (msdfileinfo == null)
            //    {
            //        return;
            //    }

            //    String node_text = "LOCAL_NETWORK";
            //    String node_tage = "LOCAL_NETWORK";
            //    TreeNode node_network = new TreeNode();
            //    TreeNode node_date = new TreeNode();
            //    TreeNode node_station = new TreeNode();
            //    TreeNode node_channel = new TreeNode();
            //    TreeNode node_file = new TreeNode();

            //    int index_network = -1;
            //    int index_date = -1;
            //    int index_station = -1;
            //    int index_channel = -1;
            //    int index_file = -1;

            //    node_text = msdfileinfo.StreamID.NetworkName;//.NetworkName;
            //    node_tage = files_file;
            //    index_network = FileNodesForXml.Networks.FindIndex(q => q.Name == node_text && q.Tag == node_tage);
            //    if (index_network < 0)
            //    {
            //        index_network = FileNodesForXml.Networks.Count;
            //        FileNodesForXml.Networks.Add(new FilesTreeFileNodesForXml_Network() { Name = node_text, Tag = node_tage });
            //    }

            //    node_tage = String.Format("{0}{1:00}{2:00}", msdfileinfo.Times.StartTime.Year, msdfileinfo.Times.StartTime.Month, msdfileinfo.Times.StartTime.Day);
            //    node_text = String.Format("{0}{1:00}{2:00}", msdfileinfo.Times.StartTime.Year, msdfileinfo.Times.StartTime.Month, msdfileinfo.Times.StartTime.Day);
            //    index_date = FileNodesForXml.Networks[index_network].Dates.FindIndex(q => q.Name == node_text && q.Tag == node_tage);
            //    if (index_date < 0)
            //    {
            //        index_date = FileNodesForXml.Networks[index_network].Dates.Count;
            //        FileNodesForXml.Networks[index_network].Dates.Add(new FilesTreeFileNodesForXml_Date() { Name = node_text, Tag = node_tage });
            //    }

            //    node_tage = msdfileinfo.StreamID.StationName;
            //    node_text = msdfileinfo.StreamID.StationName;
            //    index_station = FileNodesForXml.Networks[index_network].Dates[index_date].Stations.FindIndex(q => q.Name == node_text && q.Tag == node_tage);
            //    if (index_station < 0)
            //    {
            //        index_station = FileNodesForXml.Networks[index_network].Dates[index_date].Stations.Count;
            //        FileNodesForXml.Networks[index_network].Dates[index_date].Stations.Add(new FilesTreeFileNodesForXml_Station() { Name = node_text, Tag = node_tage });
            //    }

            //    node_tage = msdfileinfo.StreamID.ChannelName;
            //    node_text = msdfileinfo.StreamID.ChannelName;
            //    index_channel = FileNodesForXml.Networks[index_network].Dates[index_date].Stations[index_station].Channels.FindIndex(q => q.Name == node_text && q.Tag == node_tage);
            //    if (index_channel < 0)
            //    {
            //        index_channel = FileNodesForXml.Networks[index_network].Dates[index_date].Stations[index_station].Channels.Count;
            //        FileNodesForXml.Networks[index_network].Dates[index_date].Stations[index_station].Channels.Add(new FilesTreeFileNodesForXml_Channel() { Name = node_text, Tag = node_tage });
            //    }

            //    //node_tage = msfile_info;
            //    node_text = Path.GetFileName(msdfileinfo.Filename);
            //    index_file = FileNodesForXml.Networks[index_network].Dates[index_date].Stations[index_station].Channels[index_channel].Files.FindIndex(q => q == msdfileinfo);
            //    if (index_file < 0)
            //    {
            //        index_file = FileNodesForXml.Networks[index_network].Dates[index_date].Stations[index_station].Channels[index_channel].Files.Count;
            //        //var msfile_info = new MiniSeedFileInfo();
            //        //msfile_info.Filename=file
            //        FileNodesForXml.Networks[index_network].Dates[index_date].Stations[index_station].Channels[index_channel].Files.Add(msdfileinfo);// new FilesTreeFileNodesForXml_File() { Name = node_text, Tag = node_tage });
            //    }
            //}
            #endregion bikin_tangkalnyah

            //processconpleted++;
            BW_FilesNodes_AssignWorkers();
        }
        #endregion populate_data_feed_files

        #region populate_data_feed_local
        public void UpdateNodes_Local_FromMonitor(String msf, MS_RECORD msr)
        {
            var new_msinfo = miniseed_functions.GetInfo(msf, msr);
            if (new_msinfo == null)
            {
                return;
            }

            // update collection
            var ext_msinfo = MiniSeedFileInfos_LOCAL.Find(q => q.FileName == new_msinfo.FileName);
            if (ext_msinfo == null)
            {
                ext_msinfo = new MiniSeedFileInfo();
                ext_msinfo.FileName = new_msinfo.FileName;
                ext_msinfo.Tag = new_msinfo.Tag;
                ext_msinfo.Times = new_msinfo.Times;
                MiniSeedFileInfos_LOCAL.Add(ext_msinfo);
            }
            ext_msinfo.Times.TimeEnd = new_msinfo.Times.TimeEnd;
            ext_msinfo.TotalSamples += (Int32)(new_msinfo.Times.Duration.TotalSeconds * new_msinfo.Tag.SampleRate);

            // update node
            SeisWingFunction.UpdateNode_LOCAL(new_msinfo, (Int32)TreeDisplayStyle, ref Node_Local);
        }

        private void Bw_Watcher_Local_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var new_msinfo = e.UserState as MiniSeedFileInfo;
            if (new_msinfo != null)
            {
                var ext_msinfo = MiniSeedFileInfos_LOCAL.Find(q => q.FileName == new_msinfo.FileName);
                if (ext_msinfo == null)
                {
                    ext_msinfo = new MiniSeedFileInfo();
                    ext_msinfo.FileName = new_msinfo.FileName;
                    ext_msinfo.Tag = new_msinfo.Tag;
                    ext_msinfo.Times = new_msinfo.Times;
                    MiniSeedFileInfos_LOCAL.Add(ext_msinfo);
                }
                ext_msinfo.Times.TimeEnd = new_msinfo.Times.TimeEnd;
                ext_msinfo.TotalSamples += (Int32)(new_msinfo.Times.Duration.TotalSeconds * new_msinfo.Tag.SampleRate);
            }
        }

        private void Bw_Watcher_Local_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SeisWingFunction.UpdateNode_LOCAL(MiniSeedFileInfos_LOCAL, 1, ref Node_Local);
            Node_Local.Expand();
        }
        #endregion populate_data_feed_local

        #region populate_data_feed_remote
        private void UpdateNodes_Remote()
        {
            // teangan file xml aya teu?
            var xml_files = Directory.GetFiles(Folder_Datafeed_Remote);
            if (xml_files.Length > 0)
            {
                foreach (var xml_file in xml_files)
                {
                    AddDataFeed_Remote(xml_file);
                }
            }
        }

        private void UpdateNode_Remote(SW_MONITOR monitor, bool _pertamax = false)
        {
            pertamax = _pertamax;

            String node_name = String.Empty;
            String node_text = String.Empty;
            TreeNodeTag node_tage = null;// new object();

            SL_Connection = monitor.SL_Connection;

            // NODE_SERVER
            node_name = "REMOTE_SERVER";
            node_tage = null;
            node_text = SL_Connection.ServerAddress;
            var Node_Server = SeisWingFunction.FindTreeNode(Node_Remote.Nodes, node_name, node_text);
            if (Node_Server == null)
            {
                Node_Server = new TreeNode();
                Node_Server.Name = node_name;
                Node_Server.Tag = node_tage;
                Node_Server.Text = node_text;
                Node_Remote.Nodes.Add(Node_Server);
            }
            if (SL_Connection.ServerStatus == NodeStatuss.OK)
            {
                Node_Server.ImageKey = "server_ok_24.png";
                Node_Server.SelectedImageKey = "server_ok_24.png";
            }
            else if (SL_Connection.ServerStatus == NodeStatuss.Warning)
            {
                Node_Server.ImageKey = "server_warning_24.png";
                Node_Server.SelectedImageKey = "server_warning_24.png";
            }
            //Node_Server.ImageKey = "server_24.png";
            //Node_Server.SelectedImageKey = "server_24.png";

            // NODE_NETWORKS
            int n_networks = SL_Connection.Networks.Count;
            if (n_networks > 0)
            {
                for (int i = 0; i < n_networks; i++)
                {
                    node_name = "REMOTE_NETWORK";
                    node_tage = null;
                    node_text = SL_Connection.Networks[i].Name;
                    var Node_Network = SeisWingFunction.FindTreeNode(Node_Server.Nodes, node_name, node_text);
                    if (Node_Network == null)
                    {
                        Node_Network = new TreeNode();
                        Node_Network.Name = node_name;
                        Node_Network.Tag = node_tage;
                        Node_Network.Text = node_text;
                        Node_Server.Nodes.Add(Node_Network);
                    }
                    Node_Network.ImageKey = "network_" + SL_Connection.Networks[i].Status.ToString().ToLower() + "_24.png";//.Networks[i].NetworkStatus.ToString().ToLower() + "_24.png";
                    Node_Network.SelectedImageKey = Node_Network.ImageKey;
                    //if (SL_Connection.Networks[i].NetworkStatus == NodeStatuss.Warning)
                    //{
                    //    Node_Network.ImageKey = "network_warning_24.png";
                    //    Node_Network.SelectedImageKey = "network_warning_24.png";
                    //}
                    //else if (SL_Connection.Networks[i].NetworkStatus == NodeStatuss.OK)
                    //{
                    //    Node_Network.ImageKey = "network_ok_24.png";
                    //    Node_Network.SelectedImageKey = "network_ok_24.png";
                    //}

                    // NODE_STATIONS
                    int n_stations = SL_Connection.Networks[i].Stations.Count;
                    if (n_stations > 0)
                    {
                        for (int j = 0; j < n_stations; j++)
                        {
                            node_name = "REMOTE_STATION";
                            node_tage = new TreeNodeTag() { MonitorIndex = monitor.Index, XmlFile = SL_Connection.XmlFile };
                            node_text = SL_Connection.Networks[i].Stations[j].Name;

                            var Node_Station = SeisWingFunction.FindTreeNode(Node_Network.Nodes, node_name, node_text);
                            if (Node_Station == null)
                            {
                                Node_Station = new TreeNode();
                                Node_Station.Name = node_name;
                                Node_Station.Tag = node_tage;
                                Node_Station.Text = node_text;
                                Node_Network.Nodes.Add(Node_Station);
                            }
                            //if (SL_Connection.Networks[i].Stations[j].StationStatus == NodeStatuss.Warning)
                            //{
                            //    Node_Station.ImageKey = "station_warning_24.png";
                            //}
                            //if (SL_Connection.Networks[i].Stations[j].StationStatus == NodeStatuss.Connecting)
                            //{
                            //    Node_Station.ImageKey = "station_connecting_24.png";
                            //}
                            //else if (SL_Connection.Networks[i].Stations[j].StationStatus == NodeStatuss.Stop)
                            //{
                            //    Node_Station.ImageKey = "station_stop_24.png";
                            //}
                            //else if (SL_Connection.Networks[i].Stations[j].StationStatus == NodeStatuss.OK)
                            //{
                            //    Node_Station.ImageKey = "station_ok_24.png";
                            //}
                            Node_Station.ImageKey = "station_" + SL_Connection.Networks[i].Stations[j].Status.ToString().ToLower() + "_24.png";
                            Node_Station.SelectedImageKey = Node_Station.ImageKey;

                            // NODE_STREAMS
                            int n_streams = SL_Connection.Networks[i].Stations[j].Chanels.Count;
                            if (n_streams > 0)
                            {
                                for (int k = 0; k < n_streams; k++)
                                {
                                    node_name = "REMOTE_CHANNEL";
                                    node_tage = new TreeNodeTag() { MonitorIndex = monitor.Index, XmlFile = SL_Connection.XmlFile };
                                    node_text = SL_Connection.Networks[i].Stations[j].Chanels[k].Name;

                                    var Node_Stream = SeisWingFunction.FindTreeNode(Node_Station.Nodes, node_name, node_text);
                                    if (Node_Stream == null)
                                    {
                                        Node_Stream = new TreeNode();
                                        Node_Stream = new TreeNode();
                                        Node_Stream.Name = node_name;
                                        Node_Stream.Tag = node_tage;
                                        Node_Stream.Text = node_text;
                                        Node_Station.Nodes.Add(Node_Stream);
                                    }
                                    //if (SL_Connection.Networks[i].Stations[j].Streams[k].Status == NodeStatuss.Warning)
                                    //{
                                    //    Node_Stream.ImageKey = "channel_warning_24.png";
                                    //}
                                    //else if (SL_Connection.Networks[i].Stations[j].Streams[k].Status == NodeStatuss.OK)
                                    //{
                                    //    Node_Stream.ImageKey = "channel_ok_24.png";
                                    //}
                                    Node_Stream.ImageKey = "channel_" + SL_Connection.Networks[i].Stations[j].Chanels[k].Status.ToString().ToLower() + "_24.png";
                                    Node_Stream.SelectedImageKey = Node_Stream.ImageKey;
                                }
                            }
                            else
                            {
                                //Node_Station.ImageIndex = 0;
                                //Node_Station.SelectedImageIndex = 0;
                            }
                        }
                    }
                    else
                    {
                        //Node_Network.ImageIndex = 0;
                        //Node_Network.SelectedImageIndex = 0;
                    }
                }
            }

            if (pertamax)
            {
                pertamax = false;
                for (int i = 1; i < Node_Remote.Nodes.Count; i++)
                {
                    Node_Remote.Nodes[i].Expand();
                }
            }
        }

        private void AddDataFeed_RemoteNew(SL_CONNECTION sl_conn)
        {
            // existing xmlfiles
            Boolean ex_sl_conn_changed = false;
            SL_CONNECTION ex_sl_conn = null;
            var ex_xml_files = Directory.GetFiles(Folder_Datafeed_Remote, "*.xml").ToList();
            var ex_xml_file = String.Empty;

            // cikan station ieu geus aya can?
            foreach (var sl_network in sl_conn.Networks)
            {
                foreach (var sl_station in sl_network.Stations)
                {
                    var new_xml_file = Folder_Datafeed_Remote + sl_conn.ServerAddress + "#" + sl_network.Name + "#" + sl_station.Name + "#" + String.Join(",", sl_station.GetChannelNames()) + ".xml";
                    ex_xml_file = ex_xml_files.Find(q => q == new_xml_file);
                    if (ex_xml_file != null)
                    {
                        ex_sl_conn = class_seedlink.ReadXmlFile(ex_xml_file);
                        var ex_sl_network = ex_sl_conn.Networks.Find(q => q.Name == sl_network.Name);
                        if (ex_sl_network != null)
                        {
                            var ex_sl_station = ex_sl_network.Stations.Find(q => q.Name == sl_station.Name);
                            if (ex_sl_station != null)
                            {
                                foreach (var new_sl_channel in sl_station.Chanels)
                                {
                                    var ex_sl_channel = ex_sl_station.Chanels.Find(q => q.Name == new_sl_channel.Name);
                                    if (ex_sl_channel == null)
                                    {
                                        ex_sl_station.Chanels.Add(new_sl_channel);
                                        ex_sl_conn_changed = true;
                                    }
                                }
                                ex_sl_station.Chanels = ex_sl_station.Chanels.OrderBy(q => q.Name).ToList();
                            }
                        }
                    }
                }
            }

            if (ex_sl_conn != null)
            {
                if (ex_sl_conn_changed)
                {
                    ex_sl_conn.SaveXmlFile();
                }
            }
            else
            {
                ex_sl_conn_changed = true;
                if (String.IsNullOrEmpty(sl_conn.XmlFile))
                {

                    sl_conn.XmlFile = Folder_Datafeed_Remote + sl_conn.ServerAddress + "#" + sl_conn.Networks[0].Name + ";" + sl_conn.Networks[0].Stations[0].Name + ".xml";
                    sl_conn.XmlFile = Folder_Datafeed_Remote + sl_conn.ServerAddress + "#" + sl_conn.Networks[0].Name + "#" + sl_conn.Networks[0].Stations[0].Name + "#" + String.Join(",", sl_conn.Networks[0].Stations[0].GetChannelNames()) + ".xml";
                }
                sl_conn.SaveXmlFile();
            }

            if (ex_sl_conn_changed)
            {
                // asupkeun kanu itu
                AddDataFeed_Remote(sl_conn);

                // AddNewRemoteToInventory
                AddNewRemoteToInventory(sl_conn);
            }
        }

        private void AddDataFeed_Remote(String xml_file)
        {
            AddDataFeed_Remote(class_seedlink.ReadXmlFile(xml_file));
        }

        private void AddDataFeed_Remote(SL_CONNECTION SL_Connection)
        {
            Int32 index_network = -1;
            Int32 index_station = -1;
            Int32 index_stream = -1;

            foreach (var network in SL_Connection.Networks)
            {
                foreach (var station in network.Stations)
                {
                    var sw_monitor = SW_Monitors.Find(q => q.ServerAddress == SL_Connection.ServerAddress && q.NetworkName == network.Name && q.StationName == station.Name);
                    if (sw_monitor == null)
                    {
                        sw_monitor = new SW_MONITOR(this);
                        sw_monitor.Index = SL_BgWorker_Id++;
                        sw_monitor.SL_Connection = SL_Connection;
                        sw_monitor.ServerAddress = SL_Connection.ServerAddress;
                        sw_monitor.NetworkName = network.Name;
                        sw_monitor.StationName = station.Name;
                        sw_monitor.UpdateNetworkList += Ms_monitor_UpdateNetworkList;
                        sw_monitor.StartMonitor();
                        SW_Monitors.Add(sw_monitor);
                        UpdateNode_Remote(sw_monitor, true);
                    }
                }
            }
        }

        private void Ms_monitor_UpdateNetworkList(object sender, EventArgs e)
        {
            UpdateNode_Remote(sender as SW_MONITOR);
        }

        private void AddNewRemoteToInventory(SL_CONNECTION SL_Connection)
        {
            foreach (var sl_network in SL_Connection.Networks)
            {
                CL_INVENTORY ex_inventory = null;
                CL_INVENTORY new_inventory = null;
                CL_INV_NETWORK new_inv_network = null;
                foreach (var ex_inv in Inventories)
                {
                    //foreach (var ex_inv_network in ex_inv.Networks)
                    //{
                    if (ex_inv.Network.Name == sl_network.Name)
                    {
                        ex_inventory = ex_inv;
                        new_inv_network = ex_inv.Network;
                        break;
                    }
                    //}
                }

                if (new_inv_network == null)
                {
                    new_inventory = new CL_INVENTORY();
                    new_inventory.Source = "SeisWing";
                    new_inventory.Sender = "SeisWing";
                    new_inventory.Created = DateTime.UtcNow;
                    new_inventory.SchemaVersion = "1.2";
                    new_inventory.Xmlns = "http://www.fdsn.org/xml/station/1";

                    new_inv_network = new CL_INV_NETWORK();
                    new_inventory.Network = new_inv_network;   //new_inventory.Networks.Add(new_inv_network);
                }
                new_inv_network.Name = sl_network.Name;
                new_inv_network.RestrictedStatus = "open";
                new_inv_network.TimeBegin = DateTime.UtcNow;

                if (new_inv_network.Stations == null)
                {
                    new_inv_network.Stations = new List<CL_INV_STATION>();
                }

                foreach (var sl_station in sl_network.Stations)
                {
                    var inv_station = new_inv_network.Stations.Find(q => q.Name == sl_station.Name);
                    if (inv_station == null)
                    {
                        inv_station = new CL_INV_STATION();
                        new_inv_network.Stations.Add(inv_station);
                    }
                    inv_station.CreationDate = new_inv_network.TimeBegin;
                    inv_station.RestrictedStatus = "open";
                    inv_station.Name = sl_station.Name;
                    inv_station.TimeBegin = new_inv_network.TimeBegin;

                    if (inv_station.Channels == null)
                    {
                        inv_station.Channels = new List<CL_INV_CHANNEL>();
                    }

                    foreach (var sl_channel in sl_station.Chanels)
                    {
                        var inv_channel = inv_station.Channels.Find(q => q.Name == sl_channel.Name);
                        if (inv_channel == null)
                        {
                            inv_channel = new CL_INV_CHANNEL();
                            inv_station.Channels.Add(inv_channel);
                        }
                        inv_channel.Name = sl_channel.Name;
                        inv_channel.LocationCode = sl_channel.LocationName;
                        inv_channel.RestrictedStatus = "open";
                        inv_channel.SensorChannel = 1;
                        inv_channel.TimeBegin = new_inv_network.TimeBegin;

                        if (inv_channel.Response == null)
                        {
                            inv_channel.Response = new CL_INV_RESPONSE();
                        }
                    }
                }

                if (new_inventory != null)
                {
                    new_inventory.Save(FolderRepoInventory);
                    Inventories.Add(new_inventory);
                }
                else
                {
                    ex_inventory.Save(FolderRepoInventory, ex_inventory.XmlFilename);
                }
            }
        }

        private void Menu_item_AddRemoteDataFeed_Click(object sender, EventArgs e)
        {
            frm_Dialog_AddDatafeed_Remote form = new frm_Dialog_AddDatafeed_Remote(this);
            form.StartPosition = FormStartPosition.CenterParent;
            if (form.ShowDialog() == DialogResult.OK)
            {
                AddDataFeed_RemoteNew(form.SL_Connection);
            }
        }

        private void RemoveRemoteDatafeed(String server_name, String network_name, String station_name, SW_MONITOR monitor)
        {
            var SL_Connections = new List<SL_CONNECTION>();
            // teangan file xml aya teu?
            var xml_files = Directory.GetFiles(Folder_Datafeed_Remote);
            if (xml_files.Length > 0)
            {
                foreach (var xml_file in xml_files)
                {
                    SL_Connections.Add(class_seedlink.ReadXmlFile(xml_file));
                }
            }

            if (!String.IsNullOrEmpty(station_name))
            {
                // hupus station
                var slcons = SL_Connections.FindAll(q => q.ServerAddress == server_name);
                foreach (var slcon_server in slcons)
                {
                    var slcon_networks = slcon_server.Networks.FindAll(q => q.Name == network_name);
                    foreach (var slcon_network in slcon_networks)
                    {
                        //var slcon_network = slcon_server.Networks;
                        var slcon_stations = slcon_network.Stations.FindAll(q => q.Name == station_name);
                        if (slcon_network.Stations.Count == 1 /*&& slcon_server.Networks.Count == 1*/)
                        {
                            //langsung hupus
                            File.Delete(slcon_server.XmlFile);
                        }
                        else
                        {
                            RemoveInsideXmlFile(slcon_server.XmlFile, server_name, network_name, station_name);
                        }
                    }
                }
            }
            else if (!String.IsNullOrEmpty(network_name))
            {
                //// hupus network
                //var slcons_server = SL_Connections.FindAll(q => q.ServerAddress == server_name);
                //foreach (var slcon_server in slcons_server)
                //{
                //    var slcon_networks = slcon_server.Networks.FindAll(q => q.Name == network_name);
                //    File.Delete(slcon_server.XmlFile);
                //}
            }
            else
            {
                //hupus server
                var slcons = SL_Connections.FindAll(q => q.ServerAddress == server_name);
                foreach (var slcon in slcons)
                {
                    File.Delete(slcon.XmlFile);
                }

            }
        }

        private void RemoveRemoteDatafeed_Server(TreeNode node_node)
        {
            foreach (TreeNode cnode in node_node.Nodes)
            {
                RemoveRemoteDatafeed_Network(cnode);
            }
            node_node.Parent.Nodes.Remove(node_node);
        }

        private void RemoveRemoteDatafeed_Network(TreeNode node_node)
        {
            foreach (TreeNode cnode in node_node.Nodes)
            {
                RemoveRemoteDatafeed_Station(cnode);
            }
            node_node.Parent.Nodes.Remove(node_node);
        }

        private void RemoveRemoteDatafeed_Station(TreeNode node_node)
        {
            SW_MONITOR monitor = null;
            var index = (node_node.Tag as TreeNodeTag).MonitorIndex;
            monitor = SW_Monitors.Find(q => q.Index == index);

            // hupus monitor
            if (monitor != null)
            {
                if (monitor.f_waveviewer != null)
                {
                    monitor.f_waveviewer.Close();
                    monitor.f_waveviewer = null;
                }
                monitor.StopMonitor();
                SW_Monitors.Remove(monitor);

                node_node.Parent.Nodes.Remove(node_node);

                File.Delete((node_node.Tag as TreeNodeTag).XmlFile);
            }
        }

        private void Bw_Watcher_Remote_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var info = e.UserState as CL_DataFeedRemote_Info;
            if (info.Status == Enum_DataFeedRemote_Status.Created || info.Status == Enum_DataFeedRemote_Status.Existed)
            {
                AddDataFeed_Remote(info.XmlFile);
            }
        }
        #endregion populate_data_feed_remote

        private List<String> GetSelectedFiles(string attrib, string data_path, TreeNode node_node)
        {
            List<String> SelectedFiles = new List<string>();
            foreach (var cnode in node_node.Nodes)
            {
                if (attrib == "all")
                {
                    SelectedFiles.Add((cnode as TreeNode).Tag.ToString());
                }
                else if (attrib == "selected")
                {
                    if ((cnode as TreeNode).Checked)
                    {
                        SelectedFiles.Add((cnode as TreeNode).Tag.ToString());
                    }
                }
            }
            return SelectedFiles;
        }

        private void GetMiniseedInfosInsideNode(TreeNode node, Boolean seleted, ref List<MiniSeedFileInfo> selectedFiles, ChannelCodes channelCode = ChannelCodes.All)
        {
            foreach (TreeNode snode in node.Nodes)
            {
                if (snode.Name.Contains("_FILE"))
                {
                    if (!SeisWingFunction.IsRequiredChannel((snode.Tag as MiniSeedFileInfo).Tag.ChannelName, channelCode))
                    {
                        continue;
                    }

                    if (seleted)
                    {
                        if (snode.Checked)
                        {
                            selectedFiles.Add(snode.Tag as MiniSeedFileInfo);
                        }
                    }
                    else
                    {
                        selectedFiles.Add(snode.Tag as MiniSeedFileInfo);
                    }
                }
                else
                {
                    GetMiniseedInfosInsideNode(snode, seleted, ref selectedFiles, channelCode);
                }
            }
        }

        private void GetFilesInsideNode(TreeNode node, Boolean seleted, ref List<String> selectedFiles)
        {
            foreach (TreeNode snode in node.Nodes)
            {
                if (snode.Name.Contains("_FILE"))
                {
                    if (seleted)
                    {
                        if (snode.IsSelected)
                        {
                            selectedFiles.Add(snode.Tag.ToString());
                        }
                    }
                    else
                    {
                        selectedFiles.Add((snode.Tag as MiniSeedFileInfo).FileName);
                    }
                }
                else
                {
                    GetFilesInsideNode(snode, seleted, ref selectedFiles);
                }
            }
        }

        private void ExecuteDisplayLocalFiles(String path, List<string> SelectedFiles)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            if (SelectedFiles == null || SelectedFiles.Count == 0)
            {
                var msd_files = Directory.GetFiles(path);
                List<Int32> hours = new List<int>();
                foreach (var msd_file in msd_files)
                {
                    var fsdh = lib_miniseed.miniseed_functions.GetHeader(msd_file);
                    if (fsdh != null)
                    {
                        hours.Add(fsdh.start_time.Hour);
                    }
                }

                var hours_unique = new HashSet<int>(hours).ToArray();
                var winstate = FormWindowState.Maximized;
                if (hours_unique.Length > 1)
                {
                    winstate = FormWindowState.Normal;
                }
                foreach (var hour in hours_unique)
                {
                    List<String> files_group = new List<string>();
                    int nh = hours.Count;
                    for (int i = 0; i < nh; i++)
                    {
                        if (hour == hours[i])
                        {
                            files_group.Add(msd_files[i]);
                        }
                    }
                    frm_WaveViewer_Local f_WaveViewer_Local = new frm_WaveViewer_Local(this);
                    f_WaveViewer_Local.DataDirectory = path;
                    f_WaveViewer_Local.MdiParent = this;
                    f_WaveViewer_Local.WindowState = winstate;
                    f_WaveViewer_Local.Show();
                }
            }
            else
            {
                var winstate = FormWindowState.Maximized;
                frm_WaveViewer_Local f_WaveViewer_Local = new frm_WaveViewer_Local(this);
                f_WaveViewer_Local.DataDirectory = path;
                f_WaveViewer_Local.MdiParent = this;
                f_WaveViewer_Local.WindowState = winstate;
                f_WaveViewer_Local.Show();
            }
        }

        private void RemoveServer(TreeNode node, String ServerName, Int32 rec_id)
        {
            ////// hupus node
            ////trv_Menu.Nodes.Remove(node);

            ////// remove xml
            ////var xml_file = Group_Recorder[rec_id].SL_Network.XmlFile;
            ////if (!String.IsNullOrEmpty(xml_file))
            ////{
            ////    if (File.Exists(xml_file))
            ////    {
            ////        File.Delete(xml_file);
            ////    }
            ////}
            ////// recorder
            ////if (Group_Recorder[rec_id].f_waveProcessing != null)
            ////{
            ////    Group_Recorder[rec_id].f_waveProcessing = null;
            ////}
            ////Group_Recorder.RemoveAt(rec_id);
        }

        private void Btn_SaveSettings_Click(object sender, EventArgs e)
        {
            frm_Dialog_SaveTracesOption form = new frm_Dialog_SaveTracesOption(this);
            form.TraceSaveOption = TraceSaveOption;
            if (form.ShowDialog() == DialogResult.OK)
            {
                TraceSaveOption = form.TraceSaveOption;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frm_Dialog_AddNetworkDetails form = new frm_Dialog_AddNetworkDetails(this);
            form.NetworkName = "GE";
            form.StationName = "BBJI";
            form.ChannelName = "BHZ,BHN,BHE";
            form.ShowDialog();
        }

        private void magnitudeCalculationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_MagnitudeCalculation form = new frm_MagnitudeCalculation(this);
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void automaticEventDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_EventDetection_Manual_Dialog form = new Frm_EventDetection_Manual_Dialog(this);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_Setup_Inventory f_setup = new frm_Setup_Inventory(this);
            f_setup.MdiParent = this;
            f_setup.WindowState = FormWindowState.Maximized;
            f_setup.Show();
        }

        private void addNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frm_Setup_AddNetwork f_addnetwork = new frm_Setup_AddNetwork(this);
            //if (f_addnetwork.ShowDialog() == DialogResult.OK)
            //{
            //    var network = Inventory.Networks.Find(q => q.code == f_addnetwork.NetworkName);
            //    if (network == null)
            //    {
            //        network = new Class_Network();
            //        network.code = f_addnetwork.NetworkName;
            //        network.Description = f_addnetwork.NetworkDescription;
            //        Inventory.Networks.Add(network);
            //    }
            //}
        }

        private void addStationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (Inventory == null)
            //{
            //    return;
            //}
            //frm_Setup_SelectNetwork form = new frm_Setup_SelectNetwork(this);
            //if (form.ShowDialog() == DialogResult.OK)
            //{
            //    if (String.IsNullOrEmpty(form.NetworkName))
            //    {
            //        return;
            //    }
            //    var network = Inventory.Networks.Find(q => q.Name == form.NetworkName);
            //    if (network == null)
            //    {
            //        return;
            //    }
            //    frm_Setup_Station f_Setup_AddStation = new frm_Setup_Station(this);
            //    f_Setup_AddStation.NetworkName = network.Name;
            //    if (f_Setup_AddStation.ShowDialog() == DialogResult.OK)
            //    {
            //    }
            //}
        }

        public List<MiniSeedFileInfo> FindMsdInfoInCollection(List<String> network_names, List<String> station_names, List<String> channel_names, TimeWindow timewindow)
        {
            List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();

            foreach (var fileinfo in MiniSeedFileInfos_FILES)
            {
                if (!network_names.Contains(fileinfo.Tag.NetworkName))
                {
                    continue;
                }
                if (!station_names.Contains(fileinfo.Tag.StationName))
                {
                    continue;
                }
                if (!channel_names.Contains(fileinfo.Tag.ChannelName))
                {
                    continue;
                }
                //if ((timewindow.TimeBegin >= fileinfo.Times.TimeBegin && timewindow.TimeBegin <= fileinfo.Times.TimeEnd) || (timewindow.TimeEnd >= fileinfo.Times.TimeBegin && timewindow._endTime <= fileinfo.Times._endTime))
                //{
                //    miniSeedFileInfos.Add(fileinfo);
                //}
                //if (timewindow.TimeBegin >= fileinfo.Times.TimeBegin && timewindow.TimeBegin <= fileinfo.Times.TimeEnd)
                //{
                //    miniSeedFileInfos.Add(fileinfo);
                //}
                if (fileinfo.Times.TimeEnd < timewindow.TimeBegin)
                {
                    continue;
                }
                if (fileinfo.Times.TimeBegin > timewindow.TimeEnd)
                {
                    continue;
                }
                miniSeedFileInfos.Add(fileinfo);
            }
            int ab = 0;
            foreach (var fileinfo in MiniSeedFileInfos_LOCAL)
            {
                if (!network_names.Contains(fileinfo.Tag.NetworkName))
                {
                    continue;
                }
                if (!station_names.Contains(fileinfo.Tag.StationName))
                {
                    continue;
                }
                if (!channel_names.Contains(fileinfo.Tag.ChannelName))
                {
                    continue;
                }
                //if ((timewindow.TimeBegin >= fileinfo.Times.TimeBegin && timewindow.TimeBegin <= fileinfo.Times.TimeEnd) || (timewindow.TimeEnd >= fileinfo.Times.TimeBegin && timewindow._endTime <= fileinfo.Times._endTime))
                //{
                //    miniSeedFileInfos.Add(fileinfo);
                //}
                //if (timewindow.TimeBegin >= fileinfo.Times.TimeBegin && timewindow.TimeBegin <= fileinfo.Times.TimeEnd)
                //{
                //    miniSeedFileInfos.Add(fileinfo);
                //}
                if (fileinfo.Times.TimeEnd < timewindow.TimeBegin)
                {
                    continue;
                }
                if (fileinfo.Times.TimeBegin > timewindow.TimeEnd)
                {
                    continue;
                }
                miniSeedFileInfos.Add(fileinfo);
            }

            //if (channel_names.Count > 0)
            //{
            //    foreach (var channel_name in channel_names)
            //    {
            //        if (channel_name.Length == 1)
            //        {
            //            requexted_infos = miniSeedFileInfos.FindAll(q => q.StreamTag.ChannelName.Substring(2, 1) == channel_name);
            //        }
            //        else
            //        {
            //            requexted_infos = miniSeedFileInfos.FindAll(q => q.StreamTag.ChannelName == channel_name);
            //        }
            //        if (requexted_infos.Count > 0)
            //        {
            //            miniSeedFileInfos_channel.AddRange(requexted_infos);
            //        }
            //    }
            //}
            //else
            //{
            //    miniSeedFileInfos_channel = miniSeedFileInfos;
            //}

            return miniSeedFileInfos;
        }

        //public List<MiniSeedFileInfo> FindMsdInfoInCollection(List<String> network_names, List<String> station_names, List<String> channel_names, TimeWindow timewindow)
        //{
        //    List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
        //    foreach (var network in network_names)
        //    {
        //        foreach (var station in station_names)
        //        {
        //            miniSeedFileInfos.AddRange(FindMsdInfoInCollection(network, station, timewindow));
        //        }
        //    }
        //    return miniSeedFileInfos;
        //}

        public List<MiniSeedFileInfo> FindMsdInfoInCollection(String network_name, String station_name, TimeWindow timewindow)
        {
            List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();

            // nu files
            foreach (var fileinfo in MiniSeedFileInfos_FILES)
            {
                if (fileinfo.Tag.NetworkName != network_name)
                {
                    continue;
                }
                if (fileinfo.Tag.StationName != station_name)
                {
                    continue;
                }
                //if ((timewindow.TimeBegin >= fileinfo.Times.TimeBegin && timewindow.TimeBegin <= fileinfo.Times.TimeEnd) || (timewindow.TimeEnd >= fileinfo.Times.TimeBegin && timewindow._endTime <= fileinfo.Times._endTime))
                //{
                //    miniSeedFileInfos.Add(fileinfo);
                //}
                //if (timewindow.TimeBegin >= fileinfo.Times.TimeBegin && timewindow.TimeBegin <= fileinfo.Times.TimeEnd)
                //{
                //    miniSeedFileInfos.Add(fileinfo);
                //}
                if (fileinfo.Times.TimeEnd < timewindow.TimeBegin)
                {
                    continue;
                }
                if (fileinfo.Times.TimeBegin > timewindow.TimeEnd)
                {
                    continue;
                }
                miniSeedFileInfos.Add(fileinfo);
            }

            ////network heula
            //var node_network = SeisWingFunction.FindTreeNode(Node_Local.Nodes, "LOCAL_NETWORK", network_name);
            //if (node_network != null)
            //{
            //    var node_date = SeisWingFunction.FindTreeNode(node_network.Nodes, "LOCAL_DATE", timewindow.TimeBegin.ToString("yyyyMMdd"));
            //    if (node_date != null)
            //    {
            //        var node_station = SeisWingFunction.FindTreeNode(node_date.Nodes, "LOCAL_STATION", station_name);
            //        if (node_station != null)
            //        {
            //            foreach (TreeNode node_channel in node_station.Nodes)
            //            {
            //                foreach (TreeNode node_file in node_channel.Nodes)
            //                {
            //                    var fileinfo = node_file.Tag as MiniSeedFileInfo;
            //                    //if ((timewindow.StartTime >= fileinfo.Times.StartTime && timewindow.StartTime <= fileinfo.Times._endTime) || (timewindow._endTime >= fileinfo.Times.StartTime && timewindow._endTime <= fileinfo.Times._endTime))
            //                    //{
            //                    //    miniSeedFileInfos.Add(fileinfo);
            //                    //}
            //                    //if (timewindow.StartTime >= fileinfo.Times.StartTime && timewindow.StartTime <= fileinfo.Times._endTime)
            //                    //{
            //                    //    miniSeedFileInfos.Add(fileinfo);
            //                    //}
            //                    if (fileinfo.Times.TimeEnd < timewindow.TimeBegin)
            //                    {
            //                        continue;
            //                    }
            //                    if (fileinfo.Times.TimeBegin > timewindow.TimeEnd)
            //                    {
            //                        continue;
            //                    }
            //                    miniSeedFileInfos.Add(fileinfo);
            //                }
            //            }
            //        }
            //    }
            //}
            return miniSeedFileInfos;
        }

        public List<MiniSeedFileInfo> FindMsdInfoInTree(List<String> stations, TimeWindow timewindow)
        {
            List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
            //var node_network = SeisWingFunction.FindTreeNode(Node_Local.Nodes, "LOCAL_NETWORK", pick._waveformID.NetworkName);
            //if (node_network != null)
            //{
            //    var node_date = SeisWingFunction.FindTreeNode(node_network.Nodes, "LOCAL_DATE", pick._time._value.ToString("yyyyMMdd"));
            //    if (node_date != null)
            //    {
            //        var node_station = SeisWingFunction.FindTreeNode(node_date.Nodes, "LOCAL_STATION", pick._waveformID.StationName);
            //        if (node_station != null)
            //        {
            //            var node_channel = SeisWingFunction.FindTreeNode(node_station.Nodes, "LOCAL_CHANNEL", pick._waveformID.ChannelName);
            //            if (node_channel != null)
            //            {
            //                // teangan file-na
            //                //var timewindow = new TimeWindow();
            //                //timewindow.StartTime = new DateTimeUing(pick._time._value.Ticks - 10 * 60 * 1000);
            //                //timewindow._endTime = pick._time._value.AddMinutes(10);
            //                foreach (TreeNode node_file in node_channel.Nodes)
            //                {
            //                    var fileinfo = node_file.Tag as MiniSeedFileInfo;
            //                    if (timewindow.StartTime.Ticks >= fileinfo.Times.StartTime.Ticks && timewindow.StartTime.Ticks <= fileinfo.Times._endTime.Ticks)
            //                    {
            //                        miniSeedFileInfos.Add(fileinfo);
            //                    }
            //                    ////var header = miniseed_functions.GetHeaderFromFile(fileinfo);
            //                    //if (timewindow.StartTime.Ticks-10*60*1000 < fileinfo.Times.StartTime.Ticks && timewindow._endTime.Ticks + 10 * 60 * 1000 > fileinfo.Times.StartTime.Ticks)
            //                    //{
            //                    //    miniSeedFileInfos.Add(fileinfo);
            //                    //    //msd_files.Add(fileinfo);
            //                    //}
            //                }
            //            }
            //        }
            //    }
            //}
            return miniSeedFileInfos;
        }

        public List<MiniSeedFileInfo> FindMsdInfoInTree(CL_EP_PICK pick, TimeWindow timewindow)
        {
            List<MiniSeedFileInfo> miniSeedFileInfos = new List<MiniSeedFileInfo>();
            var node_network = SeisWingFunction.FindTreeNode(Node_Local.Nodes, "LOCAL_NETWORK", pick.WaveformTag.NetworkName);
            if (node_network != null)
            {
                var node_date = SeisWingFunction.FindTreeNode(node_network.Nodes, "LOCAL_DATE", pick.Time.Value.ToString("yyyyMMdd"));
                if (node_date != null)
                {
                    var node_station = SeisWingFunction.FindTreeNode(node_date.Nodes, "LOCAL_STATION", pick.WaveformTag.StationName);
                    if (node_station != null)
                    {
                        var node_channel = SeisWingFunction.FindTreeNode(node_station.Nodes, "LOCAL_CHANNEL", pick.WaveformTag.ChannelName);
                        if (node_channel != null)
                        {
                            // teangan file-na
                            //var timewindow = new TimeWindow();
                            //timewindow.StartTime = new DateTimeUing(pick._time._value.Ticks - 10 * 60 * 1000);
                            //timewindow._endTime = pick._time._value.AddMinutes(10);
                            foreach (TreeNode node_file in node_channel.Nodes)
                            {
                                var fileinfo = node_file.Tag as MiniSeedFileInfo;
                                if (timewindow.TimeBegin.Ticks >= fileinfo.Times.TimeBegin.Ticks && timewindow.TimeBegin.Ticks <= fileinfo.Times.TimeEnd.Ticks)
                                {
                                    miniSeedFileInfos.Add(fileinfo);
                                }
                                ////var header = miniseed_functions.GetHeaderFromFile(fileinfo);
                                //if (timewindow.StartTime.Ticks-10*60*1000 < fileinfo.Times.StartTime.Ticks && timewindow._endTime.Ticks + 10 * 60 * 1000 > fileinfo.Times.StartTime.Ticks)
                                //{
                                //    miniSeedFileInfos.Add(fileinfo);
                                //    //msd_files.Add(fileinfo);
                                //}
                            }
                        }
                    }
                }
            }
            return miniSeedFileInfos;
        }

        private void viewRawPicksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ViewPicks f_viewPicks = new frm_ViewPicks(this);
            f_viewPicks.MdiParent = this;
            f_viewPicks.WindowState = FormWindowState.Maximized;
            f_viewPicks.PickType = "RAW";
            f_viewPicks.Show();
        }

        private void viewCommonPicksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ViewPicks f_viewPicks = new frm_ViewPicks(this);
            f_viewPicks.MdiParent = this;
            f_viewPicks.WindowState = FormWindowState.Maximized;
            f_viewPicks.PickType = "COMMON";
            f_viewPicks.Show();
        }

        private void viewEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ViewEvents f_viewEvents = new frm_ViewEvents(this);
            f_viewEvents.MdiParent = this;
            f_viewEvents.WindowState = FormWindowState.Maximized;
            f_viewEvents.Show();
        }

        public void LoadInventory()
        {
            var inventory_files = Directory.GetFiles(FolderRepoInventory, "*.xml");
            foreach (var invfile in inventory_files)
            {
                var inventory = InventoryFunctions.Read_FDSN_Xml(invfile);
                Inventories.Add(inventory);
            }
        }

        public void SaveInventory()
        {
            ////////// save inventory
            ////////if (Inventory != null&& Inventory.Changed)
            ////////{
            ////////    // save inventory
            ////////    Inventory.XmlFilename = FolderRepoInventory + "inv.xml";
            ////////    InventoryFunctions.SaveInventory(Inventory, true);
            ////////}
        }

        private void frm_parent_FormClosing(object sender, FormClosingEventArgs e)
        {
            //hupus file di temp/seiswing/traces
            if (Directory.Exists(FolderTemp + "traces"))
            {
                Directory.Delete(FolderTemp + "traces", true);
            }

            SaveInventory();
        }

        private void plotEventsOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ViewEvents_Map f_viewEventsOnMap = new frm_ViewEvents_Map(this);
            f_viewEventsOnMap.MdiParent = this;
            f_viewEventsOnMap.WindowState = FormWindowState.Maximized;
            f_viewEventsOnMap.Show();
            //MainForm mainForm = new MainForm();
            //mainForm.MdiParent = this;
            //mainForm.WindowState = FormWindowState.Maximized;
            //mainForm.Show();
        }

        private void setupNetworkStationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_Setup_Inventory f_setup = new frm_Setup_Inventory(this);
            f_setup.MdiParent = this;
            f_setup.WindowState = FormWindowState.Maximized;
            f_setup.Show();
        }

        private void phasePickingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_PhasePickingManual f_manualPhasePicking = new frm_PhasePickingManual(this);
            f_manualPhasePicking.MdiParent = this;
            f_manualPhasePicking.WindowState = FormWindowState.Maximized;
            f_manualPhasePicking.Show();
        }

        private void eventLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_EventLocation f_eventLocation = new frm_EventLocation(this);
            f_eventLocation.MdiParent = this;
            f_eventLocation.WindowState = FormWindowState.Maximized;
            f_eventLocation.Show();
        }

        private void spectraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_ViewDataSpectra f_ViewDataSpectra = new frm_ViewDataSpectra(this);
            f_ViewDataSpectra.MdiParent = this;
            f_ViewDataSpectra.WindowState = FormWindowState.Maximized;
            f_ViewDataSpectra.Show();
        }

        private void compareSpectraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_Process_Spectra_Compare f_Process_Spectra_Compare = new frm_Process_Spectra_Compare(this);
            f_Process_Spectra_Compare.MdiParent = this;
            f_Process_Spectra_Compare.WindowState = FormWindowState.Maximized;
            f_Process_Spectra_Compare.Show();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            velocityModelToolStripMenuItem_Click(null, null);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            frm_Setup_Velmod_New f_Setup_Velmod_New = new frm_Setup_Velmod_New(this);
            f_Setup_Velmod_New.StartPosition = FormStartPosition.CenterParent;
            f_Setup_Velmod_New.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            CL_EP ep = new CL_EP(@"C:\\Users\\awan\\AppData\\Local\\Temp\\~SeisWing\\cpicks\\20200923T063504Z582.xml");
            string inv_file = @"C:\Users\awan\AppData\Local\SeisWing\inventories\inv.xml";
            string strout = string.Empty;

            //////HYPO71 hypo71 = new HYPO71();
            //////hypo71.EventParameter = ep;
            //////hypo71.FolderTempEvents = FolderEvents;
            //////hypo71.FolderTempHypo71 = FolderHypo71;
            //////hypo71.Hypo71_Exe_Source = Application.StartupPath + "\\Hypo71\\HYPO71PC.exe";
            //////hypo71.Inventory = Inventory;
            //////hypo71.VelocityModelToUse = VelocityModelInUse;
            //////hypo71.EvaluationMode = EvaluationModes.MANUAL;
            //////hypo71.Status = OriginStatus.REFINED;
            //////hypo71.locate();

            StreamWriter sw = new StreamWriter(@"C:\Users\awan\AppData\Local\SeisWing\hypo71\HYPO71PC.INP");
            sw.Write(strout);
            sw.Close();
        }

        private void velocityModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_Setup_Velmod f_Setup_Velmod = new frm_Setup_Velmod(this);
            f_Setup_Velmod.StartPosition = FormStartPosition.CenterParent;
            if (f_Setup_Velmod.ShowDialog() == DialogResult.OK)
            {
                VelocityModelInUse = f_Setup_Velmod.SelectedVelocityModel;

                VelocityModelInUseName = VelocityModelInUse.Name;
                VelocityModelInUseFileName = VelocityModelInUse.FileName;

                // set velocity model ku ieu
                StreamWriter sw = new StreamWriter(VelocityModelSettingFile, false);
                sw.WriteLine(VelocityModelInUseName);
                sw.WriteLine(VelocityModelInUseFileName);
                sw.Close();
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ConfigAutopick autopick_config = new ConfigAutopick();
            autopick_config.Filename = @"C:\Users\awan\AppData\Local\SeisWing\seiscomp3\scautopick.cfg";
            autopick_config.Read();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            //var parent = this;
            //var ep = new CL_EP();
            //List<CommonPicks> commonPicks;
            //var CommonPickFiles = new List<String>();
            //var RawPickFiles = Directory.GetFiles(@"C:\Users\awan\AppData\Local\Temp\~SeisWing\rpicks").ToList();
            //var MinStationsDetectEvent = 3;
            //TimeWindow ProcessingTime = new TimeWindow();
            //ProcessingTime.TimeBegin = new DateTimeUing(2020, 12, 9);
            //ProcessingTime.TimeEnd = new DateTimeUing(2020, 12, 10);

            ////commonPicks = ep.GetCommonPicksFromRaws(Directory.GetFiles(parent.FolderTempRawPick).ToList(), ProcessingTime, false);
            //commonPicks = ep.GetCommonPicksFromRaws(RawPickFiles, ProcessingTime, false, MinStationsDetectEvent);
            //if (commonPicks != null)
            //{
            //    // ayeuna bikin eventparameterna keur autoloc
            //    for (Int32 k = 0; k < commonPicks.Count; k++)
            //    {
            //        var ccommonPick = commonPicks[k];
            //        var eventParameter = new CL_EP();
            //        eventParameter.XmlFilename = parent.FolderResult_PicksCommon + ccommonPick.FisrtPickTime.ToStringFileName() + ".xml";
            //        eventParameter.Amplitudes = ccommonPick.Amplitudes;
            //        eventParameter.Picks = ccommonPick.Picks;
            //        eventParameter.Save();
            //        CommonPickFiles.Add(eventParameter.XmlFilename);
            //    }
            //    //uC_EventDetermination.ProcessEventLocation(eventParameterFiles);
            //}
            //else
            //{
            //    commonPicks = ep.GetCommonPicks(Directory.GetFiles(parent.FolderResult_PicksCommon).ToList(), ProcessingTime);
            //    //commonPicks = ep.GetCommonPicks(PickFiles);
            //    // ayeuna bikin eventparameterna keur autoloc
            //    //List<String> eventParameterFiles = new List<String>();
            //    for (Int32 k = 0; k < commonPicks.Count; k++)
            //    {
            //        var ccommonPick = commonPicks[k];
            //        //if (ccommonPick.FisrtPickTime == null)
            //        //{
            //        //    continue;
            //        //}
            //        var eventParameter = new CL_EP();
            //        eventParameter.XmlFilename = parent.FolderResult_PicksCommon + ccommonPick.FisrtPickTime.ToStringFileName() + ".xml";
            //        eventParameter.Amplitudes = ccommonPick.Amplitudes;
            //        eventParameter.Picks = ccommonPick.Picks;
            //        //eventParameter.SaveToXml();
            //        CommonPickFiles.Add(eventParameter.XmlFilename);
            //    }
            //}
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_License f_License = new Frm_License(this);
            f_License.StartPosition = FormStartPosition.CenterParent;
            if (f_License.ShowDialog() == DialogResult.OK)
            {
                IsLicensed = CheckLicense();
            }
        }

        private void servicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_Setup_Service form = new frm_Setup_Service(this);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void addRemoteDatafeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Menu_item_AddRemoteDataFeed_Click(null, null);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_Dialog_Settings f_Dialog_Settings = new frm_Dialog_Settings(this);
            f_Dialog_Settings.StartPosition = FormStartPosition.CenterParent;
            if (f_Dialog_Settings.ShowDialog() == DialogResult.OK)
            {
                // save settings
                Settings.FileSettingsWr();

                // if timezone changed
                if (f_Dialog_Settings.TimeZoneChanged)
                {
                    // mun aya waveform viewer, change
                    foreach (var child in MdiChildren)
                    {
                        if (child is frm_WaveViewer_Remote)
                        {
                            (child as frm_WaveViewer_Remote).ClearWaveforms(f_Dialog_Settings.TimeZoneChangedValue);
                        }
                    }
                }
            }
        }

        private void Tsm_PeakValues_Click(object sender, EventArgs e)
        {
            // mun aya waveform viewer, change
            foreach (var child in MdiChildren)
            {
                if (child is frm_WaveViewer_Remote)
                {
                    (child as frm_WaveViewer_Remote).DisplayProcessingPeak();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Frm_Process_Picks form = new Frm_Process_Picks(this);
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }
    }
}