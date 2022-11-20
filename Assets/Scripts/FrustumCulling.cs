using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FrustumCulling : MonoBehaviour
{
    //Inicializa las constantes
    private const int maxPlanes = 6; //Cantidad de planos del frstrum.
    private const int maxObjecTest = 5; //Object Test.
    private const int AABBPoints = 8; //Axis Aligned Bounding Boxes points.

    //Inicializo un array de planos con la cantidad de planos del frustrum.
    Plane[] plane = new Plane[maxPlanes];

    //Inicializo una camara.
    Camera camera;

    //Inicializo un array de game objects.
    [SerializeField] GameObject[] objectsTest = new GameObject[maxObjecTest];

    //Plano cercano de la camara.
    [SerializeField] Vector3 nearTopLeft;
    [SerializeField] Vector3 nearTopRight;
    [SerializeField] Vector3 nearDownLeft;
    [SerializeField] Vector3 nearDownRight;

    //Plano lejano de la camara.
    [SerializeField] Vector3 farTopLeft;
    [SerializeField] Vector3 farTopRight;
    [SerializeField] Vector3 farDownLeft;
    [SerializeField] Vector3 farDownRight;

    //Creo un struc object test.
    public struct Obj
    {
        public GameObject gameObject;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public Vector3[] aabb;
        public Vector3 scale;
        public Vector3 extents;
    }

    //Inicializo un array del struct object. 
    [SerializeField] Obj[] objs = new Obj[maxObjecTest];

    private void Awake() //Se llama automaticamente al iniciar el scrip.
    {
        //Hago que la camara sea igual a la main camara.
        camera = Camera.main;
    }

    void Start() //Se llama automaticamente despues del awake.
    {
        //Creo los planos del frutrum.
        for (int i = 0; i < maxPlanes; i++)
        {
            plane[i] = new Plane();
        }

        //Seteo el todos los objects test. 
        for (int i = 0; i < maxObjecTest; i++)
        {
            objs[i].gameObject = objectsTest[i];
            objs[i].meshFilter = objectsTest[i].GetComponent<MeshFilter>();
            objs[i].meshRenderer = objectsTest[i].GetComponent<MeshRenderer>();
            objs[i].aabb = new Vector3[AABBPoints];
            objs[i].extents = objs[i].meshRenderer.bounds.extents;
            objs[i].scale = objs[i].meshRenderer.bounds.size;
        }
    }

    private void FixedUpdate() //Se llama un vez por frame.
    {
        UpdateFrustrumPlanes();//Llamo a la funcion que updatea los planos del frustrum.
    }

    void UpdateFrustrumPlanes()
    {
        //*Plano cercano*
        Vector3 nearPlanePos = camera.transform.position; //Guardo la posicion de la camara
        nearPlanePos += camera.transform.forward * camera.nearClipPlane; //Actualizo el plano cercano de la camara.
        plane[0].SetNormalAndPosition(camera.transform.forward, nearPlanePos); //Seteo el plano cercano del frustrum.

        //*Plano cercano*
        Vector3 farPlanePos = camera.transform.position; //Guardo la posicion de la camara
        farPlanePos += camera.transform.forward * camera.farClipPlane; //Actualizo el plano lejano de la camara.
        plane[1].SetNormalAndPosition(camera.transform.forward * -1, farPlanePos); //Seteo el plano lejano del frustrum.

        SetFarPoints(farPlanePos);//Lamo a la funcion para setear los puntos del plano lejano del Frustrum.
        SetNearPoints(nearPlanePos);//Lamo a la funcion para setear los puntos del plano cercano del Frustrum.

        //*Plano izquierdo*
        plane[2].Set3Points(camera.transform.position, farDownLeft, farTopLeft); //Seteo el plano Izquierdo del frustrum.

        //*Plano derecho*
        plane[3].Set3Points(camera.transform.position, farTopRight, farDownRight); //Seteo el Derecho lejano del frustrum.

        //*Plano de arriba*
        plane[4].Set3Points(camera.transform.position, farTopLeft, farTopRight); //Seteo el plano de arriba del frustrum.

        //*Plano de abajo*
        plane[5].Set3Points(camera.transform.position, farDownRight, farDownLeft); //Seteo el plano de abajo del frustrum.

        for (int i = 2; i < maxPlanes; i++)
        {
            plane[i].Flip();//Hace que el plano mire en la direccion opuesta.
        }

        for (int i = 0; i < maxObjecTest; i++)
        {
            SetAABB(ref objs[i]);//Seteo el AABB de todos los objetos.
        }

        for (int i = 0; i < maxObjecTest; i++)
        {
            ObjectCollision(objs[i]); //Chequo de todos los objetos, para saber si estan adentro o afuera del frustrum.
        }
    }

    //Funcion para setear los puntos del plano lejano
    public void SetFarPoints(Vector3 farPlanePos)
    {
        float halfCameraFarPlaneHeight = Mathf.Tan((camera.fieldOfView / 2) * Mathf.Deg2Rad) * camera.farClipPlane; //Guardo el alto del plano lejano de la camara. (La mitad)
        float halfCameraFarPlaneWidth = (camera.aspect * halfCameraFarPlaneHeight); //Guardo el ancho del plano lejano de la camara. (La mitad)

        Vector3 farPlaneDist = camera.transform.position + (camera.transform.forward * camera.farClipPlane); //Guardo la distancia del plano lejano de la camara.

        farTopLeft = farPlaneDist + (camera.transform.up * halfCameraFarPlaneHeight) - (camera.transform.right * halfCameraFarPlaneWidth); //Seteo el punto de arriba a la izquierda.

        farTopRight = farPlaneDist + (camera.transform.up * halfCameraFarPlaneHeight) + (camera.transform.right * halfCameraFarPlaneWidth); //Seteo el punto de arriba a la derecha.

        farDownLeft = farPlaneDist - (camera.transform.up * halfCameraFarPlaneHeight) - (camera.transform.right * halfCameraFarPlaneWidth); //Seteo el punto de abajo a la izquierda.

        farDownRight = farPlaneDist - (camera.transform.up * halfCameraFarPlaneHeight) + (camera.transform.right * halfCameraFarPlaneWidth); //Seteo el punto de abajo a la derecha.
    }

    //Funcion para setear los puntos del plano cercano
    public void SetNearPoints(Vector3 nearPlanePos)
    {
        float halfCameraNearPlaneHeight = Mathf.Tan((camera.fieldOfView / 2) * Mathf.Deg2Rad) * camera.nearClipPlane; //Guardo el alto del plano cercano de la camara. (La mitad)
        float HalfCameraNearPlaneWidth = (camera.aspect * halfCameraNearPlaneHeight); //Guardo el ancho del plano cercano de la camara. (La mitad)

        Vector3 nearPlaneDist = camera.transform.position + (camera.transform.forward * camera.nearClipPlane); //Guardo la distancia del plano cercano de la camara.

        nearTopLeft = nearPlaneDist + (camera.transform.up * halfCameraNearPlaneHeight) - (camera.transform.right * HalfCameraNearPlaneWidth); //Seteo el punto de arriba a la izquierda.

        nearTopRight = nearPlaneDist + (camera.transform.up * halfCameraNearPlaneHeight) + (camera.transform.right * HalfCameraNearPlaneWidth); //Seteo el punto de arriba a la derecha.

        nearDownLeft = nearPlaneDist - (camera.transform.up * halfCameraNearPlaneHeight) - (camera.transform.right * HalfCameraNearPlaneWidth); //Seteo el punto de abajo a la izquierda.

        nearDownRight = nearPlaneDist - (camera.transform.up * halfCameraNearPlaneHeight) + (camera.transform.right * HalfCameraNearPlaneWidth); //Seteo el punto de abajo a la derecha.
    }

    //Funcion para setear los puntos del Axis Aligned Bounding Boxes.
    public void SetAABB(ref Obj actualObj) 
    {
        if(actualObj.scale != actualObj.gameObject.transform.localScale) //Si la escal del objeto es diferente a su escala local, el objeto se vuelve a setear con su nueva escala.
        {
            Quaternion rotation = actualObj.gameObject.transform.rotation;
            actualObj.gameObject.transform.rotation = Quaternion.identity;
            actualObj.extents = actualObj.meshRenderer.bounds.extents;
            actualObj.scale = actualObj.gameObject.transform.localScale;
            actualObj.gameObject.transform.rotation = rotation;
        }

        Vector3 size = actualObj.extents; //Guardo el extent del objeto.
        Vector3 center = actualObj.meshRenderer.bounds.center; //Guardo el centro del objeto.

        //*Seteo todos los puntos del AABB*
        actualObj.aabb[0] = new Vector3(center.x - size.x, center.y + size.y, center.z - size.z); //Esquina superior izquierda del frente.
        actualObj.aabb[1] = new Vector3(center.x + size.x, center.y + size.y, center.z - size.z); //Esquina superior derecha del frente.
        actualObj.aabb[2] = new Vector3(center.x - size.x, center.y - size.y, center.z - size.z); //Esquina inferior izquierda del frente.
        actualObj.aabb[3] = new Vector3(center.x + size.x, center.y - size.y, center.z - size.z); //Esquina inferior derecha del frente.
        actualObj.aabb[4] = new Vector3(center.x - size.x, center.y + size.y, center.z + size.z); //Esquina superior izquierda de atras.
        actualObj.aabb[5] = new Vector3(center.x + size.x, center.y + size.y, center.z + size.z); //Esquina superior derecha de atras.
        actualObj.aabb[6] = new Vector3(center.x - size.x, center.y - size.y, center.z + size.z); //Esquina inferior izquierda de atras.
        actualObj.aabb[7] = new Vector3(center.x + size.x, center.y - size.y, center.z + size.z); //Esquina inferior derecha de atras.

        //Transformo las posiciiones en puntos dentro del espacio
        actualObj.aabb[0] = transform.TransformPoint(actualObj.aabb[0]);
        actualObj.aabb[1] = transform.TransformPoint(actualObj.aabb[1]);
        actualObj.aabb[2] = transform.TransformPoint(actualObj.aabb[2]);
        actualObj.aabb[3] = transform.TransformPoint(actualObj.aabb[3]);
        actualObj.aabb[4] = transform.TransformPoint(actualObj.aabb[4]);
        actualObj.aabb[5] = transform.TransformPoint(actualObj.aabb[5]);
        actualObj.aabb[6] = transform.TransformPoint(actualObj.aabb[6]);
        actualObj.aabb[7] = transform.TransformPoint(actualObj.aabb[7]);

        //Roto el punto en la misma direccion que el objeto (punto a rotar, pivot en el que rota, angulo en cada eje).
        actualObj.aabb[0] = RotatePointAroundPivot(actualObj.aabb[0], actualObj.gameObject.transform.position, actualObj.gameObject.transform.rotation.eulerAngles);
        actualObj.aabb[1] = RotatePointAroundPivot(actualObj.aabb[1], actualObj.gameObject.transform.position, actualObj.gameObject.transform.rotation.eulerAngles);
        actualObj.aabb[2] = RotatePointAroundPivot(actualObj.aabb[2], actualObj.gameObject.transform.position, actualObj.gameObject.transform.rotation.eulerAngles);
        actualObj.aabb[3] = RotatePointAroundPivot(actualObj.aabb[3], actualObj.gameObject.transform.position, actualObj.gameObject.transform.rotation.eulerAngles);
        actualObj.aabb[4] = RotatePointAroundPivot(actualObj.aabb[4], actualObj.gameObject.transform.position, actualObj.gameObject.transform.rotation.eulerAngles);
        actualObj.aabb[5] = RotatePointAroundPivot(actualObj.aabb[5], actualObj.gameObject.transform.position, actualObj.gameObject.transform.rotation.eulerAngles);
        actualObj.aabb[6] = RotatePointAroundPivot(actualObj.aabb[6], actualObj.gameObject.transform.position, actualObj.gameObject.transform.rotation.eulerAngles);
        actualObj.aabb[7] = RotatePointAroundPivot(actualObj.aabb[7], actualObj.gameObject.transform.position, actualObj.gameObject.transform.rotation.eulerAngles);


    }

    //Funcion para rotar un punto alrededor de un pivote.
    //*https://gamedev.stackexchange.com/questions/133038/unity-rotate-a-point-around-an-arbitrary-axis-and-anchor*
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public void ObjectCollision(Obj actualObj) 
    {
        bool isInside = false;

        for (int i = 0; i < AABBPoints; i++) //Recorro todos los puntos del AABB.
        {
            int counter = maxPlanes; //Creo un contador con la maxima cantidad de planos.

            for (int j = 0; j < maxPlanes; j++) //Recorro todos los planos. 
            {
                if (plane[j].GetSide(actualObj.aabb[i])) //Si el punto esta en el lado positivo del plano al contador se le resta 1, se chequea con todos los puntos.
                {
                    counter--;
                }
            }

            if (counter == 0) //Si todos los puntos del AABB se encuentran dentro del lado positivo de los 6 planos, la figura esta adentro del frustrum. 
            {
                isInside = true; //La variable se hace true.
                break;
            }
        }

        if (isInside) //Si el objeto esta adentro del frustrum, 
        {
            for (int i = 0; i < actualObj.meshFilter.mesh.vertices.Length; i++) //Rocorro todos los vertices del objeto actual.
            {
                int counter = maxPlanes; //Creo un contador con la maxima cantidad de planos.

                for (int j = 0; j < maxPlanes; j++) //Recorro todos los planos. 
                {
                    if (plane[j].GetSide(actualObj.gameObject.transform.TransformPoint(actualObj.meshFilter.mesh.vertices[i]))) //Si el vertice esta en el lado positivo del plano al contador se le resta 1, se cheque con todos los vertices.
                    {
                        counter--;
                    }
                }

                if (counter == 0) //Si todos los vertices del objeto actual se encuentran delntro del lado positivo de los 6 planos, la figura se hace visible.
                {
                    actualObj.gameObject.SetActive(true);
                    break;
                }
            }
        }

        else
        {
            if (actualObj.gameObject.activeSelf) //Si el objeto esta activado y se encuentra duera del Frustrum, se desactiva. 
            {
                actualObj.gameObject.SetActive(false);
            }
        }
    
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Gizmos.color = Color.green;

        //Dibujo el plano lejano.
        DrawPlane(farTopRight, farDownRight, farDownLeft, farTopLeft);

        //Dibujo el plano cercano.
        DrawPlane(nearTopRight, nearDownRight, nearDownLeft, nearTopLeft);

        //Dibujo el plano izquierdo.
        DrawPlane(nearTopLeft, farTopLeft, farDownLeft, nearDownLeft);

        //Dibujo el plano derecho.
        DrawPlane(nearTopRight, farTopRight, farDownRight, nearDownRight);

        //Dibujo el plano superior.
        DrawPlane(nearTopLeft, farTopLeft, farTopRight, nearTopRight);

        //Dibujo el plano inferior.
        DrawPlane(nearDownLeft, farDownLeft, farDownRight, nearDownRight);

        //Dibujo el AABB en cada objeto.
        for (int i = 0; i < maxObjecTest; i++)
        {
            DrawAABB(ref objs[i]);
        }
    }

    //Funcion para dibujar los planos del frustrum.
    public void DrawPlane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        Gizmos.color = Color.green;
    }

    //Funcion para dibujar el AABB.
    public void DrawAABB(ref Obj actualObj) 
    {
        Gizmos.color = Color.magenta;

        for (int i = 0; i < AABBPoints; i++)
        {
            Gizmos.DrawSphere(actualObj.aabb[i], 0.05f);
        }

        //Dibujado de la caja que se forma uniendo los puntos del AABB.
        Gizmos.DrawLine(actualObj.aabb[0], actualObj.aabb[1]);
        Gizmos.DrawLine(actualObj.aabb[0], actualObj.aabb[4]);
        Gizmos.DrawLine(actualObj.aabb[1], actualObj.aabb[3]);
        Gizmos.DrawLine(actualObj.aabb[2], actualObj.aabb[0]);
        Gizmos.DrawLine(actualObj.aabb[3], actualObj.aabb[2]);
        Gizmos.DrawLine(actualObj.aabb[4], actualObj.aabb[5]);
        Gizmos.DrawLine(actualObj.aabb[5], actualObj.aabb[7]);
        Gizmos.DrawLine(actualObj.aabb[5], actualObj.aabb[1]);
        Gizmos.DrawLine(actualObj.aabb[6], actualObj.aabb[2]);
        Gizmos.DrawLine(actualObj.aabb[6], actualObj.aabb[4]);
        Gizmos.DrawLine(actualObj.aabb[7], actualObj.aabb[3]);
        Gizmos.DrawLine(actualObj.aabb[7], actualObj.aabb[6]);

        Gizmos.color = Color.green;
    }
}
