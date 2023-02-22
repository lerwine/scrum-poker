using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    [DataContract]
    public class SprintGroupingResponse
    {
        private string _title = "";
        [DataMember(Name = "title", IsRequired = true)]
        public string Title
        {
            get { return _title; }
            set { _title = value.EmptyIfNullOrTrimmed(); }
        }

        private string _description;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }


        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set { _startDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "startDate", EmitDefaultValue = false)]
        private string __StartDate
        {
            get { return _startDate.ToJsonDateString(); }
            set { _startDate = value.JsonStringToDate().ToLocalDate(); }
        }

        private DateTime? _plannedEndDate;
        public DateTime? PlannedEndDate
        {
            get { return _plannedEndDate; }
            set { _plannedEndDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "plannedEndDate", EmitDefaultValue = false)]
        private string __PlannedEndDate
        {
            get { return _plannedEndDate.ToJsonDateString(); }
            set { _plannedEndDate = value.JsonStringToDate().ToLocalDate(); }
        }

        public override string ToString() { return this.ToJSON<SprintGroupingResponse>(); }
    }
}
