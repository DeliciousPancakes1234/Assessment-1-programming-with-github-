using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnlinePlayerAttacks : MonoBehaviour
{
    Animator anim;
    [Header("Ranged Attack Settings")]
    public GameObject projectile;
    public Transform launchPoint;

    public float lobSpeed, lobLift;

    [Header("Melee Attack Settings")]
    public GameObject meleeCollider;
    public float attackDelay;
    public float attackLifeTime;

    private PhotonView view;

    AudioSource shootingSound;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        shootingSound = GetComponent<AudioSource>();
        view = GetComponent<PhotonView>();
        meleeCollider.GetComponent<PlayerAttackCollision>().playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
    }

    // Update is called once per frame
    void Update()
    {
        if (!view.IsMine) return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.Play("Shoot");
            Shoot();
            shootingSound.Play();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            view.RPC("OnlineMeleeAttack", RpcTarget.All);
            anim.Play("MeleeAttack");
        }
        if (Input.GetKey(KeyCode.L))
        {
            anim.SetBool("Taunt", true);
        }
        else
        {
            anim.SetBool("Taunt", false);
        }
    }
    void Shoot()
    {
        GameObject currentproj = PhotonNetwork.Instantiate(projectile.name, launchPoint.position, Quaternion.LookRotation(transform.forward));

        currentproj.GetComponent<Rigidbody>().AddForce((anim.transform.forward * lobSpeed) + (Vector3.up * lobLift), ForceMode.Impulse);
        currentproj.GetComponent<PlayerAttackCollision>().playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
    }
    [PunRPC]
    void OnlineMeleeAttack()
    {
        Invoke("ActivateMeleeCollider", attackDelay);
    }
    void ActivateMeleeCollider()
    {
        meleeCollider.SetActive(true);
        Invoke("DeactivateMeleeCOllider", attackLifeTime);
    }
    void DeactivateMeleeCOllider()
    {
        meleeCollider.SetActive(false);
    }


}
