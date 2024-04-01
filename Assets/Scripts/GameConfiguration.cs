using UnityEngine;

public class GameConfiguration : MonoBehaviour
{
    public static bool withProgressBar = false;
    //public static GameConfiguration gc;
    public static char mode = 'P';  //by default is pinch
    /*
    public const float POSA_X = -0.1801654f;
    public const float POSA_Y = 0.7907997f;
    public const float POSA_Z = 0.301839f;
    public const float POSB_X = 0.1921346f;
    public const float POSB_Y = 0.7907997f;
    public const float POSB_Z = 0.301839f;
    */
    public const float POSA_X = -0.1001654f;
    public const float POSA_Y = 0.7907997f;
    public const float POSA_Z = 0.251839f;
    public const float POSB_X = 0.1121346f;
    public const float POSB_Y = 0.7907997f;
    public const float POSB_Z = 0.251839f;

    //targetforce = X * weight + Y;

    /*
    //development testing (old sensor)
    //targetForce = 0.5451f * weight + 268.73f;  //thumb
    //public const float THUMB_X = 0.5451f;
    //public const float THUMB_Y = 268.73f;
    */

    /*
    //1-10 tester linear
    //y = 0.604x + 371.4
    public const float THUMB_X = 0.604f;
    public const float THUMB_Y = 371.4f;
    */

    //19-20 tester loge
    //y = 307.07ln(x) - 1295.9
    // public const float THUMB_X = 307.07f;
    // public const float THUMB_Y = -1295.9f;
    //study4
    // public const float THUMB_X = 279.64f;
    // public const float THUMB_Y = -1202.6f;

    /*
    //development testing (old sensor)
    //targetForce = 0.0679f * weight + 143.02f;  //palm
    //public const float PALM_X = 0.0679f;
    //public const float PALM_Y = 143.02f;
    */

    /*
    //1-10 tester linear
    //y = 0.107x + 363
    public const float PALM_X = 0.107f;
    public const float PALM_Y = 363f;
    */

    //19-20 tester loge
    //y = 430.15ln(x) - 2832.4
    // public const float PALM_X = 430.15f;
    // public const float PALM_Y = -2832.4f;
    //study4
    // public const float PALM_X = 409.72f;
    // public const float PALM_Y = -2738.6f;

    //study4
    public const float PALM_X = 384.83f;
    public const float PALM_Y = -2667.4f;

    //study4
    public const float THUMB_X = 304.93f;
    public const float THUMB_Y = -1434.6f;

    public static bool CDRatio = false;


    private void Awake()
    {
        /*
        if (gc !=null)
        {
            Destroy(gameObject);
            return;
        }
        gc = this;
        DontDestroyOnLoad(gameObject);
        */
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PinchTypePressed()
    {
        mode = 'P';
    }

    public void PalmTypePressed()
    {
        mode = 'M';
    }
}