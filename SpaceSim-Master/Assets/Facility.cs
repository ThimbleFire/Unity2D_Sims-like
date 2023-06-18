using System;
using System.Collection.Generic;
using UnityEngine;

public class Facility : MonoBehaviour {

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
    public bool Engaged { get; set; } = false;
    public bool Broken { get; set; } = false;
    public Vector3Int Coordinates { get; set; } = Vector3Int.zero;
    
    /// <summary> When an NPC starts an interaction, roll to see whether facility breaks </summary>
    public virtual void InteractStart() => GameTime.OnTck += GameTime_OnTick;
    public virtual void InteractEnd() => GameTime.OnTck -= GameTime_OnTick;
    protected virtual void GameTime_OnTick() { }
}

public static class Facilities {

    private static List<Facility> FacilityList = new List<Facility>();
    
    public static void Sort()                                        =>        FacilityList.Sort();
    public static void Add(Facility f)                               =>        FacilityList.Add(f);
    public static void Add(GameObject prefab, Vector3 worldPosition) =>        FacilityList.Add( GameObject.Instantiate(prefab, worldPosition, Quaternion.Identify).GetComponent<Facility>() );
    public static Facility Get(Vector3Int coordinates)               => return FacilityList.FindAll( x => x.Coordinates == coordinates) != null ? FacilityList.FindAll( x => x.Coordinates == coordinates)[0] : null;
    public static Facility Get(Facility.Type t)                      => return FacilityList.FindAll( x => x.m_type == t) != null ? FacilityList.FindAll( x=> x.m_type == t) : null;
    public static void Remove(Vector3Int coordinates)                =>        Remove( Get( coordinates ) );
    public static void Remove(Facility f) {
        FacilityList.Remove(f);
        Destroy(f.gameObject);
    }
}
