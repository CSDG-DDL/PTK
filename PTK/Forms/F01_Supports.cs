using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTK.Forms
{
    public partial class F01_Supports : Form
    {
        #region fields
        private CheckBox[] supArray = new CheckBox[6];
        string boolSupString;
        bool[] boolSupArray = new bool[6];
        #endregion

        #region constructors
        public F01_Supports(string _boolSupString)
        {
            InitializeComponent();
            boolSupString = _boolSupString;
            setValues();
            this.DialogResult = DialogResult.Cancel;
            this.CancelButton = button2;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Icon = PTK.Properties.Resources.formico_support;
        }
        #endregion

        #region properties
        public string BoolSupString 
        {
            get { return boolSupString; }
            set { boolSupString = value; }
        }
        #endregion

        #region methods
        // loading event
        private void setValues()
        {
            boolSupArray = Support.ConditionsStringToArray(boolSupString);

            checkBox1.Checked = boolSupArray[0];
            checkBox2.Checked = boolSupArray[1];
            checkBox3.Checked = boolSupArray[2];
            checkBox4.Checked = boolSupArray[3];
            checkBox5.Checked = boolSupArray[4];
            checkBox6.Checked = boolSupArray[5];
            
        }

        

        private void F01_Supports_Load(object sender, EventArgs e)
        {

        }

        private void F01_Supports_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        // "OK" button
        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true) { boolSupArray[0] = true; } else boolSupArray[0] = false;
            if (checkBox2.Checked == true) { boolSupArray[1] = true; } else boolSupArray[1] = false;
            if (checkBox3.Checked == true) { boolSupArray[2] = true; } else boolSupArray[2] = false;
            if (checkBox4.Checked == true) { boolSupArray[3] = true; } else boolSupArray[3] = false;
            if (checkBox5.Checked == true) { boolSupArray[4] = true; } else boolSupArray[4] = false;
            if (checkBox6.Checked == true) { boolSupArray[5] = true; } else boolSupArray[5] = false;

            BoolSupString = "";
            for (int i = 0; i < 6; i++)
            {
                int tempVal;
                if (boolSupArray[i] == true) tempVal = 1;
                else tempVal = 0;
                BoolSupString += tempVal.ToString();
            }

            this.DialogResult = DialogResult.OK;
        }

        // "Cancel" button
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
