using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace arabagalerisi
{
    public partial class Form2 : Form
    {
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\arabagalerisi.mdb");
        DataSet ds = new DataSet();
        BindingSource bs = new BindingSource();
        public Form2()
        {
            InitializeComponent();
        }

        void verileri_cek()
        {
            string sec = "select mo.*,ma.markaadi from model as mo, marka as ma where mo.markakodu=ma.markakodu";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["model"] != null) ds.Tables["model"].Clear();
            da.Fill(ds, "model");
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            verileri_cek();

            bs.DataSource = ds.Tables["model"];
            dataGridView1.DataSource = bs;

            tbmodeladi.DataBindings.Add("Text", bs, "modeladi");
            tbmodelkodu.DataBindings.Add("Text", bs, "modelkodu");
            cbmarkaadi.DataBindings.Add("Text", bs, "markaadi");

            string sec = "select * from marka";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            da.Fill(ds, "marka");

            cbmarkaadi.DataSource = ds.Tables["marka"];
            cbmarkaadi.DisplayMember = "markaadi";
            cbmarkaadi.ValueMember = "markakodu";
        }

        private void btnekle_Click(object sender, EventArgs e)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "insert into model (modeladi,markakodu) values (@modeladi,@markakodu)";
            cmd.Parameters.AddWithValue("model", tbmodeladi.Text);
            cmd.Parameters.AddWithValue("markakodu", cbmarkaadi.SelectedValue);

            cmd.ExecuteNonQuery();

            verileri_cek();
        }

        private void btnsil_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Kayıt Silinsin mi?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (cevap == DialogResult.Yes)
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = "delete from model where modelkodu=@modelkodu";
                cmd.Parameters.AddWithValue("modelkodu", tbmodelkodu.Text);

                cmd.ExecuteNonQuery();

                verileri_cek();
            }
        }

        private void btntemizle_Click(object sender, EventArgs e)
        {
            tbmodeladi.Clear(); tbmodelkodu.Clear(); cbmarkaadi.SelectedIndex = -1;
        }

        private void btnduzenle_Click(object sender, EventArgs e)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            cmd.CommandText = "update model set modeladi=@modeladi where modelkodu=@modelkodu";
            cmd.Parameters.AddWithValue("modeladi", tbmodeladi.Text);
            cmd.Parameters.AddWithValue("modelkodu", tbmodelkodu.Text);

            cmd.ExecuteNonQuery();

            verileri_cek();
        }
    }
}
