namespace Calculator;

public class Operation : IOperation
{
    public int Add(int a, int b) => a + b;

    public int Subtract(int a, int b) => a - b;

    public int Multiply(int a, int b) => a * b;

    public double Divide(int a, int b)
    {
        if (b == 0)
            throw new DivideByZeroException("Division by zero is not allowed");
        return (double)a / b;
    }

    public int Power(int a, int b)
    {
        if (b < 0)
            throw new ArgumentOutOfRangeException(nameof(b), "Exponent cannot be negative");

        if (b == 0)
            return 1;

        int result = 1;
        for (int i = 0; i < b; i++)
        {
            result *= a;
        }
        return result;
    }

    public int Square(int a) => a * a;

    public int Cube(int a) => a * a * a;

    public int Factorial(int a)
    {
        if (a < 0)
            throw new ArgumentOutOfRangeException(nameof(a), "Factorial cannot be calculated for negative numbers");

        if (a == 0 || a == 1)
            return 1;

        int result = 1;
        for (int i = 2; i <= a; i++)
        {
            result *= i;
        }
        return result;
    }

    public int SquareRoot(int a)
    {
        if (a < 0)
            throw new ArgumentOutOfRangeException(nameof(a), "Square root cannot be calculated for negative numbers");

        if (a == 0 || a == 1)
            return a;

        return (int)Math.Sqrt(a);
    }

    public int CubeRoot(int a)
    {
        if (a == 0)
            return 0;

        if (a > 0)
            return (int)Math.Round(Math.Pow(a, 1.0 / 3.0));
        else
            return -(int)Math.Round(Math.Pow(-a, 1.0 / 3.0));
    }

    public bool IsEven(int number) => number % 2 == 0;
}