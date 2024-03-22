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

        public SquareMatrix(int size)
        {
            this.Size = size;
            this.Matrix = new int[size, size];
        }

        public SquareMatrix(int size, int minValue, int maxValue) : this(size) 
        {
            Random rand = new Random();
            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < size; ++columnIndex)
                {
                    this.Matrix[rowIndex, columnIndex] = rand.Next(minValue, maxValue);
                }
            }
        }

        public void Printmatrix() 
        {
            for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
                {
                    Console.Write(Matrix[rowIndex, columnIndex] + " ");
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

        public static SquareMatrix operator +(SquareMatrix matrix1, SquareMatrix matrix2) 
        {
            SquareMatrix result = (SquareMatrix)matrix1.Clone();

            for (int rowIndex = 0; rowIndex < matrix1.Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < matrix1.Size; ++columnIndex)
                {
                    result.Matrix[rowIndex, columnIndex] = matrix1.Matrix[rowIndex, columnIndex] + matrix2.Matrix[rowIndex, columnIndex];
                }
            }

            return result;
        }
        public static SquareMatrix operator *(SquareMatrix matrix1, SquareMatrix matrix2) 
        {
            SquareMatrix result = (SquareMatrix)matrix1.Clone();

            for (int rowIndex = 0; rowIndex < matrix1.Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < matrix1.Size; ++columnIndex)
                {
                    for (int elementIndex = 0; elementIndex < matrix1.Size; elementIndex++)
                    {
                        result.Matrix[rowIndex, columnIndex] += matrix1.Matrix[rowIndex, elementIndex] * matrix2.Matrix[elementIndex, columnIndex];
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

            if (Size != other.Size)
            {
                return Size.CompareTo(other.Size);
            }

            for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
                {
                    int comparison = Matrix[rowIndex, columnIndex].CompareTo(other.Matrix[rowIndex, columnIndex]);
                    if (comparison != 0)
                    {
                        return comparison;
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
            StringBuilder stringbuild = new StringBuilder();
            for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
                {
                    stringbuild.Append(Matrix[rowIndex, columnIndex]);
                    stringbuild.Append(" ");
                }
                stringbuild.AppendLine();
            }
            return stringbuild.ToString();
        }

        public static explicit operator SquareMatrix(int[,] Array) 
        {
            int size = Array.GetLength(0);
            SquareMatrix result = new SquareMatrix(size);
            result.Matrix = Array;
            return result;
        }

        public SquareMatrix Inverse()
        {
            double[,] augmentedMatrix = new double[Size, 2 * Size];
            SquareMatrix inverseMatrix = new SquareMatrix(Size);

            for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
                {
                    augmentedMatrix[rowIndex, columnIndex] = Matrix[rowIndex, columnIndex];
                }
                augmentedMatrix[rowIndex, rowIndex + Size] = 1; 
            }

            for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
            {
                if (augmentedMatrix[rowIndex, rowIndex] == 0)
                {
                    for (int columnIndex = rowIndex + 1; columnIndex < Size; ++columnIndex)
                    {
                        if (augmentedMatrix[columnIndex, rowIndex] != 0)
                        {
                            SwapRows(augmentedMatrix, rowIndex, columnIndex);
                            break;
                        }
                    }
                }

                if (augmentedMatrix[rowIndex, rowIndex] == 0)
                {
                    throw new NonInvertibleMatrixException("Матрица необратима.");
                }

                double factor = augmentedMatrix[rowIndex, rowIndex];
                for (int columnIndex = rowIndex; columnIndex < 2 * Size; ++columnIndex)
                {
                    augmentedMatrix[rowIndex, columnIndex] /= factor;
                }

                for (int columnIndex = rowIndex + 1; columnIndex < Size; ++columnIndex)
                {
                    double factor2 = augmentedMatrix[columnIndex, rowIndex];
                    for (int elementIndex = rowIndex; elementIndex < 2 * Size; ++elementIndex)
                    {
                        augmentedMatrix[columnIndex, elementIndex] -= factor2 * augmentedMatrix[rowIndex, elementIndex];
                    }
                }
            }

            for (int rowIndex = Size - 1; rowIndex >= 0; --rowIndex)
            {
                for (int columnIndex = rowIndex - 1; columnIndex >= 0; --columnIndex)
                {
                    double factor3 = augmentedMatrix[columnIndex, rowIndex];
                    for (int elementIndex = 0; elementIndex < 2 * Size; ++elementIndex)
                    {
                        augmentedMatrix[columnIndex, elementIndex] -= factor3 * augmentedMatrix[rowIndex, elementIndex];
                    }
                }
            }

            for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
                {
                    inverseMatrix.Matrix[rowIndex, columnIndex] = (int)augmentedMatrix[rowIndex, columnIndex + Size];
                }
            }

            return inverseMatrix;
        }

        private void SwapRows(double[,] matrix, int row1, int row2)
        {
            for (int elementIndex = 0; elementIndex < Size; ++elementIndex)
            {
                double temp = Matrix[row1, elementIndex];
                matrix[row1, elementIndex] = matrix[row2, elementIndex];
                matrix[row2, elementIndex] = temp;
            }
        }

        public int Determinant()
        {
            return CalculateDeterminant(Matrix, Size);
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
                int[,] submatrix = new int[size - 1, size - 1];
                for (int columnIndex = 1; columnIndex < size; ++columnIndex)
                {
                    int columnIndexMatrix = 0;
                    for (int elementIndex = 0; elementIndex < size; ++elementIndex)
                    {
                        if (elementIndex == rowIndex) continue;
                        submatrix[columnIndex - 1, columnIndexMatrix] = matrix[columnIndex, elementIndex];
                        ++columnIndexMatrix;
                    }
                }

                determinant += sign * matrix[0, rowIndex] * CalculateDeterminant(submatrix, size - 1);
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SquareMatrix other = (SquareMatrix)obj;

            for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
                {
                    if (Matrix[rowIndex, columnIndex] != other.Matrix[rowIndex, columnIndex])
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
            for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
                {
                    hash = hash * 31 + Matrix[rowIndex, columnIndex]; 
                }
            }
            return hash;
        }
    }

    public static class ExtendingMetods
    {
        public static SquareMatrix Transpose(this SquareMatrix matrixForTransposition)
        {
            SquareMatrix result = new SquareMatrix(matrixForTransposition.Size);

            for (int rowIndex = 0; rowIndex < matrixForTransposition.Size; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < matrixForTransposition.Size; ++columnIndex)
                {
                    result.Matrix[rowIndex, columnIndex] = matrixForTransposition.Matrix[columnIndex, rowIndex];
                }
            }

            return result;
        }

        public static int Trace(this SquareMatrix Tracematrix)
        {
            int trace = 0;

            for (int rowIndex = 0; rowIndex < Tracematrix.Size; ++rowIndex)
            {
                trace += Tracematrix.Matrix[rowIndex, rowIndex];
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
