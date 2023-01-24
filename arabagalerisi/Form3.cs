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
    public partial class Form3 : Form
    {
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\arabagalerisi.mdb");
        DataSet ds = new DataSet(); //dataset oluşturduk
        BindingSource bs = new BindingSource();
        string düzeltileceknumara;
        public Form3()
        {
            InitializeComponent();
        }

        void verileri_cek()
        {
            string sec = "select a.*,ma.markaadi,mo.modeladi from arabalar as a, marka as ma, model as mo where a.markakodu=ma.markakodu and a.modelkodu=mo.modelkodu";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["arabalar"] != null) ds.Tables["arabalar"].Clear();
            da.Fill(ds, "arabalar");             
        }
      
        void veritabanina_bagla()
        {
            tbnumara.DataBindings.Add("Text", bs, "arabakodu");
            cbmarka.DataBindings.Add("Text", bs, "markaadi");
            cbmodel.DataBindings.Add("Text", bs, "modeladi");
            tbkm.DataBindings.Add("Text", bs, "km");
            tbfiyat.DataBindings.Add("Text", bs, "fiyat");
            tbhasar.DataBindings.Add("Text", bs, "hasarkaydi");
            pictureBox1.DataBindings.Add("ImageLocation", bs, "fotograf");
        }

         bool kayit_varmi(string aranannumara)
        {
            string sec = "select * from arabalar where arabakodu='" + aranannumara + "'";
            OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
            if (ds.Tables["aranan"] != null) ds.Tables["aranan"].Clear();
            da.Fill(ds, "aranan");
            if (ds.Tables["aranan"].Rows.Count <= 0) return true;
            else return false;
        }
        void ilerigerikontrol(object sender, EventArgs e)
        {
            if (bs.Count -1 == bs.Position)
            {
                btnileri.Enabled = false;
            }
            else btnileri.Enabled = true;
            if (bs.Position == 0)
            {
                btngeri.Enabled = false;
            }
            else btngeri.Enabled = true;
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            string seckomutu = "select * from marka";
            OleDbDataAdapter da = new OleDbDataAdapter(seckomutu, conn);
            da.Fill(ds, "marka"); da.Fill(ds, "marka1");    //datasetin marka table'ına doldursun
            cbmarka.DataSource = ds.Tables["marka"];
            cbmarka.DisplayMember = "markaadi";
            cbmarka.ValueMember = "markakodu"; //buraya kadar olan cbmarka combobox'ını doldurmak için gereken kodlar
            
            cbmarkaara.DataSource = ds.Tables["marka1"];
            cbmarkaara.DisplayMember = "markaadi";
            cbmarkaara.ValueMember = "markakodu";
            
            cbmarka_SelectedIndexChanged(sender, e);

            verileri_cek();
            bs.DataSource = ds.Tables["arabalar"];
            dataGridView1.DataSource = bs;

            veritabanina_bagla();
            ilerigerikontrol(sender, e);
            bs.PositionChanged += new EventHandler(ilerigerikontrol);

            dataGridView1_SelectionChanged(sender, e);
        }

        private void cbmarka_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string seckomutu = "select * from model where markakodu =" + cbmarka.SelectedValue;
                OleDbDataAdapter da = new OleDbDataAdapter(seckomutu, conn);
                if (ds.Tables["model"] != null) ds.Tables["model"].Clear();//ds tables eğer doluysa temizle
                da.Fill(ds, "model");
                cbmodel.DataSource = ds.Tables["model"];
                cbmodel.DisplayMember = "modeladi";
                cbmodel.ValueMember = "modelkodu"; //buraya kadar olan kodcbmodel comboboxlarını doldurur
            }
            catch { }
        }

        private void btntemizle_Click(object sender, EventArgs e)
        {
            tbnumara.Clear(); tbfiyat.Clear(); tbhasar.Clear(); tbkm.Clear(); cbmarka.SelectedIndex = -1; cbmodel.SelectedIndex = -1; pictureBox1.Image = null;     
        }
        private void btnekle_Click(object sender, EventArgs e)
        {
            if (kayit_varmi(tbnumara.Text))
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = "insert into arabalar (arabakodu, markakodu, modelkodu, km, fiyat, fotograf, hasarkaydi) values (@arabakodu, @markakodu, @modelkodu, @km, @fiyat, @fotograf, @hasarkaydi)";
                cmd.Parameters.AddWithValue("@arabakodu", tbnumara.Text);
                cmd.Parameters.AddWithValue("@markakodu", cbmarka.SelectedValue);
                cmd.Parameters.AddWithValue("@modelkodu", cbmodel.SelectedValue);
                cmd.Parameters.AddWithValue("@km", tbkm.Text);
                cmd.Parameters.AddWithValue("@fiyat", tbfiyat.Text);
                cmd.Parameters.AddWithValue("@fotograf", pictureBox1.ImageLocation);
                cmd.Parameters.AddWithValue("@hasarkaydi", tbhasar.Text);
                cmd.ExecuteNonQuery();
                verileri_cek();
            }
            else
            {
                MessageBox.Show("Bu numaralı kayıt zaten var!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnduzenle_Click(object sender, EventArgs e)
        { 
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "update arabalar set arabakodu=@arabakodu, markakodu=@markakodu, modelkodu=@modelkodu, km=@km, fiyat=@fiyat, fotograf=@fotograf, hasarkaydi=@hasarkaydi where arabakodu=@tutarabakodu";
            cmd.Parameters.AddWithValue("@arabakodu", tbnumara.Text);
            cmd.Parameters.AddWithValue("@markakodu", cbmarka.SelectedValue);
            cmd.Parameters.AddWithValue("@modelkodu", cbmodel.SelectedValue);
            cmd.Parameters.AddWithValue("@km", tbkm.Text);
            cmd.Parameters.AddWithValue("@fiyat", tbfiyat.Text);
            cmd.Parameters.AddWithValue("@fotograf", pictureBox1.ImageLocation);
            cmd.Parameters.AddWithValue("@hasarkaydi", tbhasar.Text);
            cmd.Parameters.AddWithValue("@tutarabakodu", düzeltileceknumara);
            cmd.ExecuteNonQuery();
            verileri_cek();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory=Application.StartupPath + "\\resim\\";
            DialogResult cevap = openFileDialog1.ShowDialog();
            if (cevap == DialogResult.OK)
            {
                pictureBox1.ImageLocation = openFileDialog1.FileName;
            }
        }

        private void btnsil_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Kaydı Silmek İstediğinize Emin misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (cevap == DialogResult.Yes)
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = "delete from arabalar where arabakodu=@arabakodu";
                cmd.Parameters.AddWithValue("arabakodu", tbnumara.Text);
                cmd.ExecuteNonQuery();
                verileri_cek();
            }
        }

        private void cbmarkaara_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sec = "select a.*, ma.markaadi, mo.modeladi from arabalar as a, marka as ma, model as mo where a.markakodu=ma.markakodu and a.modelkodu=mo.modelkodu and a.markakodu=" + cbmarkaara.SelectedValue;
                OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
                if (ds.Tables["arabalar"] != null) ds.Tables["arabalar"].Clear();
                da.Fill(ds, "arabalar");
            }
            catch { }
        }

        private void btnileri_Click(object sender, EventArgs e)
        {
            bs.Position++;
        }

        private void btngeri_Click(object sender, EventArgs e)
        {
            bs.Position--;
        }

        private void tbnumaraara_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string sec = "select a.*, ma.markaadi, mo.modeladi from arabalar as a, marka as ma, model as mo where a.markakodu=ma.markakodu and a.modelkodu=mo.modelkodu and a.arabakodu like '%" + tbnumaraara.Text + "%'";
                OleDbDataAdapter da = new OleDbDataAdapter(sec, conn);
                if (ds.Tables["arabalar"] != null) ds.Tables["arabalar"].Clear();
                da.Fill(ds, "arabalar");
            }
            catch
            {

            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            düzeltileceknumara = tbnumara.Text;
        }
    }
}
