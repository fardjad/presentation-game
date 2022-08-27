# Presentation Game

This repository contains the source-code of the serious game I built back in 
2018-2019 for my master's thesis. The game was used to study the impact of 
virtual reality elements on a serious game with the serious purpose of oral 
presentation training.

## Overview

This game can be played with a virtual reality headset or with a keyboard and 
mouse. The objective is for the player to give a presentation to a virtual 
audience with their own slides!

![Slide](/docs/Slide.png?raw=true)

During the presentation, the player must walk around the classroom, maintain 
eye-contact with the audience, and mention a pre-defined set of keywords on 
each slide while keeping an eye on time.

![Audience](/docs/Audience.png?raw=true)

The audience reacts to the presentation; if the player fails to maintain 
eye-contact with the virtual characters, they will start to look around and 
talk with each other.

At the end of the presentation, the characters ask some questions from the 
presenter.

![Question](/docs/Question.png?raw=true)

Once the player finishes answering the questions, the session ends and the game 
provides some basic statistics on the performance of the player instantly.

![Performance Results](/docs/Performance%20Results.png?raw=true)

In addition to the basic stats, it saves the movement data in a JSON file for 
further analysis.

## Technology & Assets

* Game Engine: https://unity.com
* VR Software: https://www.steamvr.com
* 3D Modeling Software: https://www.sketchup.com
* Character models and animations: https://www.mixamo.com

Please refer to the included assets for more information on the software 
components used in the project.

## Instructions for Running the Game

### Requirements

1. Windows
2. SteamVR
3. Microsoft Powerpoint
4. Microsoft Visual Studio 2017
5. Unity 2018.2.13f
6. Node.js 8+

### Steps

1. Clone the repository
2. Build all of the Node.js projects in the [Companion Software](/Companion%20Software/) directory
3. Build and install the [PowerPoint extension](/Companion%20Software/vsto-presentation-game-powerpoint-addin/)
4. Convert the presenters' PowerPoint slides to a sequence of images and place them [here](/Companion%20Software/node-presentation-game-backend/slides/)
5. Use the PowerPoint extension to add keywords to the slides or mark them as 
   question slides. Then export the slides model JSON and save it in [model.json](/Companion%20Software/node-presentation-game-backend/slides/model.json)
6. Run [run.bat](/Companion%20Software/node-presentation-game-servers-runner/run.bat)
   to start the backend and frontend servers
8. Optionally connect and configure the VR hardware and run the game
9. After each session, the in-game data will be save [here](/Companion%20Software/node-presentation-game-backend/data)

## Citing this Work

Please refer to [CITATION.cff](/CITATION.cff)

## License

[CC BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0)
