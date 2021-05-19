using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Studio.Cropp
{
    class RCrop
    {

        // The original image.
        private static Bitmap OriginalImage;

        // The currently cropped image.
        private static Bitmap CroppedImage;

        // The currently scaled cropped image.
        private static Bitmap ScaledImage;

        // The cropped image with the selection rectangle.
        private static Bitmap DisplayImage;
        private static Graphics DisplayGraphics;

        // The current scale.
        private static float ImageScale = 1f;


        public static void open_RCROP_Click()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;

                    OpenFileDialog OFD = new OpenFileDialog();
                    OFD.Filter = "Picture files | *.bmp; *.jpg; *.gif; *.png; *.tif | All Files | *.* ";
                    OFD.Title = "Open an Image File";

                    if (OFD.ShowDialog() == DialogResult.OK)
                    {

                        OriginalImage = Common.LoadBitmapUnlocked(OFD.FileName);
                        CroppedImage = OriginalImage.Clone() as Bitmap;

                        MakeScaledImage();
                    }
                }
            }
        }

        public static void save_RCROP_Click()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;

                    SaveFileDialog SFD = new SaveFileDialog();
                    SFD.Filter = "Picture files | *.bmp; *.jpg; *.gif; *.png; *.tif | All Files | *.* ";
                    SFD.Title = "Save an Image File";

                    if ((SFD.ShowDialog() == DialogResult.OK) && (frmTemp.picbx_image_RCROP.Image != null))
                    {
                        Common.SaveImage(CroppedImage, SFD.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("Perhaps no image to save");
                }
            }
        }
        
        // Make the scaled cropped image.
        private static void MakeScaledImage()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;

                    int wid = (int) (ImageScale * (CroppedImage.Width));
                    int hgt = (int) (ImageScale * (CroppedImage.Height));
                    if (wid <= 0)
                    {
                        wid = 1;
                    }
                    else if (wid > 5000)
                    {
                        wid = 5000;
                    }

                    if (hgt <= 0)
                    {
                        hgt = 1;
                    }
                    else if (hgt > 5000)
                    {
                        hgt = 5000;
                    }
                    
                    ScaledImage = new Bitmap(wid, hgt);
                    using (Graphics gr = Graphics.FromImage(ScaledImage))
                    {
                        Rectangle src_rect = new Rectangle(0, 0,
                            CroppedImage.Width, CroppedImage.Height);
                        Rectangle dest_rect = new Rectangle(0, 0, wid, hgt);
                        gr.PixelOffsetMode = PixelOffsetMode.Half;
                        gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                        gr.DrawImage(CroppedImage, dest_rect, src_rect,
                            GraphicsUnit.Pixel);
                    }

                    DisplayImage = ScaledImage.Clone() as Bitmap;
                   // if (DisplayGraphics != null) DisplayGraphics.Dispose();
                   // DisplayGraphics = Graphics.FromImage(DisplayImage);
                   GC.Collect();
                    frmTemp.picbx_image_RCROP.Image = DisplayImage;
                    frmTemp.picbx_image_RCROP.Visible = true;
                }
            }
        }



        public static void trkbr_scale_RCROP()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;

                    ImageScale = (frmTemp.trkbr_scale_RCROP.Value * 50) / 100f;

                    MakeScaledImage();
                }
            }
        }

        public static void reset_RCROP_Click()
        {
            CroppedImage = OriginalImage.Clone() as Bitmap;
            MakeScaledImage();
        }



        // Let the user select an area.
        private static bool Drawing = false;
        private static Point StartPoint, EndPoint;

        public static void RCROP_MouseDown(MouseEventArgs e)
        {
            Drawing = true;

            StartPoint = RoundPoint(e.Location);

            // Draw the area selected.
            DrawSelectionBox(StartPoint);
        }

        public static void RCROP_MouseMove(MouseEventArgs e)
        {
            if (!Drawing) return;

            // Draw the area selected.
            DrawSelectionBox(RoundPoint(e.Location));
        }

        public static void RCROP_MouseUp()
        {
            if (!Drawing) return;
            Drawing = false;

            // Crop.
            // Get the selected area's dimensions.
            int x = (int)(Math.Min(StartPoint.X, EndPoint.X) / ImageScale);
            int y = (int)(Math.Min(StartPoint.Y, EndPoint.Y) / ImageScale);
            int width = (int)(Math.Abs(StartPoint.X - EndPoint.X) / ImageScale);
            int height = (int)(Math.Abs(StartPoint.Y - EndPoint.Y) / ImageScale);

            if ((width == 0) || (height == 0))
            {
                MessageBox.Show("Width and height must be greater than 0.");
                return;
            }

            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);

            // Copy that part of the image to a new bitmap.
            Bitmap new_image = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(new_image))
            {
                gr.DrawImage(CroppedImage, dest_rect, source_rect,
                    GraphicsUnit.Pixel);
            }
            CroppedImage = new_image;

            // Display the new scaled image.
            MakeScaledImage();
        }

        // Round the point to the nearest unscaled pixel location.
        private static Point RoundPoint(Point point)
        {
            int x = (int)(ImageScale * (int)(point.X / ImageScale));
            int y = (int)(ImageScale * (int)(point.Y / ImageScale));
            return new Point(x, y);
        }

        // Draw the area selected.
        private static void DrawSelectionBox(Point end_point)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;
                    // Save the end point.
                    EndPoint = end_point;
                    if (EndPoint.X < 0) EndPoint.X = 0;
                    if (EndPoint.X >= ScaledImage.Width) EndPoint.X = ScaledImage.Width - 1;
                    if (EndPoint.Y < 0) EndPoint.Y = 0;
                    if (EndPoint.Y >= ScaledImage.Height) EndPoint.Y = ScaledImage.Height - 1;

                    // Reset the image.
                    DisplayGraphics.DrawImageUnscaled(ScaledImage, 0, 0);

                    // Draw the selection area.
                    int x = Math.Min(StartPoint.X, EndPoint.X);
                    int y = Math.Min(StartPoint.Y, EndPoint.Y);
                    int width = Math.Abs(StartPoint.X - EndPoint.X);
                    int height = Math.Abs(StartPoint.Y - EndPoint.Y);
                    DisplayGraphics.DrawRectangle(Pens.Red, x, y, width, height);
                    frmTemp.picbx_image_RCROP.Refresh();
                }
            }
        }
    }
}
