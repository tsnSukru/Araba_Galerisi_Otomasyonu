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
    public partial class Form1 : Form
    {
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\arabagalerisi.mdb");
        DataSet ds = new DataSet();
        BindingSource bs = new BindingSource();
        public Form1()
        {
            InitializeComponent();
        }
        void verileri_cek()
        {
            string sec = "select * from marka"; //yıldız hepsini çek demek
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["marka"] != null) ds.Tables["marka"].Clear();
            da.Fill(ds, "marka");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            verileri_cek();
            bs.DataSource = ds.Tables["marka"];
            dataGridView1.DataSource = bs;
            tbmarkaadi.DataBindings.Add("Text", bs, "markaadi");
            tbmarkakodu.DataBindings.Add("Text", bs, "markakodu");

        }

        private void btntemizle_Click(object sender, EventArgs e)
        {
            tbmarkaadi.Clear();
            tbmarkakodu.Clear();
        }

        private void btnekle_Click(object sender, EventArgs e)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "insert into marka (markaadi) Values (@markakodu)";
            cmd.Parameters.AddWithValue("markaadi", tbmarkaadi.Text);
            cmd.ExecuteNonQuery();

            verileri_cek();
        }

        private void btnsil_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Kaydı Silmek İstiyor musunuz?", "Uyarı!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (cevap == DialogResult.Yes)
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = "delete from marka where markakodu=@markakodu";
                cmd.Parameters.AddWithValue("@markakodu", tbmarkakodu.Text);
                cmd.ExecuteNonQuery();

                verileri_cek();
            }
        }

        private void btnduzenle_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Düzenlemek İstediğinize Eminmisiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (cevap == DialogResult.Yes)
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = "update marka set markaadi=@markaadi where markakodu=@markakodu";
                cmd.Parameters.AddWithValue("@markaadi", tbmarkaadi.Text);
                cmd.Parameters.AddWithValue("@markakodu", tbmarkakodu.Text);
                cmd.ExecuteNonQuery();
            }

            verileri_cek();
        }
    }
}
