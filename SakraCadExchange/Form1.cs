using System.Text;

namespace SakraCadExchange
{
    public partial class Form1 : Form
    {
        DrawContext? DrawContext = null;
        SakraCadHelper.SkcDocument? mDoc = null;

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
//                    var ps = SakraCadHelper.SkcDocument.GetPageCount(path);
                    mDoc = new SakraCadHelper.SkcDocument();
                    mDoc.Read(path);
                    WriteToTextBox(mDoc);
                    //読み込み成功。失敗は例外で対処。
                    //mLayers = reader.Layers;
                    //mHeader = reader.Header;
                    //LayerNameToTextBox(reader);
                    //DrawContextは表示する時に使う情報保持オブジェクト。
                    double w = mDoc.PaperInfo.Width;
                    double h = mDoc.PaperInfo.Height;
                    if(mDoc.PaperInfo.Horz == 0)
                    {
                        (w, h) = (h, w);
                    }
                    DrawContext = new DrawContext(w, h);
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            if (DrawContext == null) return;
            var saved = g.Save();
            g.TranslateTransform(
                (float)(panel1.AutoScrollPosition.X),
                (float)(panel1.AutoScrollPosition.Y)
            );
            g.ScaleTransform(DrawContext.TranslateScale, DrawContext.TranslateScale);
            var drawer = new SakraCadDrawer(mDoc);
            drawer.OnDraw(g, DrawContext);
            g.Restore(saved);

        }
    }
}