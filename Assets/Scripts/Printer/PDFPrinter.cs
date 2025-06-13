using System.Diagnostics;
using UnityEngine;
using Sirenix.OdinInspector;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Collections;
using Debug = UnityEngine.Debug;


public class PDFPrinter : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private string pdfFileNameDebug;
    
    private GameManager gameManager;
    private string printer1Name = "EPSON TM-T20 Receipt";
    private string printer2Name = "EPSON TM-T20 Receipt5";
    private string pdfFileName = "map.pdf";
    private string pdfPath = Application.dataPath + "/Logs/";
    private string sumatraPath = Application.dataPath + "/StreamingAssets/SumatraPDF/SumatraPDF.exe";
    private int imageCounter;

    void Start()
    {
        gameManager = GameManager.Instance;
        Initialize();
    }

    public void Initialize()
    {
        imageCounter = 0;
    }

    private void CreatePDFWithImage(Texture2D texture, bool longTicket)
    {
        string imageName = Path.Combine(gameManager.LogFolderPath, $"map_{imageCounter}.png");
        imageCounter++;

        byte[] bytes = texture.EncodeToPNG();

        // Save the image to a file
        using (FileStream fs = new FileStream(imageName, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            fs.Write(bytes, 0, bytes.Length);
        }

        System.Threading.Thread.Sleep(100); // Wait for the file to be written

        // Create the PDF document
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        page.Width = XUnit.FromCentimeter(8);
        page.Height = XUnit.FromCentimeter(longTicket ? 327.6 : 29.7);

        XGraphics gfx = XGraphics.FromPdfPage(page);
        double currentY = 0;

        // --- Image d'en-tête
        string headerPath = Path.Combine(Application.dataPath, "Map/LIGNE_SUPERIEURE.png"); // <-- ton image ici
        if (File.Exists(headerPath))
        {
            XImage headerImage = XImage.FromFile(headerPath);
            double maxWidth = page.Width;
            double ratio = headerImage.PixelHeight / (double)headerImage.PixelWidth;
            double height = maxWidth * ratio;

            gfx.DrawImage(headerImage, 0, currentY, maxWidth, height);
            currentY += height;

            // Optionnel : espace sous le header
            currentY += XUnit.FromCentimeter(0.5).Point;
        }
        else
        {
            Debug.LogWarning("Image d'en-tête introuvable : " + headerPath);
        }

        // --- Image de la caméra
        XImage textureImage = XImage.FromFile(imageName);
        double imageWidth = page.Width.Point;
        double imageHeight = textureImage.PixelHeight * (imageWidth / textureImage.PixelWidth);

        gfx.DrawImage(textureImage, 0, currentY, imageWidth, imageHeight);
        currentY += imageHeight;

        // --- Sauvegarde
        string outputPath = Path.Combine(pdfPath, pdfFileName);
        document.Save(outputPath);
        Debug.Log("PDF avec image + texture généré : " + outputPath);
    }

    public Texture2D CaptureCameraSquare(Camera cam, int size)
    {
        // Create a square RenderTexture
        RenderTexture rt = new RenderTexture(size, size, 24);
        cam.targetTexture = rt;

        Texture2D result = new Texture2D(size, size, TextureFormat.RGB24, false);
        cam.Render();

        RenderTexture.active = rt;
        result.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        result.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        return result;
    }
    
    public void PrintPDF(string fileName, string printerName)
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = sumatraPath,
            Arguments = $"-print-to \"{printerName}\" -print-settings \"fit,bin=B_BOTH\" \"{pdfPath + fileName}\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };

        System.Diagnostics.Process.Start(startInfo);
        Debug.Log("Printing PDF: " + fileName + " to printer: " + printerName);
    }

    [Button]
    public void PrintLandmarkPDF(int percentage)
    {
        //PrintPDF("baki_wallpaper.pdf", printer2Name);
        CreatePDFWithLogoAndText(percentage);
        PrintPDF(pdfFileName, printer2Name);
    }
    
    
    private void CreatePDFWithLogoAndText(int percentage)
    {
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        page.Width = XUnit.FromCentimeter(8);
        page.Height = XUnit.FromCentimeter(29.7);

        XGraphics gfx = XGraphics.FromPdfPage(page);

        double currentY = 0;
        
        // --- Première image
        string secondPath = Path.Combine(Application.streamingAssetsPath, "separation.png");
        if (!File.Exists(secondPath))
        {
            Debug.LogError("Seconde image introuvable à : " + secondPath);
            return;
        }

        XImage secondImage = XImage.FromFile(secondPath);
        double secondMaxWidth = page.Width;
        double secondRatio = secondImage.PixelHeight / (double)secondImage.PixelWidth;
        double secondHeight = secondMaxWidth * secondRatio;

        gfx.DrawImage(secondImage, 0, currentY, secondMaxWidth, secondHeight);
        currentY += secondHeight;
        
        currentY += XUnit.FromCentimeter(2).Point;

        // --- Première image (logo)
        string logoPath = Path.Combine(Application.streamingAssetsPath, "logo.png");
        if (!File.Exists(logoPath))
        {
            Debug.LogError("Logo introuvable à : " + logoPath);
            return;
        }

        XImage logo = XImage.FromFile(logoPath);
        double logoMaxWidth = page.Width;
        double logoRatio = logo.PixelHeight / (double)logo.PixelWidth;
        double logoHeight = logoMaxWidth * logoRatio;

        gfx.DrawImage(logo, 0, currentY, logoMaxWidth, logoHeight);
        currentY += logoHeight;
        
        // --- Espace avant texte
        currentY += XUnit.FromCentimeter(1).Point;

        // --- Texte centré
        string text = percentage + "% data collected";
        XFont font = new XFont("Arial", 16, XFontStyle.Bold);
        XSize textSize = gfx.MeasureString(text, font);
        double x = (page.Width - textSize.Width) / 2;
        gfx.DrawString(text, font, XBrushes.Black, new XPoint(x, currentY));
        currentY += textSize.Height;

        // --- Espace blanc entre les deux (par exemple 2 cm)
        double spacing = XUnit.FromCentimeter(2).Point;
        currentY += spacing;

        // --- Troisième image
        string thirdPath = Path.Combine(Application.streamingAssetsPath, "separation.png");
        if (!File.Exists(thirdPath))
        {
            Debug.LogError("Seconde image introuvable à : " + thirdPath);
            return;
        }

        XImage thirdImage = XImage.FromFile(thirdPath);
        double thirdMaxWidth = page.Width;
        double thirdRatio = thirdImage.PixelHeight / (double)thirdImage.PixelWidth;
        double thirdHeight = thirdMaxWidth * thirdRatio;

        gfx.DrawImage(thirdImage, 0, currentY, thirdMaxWidth, thirdHeight);
        currentY += thirdHeight;

        // --- Sauvegarde
        string outputPath = Path.Combine(pdfPath, pdfFileName);
        document.Save(outputPath);
        Debug.Log("PDF avec deux images généré : " + outputPath);
    }



    public IEnumerator Print(bool longTicket = false)
    {
        yield return new WaitForEndOfFrame();
        const int size = 512;
        Texture2D square = CaptureCameraSquare(camera, size);
        CreatePDFWithImage(square, longTicket);
        PrintPDF(pdfFileName, printer1Name);
    }
    
    [Button]
    public void PrintDebug()
    {
        const int size = 512; // ou 1024 selon ta qualité
        Texture2D square = CaptureCameraSquare(camera, size);
        PrintPDF(pdfFileNameDebug, printer1Name);
    }
}
