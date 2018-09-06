using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.GUI;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;


namespace PTK
{
    public partial class ParaCoorForm : Form
    {
        private ParallelCoordinatesViewerComponent comp;

        //レジストリの書き換え用変数
        const string FEATURE_BROWSER_EMULATION = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
        const string FEATURE_BROWSER_EMULATION64 = @"Software\Wow6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
        Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(FEATURE_BROWSER_EMULATION);

        string process_name = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
        string process_dbg_name = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".vshost.exe";


        public ParaCoorForm()
        {
            InitializeComponent();
        }
        public ParaCoorForm(ParallelCoordinatesViewerComponent _comp)
        {
            InitializeComponent();
            Owner = Grasshopper.Instances.DocumentEditor;
            comp = _comp;

            //フォームで使うIEのバージョンを上げるため、レジストリの書き換え
            regkey.SetValue(process_name, 11001, Microsoft.Win32.RegistryValueKind.DWord);
            regkey.SetValue(process_dbg_name, 11001, Microsoft.Win32.RegistryValueKind.DWord);

            //選択データの受信クラス実体化
            webBrowserForParaCood.ObjectForScripting = new ReceiveChoiceData();

            //フォームにグラフ用HTMLを表示
            //string curDir = Directory.GetCurrentDirectory();
            string curDir = Grasshopper.Folders.AppDataFolder;
            Uri u = new Uri(curDir);
            string path = u.AbsoluteUri + "Libraries/ParaCoorFormItem/Viewer.html";
            //webBrowserForParaCood.Navigate(new Uri("file:///"+ curDir + "Librarues/ParaCoorFormItem/Viewer.html"));
            webBrowserForParaCood.Navigate(path);

            //Stream st = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            //webBrowserForParaCood.DocumentStream = st;
        }

        //-------Disable GH's canvas (becomes a white background)
        private void OwnerCanvasEnable(bool IsEnable)
        {
            GH_DocumentEditor ghCanvas = Owner as GH_DocumentEditor;
            if (IsEnable)
                ghCanvas.EnableUI();
            else
                ghCanvas.DisableUI();
        }

        private void ParaCoorForm_Shown(object sender, EventArgs e)
        {
            OwnerCanvasEnable(false);
        }

        //-------Reset form when closing form
        protected override void OnClosed(EventArgs e)
        {
            OwnerCanvasEnable(true);
            base.OnClosed(e);
            ParaCoorOption.paraCoorFrom = null;
        }
        private void ParaCoorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //regkey.DeleteValue(process_name);
            //regkey.DeleteValue(process_dbg_name);
            regkey.Close();
        }

        //HTMLファイルが読み込まれたら実行
        private void webBrowserForParaCood_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            List<string> jsons = comp.ParetoSolutions.ConvertAll(p => p.ToJSON());
            string json = "[" + string.Join(",", jsons) + "]";
            webBrowserForParaCood.Document.InvokeScript("updateData", new string[] { json });
        }

        //選択データ受信用クラス
        [System.Runtime.InteropServices.ComVisible(true)]
        public class ReceiveChoiceData
        {
            public void ReceiveChoiceDataFunction(string data)
            {
                ParaCoorOption.SetParamFromForm(data);
            }
        }

        private void ParaCoorForm_Load(object sender, EventArgs e)
        {

        }
    }

   
}
