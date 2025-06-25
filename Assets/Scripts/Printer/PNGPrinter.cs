using System.Diagnostics;
using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;


public class PNGPrinter : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera textCamera;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private string pngFileNameDebug;
    
    private GameManager gameManager;
    private string printer1Name = "EPSON TM-T20 Receipt";
    private string printer2Name = "EPSON TM-T20 Receipt5";
    private string logsFolder = Application.dataPath + "/Logs/";
    [SerializeField] private string ticketPath = Path.Combine(Application.dataPath + "/Logs/", $"ticket.png");
    private string sumatraPath = Application.dataPath + "/StreamingAssets/SumatraPDF/SumatraPDF.exe";
    private string imageMagikPath = Application.dataPath + "/StreamingAssets/ImageMagick/magick.exe";
    private int imageCounter;

    private const int dpi = 300;
    private const float cmToInch = 0.393701f;
    
    private int lawIncrement = 0;
    private int illustrationIncrement = 0;
    
    void Start()
    {
        gameManager = GameManager.Instance;
        Initialize();
    }

    public void Initialize()
    {
        imageCounter = 0;
        ticketPath = Path.Combine(gameManager.CurrentGameLogFolder, $"ticket.png");
        lawIncrement = 0;
        illustrationIncrement = 0;
    }
    
    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(newWidth, newHeight,  TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }
    
    [Button]
    public void PrintPNG(string fileName, string printerName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = sumatraPath,
            Arguments = $"-print-to \"{printerName}\" -print-settings \"fit\" \"{fileName}\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };

        Process.Start(startInfo);
        //Debug.Log("Printing PNG : " + fileName + " to printer: " + printerName);
    }
    
    // -------------------------------------------
    //             GENERATE MAP TICKET
    // -------------------------------------------

    private void CaptureMapCamera(Camera cam, int size)
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

        // Texture to PNG
        string imageName = Path.Combine(gameManager.CurrentGameLogFolder, $"map_{imageCounter}.png");
        byte[] bytes = result.EncodeToPNG();
        string filePath = Path.Combine(logsFolder, imageName);
        File.WriteAllBytes(filePath, bytes);
    }

    private void GenerateMapTicket()
    {
        // Ticket dimensions
        const int dpi = 300;
        float cmToInch = 0.393701f;
        int fullWidth = Mathf.RoundToInt(8f * cmToInch * dpi);     // ~945 px
        int borderRight = Mathf.RoundToInt(1f * cmToInch * dpi);   // ~118 px
        int imageWidth = fullWidth - borderRight;                 // ~827 px
        int height = Mathf.RoundToInt(29.7f * cmToInch * dpi);     // ~3508 px
    
        // Create the texture
        Texture2D finalTexture = new Texture2D(fullWidth, height, TextureFormat.RGB24, false);
        Color32[] whiteFill = new Color32[fullWidth * height];
        for (int i = 0; i < whiteFill.Length; i++) whiteFill[i] = Color.white;
        finalTexture.SetPixels32(whiteFill);
    
        // Load the header
        string headerPath = Path.Combine(Application.streamingAssetsPath, "PrinterAssets/LIGNE_SUPERIEURE.png");
        Texture2D headerTex = new Texture2D(2, 2);
        headerTex.LoadImage(File.ReadAllBytes(headerPath));
    
        // Load the map image
        string cameraImagePath = Path.Combine(gameManager.CurrentGameLogFolder, $"map_{imageCounter}.png");
        Texture2D cameraTex = new Texture2D(2, 2);
        cameraTex.LoadImage(File.ReadAllBytes(cameraImagePath));
    
        int currentY = height;
    
        // Draw header
        int headerH = Mathf.RoundToInt(headerTex.height * (imageWidth / (float)headerTex.width));
        currentY -= headerH;
        Texture2D resizedHeader = ResizeTexture(headerTex, imageWidth, headerH);
        finalTexture.SetPixels(0, currentY, imageWidth, headerH, resizedHeader.GetPixels());
    
        currentY -= 75; // Add a space
        
        // Draw map image
        int camH = Mathf.RoundToInt(cameraTex.height * (imageWidth / (float)cameraTex.width));
        currentY -= camH;
        Texture2D resizedCam = ResizeTexture(cameraTex, imageWidth, camH);
        finalTexture.SetPixels(0, currentY, imageWidth, camH, resizedCam.GetPixels());
    
        finalTexture.Apply();
        File.WriteAllBytes(ticketPath, finalTexture.EncodeToPNG());
    }
    
    public IEnumerator PrintMapTicket(bool longTicket = false)
    {
        yield return new WaitForEndOfFrame();
        const int size = 512;
        CaptureMapCamera(playerCamera, size);
        GenerateMapTicket();
        PrintPNG(ticketPath, printer1Name);
        imageCounter++;
    }
    
    // -------------------------------------------
    //          GENERATE LANDMARK TICKET
    // -------------------------------------------
    
    private void CaptureTextCamera(int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        textCamera.targetTexture = rt;

        Texture2D result = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Important si tu modifies le texte juste avant
        Canvas.ForceUpdateCanvases();

        textCamera.Render();

        RenderTexture.active = rt;
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        textCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        byte[] bytes = result.EncodeToPNG();
        File.WriteAllBytes(ticketPath, bytes);
    }
    
    private void GenerateLandmarkTicket(int percentage)
    {
        // Ticket dimensions
        const int dpi = 300;
        float cmToInch = 0.393701f;
        int fullWidth = Mathf.RoundToInt(8f * cmToInch * dpi);     // ~945 px
        int borderRight = Mathf.RoundToInt(1f * cmToInch * dpi);   // ~118 px
        int imageWidth = fullWidth - borderRight;                 // ~827 px
        int height = Mathf.RoundToInt(29.7f * cmToInch * dpi);     // ~3508 px
    
        // Create the texture
        Texture2D finalTexture = new Texture2D(fullWidth, height, TextureFormat.RGB24, false);
        Color32[] whiteFill = new Color32[fullWidth * height];
        for (int i = 0; i < whiteFill.Length; i++) whiteFill[i] = Color.white;
        finalTexture.SetPixels32(whiteFill);
        
        // Load the header
        string headerPath = Path.Combine(Application.streamingAssetsPath, "PrinterAssets/DEBUT_POURCENTAGE.png");
        Texture2D headerTex = new Texture2D(2, 2);
        headerTex.LoadImage(File.ReadAllBytes(headerPath));
        
        // Capture the text camera
        percentageText.text = $"{percentage}%";
        CaptureTextCamera(512, 512/4);
        
            // Load the footer
            string footerPath = Path.Combine(Application.streamingAssetsPath, "PrinterAssets/FIN_POURCENTAGE.png");
            Texture2D footerTex = new Texture2D(2, 2);
            footerTex.LoadImage(File.ReadAllBytes(footerPath));
            
            int currentY = height;
            
            // Draw header
            int headerH = Mathf.RoundToInt(headerTex.height * (imageWidth / (float)headerTex.width));
            currentY -= headerH;
            Texture2D resizedHeader = ResizeTexture(headerTex, imageWidth, headerH);
            finalTexture.SetPixels(0, currentY, imageWidth, headerH, resizedHeader.GetPixels());
            
        // Load the text image
        string cameraImagePath = ticketPath;
        Texture2D cameraTex = new Texture2D(2, 2);
        cameraTex.LoadImage(File.ReadAllBytes(cameraImagePath));
        
        // Draw map image
        int camH = Mathf.RoundToInt(cameraTex.height * (imageWidth / (float)cameraTex.width));
        currentY -= camH;
        Texture2D resizedCam = ResizeTexture(cameraTex, imageWidth, camH);
        finalTexture.SetPixels(0, currentY, imageWidth, camH, resizedCam.GetPixels());
        
        // Draw header
        int footerH = Mathf.RoundToInt(footerTex.height * (imageWidth / (float)footerTex.width));
        currentY -= footerH;
        Texture2D resizedFooter = ResizeTexture(footerTex, imageWidth, footerH);
        finalTexture.SetPixels(0, currentY, imageWidth, footerH, resizedFooter.GetPixels());
        
        finalTexture.Apply();
        File.WriteAllBytes(ticketPath, finalTexture.EncodeToPNG());
    }
    
    [Button]
    public void PrintLandmarkTicket(int percentage)
    {
        GenerateLandmarkTicket(percentage);
        PrintPNG(ticketPath, printer2Name);
    }
    
    
    // -------------------------------------------
    //          GENERATE CHARTE TICKET
    // -------------------------------------------

    private void BlendTexture(Texture2D background, Texture2D overlay, int posX, int posY)
    {
        Color[] bgPixels = background.GetPixels();
        Color[] ovPixels = overlay.GetPixels();

        int bgWidth = background.width;
        int bgHeight = background.height;
        int ovWidth = overlay.width;
        int ovHeight = overlay.height;

        for (int y = 0; y < ovHeight; y++)
        {
            int bgY = posY + y;
            if (bgY < 0 || bgY >= bgHeight) continue;

            for (int x = 0; x < ovWidth; x++)
            {
                int bgX = posX + x;
                if (bgX < 0 || bgX >= bgWidth) continue;

                int bgIndex = bgY * bgWidth + bgX;
                int ovIndex = y * ovWidth + x;

                Color bgColor = bgPixels[bgIndex];
                Color ovColor = ovPixels[ovIndex];

                float alpha = ovColor.a;
                if (alpha == 0) continue; // Rien à faire si overlay totalement transparent

                // Fusion alpha
                Color blended = Color.Lerp(bgColor, ovColor, alpha);

                bgPixels[bgIndex] = blended;
            }
        }

        background.SetPixels(bgPixels);
        background.Apply();
    }

    private int DrawTexture(Texture2D finalTexture, string path, int imageWidth, int y)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(File.ReadAllBytes(path));
        int texHeigth = Mathf.RoundToInt(texture.height * (imageWidth / (float)texture.width));
        y -= texHeigth;
        Texture2D resizedTexture = ResizeTexture(texture, imageWidth, texHeigth);
        BlendTexture(finalTexture, resizedTexture, 0, y);

        return y;
    }

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
    
    private void DrawBorders(Texture2D finalTexture, int imageWidth, int height, int page)
    {
        int y = height;
        string borderPath = "Assets/CHARTE/BORDURES/";
        
        y = DrawTexture(finalTexture, Path.Combine(borderPath, $"{page + 1}_BORDURE_DEBUT.png"), imageWidth, y);
        for (int i = 0; i < 5; i++)
        {
            y = DrawTexture(finalTexture, Path.Combine(borderPath, $"{page + 1}_BORDURE_MILIEU.png"), imageWidth, y);
        }
        y = DrawTexture(finalTexture, Path.Combine(borderPath, $"{page + 1}_BORDURE_FIN.png"), imageWidth, y);
        
    }

    private void DrawIllustrations(Texture2D finalTexture, int imageWidth, int height, List<string> illustrations)
    {
        int y = height - 400;
        
        string gauche = "Assets/CHARTE/ILLUS_GAUCHE/";
        string droite = "Assets/CHARTE/ILLUS_DROITE/";

        int i = 0;
        while (i < 3)
        {
            Debug.Log("ILLUS = " + illustrationIncrement + " / " + illustrations.Count);
            if (illustrationIncrement >= illustrations.Count) break; // Si pas assez d'illustrations, on arrête
            string path = (i % 2 == 0 ? gauche : droite) + illustrations[illustrationIncrement] + ".png";
            y = DrawTexture(finalTexture, path, imageWidth, y);
            i++;
            illustrationIncrement++;
        }
    }
    
    private void DrawLaws(Texture2D finalTexture, int imageWidth, int height, List<string> laws)
    {
        int y = height - 400;
        string path = "Assets/CHARTE/LOIS/";

        int i = 0;
        while (i < 5)
        {
            if (lawIncrement >= laws.Count) break; // Si pas assez de lois, on arrête
            y = DrawTexture(finalTexture, path + laws[lawIncrement] + ".png", imageWidth, y);
            lawIncrement++;
            i++;
        }
        
    }
    
    private void DrawFrames(Texture2D finalTexture, int imageWidth, int height, int page)
    {
        string framePath = "Assets/CHARTE/CADRES/";
        int y = height;
        
        y = DrawTexture(finalTexture, Path.Combine(framePath, $"{page + 1}_CADRE_DEBUT.png"), imageWidth, y);
        for (int i = 0; i < 5; i++)
        {
            y = DrawTexture(finalTexture, Path.Combine(framePath, $"{page + 1}_CADRE_MILIEU.png"), imageWidth, y);
        }
        y = DrawTexture(finalTexture, Path.Combine(framePath, $"{page + 1}_CADRE_FIN.png"), imageWidth, y);
    }
    
    private void GenerateCharteTicket(List<string> laws, List<string> illustrations)
    {
        // Ticket dimensions
        int fullWidth = Mathf.RoundToInt(8f * cmToInch * dpi);     // ~945 px
        int borderRight = Mathf.RoundToInt(1f * cmToInch * dpi);   // ~118 px
        int imageWidth = fullWidth - borderRight;                 // ~827 px
        int height = Mathf.RoundToInt(29.7f * cmToInch * dpi);     // ~3508 px
        
        

        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Generating charte ticket chunk: " + i);
            // Create the texture
            Texture2D finalTexture = new Texture2D(fullWidth, height, TextureFormat.RGBA32, false);
            Color32[] whiteFill = new Color32[fullWidth * height];
            for (int k = 0; k < whiteFill.Length; k++) whiteFill[k] = new Color32(255, 255, 255, 255);
            finalTexture.SetPixels32(whiteFill);
            
            DrawBackgrounds(finalTexture, imageWidth, height); // Backgrounds
            DrawBorders(finalTexture, imageWidth, height, i); // Borders
            DrawIllustrations(finalTexture, imageWidth, height, illustrations); // Illustrations
            DrawLaws(finalTexture, imageWidth, height, laws); // Laws
            DrawFrames(finalTexture, imageWidth, height, i); // Frames
            if (i == 0)
                DrawTexture(finalTexture, "Assets/CHARTE/DEBUT_CHARTE.png", imageWidth, height); // Charte header
            if (i == 2)
                DrawTexture(finalTexture, "Assets/CHARTE/FIN_CHARTE.png", imageWidth, 150); // Charte header
            
            finalTexture.Apply();
            
            // Editor
            if (gameManager == null)
            {
                Debug.Log("Saving charte ticket chunk to logs folder: " + logsFolder + $"/charte{i}.png");
                File.WriteAllBytes(logsFolder + $"/charte{i}.png", finalTexture.EncodeToPNG());
            }
            // Runtime
            else
                File.WriteAllBytes(gameManager.CurrentGameLogFolder + $"/charte{i}.png", finalTexture.EncodeToPNG());
        }
    }

    void GenerateWhitePNG()
    {
        // Ticket dimensions
        int fullWidth = Mathf.RoundToInt(8f * cmToInch * dpi);     // ~945 px
        int height = Mathf.RoundToInt(135f * cmToInch * dpi);     // ~3508 px

        // Create the texture
        Texture2D finalTexture = new Texture2D(fullWidth, height, TextureFormat.RGBA32, false);
        Color32[] whiteFill = new Color32[fullWidth * height];
        for (int i = 0; i < whiteFill.Length; i++) whiteFill[i] = new Color32(255, 255, 255, 255);
        finalTexture.SetPixels32(whiteFill);
        
        finalTexture.Apply();
        
        File.WriteAllBytes(logsFolder + "charte2.png", finalTexture.EncodeToPNG());
    }
    
    void GenerateWhitePNG2()
    {
        // Ticket dimensions
        int fullWidth = Mathf.RoundToInt(8f * cmToInch * dpi);     // ~945 px
        int height = Mathf.RoundToInt(57.6f * cmToInch * dpi);     // ~3508 px

        // Create the texture
        Texture2D finalTexture = new Texture2D(fullWidth, height, TextureFormat.RGBA32, false);
        Color32[] whiteFill = new Color32[fullWidth * height];
        for (int i = 0; i < whiteFill.Length; i++) whiteFill[i] = new Color32(255, 255, 255, 255);
        finalTexture.SetPixels32(whiteFill);
        
        finalTexture.Apply();
        
        File.WriteAllBytes(logsFolder + "charte3.png", finalTexture.EncodeToPNG());
    }
    
    void MergePNGChunks(List<string> chunkPaths, string outputPath)
    {
        string args = "";
        foreach (var path in chunkPaths)
        {
            args += $"\"{path}\" ";
        }
        args += $"-append "; // Append vertically
        
        args += $"\"{outputPath}\"";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = imageMagikPath,
            Arguments = args,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };

        using (Process process = Process.Start(psi))
        {
            process.WaitForExit();
            Debug.Log("ImageMagick output: " + process.StandardOutput.ReadToEnd());
            Debug.LogError("ImageMagick errors: " + process.StandardError.ReadToEnd());
        }
    }
    
    [Button]
    public void PrintCharteTicket(List<string> laws, List<string> illustrations)
    {
        lawIncrement = 0;
        illustrationIncrement = 0;
        
        laws = new List<string>();
        illustrations = new List<string>();
        laws.Add("ART_1");
        laws.Add("ART_2");
        laws.Add("ART_3");
        laws.Add("ART_4");
        laws.Add("ART_5");
        laws.Add("ART_6");
        laws.Add("EGO_1");
        laws.Add("EGO_2");
        laws.Add("EGO_3");
        laws.Add("EGO_4");
        laws.Add("EGO_5");
        laws.Add("EGO_6");
        illustrations.Add("POULE");
        illustrations.Add("ROUE");
        illustrations.Add("CYGNE");
        illustrations.Add("BATEAU");
        illustrations.Add("LAMPADAIRE");
        illustrations.Add("ARBRE");
        illustrations.Add("BALANCE");
        illustrations.Add("BOWLING");
        
        GenerateCharteTicket(laws, illustrations);
        
        /*
        GenerateWhitePNG();
        GenerateWhitePNG2();
        List<string> chunks = new List<string>
        {
            logsFolder + "charte.png",
            logsFolder + "charte2.png",
            logsFolder + "charte3.png"
        };
        string outputPath = logsFolder + "charte_finale.png";
        MergePNGChunks(chunks, outputPath); */
        
        
        // Editor
        if (gameManager == null)
        {
            PrintPNG(logsFolder + "charte0.png", printer2Name);
            PrintPNG(logsFolder + "charte1.png", printer2Name);
            PrintPNG(logsFolder + "charte2.png", printer2Name);
        }
        // Runtime
        else
        {
            PrintPNG(gameManager.CurrentGameLogFolder + "/charte0.png", printer2Name);
            PrintPNG(gameManager.CurrentGameLogFolder + "/charte1.png", printer2Name);
            PrintPNG(gameManager.CurrentGameLogFolder + "/charte2.png", printer2Name);
        }
    }

    [Button]
    void Test()
    {
        List<string> chunks = new List<string>
        {
            Path.GetFullPath(Path.Combine(logsFolder, "charte.png")),
            Path.GetFullPath(Path.Combine(logsFolder, "charte2.png")),
        };
        string outputPath = logsFolder + "charte_finale.png";
        PrintPNG(logsFolder + "charte_finale.png", printer1Name);
        //MergePNGChunks(chunks, outputPath);
    }
}