using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{

    public GameObject player;
    public float cameraDistance = 6f;
    public float cameraSpeed = 0.015f;
    public Vector3 cameraPositionRelative = new Vector3(-1f, -1f, 1f);
    public int zoomLimit = -4;
    Vector3 cameraPositionRelativeFinal;

    private List<Vector3> filteredPositions = new List<Vector3>();

    private Dictionary<GameObject, Material[]> oldMaterials = new Dictionary<GameObject, Material[]>();

    private List<GameObject> hits = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        cameraPositionRelativeFinal = cameraPositionRelative;
    }

    //void Update()
    //{
    //    Ray topLeft = Camera.main.ScreenPointToRay(new Vector3(0, 0, 0));
    //    Ray topRight = Camera.main.ScreenPointToRay(new Vector3(Screen.width, 0, 0));
    //    Ray botRight = Camera.main.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0));
    //    Ray botLeft = Camera.main.ScreenPointToRay(new Vector3(0, Screen.height, 0));


    //    var newHits = Physics.RaycastAll(player.transform.position, (this.transform.position - player.transform.position), Vector3.Distance(player.transform.position, this.transform.position)).Select(i => i.transform.gameObject).ToList();

    //    var size = player.GetComponent<CapsuleCollider>().height * 2;
    //    var newPos = player.transform.position +new Vector3(0, 0, 0);
    //    var hits2 = Physics.RaycastAll(newPos, (this.transform.position - newPos), Vector3.Distance(newPos, this.transform.position)).Select(i => i.transform.gameObject).ToList();
    //    newPos = player.transform.position + new Vector3(size, 0, 0);
    //    var hits3 = Physics.RaycastAll(newPos, (this.transform.position - newPos), Vector3.Distance(newPos, this.transform.position)).Select(i => i.transform.gameObject).ToList();
    //    newPos = player.transform.position + new Vector3(-size, 0, 0);
    //    var hits4 = Physics.RaycastAll(newPos, (this.transform.position - newPos), Vector3.Distance(newPos, this.transform.position)).Select(i => i.transform.gameObject).ToList();
    //    newPos = player.transform.position + new Vector3(0, 0, size);
    //    var hits5 = Physics.RaycastAll(newPos, (this.transform.position - newPos), Vector3.Distance(newPos, this.transform.position)).Select(i => i.transform.gameObject).ToList();

 
    //    var realHits = newHits.Where(hit =>
    //        {
    //            var corners = 0f;
    //            if (hits2.Contains(hit)) corners++;
    //            if (hits3.Contains(hit)) corners++;
    //            if (hits4.Contains(hit)) corners++;
    //            if (hits5.Contains(hit)) corners++;
    //            return (corners > 2);
    //        }
    //    ).ToList();

    //    realHits.ForEach(hit =>
    //    {
    //        Renderer r = hit.GetComponent<Renderer>();
    //        if (r)
    //        {
    //            if (!oldMaterials.ContainsKey(hit))
    //            {
    //                oldMaterials[hit] = r.materials;
    //            }

    //            Material[] mats2 =new Material[r.materials.Length];
    //            for (var i = 0; i < r.materials.Length; i++)
    //            {

    //                var newMaterial = GetComponentInChildren<MeshRenderer>().material;
    //                mats2[i] = newMaterial;
    //            }
    //            r.materials = mats2;
    //        }
    //    });
        
    //    foreach (var hit in hits)
    //    {
    //        if (!newHits.Contains(hit))
    //        {
    //            Renderer r = hit.GetComponent<Renderer>();
    //            if (r)
    //            {
    //                if (oldMaterials.ContainsKey(hit))
    //                {
    //                    r.materials = oldMaterials[hit];
    //                    oldMaterials.Remove(hit);
    //                }
    //            }
    //        }
    //    }
        
    //    hits = newHits;

    //}

    private void LateUpdate() {
        float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
        //float JoystickChange = Input.GetAxis("CameraXBOX");
        if (ScrollWheelChange != 0/* || JoystickChange != 0*/) {
            float R = ScrollWheelChange * zoomLimit;
            //if (JoystickChange != 0)
            //    R = JoystickChange;
            float PosX = Camera.main.transform.eulerAngles.x + 90;
            float PosY = -1 * (Camera.main.transform.eulerAngles.y - 90);
            PosX = PosX / 180 * Mathf.PI;
            PosY = PosY / 180 * Mathf.PI;
            float X = R * Mathf.Sin(PosX) * Mathf.Cos(PosY);
            float Z = R * Mathf.Sin(PosX) * Mathf.Sin(PosY);
            float Y = R * Mathf.Cos(PosX);
            cameraPositionRelativeFinal = cameraPositionRelative + new Vector3(X, Y, Z);
        }
    }


    public void fastMove()
    {
        transform.forward = cameraPositionRelativeFinal;
        var distance = cameraPositionRelativeFinal.magnitude * cameraDistance;
        Vector3 desiredCameraLocation = player.transform.position - cameraPositionRelativeFinal * distance;
        Vector3 difference = (transform.position - desiredCameraLocation);
        transform.position -= difference;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.forward = cameraPositionRelativeFinal;
        var distance = cameraPositionRelativeFinal.magnitude * cameraDistance;
        Vector3 desiredCameraLocation = player.transform.position - cameraPositionRelativeFinal * distance;
        Vector3 difference = (transform.position - desiredCameraLocation);
        transform.position -= difference * cameraSpeed;
    }
}
