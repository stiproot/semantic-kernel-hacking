using System.ComponentModel;
using System.Globalization;
using Microsoft.SemanticKernel;

namespace Plugins.MathPlugin;

public class Math
{
    [KernelFunction, Description("Take the square root of a number")]
    public double Sqrt([Description("The number to take a square root of")] double input)
    {
        return System.Math.Sqrt(input);
    }

    [KernelFunction, Description("Multiply two numbers")]
    public static double Multiply(
        [Description("The first number to multiply")] double input,
        [Description("The second number to multiply")] double number2
    )
    {
        return input * number2;
    }
}