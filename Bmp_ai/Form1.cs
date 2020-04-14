using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bmp_ai
{
    public partial class Form1 : Form
    {
        private NeiroWeb nw;
        private int[,] arr;
        private Point startP;
        public string path = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void SelectPicture_file(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;
            pictureBox4.Image = Image.FromFile(ofd.FileName);
        }

            private void SelectPicture(object sender, EventArgs args)
        {
            if (pictureBox1.Image != null)
            {
                button3.Enabled = !button3.Enabled;
                button4.Enabled = !button4.Enabled;
            }
            label2.Text = $"File: none";
            Learn();
        }

        private void Learn()
        {
            int[,] clipArr = NeiroGraphUtils.CutImageToArray((Bitmap)pictureBox1.Image, new Point(pictureBox1.Width, pictureBox1.Height));

            //int[,] clipArr = NeiroGraphUtils.CutImageToArray((Bitmap)pictureBox4.Image, new Point(pictureBox4.Width, pictureBox4.Height));

            if (clipArr == null)
                return;

            arr = NeiroGraphUtils.LeadArray(clipArr, new int[NeiroWeb.neironInArrayWidth, NeiroWeb.neironInArrayHeight]);
            pictureBox3.Image = NeiroGraphUtils.GetBitmapFromArr(clipArr);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = NeiroGraphUtils.GetBitmapFromArr(arr);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            string s = nw.CheckLitera(arr);
            if (s == null)
                s = "idk";

            label7.Text = $"{s}";
        }

        private void AddSymbolToList(string symbol)
        {
            if (symbol == null || symbol.Length == 0)
            {
                MessageBox.Show("Значение не может иметь длину 0 символов.");
                return;
            }
            MessageBox.Show($"Образ для символа: {symbol} добавлен.");
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) Learn();
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point endP = new Point(e.X, e.Y);
                Bitmap image = (Bitmap)pictureBox1.Image;
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.DrawLine(new Pen(Color.BlueViolet), startP, endP);
                }
                pictureBox1.Image = image;
                startP = endP;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startP = new Point(e.X, e.Y);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = !textBox1.Enabled;
            button2.Enabled = !button2.Enabled;
            button3.Enabled = !button3.Enabled;
            button4.Enabled = !button4.Enabled;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string litera = textBox1.Text;
                textBox1.Enabled = !textBox1.Enabled;
                button2.Enabled = !button2.Enabled;

                if (comboBox1.Items.Contains(textBox1.Text))
                {
                    AddSymbolToList(textBox1.Text);
                    nw.SetTraining(textBox1.Text, arr);
                    nw.SaveState();
                    picturebox_clean();
                }
                else
                {
                    if (litera.Length == 0)
                    {
                        return;
                    }
                    comboBox1.Items.Add(litera);
                    comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                    nw.SetTraining(litera, arr);
                    nw.SaveState();
                    MessageBox.Show($"Символ {textBox1.Text} добавлен в память.");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string litera = label7.Text;
            nw.SetTraining(litera, arr);
            nw.SaveState();
            picturebox_clean();
            button3.Enabled = !button3.Enabled;
            button4.Enabled = !button4.Enabled;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NeiroGraphUtils.ClearImage(pictureBox1);
            nw = new NeiroWeb();
            string[] items = nw.GetLiteras();
            if (items.Length > 0)
            {
                comboBox1.Items.AddRange(items);
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            picturebox_clean();
        }

        private void button5_patch_for_train(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                label6.Text = FBD.SelectedPath;
            }
        }

        private void button6_train(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            nw.SaveState();
        }

        public void picturebox_clean()
        {
            NeiroGraphUtils.ClearImage(pictureBox1);
            NeiroGraphUtils.ClearImage(pictureBox2);
            NeiroGraphUtils.ClearImage(pictureBox3);
        }
    }
}
