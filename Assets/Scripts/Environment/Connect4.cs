using NNNCSharp.Components.Autodiff;
using NNNCSharp.Components.DQNEnvironments;
using NNNCSharp.Components.Models;
using NNNCSharp.Components.Utilities;
using System;
using System.Collections.Generic;

namespace Environment
{
    public class Connect4 : DQNEnvironment, ISelfPlay
    {
        // Base Environment API overrides
        public override Tensor StateFormat => new(new int[] { 1, RowCount * ColumnCount + 1 }); // encodes state of all 42 board positions + player to move
        public override int ActionCount => ColumnCount; // 1 per column

        // Self-play interface API overrides
        public bool AgentTurn { get; set; } = true;
        public int OpponentCount { get; set; }
        public int OpponentIndex { get; set; }

        // Internal board representation
        const int RowCount = 6;
        const int ColumnCount = 7;
        readonly Tensor State = new(new int[] { RowCount, ColumnCount }); // 1 -> red, 0 -> empty, -1 -> yellow
        public bool RedTurn { get; private set; } = true;

        // Training parameters
        const float WinReward = 2.0f;
        const float TieReward = 0.15f;

        // Utilities
        public Random Random { get; init; } = new();

        public Connect4() { }

        ~Connect4()
        {
            State.Dispose();
        }

        // Base Environment API overrides

        public override Tensor GetNormalizedState()
        {
            return GetState(); // no normalization needed - all values already between -1 and 1
        }

        public override Tensor GetState()
        {
            Tensor state = new(new int[] { 43 });
            State.Data.CopyTo(state.Data);
            state[42] = RedTurn ? 1.0f : -1.0f;
            return state;
        }

        public override void Reset()
        {
            AgentTurn = Random.Next(2) == 1; // randomly pick agent to play red or yellow
            (this as ISelfPlay).UpdateOpponentIndex(); // select a new opponent agent for the next episode

            // Reset all positions to empty
            for (int i = 0; i < State.ElementCount; i++)
            {
                State[i] = 0.0f;
            }
            RedTurn = true;
        }

        public override int PickAgentAction(Tensor qValues, Tensor state = null)
        {
            state ??= GetNormalizedState();

            int action = Tensor.ArgMax(qValues);
            while (!ValidAction(action, state))
            {
                qValues[action] = float.MinValue;
                action = Tensor.ArgMax(qValues);
            }
            return action;
        }

        public override int PickRandomAction()
        {
            List<int> validActions = new();
            for (int col = 0; col < ColumnCount; col++)
            {
                if (ValidAction(col)) validActions.Add(col);
            }
            return validActions[Random.Next(validActions.Count)];
        }

        public override bool ValidAction(int action, Tensor state = null)
        {
            state ??= GetNormalizedState();
            using Tensor gridState = new(new int[] { 6, 7 });
            state.Data[..^1].CopyTo(gridState.Data);
            for (int row = 0; row < RowCount; row++)
            {
                if (gridState[row, action] == 0.0f) return true;
            }
            return false;
        }

        public override (float reward, Tensor nextState, bool done) Step(int action, int steps)
        {
            TakeAction(action);
            var (reward, won) = EvaluateAction(action);
            return (reward, GetNormalizedState(), won || BoardFilled());
        }

        public override float TestTrainingProgress(Model agent, int testEpisodes)
        {
            int wins = 0, ties = 0;
            for (int e = 0; e < testEpisodes; e++)
            {
                Reset();
                var (won, tied) = PlayRandom(agent);
                if (won) wins++;
                else if (tied) ties++;
            }

            float winPercent = ((float)wins / testEpisodes) * 100.0f;
            float tiePercent = ((float)ties / testEpisodes) * 100.0f;
            NNNLog.WriteLine($"Win percentage vs randomly-acting opponent: {winPercent:F2}");
            NNNLog.WriteLine($"Tie percentage vs randomly-acting opponent: {tiePercent:F2}");
            NNNLog.WriteLine($"Win + tie percentage vs randomly-acting opponent: {(winPercent + tiePercent):F2}");
            return 2.0f * winPercent + tiePercent;
        }

        // Additional self-play interface API overrides

        public int GetAgentAction(Model agent, Tensor state = null)
        {
            state ??= GetNormalizedState();

            using var wrapped = Tensor.WrapBatch(state);
            using var predicted = agent.Predict(wrapped);
            return PickAgentAction(predicted, state);
        }

        // Additional environment-specific functionality

        (float reward, bool won) EvaluateAction(int action)
        {
            bool won = Won(action);
            bool tied = !won && BoardFilled();

            return (won ? WinReward : (tied ? TieReward : 0.0f), won);
        }

        public void TakeAction(int action)
        {
            for (int row = RowCount - 1; row >= 0; row--)
            {
                if (State[row, action] == 0.0f)
                {
                    State[row, action] = RedTurn ? 1.0f : -1.0f;
                    break;
                }
            }
            RedTurn = !RedTurn;
            AgentTurn = !AgentTurn;
        }

        public bool Won(int action)
        {
            // Find row which was filled - action = column
            int row = 0;
            float searchVal = 0.0f;
            for (; row < RowCount; row++)
            {
                if (State[row, action] != 0.0f)
                {
                    searchVal = State[row, action];
                    break;
                }
            }

            // Check whether 4 or more positions in horizontal, diagonal, or vertical direction are filled with same value
            int filled;
            for (int dir = 0; dir < 4; dir++)
            {
                filled = 1;
                var (rowStep, colStep) = dir switch
                {
                    0 => (0, -1), // horizontal - starting towards left
                    1 => (-1, -1), // diagonal - starting towards top left
                    2 => (-1, 0), // vertical - starting towards top
                    3 => (-1, 1), // diagonal - starting towards top right
                    _ => throw new Exception("Invalid Direction")
                };

                // Count positions in given direction
                int checkRow = row + rowStep, checkCol = action + colStep;
                filled += CountRowConnections(searchVal, checkRow, rowStep, checkCol, colStep);
                if (filled >= 4) return true;

                // Count positions in opposite direction
                rowStep *= -1;
                colStep *= -1;
                checkRow = row + rowStep;
                checkCol = action + colStep;
                filled += CountRowConnections(searchVal, checkRow, rowStep, checkCol, colStep);
                if (filled >= 4) return true;
            }

            return false;
        }

        int CountRowConnections(float searchVal, int checkRow, int rowStep, int checkCol, int colStep)
        {
            int connections = 0;
            while (0 <= checkRow && checkRow < RowCount && 0 <= checkCol && checkCol < ColumnCount)
            {
                if (State[checkRow, checkCol] == searchVal)
                {
                    if (++connections >= 3) return connections;
                }
                else break;
                checkRow += rowStep;
                checkCol += colStep;
            }

            return connections;
        }

        public bool BoardFilled()
        {
            foreach (var pos in State.Data)
            {
                if (pos == 0.0f) return false;
            }
            return true;
        }

        (bool won, bool tied) PlayRandom(Model agent)
        {
            bool? won = null;
            bool tied = false;

            while (won == null && !tied)
            {
                bool agentActing = AgentTurn;
                int action = AgentTurn ? GetAgentAction(agent) : PickRandomAction();
                TakeAction(action);
                if (Won(action)) won = agentActing;
                tied = won == null && BoardFilled();
            }

            return (won != null ? won.Value : false, tied);
        }

        public Tensor GetBoard() => State;
    }
}
