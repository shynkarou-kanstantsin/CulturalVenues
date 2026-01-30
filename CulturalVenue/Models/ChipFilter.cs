using CommunityToolkit.Mvvm.ComponentModel;

namespace CulturalVenue.Models
{
    public partial class ChipFilter : ObservableObject
    {
        public string Name { get; set; }
        public string ImageSource { get; set; }

        [ObservableProperty]
        private bool isSelected;
    }
}
