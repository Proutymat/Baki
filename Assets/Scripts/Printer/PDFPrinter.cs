using System.Diagnostics;
using UnityEngine;
using Sirenix.OdinInspector;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Collections;
using System.Collections.Generic;
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

    private void CreatePDFWithImage(Texture2D texture, bool longTicket)
    {
        string imageName = Path.Combine(gameManager.CurrentGameLogFolder, $"map_{imageCounter}.png");
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

    private void CreatePDFWithText(List<string> laws)
    {
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        page.Width = XUnit.FromCentimeter(8);
        page.Height = XUnit.FromCentimeter(327.6); // Long format ticket

        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Arial", 16, XFontStyle.Bold);
    
        double currentY = 0;

        // --- Logo en haut
        string logoPath = Path.Combine(Application.streamingAssetsPath, "logo.png");
        if (File.Exists(logoPath))
        {
            XImage logo = XImage.FromFile(logoPath);
            double maxWidth = page.Width;
            double ratio = logo.PixelHeight / (double)logo.PixelWidth;
            double height = maxWidth * ratio;

            gfx.DrawImage(logo, 0, currentY, maxWidth, height);
            currentY += height;

            // Marge sous le logo
            currentY += XUnit.FromCentimeter(1).Point;
        }
        else
        {
            Debug.LogWarning("Logo introuvable à : " + logoPath);
        }

        // --- Texte centré
        double lineSpacing = font.GetHeight(gfx) + XUnit.FromCentimeter(0.5).Point;

        foreach (string law in laws)
        {
            if (currentY + lineSpacing > page.Height)
            {
                // Nouvelle page
                page = document.AddPage();
                page.Width = XUnit.FromCentimeter(8);
                page.Height = XUnit.FromCentimeter(327.6);
                gfx = XGraphics.FromPdfPage(page);
                currentY = XUnit.FromCentimeter(1).Point; // Marge en haut
            }

            XSize textSize = gfx.MeasureString(law, font);
            double x = (page.Width - textSize.Width) / 2;
            gfx.DrawString(law, font, XBrushes.Black, new XPoint(x, currentY));
            currentY += lineSpacing;
        }

        // --- Sauvegarde
        string outputPath = Path.Combine(pdfPath, pdfFileName);
        document.Save(outputPath);
        Debug.Log("PDF avec logo et texte centré généré : " + outputPath);
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
    
    public void PrintLawsPDF(List<string> laws)
    {
        CreatePDFWithText(laws);
        PrintPDF(pdfFileName, printer2Name);
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
    public void PrintDebug(int printerIndex = 1)
    {
        if (printerIndex == 1)
        {
            PrintPDF(pdfFileNameDebug, printer1Name);
        }
        else if (printerIndex == 2)
        {
            PrintPDF(pdfFileNameDebug, printer2Name);
        }
    }
    
    // -------------------------------------------
    //          GENERATE CHARTE TICKET
    // -------------------------------------------
/*
    private void DrawBackgrounds(Texture2D finalTexture, int imageWidth, int height)
    {
        int y = height;

        y = DrawTexture(finalTexture, "Assets/CHARTE/FOND_DEBUT.png", imageWidth, y);
        for (int i = 0; i < 5; i++)
        {
            y = DrawTexture(finalTexture, "Assets/CHARTE/FOND_MILIEU.png", imageWidth, y);
        }
        y = DrawTexture(finalTexture, "Assets/CHARTE/FOND_FIN.png", imageWidth, y);
    }
    
    private void DrawBorders(Texture2D finalTexture, int imageWidth, int height)
    {
        int y = height;
        string borderPath = "Assets/CHARTE/BORDURES/";
        
        y = DrawTexture(finalTexture, Path.Combine(borderPath, "1_BORDURE_DEBUT.png"), imageWidth, y);
        for (int i = 0; i < 5; i++)
        {
            y = DrawTexture(finalTexture, Path.Combine(borderPath, "1_BORDURE_MILIEU.png"), imageWidth, y);
        }
        y = DrawTexture(finalTexture, Path.Combine(borderPath, "1_BORDURE_FIN.png"), imageWidth, y);
        
        Debug.Log("y position after borders: " + y);
    }

    private void DrawIllustrations(Texture2D finalTexture, int imageWidth, int height, List<string> illustrations)
    {
        int y = height - 400;
        
        string gauche = "Assets/CHARTE/ILLUS_GAUCHE/";
        string droite = "Assets/CHARTE/ILLUS_DROITE/";

        for (int i = 0; i < illustrations.Count; i++)
        {
            string path = (i % 2 == 0 ? gauche : droite) + illustrations[i] + ".png";
            Debug.Log("Drawing illustration: " + illustrations[i]);
            y = DrawTexture(finalTexture, path, imageWidth, y);
        }
    }
    
    private void DrawLaws(Texture2D finalTexture, int imageWidth, int height, List<string> laws)
    {
        int y = height - 400;
        string path = "Assets/CHARTE/LOIS/";
        
        for (int i = 0; i < laws.Count; i++)
        {
            Debug.Log("Drawing law: " + laws[i]);
            y = DrawTexture(finalTexture, path + laws[i] + ".png", imageWidth, y);
        }
    }
    
    private void DrawFrames(Texture2D finalTexture, int imageWidth, int height)
    {
        string framePath = "Assets/CHARTE/CADRES/";
        int y = height;
        
        y = DrawTexture(finalTexture, Path.Combine(framePath, "1_CADRE_DEBUT.png"), imageWidth, y);
        for (int i = 0; i < 5; i++)
        {
            y = DrawTexture(finalTexture, Path.Combine(framePath, "1_CADRE_MILIEU.png"), imageWidth, y);
        }
        y = DrawTexture(finalTexture, Path.Combine(framePath, "1_CADRE_FIN.png"), imageWidth, y);
    }
    
    /*
    private int DrawImage(Texture2D finalTexture, string path, int imageWidth, int y)
    {
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
    }*/
    


    private void GenerateCharteTicket(List<string> laws, List<string> illustrations)
    {
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        page.Width = XUnit.FromCentimeter(8);
        page.Height = XUnit.FromCentimeter(327.6); // Long format ticket

        XGraphics gfx = XGraphics.FromPdfPage(page);
        
        double borderY = 0;
        double frameY = 0;
        double illustrationY = 0;
        double lawY = 0;
        
        string borderPath = "Assets/CHARTE/BORDURES/";
        /*
        string path = Path.Combine(borderPath, "1_BORDURE_DEBUT.png");
        XImage image = XImage.FromFile(path);
        double maxWidth = page.Width;
        double ratio = image.PixelHeight / (double)image.PixelWidth;
        double height = maxWidth * ratio;
        gfx.DrawImage(image, 0, borderY, maxWidth, height);
        borderY += height;*/
        
        string framePath = "Assets/CHARTE/CADRES/";
        
        string apath = Path.Combine(framePath, "1_CADRE_DEBUT.png");
        XImage aimage = XImage.FromFile(apath);
        double amaxWidth = page.Width;
        double aratio = aimage.PixelHeight / (double)aimage.PixelWidth;
        double aheight = amaxWidth * aratio;
        gfx.DrawImage(aimage, 0, frameY, amaxWidth, aheight);
        frameY += aheight;

        // --- Sauvegarde
        string outputPath = Path.Combine(pdfPath, pdfFileName);
        document.Save(outputPath);
        Debug.Log("PDF avec image + texture généré : " + outputPath);
    }
    
    [Button]
    public void PrintCharteTicket(List<string> laws, List<string> illustrations)
    {
        laws = new List<string>();
        illustrations = new List<string>();
        laws.Add("ART_1");
        laws.Add("ART_2");
        laws.Add("ART_3");
        illustrations.Add("POULE");
        illustrations.Add("ROUE");
        
        GenerateCharteTicket(laws, illustrations);
        PrintPDF(pdfFileName, printer1Name);

    }
}
