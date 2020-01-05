using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dcmFoobar2000.Code
{
    public class VisualizationDataEventArgs : EventArgs
    {
        public int ChannelCount { get; private set; }
        public float[] Samples { get; private set; }

        public VisualizationDataEventArgs(int channelCount, float[] samples)
        {
            ChannelCount = channelCount;
            Samples = samples;
        }
    }
}
