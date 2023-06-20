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

    public static UnityEngine.Vector3 CellToWorld(UnityEngine.Vector3Int coordinates) {
        return new UnityEngine.Vector3( coordinates.x * 0.08f, coordinates.y * 0.08f, 0.0f );
    }
}
