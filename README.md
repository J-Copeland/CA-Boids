# CA-Boids
Layering a Boids model with cellular automata in the hopes of simulating more complex behaviours.

## Description:
Boids, as described by Craig Reynolds (1987), are simulated flocking creatures whose behaviour is emergent from three simple rules:
- Separation: Boids must steer to avoid flockmates
- Alignment: Boids must steer towards the average direction the flock is facing
- Cohesion: Boids must steer towards the average position of the flock

Based on these simple rules, the simulated 'bird-oid' creatures display behaviour similar to that of their real life counterparts. There are many variations and extensions to this model, and this project proposes another - namely by introducing a context-dependant weighting to the three behaviours. This weighting is calculated based on the ratio of live-to-dead cells in a cellular automaton (CA), and within this CA, the individual cells are initially populated based on the urgency of the three rules that control the boids.
Broken down into a series of steps, the system works like this:
1. each boid calculates an 'urgency' value for each of it's behaviours
2. the urgency of each behaviour acts as a likelihood to populate cells in a triangular grid representing each boid's 'brain'
3. after a fixed number of steps in the boids' CA, the total number of live cells in the three corners of the triangle (each representing one behaviour) are counted
4. the percentage of live cells in each corner are used as the actual weighting for the boid rules

I hope (and predict) that by allowing the initial urgency values to interact with each other (as well as an element of randomness) to the boids' brains, this added layer of obfuscation will result in a model that more accurately mirrors the seemingly random behaviours displayed by real animals. 

I also aim to generate a random seed for each boid to control the random values, this reintroduces determinism to the model, as well as giving each boid a unique 'nature' - suppose a boid with a CA size of T₄ (4 rows of triangles: 10 cells, 3 cells dedicated to each rule with 1 central cell) and a seed starting with three 0's, that boid's nature would prevent it from following one of the rules.

### Where I'm up to:
- Working cellular automata - uses a triangle based grid instead of a square to fit the 3 rules boids follow
- (Mostly) working boids model - I'm using quaternions to handle the boids' target rotations and blending these is currently uneven (explained further below)
- Boundary system for boids - if the boids move past a certain point within a cube of a given size, they'll be shifted to the other side, allowing for an enclosed testing environment
- Camera control system - allows the program user free movement within the environment, or to lock on and follow certain boids
- Link between cellular automata and boids - each boid agent has it's own cellular automaton, and each cellular automaton can be seen by selecting a boid

### What's happening next:
- Better quaternion blending (currently working on) - researching and implementing a better method to facilitate more complex blending between weighted quaternions, made harder by the fact that I'm working with three quaternions instead of two. I'm currently considering either a SLERP (Spherical Linear Interpolation) or a QUEST (Quaternion Estimator) algorithm implementation.
- Calculating urgencies for behaviours
- More granular cellular automata rule control - updating the model from Carter Bays' (1994) demonstrated LFR rule to a 2 dimensional variation of the wolfram code, this will allow for better random cell behaviour using certain rules (e.g. rule 53), as demonstrated by Paul Cousin (2024) 


## Sources:
- Reynolds, C.W. (1987). 'Flocks, herds and schools: A distributed behavioral model'. *ACM SIGGRAPH Computer Graphics* [online] 21(4):25–34, doi:10.1145/37402.37406.
- Bays, C. (1994). 'Cellular Automata in the Triangular Tessellation'. *Complex Sytems* 8(2):127-150, [online] Available at: https://content.wolfram.com/sites/13/2018/02/08-2-4.pdf [Accessed 28 Oct. 2025].
- Cousin, P. (2024). 'Triangular Automata: The 256 Elementary Cellular Automata of the Two-Dimensional Plane'. *Complex Systems* [online] 33(3):253–276, doi:10.25088/complexsystems.33.3.253.

‌
