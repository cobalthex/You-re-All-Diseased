#region File Description
//-----------------------------------------------------------------------------
// MainForm.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
#endregion

namespace TrueTypeConverter
{
    /// <summary>
    /// Utility for rendering Windows fonts out into a BMP file
    /// which can then be imported into the XNA Framework using
    /// the Content Pipeline FontTextureProcessor.
    /// </summary>
    public partial class MainForm : Form
    {
        Bitmap globalBitmap;
        Graphics globalGraphics;
        Font font;
        string fontError;
        bool useCustomFont = false;


        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            globalBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            globalGraphics = Graphics.FromImage(globalBitmap);

            foreach (FontFamily font in FontFamily.Families)
                FontName.Items.Add(font.Name);

            FontName.Text = "Sans";
        }


        /// <summary>
        /// When the font selection changes, create a new Font
        /// instance and update the preview text label.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void SelectionChanged()
        {
            try
            {
                // Parse the size selection.
                float size;

                if (!float.TryParse(FontSize.Text, out size) || (size <= 0))
                {
                    fontError = "Invalid font size '" + FontSize.Text + "'";
                    return;
                }

                // Parse the font style selection.
                FontStyle style;

                try
                {
                    style = (FontStyle)Enum.Parse(typeof(FontStyle), FontStyle.Text);
                }
                catch
                {
                    fontError = "Invalid font style '" + FontStyle.Text + "'";
                    return;
                }

                Font newFont;

                // Create the new font.
                if (!useCustomFont)
                    newFont = new Font(FontName.Text, size, style);
                else
                {
                    newFont = new Font(font.FontFamily, size, style);
                }

                if (font != null)
                    font.Dispose();

                Sample.Font = font = newFont;

                fontError = null;
            }
            catch (Exception exception)
            {
                fontError = exception.Message;
            }
        }


