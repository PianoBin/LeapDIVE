//Find if hand in pinch position (Thumb tip close to one of other finger joints)
//If RigidBody is nearby, hold object relative to where hand is
//Modified Capsule Hand script to only produce blue colored hands for left, and red colored hands for right

using UnityEngine;
using Leap;
using System.Collections.Generic;
using Leap.Unity;

public class PinchAndHold : MonoBehaviour {

    Controller control;
    GameObject grabbed; //for left hand holds
    GameObject grabbed2; //for right hand holds

    bool isPinchingLeft = false;
    bool isPinchingRight = false;

    Color[] colorList;
    private static Color[] _leftColorList = { Color.blue}; 
    private static Color[] _rightColorList = { Color.red};
    Color colorChangeLeft;
    Color colorChangeRight;
    int colorCount;
    int colorCount2;
    bool isReady;

    // Use this for initialization
    void Start () {
        control = new Controller();
        grabbed = null;
        grabbed2 = null;
        Debug.Log("Running");
        colorList = getColors(); //remember colors of objects
        colorChangeLeft = _leftColorList[0];
        colorChangeRight = _rightColorList[0];
        colorCount = 0;
        colorCount2 = 0;
        isReady = false;
    }

    Color[] getColors() { //get colors of objects
        GameObject[] objectList = GameObject.FindGameObjectsWithTag("CanGrab");
        int amount = objectList.Length;
        Color[] colors = new Color[amount];
        int count = 0;
        foreach (GameObject item in objectList) {
            colors[count] = item.GetComponent<Renderer>().material.color;
            count++;
        }

        return colors;
    }

    void setColors(Color[] oldColors, Transform held) { //return back to normal colors
        GameObject[] objectList = GameObject.FindGameObjectsWithTag("CanGrab");
        int count = 0;
        foreach (GameObject item in objectList) {
            if (held.transform.name != item.transform.name) {
                item.GetComponent<Renderer>().material.color = oldColors[count];
            }
            count++;
        }
    }

    GameObject Hold(Vector3 pos, GameObject oldObject, bool leftHold, bool rightHold) {//find nearest object within Player's bubble 
        GameObject obj = null; //local
        bool objFound = false;
        //REQUIRES VECTOR 3, converted from earlier
        GameObject[] nearby = GameObject.FindGameObjectsWithTag("CanGrabActual"); //list of objects with tag
        float old_dist = 1.5f;

        for (int count = 0; count < nearby.Length; count++) {

            float new_dist = Vector3.Distance(nearby[count].transform.position, pos);
            Debug.Log(nearby[count].name + " " + new_dist);
            if (new_dist < old_dist && nearby[count].transform.name != oldObject.transform.name) {
                old_dist = new_dist;
                obj = nearby[count];
                objFound = true;
            }
        }

        if (objFound && leftHold) {
            obj.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue; //Color obj 
            Debug.Log(obj.name + " IS FOUND");
            return obj;
        }
        else if (objFound && rightHold) {
            obj.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red; //Color obj 
            Debug.Log(obj.name + " IS FOUND");
            return obj;
        }
        else {
            Debug.Log("Nothing FOUND");
            return null;
        }
    }


    void allButObj(GameObject obj) {
        string objName = obj.name;
        GameObject[] nearby = GameObject.FindGameObjectsWithTag("CanGrabActual");
        foreach (GameObject item in nearby) {
            if (!objName.Equals(item.name)) {
                item.GetComponent<Rigidbody>().isKinematic = false;
                item.GetComponent<Rigidbody>().useGravity = true;
                item.GetComponent<Rigidbody>().AddForce(0, -9.8f, 0, ForceMode.Acceleration);
            }
        }
    }

    void allButObjMulti(GameObject obj, GameObject obj2) {
        string objName = obj.name;
        string objName2 = obj2.name;
        GameObject[] nearby = GameObject.FindGameObjectsWithTag("CanGrabActual");
        foreach (GameObject item in nearby)
        {
            if (!objName.Equals(item.name) && !objName2.Equals(item.name)) {
                item.GetComponent<Rigidbody>().isKinematic = false;
                item.GetComponent<Rigidbody>().useGravity = true;
                item.GetComponent<Rigidbody>().AddForce(0, -9.8f, 0, ForceMode.Acceleration);
            }
        }
    }


