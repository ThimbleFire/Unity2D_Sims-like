# SpaceSim

![image](https://github.com/ThimbleFire/SpaceSim/assets/14812476/5d4e8c44-1746-4a81-b76a-d7b391440576)

To do:

* [ ] Animate UI
* [ ] Add a 3rd dimension to walls

Space is big. There's a 8x8 grid of universes, each containing between 1 and 7 planets, and many docking stations.
Space travel is calculated just like path finding. When an event occurs, a random station, planet, wreck, etc, is selected from a random universe, and the player can choose how to respond.
The navigations officer is responsible for calculating a path, then passing that path on to the captain.

1. Leon
2. Sarcoph
3. Jenova
4. Numaki
5. Rosswell
6. Terilla
7. Mort
8. Atomos
9. Cerno
10. Cardo
11. Phalix
12. Janua
13. Old Bailey
14. XN-IX
15. XN-XIV
16. 


Later...

* [ ] NPCs currently subscribe to job facilities while perfoming jobs.
      While perfoming jobs there's a <rank>/256 chance they'll aquire knowledge.
      The knowledge that facilities offers change according to event circumstance.
* [ ] Add List<Knowledge> to Entities, where Knowledge is type enum { Destination, DamagedFacility, GunneryTarget, Sickness 
