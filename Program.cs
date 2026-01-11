
using CasinoGame.Core;

namespace CasinoGame {
    internal class Program {
        static void Main(string[] args) {

            var casino = new Casino();
            casino.StartGame();

            var loop = new GameLoop(casino);
            loop.Run();
        }
    }
}