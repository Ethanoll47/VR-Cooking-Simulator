using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;


public class ForceManager : MonoBehaviour
{
    public string ID;
    public int weight; //force equivalent in gram
    private ConnectBluetoothFSR bm;
    private int fsr;
    private float targetForce;
    private float xOffset = 0;
    private float yOffset = 0;
    private float zOffset = 0;
    private bool isGrabbed;
    private HandVisual hv;
    private Grabbable gb;
    private Vector3 newObjectPosition;
    private Vector3 newHandPosition;    
    public OVRSkeleton skeleton;
  

    /*
    private const float UPPERLIMIT = 1.17f;  //not lift over this limit
    private const float LOWERLIMIT = 0.79f;  //not drop below this limit
    */
    private float beforeYOffset = 0;  //to control when lifting up, keep track of the offset to not allow it to reduce if fsr > target force

    private int moveStatus = 0; //1 = accelerate, 2 = decelerate, 0 = stationary
    private float offsetFromOriHand = 0;


    // Start is called before the first frame update
    void Start()
    {        
        //gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        bm = GameObject.Find("BTManager").GetComponent<ConnectBluetoothFSR>();
        //hv = GameObject.Find("/OVRCameraRig/OVRInteraction/OVRHands/RightHandSynthetic/RightHandVisual").GetComponent<HandVisual>();  
        //OR
        hv = GameObject.FindWithTag("HV").GetComponent<HandVisual>();
        gb = GetComponent<Grabbable>();
        //gb.includeInExperiment = true;

        skeleton = GameObject.Find("RightOVRHand").GetComponent<OVRSkeleton>();

    }

