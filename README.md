# PlanetGeneration
## [Google Slides](https://docs.google.com/presentation/d/1SNPC4vbOUnWQxdZw4O68Bk-GxxRdYBxvVE_aXRabYLE/edit?usp=sharing)

Procedural planet generation inspired by a video series by Sebastian Lague:  
https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8

![Generated planet sample](Images/image.png)

## How the sphere is done

  Each planet is a sphere, we use 6 planes (corresponding to every face of a cube).
  We then normalize the position of each triangle to get a sphere shape.
  - Pros: each triangles are roughly the same size and the resolution of the plane can be changed easily
  - Cons: the surface normals aren't conistent around the edges, which will lead to visible seams when lit  

![Alt text](Images/firefox_5i2O1KCQJG.gif)

## Planet settings
### General tweaks  
![Alt text](Images/image-1.png)  
The resolution corresponds to how many squares each planes contains, minus one. So a resolution set to 60 correponds to a plane containing 59x59 squares.  
Face render mask can be used to tell which of the faces of the sphere you want to display (for performance). 

### Shape settings
![Alt text](Images/image-2.png)  

#### Filter type 
![Alt text](Images/image-6.png)  
*Simple noise*  
![Alt text](Images/image-5.png)  
*Rigid noise*  

**Strength:** the strength of the noise. Higher value means a greater difference between the highest and the lowest point.   
**Number of layers:** layers added together to make more interesting terrain. Start at a relatively low roughness (`Base roughness`) but high amplitude (`Strength`) to create the basic shape of the mesh, then we increment the roughness (by the `Roughness` value) and decrement the amplitude (`Strength` multiplied by the `Persistence` value) to get more details.  
![Alt text](Images/image-4.png)  
**Base roughness:** base frequency of the noise. High value means more abrupt and frequent changes in the noise.    
**Roughness:** for each layers, we will increment the roughness by this value, to get progressively more details.  
**Persistence:** the value with which the strength will be multiply by for each of the noise layers. If <1, the amplitude will decrease.  
**Center:** allows to move the center of the noise.  
**Min value:** the minimum elevation value for the noise to be used. It allows to have an ocean level.  
![Alt text](Images/Unity_J85lZOAVod.gif) 



### Color settings
![Alt text](Images/image-3.png)  
#### Biomes 
You can add as many biomes as you wish to have different climates on your planet. Each of these biomes consists of:  
**Gradient:** is the color palette of the terrain in the biome. The left side corresponds to the lowest terrain and the further you go right, the highest the terrain needs to be in order to be colored.  
**Tint:** is used to apply a tint on the color palette of the biome.  
**Start height:** tells at which fraction of the planet the biome starts.  
**Tint percent:** allows to adjust the strongness of the tint.  
![Alt text](Images/Unity_lYR1DlJUKv.gif)
  
**Noise:** is used in order to have more interesting edges on the biome.  
**Blend amount:** allows to soften the demarcation between the biomes.  
![Alt text](Images/Unity_nm50Mwk3aN.gif)  

**Ocean color:** is the color palette of the ocean, the same rules apply as for terrain.
![Alt text](Images/Unity_Qijaa89eJD.gif)