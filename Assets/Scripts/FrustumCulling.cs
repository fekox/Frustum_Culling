using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustumCulling : MonoBehaviour
{
    [SerializeField] private GameObject objecTest;

    [SerializeField] private GameObject plaDown;
    [SerializeField] private GameObject plaUp;
    [SerializeField] private GameObject plaRight;
    [SerializeField] private GameObject plaLeft;
    [SerializeField] private GameObject plaBack;
    [SerializeField] private GameObject plaFront;

    private const int maxPlanes = 6;
    private const int maxPoints = 8;

    Plane[] plane = new Plane[maxPlanes];
    Vector3[] point = new Vector3[maxPoints];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < maxPlanes; i++)
        {
            plane[i] = new Plane();
        }

        plane[0] = new Plane(Vector3.up, plaDown.transform.position);
        plane[1] = new Plane(Vector3.down, plaUp.transform.position);
        plane[2] = new Plane(Vector3.left, plaRight.transform.position);
        plane[3] = new Plane(Vector3.right, plaLeft.transform.position);
        plane[4] = new Plane(Vector3.back, plaBack.transform.position);
        plane[5] = new Plane(Vector3.forward, plaFront.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        plane[0].SetNormalAndPosition(Vector3.up, plaDown.transform.position);
        plane[1].SetNormalAndPosition(Vector3.down, plaUp.transform.position);
        plane[2].SetNormalAndPosition(Vector3.left, plaRight.transform.position);
        plane[3].SetNormalAndPosition(Vector3.right, plaLeft.transform.position);
        plane[4].SetNormalAndPosition(Vector3.back, plaBack.transform.position);
        plane[5].SetNormalAndPosition(Vector3.forward, plaFront.transform.position);

        bool isInside = false;

        Vector3 scale = objecTest.transform.localScale / 2;

        for (int i = 0; i < maxPoints; i++)
        {
            point[i] = objecTest.transform.position;
        }

        Vector3 foward;
        Vector3 up;
        Vector3 right;

        foward = objecTest.transform.forward;
        up = objecTest.transform.up;
        right = objecTest.transform.right;
        
        point[0] += scale.x * right + scale.y * up + scale.z * foward;
        point[1] += scale.x * right + scale.y * up + -scale.z * foward;
        point[2] += scale.x * right + -scale.y * up + scale.z * foward;
        point[3] += scale.x * right + -scale.y * up + -scale.z * foward;
        point[4] += -scale.x * right + scale.y * up + scale.z * foward;
        point[5] += -scale.x * right + scale.y * up + -scale.z * foward;
        point[6] += -scale.x * right + -scale.y * up + scale.z * foward;
        point[7] += -scale.x * right + -scale.y * up + -scale.z * foward;

        //Cada punto recorre todos los planos
        for (int i = 0; i < maxPoints; i++)
        {
            int counter = maxPlanes;

            for (int j = 0; j < maxPlanes; j++)
            {
                if (plane[j].GetSide(point[i]))
                {
                    counter--;
                }
            }

            if (counter == 0)
            {
                Debug.Log("Está adentro");
                isInside = true;
                break;
            }
        }

        if (isInside) 
        {
            if(!objecTest.activeSelf) 
            {
                objecTest.SetActive(true);
            }
        }
        else 
        {
            if(objecTest.activeSelf) 
            {
                Debug.Log("Está afuera");
                objecTest.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < maxPoints; i++)
        {
            Gizmos.DrawSphere(point[i], 0.2f);
        }
    }
}
