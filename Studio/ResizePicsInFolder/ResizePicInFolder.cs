using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;




namespace Studio.ResizePicsInFolder
{
    class ResizePicInFolder
    {
        private static FolderBrowserDialog FBD = new FolderBrowserDialog();

        public static void RAIF_Initialise()
        {

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    frmTemp.txtbx_directory_RAIF.Text = Properties.Settings.Default.RAIF_directory;
                    if (frmTemp.txtbx_directory_RAIF.Text.Length == 0) frmTemp.txtbx_directory_RAIF.Text = Application.StartupPath;

                    frmTemp.txtbx_scale_RAIF.Text = Properties.Settings.Default.RAIF_scale;

                    frmTemp.grpbx_size_RAIF.Visible = true;
                    frmTemp.grpbx_scale_RAIF.Visible = false;
                    frmTemp.rdobtn_bySize_RAIF.Checked = true;
                    frmTemp.rdobtn_byScale_RAIF.Checked = false;


                }
            }


        }

        public static void ChangeType(bool WhichIsChecked)
        {

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    if (WhichIsChecked)
                    {
                        frmTemp.grpbx_size_RAIF.Visible = false;
                        frmTemp.grpbx_scale_RAIF.Visible = true;
                    }
                    else
                    {
                        frmTemp.grpbx_size_RAIF.Visible = true;
                        frmTemp.grpbx_scale_RAIF.Visible = false;
                    }



                }
            }
        }

        public static void RAIF_SaveSettings()
        {

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    Properties.Settings.Default.RAIF_directory = frmTemp.txtbx_directory_RAIF.Text;
                    Properties.Settings.Default.RAIF_scale = frmTemp.txtbx_scale_RAIF.Text;

                    Properties.Settings.Default.Save();


                }
            }



        }

        public static void OpenDirectory_RAIF()
        {

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    try
                    {
                        FBD.SelectedPath = frmTemp.txtbx_directory_RAIF.Text;
                    }
                    catch
                    {
                    }

                    if (FBD.ShowDialog() == DialogResult.OK)
                    {
                        frmTemp.txtbx_directory_RAIF.Text = FBD.SelectedPath;
                    }
                }
            }

        }

        public static void ResizeDirectory_RAIF_ByScale()
        {

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;

                    float scale = float.Parse(frmTemp.txtbx_scale_RAIF.Text);
                    if (scale == 0)
                    {
                        frmTemp.txtbx_scale_RAIF.Text = "1";
                        scale = float.Parse("1");
                    }

                    frmTemp.Cursor = Cursors.WaitCursor;
                    frmTemp.Refresh();

                    DirectoryInfo dir_info = new DirectoryInfo(frmTemp.txtbx_directory_RAIF.Text);
                    foreach (FileInfo file_info in dir_info.GetFiles())
                    {
                        try
                        {
                            string ext = file_info.Extension.ToLower();
                            if ((ext == ".bmp") || (ext == ".gif") ||
                                (ext == ".jpg") || (ext == ".jpeg") ||
                                (ext == ".png"))
                            {
                                Bitmap bm = new Bitmap(file_info.FullName);
                                frmTemp.picbx_folder_resize_RAIF.Image = bm;
                                Application.DoEvents();

                                Rectangle from_rect =
                                    new Rectangle(0, 0, bm.Width, bm.Height);

                                int wid2 = (int)Math.Round(scale * bm.Width);
                                int hgt2 = (int)Math.Round(scale * bm.Height);
                                Bitmap bm2 = new Bitmap(wid2, hgt2);
                                Rectangle dest_rect = new Rectangle(0, 0, wid2, hgt2);
                                using (Graphics gr = Graphics.FromImage(bm2))
                                {
                                    gr.InterpolationMode =
                                        InterpolationMode.HighQualityBicubic;
                                    gr.DrawImage(bm, dest_rect, from_rect,
                                        GraphicsUnit.Pixel);
                                }

                                string new_name = file_info.FullName;
                                new_name = new_name.Substring(0, new_name.Length - ext.Length);
                                new_name += "_s" + ext;
                                Common.SaveImage(bm2, new_name);
                            } // if it's a graphic extension
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error processing file '" +
                                            file_info.Name + "'\n" + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    } // foreach file_info

                    frmTemp.picbx_folder_resize_RAIF.Image = null;
                    frmTemp.Cursor = Cursors.Default;


                }
            }
        }

        public static void ResizeDirectory_RAIF_BySize()
        {

            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType() == typeof(Form1))
                {
                    Form1 frmTemp = (Form1)frm;


                    // See which values we will set.
                    bool set_width = frmTemp.chkbx_SetWidth.Checked;
                    bool set_height = frmTemp.chkbx_SetHeight.Checked;

                    if (!set_width && !set_height)
                    {
                        MessageBox.Show("You must set at least one of the width and height.",
                            "No Boxes Checked", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Get the new width and height values.
                    int new_width = 0;
                    int new_height = 0;
                    int.TryParse(frmTemp.txtBx_NewWidth.Text, out new_width);
                    int.TryParse(frmTemp.txtbx_NewHeight.Text, out new_height);

                    if (set_width && (new_width < 1))
                    {
                        MessageBox.Show("The new width must be at least 1.",
                            "Invalid Width", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (set_height && (new_height < 1))
                    {
                        MessageBox.Show("The new height must be at least 1.",
                            "Invalid Height", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    frmTemp.Cursor = Cursors.WaitCursor;
                    frmTemp.Refresh();

                    DirectoryInfo dir_info = new DirectoryInfo(frmTemp.txtbx_directory_RAIF.Text);
                    foreach (FileInfo file_info in dir_info.GetFiles())
                    {
                        try
                        {
                            string ext = file_info.Extension.ToLower();
                            if ((ext == ".bmp") || (ext == ".gif") ||
                                (ext == ".jpg") || (ext == ".jpeg") ||
                                (ext == ".png"))
                            {
                                using (Bitmap bm = new Bitmap(file_info.FullName))
                                {
                                    frmTemp.picbx_folder_resize_RAIF.Image = bm;
                                    frmTemp.picbx_folder_resize_RAIF.Refresh();

                                    Bitmap bm2 = ResizeBitmap(bm,
                                        set_width, set_height,
                                        new_width, new_height);

                                    string new_name = file_info.FullName;
                                    new_name = new_name.Substring(0,
                                        new_name.Length - ext.Length);
                                    new_name += "_s" + ext;
                                    Common.SaveImage(bm2, new_name);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error processing file '" +
                                file_info.Name + "'\n" + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    } // foreach file_info

                    frmTemp.picbx_folder_resize_RAIF.Image = null;
                    frmTemp.Text = "howto_resize_pics2";
                    frmTemp.Cursor = Cursors.Default;
                }
            }
        }


        // Resize a bitmap to the given dimensions.
        // If one dimension is omitted, scale the image uniformly.
        private static Bitmap ResizeBitmap(Bitmap bm,
        bool set_width, bool set_height,
        int new_width, int new_height)
        {
            Rectangle from_rect =
                new Rectangle(0, 0, bm.Width, bm.Height);

            // Calculate the image's new width and height.
            int wid2, hgt2;
            if (set_width)
                wid2 = new_width;
            else
                wid2 = bm.Width * new_height / bm.Height;
            if (set_height)
                hgt2 = new_height;
            else
                hgt2 = bm.Height * new_width / bm.Width;

            // Make the new image.
            Bitmap bm2 = new Bitmap(wid2, hgt2);

            // Draw the original image onto the new bitmap.
            Rectangle dest_rect = new Rectangle(0, 0, wid2, hgt2);
            using (Graphics gr = Graphics.FromImage(bm2))
            {
                gr.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;
                gr.DrawImage(bm, dest_rect, from_rect,
                    GraphicsUnit.Pixel);
            }

            return bm2;
        }

       
    }
}
