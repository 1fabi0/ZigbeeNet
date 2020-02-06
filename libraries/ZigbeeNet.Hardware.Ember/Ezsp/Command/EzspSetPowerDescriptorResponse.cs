//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:3.0.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZigBeeNet.Hardware.Ember.Ezsp.Command
{
    using ZigBeeNet.Hardware.Ember.Internal.Serializer;
    
    
    /// <summary>
    /// Class to implement the Ember EZSP command " setPowerDescriptor ".
    /// Sets the power descriptor to the specified value. The power descriptor is a dynamic value,
    /// therefore you should call this function whenever the value changes.
    /// This class provides methods for processing EZSP commands.
    /// </summary>
    public class EzspSetPowerDescriptorResponse : EzspFrameResponse
    {
        
        public const int FRAME_ID = 22;
        
        public EzspSetPowerDescriptorResponse(int[] inputBuffer) : 
                base(inputBuffer)
        {
        }
        
        public override string ToString()
        {
            return base.ToString();
        }
    }
}