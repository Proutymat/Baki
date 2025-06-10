using System.Diagnostics;
using UnityEngine;
using Sirenix.OdinInspector;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Collections;




public class PDFPrinter : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private string pdfFileNameDebug;
    
    private GameManager gameManager;
    private string printerName = "EPSON TM-T20 Receipt";
    private string pdfFileName = "map.pdf";
    private string pdfPath = Application.dataPath + "/Logs/";
    private string sumatraPath = Application.dataPath + "/StreamingAssets/SumatraPDF/SumatraPDF.exe";
    private int imageCounter;

    void Start()
    {
        gameManager = GameManager.Instance;
        Initialize();
    }

    private void Initialize()
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
        XImage image = XImage.FromFile(imageName);

        double imageWidth = page.Width.Point;
        double imageHeight = imageWidth;
        gfx.DrawImage(image, 0, 0, imageWidth, imageHeight);

        string outputPath = Path.Combine(pdfPath, pdfFileName);
        document.Save(outputPath);
        UnityEngine.Debug.Log("PDF generated at : " + outputPath);
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
    
    public void PrintPDF(string fileName)
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = sumatraPath,
            Arguments = $"-print-to \"{printerName}\" -print-settings \"fit,bin=B_BOTH,margins=small\" \"{pdfPath + fileName}\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };

        System.Diagnostics.Process.Start(startInfo);
    }


    public IEnumerator Print(bool longTicket = false)
    {
        yield return new WaitForEndOfFrame();
        const int size = 512;
        Texture2D square = CaptureCameraSquare(camera, size);
        CreatePDFWithImage(square, longTicket);
        PrintPDF(pdfFileName);
    }
    
    [Button]
    public void PrintDebug()
    {
        const int size = 512; // ou 1024 selon ta qualité
        Texture2D square = CaptureCameraSquare(camera, size);
        PrintPDF(pdfFileNameDebug);
    }
}
