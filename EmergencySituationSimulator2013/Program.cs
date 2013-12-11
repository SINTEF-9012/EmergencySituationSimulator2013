﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Threading;
using EmergencySituationSimulator2013.HereAPI;
using EmergencySituationSimulator2013.Model;
using EmergencySituationSimulator2013.Visitors;

namespace EmergencySituationSimulator2013
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("BridgeSimpleSimulator");

            // Load settings
            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            // Setting up the Here API
            HereConfig.AppId = appSettings["HereAppId"];
            HereConfig.AppCode = appSettings["HereAppCode"];

            // Retreive the GPS position for the address
            var placeInfos = new HereGeocoder(appSettings["place"]);


            // Setting up the Oracle
            Oracle.Generator = Random.fromSeed(appSettings["seed"]);
            Oracle.Center = placeInfos.Location;
            double.TryParse(appSettings["areaRadius"], out Oracle.AreaRadius);
            

            /*for (var i = 0; i < 10; ++i)
            {
                new PoliceCar();
            }
            new FireTruck();
            new Patient();
            new Zombie();*/

            


            var transmission = new Transmission(appSettings["connection"], appSettings["senderID"]);
            

            var center = Oracle.CreateLocation();

            for (var i = 0; i < 1; ++i)
            {
                var fireTruck = new FireTruck();
                var path = new HereRoute(Oracle.CreateLocation(), center);

                var fireTruckPilot = new LocationPilot(fireTruck, path.Route, path.Speeds)
                    {
                        MaxSpeed = 300.5
                    };


                var timer = new Timer(delegate
                {
                    if (!fireTruckPilot.AtDestination)
                    {
                        fireTruckPilot.Tick(0.08);
                        Console.WriteLine(fireTruck.Location + " - " + fireTruckPilot.CurrentSpeed);
                    }

                }, null, 0, 300);
            }

            var masterVisitor = new MasterVisitor();
            var textDebugVisitor = new TextDebugVisitor();

            var lapin = new Timer(delegate
                {
                    masterVisitor.StartTransaction();
                    Entity.VisitAll(masterVisitor);
                    transmission.Send(masterVisitor.Transaction);
                    Console.WriteLine("sent :-)");
                    Entity.VisitAll(textDebugVisitor);
                }, null, 0, 300);
                
            
            Thread.Sleep(Timeout.Infinite);
            return;
            // Load settings
            /*NameValueCollection appSettings = ConfigurationManager.AppSettings;

            int areaRadius, patientsNumber, patientsGroupsNumber, resourcesNumber, resourcesGroupsNumber;

            int.TryParse(appSettings["areaRadius"], out areaRadius);
            int.TryParse(appSettings["patientsNumber"], out patientsNumber);
            int.TryParse(appSettings["patientsGroupsNumber"], out patientsGroupsNumber);
            int.TryParse(appSettings["resourcesNumber"], out resourcesNumber);
            int.TryParse(appSettings["resourcesGroupsNumber"], out resourcesGroupsNumber);

            Location center = Location.fromString(appSettings["locationCenter"]);

            Random random = Random.fromSeed(appSettings["seed"]);

            double patientsSpeedRatio, resourcesSpeedRatio, speedInsideGroup;
            double.TryParse(appSettings["patientsSpeedRaio"], NumberStyles.Any, CultureInfo.InvariantCulture,
                out patientsSpeedRatio);
            double.TryParse(appSettings["resourcesSpeedRaio"], NumberStyles.Any, CultureInfo.InvariantCulture,
                out resourcesSpeedRatio);
            double.TryParse(appSettings["speedInsideGroup"], NumberStyles.Any, CultureInfo.InvariantCulture,
                out speedInsideGroup);

            // Create entities

            var patients = new List<OLdEntity>(patientsNumber);
            //var resources = new List<OLdEntity>(resourcesNumber);
            var groups = new List<Group>(patientsGroupsNumber + resourcesGroupsNumber);

            Group currentGroup = null;
            Location currentLocation = null;
            int nbGroup = 0;

            for (int i = 0; i < patientsNumber; ++i)
            {
                if (currentGroup == null ||
                    (nbGroup < patientsGroupsNumber && random.Next(patientsNumber/patientsGroupsNumber) == 1))
                {
                    currentGroup = new Group(patientsSpeedRatio);
                    currentLocation = new Location()
                    {
                        lat = center.lat,
                        lng = center.lng
                    };

                    currentLocation.Move(random.NextDouble()*360.0, random.NextGaussianDouble(areaRadius/2, areaRadius/4));

                    groups.Add(currentGroup);
                }

                var patient = new OLdEntity(Math.Abs(random.NextGaussianDouble(speedInsideGroup, speedInsideGroup)));

                // Place the patient on the area
                patient.location.lat = currentLocation.lat;

                patient.location.lng = currentLocation.lng;
                patient.location.Move(random.NextDouble()*360.0, random.NextGaussianDouble(currentGroup.entities.Count*2+5, currentGroup.entities.Count+10));

                currentGroup.AddEntity(patient);
                patients.Add(patient);
            }

            // Start the communication system
            var transmission = new Transmission(appSettings["connection"], appSettings["senderID"]);
            transmission.init(patients);

            // Start the main loop

            var timer = new Timer(delegate
            {
                foreach (var group in groups)
                {
                    group.Move(random, 0.08);
                }
                foreach (OLdEntity patient in patients)
                {
                    patient.Move(random, 0.08);
                }

                transmission.update(patients);
            }, null, 0, 300);

            

            // Commit when the user type enter
            do
            {
                
                transmission.update(patients);
            } while (!Console.ReadLine().Contains("exit"));*/
        }
    }
}