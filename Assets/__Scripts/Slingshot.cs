using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static public Slingshot S;

    [Header("Set in Inspector")]
    public GameObject prefabProjectile;
    public GameObject prefabProjectileLine;
    public float velocityMult = 8f;

    [Header("Set Dynamically")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    public int impactForce; // сила удару 
    private Rigidbody projectileRigidbody;
    public GameObject projectileLine;
    public List<GameObject> projectileLines;
    private LineRenderer elastic;


    static public Vector3 LAUNCH_POS
    {
        get 
        {
            if(S == null) return Vector3.zero;
            return S.launchPos;
        }
    }


    void Awake() 
    {
        S = this;
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;  
        
        projectileLine = Instantiate(prefabProjectileLine);

        projectileLines = new List<GameObject>();
        projectileLines.Add(projectileLine);

        elastic = new LineRenderer();
        elastic = GetComponent<LineRenderer>();
        elastic.enabled = false;
    }

    void OnMouseEnter()
    {
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
       launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(prefabProjectile) as GameObject;

        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;  
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;

        BuildProlectileLine();
        elastic.SetPosition(0, launchPos);
        elastic.enabled = true;

    }
    private void BuildProlectileLine()
    {
        projectileLine = Instantiate(prefabProjectileLine) as GameObject;
        projectileLines.Add(projectileLine);
        if(projectileLines.Count > 3 ) 
        {
            DestroyProjectileLines();
        }
    }
    public void DestroyProjectileLines()
    {
        List<GameObject> destrProjectileLines= new List<GameObject>();
        destrProjectileLines.Add(projectileLines[0]);
        projectileLines.RemoveAt(0);
        foreach (GameObject line in destrProjectileLines)
        {
            Destroy(line);
        }
    }

    void Update()
    {
        if(!aimingMode) return;
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);
        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        elastic.SetPosition(1, projPos);
        if ( Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            elastic.enabled = false; 
            projectileRigidbody.velocity = - mouseDelta * velocityMult;

            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
        }
    }
}
