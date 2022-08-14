using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;
using YoloV4Tiny;

public class InferenceYOLO : WebCamera
{
    [SerializeField, Range(0, 1)] float _threshold = 0.5f;
    [SerializeField] YoloV4Tiny.ResourceSet _resources = null;
    [SerializeField] Marker _markerPrefab = null;

    ObjectDetector _detector;
    Marker[] _markers = new Marker[50];

    protected override void Awake()
    {
        base.Awake();

        _detector = new ObjectDetector(_resources);
        for (var i = 0; i < _markers.Length; i++)
        {
            _markers[i] = Instantiate(_markerPrefab, this.transform);
        }
    }

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        _detector.ProcessImage(input, _threshold);

        var i = 0;
        foreach (var d in _detector.Detections)
        {
            if (i == _markers.Length) break;
            _markers[i++].SetAttributes(d);
        }

        for (; i < _markers.Length; i++)
        {
            _markers[i].Hide();
        }

        output = toTexture2D(input);
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
