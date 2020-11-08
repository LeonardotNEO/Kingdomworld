﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public int inventoryCapacity;
    public string inventoryName;
    private GameManager gameManager;
    public int inventoryID;
    ShowMainInventory showMainInv;
    ShowBuildingInventory showbuildingInv;
    
    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        inventoryName = transform.name;

        if(transform.tag == "player"){
            inventoryCapacity = 30;
        }
        showMainInv = GameObject.FindGameObjectWithTag("InventoryMenuUI").transform.Find("Background/Scroll View/Viewport/MainInventory").GetComponent<ShowMainInventory>();
        showbuildingInv = GameObject.FindGameObjectWithTag("BuildingOpenUI").transform.Find("Background/Inventory/Scroll View/Viewport/BuildingInventory").GetComponent<ShowBuildingInventory>();
    }

    void Start()
    {
          instatiateInventory();
    }

    public void instatiateInventory(){
        if(transform.tag == "Residential" || transform.tag == "Industrial"){
            inventoryCapacity = gameManager.getBuildingCatalog().getBuildingByName(transform.name).getStorageCapacity();
        }
        for(int i = 0; i < inventoryCapacity; i++){
            inventorySlots.Add(new InventorySlot());
        }

        // Gets the inventory catalog and add this inventory to it
        if(transform.tag == "player"){
            gameManager.getInventoryCatalog().addInventoryToCatalogAtIndex(0, this);
            inventoryID = 0;
        } else {
            gameManager.getInventoryCatalog().addInventoryToCatalog(this);
            inventoryID = gameManager.getInventoryCatalog().getAmountOfInventoriesInCatalog();
        } 
    }
    
    public Inventory getInventory(){
        return this;
    }

    public int getInventoryCapacity(){
        return inventoryCapacity;
    }

    public string getInventoryName(){
        return inventoryName;
    }


    public void clickInventoryItem(int slotnumber, Inventory toInventory){
        if(gameManager.getBuildingCatalog().getBuildingLastClicked() && gameManager.GetUI().getBuildingInventoryOpen() && this.getInventorySlot(slotnumber).currentAmountInSlot != 0){
            if(gameManager.getBuildingCatalog().getBuildingLastClickedAttributes().getPlayerEnteredBuilding()){

                int amountToRemove = toInventory.addItemToInventory(this.getInventorySlot(slotnumber).getItemInSlot() , this.getInventorySlot(slotnumber).getCurrentAmountInSlot());
                removeAmountFromSpecificSlot(this.getInventorySlot(slotnumber), this.getInventorySlot(slotnumber).getCurrentAmountInSlot() - amountToRemove);

            } else {
                gameManager.getMessageLogText().addMessageToLog("The player needs to be inside the building in order to transfer items to building inventory");
            }
        }
        updateInventoryInterface();
    }

    public void removeAmountFromSpecificSlot(InventorySlot inventorySlotSpecific, int amount){
        inventorySlotSpecific.decreaseCurrentAmountInSlot(amount);
        updateInventoryInterface();
    }

    public int addItemToInventory(string item, int amount){
        int amountLeftToAdd = amount;
        List<InventorySlot> invSlotsWithItemAlready = new List<InventorySlot>();

        foreach(InventorySlot inventorySlot in inventorySlots){
            if(inventorySlot.getItemInSlot() == item && inventorySlot.getCurrentAmountInSlot() < 99){
                invSlotsWithItemAlready.Add(inventorySlot);
            }
        }
        
        foreach(InventorySlot inventorySlot in invSlotsWithItemAlready){
            if(amountLeftToAdd == 0){
                break;
            }
            if(amountLeftToAdd + inventorySlot.getCurrentAmountInSlot() <= inventorySlot.getslotCapacity()){
                int amountToAddToThisSlot = amountLeftToAdd;
                inventorySlot.increaseCurrentAmountInSlot(amountToAddToThisSlot);
                amountLeftToAdd -= amountToAddToThisSlot;
            } else {
                int amountToAddToThisSlot = inventorySlot.getslotCapacity() - inventorySlot.getCurrentAmountInSlot();
                inventorySlot.increaseCurrentAmountInSlot(amountToAddToThisSlot);
                amountLeftToAdd -= amountToAddToThisSlot;
            }
        }


        foreach(InventorySlot inventorySlot in inventorySlots){
            if(amountLeftToAdd == 0){
                break;
            }
            if(inventorySlot.getItemInSlot() == item){
                if(amountLeftToAdd + inventorySlot.getCurrentAmountInSlot() <= inventorySlot.getslotCapacity()){
                    int amountToAddToThisSlot = amountLeftToAdd;
                    inventorySlot.increaseCurrentAmountInSlot(amountToAddToThisSlot);
                    amountLeftToAdd -= amountToAddToThisSlot;
                } else {
                    int amountToAddToThisSlot = inventorySlot.getslotCapacity() - inventorySlot.getCurrentAmountInSlot();
                    inventorySlot.increaseCurrentAmountInSlot(amountToAddToThisSlot);
                    amountLeftToAdd -= amountToAddToThisSlot;
                }
            } else {
                if(inventorySlot.getCurrentAmountInSlot() == 0){
                    if(amountLeftToAdd + inventorySlot.getCurrentAmountInSlot() <= inventorySlot.getslotCapacity()){
                        int amountToAddToThisSlot = amountLeftToAdd;
                        inventorySlot.increaseCurrentAmountInSlot(amountToAddToThisSlot);
                        inventorySlot.setItemInSlot(item);
                        amountLeftToAdd -= amountToAddToThisSlot;
                    } else {
                        int amountToAddToThisSlot = inventorySlot.getslotCapacity() - inventorySlot.getCurrentAmountInSlot();
                        inventorySlot.increaseCurrentAmountInSlot(amountToAddToThisSlot);
                        amountLeftToAdd -= amountToAddToThisSlot;
                    }
                }
            }
        }
        if(amountLeftToAdd != 0){
            gameManager.getMessageLogText().addMessageToLog("Inventory is full! " + amountLeftToAdd + " " + item + " could not be added to the inventory.");
        }
        updateInventoryInterface();
        return amountLeftToAdd;
    }

    
    public void removeItemFromInventory(string item, int amount){
        int amountToRemove = amount;

        for(int i = inventoryCapacity - 1; i > -1; i--){
            
            if(inventorySlots[i].getItemInSlot() == item){
                int amountToDecrease;
                if(amountToRemove >= inventorySlots[i].getCurrentAmountInSlot()){
                    amountToDecrease = inventorySlots[i].getCurrentAmountInSlot();
                } else {
                    amountToDecrease = amountToRemove;
                }
                inventorySlots[i].decreaseCurrentAmountInSlot(amountToDecrease);
                amountToRemove -= amountToDecrease;
                if(inventorySlots[i].getCurrentAmountInSlot() == 0){
                    inventorySlots[i].setItemInSlot(null);
                }
            }
            if(amountToRemove == 0){
                break;
            }
        }
        updateInventoryInterface();
    }

    public bool checkIfListOfItemsAreInInventory(Dictionary<string, int> checkList){
        string trueOrFalse = "";
        foreach(var item in checkList){
            int itemAmountFound = 0;

            for(int i = 0; i < inventoryCapacity; i++){
                if(item.Key == inventorySlots[i].getItemInSlot()){
                    itemAmountFound += inventorySlots[i].getCurrentAmountInSlot();
                }
            }
            if(itemAmountFound >= item.Value){
                trueOrFalse += "true";
            } else {
                trueOrFalse += "false";
            }
        }
        if(trueOrFalse.Contains("false")){
            return false;
        } else {
            return true;
        }
    }

    public void sendItemsFromInventory1ToIventory2(Inventory inv1, Inventory inv2, InventorySlot inventorySlot){
        inv2.addItemToInventory(inventorySlot.getItemInSlot(), inventorySlot.getCurrentAmountInSlot());
        inv1.removeItemFromInventory(inventorySlot.getItemInSlot(), inventorySlot.getCurrentAmountInSlot());
    }

    public List<InventorySlot> getInventorySlots(){
        return inventorySlots;
    }

    public string getInventoryToString(){
        string inventoryString = "";
        foreach(InventorySlot inventorySlot in inventorySlots){
            inventoryString += inventorySlot.getCurrentAmountInSlot() + " " + inventorySlot.getItemInSlot();
        }
        return inventoryString;
    }

    public InventorySlot getInventorySlot(int index){
        return inventorySlots[index];
    }
    public void setInventoryCapacity(int number){
        inventoryCapacity = number;
    }

    public void updateInventoryInterface(){
        if(GameObject.FindGameObjectWithTag("InventoryMenuUI").transform.Find("Background").gameObject.activeSelf){
            showMainInv.updateInventory();
        }
        if(GameObject.FindGameObjectWithTag("BuildingOpenUI").transform.Find("Background").gameObject.activeSelf){
            showbuildingInv.updateInventory();
        }
    }
}