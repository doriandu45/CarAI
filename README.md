# A genetic algorithm example using cars and Unity

# Downloads
You can find the pre-built binaries for Windows, Linux and MacOSX [here](https://github.com/doriandu45/CarAI/releases)

# How to use?
Simply run the executable. At the beginning, you can change the following settings:
  - Cars per batch: How many cars spawn each time. Setting it too high (100-200 or more) may cause lag depending of your computer
  - Batches: how many batches of n cars per generation. After all the batches are done, the next generation is generated and start automatically
  - Checkpoint cooldown: The time cars have to reach a checkpoint. If they don't reach it in time, they die
  - Cooldown scaling: Each generation, the cooldown gets multiplied by this value. The goal is to reduce progressiveley the cooldown. You can set it to 1 if you don't want the cooldown to change, or even higher if you want the cooldown to increase
  - Minimum cooldown: the cooldown won't get lower than this value
  - Mutation rate: it represent "1 chance out of x" for a mutation to occur. Setting it high is recommended.
 
# What is it?
At the beginning, it's a little personnal project, and also the first time I used Unity ever. It was made for a quick demo during a presentation in my English class about AI.
WARNING: The code may not be great, but it works. Also, the comments are in French

