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

        public Form1()
        {
            //foreach (var item in liters) comboBox1.Items.Add(item.ToString());
            
            InitializeComponent();
        }

        private void SelectPicture(object sender, EventArgs args)
        {
            //OpenFileDialog ofd = new OpenFileDialog();

            //if (ofd.ShowDialog() == DialogResult.Cancel)
            //    return;

            //try
            //{
                //pictureBox1.Image = Image.FromFile(ofd.FileName);
                //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                if (pictureBox1.Image != null)
                {
                    button3.Enabled = !button3.Enabled;
                    button4.Enabled = !button4.Enabled;
                }
                label2.Text = $"File: none";
                nw = new NeiroWeb();
                Learn();
            //}
            //catch
            //{
            //    MessageBox.Show("Загружено не изображение", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
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
            //DialogResult askResult = MessageBox.Show("Результат распознавания - " + s + " ?", "", MessageBoxButtons.YesNo);
            //if (askResult != DialogResult.Yes || !enableTraining || MessageBox.Show("Добавить этот образ в память нейрона '" + s + "'", "", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            nw.SetTraining(s, arr);
            // очищаем рисунки
            //NeiroGraphUtils.ClearImage(pictureBox2);
            //NeiroGraphUtils.ClearImage(pictureBox3);
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
    }
}
