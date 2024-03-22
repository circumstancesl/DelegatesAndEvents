using System;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace DelegatesAndEvents
{
    // исключение для несовместимых матриц
    public class IncompatibleMatrixException : Exception
    {
        public IncompatibleMatrixException(string message) : base(message) { }
    }

    // исключение для невозможности вычисления обратной матрицы
    public class NonInvertibleMatrixException : Exception
    {
        public NonInvertibleMatrixException(string message) : base(message) { }
    }

    public class SquareMatrix : ICloneable, IComparable<SquareMatrix>
    {
        public int Size;
        public int[,] Matrix;

        public SquareMatrix(int Size)
        {
            this.Size = Size;
            this.Matrix = new int[Size, Size];
        }

        public SquareMatrix(int Size, int MinValue, int MaxValue) : this(Size) 
        {
            Random rand = new Random();
            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
                {
                    this.Matrix[RowIndex, ColumnIndex] = rand.Next(MinValue, MaxValue);
                }
            }
        }

        public void PrintMatrix() 
        {
            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
                {
                    Console.Write(Matrix[RowIndex, ColumnIndex] + " ");
                }
                Console.WriteLine();
            }
        }

        public SquareMatrix(SquareMatrix other)
        {
            Size = other.Size;
            Matrix = new int[Size, Size];
            Array.Copy(other.Matrix, Matrix, other.Matrix.Length);
        }

        public object Clone()
        {
            return new SquareMatrix(this);
        }

        public static SquareMatrix operator +(SquareMatrix Matrix1, SquareMatrix Matrix2) 
        {
            SquareMatrix result = (SquareMatrix)Matrix1.Clone();

            for (int RowIndex = 0; RowIndex < Matrix1.Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Matrix1.Size; ++ColumnIndex)
                {
                    result.Matrix[RowIndex, ColumnIndex] = Matrix1.Matrix[RowIndex, ColumnIndex] + Matrix2.Matrix[RowIndex, ColumnIndex];
                }
            }

            return result;
        }
        public static SquareMatrix operator *(SquareMatrix Matrix1, SquareMatrix Matrix2) 
        {
            SquareMatrix result = (SquareMatrix)Matrix1.Clone();

            for (int RowIndex = 0; RowIndex < Matrix1.Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Matrix1.Size; ++ColumnIndex)
                {
                    for (int ElementIndex = 0; ElementIndex < Matrix1.Size; ElementIndex++)
                    {
                        result.Matrix[RowIndex, ColumnIndex] += Matrix1.Matrix[RowIndex, ElementIndex] * Matrix2.Matrix[ElementIndex, ColumnIndex];
                    }
                }
            }

            return result;
        }

        public static bool operator ==(SquareMatrix Matrix1, SquareMatrix Matrix2)
        {
            if (ReferenceEquals(Matrix1, Matrix2))
            {
                return true;
            }

            if ((object)Matrix1 == null || (object)Matrix2 == null)
            {
                return false;
            }

            return Matrix1.Equals(Matrix2);
        }

        public static bool operator !=(SquareMatrix Matrix1, SquareMatrix Matrix2)
        {
            return !(Matrix1 == Matrix2);
        }

        public int CompareTo(SquareMatrix other)
        {
            if (other == null)
            {
                return 1;
            }

            if (Size != other.Size)
            {
                return Size.CompareTo(other.Size);
            }

            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
                {
                    int Comparison = Matrix[RowIndex, ColumnIndex].CompareTo(other.Matrix[RowIndex, ColumnIndex]);
                    if (Comparison != 0)
                    {
                        return Comparison;
                    }
                }
            }

            return 0;
        }

        public static bool operator <(SquareMatrix Matrix1, SquareMatrix Matrix2)
        {
            return Matrix1.CompareTo(Matrix2) < 0;
        }

        public static bool operator >(SquareMatrix Matrix1, SquareMatrix Matrix2)
        {
            return Matrix1.CompareTo(Matrix2) > 0;
        }

        public static bool operator <=(SquareMatrix Matrix1, SquareMatrix Matrix2)
        {
            return Matrix1.CompareTo(Matrix2) <= 0;
        }

        public static bool operator >=(SquareMatrix Matrix1, SquareMatrix Matrix2)
        {
            return Matrix1.CompareTo(Matrix2) >= 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
                {
                    sb.Append(Matrix[RowIndex, ColumnIndex]);
                    sb.Append(" ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static explicit operator SquareMatrix(int[,] Array) 
        {
            int size = Array.GetLength(0);
            SquareMatrix Result = new SquareMatrix(size);
            Result.Matrix = Array;
            return Result;
        }

        public SquareMatrix Inverse()
        {
            double[,] AugmentedMatrix = new double[Size, 2 * Size];
            SquareMatrix InverseMatrix = new SquareMatrix(Size);

            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
                {
                    AugmentedMatrix[RowIndex, ColumnIndex] = Matrix[RowIndex, ColumnIndex];
                }
                AugmentedMatrix[RowIndex, RowIndex + Size] = 1; 
            }

            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                if (AugmentedMatrix[RowIndex, RowIndex] == 0)
                {
                    for (int ColumnIndex = RowIndex + 1; ColumnIndex < Size; ++ColumnIndex)
                    {
                        if (AugmentedMatrix[ColumnIndex, RowIndex] != 0)
                        {
                            SwapRows(AugmentedMatrix, RowIndex, ColumnIndex);
                            break;
                        }
                    }
                }

                if (AugmentedMatrix[RowIndex, RowIndex] == 0)
                {
                    throw new NonInvertibleMatrixException("Матрица необратима.");
                }

                double Factor = AugmentedMatrix[RowIndex, RowIndex];
                for (int ColumnIndex = RowIndex; ColumnIndex < 2 * Size; ++ColumnIndex)
                {
                    AugmentedMatrix[RowIndex, ColumnIndex] /= Factor;
                }

                for (int ColumnIndex = RowIndex + 1; ColumnIndex < Size; ++ColumnIndex)
                {
                    double Factor2 = AugmentedMatrix[ColumnIndex, RowIndex];
                    for (int ElementIndex = RowIndex; ElementIndex < 2 * Size; ++ElementIndex)
                    {
                        AugmentedMatrix[ColumnIndex, ElementIndex] -= Factor2 * AugmentedMatrix[RowIndex, ElementIndex];
                    }
                }
            }

            for (int RowIndex = Size - 1; RowIndex >= 0; --RowIndex)
            {
                for (int ColumnIndex = RowIndex - 1; ColumnIndex >= 0; --ColumnIndex)
                {
                    double Factor3 = AugmentedMatrix[ColumnIndex, RowIndex];
                    for (int ElementIndex = 0; ElementIndex < 2 * Size; ++ElementIndex)
                    {
                        AugmentedMatrix[ColumnIndex, ElementIndex] -= Factor3 * AugmentedMatrix[RowIndex, ElementIndex];
                    }
                }
            }

            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
                {
                    InverseMatrix.Matrix[RowIndex, ColumnIndex] = (int)AugmentedMatrix[RowIndex, ColumnIndex + Size];
                }
            }

            return InverseMatrix;
        }

        private void SwapRows(double[,] Matrix, int Row1, int Row2)
        {
            for (int ElementIndex = 0; ElementIndex < Size; ++ElementIndex)
            {
                double Temp = Matrix[Row1, ElementIndex];
                Matrix[Row1, ElementIndex] = Matrix[Row2, ElementIndex];
                Matrix[Row2, ElementIndex] = Temp;
            }
        }

        public int Determinant()
        {
            return CalculateDeterminant(Matrix, Size);
        }

        private static int CalculateDeterminant(int[,] Matrix, int Size)
        {
            if (Size == 1)
            {
                return Matrix[0, 0];
            }

            int Determinant = 0;
            int Sign = 1;

            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                int[,] SubMatrix = new int[Size - 1, Size - 1];
                for (int ColumnIndex = 1; ColumnIndex < Size; ++ColumnIndex)
                {
                    int columnIndex = 0;
                    for (int ElementIndex = 0; ElementIndex < Size; ++ElementIndex)
                    {
                        if (ElementIndex == RowIndex) continue;
                        SubMatrix[ColumnIndex - 1, columnIndex] = Matrix[ColumnIndex, ElementIndex];
                        columnIndex++;
                    }
                }

                Determinant += Sign * Matrix[0, RowIndex] * CalculateDeterminant(SubMatrix, Size - 1);
                Sign = -Sign;
            }

            return Determinant;
        }

        public static bool operator true(SquareMatrix Matrix)
        {
            return !Matrix.Equals(Matrix);
        }

        public static bool operator false(SquareMatrix Matrix)
        {
            return Matrix.Equals(Matrix);
        }

        public override bool Equals(object Obj)
        {
            if (Obj == null || GetType() != Obj.GetType())
            {
                return false;
            }

            SquareMatrix other = (SquareMatrix)Obj;

            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
                {
                    if (Matrix[RowIndex, ColumnIndex] != other.Matrix[RowIndex, ColumnIndex])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int Hash = 17; 
            for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
                {
                    Hash = Hash * 31 + Matrix[RowIndex, ColumnIndex]; 
                }
            }
            return Hash;
        }
    }

    public static class ExtendingMetods
    {
        public static SquareMatrix Transpose(this SquareMatrix MatrixForTransposition)
        {
            SquareMatrix Result = new SquareMatrix(MatrixForTransposition.Size);

            for (int RowIndex = 0; RowIndex < MatrixForTransposition.Size; ++RowIndex)
            {
                for (int ColumnIndex = 0; ColumnIndex < MatrixForTransposition.Size; ++ColumnIndex)
                {
                    Result.Matrix[RowIndex, ColumnIndex] = MatrixForTransposition.Matrix[ColumnIndex, RowIndex];
                }
            }

            return Result;
        }

        public static int Trace(this SquareMatrix TraceMatrix)
        {
            int trace = 0;

            for (int RowIndex = 0; RowIndex < TraceMatrix.Size; ++RowIndex)
            {
                trace += TraceMatrix.Matrix[RowIndex, RowIndex];
            }

            return trace;
        }
    }

    public delegate SquareMatrix DiagonalizeMatrixDelegate(SquareMatrix matrix);

    public abstract class IOperation
    {
        public string OperationType;
    }

    class Add : IOperation
    {
        public Add()
        {
            OperationType = "1";
        }
    }

    class Multiplication : IOperation
    {
        public Multiplication()
        {
            OperationType = "2";
        }
    }

    class Equal : IOperation
    {
        public Equal()
        {
            OperationType = "3";
        }
    }

    class Determinant : IOperation
    {
        public Determinant()
        {
            OperationType = "4";
        }
    }

    class Inverse : IOperation
    {
        public Inverse()
        {
            OperationType = "5";
        }
    }

    class Transposition : IOperation
    {
        public Transposition()
        {
            OperationType = "6";
        }
    }

    class Trace : IOperation
    {
        public Trace()
        {
            OperationType = "7";
        }
    }

    class Diagonal : IOperation
    {
        public Diagonal()
        {
            OperationType = "8";
        }
    }

    public abstract class BaseHandler
    {
        protected BaseHandler Next;
        protected IOperation Operation;
        protected delegate void RunFunction();
        protected RunFunction TargetFunction;
        public SquareMatrix Matrix1;
        public SquareMatrix Matrix2;

        public BaseHandler()
        {
            Next = null;
        }

        public virtual void Handle(IOperation operation, SquareMatrix matrixOne, SquareMatrix matrixTwo)
        {
            Matrix1 = matrixOne;
            Matrix2 = matrixTwo;

            if (Operation.OperationType == operation.OperationType)
            {
                Console.WriteLine("\nОперация успешно обработана");
                TargetFunction();
            }
            else
            {
                Console.WriteLine("Не могу обработать, отправляю следующему обработчику...");

                if (Next != null)
                {
                    Next.Handle(operation, matrixOne, matrixTwo);
                }
                else
                {
                    Console.WriteLine("Неизвестная операция, не могу обработать.");
                }
            }
        }

        protected void SetNextHandler(BaseHandler newHandler)
        {
            Next = newHandler;
        }
    }

    class AddHandler : BaseHandler
    {
        public AddHandler()
        {
            Operation = new Add();
            Next = new MultiplicationHandler();
            TargetFunction = delegate ()
            {
                Console.WriteLine("\nМатрица 1:");
                Console.WriteLine(Matrix1);
                Console.WriteLine("Матрица 2:");
                Console.WriteLine(Matrix2);
                Console.WriteLine("Сумма матриц:");
                Console.WriteLine(Matrix1 + Matrix2);
            };
        }
    }

    class MultiplicationHandler : BaseHandler
    {
        public MultiplicationHandler()
        {
            Operation = new Multiplication();
            Next = new EqualHandler();
            TargetFunction = delegate ()
            {
                Console.WriteLine("\nМатрица 1:");
                Console.WriteLine(Matrix1);
                Console.WriteLine("Матрица 2:");
                Console.WriteLine(Matrix2);
                Console.WriteLine("Произведение матриц:");
                Console.WriteLine(Matrix1 * Matrix2);
            };
        }
    }

    class EqualHandler : BaseHandler
    {
        public EqualHandler()
        {
            Operation = new Equal();
            Next = new DeterminantHandler();
            TargetFunction = delegate ()
            {
                Console.WriteLine("\nМатрица 1:");
                Console.WriteLine(Matrix1);
                Console.WriteLine("Матрица 2:");
                Console.WriteLine(Matrix2);
                Console.WriteLine("Матрицы равны: " + (Matrix1 == Matrix2) + "\n");
            };
        }
    }

    class DeterminantHandler : BaseHandler
    {
        public DeterminantHandler()
        {
            Operation = new Determinant();
            Next = new InverseHandler();
            TargetFunction = delegate ()
            {
                Console.WriteLine("\nМатрица:");
                Console.WriteLine(Matrix1);
                Console.WriteLine("Определитель матрицы: " + Matrix1.Determinant() + "\n");
            };
        }
    }

    class InverseHandler : BaseHandler
    {
        public InverseHandler()
        {
            Operation = new Inverse();
            Next = new TranspositionHandler();
            TargetFunction = delegate ()
            {
                Console.WriteLine("\nМатрица:");
                Console.WriteLine(Matrix1);
                Console.WriteLine("Обратная матрица:");
                Console.WriteLine(Matrix1.Inverse());
            };
        }
    }

    class TranspositionHandler : BaseHandler
    {
        public TranspositionHandler()
        {
            Operation = new Transposition();
            Next = new TraceHandler();
            TargetFunction = delegate ()
            {
                Console.WriteLine("\nМатрица:");
                Console.WriteLine(Matrix1);
                Console.WriteLine("Транспонированная матрица:");
                Console.WriteLine(Matrix1.Transpose());
            };
        }
    }

    class TraceHandler : BaseHandler
    {
        public TraceHandler()
        {
            Operation = new Trace();
            Next = new DiagonalHandler();
            TargetFunction = delegate ()
            {
                Console.WriteLine("\nМатрица:");
                Console.WriteLine(Matrix1);
                Console.WriteLine("След матрицы: " + Matrix1.Trace() + "\n");
            };
        }
    }

    class DiagonalHandler : BaseHandler
    {
        public DiagonalHandler()
        {
            Operation = new Diagonal();
            Next = null;
            DiagonalizeMatrixDelegate diagonalizeMatrixDelegate = delegate (SquareMatrix matrixForDiagonalize)
            {
                for (int rowIndex = 0; rowIndex < matrixForDiagonalize.Size; ++rowIndex)
                {
                    for (int columnIndex = 0; columnIndex < matrixForDiagonalize.Size; ++columnIndex)
                    {
                        if (rowIndex != columnIndex)
                        {
                            matrixForDiagonalize.Matrix[rowIndex, columnIndex] = 0;
                        }
                    }
                }
                return matrixForDiagonalize;
            };

            TargetFunction = delegate ()
            {
                Console.WriteLine("\nМатрица:");
                Console.WriteLine(Matrix1);
                Console.WriteLine("Диагонализированная матрица:");
                Console.WriteLine(diagonalizeMatrixDelegate(Matrix1));
            };
        }
    }

    public class ChainApplication
    {
        private BaseHandler operationHandler;

        public ChainApplication()
        {
            operationHandler = new AddHandler();
        }

        public void Run(IOperation operation, SquareMatrix matrix1, SquareMatrix matrix2)
        {
            operationHandler.Handle(operation, matrix1, matrix2);
        }
    }

    class Program
    {
        static void Main()
        {
            Random rand = new Random();
            int SizeMatrix = rand.Next(1, 5); 
            int MinValueElementMatrix = 1; 
            int MaxValueElementMatrix = 10; 


            SquareMatrix Matrix1 = new SquareMatrix(SizeMatrix, MinValueElementMatrix, MaxValueElementMatrix);
            SquareMatrix Matrix2 = new SquareMatrix(SizeMatrix, MinValueElementMatrix, MaxValueElementMatrix);

            Console.WriteLine("Какаю операцию вы хотите выполнить?\n" +
                              "[1] Сложить две случайные матрицы\n" +
                              "[2] Умножить две случайные матрицы\n" +
                              "[3] Посчитать определитель случайной матрицы\n" +
                              "[4] Найти матрицу, обратную случайной матрице\n" +
                              "[5] Транспонировать случайную матрицу\n" +
                              "[6] Найти след случайной матрицы\n" +
                              "[7] Привести матрицу к диагональному виду\n");
            Console.Write("Ваш выбор (цифра): ");
            string userChoice = Console.ReadLine();

            switch (userChoice)
            {
                case "1":
                    ChainApplication chainApplication1 = new ChainApplication();
                    chainApplication1.Run(new Add(), Matrix1, Matrix2);
                    break;
                case "2":
                    ChainApplication chainApplication2 = new ChainApplication();
                    chainApplication2.Run(new Multiplication(), Matrix1, Matrix2);
                    break;
                case "3":
                    ChainApplication chainApplication3 = new ChainApplication();
                    chainApplication3.Run(new Determinant(), Matrix1, Matrix2);
                    break;
                case "4":
                    ChainApplication chainApplication4 = new ChainApplication();
                    chainApplication4.Run(new Inverse(), Matrix1, Matrix2);
                    break;
                case "5":
                    ChainApplication chainApplication5 = new ChainApplication();
                    chainApplication5.Run(new Transposition(), Matrix1, Matrix2);
                    break;
                case "6":
                    ChainApplication chainApplication6 = new ChainApplication();
                    chainApplication6.Run(new Trace(), Matrix1, Matrix2);
                    break;
                case "7":
                    ChainApplication chainApplication7 = new ChainApplication();
                    chainApplication7.Run(new Diagonal(), Matrix1, Matrix2);
                    break;
            }

            Console.ReadKey();
            Console.WriteLine();
        }
    }
}