    // Update is called once per frame
    void Update()
    {
        
              
        //if the object drop on the floor, move it back to the table
        /*
        if (gameObject.transform.position.y < (LOWERLIMIT - 0.02))
        {
            //Debug.Log("woan ning: ID is:" + ID);
            if (ID.Equals("A"))
            {
                
                gameObject.transform.position = new Vector3(GameConfiguration.POSA_X, GameConfiguration.POSA_Y, GameConfiguration.POSA_Z);
            }
            else if (ID.Equals("B"))
            {
                gameObject.transform.position = new Vector3(GameConfiguration.POSB_X, GameConfiguration.POSB_Y, GameConfiguration.POSB_Z);
            }
        }
        */

        //derived targetforce from weight
        if (GameConfiguration.mode == 'P')
        {
            targetForce = GameConfiguration.THUMB_X * (float) System.Math.Log(weight) + GameConfiguration.THUMB_Y;  //thumb 
            fsr = bm.fsr1;
        }
        else if (GameConfiguration.mode == 'M')
        {         
            targetForce = GameConfiguration.PALM_X * (float) System.Math.Log(weight) + GameConfiguration.PALM_Y;  //palm 
            fsr = bm.fsr2;
        }

        if (targetForce < 0)  //to avoid too light object with negative targetforce
        {
            targetForce = 50;
        }

        isGrabbed = gb.isGrabbed;  //to check if the object has been grabbed, this value is obtained from Grabbable.cs 
        //Debug.Log("woan ning: is Hand Grabbed:" + isGrabbed);

        if (isGrabbed)
        {
            //to obtain the index finger position, so can display the object at the finger tip
            Vector3 indexposition = Vector3.zero;
            foreach (OVRBone bone in skeleton.Bones)
            {
                if (bone.Id == OVRSkeleton.BoneId.Hand_IndexTip)
                {
                    indexposition = bone.Transform.position;
                    //Debug.Log("woan ning: index position:" + indexposition);
                }
            }

            //obtain the real hand position when first grab
            if (newHandPosition.Equals(Vector3.zero))
            {
                //Debug.Log("new hand position is null");
                newHandPosition = hv.oriHandPosition;
                newObjectPosition = indexposition;  //set object position at the index finger position
            }
            else
            {
                //Debug.Log("new hand position NOT NULL");
            }

       
        
            yOffset = ((fsr - targetForce) / targetForce) / 100.0f;            

            //Debug.Log("woan ning: yOffset = " + yOffset);

            if (fsr < targetForce)            {
                
                if (!gb.move)  //if the object is at stationary stage
                {
                    //do nothing, not enough force to move the object
                    Debug.Log("woan ning: can't move object of weight= " + weight + ", target force= " + targetForce);
                    moveStatus = 0;  //set to stationary status
                }
                else
                {
                    //Debug.Log("woan ning: decelerate");
                    moveStatus = 2;  //set to decelerate status
                    if (GameConfiguration.CDRatio)  // if CD ratio
                    {
                        //keep decelerating until a threshold is met then drop the object
                        newHandPosition = new Vector3(newHandPosition.x + xOffset, newHandPosition.y + yOffset, newHandPosition.z + zOffset);
                        newObjectPosition = new Vector3(newObjectPosition.x + xOffset, newObjectPosition.y + yOffset, newObjectPosition.z + zOffset);

                        if ((hv.oriHandPosition.y - newHandPosition.y) > 0.15)  //threshold
                        {
                            //drop the object
                            gb.EndTransform();
                        }
                    }
                    else   //if no CD ratio
                    {
                        gb.EndTransform();
                    }
                    
                }
            }
            else //if fsr1 >=targetforce
            {               
                if ((gb.move) && (moveStatus != 2))  //if object is moving and it is not just turn accelerate from deceleration
                {
                    yOffset *= 8;  //accelerate by offset with visible quantum                                   
                }
                else
                {
                    //start to accelerate the object from stationary or deceleration, reset all the yOffset
                    yOffset = 0;
                    beforeYOffset = 0;
                    if (moveStatus == 2) //if it is previously from deceleration
                    {
                        offsetFromOriHand = newHandPosition.y - hv.oriHandPosition.y;  //record the hand offset from ori hand, so that it can be used to place the hand at the current position
                    }
                }
                moveStatus = 1; //set to accelerate
                gb.move = true;  //set object as moving  

                if (yOffset >= beforeYOffset) //when accelerating and the current yOffset is more than the highest yOffset
                {
                    beforeYOffset = yOffset;  //update the beforeYOffset to hold the highest value of yOffset
                }
                else
                {
                    yOffset = beforeYOffset;  //if current yOffset is not higher, use the highest yOffset, to avoid object from dropping so no wrong illusion interpretation (during accelerating)
                }

                if (GameConfiguration.CDRatio)  //if CD Ratio
                {
                    //accelerate following the ori hand position with add on offset to show accelerate effect
                    newHandPosition = new Vector3(hv.oriHandPosition.x + xOffset, hv.oriHandPosition.y + yOffset + offsetFromOriHand, hv.oriHandPosition.z + zOffset);
                    if (GameConfiguration.mode == 'M')
                    {
                        //apply offset so the object is at the middle of the palm
                        newObjectPosition = new Vector3(indexposition.x + xOffset, indexposition.y + yOffset + offsetFromOriHand - 0.02f, indexposition.z + zOffset);
                    }
                    else
                    {
                        newObjectPosition = new Vector3(indexposition.x + xOffset, indexposition.y + yOffset + offsetFromOriHand, indexposition.z + zOffset);
                    }
                }
                else  //if no CD Ratio
                {
                    newHandPosition = hv.oriHandPosition;
                    newObjectPosition = newHandPosition;
                    if (GameConfiguration.mode == 'M')
                        newObjectPosition = new Vector3(indexposition.x, indexposition.y - 0.02f, indexposition.z);  //move object to middle of the palm                    
                    else {
                        newObjectPosition = indexposition;
                    }
                }
            }
            
            //comment it out first
            /*

            if (newObjectPosition.y < LOWERLIMIT)  // to avoid it go below the table surface
            {
                newObjectPosition.y = LOWERLIMIT;
                
            }

            if (newObjectPosition.y > UPPERLIMIT) // if it goes beyond the bar, release it
            {
                newObjectPosition.y = UPPERLIMIT;
            }

            if (newHandPosition.y < LOWERLIMIT)  // to avoid it go below the table surface
            {
                newHandPosition.y = LOWERLIMIT;
                
            }

            if (newHandPosition.y > UPPERLIMIT) // if it goes beyond the bar, release it
            {
                newHandPosition.y = UPPERLIMIT;
            }
            */

            //set the object and hand position to be displayed by Grabbable.cs and HandVisual.cs
            gb.newPosition = newObjectPosition;
            //newHandPosition = new Vector3(newObjectPosition.x + 0.1f, newObjectPosition.y, newObjectPosition.z - 0.08f);  //nouse old code to offset based on hand wrist position
            hv.newHandPosition = newHandPosition;
            //Debug.Log("woan ning: y position: hand: " + newHandPosition.y);
            //Debug.Log("woan ning: y position: object: " + newObjectPosition.y);

        }
        else  //when release cube
        {
            newHandPosition = Vector3.zero;
            offsetFromOriHand = 0;
            moveStatus = 0;

        }
    }

}
