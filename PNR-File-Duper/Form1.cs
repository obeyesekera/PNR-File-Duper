using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace PNR_File_Gen
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private string xDocFile = "";
        private string pnrPath = "";

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Title = "Select Source PNR File";
            ofd.DefaultExt = "xml";
            ofd.Filter = "PNR files (*.xml)|*.xml|All files (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lblSourceFile.Text = ofd.SafeFileName;
                xDocFile = ofd.FileName;
                pnrPath = Path.GetDirectoryName(xDocFile);

                btnLoad.Enabled = false;
                fieldEnabler(true);
            }
        }

        private void fieldEnabler(bool status)
        {
            txtFirstFlight.Enabled = status;
            txtFlightCount.Enabled = status;
            txtFlightPrefix.Enabled = status;
            dtpArrivalTime.Enabled = status;
            dtpDepartureTime.Enabled = status;
            txtDelayTime.Enabled = status;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            btnLoad.Enabled = false;
            btnGenerate.Enabled = false;
            fieldEnabler(false);
                        
            int startFlifht = int.Parse(txtFirstFlight.Text);
            int flightCount = int.Parse(txtFlightCount.Text);
            int endFlight = startFlifht + flightCount;
            int delayTime = int.Parse(txtDelayTime.Text);

            DateTime newArrivalTime = dtpArrivalTime.Value;
            DateTime newDepartureTime = dtpDepartureTime.Value;

            for (int i = startFlifht; i < endFlight; i++)
            {
                if (i > startFlifht)
                {
                    newArrivalTime = newArrivalTime.AddMinutes(delayTime);
                    newDepartureTime = newDepartureTime.AddMinutes(delayTime);
                }

                string newFlight = txtFlightPrefix.Text + i;
                string newArrivalTimeString = newArrivalTime.ToString("yyyy-MM-dd")+ "T" +newArrivalTime.ToString("HH:mm:ss");
                string newDepartureTimeString = newDepartureTime.ToString("yyyy-MM-dd") + "T" + newDepartureTime.ToString("HH:mm:ss");

                generateNewXML(newFlight, newArrivalTimeString, newDepartureTimeString);
            }

            MessageBox.Show("Completed");
            Application.Exit();

        }

        private void generateNewXML(string nFlight, string nArrival, string nDeparture)
        {
            try
            {
                string fileLoc = pnrPath;
                XmlDocument doc = new XmlDocument();
                doc.Load(xDocFile);

                changeXMLVal(doc, "FlightNumber", nFlight);
                changeXMLVal(doc, "ArrivalDateTime", nArrival);
                changeXMLVal(doc, "DepartureDateTime", nDeparture);

                doc.Save(pnrPath + "\\" + nFlight + ".xml");
                doc = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                /*
                 * Possible Exceptions:
                 *  System.ArgumentException
                 *  System.ArgumentNullException
                 *  System.InvalidOperationException
                 *  System.IO.DirectoryNotFoundException
                 *  System.IO.FileNotFoundException
                 *  System.IO.IOException
                 *  System.IO.PathTooLongException
                 *  System.NotSupportedException
                 *  System.Security.SecurityException
                 *  System.UnauthorizedAccessException
                 *  System.UriFormatException
                 *  System.Xml.XmlException
                 *  System.Xml.XPath.XPathException
                */
            }
        }

        private XmlDocument changeXMLVal(XmlDocument doc, string element, string value)
        {
            XmlNode node = doc.SelectSingleNode("/root/FlightDetails/" + element);
            if (node != null)
            {
                node.InnerText = value;
            }
            else
            {
                XmlNode root = doc.DocumentElement;
                XmlElement elem;
                elem = doc.CreateElement(element);
                elem.InnerText = value;
                root.AppendChild(elem);
            }

            return doc;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            lblSourceFile.Text = "Source file not selected";
            btnGenerate.Enabled = false;
            fieldEnabler(false);
        }

        private void txtFlightPrefix_TextChanged(object sender, EventArgs e)
        {
            validateGenerateBtn();
        }

        private void txtFirstFlight_TextChanged(object sender, EventArgs e)
        {
            validateGenerateBtn();
        }

        private void txtFlightCount_TextChanged(object sender, EventArgs e)
        {
            validateGenerateBtn();
        }

        private void validateGenerateBtn()
        {
            if ((txtFlightPrefix.TextLength > 0)&&
                (txtFirstFlight.TextLength > 0)&&
                (txtFlightCount.TextLength > 0)&&
                (txtDelayTime.TextLength > 0))
            {
                btnGenerate.Enabled = true;
            }
            else
            {
                btnGenerate.Enabled = false;
            }
        }

        private void txtGroundTime_TextChanged(object sender, EventArgs e)
        {
            validateGenerateBtn();
        }

        private void txtDelayTime_TextChanged(object sender, EventArgs e)
        {
            validateGenerateBtn();
        }
    }
}
