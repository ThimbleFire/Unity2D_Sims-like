public class Helper
{
    public static string[] RandomName = new string[] {
        "Milad",
        "Phoe",
        "Tony",
        "Beggsy",
        //
        "Rorke",
        "Soloman",
        "Burk",
        "Charlie",
        "Fodder",
        //
        "Riley",
        "Reese",
        "John",
        "Finch",
        "Fusco",
        "Shaw",
        "Root",
        //
        "Roger",
        "Major",
        "Victor",
        //
        "White",
        "Black",
        //
        "Raegan",
        "Bush",
        "Washington",
        "Lincoln",
        "Roosevelt",
        //
        "Miller",
        "Marcus",
        "Ash",
        "Old Greg",
        "Deacon",
        "'arry",
        "Bez",
        "Tich",
        "Fry",
        "Lawry",
        "Hunter",
        //
        "Thimble"
    };

    public static string GetRandomName { get { return RandomName[UnityEngine.Random.Range( 0, RandomName.Length )]; } }

    public static UnityEngine.Vector3 CellToWorld( UnityEngine.Vector3Int coordinates ) {
        return new UnityEngine.Vector3( coordinates.x * 0.08f, coordinates.y * 0.08f, 0.0f );
    }
}
