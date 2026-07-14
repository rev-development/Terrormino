namespace Helpers
{
    public class WrappedLoopCounter
    {

        public int CurrentIncrement = 0;

        public WrappedLoopCounter(int maxIncrement) {
            MaxIncrement = maxIncrement;
        }

        public int MaxIncrement { get; }

        public void Increment() {
            if (CurrentIncrement + 1 >= MaxIncrement)
            {
                CurrentIncrement = 0;
            }
            else
            {
                CurrentIncrement += 1;
            }
        }

    }
}