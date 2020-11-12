using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyAfter : MonoBehaviour
{

    //Unistava efekte da se ne bi prenagomilavali
    void Start()
    {

        Destroy(gameObject, 1.5f);

    }


}
