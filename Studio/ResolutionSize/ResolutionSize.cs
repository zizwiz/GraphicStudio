using System.Drawing;
using System.Windows.Forms;

namespace Studio.ResolutionSized
{
    class ResolutionSize
    {
        private static Bitmap OriginalBitmap;

        public static void open_RESZ_Click()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    OpenFileDialog OFD = new OpenFileDialog();
                    OFD.Filter = "Picture files | *.bmp; *.jpg; *.gif; *.png; *.tif | All Files | *.* ";
                    OFD.Title = "Open an Image File";

                    if (OFD.ShowDialog() == DialogResult.OK)
                    {
                        OriginalBitmap = new Bitmap(OFD.FileName);
                        frmTemp.picbx_image_RESZ.Image = OriginalBitmap;
                        using (Graphics gr = Graphics.FromImage(OriginalBitmap))
                        {
                            frmTemp.txtbx_dpiX_RESZ.Text = gr.DpiX.ToString();
                            frmTemp.txtbx_dpiY_RESZ.Text = gr.DpiY.ToString();
                        }

                        frmTemp.txtbx_width_RESZ.Text = OriginalBitmap.Width.ToString();
                        frmTemp.txtbx_height_RESZ.Text = OriginalBitmap.Height.ToString();
                    }
                }
            }
        }



        public static void save_RESZ_Click()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    SaveFileDialog SFD = new SaveFileDialog();
                    SFD.Filter = "Picture files | *.bmp; *.jpg; *.gif; *.png; *.tif | All Files | *.* ";
                    SFD.Title = "Save an Image File";

                    if ((SFD.ShowDialog() == DialogResult.OK) && (frmTemp.picbx_image_RESZ.Image!=null))
                    {
                        int new_wid = int.Parse(frmTemp.txtbx_width_RESZ.Text);
                        int new_hgt = int.Parse(frmTemp.txtbx_height_RESZ.Text);

                        using (Bitmap bm = new Bitmap(new_wid, new_hgt))
                        {
                            Point[] points =
                            {
                                new Point(0, 0),
                                new Point(new_wid, 0),
                                new Point(0, new_hgt),
                            };
                            using (Graphics gr = Graphics.FromImage(bm))
                            {
                                gr.DrawImage(OriginalBitmap, points);
                            }

                            float dpix = float.Parse(frmTemp.txtbx_dpiX_RESZ.Text);
                            float dpiy = float.Parse(frmTemp.txtbx_dpiY_RESZ.Text);
                            bm.SetResolution(dpix, dpiy);
                            Common.SaveImage(bm, SFD.FileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Perhaps no image to save");
                    }
                }
            }
        }
    }
}
