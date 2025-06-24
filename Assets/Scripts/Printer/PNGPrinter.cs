using System.Diagnostics;
using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;


public class PNGPrinter : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private string pngFileNameDebug;
    
    private GameManager gameManager;
    private string printer1Name = "EPSON TM-T20 Receipt";
    private string printer2Name = "EPSON TM-T20 Receipt5";
    private string logsFolder = Application.dataPath + "/Logs/";
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
    
    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
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
        Debug.Log("Printing PNG : " + fileName + " to printer: " + printerName);
    }
    
    // -------------------------------------------
    //             GENERATE MAP TICKET
    // -------------------------------------------
    
    private Texture2D CaptureCameraSquare(Camera cam, int size)
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
    
    private void TextureToPNG(Texture2D texture)
    {
        string imageName = Path.Combine(gameManager.CurrentGameLogFolder, $"map_{imageCounter}.png");
        byte[] bytes = texture.EncodeToPNG();
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
    
        currentY -= 75; // Add a small margin after the header
        
        // Draw camera image
        int camH = Mathf.RoundToInt(cameraTex.height * (imageWidth / (float)cameraTex.width));
        currentY -= camH;
        Texture2D resizedCam = ResizeTexture(cameraTex, imageWidth, camH);
        finalTexture.SetPixels(0, currentY, imageWidth, camH, resizedCam.GetPixels());
    
        finalTexture.Apply();
    
        string outputPath = Path.Combine(gameManager.CurrentGameLogFolder, $"ticket.png");
        File.WriteAllBytes(outputPath, finalTexture.EncodeToPNG());
    }
    
    public IEnumerator PrintMapTicket(bool longTicket = false)
    {
        yield return new WaitForEndOfFrame();
        const int size = 512;
        Texture2D mapScreen = CaptureCameraSquare(camera, size);
        TextureToPNG(mapScreen);
        GenerateMapTicket();
        imageCounter++;
        PrintPNG((Path.Combine(gameManager.CurrentGameLogFolder, $"ticket.png")), printer1Name);
    }
    
    // -------------------------------------------
    //          GENERATE LANDMARK TICKET
    // -------------------------------------------

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
        
        // To complete
    }
    
    public IEnumerator PrintLandmarkTicket(int percentage)
    {
        yield return new WaitForEndOfFrame();
        GenerateLandmarkTicket(percentage);
        imageCounter++;
        PrintPNG((Path.Combine(gameManager.CurrentGameLogFolder, $"ticket.png")), printer2Name);
    }
}