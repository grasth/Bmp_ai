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

        private void SelectPicture(object sender, EventArgs args)
        {
            if (pictureBox1.Image != null)
            {
                button3.Enabled = !button3.Enabled;
                button4.Enabled = !button4.Enabled;
            }
            label2.Text = $"File: none";
            nw = new NeiroWeb();
            Learn();
        }

        private void Learn()
        {
            int[,] clipArr = NeiroGraphUtils.CutImageToArray((Bitmap)pictureBox1.Image, new Point(pictureBox1.Width, pictureBox1.Height));
            if (clipArr == null) return;
            arr = NeiroGraphUtils.LeadArray(clipArr, new int[NeiroWeb.neironInArrayWidth, NeiroWeb.neironInArrayHeight]);
            pictureBox3.Image = NeiroGraphUtils.GetBitmapFromArr(clipArr);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = NeiroGraphUtils.GetBitmapFromArr(arr);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            string s = nw.CheckLitera(arr);
            if (s == null) s = "null";
            label5.Text = $"Result: {s}";
            nw.SetTraining(s, arr);
        }

        private void AddSymbolToList(string symbol)
        {
            if (symbol == null || symbol.Length == 0)
            {
                MessageBox.Show("Значение не может иметь длину 0 символов.");
                return;
            }
            comboBox1.Items.Add(symbol);
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            MessageBox.Show("Сейчас значение '" + symbol + "' в списке, теперь можно научить нейросеть сеть его распознавать.");
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
                AddSymbolToList(textBox1.Text);
                textBox1.Enabled = !textBox1.Enabled;
                button2.Enabled = !button2.Enabled;
                string litera = textBox1.Text;
                if (litera.Length == 0)
                {
                    return;
                }
                nw.SetTraining(litera, arr);
                nw.SaveState();
                NeiroGraphUtils.ClearImage(pictureBox1);
                NeiroGraphUtils.ClearImage(pictureBox2);
                NeiroGraphUtils.ClearImage(pictureBox3);
                MessageBox.Show("Выбранный символ '" + litera + "' успешно добавлен в память сети");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = !button3.Enabled;
            button4.Enabled = !button4.Enabled;
            string litera = comboBox1.SelectedIndex >= 0 ? (string)comboBox1.Items[comboBox1.SelectedIndex] : comboBox1.Text;
            nw.SaveState();
            nw.SetTraining(litera, arr);
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
            NeiroGraphUtils.ClearImage(pictureBox1);
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
    }
}
