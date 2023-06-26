public class Knowledge
{
    public enum Category {
        Destination,            // navigations officer, captain
        ConditionFacility,      // engineer, gunnery
        ConditionEntity,        // medic
    };

    //These conditions should belong to the entity and facility classes
    public enum ConditionFacility {
        Broken,
        NoPower,
        Loaded,
        Unloaded
    };
    public enum ConditionEntity {
        Hungry,
        Thirsty,
        Bladdered,
        WoundLight,
        WoundMedium,
        WoundBad,
        WoundFatal,
        Dead,
        Happy,
        Sad,
        Depressed,
        Suicidal
    };
    
    private Category category;
    private Facility facility;
    private Entity entity;
    private string destination;
    
    public Knowledge( ConditionFacility condition, Facility facility ) {
        category = Category.ConditionFacility;
    }
    public Knowledge( ConditionEntity condition, Entity entity ) {
        category = Category.ConditionEntity;
    }
    public Knowledge( string destination ) {
        category = Category.Destination;
    }    

}
