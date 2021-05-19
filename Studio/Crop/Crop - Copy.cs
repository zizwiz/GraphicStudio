using System;
using System.Drawing;
using System.Windows.Forms;

namespace Studio.Cropp
{
    class Crop
    {
        private static Point ULCorner = new Point(0, 0);

        private static bool Dragging = false;
        private static Point LastPoint;

        private static Bitmap OriginalImage;
        private static Bitmap ScaledImage;

        private static float ImageScale = 1f;

        public static void OpenFile_CROP()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    OpenFileDialog OFD = new OpenFileDialog();

                    if (OFD.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // Load the image.
                            OriginalImage = Common.LoadBitmapUnlocked(OFD.FileName);
                            ShowScaledImage();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }



        public static void SaveFile_CROP()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    SaveFileDialog SFD = new SaveFileDialog();

                    if (SFD.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // Copy the selected area into a new Bitmap.
                            Rectangle rect = SelectionRectangle(false);
                            Bitmap bm = new Bitmap(rect.Width, rect.Height);
                            using (Graphics gr = Graphics.FromImage(bm))
                            {
                                gr.DrawImage(OriginalImage, 0, 0, rect,
                                    GraphicsUnit.Pixel);
                            }

                            // Save the new Bitmap.
                            Common.SaveImage(bm, SFD.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }


        // Display the scaled image.
        private static void ShowScaledImage()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    if (OriginalImage == null) return;
                    int scaled_width = (int)(OriginalImage.Width * ImageScale);
                    int scaled_height = (int)(OriginalImage.Height * ImageScale);
                    if ((scaled_height > 0) || (scaled_width > 0))
                    {
                        ScaledImage = new Bitmap(scaled_width, scaled_height);
                    }
                    else
                    {
                        ScaledImage = new Bitmap(1, 1);
                    }

                    using (Graphics gr = Graphics.FromImage(ScaledImage))
                    {
                        Point[] dest_points =
                        {
                            new Point(0, 0),
                            new Point(scaled_width - 1, 0),
                            new Point(0, scaled_height - 1),
                        };
                        Rectangle src_rect = new Rectangle(
                            0, 0,
                            OriginalImage.Width - 1,
                            OriginalImage.Height - 1);
                        gr.DrawImage(OriginalImage, dest_points, src_rect, GraphicsUnit.Pixel);
                    }

                    frmTemp.picbx_image_CROP.Image = ScaledImage;
                    frmTemp.picbx_image_CROP.Visible = true;
                    frmTemp.picbx_image_CROP.Refresh();
                }
            }
        }


        // Return the currently selected area.
        private static Rectangle SelectionRectangle(bool scaled)
        {
            int x = 0, y = 0, width = 0, height = 0;

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    // Get the desired dimensions.
                    // If there's a problem, return a zero-size rectangle.
                    if (!int.TryParse(frmTemp.txtbx_Width.Text, out width))
                        return new Rectangle();
                    if (!int.TryParse(frmTemp.txtbx_Height.Text, out height))
                        return new Rectangle();
                    x = ULCorner.X;
                    y = ULCorner.Y;

                    // Return the rectangle.
                    if (scaled)
                    {
                        x = (int)(x * ImageScale);
                        y = (int)(y * ImageScale);
                        width = (int)(width * ImageScale);
                        height = (int)(height * ImageScale);
                    }
                }
            }
            return new Rectangle(x, y, width, height);
        }



        public static void image_CROP_MouseDown(MouseEventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    // If the mouse is not inside the
                    // selection rectangle, do nothing.
                    Rectangle rect = SelectionRectangle(true);
                    if (!rect.Contains(e.Location)) return;

                    // Start the drag.
                    LastPoint = e.Location;
                    Dragging = true;
                }
            }
        }

        public static void image_CROP_MouseMove(MouseEventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    if (!Dragging) return;
                    ULCorner.X += (int)((e.Location.X - LastPoint.X) / ImageScale);
                    ULCorner.Y += (int)((e.Location.Y - LastPoint.Y) / ImageScale);
                    LastPoint = e.Location;
                    frmTemp.picbx_image_CROP.Refresh();
                }
            }
        }

        public static void image_CROP_MouseUp(MouseEventArgs e)
        {
            Dragging = false;
        }

        public static void image_CROP_Paint(PaintEventArgs e)
        {
            try
            {
                // Draw the selection rectangle.
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    Rectangle rect = SelectionRectangle(true);
                    e.Graphics.DrawRectangle(pen, rect);

                    pen.Color = Color.Yellow;
                    pen.DashPattern = new float[] { 5, 5 };
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
            catch
            {
            }
        }
        
        public static void scale_crop_TextChanged()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;

                    if (frmTemp.txtbx_scale_crop.Text == "")
                    {
                        frmTemp.txtbx_scale_crop.Text = "1";
                    }
                    // Get the scale factor.
                    ImageScale = float.Parse(frmTemp.txtbx_scale_crop.Text) / 100f;
                   

                    ShowScaledImage();
                }
            }
        }


        public static void SizeChanged()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1) frm;

                    frmTemp.picbx_image_CROP.Refresh();
                }
            }
        }

    }
}
