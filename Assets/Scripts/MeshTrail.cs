using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2.0f;
    public MovementInput moveScript;
    public float speedBoost = 6;
    public Animator animator;
    public float animSpeedBoost = 1.5f;

    [Header("Mesh Releted")]
    public float meshRefreshRate = 1.0f;
    public float meshDestoryDelay = 3.0f;
    public Transform positionToSpawn;

    [Header("Shader Releted")]
    public Material mat;
    public string shaderVerRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRetreshRate = 0.05f;

    private SkinnedMeshRenderer[] skinnedRenderer;
    private bool isTraiActive;

    private float normalSpeed;
    private float normalAnimSpeed;

    IEnumerator AnimeteMaterialFloat(Material m, float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shaderVerRef);

        while(valueToAnimate >  valueGoal)
        {
            valueToAnimate -= rate;
            m.SetFloat(shaderVerRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator ActivateTrail(float timeActivated)
    {
        normalSpeed = moveScript.movementSpeed;
        moveScript.movementSpeed = speedBoost;

        normalAnimSpeed = animator.GetFloat("animSpeed");
        animator.SetFloat("animSpeed", animSpeedBoost);

        while(timeActivated > 0)
        {
            timeActivated -= meshRefreshRate;

            if(skinnedRenderer == null)
                skinnedRenderer = positionToSpawn. GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i = 0; i < skinnedRenderer.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position,positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh m = new Mesh();
                skinnedRenderer[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = mat;

                StartCoroutine(AnimeteMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRetreshRate));

                Destroy(gObj, meshDestoryDelay);
            }
            yield return new WaitForSeconds(meshRefreshRate);
        }
        moveScript.movementSpeed = normalSpeed;
        animator.SetFloat("animSpeed", normalAnimSpeed);
        isTraiActive = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isTraiActive)
        {
            isTraiActive=true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }
}
