using MinimalisticWPF.MessageFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Message
{
    public interface IMessageFlow
    {
        public event MessageFlowHandler? MessageFlowRecieved;
        public void SendMessageFlow(string name, params object?[] messages);
        public void RecieveMessageFlow(object sender, MessageFlowArgs e);
    }
}
