using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerConn>() && collision.GetComponent<PhotonView>().IsMine)
        {
            this.GetComponent<PhotonView>().RPC("DestroyItem", RpcTarget.All);
            collision.gameObject.GetComponent<PlayerConn>().GetItem();
        }
    }

    [PunRPC]
    private void DestroyItem()
    {
        Destroy(this.gameObject);
    }

}
