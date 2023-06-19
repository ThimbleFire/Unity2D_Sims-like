using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
    public static string[] RandomName = new string[] {
        "John",
        "Riley",
        "Reese",
        "Milad",
        "Phoe",
        "Tony",
        "Miller",
        "Rorke",
        "Soloman",
        "Burk",
        "Raegan",
        "Finch",
        "Fusco",
        "Shaw",
        "Rodge",
        "Marcus",
        "Ash",
        "Old Greg",
        "Deacon",
        "'arry",
        "Bez",
        "Tich",
        "Lawry"
    };

    public static string GetRandomName { get { return RandomName[UnityEngine.Random.Range( 0, RandomName.Length )]; } }
}
