# PlanetGeneration
Procedural generation including zone modeling for better custom user design

![Generated planet sample](image.png)

## How the sphere is done

  Each planet is a sphere, we use 6 planes (corresponding to every face of a cube).
  We then normalize the position of each triangle to get a sphere shape.
  - Pros : each triangles are roughly the same size and the resolution of the plane can be changed easily
  - Cons : the surface normals aren't conistent around the edges, which will lead to visible seams when lit  

![Alt text](firefox_5i2O1KCQJG.gif)

## Planet settings
### General tweaks  
![Alt text](image-1.png)  
The resolution corresponds to how many squares each planes contains, minus one. So a resolution set to 60 correponds to a plane containing 59x59 squares.  
Face render mask can be used to tell which of the faces of the sphere you want to display (for performance) 

#### Shape settings
![Alt text](image-2.png)

#### Color settings
![Alt text](image-3.png)