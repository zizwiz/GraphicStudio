using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Studio.Adjustments
{
    class Brightness
    {
        // The original image.
        private static Bitmap OriginalImage;

        // The rotated image.
        private static Bitmap AdjustedImage;

        // The reset image.
        private static Bitmap ResetImage;

        public static bool Brightflag = false;


        public static void open_ADJ_Click()
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
                        OriginalImage = Common.LoadBitmapUnlocked(OFD.FileName);
                        AdjustedImage = OriginalImage.Clone() as Bitmap;
                        ResetImage = OriginalImage.Clone() as Bitmap;

                        frmTemp.picbx_original_ADJ.Image = OriginalImage;
                        frmTemp.picbx_adjustments_ADJ.Image = OriginalImage;

                        frmTemp.hscrollbar_brightness_ADJ.Value = 100;
                        frmTemp.hscrollbar_gamma_ADJ.Value = 7;

                        frmTemp.hscrollbar_red_ADJ.Value = 128;
                        frmTemp.hscrollbar_green_ADJ.Value = 128;
                        frmTemp.hscrollbar_blue_ADJ.Value = 128;
                        frmTemp.picbx_colour.BackColor = Color.FromArgb(128, 128, 128);


                        AdjustBrightness();
                    }
                }
            }
        }

        public static void save_ADJ_Click()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    SaveFileDialog SFD = new SaveFileDialog();
                    SFD.Filter = "Picture files | *.bmp; *.jpg; *.gif; *.png; *.tif | All Files | *.* ";
                    SFD.Title = "Save an Image File";

                    if ((SFD.ShowDialog() == DialogResult.OK) && (frmTemp.picbx_adjustments_ADJ.Image != null))
                    {

                        Common.SaveImage(frmTemp.picbx_adjustments_ADJ.Image, SFD.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("Perhaps no image to save");
                }
            }
        }

        public static void reset_ADJ_Click()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    frmTemp.picbx_original_ADJ.Image = ResetImage;
                    frmTemp.picbx_adjustments_ADJ.Image = ResetImage;

                    frmTemp.hscrollbar_brightness_ADJ.Value = 100; 
                    frmTemp.hscrollbar_gamma_ADJ.Value = 7;

                    frmTemp.hscrollbar_red_ADJ.Value = 128;
                    frmTemp.hscrollbar_green_ADJ.Value = 128;
                    frmTemp.hscrollbar_blue_ADJ.Value = 128;
                    frmTemp.picbx_colour.BackColor = Color.FromArgb(128, 128, 128);

                    Brightflag = false;
                    Gamma.Gammaflag = false;
                    Colourise.Colourflag = false;

                    AdjustBrightness();
                }
            }
        }







        // Perform the brightness adjustment and display the result.
        public static void AdjustBrightness()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    //  lblBrightness.Text = "Brightness = " + (frmTemp.hscrollbar_brightness_ADJ.Value / 100.0);

                    if ((Gamma.Gammaflag)||(Colourise.Colourflag))
                    {
                        frmTemp.picbx_original_ADJ.Image = frmTemp.picbx_adjustments_ADJ.Image;
                        Gamma.Gammaflag = false;
                        Colourise.Colourflag = false;
                    }

                    Brightflag = true;
                    frmTemp.picbx_adjustments_ADJ.Image =
                        AdjustBrightness(frmTemp.picbx_original_ADJ.Image, (float)(frmTemp.hscrollbar_brightness_ADJ.Value / 100.0));
                }
            }
        }

        // Adjust the image's brightness.
        private static Bitmap AdjustBrightness(Image image, float brightness)
        {
            // Make the ColorMatrix.
            float b = brightness;
            ColorMatrix cm = new ColorMatrix(new float[][]
            {
                new float[] {b, 0, 0, 0, 0},
                new float[] {0, b, 0, 0, 0},
                new float[] {0, 0, b, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1},
            });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(cm);

            // Draw the image onto the new bitmap while applying the new ColorMatrix.
            Point[] points =
            {
                new Point(0, 0),
                new Point(image.Width, 0),
                new Point(0, image.Height),
            };
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            // Make the result bitmap.
            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
            }

            // Return the result.
            return bm;
        }
    }
}
