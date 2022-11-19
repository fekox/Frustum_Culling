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

        //Seteo el todos los objects test dentro del array.
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
}
