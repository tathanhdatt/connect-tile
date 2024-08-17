using System.Text;

namespace MatchingPairAlgorithm
{
    public class Graph
    {
        private int[,] Data { get; set; }

        public int NumberRow { get; private set; } = 0;

        public int NumberCol { get; private set; } = 0;

        public Graph(int numberRow, int numberCol) {
            Data = new int[numberRow, numberCol];
            this.NumberRow = numberRow;
            this.NumberCol = numberCol;
        }

        public Graph(int[,] data)
        {
            this.Data = data;
            this.NumberRow = data.GetLength(0);
            this.NumberCol = data.GetLength(1);
        }

        public void SetValue(int row, int col, int value) {
            Data[row, col] = value;
        }

        public int GetValue(int row, int col) {
            return Data[row, col];
        }

        public override string ToString() {
            StringBuilder stringBuilder = new();
            for (var i = 0; i < Data.GetLength(0); i++) {
                for (var j = 0; j < Data.GetLength(1); j++) {
                    stringBuilder.Append(Data[i, j]);
                    stringBuilder.Append(' ');
                }

                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }
    }
}

