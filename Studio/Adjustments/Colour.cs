using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Studio.Adjustments
{
    class Colourise
    {
        public static bool Colourflag = false;

        // Color the picture.
        public static void ColourPicture()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;

                    if ((Gamma.Gammaflag) || (Brightness.Brightflag))
                    {
                        frmTemp.picbx_original_ADJ.Image = frmTemp.picbx_adjustments_ADJ.Image;
                        Gamma.Gammaflag = false;
                        Brightness.Brightflag = false;
                    }

                    frmTemp.picbx_colour.BackColor = Color.FromArgb(frmTemp.hscrollbar_red_ADJ.Value, frmTemp.hscrollbar_green_ADJ.Value, frmTemp.hscrollbar_blue_ADJ.Value);
                    frmTemp.picbx_adjustments_ADJ.Image = ToColourTone(frmTemp.picbx_original_ADJ.Image, frmTemp.picbx_colour.BackColor);

                    Colourflag = true;

                }
            }
        }

        // Convert an image to sepia tone.
        private static Bitmap ToColourTone(Image image, Color colour)
        {
            Bitmap bm = null;

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;

                    float scale = (frmTemp.hscrollbar_brightness_ADJ.Value / 128f); // * (float)(2.56);

                    float r = colour.R / 255f * scale;
                    float g = colour.G / 255f * scale;
                    float b = colour.B / 255f * scale;

                    // Make the ColorMatrix.
                    ColorMatrix cm = new ColorMatrix(new float[][]
                    {
                        new float[] {r, 0, 0, 0, 0},
                        new float[] {0, g, 0, 0, 0},
                        new float[] {0, 0, b, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(cm);

                    // Draw the image onto the new bitmap while applying the new ColorMatrix.
                    Point[] points =
                    {
                        new Point(0, 0),
                        new Point(image.Width - 1, 0),
                        new Point(0, image.Height - 1),
                    };
                    Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                    // Make the result bitmap.
                    bm = new Bitmap(image.Width, image.Height);
                    using (Graphics gr = Graphics.FromImage(bm))
                    {
                        gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
                    }
                }
            }

            // Return the result.
                    return bm;
               
            
        }
    }
}
