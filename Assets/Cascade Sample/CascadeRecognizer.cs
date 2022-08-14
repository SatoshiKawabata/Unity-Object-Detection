using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;

public class CascadeRecognizer : WebCamera
{
    public TextAsset faces;
    private CascadeClassifier cascadeFaces;

    protected override void Awake()
    {
        base.Awake();

        // classifier
        FileStorage storageFaces = new FileStorage(faces.text, FileStorage.Mode.Read | FileStorage.Mode.Memory);
        cascadeFaces = new CascadeClassifier();
        if (!cascadeFaces.Read(storageFaces.GetFirstTopLevelNode()))
        {
            throw new System.Exception("FaceProcessor.Initialize: Failed to load faces cascade classifier");
        }
    }

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        Mat image = OpenCvSharp.Unity.TextureToMat(input);
        Mat gray = image.CvtColor(ColorConversionCodes.BGR2GRAY);
        Mat equalizeHistMat = new Mat();
        Cv2.EqualizeHist(gray, equalizeHistMat);
        OpenCvSharp.Rect[] rawFaces = cascadeFaces.DetectMultiScale(gray, 1.1, 6);
        foreach (var faceRect in rawFaces)
        {
            Cv2.Rectangle((InputOutputArray)image, faceRect, Scalar.LightGreen, 2);
        }
        output = OpenCvSharp.Unity.MatToTexture(image);
        return true;
    }


    private Texture2D toTexture2D(Texture tex)
    {
        var sw = tex.width;
        var sh = tex.height;
        var format = TextureFormat.RGBA32;
        var result = new Texture2D(sw, sh, format, false);
        var currentRT = RenderTexture.active;
        var rt = new RenderTexture(sw, sh, 32);
        Graphics.Blit(tex, rt);
        RenderTexture.active = rt;
        var source = new UnityEngine.Rect(0, 0, rt.width, rt.height);
        result.ReadPixels(source, 0, 0);
        result.Apply();
        RenderTexture.active = currentRT;
        return result;
    }
}