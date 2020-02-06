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
    using ZigBeeNet.Hardware.Ember.Ezsp.Structure;
    
    
    /// <summary>
    /// Class to implement the Ember EZSP command " getLibraryStatus ".
    /// This retrieves the status of the passed library ID to determine if it is compiled into the
    /// stack.
    /// This class provides methods for processing EZSP commands.
    /// </summary>
    public class EzspGetLibraryStatusRequest : EzspFrameRequest
    {
        
        public const int FRAME_ID = 1;
        
        /// <summary>
        ///  The ID of the library being queried.
        /// </summary>
        private EmberLibraryId _libraryId;
        
        private EzspSerializer _serializer;
        
        public EzspGetLibraryStatusRequest()
        {
            _frameId = FRAME_ID;
            _serializer = new EzspSerializer();
        }
        
        /// <summary>
        /// The libraryId to set as <see cref="EmberLibraryId"/> </summary>
        public void SetLibraryId(EmberLibraryId libraryId)
        {
            _libraryId = libraryId;
        }
        
        /// <summary>
        ///  The ID of the library being queried.
        /// Return the libraryId as <see cref="EmberLibraryId"/>
        /// </summary>
        public EmberLibraryId GetLibraryId()
        {
            return _libraryId;
        }
        
        /// <summary>
        /// Method for serializing the command fields </summary>
        public override int[] Serialize()
        {
            SerializeHeader(_serializer);
            _serializer.SerializeEmberLibraryId(_libraryId);
            return _serializer.GetPayload();
        }
        
        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append("EzspGetLibraryStatusRequest [libraryId=");
            builder.Append(_libraryId);
            builder.Append(']');
            return builder.ToString();
        }
    }
}