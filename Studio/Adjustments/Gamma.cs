using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Studio.Adjustments
{
    class Gamma
    {
        public static bool Gammaflag = false;

        // Perform the gamma adjustment and display the result.
        public static void AdjustGamma()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;
                    //lblGamma.Text = "Gamma = " + (scrGamma.Value / 10.0).ToString();

                    if ((Brightness.Brightflag)||(Colourise.Colourflag))
                    {
                        frmTemp.picbx_original_ADJ.Image = frmTemp.picbx_adjustments_ADJ.Image;
                        Brightness.Brightflag = false;
                        Colourise.Colourflag = false;
                    }

                    Gammaflag = true;
                    frmTemp.picbx_adjustments_ADJ.Image =
                        AdjustGamma(frmTemp.picbx_original_ADJ.Image, (float) (frmTemp.hscrollbar_gamma_ADJ.Value / 10.0));
                }
            }
        }

        // Perform gamma correction on the image.
        public static Bitmap AdjustGamma(Image image, float gamma)
        {
            // Set the ImageAttributes object's gamma value.
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetGamma(gamma);

            // Draw the image onto the new bitmap while applying the new gamma value.
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