        /// <summary>
        /// Selection changed event handler.
        /// </summary>
        private void FontName_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            useCustomFont = false;
            SelectionChanged();
        }


        /// <summary>
        /// Selection changed event handler.
        /// </summary>
        private void FontStyle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SelectionChanged();
        }


        /// <summary>
        /// Selection changed event handler.
        /// </summary>
        private void FontSize_TextUpdate(object sender, System.EventArgs e)
        {
            SelectionChanged();
        }


        /// <summary>
        /// Selection changed event handler.
        /// </summary>
        private void FontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged();
        }


        /// <summary>
        /// Event handler for when the user clicks on the Export button.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void Export_Click(object sender, EventArgs e)
        {
            // If the current font is invalid, report that to the user.
            if (fontError != null)
                throw new ArgumentException(fontError);

            try
            {
                // If the current font is invalid, report that to the user.
                if (fontError != null)
                    throw new ArgumentException(fontError);

                // Convert the character range from string to integer,
                // and validate it.
                int minChar = ParseHex(MinChar.Text);
                int maxChar = ParseHex(MaxChar.Text);

                if ((minChar >= maxChar) ||
                    (minChar < 0) || (minChar > 0xFFFF) ||
                    (maxChar < 0) || (maxChar > 0xFFFF))
                {
                    throw new ArgumentException("Invalid character range " +
                                                MinChar.Text + " - " + MaxChar.Text);
                }

                // Choose the output file.
                SaveFileDialog fileSelector = new SaveFileDialog();

                fileSelector.Title = "Export Font";
                fileSelector.DefaultExt = "png";
                fileSelector.Filter = "PNG file (*.png)|*.png";

                if (fileSelector.ShowDialog() == DialogResult.OK)
                {
                    // Build up a list of all the glyphs to be output.
                    List<Bitmap> bitmaps = new List<Bitmap>();
                    List<int> xPositions = new List<int>();
                    List<int> yPositions = new List<int>();

                    try
                    {
                        int padding = 0;
                        System.IO.StreamWriter txtWrite = new System.IO.StreamWriter(fileSelector.FileName + ".txt");

                        int width = padding;
                        int height = padding;
                        int lineWidth = padding;
                        int lineHeight = padding;
                        int count = 0;

                        // Rasterize each character in turn,
                        // and add it to the output list.
                        int n = 0;
                        for (char ch = (char)minChar; ch < maxChar; ch++, n++)
                        {
                            Bitmap bitmap = RasterizeCharacter(ch);

                            bitmaps.Add(bitmap);

                            xPositions.Add(lineWidth);
                            yPositions.Add(height);

                            lineWidth += bitmap.Width + padding;
                            lineHeight = Math.Max(lineHeight, bitmap.Height + padding);

                            if (txtWrite != null)
                                txtWrite.WriteLine(string.Format("{0}\0{1}\0{2}\0{3}\0{4}", ch, xPositions[n], yPositions[n], bitmap.Width, Math.Max(lineHeight - 1, bitmap.Height)));

                            // Output 16 glyphs per line, then wrap to the next line.
                            if ((++count == 16) || (ch == maxChar - 1))
                            {
                                width = Math.Max(width, lineWidth);
                                height += lineHeight;
                                lineWidth = padding;
                                lineHeight = padding;
                                count = 0;
                            }
                        }

                        if (txtWrite != null)
                            txtWrite.Close();

                        using (Bitmap bitmap = new Bitmap(width, height,
                                                          PixelFormat.Format32bppArgb))
                        {
                            // Arrage all the glyphs onto a single larger bitmap.
                            using (Graphics graphics = Graphics.FromImage(bitmap))
                            {
                                graphics.Clear(Color.Transparent);
                                graphics.CompositingMode = CompositingMode.SourceCopy;

                                for (int i = 0; i < bitmaps.Count; i++)
                                {
                                    graphics.DrawImage(bitmaps[i], xPositions[i], yPositions[i]);
                                }

                                graphics.Flush();
                            }

                            // Save out the combined bitmap.
                            string path = System.IO.Path.GetExtension(fileSelector.FileName);
                            bitmap.Save(fileSelector.FileName, ImageFormat.Png); //default

                        }
                    }
                    finally
                    {
                        // Clean up temporary objects.
                        foreach (Bitmap bitmap in bitmaps)
                            bitmap.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                // Report any errors to the user.
                MessageBox.Show(exception.Message, Text + " Error");
            }
        }

        /// <summary>
        /// Event handler for when the user clicks on the Export (XNA) button.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ExportXNA_Click(object sender, EventArgs e)
        {
            try
            {
                // If the current font is invalid, report that to the user.
                if (fontError != null)
                    throw new ArgumentException(fontError);

                // Convert the character range from string to integer,
                // and validate it.
                int minChar = ParseHex(MinChar.Text);
                int maxChar = ParseHex(MaxChar.Text);

                if ((minChar >= maxChar) ||
                    (minChar < 0) || (minChar > 0xFFFF) ||
                    (maxChar < 0) || (maxChar > 0xFFFF))
                {
                    throw new ArgumentException("Invalid character range " +
                                                MinChar.Text + " - " + MaxChar.Text);
                }

                // Choose the output file.
                SaveFileDialog fileSelector = new SaveFileDialog();

                fileSelector.Title = "Export Font";
                fileSelector.DefaultExt = "png";
                fileSelector.Filter = "PNG file (*.png)|*.png|Bitmap file (*.bmp)|*.bmp|TIFF file (*.tif)|*.tif|JPEG file (*.jpg)|*.jpg|All files (*.*)|*.*";

                if (fileSelector.ShowDialog() == DialogResult.OK)
                {
                    // Build up a list of all the glyphs to be output.
                    List<Bitmap> bitmaps = new List<Bitmap>();
                    List<int> xPositions = new List<int>();
                    List<int> yPositions = new List<int>();

                    try
                    {
                        int padding = 2;

                        int width = padding;
                        int height = padding;
                        int lineWidth = padding;
                        int lineHeight = padding;
                        int count = 0;

                        // Rasterize each character in turn,
                        // and add it to the output list.
                        int n = 0;
                        for (char ch = (char)minChar; ch < maxChar; ch++, n++)
                        {
                            Bitmap bitmap = RasterizeCharacter(ch);

                            bitmaps.Add(bitmap);

                            xPositions.Add(lineWidth);
                            yPositions.Add(height);

                            lineWidth += bitmap.Width + padding;
                            lineHeight = Math.Max(lineHeight, bitmap.Height + padding);

                            // Output 16 glyphs per line, then wrap to the next line.
                            if ((++count == 16) || (ch == maxChar - 1))
                            {
                                width = Math.Max(width, lineWidth);
                                height += lineHeight;
                                lineWidth = padding;
                                lineHeight = padding;
                                count = 0;
                            }
                        }

                        using (Bitmap bitmap = new Bitmap(width, height,
                                                          PixelFormat.Format32bppArgb))
                        {
                            // Arrage all the glyphs onto a single larger bitmap.
                            using (Graphics graphics = Graphics.FromImage(bitmap))
                            {
                                graphics.Clear(Color.Magenta);
                                graphics.CompositingMode = CompositingMode.SourceCopy;

                                for (int i = 0; i < bitmaps.Count; i++)
                                {
                                    graphics.DrawImage(bitmaps[i], xPositions[i], yPositions[i]);
                                }

                                graphics.Flush();
                            }

                            // Save out the combined bitmap.
                            string path = System.IO.Path.GetExtension(fileSelector.FileName);
                            if (path == ".bmp")
                                bitmap.Save(fileSelector.FileName, ImageFormat.Bmp);
                            else if (path == ".tif")
                                bitmap.Save(fileSelector.FileName, ImageFormat.Tiff);
                            else if (path == ".jpg")
                                bitmap.Save(fileSelector.FileName, ImageFormat.Jpeg);
                            else
                                bitmap.Save(fileSelector.FileName, ImageFormat.Png); //default

                        }
                    }
                    finally
                    {
                        // Clean up temporary objects.
                        foreach (Bitmap bitmap in bitmaps)
                            bitmap.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                // Report any errors to the user.
                MessageBox.Show(exception.Message + "\n\n" + exception.StackTrace, Text + " Error");
            }
        }


        /// <summary>
        /// Helper for rendering out a single font character
        /// into a System.Drawing bitmap.
        /// </summary>
        private Bitmap RasterizeCharacter(char ch)
        {
            string text = ch.ToString();

            SizeF size = globalGraphics.MeasureString(text, font);

            int width = (int)Math.Ceiling(size.Width);
            int height = (int)Math.Ceiling(size.Height);

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                if (Antialias.Checked)
                {
                    graphics.TextRenderingHint =
                        TextRenderingHint.AntiAliasGridFit;
                }
                else
                {
                    graphics.TextRenderingHint =
                        TextRenderingHint.SingleBitPerPixelGridFit;
                }

                graphics.Clear(Color.Transparent);

                using (Brush brush = new SolidBrush(fontColor.BackColor))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;

                    graphics.DrawString(text, font, brush, 0, 0, format);
                }

                graphics.Flush();
            }

            return CropCharacter(bitmap);
        }


        /// <summary>
        /// Helper for cropping ununsed space from the sides of a bitmap.
        /// </summary>
        private Bitmap CropCharacter(Bitmap bitmap)
        {
            int cropLeft = 0;
            int cropRight = bitmap.Width - 1;

            // Remove unused space from the left.
            while ((cropLeft < cropRight) && (BitmapIsEmpty(bitmap, cropLeft)))
                cropLeft++;

            // Remove unused space from the right.
            while ((cropRight > cropLeft) && (BitmapIsEmpty(bitmap, cropRight)))
                cropRight--;

            // Don't crop if that would reduce the glyph down to nothing at all!
            if (cropLeft == cropRight)
                return bitmap;

            // Add some padding back in.
            cropLeft -= (int)separation.Value;
            cropRight += (int)separation.Value + 1;

            int width = cropRight - cropLeft;

            // Crop the glyph.
            Bitmap croppedBitmap = new Bitmap(width, bitmap.Height + ((int)separation.Value << 1), bitmap.PixelFormat);

            using (Graphics graphics = Graphics.FromImage(croppedBitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(bitmap, 0, 0, new Rectangle(cropLeft, 0 - (int)separation.Value, width, bitmap.Height + (int)separation.Value), GraphicsUnit.Pixel);
                graphics.Flush();
            }

            bitmap.Dispose();

            return croppedBitmap;
        }


        /// <summary>
        /// Helper for testing whether a column of a bitmap is entirely empty.
        /// </summary>
        private static bool BitmapIsEmpty(Bitmap bitmap, int x)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                if (bitmap.GetPixel(x, y).A != 0)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Helper for converting strings to integer.
        /// </summary>
        static int ParseHex(string text)
        {
            NumberStyles style;

            if (text.StartsWith("0x"))
            {
                style = NumberStyles.HexNumber;
                text = text.Substring(2);
            }
            else
            {
                style = NumberStyles.Integer;
            }

            int result;

            if (!int.TryParse(text, style, null, out result))
                return -1;

            return result;
        }

        private void previewText_TextChanged(object sender, EventArgs e)
        {
            Sample.Text = previewText.Text;
        }

        private void customFont_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Choose the output file.
            OpenFileDialog fileSelector = new OpenFileDialog();

            fileSelector.Title = "Open Font";
            fileSelector.DefaultExt = "ttf";
            fileSelector.Filter = "TrueType font (*.ttf)|*.ttf|OpenType font (*.otf)|*.otf|Generic font file (*.fon)|*.fon|All files (*.*)|*.*";

            //load font
            if (fileSelector.ShowDialog() == DialogResult.OK)
            {
                PrivateFontCollection pfc = new PrivateFontCollection();
                try
                {
                    pfc.AddFontFile(fileSelector.FileName);
                    //label1.Font = new Font(pfc.Families[0], 16, FontStyle.Regular);
                }
                catch (Exception expt)
                {
                    MessageBox.Show("Invalid font file\n" + expt.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                font = new Font(pfc.Families[0], 16, System.Drawing.FontStyle.Regular);
                useCustomFont = true;
                SelectionChanged(); //update
            }
        }

        private void fontColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fontColor.BackColor = cd.Color;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ExportClassic_Click(object sender, EventArgs e)
        {
            try
            {
                if (fontError != null)
                    throw new ArgumentException(fontError);

                // Convert the character range from string to integer,
                // and validate it.
                int minChar = ParseHex(MinChar.Text);
                int maxChar = ParseHex(MaxChar.Text);

                if ((minChar >= maxChar) ||
                    (minChar < 0) || (minChar > 0xFFFF) ||
                    (maxChar < 0) || (maxChar > 0xFFFF))
                {
                    throw new ArgumentException("Invalid character range " +
                                                MinChar.Text + " - " + MaxChar.Text);
                }

                // Choose the output file.
                SaveFileDialog fileSelector = new SaveFileDialog();

                fileSelector.Title = "Export Font";
                fileSelector.DefaultExt = "png";
                fileSelector.Filter = "PNG file (*.png)|*.png|Bitmap file (*.bmp)|*.bmp|TIFF file (*.tif)|*.tif|JPEG file (*.jpg)|*.jpg|All files (*.*)|*.*";

                if (fileSelector.ShowDialog() == DialogResult.OK)
                {
                    List<Bitmap> bitmaps = new List<Bitmap>();
                    int maxW = 0, maxH = 0;
                    int numPerRow = (int)Math.Sqrt(maxChar - minChar) + 1;

                    try
                    {

                        // Rasterize each character in turn,
                        // and add it to the output list.
                        for (char ch = (char)minChar; ch < maxChar; ch++)
                        {
                            Bitmap bitmap = RasterizeCharacter(ch);

                            if (bitmap.Width > maxW)
                                maxW = bitmap.Width;
                            if (bitmap.Height > maxH)
                                maxH = bitmap.Height;

                            bitmaps.Add(bitmap);
                        }

                        using (Bitmap bitmap = new Bitmap(numPerRow * maxW, ((int)(maxChar - minChar) / numPerRow) * maxH, PixelFormat.Format32bppArgb))
                        {
                            // Arrage all the glyphs onto a single larger bitmap.
                            using (Graphics graphics = Graphics.FromImage(bitmap))
                            {
                                graphics.CompositingMode = CompositingMode.SourceCopy;

                                for (int i = 0; i < bitmaps.Count; i++)
                                {
                                    int x = i % numPerRow;
                                    int y = i / numPerRow;
                                    graphics.DrawImage(bitmaps[i], x * maxW, y * maxH);
                                }

                                graphics.Flush();
                            }

                            // Save out the combined bitmap.
                            string path = System.IO.Path.GetExtension(fileSelector.FileName);
                            if (path == ".bmp")
                                bitmap.Save(fileSelector.FileName, ImageFormat.Bmp);
                            else if (path == ".tif")
                                bitmap.Save(fileSelector.FileName, ImageFormat.Tiff);
                            else if (path == ".jpg")
                                bitmap.Save(fileSelector.FileName, ImageFormat.Jpeg);
                            else
                                bitmap.Save(fileSelector.FileName, ImageFormat.Png); //default


                        }
                    }
                    finally
                    {
                        // Clean up temporary objects.
                        foreach (Bitmap bitmap in bitmaps)
                            bitmap.Dispose();

                        if (maxW > 0 && maxH > 0)
                            MessageBox.Show("Char width: " + maxW + "\nChar height: " + maxH + "\nColumns: " + numPerRow, "Operation complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception exception)
            {
                // Report any errors to the user.
                MessageBox.Show(exception.Message, Text + " Error");
            }
        }
    }
}
