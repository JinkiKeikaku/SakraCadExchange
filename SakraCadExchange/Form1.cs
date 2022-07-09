using System.Text;

namespace SakraCadExchange
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new OpenFileDialog();
            d.Filter = "SakraCad drawing files|*.skc|All files|*.*";
            if (d.ShowDialog() != DialogResult.OK) return;
            OpenFile(d.FileName);
        }

        private void OpenFile(String path)
        {
            try
            {
                if (Path.GetExtension(path) == ".skc")
                {
                    var doc = new SakraCadHelper.SkcDocument();
                    doc.Read(path);
                    WriteToTextBox(doc);
                    //�ǂݍ��ݐ����B���s�͗�O�őΏ��B
                    //mLayers = reader.Layers;
                    //mHeader = reader.Header;
                    //LayerNameToTextBox(reader);
                    ////DrawContext�͕\�����鎞�Ɏg�����ێ��I�u�W�F�N�g�B
                    //DrawContext = new DrawContext(reader.Header);
                    ////�X�N���[���o�[�Ȃ񂩂̐ݒ�B
                    //CalcSize();
                    //panel1�𖳌�������panel1��paint���Ă΂��B
                    panel1.Invalidate();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error");
//                DrawContext = null;
                panel1.Invalidate();
            }
        }



        void WriteToTextBox(SakraCadHelper.SkcDocument doc)
        {
            var sw = new StringWriter();
            doc.Write(sw);
            textBox1.Text= sw.ToString();
        }


    }
}