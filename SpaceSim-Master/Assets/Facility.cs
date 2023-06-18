using System;
using System.Collection.Generic;
using UnityEngine;

public class Facility : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public enum EType {
        Undefined,
        Toilet, 
        Fridge, 
        Sink, 
        Engine, 
        Navigations, 
        LifeSupport, 
        CaptainsChair
    };
    
    public EType Type {get; set;} = EType.Undefined;
    public bool Broken { get; set; } = false;
    public Vector3Int Coordinates { get; set; } = Vector3Int.zero;
    // temporarily removed. Use transform.position instead
    //public Vector3 WorldPosition { get; set; } = Vector3.zero;
    
    /// <summary> When an NPC starts an interaction, roll to see whether facility breaks </summary>
    public virtual void InteractStart() => GameTime.OnTck += GameTime_OnTick;
    public virtual void InteractEnd() => GameTime.OnTck -= GameTime_OnTick;
    
    protected virtual void GameTime_OnTick() {
        if(!broken)
            DamageRoll();
    }
    
    /// <summary> While engaged, every tick there's a 1 in 256 chance the facility will break.
    /// We might change this in the future so skilled crew damage facilities less often. </summary>
    private void DamageRoll() {
        byte r = Random.Range(0, 255);
        if(r <= 0)
            Broken = true;
    }
    
    // This should be moved to a more appropriate class
    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Pointer down");
    } 
    // This should be moved to a more appropriate class
    public void OnPointerUp(PointerEventData eventData) {
        Debug.Log("Pointer up");        
    }
}

public static class Facilities {

    public static PlacementMode { get; set; } = false;
    
    private static List<Facility> FacilityList = new List<Facility>();
    
    public static void Sort()                                        =>        FacilityList.Sort();
    public static void Add(GameObject prefab, Vector3 worldPosition, Vector3Int coordinates) {
        Facility f = GameObject.Instantiate(prefab, worldPosition, Quaternion.Identify).GetComponent<Facility>();
        f.Coordinates = coordinates;
    }
    public static Facility Get(Vector3Int coordinates)               => return FacilityList.FindAll( x => x.Coordinates == coordinates) != null ? FacilityList.FindAll( x => x.Coordinates == coordinates)[0] : null;
    public static Facility Get(Facility.Type t)                      => return FacilityList.FindAll( x => x.m_type == t) != null ? FacilityList.FindAll( x=> x.m_type == t) : null;
    public static void Remove(Vector3Int coordinates)                =>        Remove( Get( coordinates ) );
    public static void Remove(Facility f) {
        FacilityList.Remove(f);
        Destroy(f.gameObject);
    }
}
