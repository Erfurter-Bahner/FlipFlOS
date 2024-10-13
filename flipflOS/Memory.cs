using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flipflOS
{
    class Memory
    {
        Cosmos.Core.ManagedMemoryBlock newBlock = new Cosmos.Core.ManagedMemoryBlock(32);
        public void writeAt(uint index, byte value)
        {
            newBlock.Write16(index, value);
        }
        public ushort readAt(uint index)
        {
            ushort retval = 0;
            retval = newBlock.Read16(index*2);
            return retval;
        }
    }
}
