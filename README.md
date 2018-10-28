# ProcJam-2018
ProcJam 2018 Entry. Simulate basic population genetics on fake creatures' DNA. Concepts of mutation, crossover, fitness distribution covered.

Try it Out Here - https://barrettotte.github.io/ProcJam-2018/WebGL/index.html

## Links
* Procedural Generation Jam 2018 - https://itch.io/jam/procjam
* WebGL Build - Try it Out Here - https://barrettotte.github.io/ProcJam-2018/WebGL/index.html
* Procedural Generation Jam 2018 Entry Itch.io Page - XXX
* This idea was prototyped in Python first, https://github.com/barrettotte/Population-Genetics-Sim


## Summary
* This script will simulate a generation of "creatures" generation after generation until the ideal color allele is found.
* Each creature has one allele in their "DNA" for color and is passed to offspring based on fundamentals of genetics (crossover, mutation). 
* Depending on how close the creature's color allele is to the ideal allele, it is given a fitness value which affects its chances of reproducing with another creature.


## Process
* **Initialize** - Population N random DNA
* **Selection** - Evaluate fitness, generate mating pool
* **Reproduction** - Repeat N times, pick parents based on "weighted random" of mating rate
* **Crossover** - Combine DNA through some crossover method
* **Mutation** - Mutate child based on mutation rate
* Add child to new generation
* Discard old population
* Repeat


### Additional Process
* A percentage of "Top" fitness organisms will asexually reproduce to keep their ideal DNA in the mating pool.
* There is a small chance a random organism will asexually reproduce


## Sources:
* Basics of bitwise crossover/mutation http://www.obitko.com/tutorials/genetic-algorithms/crossover-mutation.php
* Noun and Adjective Lists https://github.com/aaronbassett/Pass-phrase


