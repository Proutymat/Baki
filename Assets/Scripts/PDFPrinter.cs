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
    [SerializeField] private string fileName = "receipt.pdf";
    [SerializeField] private string pdfPath;
    private string sumatraPath = @"C:\Users\ELE66e033cdbc70f\AppData\Local\SumatraPDF";
    private string printerName = "EPSON TM-T20 Receipt";
    private string folderPath;

    void Start()
    {
        folderPath = Application.dataPath + "/Logs";
        pdfPath = Application.dataPath + "/Logs/";
        sumatraPath = @"C:\Users\ELE66e033cdbc70f\AppData\Local\SumatraPDF\SumatraPDF.exe";
    }

    private string GetUniqueImagePath()
    {
        int counter = 0;
        string tempImagePath;

        do
        {
            tempImagePath = Path.Combine(folderPath, $"capture_{counter}.png");
            counter++;
        } while (File.Exists(tempImagePath));

        return tempImagePath;
    }

    private void CreatePDFWithImage(Texture2D tex)
    {
        // Obtenir un nom de fichier unique
        string tempImagePath = GetUniqueImagePath();

        // Convertir Texture2D en PNG
        byte[] bytes = tex.EncodeToPNG();

        // Sauvegarder l’image en fichier
        using (FileStream fs = new FileStream(tempImagePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            fs.Write(bytes, 0, bytes.Length);
        }

        System.Threading.Thread.Sleep(100); // Attendre un peu pour éviter les conflits

        // Créer le document PDF
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        page.Width = XUnit.FromCentimeter(8);
        page.Height = XUnit.FromCentimeter(29.7);

        XGraphics gfx = XGraphics.FromPdfPage(page);
        XImage image = XImage.FromFile(tempImagePath);

        double imageWidth = page.Width.Point;
        double imageHeight = imageWidth;
        gfx.DrawImage(image, 0, 0, imageWidth, imageHeight);

        string outputPath = Path.Combine(pdfPath, fileName);
        document.Save(outputPath);
        UnityEngine.Debug.Log("PDF généré à : " + outputPath);

        // Image non supprimée (tu peux ajouter un bouton de nettoyage si besoin)
    }


    
    public Texture2D CaptureCameraSquare(Camera cam, int size)
    {
        // Crée un RenderTexture carré
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
    
	[Button]
    public void PrintPDF()
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = sumatraPath,
            Arguments = $"-print-to \"{printerName}\" -print-settings \"fit\" \"{pdfPath + fileName}\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };

        System.Diagnostics.Process.Start(startInfo);
    }
    
    [Button]
    public IEnumerator Print()
    {
        yield return new WaitForEndOfFrame();
        const int size = 512; // ou 1024 selon ta qualité
        Texture2D square = CaptureCameraSquare(camera, size);
        CreatePDFWithImage(square);
        PrintPDF();
    }
}
