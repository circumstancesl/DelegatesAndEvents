using System;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace DelegatesAndEvents
{
    // исключение для несовместимых матриц
    public class IncompatiblematrixException : Exception
    {
        public IncompatiblematrixException(string message) : base(message) { }
    }

    // исключение для невозможности вычисления обратной матрицы
    public class NonInvertiblematrixException : Exception
    {
        public NonInvertiblematrixException(string message) : base(message) { }
    }

    public class SquareMatrix : ICloneable, IComparable<SquareMatrix>
    {
        public int size;
        public int[,] matrix;

        public SquareMatrix(int size)
        {
            this.size = size;
            this.matrix = new int[size, size];
        }

        public SquareMatrix(int size, int MinValue, int MaxValue) : this(size) 
        {
            Random rand = new Random();
            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < size; ++columnIndex)
                {
                    this.matrix[rowIndex, columnIndex] = rand.Next(MinValue, MaxValue);
                }
            }
        }

        public void Printmatrix() 
        {
            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < size; ++columnIndex)
                {
                    Console.Write(matrix[rowIndex, columnIndex] + " ");
                }
                Console.WriteLine();
            }
        }

        public SquareMatrix(SquareMatrix other)
        {
            size = other.size;
            matrix = new int[size, size];
            Array.Copy(other.matrix, matrix, other.matrix.Length);
        }

        public object Clone()
        {
            return new SquareMatrix(this);
        }

        public static SquareMatrix operator +(SquareMatrix matrix1, SquareMatrix matrix2) 
        {
            SquareMatrix result = (SquareMatrix)matrix1.Clone();

            for (int rowIndex = 0; rowIndex < matrix1.size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < matrix1.size; ++columnIndex)
                {
                    result.matrix[rowIndex, columnIndex] = matrix1.matrix[rowIndex, columnIndex] + matrix2.matrix[rowIndex, columnIndex];
                }
            }

            return result;
        }
        public static SquareMatrix operator *(SquareMatrix matrix1, SquareMatrix matrix2) 
        {
            SquareMatrix result = (SquareMatrix)matrix1.Clone();

            for (int rowIndex = 0; rowIndex < matrix1.size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < matrix1.size; ++columnIndex)
                {
                    for (int elementIndex = 0; elementIndex < matrix1.size; elementIndex++)
                    {
                        result.matrix[rowIndex, columnIndex] += matrix1.matrix[rowIndex, elementIndex] * matrix2.matrix[elementIndex, columnIndex];
                    }
                }
            }

            return result;
        }

        public static bool operator ==(SquareMatrix matrix1, SquareMatrix matrix2)
        {
            if (ReferenceEquals(matrix1, matrix2))
            {
                return true;
            }

            if ((object)matrix1 == null || (object)matrix2 == null)
            {
                return false;
            }

            return matrix1.Equals(matrix2);
        }

        public static bool operator !=(SquareMatrix matrix1, SquareMatrix matrix2)
        {
            return !(matrix1 == matrix2);
        }

        public int CompareTo(SquareMatrix other)
        {
            if (other == null)
            {
                return 1;
            }

            if (size != other.size)
            {
                return size.CompareTo(other.size);
            }

            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < size; ++columnIndex)
                {
                    int Comparison = matrix[rowIndex, columnIndex].CompareTo(other.matrix[rowIndex, columnIndex]);
                    if (Comparison != 0)
                    {
                        return Comparison;
                    }
                }
            }

            return 0;
        }

        public static bool operator <(SquareMatrix matrix1, SquareMatrix matrix2)
        {
            return matrix1.CompareTo(matrix2) < 0;
        }

        public static bool operator >(SquareMatrix matrix1, SquareMatrix matrix2)
        {
            return matrix1.CompareTo(matrix2) > 0;
        }

        public static bool operator <=(SquareMatrix matrix1, SquareMatrix matrix2)
        {
            return matrix1.CompareTo(matrix2) <= 0;
        }

        public static bool operator >=(SquareMatrix matrix1, SquareMatrix matrix2)
        {
            return matrix1.CompareTo(matrix2) >= 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < size; ++columnIndex)
                {
                    sb.Append(matrix[rowIndex, columnIndex]);
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
            Result.matrix = Array;
            return Result;
        }

        public SquareMatrix Inverse()
        {
            double[,] augmentedmatrix = new double[size, 2 * size];
            SquareMatrix inversematrix = new SquareMatrix(size);

            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < size; ++columnIndex)
                {
                    augmentedmatrix[rowIndex, columnIndex] = matrix[rowIndex, columnIndex];
                }
                augmentedmatrix[rowIndex, rowIndex + size] = 1; 
            }

            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                if (augmentedmatrix[rowIndex, rowIndex] == 0)
                {
                    for (int columnIndex = rowIndex + 1; columnIndex < size; ++columnIndex)
                    {
                        if (augmentedmatrix[columnIndex, rowIndex] != 0)
                        {
                            SwapRows(augmentedmatrix, rowIndex, columnIndex);
                            break;
                        }
                    }
                }

                if (augmentedmatrix[rowIndex, rowIndex] == 0)
                {
                    throw new NonInvertiblematrixException("Матрица необратима.");
                }

                double factor = augmentedmatrix[rowIndex, rowIndex];
                for (int columnIndex = rowIndex; columnIndex < 2 * size; ++columnIndex)
                {
                    augmentedmatrix[rowIndex, columnIndex] /= factor;
                }

                for (int columnIndex = rowIndex + 1; columnIndex < size; ++columnIndex)
                {
                    double factor2 = augmentedmatrix[columnIndex, rowIndex];
                    for (int elementIndex = rowIndex; elementIndex < 2 * size; ++elementIndex)
                    {
                        augmentedmatrix[columnIndex, elementIndex] -= factor2 * augmentedmatrix[rowIndex, elementIndex];
                    }
                }
            }

            for (int rowIndex = size - 1; rowIndex >= 0; --rowIndex)
            {
                for (int columnIndex = rowIndex - 1; columnIndex >= 0; --columnIndex)
                {
                    double factor3 = augmentedmatrix[columnIndex, rowIndex];
                    for (int elementIndex = 0; elementIndex < 2 * size; ++elementIndex)
                    {
                        augmentedmatrix[columnIndex, elementIndex] -= factor3 * augmentedmatrix[rowIndex, elementIndex];
                    }
                }
            }

            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < size; ++columnIndex)
                {
                    inversematrix.matrix[rowIndex, columnIndex] = (int)augmentedmatrix[rowIndex, columnIndex + size];
                }
            }

            return inversematrix;
        }

        private void SwapRows(double[,] matrix, int Row1, int Row2)
        {
            for (int elementIndex = 0; elementIndex < size; ++elementIndex)
            {
                double Temp = matrix[Row1, elementIndex];
                matrix[Row1, elementIndex] = matrix[Row2, elementIndex];
                matrix[Row2, elementIndex] = Temp;
            }
        }

        public int Determinant()
        {
            return CalculateDeterminant(matrix, size);
        }

        private static int CalculateDeterminant(int[,] matrix, int size)
        {
            if (size == 1)
            {
                return matrix[0, 0];
            }

            int determinant = 0;
            int sign = 1;

            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                int[,] Submatrix = new int[size - 1, size - 1];
                for (int columnIndex = 1; columnIndex < size; ++columnIndex)
                {
                    int columnIndexMatrix = 0;
                    for (int elementIndex = 0; elementIndex < size; ++elementIndex)
                    {
                        if (elementIndex == rowIndex) continue;
                        Submatrix[columnIndex - 1, columnIndexMatrix] = matrix[columnIndex, elementIndex];
                        ++columnIndexMatrix;
                    }
                }

                determinant += sign * matrix[0, rowIndex] * CalculateDeterminant(Submatrix, size - 1);
                sign = -sign;
            }

            return determinant;
        }

        public static bool operator true(SquareMatrix matrix)
        {
            return !matrix.Equals(matrix);
        }

        public static bool operator false(SquareMatrix matrix)
        {
            return matrix.Equals(matrix);
        }

        public override bool Equals(object Obj)
        {
            if (Obj == null || GetType() != Obj.GetType())
            {
                return false;
            }

            SquareMatrix other = (SquareMatrix)Obj;

            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < size; ++columnIndex)
                {
                    if (matrix[rowIndex, columnIndex] != other.matrix[rowIndex, columnIndex])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17; 
            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < size; ++columnIndex)
                {
                    hash = hash * 31 + matrix[rowIndex, columnIndex]; 
                }
            }
            return hash;
        }
    }

    public static class ExtendingMetods
    {
        public static SquareMatrix Transpose(this SquareMatrix matrixForTransposition)
        {
            SquareMatrix Result = new SquareMatrix(matrixForTransposition.size);

            for (int rowIndex = 0; rowIndex < matrixForTransposition.size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < matrixForTransposition.size; ++columnIndex)
                {
                    Result.matrix[rowIndex, columnIndex] = matrixForTransposition.matrix[columnIndex, rowIndex];
                }
            }

            return Result;
        }

        public static int Trace(this SquareMatrix Tracematrix)
        {
            int trace = 0;

            for (int rowIndex = 0; rowIndex < Tracematrix.size; ++rowIndex)
            {
                trace += Tracematrix.matrix[rowIndex, rowIndex];
            }

            return trace;
        }
    }

    public delegate SquareMatrix DiagonalizematrixDelegate(SquareMatrix matrix);

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
        public SquareMatrix matrix1;
        public SquareMatrix matrix2;

        public BaseHandler()
        {
            Next = null;
        }

        public virtual void Handle(IOperation operation, SquareMatrix matrixOne, SquareMatrix matrixTwo)
        {
            matrix1 = matrixOne;
            matrix2 = matrixTwo;

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
                Console.WriteLine(matrix1);
                Console.WriteLine("Матрица 2:");
                Console.WriteLine(matrix2);
                Console.WriteLine("Сумма матриц:");
                Console.WriteLine(matrix1 + matrix2);
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
                Console.WriteLine(matrix1);
                Console.WriteLine("Матрица 2:");
                Console.WriteLine(matrix2);
                Console.WriteLine("Произведение матриц:");
                Console.WriteLine(matrix1 * matrix2);
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
                Console.WriteLine(matrix1);
                Console.WriteLine("Матрица 2:");
                Console.WriteLine(matrix2);
                Console.WriteLine("Матрицы равны: " + (matrix1 == matrix2) + "\n");
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
                Console.WriteLine(matrix1);
                Console.WriteLine("Определитель матрицы: " + matrix1.Determinant() + "\n");
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
                Console.WriteLine(matrix1);
                Console.WriteLine("Обратная матрица:");
                Console.WriteLine(matrix1.Inverse());
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
                Console.WriteLine(matrix1);
                Console.WriteLine("Транспонированная матрица:");
                Console.WriteLine(matrix1.Transpose());
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
                Console.WriteLine(matrix1);
                Console.WriteLine("След матрицы: " + matrix1.Trace() + "\n");
            };
        }
    }

    class DiagonalHandler : BaseHandler
    {
        public DiagonalHandler()
        {
            Operation = new Diagonal();
            Next = null;
            DiagonalizematrixDelegate diagonalizematrixDelegate = delegate (SquareMatrix matrixForDiagonalize)
            {
                for (int rowIndex = 0; rowIndex < matrixForDiagonalize.size; ++rowIndex)
                {
                    for (int columnIndex = 0; columnIndex < matrixForDiagonalize.size; ++columnIndex)
                    {
                        if (rowIndex != columnIndex)
                        {
                            matrixForDiagonalize.matrix[rowIndex, columnIndex] = 0;
                        }
                    }
                }
                return matrixForDiagonalize;
            };

            TargetFunction = delegate ()
            {
                Console.WriteLine("\nМатрица:");
                Console.WriteLine(matrix1);
                Console.WriteLine("Диагонализированная матрица:");
                Console.WriteLine(diagonalizematrixDelegate(matrix1));
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
            int sizeMatrix = rand.Next(1, 5); 
            int minValueElementmatrix = 1; 
            int maxValueElementmatrix = 10; 


            SquareMatrix matrix1 = new SquareMatrix(sizeMatrix, minValueElementmatrix, maxValueElementmatrix);
            SquareMatrix matrix2 = new SquareMatrix(sizeMatrix, minValueElementmatrix, maxValueElementmatrix);

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

            var operations = new Dictionary<string, IOperation>
            {
                { "1", new Add() },
                { "2", new Multiplication() },
                { "3", new Determinant() },
                { "4", new Inverse() },
                { "5", new Transposition() },
                { "6", new Trace() },
                { "7", new Diagonal() },
            };

            if (operations.TryGetValue(userChoice, out var operation))
            {
                ChainApplication chainApplication = new ChainApplication();
                chainApplication.Run(operation, matrix1, matrix2);
            }
            else
            {
                Console.WriteLine("Неверный выбор операции");
            }

            Console.ReadKey();
            Console.WriteLine();
        }
    }
}
