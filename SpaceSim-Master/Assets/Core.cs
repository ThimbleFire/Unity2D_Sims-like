public class Core{
   
   // Behaviour is what they are currently doing
   public enum CurrentBehaviour {
      Eating = 1, 
      Drinking = 2, 
      Playing = 3, 
      Defecating = 4,
      
      Captaining = 5,
      
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
       
      Oxygen = 64
    }
   
   // Solutions are flags that corrospond to impulses, and are ways to eliviate impulses
    public enum Solutions {
       Fridge = 1,
       Sink = 2,
       Play = 3,
       Toilet = 4,
       Chair = 5
       
          
       LifeSupport = 64,
    }
   
   // Responsibilites are desegnated jobs for NPCs
    public enum Responsibility {
        Captain, Engineer, Gunner, Medic, Mediator 
    }
}
