# MonteCarlo

## Disclaimer
  This is my FIRST TIME using UWP. There has been a lot of change since WFP, the two specific ones i encountered are the eventhandling for user input and Bitmap class as a whole. I used WriteableBitmapEX To manipulate bitmap in this application.
  This is also my FIRST TIME dealing with async operation. Along with setting up UI update during an Async operation.
  Also as I dive into the world of async, I completely forgot about commiting.
  
## Introduction
  This application will take an number from 0 to int.MaxValue, and generates that many dots. The dots will be generated within the grid of (0.0,0.0) to (1.0,1.0). It will check if the point is within the arc from (1.0,0.0) to (0.0,1.0) and calculate the amount to estimate the PI value.
  The application will provide an random "RandomSeed", however user can provide their own for persistent. Once a input is given, user cannot go back to the default seed.
  The application will also provide a progress bar for user to see how far in the operation is the application. The result will be visualized into an 400 by 400 image with Blue and Orange as color. where the white spot have the least amount of dots landed in them and Blue/Orange spots has the most dots landed in them.
