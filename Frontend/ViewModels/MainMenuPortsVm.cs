using SharedModels.Models;
using SharedModels.Util;

namespace Frontend.ViewModels;

public class MainMenuPortsVm
{
    public List<Port>? Ports { get; set; }

    public MainMenuPortsVm()
    {
        LoadDummyData();
    }

    private void LoadDummyData()
    {
        Ports =
        [
            new Port
            {
                Id = 1,
                Name = "Port of Singapore",
                CountryCode = "SG",
                Location = new Point { Latitude = 1.2644, Longitude = 103.8401 }
            },

            new Port
            {
                Id = 2,
                Name = "Port of Rotterdam",
                CountryCode = "NL",
                Location = new Point { Latitude = 51.9470, Longitude = 4.1399 }
            },

            new Port
            {
                Id = 3,
                Name = "Port of Los Angeles",
                CountryCode = "US",
                Location = new Point { Latitude = 33.7292, Longitude = -118.2620 }
            }
        ];
    }
}