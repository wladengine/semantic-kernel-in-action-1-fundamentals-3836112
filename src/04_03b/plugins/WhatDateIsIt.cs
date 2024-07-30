using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace _04_03b;

public class WhatDateIsIt
{
    [KernelFunction, Description("Get the current date")]
    public string Date(IFormatProvider? formatProvider = null) =>
            DateTimeOffset.UtcNow.ToString("D", formatProvider);
}