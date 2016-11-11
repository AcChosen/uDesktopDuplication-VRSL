﻿using UnityEngine;

namespace uDesktopDuplication
{

[AddComponentMenu("uDesktopDuplication/Texture")] 
public class Texture : MonoBehaviour
{
    private Monitor monitor_;
    public Monitor monitor 
    { 
        get { return monitor_; }
        set 
        { 
            monitor_ = value;
            material = GetComponent<Renderer>().material; // clone
            material.mainTexture = monitor_.texture;
            material.SetFloat("_Width", transform.localScale.x);
        }
    }

    public int monitorId
    { 
        get { return monitor.id; }
        set { monitor = Manager.monitors[Mathf.Clamp(value, 0, Manager.monitorCount - 1)]; }
    }

    [Header("Invert UVs")]
    public bool invertX = false;
    public bool invertY = false;

    [Header("Clip")]
    public bool useClip = false;
    public Vector2 clipPos = Vector2.zero;
    public Vector2 clipScale = new Vector2(0.2f, 0.2f);

    public enum Bend
    {
        Off = 0,
        Y = 1,
        Z = 2,
    }

    public Bend bend
    {
        get 
        {
            return (Bend)material.GetInt("_Bend");
        }
        set 
        {
            switch (value) {
                case Bend.Off:
                    material.SetInt("_Bend", 0);
                    material.DisableKeyword("_BEND_OFF");
                    material.DisableKeyword("_BEND_Y");
                    material.DisableKeyword("_BEND_Z");
                    break;
                case Bend.Y:
                    material.SetInt("_Bend", 1);
                    material.DisableKeyword("_BEND_OFF");
                    material.EnableKeyword("_BEND_Y");
                    material.DisableKeyword("_BEND_Z");
                    break;
                case Bend.Z:
                    material.SetInt("_Bend", 2);
                    material.DisableKeyword("_BEND_OFF");
                    material.DisableKeyword("_BEND_Y");
                    material.EnableKeyword("_BEND_Z");
                    break;
            }
        }
    }

    public float radius
    {
        get { return material.GetFloat("_Radius"); }
        set { material.SetFloat("_Radius", value); }
    }

    public float width
    {
        get { return material.GetFloat("_Width"); }
        set { material.SetFloat("_Width", value); }
    }

    public Material material
    {
        get;
        private set;
    }

    void Awake()
    {
        if (!GetComponent<Cursor>())
        {
            gameObject.AddComponent<Cursor>();
        }
    }

    void OnEnable()
    {
        if (monitor == null) {
            monitor = Manager.primary;
        }
    }

    void Update()
    {
        monitor.shouldBeUpdated = true;
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        Invert();
        Rotate();
        Clip();
    }

    void Invert()
    {
        if (invertX) {
            material.EnableKeyword("INVERT_X");
        } else {
            material.DisableKeyword("INVERT_X");
        }

        if (invertY) {
            material.EnableKeyword("INVERT_Y");
        } else {
            material.DisableKeyword("INVERT_Y");
        }
    }

    void Rotate()
    {
        switch (monitor.rotation)
        {
            case MonitorRotation.Identity:
                material.DisableKeyword("ROTATE90");
                material.DisableKeyword("ROTATE180");
                material.DisableKeyword("ROTATE270");
                break;
            case MonitorRotation.Rotate90:
                material.EnableKeyword("ROTATE90");
                material.DisableKeyword("ROTATE180");
                material.DisableKeyword("ROTATE270");
                break;
            case MonitorRotation.Rotate180:
                material.DisableKeyword("ROTATE90");
                material.EnableKeyword("ROTATE180");
                material.DisableKeyword("ROTATE270");
                break;
            case MonitorRotation.Rotate270:
                material.DisableKeyword("ROTATE90");
                material.DisableKeyword("ROTATE180");
                material.EnableKeyword("ROTATE270");
                break;
            default:
                break;
        }
    }

    void Clip()
    {
        if (useClip) {
            material.EnableKeyword("USE_CLIP");
            material.SetVector("_ClipPositionScale", new Vector4(clipPos.x, clipPos.y, clipScale.x, clipScale.y));
        } else {
            material.DisableKeyword("USE_CLIP");
        }
    }
}

}