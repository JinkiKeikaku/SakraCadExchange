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
                    //読み込み成功。失敗は例外で対処。
                    //mLayers = reader.Layers;
                    //mHeader = reader.Header;
                    //LayerNameToTextBox(reader);
                    ////DrawContextは表示する時に使う情報保持オブジェクト。
                    //DrawContext = new DrawContext(reader.Header);
                    ////スクロールバーなんかの設定。
                    //CalcSize();
                    //panel1を無効化してpanel1のpaintが呼ばれる。
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