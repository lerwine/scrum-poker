using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Threading;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public class ScrumSession : TitleAndIdentifier
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

        private static readonly PropertyDescriptor _pdInitiative;
        private SprintGrouping _initiative;
        [DataMember(Name = "initiative", EmitDefaultValue = false)]
        public SprintGrouping Initiative
        {
            get { return _initiative; }
            set
            {
                if (value.SetIfDifferentObject(SyncRoot, ref _initiative))
                    RaisePropertyChanged(_pdInitiative);
            }
        }

        private static readonly PropertyDescriptor _pdEpic;
        private SprintGrouping _epic;
        [DataMember(Name = "epic", EmitDefaultValue = false)]
        public SprintGrouping Epic
        {
            get { return _epic; }
            set
            {
                if (value.SetIfDifferentObject(SyncRoot, ref _epic))
                    RaisePropertyChanged(_pdEpic);
            }
        }

        private static readonly PropertyDescriptor _pdMilestone;
        private SprintGrouping _milestone;
        [DataMember(Name = "milestone", EmitDefaultValue = false)]
        public SprintGrouping Milestone
        {
            get { return _milestone; }
            set
            {
                if (value.SetIfDifferentObject(SyncRoot, ref _milestone))
                    RaisePropertyChanged(_pdMilestone);
            }
        }

        private static readonly PropertyDescriptor _pdPlannedStartDate;
        private DateTime? _plannedStartDate;
        public DateTime? PlannedStartDate
        {
            get { return _plannedStartDate; }
            set
            {
                if (value.ToLocalDate(SyncRoot, ref _plannedStartDate))
                    RaisePropertyChanged(_pdPlannedStartDate);
            }
        }

        [DataMember(Name = "plannedStartDate", EmitDefaultValue = false)]
#pragma warning disable IDE1006, IDE0051
        private string __PlannedStartDate
#pragma warning restore IDE1006, IDE0051
        {
            get { return _plannedStartDate.ToJsonDateString(); }
            set
            {
                if (value.JsonStringToDate().ToLocalDate(SyncRoot, ref _plannedStartDate))
                    RaisePropertyChanged(_pdPlannedStartDate);
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
#pragma warning disable IDE1006, IDE0051
        private string __PlannedEndDate
#pragma warning restore IDE1006, IDE0051
        {
            get { return _plannedEndDate.ToJsonDateString(); }
            set
            {
                if (value.JsonStringToDate().ToLocalDate(SyncRoot, ref _plannedEndDate))
                    RaisePropertyChanged(_pdPlannedEndDate);
            }
        }

        private static readonly PropertyDescriptor _pdurrentScopePoints;
        private int _currentScopePoints = 0;
        [DataMember(Name = "currentScopePoints", IsRequired = true)]
        [Range(0, int.MaxValue)]
        public int CurrentScopePoints
        {
            get { return _currentScopePoints; }
            set
            {
                if (value.SetIfDifferent(SyncRoot, ref _currentScopePoints))
                    RaisePropertyChanged(_pdurrentScopePoints);
            }
        }

        private static readonly PropertyDescriptor _pdSprintCapacity;
        private int? _sprintCapacity;
        [DataMember(Name = "sprintCapacity", EmitDefaultValue = false)]
        [Range(0, int.MaxValue)]
        public int? SprintCapacity
        {
            get { return _sprintCapacity; }
            set
            {
                if (value.SetIfDifferent(SyncRoot, ref _sprintCapacity))
                    RaisePropertyChanged(_pdSprintCapacity);
            }
        }

        private static readonly PropertyDescriptor _pdDeckId;
        private Guid _deckId = Guid.Empty;
        public Guid DeckId
        {
            get { return _deckId; }
            set
            {
                if (value.SetIfDifferent(SyncRoot, ref _deckId))
                    RaisePropertyChanged(_pdDeckId);
            }
        }

        [DataMember(Name = "deckId", IsRequired = true)]
#pragma warning disable IDE1006, IDE0051
        private string __DeckId
#pragma warning restore IDE1006, IDE0051
        {
            get { return _deckId.ToJsonString(); }
            set
            {
                if ((value.JsonStringToGuid() ?? Guid.Empty).SetIfDifferent(SyncRoot, ref _deckId))
                    RaisePropertyChanged(_pdDeckId);
            }
        }

        private static readonly PropertyDescriptor _pdOrganizerId;
        private Guid _organizerId = Guid.Empty;
        public Guid OrganizerId
        {
            get { return _organizerId; }
            set
            {
                if (value.SetIfDifferent(SyncRoot, ref _organizerId))
                    RaisePropertyChanged(_pdOrganizerId);
            }
        }

        [DataMember(Name = "organizerId", IsRequired = true)]
#pragma warning disable IDE1006, IDE0051
        private string __OrganizerId
#pragma warning restore IDE1006, IDE0051
        {
            get { return _organizerId.ToJsonString(); }
            set
            {
                if ((value.JsonStringToGuid() ?? Guid.Empty).SetIfDifferent(SyncRoot, ref _organizerId))
                    RaisePropertyChanged(_pdOrganizerId);
            }
        }

        static ScrumSession()
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(ScrumSession));
            _pdDescription = pdc["Description"];
            _pdInitiative = pdc["Initiative"];
            _pdEpic = pdc["Epic"];
            _pdMilestone = pdc["Milestone"];
            _pdPlannedStartDate = pdc["PlannedStartDate"];
            _pdPlannedEndDate = pdc["PlannedEndDate"];
            _pdurrentScopePoints = pdc["CurrentScopePoints"];
            _pdSprintCapacity = pdc["SprintCapacity"];
            _pdDeckId = pdc["DeckId"];
            _pdOrganizerId = pdc["Organizer"];
        }

        public override string ToString() { return this.ToJSON<ScrumSession>(); }

        public static ScrumSession FromJSON(string jsonText) { return jsonText.FromJSON<ScrumSession>(); }
    }
}
