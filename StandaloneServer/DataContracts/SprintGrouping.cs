using System;
using System.Runtime.Serialization;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public class SprintGrouping : TitleAndIdentifier
    {
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
            set { _startDate = value; }
        }

        [DataMember(Name = "startDate", EmitDefaultValue = false)]
        private string __StartDate
        {
            get { return _startDate.ToJsonString(); }
            set { _startDate = value.JsonStringToDate(); }
        }

        private DateTime? _plannedEndDate;
        public DateTime? PlannedEndDate
        {
            get { return _plannedEndDate; }
            set { _plannedEndDate = value; }
        }

        [DataMember(Name = "plannedEndDate", EmitDefaultValue = false)]
        private string __PlannedEndDate
        {
            get { return _plannedEndDate.ToJsonString(); }
            set { _plannedEndDate = value.JsonStringToDate(); }
        }

        public override string ToString() { return this.ToJSON<SprintGrouping>(); }

        public static SprintGrouping FromJSON(string jsonText) { return jsonText.FromJSON<SprintGrouping>(); }
    }
}
