using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public class SprintGrouping : TitleAndIdentifier, ScrumPoker.DataContracts.ISprintGrouping
    {
        private static readonly PropertyDescriptor _pdDescription;
        private string _description;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get { return _description; }
            set
            {
                if (value.ToTrimmedOrNullIfEmpty(SyncRoot, ref _description))
                    RaisePropertyChanged(_pdDescription);
            }
        }

        private static readonly PropertyDescriptor _pdStartDate;
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                if (value.ToLocalDate(SyncRoot, ref _startDate))
                    RaisePropertyChanged(_pdStartDate);
            }
        }

        [DataMember(Name = "startDate", EmitDefaultValue = false)]
        private string __StartDate
        {
            get { return _startDate.ToJsonDateString(); }
            set
            {
                if (value.JsonStringToDate().ToLocalDate(SyncRoot, ref _startDate))
                    RaisePropertyChanged(_pdStartDate);
            }
        }

        private static readonly PropertyDescriptor _pdPlannedEndDate;
        private DateTime? _plannedEndDate;
        public DateTime? PlannedEndDate
        {
            get { return _plannedEndDate; }
            set
            {
                if (value.ToLocalDate(SyncRoot, ref _plannedEndDate))
                    RaisePropertyChanged(_pdPlannedEndDate);
            }
        }

        [DataMember(Name = "plannedEndDate", EmitDefaultValue = false)]
        private string __PlannedEndDate
        {
            get { return _plannedEndDate.ToJsonDateString(); }
            set
            {
                if (value.JsonStringToDate().ToLocalDate(SyncRoot, ref _plannedEndDate))
                    RaisePropertyChanged(_pdPlannedEndDate);
            }
        }

        static SprintGrouping()
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(SprintGrouping));
            _pdDescription = pdc["Description"];
            _pdStartDate = pdc["StartDate"];
            _pdPlannedEndDate = pdc["PlannedEndDate"];
        }

        public override string ToString() { return this.ToJSON<SprintGrouping>(); }

        public static SprintGrouping FromJSON(string jsonText) { return jsonText.FromJSON<SprintGrouping>(); }
    }
}
