using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Studio.TransparentiseImage
{
    class Transparentise
    {

        private const int Offset = 10;
        private static Bitmap BmOriginal;
        private static Bitmap BmMakeTransparent;
        private static Bitmap BmTransparentified;

        public static void OpenFile_T()
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Picture files | *.bmp; *.jpg; *.gif; *.png; *.tif | All Files | *.* ";
            OFD.Title = "Open an Image File";

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                BmOriginal = Common.LoadBitmapUnlocked(OFD.FileName);
                BmMakeTransparent = new Bitmap(BmOriginal);
                BmTransparentified = new Bitmap(BmOriginal);
                ShowSamples();
            }
        }

        public static void SaveFile_T()
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "Picture files | *.bmp; *.jpg; *.gif; *.png; *.tif | All Files | *.* ";
            SFD.Title = "Save an Image File";
            
            if (SFD.ShowDialog() == DialogResult.OK)
            {
               Common.SaveImage(BmTransparentified, SFD.FileName);
            }
        }

        
        public static void ResetFile_T()
        {
            if (MessageBox.Show("Discard changes?", "Discard changes?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                BmMakeTransparent = new Bitmap(BmOriginal);
                BmTransparentified = new Bitmap(BmOriginal);
                ShowSamples();
            }
        }


        public static void ExpandTransparency_T()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    frmTemp.Cursor = Cursors.WaitCursor;

                    float max_dist = float.Parse(frmTemp.txtbx_MaxDist.Text);
                    BmTransparentified =
                        ExpandTransparency(BmTransparentified, max_dist);
                    ShowSamples();

                    frmTemp.Cursor = Cursors.Default;
                }
            }
        }

        public static void ChooseColour_T(MouseEventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    int x = e.X - Offset;
                    int y = e.Y - Offset;
                    Color color = BmOriginal.GetPixel(x, y);
                    BmMakeTransparent.MakeTransparent(color);

                    int dr = int.Parse(frmTemp.txtbx_Dr_T.Text);
                    int dg = int.Parse(frmTemp.txtbx_Dg_T.Text);
                    int db = int.Parse(frmTemp.txtbx_Db_T.Text);
                    BmTransparentified = Transparentify(
                        BmTransparentified, x, y, dr, dg, db);

                    ShowSamples();
                }
            }
        }

       
        private static void ShowSamples()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    Bitmap bm;

                    bm = MakeBackground();
                    using (Graphics gr = Graphics.FromImage(bm))
                    {
                        gr.DrawImage(BmOriginal, Offset, Offset);
                    }

                    frmTemp.picbx_Original_t.Image = bm;

                    bm = MakeBackground();
                    using (Graphics gr = Graphics.FromImage(bm))
                    {
                        gr.DrawImage(BmMakeTransparent, Offset, Offset);
                    }

                    frmTemp.picbx_MakeTransparent_t.Image = bm;

                    bm = MakeBackground();
                    using (Graphics gr = Graphics.FromImage(bm))
                    {
                        gr.DrawImage(BmTransparentified, Offset, Offset);
                    }

                    frmTemp.picBx_Transparentified_T.Image = bm;

                    frmTemp.picbx_Original_t.Visible = true;
                    frmTemp.picbx_MakeTransparent_t.Visible = true;
                    frmTemp.picBx_Transparentified_T.Visible = true;
                }
            }
        }


        private static Bitmap MakeBackground()
        {
            int width = BmOriginal.Width + 2 * Offset;
            int height = BmOriginal.Height + 2 * Offset;
            Bitmap bm = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                using (LinearGradientBrush brush =
                    new LinearGradientBrush(new Point(0, 0),
                        new Point(64, 64), Color.Blue, Color.Yellow))
                {
                    gr.FillRectangle(brush, 0, 0, width, height);
                }
            }

            return bm;
        }


        // Make pixels that are near transparent ones partly transparent.
        private static Bitmap ExpandTransparency(Bitmap input_bm, float max_dist)
        {
            Bitmap result_bm = new Bitmap(input_bm);

            float[,] distances =
                GetDistancesToTransparent(input_bm, max_dist);

            int width = input_bm.Width;
            int height = input_bm.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // If this pixel is transparent, skip it.
                    if (input_bm.GetPixel(x, y).A == 0)
                        continue;

                    // See if this pixel is near a transparent one.
                    float distance = distances[x, y];
                    if (distance > max_dist) continue;
                    float scale = distance / max_dist;

                    Color color = input_bm.GetPixel(x, y);
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;

                    int a = (int)(255 * scale);
                    color = Color.FromArgb(a, r, g, b);
                    result_bm.SetPixel(x, y, color);
                }
            }
            return result_bm;
        }

        // Return an array showing how far each
        // pixel is from a transparent one.
        private static float[,] GetDistancesToTransparent(
            Bitmap bm, float max_dist)
        {
            int width = bm.Width;
            int height = bm.Height;
            float[,] distances = new float[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    distances[x, y] = float.PositiveInfinity;

            // Examine pixels.
            int dxmax = (int)max_dist;
            if (dxmax < max_dist) dxmax++;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // See if this pixel is transparent.
                    if (bm.GetPixel(x, y).A == 0)
                    {
                        for (int dx = -dxmax; dx <= dxmax; dx++)
                        {
                            int px = x + dx;
                            if ((px < 0) || (px >= width)) continue;
                            for (int dy = -dxmax; dy <= dxmax; dy++)
                            {
                                int py = y + dy;
                                if ((py < 0) || (py >= height)) continue;
                                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                                if (distances[px, py] > dist)
                                    distances[px, py] = dist;
                            }
                        }
                    }
                }
            }
            return distances;
        }

        // Make the indicated pixel's color transparent.
        private static Bitmap Transparentify(Bitmap bm_input,
            int x, int y, int dr, int dg, int db)
        {
            // Get the target color's components.
            Color target_color = bm_input.GetPixel(x, y);
            byte r = target_color.R;
            byte g = target_color.G;
            byte b = target_color.B;

            // Make a copy of the original bitmap.
            Bitmap bm = new Bitmap(bm_input);

            // Make a stack of points that we need to visit.
            Stack<Point> points = new Stack<Point>();

            // Make an array to keep track of where we've been.
            int width = bm_input.Width;
            int height = bm_input.Height;
            bool[,] added_to_stack = new bool[width, height];

            // Start at the target point.
            points.Push(new Point(x, y));
            added_to_stack[x, y] = true;
            bm.SetPixel(x, y, Color.Transparent);

            // Repeat until the stack is empty.
            while (points.Count > 0)
            {
                // Process the top point.
                Point point = points.Pop();

                // Examine its neighbors.
                for (int i = point.X - 1; i <= point.X + 1; i++)
                {
                    for (int j = point.Y - 1; j <= point.Y + 1; j++)
                    {
                        // If the point (i, j) is outside
                        // of the bitmap, skip it.
                        if ((i < 0) || (i >= width) ||
                            (j < 0) || (j >= height)) continue;

                        // If we have already considred
                        // this point, skip it.
                        if (added_to_stack[i, j]) continue;

                        // Get this point's color.
                        Color color = bm_input.GetPixel(i, j);

                        // See if this point's RGB vlues are with
                        // the allowed ranges.
                        if (Math.Abs(r - color.R) > dr) continue;
                        if (Math.Abs(g - color.G) > dg) continue;
                        if (Math.Abs(b - color.B) > db) continue;

                        // Add the point to the stack.
                        points.Push(new Point(i, j));
                        added_to_stack[i, j] = true;
                        bm.SetPixel(i, j, Color.Transparent);
                    }
                }
            }

            // Return the new bitmap.
            return bm;
        }
    }
}
