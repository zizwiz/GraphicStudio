using System;
using System.Reflection;
using System.Windows.Forms;
using Studio.Adjustments;
using Studio.ResizePicsInFolder;
using Studio.round_img_cnr;
using Studio.TransparentiseImage;
using Studio.Cropp;
using Studio.ResolutionSized;
using Studio.RotateImage;

namespace Studio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetBounds(
                Properties.Settings.Default.left,
                Properties.Settings.Default.top,
                Properties.Settings.Default.width,
                Properties.Settings.Default.height);

            ResizePicInFolder.RAIF_Initialise();
            round_Img_cnr.RC_Initialise();

            lbl_version.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.left = Left;
            Properties.Settings.Default.top = Top;
            Properties.Settings.Default.width = Width;
            Properties.Settings.Default.height = Height;

            ResizePicInFolder.RAIF_SaveSettings();
        }



        private void btn_exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_open_dir_RAIF_Click(object sender, EventArgs e)
        {
            ResizePicInFolder.OpenDirectory_RAIF();
        }

        private void btn_go_RAIF_Click(object sender, EventArgs e)
        {
            if (rdobtn_byScale_RAIF.Checked)
                ResizePicInFolder.ResizeDirectory_RAIF_ByScale();
            else
                ResizePicInFolder.ResizeDirectory_RAIF_BySize();
        }

        private void rdobtn_byScale_RAIF_CheckedChanged(object sender, EventArgs e)
        {
            ResizePicInFolder.ChangeType(rdobtn_byScale_RAIF.Checked);
        }

        private void btn_img_open_rc_Click(object sender, EventArgs e)
        {
            round_Img_cnr.OpenImage_RC();
        }

        private void btn_save_rc_Click(object sender, EventArgs e)
        {
            round_Img_cnr.SaveImage_rc();
        }

        private void scrYRadius_rc_Scroll(object sender, ScrollEventArgs e)
        {
            txtbx_y_radius_rc.Text = scrYRadius_rc.Value.ToString();
            round_Img_cnr.ShowImage();
        }

        private void scrXRadius_rc_Scroll(object sender, ScrollEventArgs e)
        {
            txtbx_x_radius_rc.Text = scrXRadius_rc.Value.ToString();
        }

        private void txtbx_x_radius_rc_TextChanged(object sender, EventArgs e)
        {
            scrXRadius_rc.Value = Int32.Parse(txtbx_x_radius_rc.Text);
        }

        private void txtbx_y_radius_rc_TextChanged(object sender, EventArgs e)
        {
            scrYRadius_rc.Value = Int32.Parse(txtbx_y_radius_rc.Text);
        }

        private void scrXRadius_rc_ValueChanged(object sender, EventArgs e)
        {
            round_Img_cnr.ShowImage();
        }

        private void scrYRadius_rc_ValueChanged(object sender, EventArgs e)
        {
            round_Img_cnr.ShowImage();
        }

        private void btn_open_T_Click(object sender, EventArgs e)
        {
            Transparentise.OpenFile_T();
        }

        private void btn_reset_T_Click(object sender, EventArgs e)
        {
            Transparentise.ResetFile_T();
        }

        private void btn_save_T_Click(object sender, EventArgs e)
        {
            Transparentise.SaveFile_T();
        }

        private void btn_expand_T_Click(object sender, EventArgs e)
        {
            Transparentise.ExpandTransparency_T();
        }

       
        private void picbx_Original_t_MouseClick(object sender, MouseEventArgs e)
        {
            Transparentise.ChooseColour_T(e);
        }
        /*
        private void btn_open_crop_Click(object sender, EventArgs e)
        {
            Crop.OpenFile_CROP();
        }

        private void btn_save_crop_Click(object sender, EventArgs e)
        {
            Crop.SaveFile_CROP();
        }

        private void picbx_image_CROP_MouseDown(object sender, MouseEventArgs e)
        {
            Crop.image_CROP_MouseDown(e);
        }

        private void picbx_image_CROP_MouseMove(object sender, MouseEventArgs e)
        {
            Crop.image_CROP_MouseMove(e);
        }

        private void picbx_image_CROP_MouseUp(object sender, MouseEventArgs e)
        {
            Crop.image_CROP_MouseUp(e);
        }

        private void picbx_image_CROP_Paint(object sender, PaintEventArgs e)
        {
            Crop.image_CROP_Paint(e);
        }

        private void txtbx_scale_crop_TextChanged(object sender, EventArgs e)
        {
            Crop.scale_crop_TextChanged();
        }
        

        private void txtbx_TextChanged(object sender, EventArgs e)
        {
            Crop.SizeChanged();
        }
        */

        private void btn_open_RESZ_Click(object sender, EventArgs e)
        {
            ResolutionSize.open_RESZ_Click();
        }

        private void btn_save_RESZ_Click(object sender, EventArgs e)
        {
            ResolutionSize.save_RESZ_Click();
        }

        private void btn_open_RCROP_Click(object sender, EventArgs e)
        {
            RCrop.open_RCROP_Click();
        }

        private void btn_reset_RCROP_Click(object sender, EventArgs e)
        {
            RCrop.reset_RCROP_Click();
        }

        private void btn_save_RCROP_Click(object sender, EventArgs e)
        {
            RCrop.save_RCROP_Click();
        }

        private void trkbr_scale_RCROP_Scroll(object sender, EventArgs e)
        {
            RCrop.trkbr_scale_RCROP();
        }

        private void picbx_image_RCROP_MouseUp(object sender, MouseEventArgs e)
        {
            RCrop.RCROP_MouseUp();
        }

        private void picbx_image_RCROP_MouseDown(object sender, MouseEventArgs e)
        {
            RCrop.RCROP_MouseDown(e);
        }

        private void picbx_image_RCROP_MouseMove(object sender, MouseEventArgs e)
        {
            RCrop.RCROP_MouseMove(e);
        }

        private void btn_open_RI_Click(object sender, EventArgs e)
        {
            Rotate.open_RI_Click();
        }

        private void btn_reset_RI_Click(object sender, EventArgs e)
        {
            Rotate.reset_RI_Click();
        }

        private void btn_save_RI_Click(object sender, EventArgs e)
        {
            Rotate.save_RI_Click();
        }

        private void picbx_RotateImage_RI_MouseDown(object sender, MouseEventArgs e)
        {
            Rotate.RotateImage_MouseDown(e);
        }

        private void picbx_RotateImage_RI_MouseMove(object sender, MouseEventArgs e)
        {
            Rotate.RotateImage_MouseMove(e);
        }

        private void picbx_RotateImage_RI_MouseUp(object sender, MouseEventArgs e)
        {
            Rotate.RotateImage_MouseUp();
        }

        private void btn_drawgrid_RI_Click(object sender, EventArgs e)
        {
            Rotate.drawgrid_RI_Click();
        }

        private void picbx_RotateImage_RI_Paint(object sender, PaintEventArgs e)
        {
            Rotate.picbx_RotateImage_RI_Paint(e);
        }

        private void btn_rotate_RI_Click(object sender, EventArgs e)
        {
            Rotate.rotate_RI_Click();
        }

        private void hscrollbar_brightness_ADJ_Scroll(object sender, ScrollEventArgs e)
        {
            Brightness.AdjustBrightness();
        }

        private void btn_open_ADJ_Click(object sender, EventArgs e)
        {
            Brightness.open_ADJ_Click();
        }

        private void btn_reset_ADJ_Click(object sender, EventArgs e)
        {
            Brightness.reset_ADJ_Click();
        }

        private void btn_save_ADJ_Click(object sender, EventArgs e)
        {
            Brightness.save_ADJ_Click();
        }

        private void hscrollbar_gamma_ADJ_Scroll(object sender, ScrollEventArgs e)
        {
            Gamma.AdjustGamma();
        }

        private void hscrollbar_red_ADJ_Scroll(object sender, ScrollEventArgs e)
        {
            Colourise.ColourPicture();
        }

        private void hscrollbar_green_ADJ_Scroll(object sender, ScrollEventArgs e)
        {
            Colourise.ColourPicture();
        }

        private void hscrollbar_blue_ADJ_Scroll(object sender, ScrollEventArgs e)
        {
            Colourise.ColourPicture();
        }
    }
}
