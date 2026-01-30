using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using CulturalVenue.Models;
using CommunityToolkit.Mvvm;

namespace CulturalVenue.ViewModels
{
    [QueryProperty(nameof(CurrentEvent), "Event")]
    public partial class EventPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private Event currentEvent;
    }
}
