# SpaceSim

![image](https://github.com/ThimbleFire/SpaceSim/assets/14812476/5d4e8c44-1746-4a81-b76a-d7b391440576)

To do:

* [ ] get the game function on mobile!
* [ ] add a starting station your NPCs can undock from, and where you can buy facilities from
* [ ] add a facility pathfind check to make sure the destination facility isn't being used
* [ ] add a ui for ship details, name, model
* [ ] add a star map so the player can see where they are, where they're heading, and what's around
* [ ] add a notification button to display event prompts

ships can only connect facilities with compatible sizes.

Shuttle - very small (12*7*1)
Frigate - medium (8*14*1)
Galleon - large (15*10*2)
Battleship - very large (10*20*3)
Goliath - giant (15*25*5)

facilities need calibrating by the engineer after being installed and before they can be used.


Space is big. There's a 4x4x4 grid of universes with the following names:

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
16. Muertuun

17. Nostralis
18. Oasis
19. Eclipse
20. Ruinova
21. Ormus
22. Malace
23. Ral
24. Dominion
25. Neuton
26. Suen
27. Gigalad
28. Stratos
29. XN-III
30. New Haven
31. Forbiddena
32. Sanctuary

33. Calimax
34. San Cosina
35. Rezifarg
36. Calipso
37. Intuerno
38. Damoros
39. Barados
40. Coloss
41. Temporus
42. Britannica
43. Leviathan
44. Sol
45. Peridot
46. Satalite
47. XN-VII
48. Scarbell

49. Quiloyd
50. Zenema
51. Xandi
52. I'unno
53. U'unno
54. We'unno
55. They'unno
56. Order
57. Morbo
58. Tsunet
59. Dak
60. Llano
61. Waifun
62. Garmot
63. Arryatlee
64. Reldawin

Later...

* [ ] NPCs currently subscribe to job facilities while perfoming jobs.
      While perfoming jobs there's a <rank>/256 chance they'll aquire knowledge.
      The knowledge that facilities offers change according to event circumstance.
* [ ] Add List<Knowledge> to Entities, where Knowledge is type enum { Destination, DamagedFacility, GunneryTarget, Sickness 
