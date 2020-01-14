using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apache.WorkFlow
{
    public class CustomTrackingParticipant : TrackingParticipant
    {
        //TODO: Fine tune the profile with the correct query.
        public IDictionary<String, object> Outputs { get; set; }
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            if (record != null)
            {
                if (record is CustomTrackingRecord)
                {
                    var customTrackingRecord = record as CustomTrackingRecord;
                    Outputs = customTrackingRecord.Data;
                }
            }
        }
    }
}