# NavyRadar

Desktop application for navy radar

Kelompok Mesin Rumput S8000

Ketua Kelompok: Hilmi Musyaffa - 23/516589/TK/56795

Anggota 1: Christian Kevin Andhika Danidaiva - 23/513576/TK/56433

Anggota 2: Harun - 23/514148/TK/56466

Anggota 3: Hilmi Musyaffa - 23/516589/TK/56795

## Class Diagram

```mermaid
classDiagram
    %% --- Model Classes ---
    class Account {
        +string Id
        +string Username
        +string Password
        +string Email
        +string Role
    }

    class AccountCaptain {
        +List~string~ AssignedShipIds
        +string CurrentShipId
    }

    class Position {
        +double Latitude
        +double Longitude
        +DateTime Timestamp
    }

    class Ship {
        +string Id
        +string Name
        +ShipType Type
        +int YearBuild
        +int LengthOverall
        +int GrossTonnage
        +string Destination
        +string Origin
        +double Speed
        +double Heading
        +Position CurrentPosition
        +ShipStatus Status
        +DateTime LastUpdate
        +DateTime EstimatedArrival
    }

    class TrackingData {
        +string ShipId
        +List~Position~ PositionHistory
        +DateTime StartTime
        +DateTime EndTime
        +double TotalDistance
        +double AverageSpeed
        +double MaxSpeed
    }

    class ShipType {
        <<enumeration>>
        CargoVessels
        Tankers
        PassengerVessels
    }

    class ShipStatus {
        <<enumeration>>
        Underway
        AtAnchor
        Moored
    }

    class AccountService {
        +Register(username, password, email, isAdmin) Account?
        +Login(username, password) Account?
        +GetUser(id) Account?
        +ListAllUser() List~Account~?
        +UpdateAccount(id, account) Account?
        +RemoveUser(id) Account?
    }

    class CaptainService {
        +AddCaptain(id) Account?
        +GetCaptain(id) AccountCaptain?
        +ListAllCaptain() List~AccountCaptain~?
        +UpdateCaptain(id, captain) Account?
        +RemoveCaptain(id) Account?
    }

    class ShipService {
        +GetShip(id) Ship?
        +ListAllShips() List~Ship~?
        +AddShip(ship) Ship
        +UpdateShipPosition(id, position) Ship?
        +UpdateShipStatus(id, status) Ship?
        +UpdateDestination(id, destination) Ship?
        +RemoveShip(id) Ship?
    }

    class TrackingDataService {
        +AddTracking(trackingData) TrackingData?
        +GetTracking(id) TrackingData?
        +ListAllTrackings() List~TrackingData~?
    }


    Account <|-- AccountCaptain
    Ship "1" *-- "1" Position : has
    Ship "1" -- "1" ShipType
    Ship "1" -- "1" ShipStatus
    TrackingData "1" *-- "0..*" Position : has history
    TrackingData "1" -- "1" Ship : tracks
    AccountCaptain "1" -- "0..*" Ship : manages

    AccountService ..> Account : uses
    CaptainService ..> Account : uses
    CaptainService ..> AccountCaptain : uses
    ShipService ..> Ship : uses
    ShipService ..> Position : uses
    ShipService ..> ShipStatus : uses
    TrackingDataService ..> TrackingData : uses
```

## ER Diagram


<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/a88207f2-8a23-4f35-adfe-5d5622b0ac0d">
  <img src="https://github.com/user-attachments/assets/78d33496-6032-44c2-98f5-15d99b40136c">
</picture>
