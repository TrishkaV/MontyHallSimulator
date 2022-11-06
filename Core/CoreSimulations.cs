using System.Diagnostics;

namespace MontyHall.Core
{
    internal class CoreSimulations
    {
        private readonly MainPage mp = new();

        /* NOTE: during tests through the method OptimizeNParallelSim() below, best "nParallel" value
         * for a "no door change" scenario is 1.
         */
        internal (int simulationsWon, int simulationsLost) RunSimulations(int nSimulations, bool isDoorChange, int nDoors, int nParallel = 1)
        {
            var simulationsWon = 0;
            var simulationsLost = 0;

            /* Logic: which door ( 1 | 2 | 3 ) is chosen at the beginning of
             * the game is indifferent so we assume that the door chosen is
             * always n. 1.
             */
            if (!isDoorChange)
                Parallel.For(0, nSimulations, new ParallelOptions { MaxDegreeOfParallelism = nParallel }, _ =>
                {
                    if (mp.winners.Contains(mp.RunContest(nDoors)[0]))
                        simulationsWon++;
                    else
                        simulationsLost++;
                });
            else
                Parallel.For(0, nSimulations, new ParallelOptions { MaxDegreeOfParallelism = nParallel }, _ =>
                {
                    var results = mp.RunContest(nDoors);
                    /* Logic: if the door choses was a winner then changing the 
                     * door will result in a loss.
                     */
                    if (mp.winners.Contains(results[0]))
                        simulationsLost++;
                    /* Logic: if the door chosen was a loser and then all other
                     * non-winning door are removed then there is a 1/x
                     * chance of winning by changing, (where x is the possibility
                     * of winning without the change).
                     */
                    else
                        simulationsWon++;
                });

            return (simulationsWon, simulationsLost);
        }

        /* NOTE: this method takes several minutes in case you want to try it
         */
        internal int OptimizeNParallelSim(int nSimulations, bool isDoorChange)
        {
            var sw = Stopwatch.StartNew();
            var elapsed = new Dictionary<int, long>();

            for (var i = 1; i < 100; i++)
            {
                sw.Start();
                RunSimulations(nSimulations, isDoorChange, i);
                sw.Stop();
                elapsed.Add(i, sw.ElapsedMilliseconds);
                sw.Reset();
            }

            var bestCycle = elapsed.First(x => x.Value == elapsed.Min(y => y.Value)).Key;
            return bestCycle;
        }
    }
}
