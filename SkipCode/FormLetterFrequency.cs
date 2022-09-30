using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SkipCode
{
    public partial class FormLetterFrequency : Form
    {
        public FormLetterFrequency()
        {
            InitializeComponent();
        }

        private void FormLetterFrequency_Load(object sender, EventArgs e)
        {
            this.chart1.Series[0].Name = ("Letter Frequency");

            DataTable source = (DataTable)this.dataGridView1.DataSource;

            this.chart1.Series[0].Name = ("Letter Frequency [%]");
            this.chart1.Series[0].Points.DataBindXY(source.Rows, "Character", source.Rows, "Quotient");
            this.chart1.Series[0].ChartType = SeriesChartType.Column;
            
            this.chart1.Series[0]["DrawingStyle"] = "Default";
            this.chart1.Series[0]["PointWidth"] = "0.6";

            this.chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
            this.chart1.ChartAreas[0].AxisX.Interval = 1;
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            DataTable source = (this.dataGridView1.DataSource as DataTable).Clone();

            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (row.DataBoundItem != null)
                {
                    DataRow r = (row.DataBoundItem as DataRowView).Row;
                    source.ImportRow(r);
                }
            }

            this.chart1.Series[0].Points.DataBindXY(source.Rows, "Character", source.Rows, "Quotient");    
        }
    }
}
