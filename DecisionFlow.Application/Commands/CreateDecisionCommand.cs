using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Application.Commands
{
    public record CreateDecisionCommand(string Title, string Description);
}
