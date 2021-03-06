﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
[SelectionBase]
public class WeaponScript1 : MonoBehaviour
{
    public static WeaponScript1 instance;
    
    public Camera myCam;
    public GameObject bulletPrefab;
    
    [Header("Bools")]
    public bool active = true;
    public bool reloading;

    private Rigidbody rb;
    private Collider collider;
    private Renderer renderer;

    [Space]
    [Header("Weapon Settings")]
    public float reloadTime = .3f;
    public int bulletAmount = 6;

    [Space] [Header("UI Settings")] 
    public GameObject winInterface;
    public GameObject loseInterface;
    
    [Space] [Header("Health")] 
    public int maxHealth = 50;
    public int currentHealth;

    public HealthBar healthBar;

    [Space] [Header("Enemy Info")] 
    public int enemyNo = 1;


    void Start()
    {
        GlobalVar.currentHealth = maxHealth;
        GlobalVar.maxHealth = maxHealth;
        
        currentHealth = maxHealth;
        
        healthBar.SetMaxHealth(maxHealth);
        
        GlobalVar.EnemyNo = enemyNo;
        
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();
        
        winInterface.SetActive(false);
        loseInterface.SetActive(false);

        ChangeSettings();
    }

    void ChangeSettings()
    {
        if (transform.parent != null)
            return;

        rb.isKinematic = (SuperHotScript.instance.weapon == this) ? true : false;
        rb.interpolation = (SuperHotScript.instance.weapon == this) ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
        collider.isTrigger = (SuperHotScript.instance.weapon == this);
    }

    public void Shoot(Vector3 pos,Quaternion rot, bool isEnemy)
    {
        if (reloading)
            return;

        if (bulletAmount <= 0)
            return;

        // if(!SuperHotScript.instance.weapon == this)
        //     bulletAmount--;

        GameObject bullet = Instantiate(bulletPrefab, pos, rot);

        if (GetComponentInChildren<ParticleSystem>() != null)
            GetComponentInChildren<ParticleSystem>().Play();

        // if(SuperHotScript1.instance.weapon == this)
        StartCoroutine(Reload());

        myCam.transform.DOComplete();
        myCam.transform.DOShakePosition(.2f, .01f, 10, 90, false, true).SetUpdate(true);

        // if(SuperHotScript.instance.weapon == this)
        //     transform.DOLocalMoveZ(-.1f, .05f).OnComplete(()=>transform.DOLocalMoveZ(0,.2f));
    }

    public void Throw()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
        s.AppendCallback(() => transform.parent = null);
        s.AppendCallback(() => transform.position = myCam.transform.position + (myCam.transform.right * .1f));
        s.AppendCallback(() => ChangeSettings());
        s.AppendCallback(() => rb.AddForce(myCam.transform.forward * 10, ForceMode.Impulse));
        s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * 20, ForceMode.Impulse));
    }

    public void Release()
    {
        active = true;
        transform.parent = null;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        collider.isTrigger = false;

        // rb.AddForce((myCam.transform.position - transform.position) * 2, ForceMode.Impulse);
        // rb.AddForce(Vector3.up * 2, ForceMode.Impulse);

    } 

    IEnumerator Reload()
    {
        // if (SuperHotScript1.instance.weapon != this)
        //     yield break;
        // SuperHotScript1.instance.ReloadUI(reloadTime);
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }
    
    IEnumerator Halt()
    {
        yield return new WaitForSeconds(1.5f);
        winInterface.SetActive(true);
        GlobalVar.AltInterfaceOpen = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    IEnumerator HaltLose()
    {
        yield return new WaitForSeconds(0.5f);
        loseInterface.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GlobalVar.AltInterfaceOpen = true;
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    private void Update()
    {
        if (GlobalVar.IsWin)
        {
            EnterWinInterface();
        } else if (GlobalVar.IsLoss)
        {
            EnterLoseInterface();
        }
        
        // Debug.Log(GlobalVar.EnemyNo);
        // DetectChange(enemyNo);
        
        healthBar.SetHealth(GlobalVar.currentHealth);
    }

    // void DetectChange(int enemyNumber)
    // {
    //     if (GlobalVar.EnemyNo != enemyNumber)
    //     {
    //         enemyNumber = GlobalVar.EnemyNo;
    //         // Debug.Log($"Global Enemy No: {GlobalVar.EnemyNo}");
    //         // Debug.Log($"Enemy Number: {enemyNumber}");
    //     }
    // }

    public void EnterWinInterface()
    {
        StartCoroutine(Halt());
    }

    public void EnterLoseInterface()
    {
        StartCoroutine(HaltLose());
    }

}
