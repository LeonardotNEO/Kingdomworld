﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttributes : MonoBehaviour
{
    private GameManager gameManager;
    private Collider player;
    public int itemAmount;
    private string itemName;
    private bool playerInBounds;


    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if(this.transform.name.Contains("Wood log")){
            itemName = "Wood log";
            itemAmount = 1;
        } else if(this.transform.name.Contains("Stone")){
            itemName = "Stone";
            itemAmount = 1;
        } else {
            itemAmount = 1;
        }

        this.transform.name = itemName;
    }

    void Update()
    {
        
    }

    public int getItemAmount(){
        return itemAmount;
    }
    public string getItemName(){
        return itemName;
    }
    
    public void OnMouseDown()
    {
        if(!gameManager.GetUI().getIsMouseOverUI()){
            StartCoroutine(walkingToItem()); 
        }
    }

    void OnMouseOver()
    {
        //GetComponentInChildren<Outline>().eraseRenderer = false;
    }

    void OnMouseExit()
    {
        //GetComponentInChildren<Outline>().eraseRenderer = true;
    }
    
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "player"){
            player = other;
            playerInBounds = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "player"){
            playerInBounds = false;
        }
    }

    public void pickUpItem(){
        if(getItemAmount() != 0){
            itemAmount = gameManager.getInventoryCatalog().getMainInventory().addItemToInventory(getItemName(), getItemAmount());
        }
        player.GetComponent<Animator>().SetTrigger("pickingUpItem");
        if(itemAmount == 0){
            Destroy(this.gameObject);
        }
    }

    public IEnumerator walkingToItem(){
        bool runLoop = true;
        gameManager.getPlayerBehavior().moveToPosition(this.transform.position);
        while(runLoop){
            if(playerInBounds){
                gameManager.getPlayerBehavior().stopPlayer();
                gameManager.getPlayerBehavior().playerLookAt(this.gameObject);
                pickUpItem();
                runLoop = false;
            }
            yield return null;
        }
    }
}

