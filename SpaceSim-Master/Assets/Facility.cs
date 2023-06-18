using System;
using System.Collection.Generic;
using UnityEngine;

public class Facility : MonoBehaviour {

    public enum Type {
        Toilet, 
        Fridge, 
        Sink, 
        Engine, 
        Navigations, 
        LifeSupport, 
        CaptainsChair
    }
    
    public Type m_type;
    public bool Engaged { get; set; } = false;
    public bool Broken { get; set; } = false;
    public Vector3Int Coordinates { get; set; } = Vector3Int.zero;
    
    /// <summary> When an NPC starts an interaction, roll to see whether facility breaks </summary>
    public void InteractStart() {
        Engaged = true;
    }
    public void InteractEnd() {
        Engaged = false;
    }
}

public static class Facilities {

    private static List<Facility> FacilityList = new List<Facility>();
    
    public static void Sort() => FacilityList.Sort();
    public static void Add(Facility f) => FacilityList.Add(f);
    public static void Remove(Facility f) => FacilityList.Remove(f);
    public static void Remove(Vector3Int coordinates) => Facility.RemoveAll( x => x.Coordinates == coordinates);
    public static Facility Get(Vector3Int coordinates) =>   return FacilityList.FindAll( x => x.Coordinates == coordinates) != null ? FacilityList.FindAll( x => x.Coordinates == coordinates)[0] : null;
    public static Facility Get(Facility.Type t) =>          return FacilityList.FindAll( x => x.m_type == t) != null ? FacilityList.FindAll( x=> x.m_type == t) : null;
}
