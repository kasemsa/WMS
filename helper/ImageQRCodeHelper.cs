using QRCoder;

namespace WarehouseManagementSystem.helper
{
    public static class ImageQRCodeHelper
    {
        private static readonly QRCodeGenerator _qrCodeGenerator = new QRCodeGenerator();
        private const int DefaultSize = 200; // Default QR code size in pixels

        public static byte[] GenerateQRCode(string content)
        {
            try
            {
                using var qrCodeData = _qrCodeGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCode(qrCodeData);
                using var ms = new MemoryStream();
                qrCode.GetGraphic(DefaultSize).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating QR code: {ex.Message}", ex);
            }
        }
    }
}
