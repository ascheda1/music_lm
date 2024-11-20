using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using TMPro;

public class MusicLMController : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

    public const int KEYEVENTF_EXTENTEDKEY = 1;
    public const int KEYEVENTF_KEYUP = 0;
    public const int VK_MEDIA_NEXT_TRACK = 0xB0;// code to jump to next track
    public const int VK_MEDIA_PLAY_PAUSE = 0xB3;// code to play or pause a song
    public const int VK_MEDIA_PREV_TRACK = 0xB1;// code to jump to prev track
    public const int VK_VOLUME_DOWN = 0xAE;
    public const int VK_VOLUME_UP = 0xAF;
    public GameObject index_end;
    public GameObject thumb_end;
    public GameObject pinky_end;
    GameObject index_sphere;
    GameObject thumb_sphere;
    GameObject pinky_sphere;

    private int slowUp = 0;
    // 0.2-0.3 deadzone
    public float index_thumb_distance = 0.0f;
    public float pinky_thumb_distance = 0.0f;

    private float finger_distance_limit = 0.03f;

    public Vector3 last_index_pos = Vector3.zero;
    public TextMeshProUGUI tmpro;
    public TextMeshProUGUI tmproMat;
    // Start is called before the first frame update
    void Start()
    {
        initFingers();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!index_end.activeInHierarchy)
        {
            last_index_pos = Vector3.zero;
            tmpro.text = "Wave your hand to control music...";
            return;
        }
        VolumeSet();
        SkipSet();
        PauseSet();
        VisualizeFingers();

        tmproMat.text = index_end.transform.position + "\n"+
            index_end.transform.rotation + "\n" +
            index_end.transform.localScale + "\n" + 
            index_thumb_distance.ToString() + "\n" + 
            pinky_thumb_distance.ToString();
    }
    private void PauseSet()
    {
        float new_distance = (pinky_end.transform.position - thumb_end.transform.position).magnitude;
        if (pinky_thumb_distance > finger_distance_limit)
        {
            if (new_distance < finger_distance_limit)
            {
                tmpro.text = "Pause/Play";
                ppTrack();
            }
        }
        pinky_thumb_distance = new_distance;
    }
    private void SkipSet()
    {        
        index_thumb_distance = (index_end.transform.position - thumb_end.transform.position).magnitude;

        if (index_thumb_distance < finger_distance_limit)
        {
            return;
        }
        if (Mathf.Sign(index_end.transform.position.x) + Mathf.Sign(last_index_pos.x) == 0)
        {
            if (last_index_pos.x > 0)
            {
                tmpro.text = ">>";
                nextTrack();
            }

            else if (last_index_pos.x < 0)
            {
                tmpro.text = "<<";
                prevTrack();
            }
        }
        last_index_pos = index_end.transform.position;
    }

    private void VolumeSet()
    {
        index_thumb_distance = (index_end.transform.position - thumb_end.transform.position).magnitude;

        if (index_thumb_distance < finger_distance_limit)
        {
            slowUp++;
            if (slowUp % 10 != 0)
            {
                return;
            }
            slowUp = 0;
            if (index_end.transform.position.y > 0.3f)
            {
                tmpro.text = "Volume+";
                volumeUp();
            }
            if (index_end.transform.position.y < 0.2f)
            {
                tmpro.text = "Volume-";
                volumeDown();
            }
        }
    }

    private void VisualizeFingers()
    {
        index_sphere.transform.position = index_end.transform.position;
        thumb_sphere.transform.position = thumb_end.transform.position;
        pinky_sphere.transform.position = pinky_end.transform.position;
    }
    private void initFingers()
    {
        //index
        index_sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        index_sphere.transform.localScale = Vector3.one * 0.01f;
        Material matIndex = new Material(Shader.Find("Standard"));
        matIndex.color = Color.green;
        index_sphere.GetComponent<Renderer>().material = matIndex;
        index_sphere.transform.parent = index_end.transform;
        //index_sphere.transform.position = Vector3.zero;

        //thumb
        thumb_sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        thumb_sphere.transform.localScale = Vector3.one * 0.01f;
        Material matThumb = new Material(Shader.Find("Standard"));
        matThumb.color = Color.red;
        thumb_sphere.GetComponent<Renderer>().material = matThumb;
        thumb_sphere.transform.parent = thumb_end.transform;
        //thumb_sphere.transform.position = Vector3.zero;

        //pinky
        pinky_sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pinky_sphere.transform.localScale = Vector3.one * 0.01f;
        Material matPinky = new Material(Shader.Find("Standard"));
        matPinky.color = Color.yellow;
        pinky_sphere.GetComponent<Renderer>().material = matPinky;
        pinky_sphere.transform.parent = pinky_end.transform;
        //thumb_sphere.transform.position = Vector3.zero;
    }
    private void prevTrack()
    {
        // Jump to previous track
        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
    }
    private void nextTrack()
    {
        // Jump to next track
        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
    }

    private void ppTrack()
    {
        // Play or Pause music
        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
    }

    private void volumeUp()
    {
        keybd_event(VK_VOLUME_UP, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
    }

    private void volumeDown()
    {
        keybd_event(VK_VOLUME_DOWN, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
    }
}



