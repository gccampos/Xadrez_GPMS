﻿
using System.Xml.Serialization;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlights : MonoBehaviour
{
    public static Highlights instance;
    public SpriteRenderer highlightsPrefab;
    Queue<SpriteRenderer> activeHighlights= new Queue<SpriteRenderer>();
    Queue<SpriteRenderer> onReserve= new Queue<SpriteRenderer>();

    void Awake(){
        instance=this;
    }
    public void SelectTiles(List<Tile> tiles){
        foreach(Tile t in tiles){
            if(onReserve.Count==0){
                  CreteHighlight();
            }
            SpriteRenderer sr=onReserve.Dequeue();
            sr.gameObject.SetActive(true);
            sr.color=StateMachineController.instance.currentlyPlaying.color;
            sr.transform.position= new Vector3(t.pos.x,t.pos.y+1,0);
            activeHighlights.Enqueue(sr);
        }
    }
    public void DeSelectTiles(){
        while(activeHighlights.Count!=0){
            SpriteRenderer sr=activeHighlights.Dequeue();
            sr.gameObject.SetActive(false);
            onReserve.Enqueue(sr);
        }
    }
    void CreteHighlight(){
        SpriteRenderer sr= Instantiate(highlightsPrefab,Vector3.zero,Quaternion.identity,transform);
        onReserve.Enqueue(sr);
    }
}
