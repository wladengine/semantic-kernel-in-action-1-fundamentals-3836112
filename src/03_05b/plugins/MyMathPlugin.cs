using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace _03_05b;

public class MyMathPlugin
{
    [KernelFunction, Description("Take a square root of number")]
    public static double Sqrt([Description("The number to take a square root of")] double number) => Math.Sqrt(number);
}