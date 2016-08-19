using UnityEngine;
using System.Collections;
using Leap.Unity;

public class Collide : MonoBehaviour {
    private HandModel GetHand(Collider col) {
        if (col.transform.parent && col.transform.parent.parent && col.transform.parent.parent.GetComponent<HandModel>()) {
            return col.transform.parent.parent.GetComponent<HandModel>();
        }
        else {
            return null;
        }
    }

    public void OnCollisionEnter(Collider col) {
        HandModel model = GetHand(col);
        if (model != null) {
            //return true;
        }
        //return false;
    }
}
