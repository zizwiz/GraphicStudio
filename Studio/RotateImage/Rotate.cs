using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Studio.RotateImage
{
    class Rotate
    {
        // The original image.
        private static Bitmap OriginalBitmap;

        // The rotated image.
        private static Bitmap RotatedBitmap;

        // The center of the bitmap.
        private static PointF ImageCenter;

        // The current angle of rotation during a drag.
        private static float CurrentAngle = 0;

        // The total angle rotated so far in previous drags.
        private static float TotalAngle = 0;

        //flags
        private static bool DrawGridFlag = false;

        public static void open_RI_Click()
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
                        OriginalBitmap = Common.LoadBitmapUnlocked(OFD.FileName);
                        RotatedBitmap = OriginalBitmap.Clone() as Bitmap;

                        MakeImage();
                    }
                }
            }
        }

        private static void MakeImage()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    // Start with no rotation.
                    CurrentAngle = 0;
                    TotalAngle = 0;

                    // Load the bitmap.
                    Bitmap bm = OriginalBitmap.Clone() as Bitmap; //new Bitmap(OFD.FileName);
                    frmTemp.picbx_RotateImage_RI.Image = OriginalBitmap;
                    frmTemp.picbx_RotateImage_RI.Visible = true;

                    // See how big the rotated bitmap must be.
                    int wid = (int)Math.Sqrt(bm.Width * bm.Width + bm.Height * bm.Height);

                    // Make the original unrotated bitmap.
                    OriginalBitmap = new Bitmap(wid, wid);

                    // Save the center of the image for calculating rotation angles.
                    ImageCenter = new PointF(wid / 2f, wid / 2f);

                    // Copy the image into the middle of the bitmap.
                    using (Graphics gr = Graphics.FromImage(OriginalBitmap))
                    {
                        // Clear with the color in the image's upper left corner.
                        gr.Clear(bm.GetPixel(0, 0));

                        //// For debugging. (Easier to see the background.)
                        // gr.Clear(Color.LightBlue);

                        // Draw the image centered.
                        gr.DrawImage(bm, (wid - bm.Width) / 2, (wid - bm.Height) / 2);
                    }

                    // Display the original image.
                    frmTemp.picbx_RotateImage_RI.Image = OriginalBitmap;

                    // Size the form to fit.
                    SizeForm();

                }
            }


        }


        public static void reset_RI_Click()
        {
            RotatedBitmap = OriginalBitmap.Clone() as Bitmap;
            MakeImage();
        }



        public static void drawgrid_RI_Click()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    if (DrawGridFlag)
                    {
                        DrawGridFlag = false;
                    }
                    else
                    {
                        DrawGridFlag = true;
                    }

                    frmTemp.picbx_RotateImage_RI.Refresh();
                }
            }
        }

        public static void picbx_RotateImage_RI_Paint(PaintEventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    if (DrawGridFlag)
                    {
                        int dx = Int16.Parse(frmTemp.txtbx_size_RI.Text);
                        int xmax = frmTemp.picbx_RotateImage_RI.ClientSize.Width;
                        int ymax = frmTemp.picbx_RotateImage_RI.ClientSize.Height;
                        for (int x = 0; x < xmax; x += dx)
                            e.Graphics.DrawLine(Pens.Silver, x, 0, x, ymax);
                        for (int y = 0; y < ymax; y += dx)
                            e.Graphics.DrawLine(Pens.Silver, 0, y, xmax, y);
                    }
                }
            }
        }



        public static void save_RI_Click()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    SaveFileDialog SFD = new SaveFileDialog();
                    SFD.Filter = "Picture files | *.bmp; *.jpg; *.gif; *.png; *.tif | All Files | *.* ";
                    SFD.Title = "Save an Image File";

                    if ((SFD.ShowDialog() == DialogResult.OK) && (frmTemp.picbx_RotateImage_RI.Image != null))
                    {
                        Common.SaveImage(RotatedBitmap, SFD.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("Perhaps no image to save");
                }
            }
        }


        // Make sure the form is big enough to show the rotated image.
        private static void SizeForm()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    int wid = frmTemp.picbx_RotateImage_RI.Right + frmTemp.picbx_RotateImage_RI.Left;
                    int hgt = frmTemp.picbx_RotateImage_RI.Bottom + frmTemp.picbx_RotateImage_RI.Left;
                    frmTemp.ClientSize = new Size(
                        Math.Max(wid, frmTemp.ClientSize.Width),
                        Math.Max(hgt, frmTemp.ClientSize.Height));
                }
            }
        }


        // Return a bitmap rotated around its center.
        private static Bitmap RotateBitmap(Bitmap bm, float angle)
        {
            // Make a bitmap to hold the rotated result.
            Bitmap result = new Bitmap(bm.Width, bm.Height);

            // Create the real rotation transformation.
            Matrix rotate_at_center = new Matrix();
            rotate_at_center.RotateAt(angle,
                new PointF(bm.Width / 2f, bm.Height / 2f));

            // Draw the image onto the new bitmap rotated.
            using (Graphics gr = Graphics.FromImage(result))
            {
                // Use smooth image interpolation.
                gr.InterpolationMode = InterpolationMode.High;

                // Clear with the color in the image's upper left corner.
                gr.Clear(OriginalBitmap.GetPixel(0, 0));

                //// For debugging. (Makes it easier to see the background.)
                //gr.Clear(Color.LightBlue);

                // Set up the transformation to rotate.
                gr.Transform = rotate_at_center;

                // Draw the image centered on the bitmap.
                gr.DrawImage(bm, 0, 0);
            }

            // Return the result bitmap.
            return result;
        }


        // Let the user click and drag to rotate.
        private static float StartAngle;
        private static bool DragInProgress = false;
        public static void RotateImage_MouseDown(MouseEventArgs e)
        {
            // Do nothing if there's no image loaded.
            if (OriginalBitmap == null) return;
            DragInProgress = true;

            // Get the initial angle from horizontal to the
            // vector between the center and the drag start point.
            float dx = e.X - ImageCenter.X;
            float dy = e.Y - ImageCenter.Y;
            StartAngle = (float)Math.Atan2(dy, dx);
        }

        public static void RotateImage_MouseMove(MouseEventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    // Do nothing if there's no drag in progress.
                    if (!DragInProgress) return;

                    // Get the angle from horizontal to the
                    // vector between the center and the current point.
                    float dx = e.X - ImageCenter.X;
                    float dy = e.Y - ImageCenter.Y;
                    float new_angle = (float)Math.Atan2(dy, dx);

                    // Calculate the change in angle.
                    CurrentAngle = new_angle - StartAngle;

                    // Convert to degrees.
                    CurrentAngle *= 180 / (float)Math.PI;

                    // Add to the previous total angle rotated.
                    CurrentAngle += TotalAngle;
                    frmTemp.lbl_angle_RI.Text = CurrentAngle.ToString("0.00") + "°";

                    // Rotate the original image to make the result bitmap.
                    RotatedBitmap = RotateBitmap(OriginalBitmap, CurrentAngle);

                    // Display the result.
                    frmTemp.picbx_RotateImage_RI.Image = RotatedBitmap;
                    frmTemp.picbx_RotateImage_RI.Refresh();
                }
            }
        }

        public static void RotateImage_MouseUp()
        {
            DragInProgress = false;

            // Save the new total angle of rotation.
            TotalAngle = CurrentAngle;
        }

        public static void rotate_RI_Click()
        {

            // RotateBitmap(Bitmap bm, float angle)
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    // Get the angle.
                    TotalAngle = float.Parse(frmTemp.txtbx_angle_RI.Text);

                    // Rotate.
                    RotatedBitmap = RotateBitmap(OriginalBitmap, TotalAngle);

                    // Display the result.
                    frmTemp.picbx_RotateImage_RI.Image = RotatedBitmap;

                    // Size the form to fit.
                    SizeForm();

                    frmTemp.lbl_angle_RI.Text = TotalAngle.ToString("0.00") + "°";
                }
            }


        }
    }
}