    void setGrabbed(GameObject obj1) {
        grabbed = obj1;
        Debug.Log("new left");
    }
    void setGrabbed2(GameObject obj2) {
        grabbed2 = obj2;
        Debug.Log("new right");
    }
    GameObject getGrabbed() {
        return grabbed;
    }
    GameObject getGrabbed2() {
        return grabbed2;
    }

    void changeObjColor (Frame theFrame) {
        if (theFrame.Hands[0].IsLeft)
        {
            if (colorCount != 2)
            {
                colorCount++;
                colorChangeLeft = _leftColorList[colorCount];
                Debug.Log("COLORCHANGELEFT" + colorChangeLeft);
            }
            else
            {
                colorCount = 0;
                colorChangeLeft = _leftColorList[colorCount];
                Debug.Log("COLORCHANGELEFT" + colorChangeLeft);
            }
        }
        else
        {
            if (colorCount2 != 2)
            {
                colorCount2++;
                colorChangeRight = _rightColorList[colorCount2];
                Debug.Log("COLORCHANGERIGHT" + colorChangeRight);
            }
            else
            {
                colorCount2 = 0;
                colorChangeRight = _rightColorList[colorCount2];
                Debug.Log("COLORCHANGERIGHT" + colorChangeRight);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        Rigidbody player = GameObject.Find("LeapPlayer").GetComponent<Rigidbody>();
        Vector3 playerPos = player.GetComponent<Rigidbody>().position; //Player/Controller position
        Vector3 temp = playerPos;
        temp.y = 0;
        playerPos = temp;

        Frame currentFrame = control.Frame();

        if (currentFrame.Hands.Count > 1) { //if both hands present in frame
            List<Hand> hands = currentFrame.Hands;        

            //assign left and right hands
            Hand leftHand = hands[0];
            Hand rightHand = hands[1];

            Vector3 palmLeft = hands[0].PalmPosition.ToVector3(); //position of left hand palm
            Vector3 palmRight = hands[1].PalmPosition.ToVector3(); //position of right hand palm

            Vector3 fingLeft = hands[0].Fingers[1].TipPosition.ToVector3();
            Vector3 fingRight = hands[1].Fingers[1].TipPosition.ToVector3();

            Vector3 left = new Vector3((palmLeft.x / -1000.0f * 2.5f), (palmLeft.z / -1000.0f) + 1.4f, (palmLeft.y / 1000.0f * 2.5f)) + playerPos; //to unity coords relative to player
            Vector3 right = new Vector3((palmRight.x / -1000.0f * 2.5f), (palmRight.z / -1000.0f) + 1.4f, (palmRight.y / 1000.0f * 2.5f)) + playerPos; //to unity coords relative to player

            Vector3 leftFing = new Vector3((fingLeft.x / -1000.0f * 2.5f), (fingLeft.z / -1000.0f) + 1.4f, (fingLeft.y / 1000.0f * 2.5f)) + playerPos; //to unity coords relative to player
            Vector3 rightFing = new Vector3((fingRight.x / -1000.0f * 2.5f), (fingRight.z / -1000.0f) + 1.4f, (fingRight.y / 1000.0f * 2.5f)) + playerPos; //to unity coords relative to player

            Vector3 leftFingBack = new Vector3((fingLeft.x / 1000.0f * 2.5f), (fingLeft.z / -1000.0f) + 1.4f, (fingLeft.y / 1000.0f * 2.5f)) + playerPos; //to unity coords relative to player
            Vector3 rightFingBack = new Vector3((fingRight.x / 1000.0f * 2.5f), (fingRight.z / -1000.0f) + 1.4f, (fingRight.y / 1000.0f * 2.5f)) + playerPos; //to unity coords relative to player

            Debug.Log("Player's Position: " + playerPos);
            Debug.Log("Left Hand Position: " + left);
            Debug.Log("Right Hand Position: " + right);

            float pinchLeft = leftHand.PinchStrength;
            float pinchRight = rightHand.PinchStrength;
            
            if (pinchLeft >= 0.5 && pinchRight >= 0.5) { 
                isPinchingLeft = true;
                isPinchingRight = true;
                GameObject item1 = Hold(left, GameObject.Find("nothing"), true, false);
                if (item1 != null) {
                    setGrabbed(item1);
                }
                GameObject item2 = Hold(right, item1, false, true);
                if (item2 != null) {
                    setGrabbed2(item2);
                }

                if (item1 != null && item2 != null) {
                    allButObjMulti(getGrabbed(), getGrabbed2());
                }
                //Debug.Log("BOTH HAVE");
            }
            else if (pinchLeft >= 0.5) { //only left hand pinched
                isPinchingLeft = true;
                isPinchingRight = false;
                GameObject item1 = Hold(left, GameObject.Find("nothing"), true, false);
                if (item1 != null)
                {
                    setGrabbed(item1);
                    allButObj(getGrabbed());
                    setColors(colorList, item1.transform.GetChild(0));
                }
                //Debug.Log("LEFT HAS");
            }
            else if (pinchRight >= 0.5) { //only right hand pinched
                isPinchingRight = true;
                isPinchingLeft = false;
                GameObject item2 = Hold(right, GameObject.Find("nothing"), false, true);
                if (item2 != null)
                {
                    setGrabbed2(item2);
                    allButObj(getGrabbed2());
                    setColors(colorList, item2.transform.GetChild(0));
                }
                //Debug.Log("RIGHT HAS");
            }
            else { //no hands in pinching position
                isPinchingLeft = false;
                isPinchingRight = false;
                setColors(colorList, GameObject.Find("nothing").transform);
                //Debug.Log("NONE HAVE");
            }
                

            if (!isPinchingLeft && getGrabbed() != null) {//not pinching anymore but holding object in left hand
                getGrabbed().GetComponent<Rigidbody>().isKinematic = false;
                getGrabbed().GetComponent<Rigidbody>().useGravity = true;
                getGrabbed().GetComponent<Rigidbody>().AddForce(0, -9.8f, 0, ForceMode.Acceleration);
                setGrabbed(null);
            }
            if (!isPinchingRight && getGrabbed2() != null) {//not pinching anymore but holding object in right hand             
                getGrabbed2().GetComponent<Rigidbody>().isKinematic = false;
                getGrabbed2().GetComponent<Rigidbody>().useGravity = true;
                getGrabbed2().GetComponent<Rigidbody>().AddForce(0, -9.8f, 0, ForceMode.Acceleration);
                setGrabbed2(null);
            }
            

            if (isPinchingLeft && grabbed != null) { //holding something in left hand, move object to palm position
                grabbed.GetComponent<Rigidbody>().isKinematic = true;
                grabbed.GetComponent<Rigidbody>().useGravity = false;

                getGrabbed().transform.position = leftFing;       
            }

            if (isPinchingRight && grabbed2 != null) {//holding something in right hand, move object to palm position
                grabbed2.GetComponent<Rigidbody>().isKinematic = true;
                grabbed2.GetComponent<Rigidbody>().useGravity = false;

                getGrabbed2().transform.position = rightFing;
            }

            Debug.Log("Left Pinch Strength" + pinchLeft);
            Debug.Log("Right Pinch Strength" + pinchRight);
            Debug.Log("END FRAME");

            isReady = true;
        }
        /*else if (currentFrame.Hands.Count == 1 && isReady == true) {
            changeObjColor(currentFrame);
            isReady = false;
        }
        else if (currentFrame.Hands.Count == 2 && isReady == true) {
            changeObjColor(currentFrame);
            changeObjColor(currentFrame);
            isReady = false;
        }
        else {

        }*/
    }
}