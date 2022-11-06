# **MontyHallSimulator**
**This is a fun project that replicates the experience of playing in the popular TV show Monty Hall!**<br><br>

What was used --><br>
**.NET**.<br>
**C#**.<br>
**MAUI**, a new promising framework by Microsoft with cross-platform build targets (Windows, MacOs, and Android currently).<br>
<br>

This graphical app consists of the top part which contains **the actual game** with a Win/Loss counter, and the bottom part which contains a **customizable simulator** to find the best winning strategy<br><br>

The simulator part runs the same logic as the actual game the number of times chosen by the user.<br><br>

Why was **multi-threading** NOT cosidered necessary:<br>
A method to use parallelism is included for performance testing but not used since parallel threads were slower than a single thread (still external to the main app).<br>
A likely explaination is that the logic for a single game iteration is so fast that the overhead of thread management is actually higher than the performance boost received.<br>
No particular logic for thread parallelism was considered necessary even in static methods since all shared data were stack items as numbers.
<br><br>

Here is a demonstration of the game:

https://user-images.githubusercontent.com/96583994/200169361-ec632bab-7ba5-42e9-89c8-2791b597b6bc.mp4

<br><br>


And of the simulator:

https://user-images.githubusercontent.com/96583994/200169418-5ac90ded-a15c-4de3-8732-8f7f9890e2a1.mp4

<br><br>


A logic for **invalid inputs handling** has been thought of:

https://user-images.githubusercontent.com/96583994/200169451-3c05497d-540c-4f70-873c-7fb5ead1eb84.mp4

<br><br>

Feel free to install and try the app yourself, a compiled version for Windows is available in the "Release" folder.<br>
Thank you for checking this project out.

