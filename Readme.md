# Console-Based RPG (Work in Progress)

This is a work-in-progress, character-based console RPG built using C# and .NET. The project serves as an exercise in advanced Object-Oriented Design (OOD), heavily utilizing Gang of Four (GoF) design patterns—such as Builder, Command, and Chain of Responsibility—to create a modular and scalable game engine. It features a custom, flicker-free Unicode rendering engine, procedural map generation, and a polymorphic inventory and equipment system.

### How to Run

Prerequisites

.NET 9.0 SDK or later installed on your system.

Alternatively, Visual Studio 2022.

###### Using the .NET CLI

Open your terminal or command prompt.

Navigate to the root directory of the project (where the OOD-Project.csproj file is located).

Run the following command:

dotnet run


##### Using Visual Studio

Open the OOD-Project.sln file in Visual Studio 2022.

Ensure the project is set as the startup project.

Press F5 or click Start to build and launch the game.

Note: The game automatically adjusts the console buffer and window size upon launch to ensure the map and UI render correctly.

#### Controls

The game relies on standard keyboard inputs to navigate the world and interact with your inventory.

W, A, S, D / Arrow Keys - Move player

E - Pick up an item from the ground

I - Cycle the inventory cursor

R - Equip the currently highlighted item

T - Unequip currently held items

G - Drop the currently highlighted item

Esc - Exit the game
