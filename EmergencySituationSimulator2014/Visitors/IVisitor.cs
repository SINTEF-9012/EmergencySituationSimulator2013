﻿using EmergencySituationSimulator2013.Model;

namespace EmergencySituationSimulator2013.Visitors
{
    public interface IVisitor
    {
        void Visit(Entity e);
        void Visit(WheeledVehicle v);
        void Visit(Patient v);
        void Visit(Zombie z);
    }
}