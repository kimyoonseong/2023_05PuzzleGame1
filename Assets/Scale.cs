using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = new Vector3(0.5f, 0.5f, 0.4f);
        this.transform.rotation = Quaternion.Euler(270, 0, 0);
    }
}
