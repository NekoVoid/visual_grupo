using UnityEngine;
using NativeWebSocket;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using UnityEngine.UI;

public class WebcamSetup : MonoBehaviour
{
  WebCamTexture webcamTexture;
  Texture2D FinalTexture, InitialTexture;
  RawImage image;
  RectTransform rectTransform;
  WebSocket websocket;
  Process server;
  async void Start()
  {
    StartServer();

    SetupWebCam();

    await StartWebSocket();
  }

  // Update is called once per frame
  void Update()
  {

    rectTransform = GetComponent<RectTransform>();
    float width = rectTransform.rect.width;
    float height = rectTransform.rect.height;
    float aspectRatio = width / height;
    float webcamAspectRatio = (float)webcamTexture.width / (float)webcamTexture.height;

    if (webcamAspectRatio > aspectRatio)
    {
      float normalizedDiffHalfed = ((webcamAspectRatio / aspectRatio) - 1f)/2f;

      image.uvRect = new Rect(0 + normalizedDiffHalfed, 0, 1-normalizedDiffHalfed, 1);
    }


    SendWebSocketMessage();
#if !UNITY_WEBGL || UNITY_EDITOR
    websocket.DispatchMessageQueue();
#endif
  }


  private void StartServer()
  {
    string path = Application.dataPath;
    ProcessStartInfo startInfo = new ProcessStartInfo
    {
      FileName = Path.Combine(path, "Python/Python313/python.exe"),
      Arguments = Path.Combine(path, "Scripts/server.py"),
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      CreateNoWindow = true
    };

    server = new Process();
    server.StartInfo = startInfo;

    server.ErrorDataReceived += (sender, e) =>
    {
      if (!string.IsNullOrEmpty(e.Data))
        UnityEngine.Debug.LogError("Server Error: " + e.Data);
    };
    server.OutputDataReceived += (sender, e) =>
    {
      if (!string.IsNullOrEmpty(e.Data))
        UnityEngine.Debug.Log("Server Output: " + e.Data);
    };
    server.Exited += (sender, e) => { UnityEngine.Debug.Log("Server exited."); };

    server.Start();
    server.BeginErrorReadLine();
    server.BeginOutputReadLine();
    
  }
  private Task StartWebSocket()
  {
    websocket = new WebSocket("ws://localhost:3000");

    websocket.OnOpen += async () =>
    {
      UnityEngine.Debug.Log("Connection open!");
      await websocket.SendText(webcamTexture.width + "," + webcamTexture.height);
    };

    websocket.OnMessage += (bytes) => { FinalTexture.LoadImage(bytes); };

    websocket.OnError += (e) => { UnityEngine.Debug.Log("Error! " + e); };
    websocket.OnClose += (e) => { UnityEngine.Debug.Log("Connection closed!"); };

    return websocket.Connect();
  }

  private void SetupWebCam()
  {
    image = GetComponent<RawImage>();
    RectTransform rectTransform = image.GetComponent<RectTransform>();
    WebCamDevice my_device = WebCamTexture.devices[0];
    webcamTexture = new WebCamTexture(my_device.name, (int)rectTransform.rect.width, (int)rectTransform.rect.height);

    FinalTexture = new Texture2D(webcamTexture.requestedWidth, webcamTexture.requestedHeight, TextureFormat.RGB24, false);
    InitialTexture = new Texture2D(webcamTexture.requestedWidth, webcamTexture.requestedHeight, TextureFormat.RGB24, false);

    image.texture = FinalTexture;
    
    webcamTexture.Play();
  }

  private async void SendWebSocketMessage()
  {
    if (websocket.State == WebSocketState.Open && webcamTexture.didUpdateThisFrame)
    {
      if (InitialTexture.width != webcamTexture.width || InitialTexture.height != webcamTexture.height)
      {
        InitialTexture.Reinitialize(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        FinalTexture.Reinitialize(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
      }
      

      InitialTexture.SetPixels32(webcamTexture.GetPixels32());
      byte[] imageBytes = InitialTexture.EncodeToJPG();

      await websocket.Send(imageBytes);
    }
  }

  private async void OnApplicationQuit()
  {
    await websocket.Close();
    if(!server.HasExited) server.Kill();
    server.Dispose();
  }
}
