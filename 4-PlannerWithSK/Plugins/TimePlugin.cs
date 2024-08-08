using System;
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace PlannerWithSK.Plugins;

public class TimePlugin
{
    [KernelFunction]
    [Description("Retrieves the current time in UTC.")]
    public string GetCurrentUtcTime() => DateTime.UtcNow.ToString("R");
}

