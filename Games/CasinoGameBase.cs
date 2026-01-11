using System;

namespace CasinoGame.Games {
    public abstract class CasinoGameBase {
        public event Action? OnWin;
        public event Action? OnLoose;
        public event Action? OnDraw;

        protected CasinoGameBase() {
            FactoryMethod();
        }

        public abstract void PlayGame();

        protected abstract void FactoryMethod();

        protected void OnWinInvoke() {
            Console.WriteLine("Result: You win!");
            Console.WriteLine("=====================================");
            OnWin?.Invoke();
        }

        protected void OnLooseInvoke() {
            Console.WriteLine("Result: You lose!");
            Console.WriteLine("=====================================");
            OnLoose?.Invoke();
        }

        protected void OnDrawInvoke() {
            Console.WriteLine("Result: Draw!");
            Console.WriteLine("=====================================");
            OnDraw?.Invoke();
        }
    }
}