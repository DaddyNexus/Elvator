using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace ElevatorPlugin
{
    public class ElevatorPluginConfiguration : IRocketPluginConfiguration
    {
        public ushort UIEffectID { get; set; }
        public string CloseButtonName { get; set; }

        [XmlArray("Elevators")]
        [XmlArrayItem("Elevator")]
        public List<Elevator> Elevators { get; set; }

        public void LoadDefaults()
        {
            UIEffectID = 22008;
            CloseButtonName = "Elevator_Close";
            Elevators = new List<Elevator>
            {
                new Elevator
                {
                    Name = "MainLobbyElevator",
                    Position = new SerializableVector3(100, 50, -20),
                    Radius = 5f,
                    UseZoneTrigger = true,
                    TriggerItemID = 0,
                    Floors = new List<Floor>
                    {
                        new Floor { ButtonName = "Floor_1", DisplayName = "Lobby", Destination = new SerializableVector3(100, 50, -20) },
                        new Floor { ButtonName = "Floor_2", DisplayName = "Apartments", Destination = new SerializableVector3(100, 70, -20) },
                        new Floor { ButtonName = "Floor_3", DisplayName = "Rooftop", Destination = new SerializableVector3(100, 90, -20) }
                    }
                },
                new Elevator
                {
                    Name = "SecretBunkerElevator",
                    Position = new SerializableVector3(-45, 20, 150),
                    Radius = 2f,
                    UseZoneTrigger = false,
                    TriggerItemID = 328,
                    Floors = new List<Floor>
                    {
                        new Floor { ButtonName = "Floor_1", DisplayName = "Surface", Destination = new SerializableVector3(-45, 20, 150) },
                        new Floor { ButtonName = "Floor_2", DisplayName = "Bunker Level 1", Destination = new SerializableVector3(-45, -10, 150) }
                    }
                }
            };
        }
    }

    public class Elevator
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public SerializableVector3 Position { get; set; }
        public float Radius { get; set; }
        public bool UseZoneTrigger { get; set; }
        public ushort TriggerItemID { get; set; }

        [XmlArray("Floors")]
        [XmlArrayItem("Floor")]
        public List<Floor> Floors { get; set; }
    }

    public class Floor
    {
        public string ButtonName { get; set; }
        public string DisplayName { get; set; }
        public SerializableVector3 Destination { get; set; }
    }

    public class SerializableVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public SerializableVector3() { }

        public SerializableVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 ToVector3() => new Vector3(X, Y, Z);

        public static implicit operator Vector3(SerializableVector3 v) => new Vector3(v.X, v.Y, v.Z);
        public static implicit operator SerializableVector3(Vector3 v) => new SerializableVector3(v.x, v.y, v.z);
    }
}