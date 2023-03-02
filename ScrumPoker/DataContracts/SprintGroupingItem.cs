using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    /// <summary>
    /// Base type representing an initiative, epic or milestone.
    /// </summary>
    [DataContract]
    public class SprintGroupingItem
    {
        private string _title = "";
        /// <summary>
        /// The title of the grouping item.
        /// </summary>
        [DataMember(Name = "title", IsRequired = true)]
        public string Title
        {
            get { return _title; }
            set { _title = value.WsNormalized(); }
        }

        private string _description;
        /// <summary>
        /// The optional description.
        /// </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }

        private DateTime? _startDate;
        /// <summary>
        /// The optional planned start date.
        /// </summary>
        public DateTime? StartDate
        {
            get { return _startDate; }
            set { _startDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "startDate", EmitDefaultValue = false)]
#pragma warning disable IDE0051, IDE1006
        private string __StartDate
#pragma warning restore IDE0051, IDE1006
        {
            get { return _startDate.ToJsonDateString(); }
            set { _startDate = value.JsonStringToDate().ToLocalDate(); }
        }

        private DateTime? _plannedEndDate;
        /// <summary>
        /// The optional planned end date.
        /// </summary>
        public DateTime? PlannedEndDate
        {
            get { return _plannedEndDate; }
            set { _plannedEndDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "plannedEndDate", EmitDefaultValue = false)]
#pragma warning disable IDE0051, IDE1006
        private string __PlannedEndDate
#pragma warning restore IDE0051, IDE1006
        {
            get { return _plannedEndDate.ToJsonDateString(); }
            set { _plannedEndDate = value.JsonStringToDate().ToLocalDate(); }
        }

        public override string ToString() { return this.ToJSON<SprintGroupingItem>(); }
    }
}
