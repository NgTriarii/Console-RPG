using OOD_Project.Entities;
using OOD_Project.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project;

// Interface for drawing the game
public interface IGameView
{
    void Initialize();

    void Render(GameModel model, Player localPlayer, int cursorPos, bool showLog, InputHandler inputChain);

    void ShowGameOver();
}
