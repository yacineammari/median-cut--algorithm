using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Median_cut
{
    public partial class Form1 : Form
    {
        public mc mc;
        public int nb_color = 2;
        public Form1()
        {
            InitializeComponent();
        }
        

        private void importerUneImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                comboBox1.Text = "--Select--";
                Bitmap o_img = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = o_img;

                mc = new mc(o_img);



                List<mpix> img_data = new List<mpix>();
                for (int x = 0; x < o_img.Width; x++){
                    for (int y = 0; y < o_img.Height; y++){
                        img_data.Add(new mpix(o_img.GetPixel(x, y), x, y));
                    }
                }

                


                mc.nb_of_distanc_val = (from x in img_data select x).Distinct().Count();


                int possible = 1;
                comboBox1.Items.Clear();
                while (possible *2 <= mc.nb_of_distanc_val)
                {
                    possible = possible * 2;
                    comboBox1.Items.Add(possible);

                }


            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nb_color = Int32.Parse(comboBox1.Text);
            Console.WriteLine(nb_color);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mc.img == null)
            {
                MessageBox.Show("vous devez importer un image.", "pas d\'image", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                mc.create_main_list();
                Cursor.Current = Cursors.WaitCursor;
                mc.nb_color = nb_color;
                pictureBox2.Image = mc.median_cut();
                Cursor.Current = Cursors.Default;
            }
       
        }
    }
}
