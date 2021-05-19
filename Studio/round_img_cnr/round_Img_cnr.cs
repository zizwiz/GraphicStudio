using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Studio.Properties;

namespace Studio.round_img_cnr
{
    class round_Img_cnr
    {
        private static Bitmap OriginalImage;

        public static void RC_Initialise()
        {

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;


                    frmTemp.txtbx_x_radius_rc.Text = frmTemp.scrXRadius_rc.Value.ToString();
                    frmTemp.txtbx_y_radius_rc.Text = frmTemp.scrYRadius_rc.Value.ToString();

                    OriginalImage = new Bitmap(Resources.abc);
                    frmTemp.picbx_image_rc.Image = OriginalImage;
                    frmTemp.picbx_image_rc.Visible = true;
                    ShowImage();

                }
            }
        }

        public static void OpenImage_RC()
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
                        frmTemp.picbx_image_rc.Image = OriginalImage;
                        frmTemp.picbx_image_rc.Visible = true;
                        ShowImage();
                    }

                }
            }
        }


        // Make and display the image with rounded corners.
        public static void ShowImage()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    // If the corners are not rounded,
                    // just use the original image.
                    if ((frmTemp.scrXRadius_rc.Value == 0) || (frmTemp.scrYRadius_rc.Value == 0))
                    {
                        frmTemp.picbx_image_rc.Image = OriginalImage;
                        return;
                    }

                    // Make a bitmap of the proper size.
                    int width = OriginalImage.Width;
                    int height = OriginalImage.Height;
                    Bitmap bm = new Bitmap(width, height);
                    using (Graphics gr = Graphics.FromImage(bm))
                    {
                        // Clear with a transparent background.
                        gr.Clear(Color.Transparent);
                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                        gr.InterpolationMode = InterpolationMode.High;

                        // Make the rounded rectangle path.
                        GraphicsPath path = MakeRoundedRect(
                            new Rectangle(0, 0, width, height),
                            frmTemp.scrXRadius_rc.Value, frmTemp.scrYRadius_rc.Value,
                            true, true, true, true);

                        // Fill with the original image.
                        using (TextureBrush brush = new TextureBrush(OriginalImage))
                        {
                            gr.FillPath(brush, path);
                        }
                    }

                    frmTemp.picbx_image_rc.Image = bm;
                }
            }
        }



        // Draw a rectangle in the indicated Rectangle
        // rounding the indicated corners.
        private static GraphicsPath MakeRoundedRect(RectangleF rect, float xradius, float yradius,
                                                bool round_ul, bool round_ur, bool round_lr, bool round_ll)
        {
            // Make a GraphicsPath to draw the rectangle.
            PointF point1, point2;

            GraphicsPath path = new GraphicsPath();

            // Upper left corner.
            if (round_ul)
            {
                RectangleF corner = new RectangleF(
                    rect.X, rect.Y,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 180, 90);
                point1 = new PointF(rect.X + xradius, rect.Y);
            }

            else point1 = new PointF(rect.X, rect.Y);

            // Top side.
            if (round_ur)
                point2 = new PointF(rect.Right - xradius, rect.Y);
            else

                point2 = new PointF(rect.Right, rect.Y);
            path.AddLine(point1, point2);

            // Upper right corner.
            if (round_ur)
            {
                RectangleF corner = new RectangleF(
                    rect.Right - 2 * xradius, rect.Y,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 270, 90);
                point1 = new PointF(rect.Right, rect.Y + yradius);
            }

            else point1 = new PointF(rect.Right, rect.Y);

            // Right side.
            if (round_lr)
                point2 = new PointF(rect.Right, rect.Bottom - yradius);
            else

                point2 = new PointF(rect.Right, rect.Bottom);
            path.AddLine(point1, point2);

            // Lower right corner.
            if (round_lr)
            {
                RectangleF corner = new RectangleF(
                    rect.Right - 2 * xradius,
                    rect.Bottom - 2 * yradius,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 0, 90);
                point1 = new PointF(rect.Right - xradius, rect.Bottom);
            }

            else point1 = new PointF(rect.Right, rect.Bottom);

            // Bottom side.
            if (round_ll)
                point2 = new PointF(rect.X + xradius, rect.Bottom);
            else

                point2 = new PointF(rect.X, rect.Bottom);
            path.AddLine(point1, point2);

            // Lower left corner.
            if (round_ll)
            {
                RectangleF corner = new RectangleF(
                    rect.X, rect.Bottom - 2 * yradius,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 90, 90);
                point1 = new PointF(rect.X, rect.Bottom - yradius);
            }

            else point1 = new PointF(rect.X, rect.Bottom);

            // Left side.
            if (round_ul)
                point2 = new PointF(rect.X, rect.Y + yradius);
            else

                point2 = new PointF(rect.X, rect.Y);
            path.AddLine(point1, point2);

            // Join with the start point.
            path.CloseFigure();

            return path;
        }


        public static void SaveImage_rc()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;

                    SaveFileDialog SFD = new SaveFileDialog();
                    SFD.Filter = "Picture files | *.bmp; *.jpg; *.gif; *.png; *.tif | All Files | *.* ";
                    SFD.Title = "Save an Image File";

                    if (SFD.ShowDialog() == DialogResult.OK)
                    {
                        Common.SaveImage(frmTemp.picbx_image_rc.Image, SFD.FileName);
                    }
                }
            }
        }

    }
}
