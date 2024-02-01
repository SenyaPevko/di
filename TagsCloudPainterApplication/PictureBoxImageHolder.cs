using System.Drawing.Imaging;
using System.Reflection;
using TagsCloudPainterApplication.Infrastructure;
using TagsCloudPainterApplication.Infrastructure.Settings;

namespace TagsCloudPainterApplication;

public class PictureBoxImageHolder : PictureBox, IImageHolder
{
    private readonly Lazy<ImageSettings> imageSettings;

    public PictureBoxImageHolder(Lazy<ImageSettings> imageSettings)
    {
        this.imageSettings = imageSettings ?? throw new ArgumentNullException(nameof(imageSettings));
    }

    public Size GetImageSize()
    {
        return GetImage().Size;
    }

    public Graphics StartDrawing()
    {
        return Graphics.FromImage(GetImage());
    }

    public void UpdateUi()
    {
        Refresh();
        Application.DoEvents();
    }

    public void SaveImage(string fileName)
    {
        var imageFormat = GetImageFormat(Path.GetExtension(fileName));
        GetImage().Save(fileName, imageFormat);
    }

    public Image GetImage()
    {
        if (Image == null || Image.Width != imageSettings.Value.Width || Image.Height != imageSettings.Value.Height)
            Image = new Bitmap(imageSettings.Value.Width, imageSettings.Value.Height, PixelFormat.Format24bppRgb);

        return Image;
    }

    private static ImageFormat GetImageFormat(string extension)
    {
        PropertyInfo prop = typeof(ImageFormat)
            .GetProperties().Where(p => p.Name.Equals(extension.Replace(".", ""), StringComparison.InvariantCultureIgnoreCase))
            .FirstOrDefault();

        return prop is not null
            ? prop.GetValue(prop) as ImageFormat
            : throw new ArgumentException($"there is no image format with {extension} extension");
    }
}