public class Core{
   
   // Behaviour is what they are currently doing
   public enum CurrentBehaviour {
      Eating = 1, 
      Drinking = 2, 
      Playing = 3, 
      Defecating = 4,
      Resting = 5,
      
      Captaining = 105,// moving the ship, increasing BoardManager.Progress;
      Repairing = 116, // something is broken, best repair it
      Monitoring = 117,// nothing is broken, best stand around
      Loading = 127,   // loading guns
      Firing = 128,    // shooting guns
      Tending = 138,   // tending to the sick
      Waiting = 139,   // no one is sick, standing around waiting in sick-bay
      Scanning = 149,  // scanning the radar
      Recalling = 150, // telling crew what's on the radar
      Idling = 254,
      Walking = 255,
    }
   
   // Impulses are things they feel they need to do
   // Impulses have a higher priority than responsibilities
   public enum Impulses {
      Hunger = 1, 
      Water = 2,
      Happiness = 3,
      Bladder = 4,
      Resting = 5
       
      Oxygen = 64
    }
   
   // Solutions are flags that corrospond to impulses, and are ways to eliviate impulses
    public enum Solutions {
       Fridge = 1,
       Sink = 2,
       Play = 3,
       Toilet = 4,
       Resting = 5, // this is not a facility and is essentially currentBehaviour.Idling for when an NPC is overworked
       
       Chair = 105
          
       LifeSupport = 64,
    }
   
   // Responsibilites are desegnated jobs for NPCs
    public enum Responsibility {
       Captain = 105,
       Engineer = 116,
       Gunner = 27,
       Medic = 138,
       Mediator = 149 
    }
}
